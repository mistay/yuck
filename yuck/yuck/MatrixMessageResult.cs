using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yuck
{
    public class MatrixMessagesResult
    {
        public string start { get; set; }
        public string end { get; set; }
        public List<MatrixMessagesChunkResult> chunk { get; set; }
    }


    public class MatrixMessagesChunkResult
    {
        public string type;
        public string room_id;
        public string sender;
        public MatrixMessagesContentResult content;
        public string event_id;
        public string user_id;
        public long age;
    }

    public class MatrixMessagesContentResult
    {
        public string algorithm;
        public string sender_key;
        public string ciphertext;
        public string session_id;
        public string device_id;
    }
}
