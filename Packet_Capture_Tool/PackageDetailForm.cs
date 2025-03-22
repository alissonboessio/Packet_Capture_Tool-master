using PacketDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Packet_Capture_Tool
{
    public partial class PackageDetailForm : Form
    {
        private readonly List<PackageDetail> DetailPackagesList;
        

        public PackageDetailForm(List<PackageDetail> detailPackages)
        {
            DetailPackagesList = detailPackages;
            InitializeComponent();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            if (packageId.TextLength.Equals(0)) return;
            else
            {
                int.TryParse(packageId.Text, out int correctParse);
                if (correctParse.Equals(0)) return;

                ClearScreen();
                PopulateScreen(DetailPackagesList.Find(x => x.Id.Equals(int.Parse(packageId.Text))));
            }            
        }

        private void PopulateScreen(PackageDetail package)
        {
            if(package == null)
            {
                MessageBox.Show("Package does not exist! Enter a valid package.", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (package.TcpPacket != null)
            {
                packageType.Text = "TCP PACKET";
                headerText.Text += SetHeader(package.TcpPacket.HeaderData);

                checksumText.Text += package.TcpPacket.Checksum.ToString() + " - is " + BooleanToString(package.TcpPacket.ValidChecksum, 1); 
                windowsSizeText.Text += package.TcpPacket.WindowSize.ToString();
                sequenceNumberText.Text += package.TcpPacket.SequenceNumber.ToString();
                acknowledgmentNumberText.Text += package.TcpPacket.AcknowledgmentNumber.ToString();
                dataOffsetText.Text += package.TcpPacket.DataOffset.ToString();

                sourceAndDestinationText.Text += SetAddress(package.IpPacket, package.TcpPacket);

                flagsText.Text += "\n" + BooleanToString(package.TcpPacket.CongestionWindowReduced) + " -> Congestion Window Reduced (CWR)";
                flagsText.Text += "\n" + BooleanToString(package.TcpPacket.ExplicitCongestionNotificationEcho) + " -> ECN-Echo";
                flagsText.Text += "\n" + BooleanToString(package.TcpPacket.Urgent) + " -> Urgent";
                flagsText.Text += "\n" + BooleanToString(package.TcpPacket.Acknowledgment) + " -> Acknowledgment";
                flagsText.Text += "\n" + BooleanToString(package.TcpPacket.Push) + " -> Push";
                flagsText.Text += "\n" + BooleanToString(package.TcpPacket.Reset) + " -> Reset";
                flagsText.Text += "\n" + BooleanToString(package.TcpPacket.Synchronize) + " -> Syn";
                flagsText.Text += "\n" + BooleanToString(package.TcpPacket.Finished) + " -> Fin";

            }
            else if(package.UdpPacket != null)
            {
                packageType.Text = "UDP PACKET";
                headerText.Text += SetHeader(package.UdpPacket.HeaderData);
                
                checksumText.Text += package.UdpPacket.Checksum.ToString() + " - is " + BooleanToString(package.UdpPacket.ValidChecksum, 1);

                sourceAndDestinationText.Text += SetAddress(package.IpPacket, package.UdpPacket);

                windowsSizeText.Text = sequenceNumberText.Text = acknowledgmentNumberText.Text = dataOffsetText.Text = flagsText.Text = "";
            }
            else if(package.ICMPv4Packet != null)
            {
                packageType.Text = "ICMPv4 PACKET";
                headerText.Text += SetHeader(package.ICMPv4Packet.HeaderData);

                checksumText.Text += package.ICMPv4Packet.Checksum.ToString();

                sourceAndDestinationText.Text += SetAddress(package.IpPacket, package.ICMPv4Packet);

                windowsSizeText.Text = sequenceNumberText.Text = acknowledgmentNumberText.Text = dataOffsetText.Text = flagsText.Text = "";
            }
            else if(package.ICMPv6Packet != null)
            {
                packageType.Text = "ICMPv6 PACKET";
                headerText.Text += SetHeader(package.ICMPv6Packet.HeaderData);

                checksumText.Text += package.ICMPv6Packet.Checksum.ToString();

                sourceAndDestinationText.Text += SetAddress(package.IpPacket, package.ICMPv6Packet);

                windowsSizeText.Text = sequenceNumberText.Text = acknowledgmentNumberText.Text = dataOffsetText.Text = flagsText.Text = "";
            }
            else if (package.IGMPv2Packet != null)
            {
                packageType.Text = "IGMPv2 PACKET";
                headerText.Text += SetHeader(package.IGMPv2Packet.HeaderData);

                checksumText.Text += package.IGMPv2Packet.Checksum.ToString();

                sourceAndDestinationText.Text += SetAddress(package.IpPacket, package.IGMPv2Packet);

                windowsSizeText.Text = sequenceNumberText.Text = acknowledgmentNumberText.Text = dataOffsetText.Text = flagsText.Text = "";
            }
        }

        private string BooleanToString(bool isTrue, int returnStringType = 0)
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

        private string SetHeader(byte[] header)
        {
            bool brokeLine = false;
            string headerText = "";
            foreach (var bytes in header)
            {
                headerText += bytes.ToString().PadLeft(4, '0') + " ";
                if (headerText.Count() > 100 && !brokeLine)
                {
                    headerText += "\n".PadRight(15, ' ');
                    brokeLine = true;
                }
            }

            return headerText;
        }

        private string SetAddress(IPPacket ip, dynamic package)
        {
            var sourcePort = ExistsPortNumber(package) ? $":{package.SourcePort.ToString()}" : string.Empty;
            var destinationPort = ExistsPortNumber(package) ? $":{package.DestinationPort.ToString()}" : string.Empty;
            return  ip.SourceAddress.ToString() + sourcePort
                    + " -> Destination: " + ip.DestinationAddress.ToString() + destinationPort
                    + " - IP Version: " + ip.Version; ;
        }

        private bool ExistsPortNumber(dynamic pack)
        {
            var form = new Form1();
            return form.IsTCPPacket(pack) || form.IsUDPPacket(pack);
        }

        private void ClearScreen()
        {
            headerText.Text = "Header: ";
            checksumText.Text = "Checksum: ";
            windowsSizeText.Text = "Windows Size: ";
            sequenceNumberText.Text = "Sequence Number: ";
            sourceAndDestinationText.Text = "Source: ";
            acknowledgmentNumberText.Text = "Acknowledgment Number: ";
            dataOffsetText.Text = "Data Offset: ";
            flagsText.Text = "--------------------------------------------------------------------------------------> FLAGS <--------------------------------------------------------------------------------------";
        }

        private void PackageDetailForm_Load(object sender, EventArgs e)
        {

        }
    }
}
