using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yuck
{
    public class MatrixMediaRequest
    {
        internal Mediatypes mediaType;

        public static Mediatypes Mediatype { get; internal set; }
        public string roomID { get; set; }
        public string sender { get; internal set; }
        public string filename { get; internal set; }

        public enum Mediatypes
        {
            CHAT_MESSAGE_IMAGE, ROOM_AVATAR
        }
    }
}
