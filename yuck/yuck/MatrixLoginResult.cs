using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yuck
{
    public class MatrixLoginResult
    {
        public string user_id { get; set; }
        public string access_token { get; set; }
        public string home_server { get; set; }
        public string device_id { get; set; }
    }
}
