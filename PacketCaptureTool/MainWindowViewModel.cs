using PacketCaptureTool.Objects;
using PacketDotNet;
using SharpPcap;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Controls;

namespace PacketCaptureTool
{
    class MainWindowViewModel : PropChange
    {

        public MainWindowViewModel()
        {
            this.packetsOptions = new List<ValueDescOption> {
                new(0, "ALL"),
                new(1, "UDP", "ip and udp"),
                new(2, "TCP", "ip and tdp"),
                new(3, "ICMPv4", "ip and icmp"),
                new(4, "ICMPv6", "icmp6"),
                new(5, "IGMPv2", "ip and igmp"),
                new(6, "ARP", "arp"),
                new(7, "LLDP", "lldp")
            };

            this.get_Device_List();
            stopCapture = false;
        }

        public ObservableCollection<PackageDetail> AllPackets { get; } = new();
        public ObservableCollection<PackageDetail> TcpPackets => new(AllPackets.Where(p => p.TcpPacket != null));
        public ObservableCollection<PackageDetail> UdpPackets => new(AllPackets.Where(p => p.UdpPacket != null));
        public ObservableCollection<PackageDetail> Icmpv4Packets => new(AllPackets.Where(p => p.ICMPv4Packet != null));


        public List<ValueDescOption> packetsOptions
        {
            get => Get<List<ValueDescOption>>();
            set => Set(value);
        }

        public CaptureDeviceList? devices
        {
            get => Get<CaptureDeviceList> ();
            set => Set(value);
        }
        public ICaptureDevice? selectedDevice
        {
            get => Get<ICaptureDevice>();
            set => Set(value);
        }

        public bool promiscuosMode
        {
            get => Get<bool>();
            set => Set(value);
        }
        public bool stopCapture
        {
            get => Get<bool>();
            set => Set(value);
        }

        public void StartCapture()
        {
            Thread l = new Thread(new ThreadStart(StartListening))
            {
                IsBackground = true
            };

            l.Start();
        }

        private void get_Device_List()
        {
            devices = CaptureDeviceList.Instance;            
        }

        string buildFilter()
        {
            if (this.packetsOptions[0].check) return "";

            string filter = string.Empty;

            foreach (var item in packetsOptions)
            {
                if (item.check)
                {
                    if (!string.IsNullOrEmpty(filter))
                    {
                        filter += $" and {item.auxValue}";
                    }
                    else
                    {
                        filter = item.auxValue;
                    }
                }
            }

            return filter;
        }

        void StartListening()
        {
            ICaptureDevice device = selectedDevice!;

            device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);

            int readTimeoutMilliseconds = 1000;
            if (promiscuosMode == true)
            {
                device.Open(DeviceModes.Promiscuous, readTimeoutMilliseconds);
            }
            else
            {
                device.Open(DeviceModes.None, readTimeoutMilliseconds);
            }

            device.Filter = buildFilter();               

            device.StartCapture();
            while (!stopCapture)
            {

            }

            device.StopCapture();
            device.Close();

            stopCapture = false;
        }

        private void device_OnPacketArrival(object sender, PacketCapture packet)
        {
            Packet pack = Packet.ParsePacket(packet.GetPacket().LinkLayerType, packet.Data.ToArray());
            DateTime time = packet.GetPacket().Timeval.Date;
            int len = packet.Data.Length;

            if (IsTCPPacket(pack))
            {
                ShowTCPPacket(pack, time, len);
            }
            //else if (IsUDPPacket(pack))
            //{
            //    ShowUDPPacket(pack, time, len);
            //}
            //else if (IsIcmpV4Packet(pack))
            //{
            //    ShowIcmpV4Packet(pack, time, len);
            //}
            //else if (IsICMPv6Packet(pack))
            //{
            //    ShowICMPv6Packet(pack, time, len);
            //}
            //else if (IsIGMPv2Packet(pack))
            //{
            //    ShowIGMPv2Packet(pack, time, len);
            //}
            //else if (IsArpPacket(pack))
            //{
            //    ShowArpPacket(pack, time, len);
            //}
            //else if (IsLldpPacket(pack))
            //{
            //    ShowLldpPacket(pack, time, len);
            //}
        }

        #region TCP
        public bool IsTCPPacket(Packet pack) => pack.Extract<TcpPacket>() != null;
        private void ShowTCPPacket(Packet pack, DateTime time, int len)
        {
            var tcpPacket = pack.Extract<TcpPacket>();
            IPPacket ipPacket = (IPPacket)tcpPacket.ParentPacket;
            AllPackets.Add(new PackageDetail(tcpPacket, null, ipPacket));

            //var srcIp = ipPacket.SourceAddress;
            //var dstIp = ipPacket.DestinationAddress;
            //var srcPort = tcpPacket.SourcePort;
            //var dstPort = tcpPacket.DestinationPort;
            //writeLine = string.Format("ID: {9} - {0}:{1}:{2},{3} - TCP Packet: {5}:{6}  -> {7}:{8}\n\n",
            //                    time.Hour, time.Minute, time.Second, time.Millisecond, len,
            //                    srcIp, srcPort, dstIp, dstPort, packageDetail.Id);
        }
        #endregion

    }
}
