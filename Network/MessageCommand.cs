using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascension.Network
{
    public enum MessageCommand
    {
        //HeartBeat test
        HEARTBEAT_REQ = 0,
        HEARTBEAT_RES = 1,

        //Game Begin
        CONNECT_REQ = 10,
        CONNECT_RES = 11,

        //Game ing
        TALK = 100,

        //Game Over
        DISCONNECT_REQ = 1000,
        DISCONNECT_RES = 1001
    }
}
