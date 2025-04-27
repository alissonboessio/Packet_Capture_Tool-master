using PacketCaptureTool.Objects;
using PacketDotNet.Lldp;
using System.IO.Packaging;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PacketCaptureTool
{
    public partial class MainWindow : Window
    {

        MainWindowViewModel viewModel = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void TcpPacketsFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is PackageDetail packet)
            {
                e.Accepted = packet.TcpPacket != null;
            }
            else
            {
                e.Accepted = false;
            }
        }


        private void BTStopCapture_Click(object sender, RoutedEventArgs e)
        {
            viewModel.StopCapture();
        }

        private void BTStartCapture_Click(object sender, RoutedEventArgs e)
        {
            viewModel.StartCapture();
        }

        private void BTCleanList_Click(object sender, RoutedEventArgs e)
        {
            viewModel.CleanList();
        }

        private void LVTCP_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem != null)
            {
                PackageDetail selectedPacket = (PackageDetail)listView.SelectedItem;

                var mainPanel = new StackPanel();
                mainPanel.Children.Add(new TextBlock { Text = $"{viewModel.GetAddressText(selectedPacket.IpPacket, selectedPacket.TcpPacket)}", FontSize = 14 });
                mainPanel.Children.Add(new TextBlock { 
                    Text = $"{viewModel.GetHeader(selectedPacket.TcpPacket.HeaderData)}", 
                    TextWrapping = TextWrapping.Wrap, 
                    Margin = new Thickness(0, 10, 0, 0) 
                });

                var grid1 = new Grid { Margin = new Thickness(0,10,0,0) };
                grid1.ColumnDefinitions.Add(new ColumnDefinition());
                grid1.ColumnDefinitions.Add(new ColumnDefinition());
                grid1.ColumnDefinitions.Add(new ColumnDefinition());

                var checksumText = new TextBlock
                {
                    Text = $"Checksum: {selectedPacket.TcpPacket.Checksum} - is {viewModel.BooleanToString(selectedPacket.TcpPacket.ValidChecksum, 1)}"
                };
                Grid.SetColumn(checksumText, 0);
                grid1.Children.Add(checksumText);

                var windowSizeText = new TextBlock
                {
                    Text = $"Window size: {selectedPacket.TcpPacket.WindowSize}"
                };
                Grid.SetColumn(windowSizeText, 1);
                grid1.Children.Add(windowSizeText);

                var sequenceNumberText = new TextBlock
                {
                    Text = $"Sequence number: {selectedPacket.TcpPacket.SequenceNumber}"
                };
                Grid.SetColumn(sequenceNumberText, 2);
                grid1.Children.Add(sequenceNumberText);

                mainPanel.Children.Add(grid1);

                var grid2 = new Grid();
                grid2.ColumnDefinitions.Add(new ColumnDefinition());
                grid2.ColumnDefinitions.Add(new ColumnDefinition());

                var ackNumberText = new TextBlock
                {
                    Text = $"Acknowledgment number: {selectedPacket.TcpPacket.AcknowledgmentNumber}"
                };
                Grid.SetColumn(ackNumberText, 0);
                grid2.Children.Add(ackNumberText);

                var dataOffsetText = new TextBlock
                {
                    Text = $"Data Offset: {selectedPacket.TcpPacket.DataOffset}"
                };
                Grid.SetColumn(dataOffsetText, 1);
                grid2.Children.Add(dataOffsetText);

                mainPanel.Children.Add(grid2);

                var secondaryPanel = new StackPanel();

                string flagsText = "Flags: ";

                flagsText += "\n" + viewModel.BooleanToString(selectedPacket.TcpPacket.CongestionWindowReduced) + " -> Congestion Window Reduced (CWR)";
                flagsText += "\n" + viewModel.BooleanToString(selectedPacket.TcpPacket.ExplicitCongestionNotificationEcho) + " -> ECN-Echo";
                flagsText += "\n" + viewModel.BooleanToString(selectedPacket.TcpPacket.Urgent) + " -> Urgent";
                flagsText += "\n" + viewModel.BooleanToString(selectedPacket.TcpPacket.Acknowledgment) + " -> Acknowledgment";
                flagsText += "\n" + viewModel.BooleanToString(selectedPacket.TcpPacket.Push) + " -> Push";
                flagsText += "\n" + viewModel.BooleanToString(selectedPacket.TcpPacket.Reset) + " -> Reset";
                flagsText += "\n" + viewModel.BooleanToString(selectedPacket.TcpPacket.Synchronize) + " -> Syn";
                flagsText += "\n" + viewModel.BooleanToString(selectedPacket.TcpPacket.Finished) + " -> Fin";

                secondaryPanel.Children.Add(new TextBlock { Text = flagsText });

                var detailWindow = new PacketDetailWindow("TCP Packet", mainPanel, secondaryPanel);
                detailWindow.Show();
            }
        }

        private void LVUDP_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem != null)
            {
                PackageDetail selectedPacket = (PackageDetail)listView.SelectedItem;

                var mainPanel = new StackPanel();
                mainPanel.Children.Add(new TextBlock { Text = $"{viewModel.GetAddressText(selectedPacket.IpPacket, selectedPacket.UdpPacket)}", FontSize = 14 });
                mainPanel.Children.Add(new TextBlock
                {
                    Text = $"{viewModel.GetHeader(selectedPacket.UdpPacket.HeaderData)}",
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 10, 0, 0)
                });
                mainPanel.Children.Add(new TextBlock
                {
                    Text = $"Checksum: {selectedPacket.UdpPacket.Checksum} - is {viewModel.BooleanToString(selectedPacket.UdpPacket.ValidChecksum, 1)}",
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 10, 0, 0)
                });

                var detailWindow = new PacketDetailWindow("UDP Packet", mainPanel, null);
                detailWindow.Show();
            }
        }

        private void LVICMPv4_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem != null)
            {
                PackageDetail selectedPacket = (PackageDetail)listView.SelectedItem;

                var mainPanel = new StackPanel();
                mainPanel.Children.Add(new TextBlock { Text = $"{viewModel.GetAddressText(selectedPacket.IpPacket, selectedPacket.ICMPv4Packet)}", FontSize = 14 });
                mainPanel.Children.Add(new TextBlock
                {
                    Text = $"{viewModel.GetHeader(selectedPacket.ICMPv4Packet.HeaderData)}",
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 10, 0, 0)
                });
                mainPanel.Children.Add(new TextBlock
                {
                    Text = $"Checksum: {selectedPacket.ICMPv4Packet.Checksum}",
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 10, 0, 0)
                });

                var detailWindow = new PacketDetailWindow("ICMPv4 Packet", mainPanel, null);
                detailWindow.Show();
            }
        }

        private void LVICMPv6_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem != null)
            {
                PackageDetail selectedPacket = (PackageDetail)listView.SelectedItem;

                var mainPanel = new StackPanel();
                mainPanel.Children.Add(new TextBlock { Text = $"{viewModel.GetAddressText(selectedPacket.IpPacket, selectedPacket.ICMPv6Packet)}", FontSize = 14 });
                mainPanel.Children.Add(new TextBlock
                {
                    Text = $"{viewModel.GetHeader(selectedPacket.ICMPv6Packet.HeaderData)}",
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 10, 0, 0)
                });
                mainPanel.Children.Add(new TextBlock
                {
                    Text = $"Checksum: {selectedPacket.ICMPv6Packet.Checksum}",
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 10, 0, 0)
                });

                var detailWindow = new PacketDetailWindow("ICMPv6 Packet", mainPanel, null);
                detailWindow.Show();
            }
        }

        private void LVIGMP_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem != null)
            {
                PackageDetail selectedPacket = (PackageDetail)listView.SelectedItem;

                var mainPanel = new StackPanel();
                mainPanel.Children.Add(new TextBlock { Text = $"{viewModel.GetAddressText(selectedPacket.IpPacket, selectedPacket.IGMPv2Packet)}", FontSize = 14 });
                mainPanel.Children.Add(new TextBlock
                {
                    Text = $"{viewModel.GetHeader(selectedPacket.IGMPv2Packet.HeaderData)}",
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 10, 0, 0)
                });
                mainPanel.Children.Add(new TextBlock
                {
                    Text = $"Checksum: {selectedPacket.IGMPv2Packet.Checksum}",
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 10, 0, 0)
                });

                var detailWindow = new PacketDetailWindow("IGMPv2 Packet", mainPanel, null);
                detailWindow.Show();
            }
        }

        private void LVARP_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem != null)
            {
                PackageDetail selectedPacket = (PackageDetail)listView.SelectedItem;

                string directionArp = $"From: {selectedPacket.ARPPacket.SenderHardwareAddress} MAC (IP {selectedPacket.ARPPacket.SenderProtocolAddress})" +
                $" To -> {selectedPacket.ARPPacket.TargetHardwareAddress} MAC (IP {selectedPacket.ARPPacket.TargetProtocolAddress})";

                var mainPanel = new StackPanel();
                mainPanel.Children.Add(new TextBlock { Text = directionArp, FontSize = 14 });
                mainPanel.Children.Add(new TextBlock
                {
                    Text = $"{viewModel.GetHeader(selectedPacket.ARPPacket.HeaderData)}",
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 10, 0, 0)
                });
                mainPanel.Children.Add(new TextBlock
                {
                    Text = $"Operation: {selectedPacket.ARPPacket.Operation}",
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 10, 0, 0)
                });

                var detailWindow = new PacketDetailWindow("ARP Packet", mainPanel, null);
                detailWindow.Show();
            }

        }

        private void LVLLDP_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem is PackageDetail selectedPacket)
            {             
                var mainPanel = new StackPanel();

                mainPanel.Children.Add(new TextBlock { Text = $"Chassis ID: {selectedPacket.ChassisId}", FontSize = 14, TextWrapping = TextWrapping.Wrap });
                mainPanel.Children.Add(new TextBlock { Text = $"Port ID: {selectedPacket.PortId}", FontSize = 14, TextWrapping = TextWrapping.Wrap });
                mainPanel.Children.Add(new TextBlock { Text = $"System Name: {selectedPacket.SystemName}", FontSize = 14, TextWrapping = TextWrapping.Wrap });
                mainPanel.Children.Add(new TextBlock { Text = $"System Description: {selectedPacket.SystemDesc}", FontSize = 14, TextWrapping = TextWrapping.Wrap });
                mainPanel.Children.Add(new TextBlock { Text = $"System Capabilities: {selectedPacket.SysCapabilities}", FontSize = 14, TextWrapping = TextWrapping.Wrap });
                mainPanel.Children.Add(new TextBlock { Text = $"Management Address: {selectedPacket.ManagementAddress}", FontSize = 14, TextWrapping = TextWrapping.Wrap });

                var detailWindow = new PacketDetailWindow("LLDP Packet", mainPanel, null);
                detailWindow.Show();
            }
        }
    }
}