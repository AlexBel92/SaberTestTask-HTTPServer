using System.Net;

namespace HTTPServer
{
    public class TcpServerSettings
    {
        public const string Position = "TcpServerSettings";

        public string IpAddressString { get; set; }
        public int GetPortNumber { get; set; }
        public int AddPortNumber { get; set; }
        public int UpdatePortNumber { get; set; }

        public IPAddress GetIPAddress() => IPAddress.Parse(IpAddressString);
    }
}
