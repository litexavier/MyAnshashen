using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascension.Network
{
    [Serializable]
    public class AscensionMessage
    {
        private Object messageValue;

        private MessageCommand command;

        public AscensionMessage(MessageCommand command)
        {
            this.command = command;
        }

        public MessageCommand getMessageCommand()
        {
            return this.command;
        }

        public Object getMessageValue()
        {
            return this.messageValue;
        }

        public void setMessageValue(Object value)
        {
            this.messageValue = value;
        }

        override
        public string ToString()
        {
            return "command : " + command.ToString();
        }
    }
}
