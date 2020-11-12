using PacketDotNet;
using System.Threading;

namespace Packet_Capture_Tool
{
    public class PackageDetail
    {
        public int Id { get; private set; }

        public UdpPacket UdpPacket { get; private set; }

        public TcpPacket TcpPacket { get; private set; }

        public IpPacket IpPacket { get; private set; }

        private static int _newId;

        public PackageDetail()
        {
            _newId = 0;
        }

        public PackageDetail(TcpPacket tcpPacket, UdpPacket udpPacket, IpPacket ipPacket)
        {
            Id = Interlocked.Increment(ref _newId);
            TcpPacket = tcpPacket;
            UdpPacket = udpPacket;
            IpPacket = ipPacket;
        }



    }
}
