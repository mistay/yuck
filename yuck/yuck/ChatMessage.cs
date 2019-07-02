using System;

namespace yuck
{
    public class ChatMessage
    {
        public string Message { get; set; }
        public string Sender { get; set; }
        public bool Displayed { get; set; }
        public string RoomID { get; set; }
        public DateTime timeReceived { get; set; }

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