using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yuck
{
    public class UserTypingInRoom
    {
        public string UserID {get; set; }
        public string RoomID {get; set; }

        private UserTypingInRoom()
        {

        }

        public UserTypingInRoom(string room_id, string user_id)
        {
            UserID = user_id;
            RoomID = room_id;
        }
    }
}