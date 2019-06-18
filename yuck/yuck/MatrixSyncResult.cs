using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yuck
{


    public class MatrixSyncResult
    {
        public string next_batch { get; set; }

        public SyncResultRooms rooms { get; set; }
    }


    public class SyncResultRooms
    {
        public Dictionary<string, MatrixSyncResultTimelineWrapper> join { get; set; }
    }

    public class MatrixSyncResultTimelineWrapper
    {
        public MatrixSyncResultTimeline timeline { get; set; }
    }

    public class MatrixSyncResultTimeline
    {
        public List<MatrixSyncResultEvents> events { get; set; }
    }

    public class MatrixSyncResultEvents
    {
        public string type { get; set; }
        public string sender { get; set; }
        public MatrixSyncResultContent content { get; set; }
        public string event_id { get; set; }
        public long origin_server_ts { get; set; }

    }
    
    public class MatrixSyncResultContent
    {
        public string algorithm { get; set; }
        public string sender_key { get; set; }
        public string ciphertext { get; set; }
        public string session_id { get; set; }
        public string device_id { get; set; }
    }
}
