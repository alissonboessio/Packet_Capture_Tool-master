using PacketCaptureTool.Objects;
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

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void LVTCP_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void LVUDP_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void LVICMPv4_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void LVICMPv6_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void LVIGMP_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void LVARP_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void LVLLDP_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}