using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yuck
{
    class MatrixRoom
    {
        public string roomID { get; set; }
        public string roomNameHumanReadable { get; set; }

        public override string ToString() 
        {
            return roomNameHumanReadable == null ? roomID : roomNameHumanReadable;
        }

    }
}
