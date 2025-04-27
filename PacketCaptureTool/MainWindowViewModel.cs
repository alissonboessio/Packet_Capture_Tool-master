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
                new(2, "TCP", "ip and tcp"),
                new(3, "ICMPv4", "ip and icmp"),
                new(4, "ICMPv6", "icmp6"),
                new(5, "IGMPv2", "ip and igmp"),
                new(6, "ARP", "arp"),
                new(7, "LLDP", "ether proto 0x88cc"),
                new(8, "HTTP", "tcp port 80"),
                new(9, "HTTPS", "tcp port 443"),
                new(10, "FTP", "tcp port 21"),
            };

            this.packetsOptions[0].check = true;

            this.get_Device_List();
            stopCapture = false;
            isRunning = false;
            AllPackets = new ObservableCollection<PackageDetail>();
        }

        public ObservableCollection<PackageDetail> AllPackets 
        {
            get => Get<ObservableCollection<PackageDetail>>();
            set => Set(value);
        }
        public ObservableCollection<PackageDetail> TcpPackets { get; set; } = new();
        public ObservableCollection<PackageDetail> UdpPackets { get; set; } = new();
        public ObservableCollection<PackageDetail> Icmpv4Packets { get; set; } = new();
        public ObservableCollection<PackageDetail> ICMPv6Packets { get; set; } = new();
        public ObservableCollection<PackageDetail> IGMPPackets { get; set; } = new();
        public ObservableCollection<PackageDetail> ARPPackets { get; set; } = new();
        public ObservableCollection<PackageDetail> LLDPPackets { get; set; } = new();


        public List<ValueDescOption> packetsOptions
        {
            get => Get<List<ValueDescOption>>();
            set
            {
                Set(value);
                OnPropertyChanged(nameof(isStartEnabled));
            }
        }

        public CaptureDeviceList? devices
        {
            get => Get<CaptureDeviceList> ();
            set => Set(value);
        }
        public ICaptureDevice? selectedDevice
        {
            get => Get<ICaptureDevice>();
            set
            {
                Set(value);
                OnPropertyChanged(nameof(isStartEnabled));
            }
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
        public bool isRunning
        {
            get => Get<bool>();
            set 
            {
                Set(value);
                OnPropertyChanged(nameof(isStartEnabled));
            } 
        }
        public bool isStartEnabled
        {
            get => !isRunning && selectedDevice != null && packetsOptions.Any(opt => opt.check);
        }

        private readonly List<PackageDetail> _buffer = new();
        private readonly object _bufferLock = new();

        private void StartBufferTimer()
        {
            var timer = new System.Timers.Timer(300); // A cada 300ms
            timer.Elapsed += (s, e) =>
            {
                List<PackageDetail> packetsToAdd;

                lock (_bufferLock)
                {
                    if (_buffer.Count == 0) return;
                    packetsToAdd = new List<PackageDetail>(_buffer);
                    _buffer.Clear();
                }

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var packet in packetsToAdd)
                    {
                        AddPacket(packet);
                    }
                });
            };
            timer.Start();
        }

        public void StartCapture()
        {
            Thread l = new Thread(new ThreadStart(StartListening))
            {
                IsBackground = true
            };

            l.Start();
            isRunning = true;
            StartBufferTimer();
        }
        public void StopCapture()
        {
            stopCapture = true;
            isRunning = false;
        }
        
        public void CleanList()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => 
            {
                AllPackets.Clear();
                TcpPackets.Clear();
                UdpPackets.Clear();
                Icmpv4Packets.Clear();
                ICMPv6Packets.Clear();
                IGMPPackets.Clear();
                ARPPackets.Clear();
                LLDPPackets.Clear();

                PackageDetail.ResetId();
            });
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
                        filter += $" or {item.auxValue}";
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
                Thread.Sleep(100);
            }

            device.StopCapture();
            device.Close();

            stopCapture = false;
        }

        public void AddPacket(PackageDetail packet)
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
            {
                AllPackets.Add(packet);

                if (packet.TcpPacket != null)
                    TcpPackets.Add(packet);
                else if (packet.UdpPacket != null)
                    UdpPackets.Add(packet);
                else if (packet.ICMPv4Packet != null)
                    Icmpv4Packets.Add(packet);
                else if (packet.ICMPv6Packet != null)
                    ICMPv6Packets.Add(packet);
                else if (packet.IGMPv2Packet != null)
                    IGMPPackets.Add(packet);
                else if (packet.ARPPacket != null)
                    ARPPackets.Add(packet);
                else if (packet.LLDPPacket != null)
                    LLDPPackets.Add(packet);
            }
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    AllPackets.Add(packet);

                    if (packet.TcpPacket != null)
                        TcpPackets.Add(packet);
                    else if (packet.UdpPacket != null)
                        UdpPackets.Add(packet);
                    else if (packet.ICMPv4Packet != null)
                        Icmpv4Packets.Add(packet);
                    else if (packet.ICMPv6Packet != null)
                        ICMPv6Packets.Add(packet);
                    else if (packet.IGMPv2Packet != null)
                        IGMPPackets.Add(packet);
                    else if (packet.ARPPacket != null)
                        ARPPackets.Add(packet);
                    else if (packet.LLDPPacket != null)
                        LLDPPackets.Add(packet);
                });
            }
        }


        private void device_OnPacketArrival(object sender, PacketCapture packet)
        {
            Packet pack = Packet.ParsePacket(packet.GetPacket().LinkLayerType, packet.Data.ToArray());
            DateTime time = packet.GetPacket().Timeval.Date.AddHours(-3);
            int len = packet.Data.Length;

            if (IsTCPPacket(pack))
            {
                AddTCPPacket(pack, time, len);
            }
            else if (IsUDPPacket(pack))
            {
                AddUDPPacket(pack, time, len);
            }
            else if (IsIcmpV4Packet(pack))
            {
                AddIcmpV4Packet(pack, time, len);
            }
            else if (IsICMPv6Packet(pack))
            {
                AddICMPv6Packet(pack, time, len);
            }
            else if (IsIGMPv2Packet(pack))
            {
                AddIGMPv2Packet(pack, time, len);
            }
            else if (IsArpPacket(pack))
            {
                AddArpPacket(pack, time, len);
            }
            else if (IsLldpPacket(pack))
            {
                AddLldpPacket(pack, time, len);
            }

        }        

        #region TCP
        public bool IsTCPPacket(Packet pack) => pack.Extract<TcpPacket>() != null;
        private void AddTCPPacket(Packet pack, DateTime time, int len)
        {
            var tcpPacket = pack.Extract<TcpPacket>();
            IPPacket ipPacket = (IPPacket)tcpPacket.ParentPacket;

            lock (_bufferLock)
            {
                _buffer.Add(new PackageDetail(tcpPacket, ipPacket, time));
            }

        }
        #endregion

        #region UDP
        public bool IsUDPPacket(Packet pack) => pack.Extract<UdpPacket>() != null;
        private void AddUDPPacket(Packet pack, DateTime time, int len)
        {
            var udpPacket = pack.Extract<UdpPacket>();
            IPPacket ipPacket = (IPPacket)udpPacket.ParentPacket;

            lock (_bufferLock)
            {
                _buffer.Add(new PackageDetail(udpPacket, ipPacket, time));
            }

        }
        #endregion

        #region ICMPv4
        public bool IsIcmpV4Packet(Packet pack) => pack.Extract<IcmpV4Packet>() != null;
        private void AddIcmpV4Packet(Packet pack, DateTime time, int len)
        {
            var icmpv4Packet = pack.Extract<IcmpV4Packet>();
            IPPacket ipPacket = (IPPacket)icmpv4Packet.ParentPacket;

            lock (_bufferLock)
            {
                _buffer.Add(new PackageDetail(icmpv4Packet, ipPacket, time));
            }

        }
        #endregion

        #region ICMPv6
        public bool IsICMPv6Packet(Packet pack) => pack.Extract<IcmpV6Packet>() != null;
        private void AddICMPv6Packet(Packet pack, DateTime time, int len)
        {
            var icmpv6Packet = pack.Extract<IcmpV6Packet>();
            IPPacket ipPacket = (IPPacket)icmpv6Packet.ParentPacket;

            lock (_bufferLock)
            {
                _buffer.Add(new PackageDetail(icmpv6Packet, ipPacket, time));
            }

        }
        #endregion

        #region IGMP
        public bool IsIGMPv2Packet(Packet pack) => pack.Extract<IgmpV2Packet>() != null;
        private void AddIGMPv2Packet(Packet pack, DateTime time, int len)
        {
            var igmpv2Packet = pack.Extract<IgmpV2Packet>();
            IPPacket ipPacket = (IPPacket)igmpv2Packet.ParentPacket;

            lock (_bufferLock)
            {
                _buffer.Add(new PackageDetail(igmpv2Packet, ipPacket, time));
            }

        }
        #endregion

        #region ARP
        public bool IsArpPacket(Packet pack) => pack.Extract<ArpPacket>() != null;

        private void AddArpPacket(Packet pack, DateTime time, int len)
        {
            var arpPacket = pack.Extract<ArpPacket>();

            lock (_bufferLock)
            {
                _buffer.Add(new PackageDetail(arpPacket, null, time));
            }

        }
        #endregion

        #region LLDP
        public bool IsLldpPacket(Packet pack) => pack.Extract<LldpPacket>() != null;

        private void AddLldpPacket(Packet pack, DateTime time, int len)
        {
            var lldpPacket = pack.Extract<LldpPacket>();

            lock (_bufferLock)
            {
                _buffer.Add(new PackageDetail(lldpPacket, null, time));
            }

        }
        #endregion

        #region DisplayData

        public string GetAddressText(IPPacket ip, dynamic package)
        {
            bool existsPortNumber = IsTCPPacket(package) || IsUDPPacket(package);
            var sourcePort = existsPortNumber ? $"{package.SourcePort}" : string.Empty;
            var destinationPort = existsPortNumber ? $"{package.DestinationPort}" : string.Empty;

            return $"Source: {ip.SourceAddress}:{sourcePort} -> Destination: {ip.DestinationAddress}:{destinationPort} - IP Version: {ip.Version}";
        }

        public string BooleanToString(bool isTrue, int returnStringType = 0)
        {
            switch (returnStringType)
            {
                case 0:
                    return isTrue ? "Set" : "Not Set";
                case 1:
                default:
                    return isTrue ? "valid" : "invalid";
            }
        }

        public string GetHeader(byte[] header)
        {
            string headerText = "Header: ";
            foreach (var bytes in header)
            {
                headerText += bytes.ToString().PadLeft(4, '0') + " ";
            }

            return headerText;
        }

        #endregion

    }
}
