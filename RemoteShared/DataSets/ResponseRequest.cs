using System.Collections.Generic;
using System.Linq;

namespace RemoteShared.DataSets
{
    public class ResponseRequest
    {
        protected static readonly byte[] ResponseRequestCommandHeader = new byte[] { 0xff, 0xff, 0xff, 0x25 };

        public Commands Command { get; set; }

        public MessageStatus Status { get; set; }

        public object Data { get; set; }

        public static bool IsResponseRequestPacket(IEnumerable<byte> source, out ResponseRequest output)
        {
            output = default(ResponseRequest);

            if (source.Take(ResponseRequestCommandHeader.Length).SequenceEqual(ResponseRequestCommandHeader))
            {
                output = source.FromJson<ResponseRequest>(ResponseRequestCommandHeader.Length);
            }

            return !output.Equals(default(ResponseRequest));
        }

        public byte[] ToByteArray()
        {
            var packet = new List<byte>(ResponseRequestCommandHeader);
            packet.AddRange(this.ToJson().ToByteArray());
            return packet.ToArray();
        }
    }
}
