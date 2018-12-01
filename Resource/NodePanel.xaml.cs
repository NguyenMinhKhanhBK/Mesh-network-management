using Mesh_Network_Manager.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Mesh_Network_Manager.Resource
{
    /// <summary>
    /// Interaction logic for NodePanel.xaml
    /// </summary>
    public partial class NodePanel : Window
    {
        public NodePanel(long NodeID, long LedStatus)
        {
            InitializeComponent();
            var vm = new NodePanelViewModel();
            vm.nodeID = NodeID;
            if (LedStatus!=0) vm.LedColor = Brushes.Green;
            else vm.LedColor = Brushes.Gray;
            this.DataContext = vm;
        }
    }
}
