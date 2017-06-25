using System.Collections.Generic;

namespace RemoteShared.DataSets
{
    public class Room : Common
    {
        public enum RoomType :byte
        {
            Public,
            Private,
        }

        public Room()
        {
            this.Type = RoomType.Public;
            this.Name = string.Empty;
        }

        public Room(RoomType type, string name)
        {
            this.Name = name;
            this.Type = type;
            this.Users = new List<User>(UserMaximum);
        }

        public RoomType Type { get; set; }

        public List<User> Users { get; set; }
    }
}
