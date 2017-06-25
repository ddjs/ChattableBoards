

namespace RemoteShared
{
    using System.Text;
    public static class RemoteExtensions
    {
        public static byte[] ToByteArray(this string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }
    }
}
