﻿using OpenRPA.Interfaces.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace OpenRPA.Net
{
    public class SigninMessage : SocketCommand
    {
        public SigninMessage() : base()
        {
            msg.command = "signin";
        }
        public SigninMessage(string username, SecureString password, string clientversion) : base()
        {
            msg.command = "signin";
            this.clientversion = clientversion;
            this.username = username;
            this.password = new System.Net.NetworkCredential(string.Empty, password).Password;
        }
        public SigninMessage(string jwt, string clientversion) : base()
        {
            msg.command = "signin";
            this.clientversion = clientversion;
            this.jwt = jwt;
        }
        public SigninMessage(SecureString jwt, string clientversion) : base()
        {
            msg.command = "signin";
            this.clientversion = clientversion;
            this.jwt = new System.Net.NetworkCredential(string.Empty, jwt).Password; 
        }
        public bool validate_only { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public TokenUser user { get; set; }
        public int websocket_package_size { get; set; }
        public string clientagent { get; set; } = "openrpa";
        public string clientversion { get; set; } = "0.0.0.1";
    }
}
