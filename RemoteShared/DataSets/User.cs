using Newtonsoft.Json;
using System.Collections.Generic;

namespace RemoteShared.DataSets
{
    public class User : Common
    {
        public User()
        {
            this.Rooms = new List<Room>(2);
        }

        public List<Room> Rooms { get; set; }
    }
}
