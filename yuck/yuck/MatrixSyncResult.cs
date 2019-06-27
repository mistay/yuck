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

        public SyncResultPresence presence { get; set; }

        public SyncResultAccountData account_data { get; set; }

    }

    public class SyncResultAccountData
    {
        public List<SyncResultsAccountDataEvents> events { get; set; }
    }
    public class SyncResultsAccountDataEvents
    {
        public string type { get; set; }
        public object content { get; set; }

    }

    public class SyncResultPresence
    {
        public List<MatrixSyncResultPresenceEvents> events { get; set; }
    }
    public class MatrixSyncResultPresenceEvents
    {
        public string type { get; set; }
        public string sender { get; set; }
        public SyncResultPresenceContent content { get; set; }

    }

    public class SyncResultPresenceContent
    {
        public string presence { get; set; }
        public long last_active_ago { get; set; }
        public bool currenty_active { get; set; }

    }
    public class SyncResultRooms
    {
        public Dictionary<string, MatrixSyncResultTimelineWrapper> join { get; set; }
    }

    public class MatrixSyncResultTimelineWrapper
    {
        public MatrixSyncResultTimeline timeline { get; set; }
        public MatrixSyncResultEphemeral ephemeral { get; set; }
    }

    public class MatrixSyncResultTimeline
    {
        public List<MatrixSyncResultEvents> events { get; set; }
    }

    public class MatrixSyncResultEphemeral
    {
        public List<MatrixSyncResultEphemeralEvents> events { get; set; }
    }

    public class MatrixSyncResultEphemeralEvents
    {
        public string type { get; set; }
        public MatrixSyncResultEphemeralContent content { get; set; }
    }
    public class MatrixSyncResultEvents
    {
        public string type { get; set; }
        public string sender { get; set; }
        public MatrixSyncResultContent content { get; set; }
        public string event_id { get; set; }
        public long origin_server_ts { get; set; }

    }
    
    public class MatrixSyncResultEphemeralContent
    {
        public List<string> user_ids { get; set; }

    }
    public class MatrixSyncResultContent
    {
        public string msgtype { get; set; }
        public string body { get; set; }
        public string format { get; set; }
        public string formatted_body { get; set; }
        public string algorithm { get; set; }
        public string sender_key { get; set; }
        public string ciphertext { get; set; }
        public string session_id { get; set; }
        public string device_id { get; set; }
        public string url { get; set; }
    }
}
