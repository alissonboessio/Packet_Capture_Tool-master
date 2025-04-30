using PacketDotNet;
using PacketDotNet.Lldp;
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

        // fields only applied to LLDPPackets.
        public string ChassisId { get; }
        public string PortId { get; }
        public string SystemName { get; }
        public string SystemDesc { get; }
        public string SysCapabilities { get; }
        public string ManagementAddress { get; }
        public string TimeToLive { get; }
        public string Other{ get; }

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

            ChassisId = "N/A";
            PortId = "N/A";
            SystemName = "N/A";
            SystemDesc = "N/A";
            SysCapabilities = "N/A";
            ManagementAddress = "N/A";                     
            TimeToLive = "N/A";                     
            Other = "";                     

            foreach (var tlv in lldpPacket.TlvCollection)
            {
                switch (tlv)
                {
                    case ChassisIdTlv chassisIdTlv:
                        ChassisId = $"{chassisIdTlv.SubType}: {chassisIdTlv.SubTypeValue}";
                        break;
                    case PortIdTlv portIdTlv:
                        PortId = $"{portIdTlv.SubType}: {portIdTlv.SubTypeValue}";
                        break;
                    case SystemNameTlv systemNameTlv:
                        SystemName = $"{systemNameTlv.Name}: {systemNameTlv.Value}";
                        break;
                    case SystemDescriptionTlv systemDescTlv:
                        SystemDesc = $"{systemDescTlv.Description}: {systemDescTlv.Value}";
                        break;
                    case SystemCapabilitiesTlv sysCapabilitiesTlv:
                        SysCapabilities = $"{sysCapabilitiesTlv.Type}: {sysCapabilitiesTlv.Capabilities}";
                        break;
                    case ManagementAddressTlv managementAddrTlv:
                        ManagementAddress = managementAddrTlv.ToString();
                        break;
                    case TimeToLiveTlv timeToLiveTlv:
                        TimeToLive = $"{timeToLiveTlv.Type}: {timeToLiveTlv.Seconds}s";
                        break;
                    case EndOfLldpduTlv endOfLldpdu:
                        var b = endOfLldpdu;
                        break;
                    default:
                        Other += tlv.ToString() + "\n";
                        break;
                }
            }
        }

    }
}
