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

        public bool directRoom { get; set; } //direct chat, only 2 users in "room"

        
        public override string ToString() 
        {
            string direct = directRoom == null ? "" : directRoom ? "direct:" : "room:";
            return direct + (roomNameHumanReadable == null ? roomID : Businesslogic.MatrixUsernameToShortUsername(roomNameHumanReadable));
        }

    }
}
