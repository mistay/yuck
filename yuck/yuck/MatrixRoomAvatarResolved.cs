using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yuck
{
    public class MatrixRoomAvatarResolvedResult
    {
        public MatrixImageInfo info { get; set; }
        public string url { get; set; }
    }

    public class MatrixImageInfo
    {
        public int h { get; set; }
        public int w { get; set; }
        public int size { get; set; }
        public string mimetype { get; set; }
        public string thumbnail_url { get; set; }
        public MatrixThumbnailInfo thumbnail_info { get; set; }
        //public MatrixEncryptedFile thumbnail_file { get; set; }
    }

    public class MatrixThumbnailInfo
    {
        public int h { get; set; }
        public int w { get; set; }
        public int size { get; set; }
    }
}