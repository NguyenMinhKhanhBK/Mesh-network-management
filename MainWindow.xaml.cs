using Mesh_Network_Manager.Model;
using Mesh_Network_Manager.Resource;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mesh_Network_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
            EventSystem.Subscribe<List<Node>>(NodeChanged);
            EventSystem.Subscribe<Node>(NodeStatusChanged);
        }


        void NodeChanged(List<Node> _msg)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (_msg != null)
                {
                    List<Button> removeButtonList = new List<Button>();
                    foreach (var obj in wrapPanelTopology.Children)
                    {
                        int foundObjectInList = 0;
                        Button btn = obj as Button;
                        if(btn!=null)
                        {
                            foreach (var node in _msg)
                            {
                                if (btn.Tag.ToString().Split('/')[0] == node.nodeID.ToString()) foundObjectInList++;
                            }
                            if (foundObjectInList == 0) removeButtonList.Add(btn);
                        }
                        
                    }

                    foreach (var btn in removeButtonList)
                    {
                        wrapPanelTopology.Children.Remove(btn);
                    }


                    foreach (var node in _msg)
                    {
                        int foundNodeInPanel = 0;
                        if(node!=null)
                        {
                            foreach (var obj in wrapPanelTopology.Children)
                            {
                                var btn = obj as Button;
                                if (btn.Tag.ToString().Split('/')[0] == node.nodeID.ToString()) foundNodeInPanel++;
                            }
                            if(foundNodeInPanel==0)
                            {
                                Button newButton = new Button();
                                newButton.Margin = new Thickness(10, 0, 10, 0);
                                newButton.Height = 100;
                                newButton.Width = 200;
                                var bc = new BrushConverter();
                                newButton.Background = (Brush)bc.ConvertFrom("#FF0099FF");
                                newButton.Tag = node.nodeID.ToString() + "/" + node.status.ToString();
                                newButton.Content = node.nodeID.ToString();
                                wrapPanelTopology.Children.Add(newButton);
                            }
                        }
                    }

                    
                }
            }
                
            );
            
        }

        void NodeStatusChanged(Node msg)
        {
            this.Dispatcher.Invoke(() =>
            {
                foreach (var obj in wrapPanelTopology.Children)
                {
                    Button btn = obj as Button;
                    if (btn != null)
                    {
                        if (btn.Tag.ToString().Split('/')[0] == msg.nodeID.ToString()) btn.Tag = msg.nodeID + "/" + msg.status;
                    }
                }
            });
            
        }

        private void ButtonClickHandler(object sender, RoutedEventArgs e)
        {
            var a = e.Source as Button;
            string tag = a.Tag.ToString();
            long _nodeID = long.Parse(tag.Split('/')[0]);
            long _status = long.Parse(tag.Split('/')[1]);
            NodePanel panel = new NodePanel(_nodeID,_status);
            var point = a.PointToScreen(Mouse.GetPosition(a));
            panel.Left = point.X;
            panel.Top = point.Y;
            a.Opacity = 0.5;
            panel.ShowDialog();
            a.Opacity = 1;
        }

        
    }
}
