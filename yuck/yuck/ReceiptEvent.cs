namespace yuck
{
    public class ReceiptEvent
    {
        public string roomID { get; set; }
        public string user_id { get; set; }
        public long ts { get; set; }

        public ReceiptEvent(string roomID, string user_id, long ts)
        {
            this.roomID = roomID;
            this.user_id = user_id;
            this.ts = ts;
        }
    }
}