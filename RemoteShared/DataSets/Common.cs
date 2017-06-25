using Newtonsoft.Json;

namespace RemoteShared.DataSets
{
    public abstract class Common
    {
        public const int UserMaximum = 10;

        public string Name { get; set; }

        public int Id { get; set; }

        public override string ToString()
        {
            return string.Format("Name: {0}, Id {1}", this.Name, this.Id);
        }
    }
}
