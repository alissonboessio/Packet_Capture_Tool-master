using PacketCaptureTool.Objects;
using System.Collections.ObjectModel;

namespace PacketCaptureTool
{
    class MainWindowViewModel : PropChange
    {

        public MainWindowViewModel()
        {
            this.packetsOptions = new List<ValueDescOption> {
                new(0, "ALL"),
                new(1, "UDP"),
                new(2, "TCP"),
                new(3, "ICMPv4"),
                new(4, "ICMPv6"),
                new(5, "ARP"),
                new(6, "LLDP")
            };
        }

        public List<ValueDescOption> packetsOptions
        {
            get => Get<List<ValueDescOption>>();
            set => Set(value);
        }


    }
}
