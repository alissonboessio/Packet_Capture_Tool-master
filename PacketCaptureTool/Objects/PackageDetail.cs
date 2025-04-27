using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketCaptureTool.Objects
{
    public class PackageDetail
    {
        public int Id { get; private set; }
        public DateTime time { get; set; }
        public UdpPacket UdpPacket { get; private set; }

        public TcpPacket TcpPacket { get; private set; }
        public IcmpV4Packet ICMPv4Packet { get; private set; }
        public IcmpV6Packet ICMPv6Packet { get; private set; }
        public IgmpV2Packet IGMPv2Packet { get; private set; }
        public ArpPacket ARPPacket { get; private set; }
        public LldpPacket LLDPPacket { get; private set; }

        public IPPacket IpPacket { get; private set; }

        private static int _newId;

        public PackageDetail()
        {
            _newId = 0;
        }

        public static void ResetId()
        {
            _newId = 0;
        }

        public PackageDetail(TcpPacket tcpPacket, IPPacket ipPacket, DateTime time)
        {
            Id = Interlocked.Increment(ref _newId);
            this.time = time;
            TcpPacket = tcpPacket;
            IpPacket = ipPacket;
        }
        public PackageDetail(UdpPacket udpPacket, IPPacket ipPacket, DateTime time)
        {
            Id = Interlocked.Increment(ref _newId);
            this.time = time;
            UdpPacket = udpPacket;
            IpPacket = ipPacket;
        }

        public PackageDetail(IcmpV4Packet iCMPv4Packet, IPPacket ipPacket, DateTime time)
        {
            Id = Interlocked.Increment(ref _newId);
            this.time = time;
            ICMPv4Packet = iCMPv4Packet;
            IpPacket = ipPacket;
        }
        public PackageDetail(IcmpV6Packet iCMPv6Packet, IPPacket ipPacket, DateTime time)
        {
            Id = Interlocked.Increment(ref _newId);
            this.time = time;
            ICMPv6Packet = iCMPv6Packet;
            IpPacket = ipPacket;
        }
        public PackageDetail(IgmpV2Packet iGMPv2Packet, IPPacket ipPacket, DateTime time)
        {
            Id = Interlocked.Increment(ref _newId);
            this.time = time;
            IGMPv2Packet = iGMPv2Packet;
            IpPacket = ipPacket;
        }
        public PackageDetail(ArpPacket arpPacket, IPPacket ipPacket, DateTime time)
        {
            Id = Interlocked.Increment(ref _newId);
            this.time = time;
            ARPPacket = arpPacket;
            IpPacket = ipPacket;
        }
        public PackageDetail(LldpPacket lldpPacket, IPPacket ipPacket, DateTime time)
        {
            Id = Interlocked.Increment(ref _newId);
            this.time = time;
            LLDPPacket = lldpPacket;
            IpPacket = ipPacket;
        }

    }
}
