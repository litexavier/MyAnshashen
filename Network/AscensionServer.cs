using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascension.Properties;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections;
using System.Windows.Forms;

namespace Ascension.Network
{
    public delegate void MessageHandler(AscensionMessage mes);

    public enum ServerType
    {
        IS_SERVER = 1,
        IS_CLIENT = 2
    }

    class AscensionServer
    {
        public class WorkerInfo
        {
            //public bool exit;
            //public DateTime lastTime;
            public const int BufferSize = 1024;
            public byte[] buffer;
            public List<byte> data;
            public Socket socket;
            public ManualResetEvent waiter;

            public WorkerInfo(Socket sock)
            {
                //this.exit = false;
                //this.lastTime = DateTime.Now;
                this.buffer = new byte[BufferSize];
                this.data = new List<byte>();
                this.socket = sock;
                waiter = new ManualResetEvent(false);
            }
        }
        //实例
        private static AscensionServer ins = new AscensionServer();

        //常量
        public static int listenPort = Settings.Default.listenPort;
        public static string myselfKey = NetworkHelper.GetIP() + ":" + listenPort;
        public const int maxWorker = 3;
        //heartbeat
        public const int expireTime = 5000;  //ms

        //各种messagecommond对应的操作
        private Hashtable actions;

        //该客户端的类型
        private ServerType serverType;

        //reciever
        private int currentConnected;
        private Hashtable currentConnectedClients;
        private Hashtable workersState;
        private Hashtable ipLists;

        //connector
        private static bool IsConnectionSuccessful = false;
        private static ManualResetEvent TimeoutObject = new ManualResetEvent(false);

        //sender
        private Queue mesQue;
        private Hashtable sendToClients;

        private AscensionServer()
        {
            actions = Hashtable.Synchronized(new Hashtable());

            serverType = ServerType.IS_CLIENT;
            //reciever
            currentConnected = 0;
            workersState = Hashtable.Synchronized(new Hashtable());
            currentConnectedClients = Hashtable.Synchronized(new Hashtable());
            ipLists = Hashtable.Synchronized(new Hashtable());
            //sender
            mesQue = Queue.Synchronized(new System.Collections.Queue());
            sendToClients = Hashtable.Synchronized(new Hashtable());
            //处理内部message
            MessageHandler req = new MessageHandler(handleConnectReq);
            this.addHandler(MessageCommand.CONNECT_REQ, req);
            MessageHandler res = new MessageHandler(handleConnectRes);
            this.addHandler(MessageCommand.CONNECT_RES, res);
        }

        /// <summary>
        /// 单例
        /// </summary>
        /// <returns>实例</returns>
        public static AscensionServer getInstance()
        {
            return ins;
        }

        public void setServerType(ServerType type)
        {
            this.serverType = type;
        }

        public ServerType getServerType()
        {
            return this.serverType;
        }

        /// <summary>
        /// 添加处理事件的handler
        /// </summary>
        /// <param name="command">命令类型</param>
        /// <param name="handler">事件方法</param>
        /// <returns>成功与否，如果已存在处理的事件返回false，即不覆盖</returns>
        public bool addHandler(MessageCommand command, MessageHandler handler)
        {
            if (actions.ContainsKey(command)) return false;
            actions[command] = handler;
            return true;
        }

        /// <summary>
        /// 连接指定的服务器,默认超时时间为5秒
        /// </summary>
        /// <param name="ip">服务器ip</param>
        /// <param name="port">服务器端口</param>
        /// <returns>成功与否</returns>
        public bool connectServer(String ip, int port)
        {
            return connectServer(ip, port, 5000);
        }

        /// <summary>
        /// 连接指定的服务器
        /// </summary>
        /// <param name="ip">服务器ip</param>
        /// <param name="port">服务器端口</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns>成功与否</returns>
        public bool connectServer(String ip, int port, int timeOut)
        {
            ip = ip.ToLower().Trim();
            if (ip == "localhost" || ip == "127.0.0.1")
            {
                ip = NetworkHelper.GetIP();
            }
            string key = ip + ":" + port;
            if (sendToClients.Contains(key))
            {
                MessageBox.Show("already connect!!!! " + key);
                return false;
            }

            TcpClient tcpclient = new TcpClient();

            tcpclient.BeginConnect(ip, port, new AsyncCallback(connectCallBack), tcpclient);

            if (TimeoutObject.WaitOne(timeOut, false))
            {
                if (IsConnectionSuccessful)
                {
                    sendToClients.Add(key, tcpclient.Client);
                    if (this.getServerType() == ServerType.IS_CLIENT)
                    {
                        AscensionMessage connReq = new AscensionMessage(MessageCommand.CONNECT_REQ);
                        Dictionary<string, object> connContent = new Dictionary<string, object>();
                        connContent.Add("ip", NetworkHelper.GetIP());
                        connContent.Add("port", listenPort);
                        connReq.setMessageValue(connContent);
                        this.sendMessage(connReq);
                    }
                    //保存ip与port，用于删除连接
                    if (!ipLists.Contains(ip))
                    {
                        HashSet<int> ports = new HashSet<int>();
                        ipLists.Add(ip, ports);
                    }
                    ((HashSet<int>)ipLists[ip]).Add(port);
                    return true;
                }
            }
            else
            {
                tcpclient.Close();
            }
            MessageBox.Show("fail connect to ip : " + ip + ", port" + port);
            return false;
        }

        /// <summary>
        /// 启动每个客户端的服务器,reciever,sender
        /// </summary>
        public void startServer()
        {
            Thread reciever = new Thread(new ThreadStart(startReciever));
            reciever.IsBackground = true;
            reciever.Start();
            Thread demonSender = new Thread(new ThreadStart(startSender));
            demonSender.IsBackground = true;
            demonSender.Start();
        }

        /// <summary>
        /// 向已连接到自身的客户端广播消息
        /// </summary>
        /// <param name="mes"></param>
        public void sendMessage(AscensionMessage mes)
        {
            mesQue.Enqueue(mes);
        }

        public bool sendMessage(AscensionMessage mes, string ip, int port)
        {
            string key = ip + ":" + port;
            if (!sendToClients.Contains(key)) return false;
            Socket client = (Socket)sendToClients[key];
            byte[] buf = NetworkHelper.Serialize<AscensionMessage>(mes);
            byte[] len = BitConverter.GetBytes(buf.Length);
            byte[] send = new byte[4 + buf.Length];
            for (int i = 0; i < 4; i++) send[i] = len[i];
            for (int i = 0; i < buf.Length; i++) send[4 + i] = buf[i];
            Hashtable state = new Hashtable();
            state.Add("c", client);
            state.Add("res", false);
            ManualResetEvent timeout = new ManualResetEvent(false);
            state.Add("timer", timeout);
            try
            {
                client.BeginSend(send, 0, send.Length, SocketFlags.None, sendEnd, state);

                if (timeout.WaitOne(expireTime, false))
                {
                    bool res = (bool)state["res"];
                    if (res == true)
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        private void connectCallBack(IAsyncResult asyncresult)
        {
            try
            {
                IsConnectionSuccessful = false;
                TcpClient tcpclient = asyncresult.AsyncState as TcpClient;

                if (tcpclient.Client != null)
                {
                    tcpclient.EndConnect(asyncresult);
                    IsConnectionSuccessful = true;
                }
            }
            catch (Exception)
            {
                IsConnectionSuccessful = false;
            }
            finally
            {
                TimeoutObject.Set();
            }
        }

        private void startSender()
        {
            while (true)
            {
                while (mesQue.Count > 0)
                {
                    AscensionMessage cur = (AscensionMessage)mesQue.Dequeue();
                    byte[] buf = NetworkHelper.Serialize<AscensionMessage>(cur);
                    byte[] len = BitConverter.GetBytes(buf.Length);
                    byte[] send = new byte[4 + buf.Length];
                    for (int i = 0; i < 4; i++) send[i] = len[i];
                    for (int i = 0; i < buf.Length; i++) send[4 + i] = buf[i];
                    foreach(DictionaryEntry et in sendToClients)
                    {
                        Socket client = (Socket)et.Value;
                        Hashtable state = new Hashtable();
                        state.Add("c", client);
                        state.Add("res", false);
                        try
                        {
                            client.BeginSend(send, 0, send.Length, SocketFlags.None, sendEnd, state);
                        }
                        catch (Exception) { }
                    }
                }

                Thread.Sleep(10);
            }
        }

        private void sendEnd(IAsyncResult ay)
        {
            Hashtable state = (Hashtable)ay.AsyncState;
            Socket client = (Socket)state["c"];
            if (client != null)
            {
                int size = client.EndSend(ay);
                state["res"] = true;
            }
            if (state.Contains("timer"))
            {
                ManualResetEvent t = (ManualResetEvent)state["timer"];
                t.Set();
            }
        }

        private void startReciever()
        {
            TcpListener listener = new TcpListener(IPAddress.Parse("0.0.0.0"), listenPort);
            listener.Start();
            try
            {
                listener.BeginAcceptSocket(new AsyncCallback(onAccept), listener);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void onAccept(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            Socket client = listener.EndAcceptSocket(ar);
            if (currentConnected < maxWorker)
            {
                currentConnected++;
                Thread worker = new Thread(new ThreadStart(recieverWorker));
                int id = worker.ManagedThreadId;
                workersState.Add(id, new WorkerInfo(client));
                worker.IsBackground = true;
                worker.Start();
            }

            listener.BeginAcceptSocket(new AsyncCallback(onAccept), listener);
        }

        private void recieverWorker()
        {
            WorkerInfo curInfo = (WorkerInfo)workersState[Thread.CurrentThread.ManagedThreadId];
            if (curInfo == null)
            {
                MessageBox.Show("thread:" + Thread.CurrentThread.ManagedThreadId + "exiting. info : " + curInfo.ToString());
                return;
            }

            try
            {
                Socket client = curInfo.socket;
                client.BeginReceive(curInfo.buffer, 0, curInfo.buffer.Length, SocketFlags.None, new AsyncCallback(onWorkerRecieve), curInfo);
                ManualResetEvent wait = curInfo.waiter;
                wait.WaitOne();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            workersState.Remove(Thread.CurrentThread.ManagedThreadId);
        }

        private void onWorkerRecieve(IAsyncResult ar)
        {
            WorkerInfo curInfo = (WorkerInfo)ar.AsyncState;
            Socket client = curInfo.socket;
            try
            {
                int bytes = client.EndReceive(ar);
                for(int i = 0; i < bytes; i++) {
                    curInfo.data.Add(curInfo.buffer[i]);
                }
                while (curInfo.data.Count > 4)
                {
                    byte[] length = curInfo.data.GetRange(0, 4).ToArray();
                    int len = BitConverter.ToInt32(length, 0);
                    if (curInfo.data.Count < len + 4) break;
                    byte[] after = curInfo.data.GetRange(4, len).ToArray();
                    AscensionMessage mes = null;
                    mes = NetworkHelper.Desrialize<AscensionMessage>(mes, after);
                    MessageHandler handler = (MessageHandler)actions[mes.getMessageCommand()];
                    if (null != handler)
                    {
                        handler(mes);
                    }
                    curInfo.data.RemoveRange(0, 4 + len);
                }
                curInfo.waiter.Set();
                client.BeginReceive(curInfo.buffer, 0, curInfo.buffer.Length, SocketFlags.None, new AsyncCallback(onWorkerRecieve), curInfo);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                //多半是断了连接，移除吗？还是支持重连？
                string ip = ((IPEndPoint)client.RemoteEndPoint).Address.ToString();
                if (ip != NetworkHelper.GetIP())
                {
                    if (ipLists.Contains(ip))
                    {
                        HashSet<int> ports = (HashSet<int>)ipLists[ip];
                        foreach (int p in ports)
                        {
                            string key = ip + ":" + p;
                            //sendToClients.Remove(key);
                            //currentConnectedClients.Remove(key);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 内部处理的AscensionMessagem,MessageCommand.CONNECT_REQ
        /// </summary>
        /// <param name="mes">MessageCommand.CONNECT_REQ</param>
        private void handleConnectReq(AscensionMessage mes)
        {
            if (this.serverType == ServerType.IS_CLIENT) return;
            Dictionary<String, Object> data = (Dictionary<String, Object>)mes.getMessageValue();
            string ip = ((string)data["ip"]).Trim();
            int port = (int)data["port"];
            string key = ip + ":" + port;
            if (!currentConnectedClients.Contains(key))
            {
                currentConnectedClients.Add(key, null);
            }
            if (!sendToClients.Contains(key))
            {
                this.connectServer(ip, port);
            }
            AscensionMessage res = new AscensionMessage(MessageCommand.CONNECT_RES);
            res.setMessageValue(currentConnectedClients);
            this.sendMessage(res);
        }

        /// <summary>
        /// 内部处理的AscensionMessagem,MessageCommand.CONNECT_RES
        /// </summary>
        /// <param name="mes">MessageCommand.CONNECT_RES</param>
        private void handleConnectRes(AscensionMessage mes)
        {
            Hashtable serverConnected = (Hashtable)mes.getMessageValue();
            foreach (DictionaryEntry et in serverConnected)
            {
                string key = (string)et.Key;
                if (myselfKey == key) continue;
                if (!sendToClients.Contains(key))
                {
                    string[] parts = key.Split(':');
                    string ip = parts[0];
                    int port = int.Parse(parts[1]);
                    this.connectServer(ip, port);
                }
            }
        }
    }
}
