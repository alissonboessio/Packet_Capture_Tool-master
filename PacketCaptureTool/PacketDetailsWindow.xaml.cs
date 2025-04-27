using System.Windows;
using System.Windows.Controls;

namespace PacketCaptureTool
{
    public partial class PacketDetailWindow : Window
    {
        public string TitleText { get; set; }
        public UIElement MainContent { get; set; }
        public UIElement SecondaryContent { get; set; }

        public PacketDetailWindow(string title, UIElement mainContent, UIElement secondaryContent)
        {
            InitializeComponent();
            TitleText = title;
            MainContent = mainContent;
            SecondaryContent = secondaryContent;
            DataContext = this;
        }
    }
}
