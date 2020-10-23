﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRPA.Net
{
    public class SocketMessage : BaseMessage
    {
        public SocketMessage() { }
        public SocketMessage(Message msg)
        {
            id = msg.id;
            replyto = msg.replyto;
            command = msg.command;
            data = msg.data;
            count = 1;
            index = 0;
        }
        public SocketMessage(Message msg, string data, int count, int index)
        {
            id = msg.id;
            replyto = msg.replyto;
            command = msg.command;
            this.data = data;
            this.count = count;
            this.index = index;
        }
        public int count { get; set; }
        public int index { get; set; }
        public void Send(WebSocketClient ws)
        {
            ws.PushMessage(this);
        }
    }
}
