using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yuck
{
    internal class ChatMessageImage : ChatMessage
    {
        public string Url {get; set;}
        public ChatMessageImage(string roomID, string sender, string message, string url) : base(roomID, sender, message)
        {
            Url = url;
        }
    }
}
