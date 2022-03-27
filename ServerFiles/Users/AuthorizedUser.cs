using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ServerFiles.Users
{
    public class AuthorizedUser
    {
        public Client socket;
        public string Nickname;
        public bool isReady = false;
        public bool isAnswering = false;

        public AuthorizedUser(Client socket, string Nickname)
        {
            this.socket = socket;
            this.Nickname = Nickname;
        }
    }
}
