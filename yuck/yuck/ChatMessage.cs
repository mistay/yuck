using System;
using System.Drawing;

namespace yuck
{
    public class ChatMessage
    {
        public string Message { get; set; }
        public string Sender { get; set; }
        public bool Displayed { get; set; }
        public string RoomID { get; set; }
        public DateTime timeReceived { get; set; }
        public Image Image { get; internal set; }

        private ChatMessage()
        {

        }

        public ChatMessage(string roomID, string sender, string message)
        {
            RoomID = roomID;
            Sender = sender;
            Message = message;
            Displayed = false;
        }
    }
}