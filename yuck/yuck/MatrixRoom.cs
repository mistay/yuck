using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yuck
{
    public class MatrixRoom
    {
        public string roomID { get; set; }
        public string roomNameHumanReadable { get; set; }

        public bool directRoom { get; set; } //direct chat, only 2 users in "room"
        public Image avatar { get; internal set; }

        public override string ToString() 
        {
            string direct = directRoom == null ? "" : directRoom ? "direct:" : "room:";


            string ret = "";
            if (directRoom)
            {
                ret = Businesslogic.MatrixUsernameToShortUsername(roomNameHumanReadable);
            } else
            {
                if (roomNameHumanReadable == null)
                    ret = roomID;
                else
                    ret = roomNameHumanReadable;

                ret += " Room";
            }
            return ret;
        }

    }
}
