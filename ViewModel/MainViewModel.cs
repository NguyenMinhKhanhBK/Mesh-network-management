using Mesh_Network_Manager.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Mesh_Network_Manager.Model;
using uPLibrary.Networking.M2Mqtt;

namespace Mesh_Network_Manager.ViewModel
{
    
  public  class MainViewModel: BaseViewModel
    {
        public string fromEspTopic = "HCMUT/MeshNetwork/ESP8266/FromESP";
        public string fromServerTopic = "HCMUT/MeshNetwork/ESP8266/FromServer";
        public string nodesTopic = "HCMUT/MeshNetwork/ESP8266/Nodes";
        public string nodesControlTopic = "HCMUT/MeshNetwork/ESP8266/NodesControl";
        public string getNodesTopic = "HCMUT/MeshNetwork/ESP8266/GetNodes";
        public ICommand OpenPanelCommand { get; set; }
        public ICommand connectCommand { get; set; }
        public ICommand disconnectCommand { get; set; }
        private List<Node> _nodeList;
        public List<Node> nodeList { get=> _nodeList; set { _nodeList = value;OnPropertyChanged(); } }

        private string _mqttBroker;
        public string mqttBroker { get=> _mqttBroker; set { _mqttBroker = value;OnPropertyChanged(); } }

        private string _mqttPort;
        public string mqttPort { get => _mqttPort; set { _mqttPort = value; OnPropertyChanged(); } }

        private bool _isConnected;
        public bool isConnected { get=>_isConnected; set { _isConnected = value;OnPropertyChanged(); } }

        private bool _isnotConnected;
        public bool isnotConnected { get => _isnotConnected; set { _isnotConnected = value; OnPropertyChanged(); } }

        MqttClient client = null;

        public string data = "[{\"nodeId\":2132934414,\"subs\":[{\"nodeId\":3256274256,\"subs\":[]}]}]";
        public MainViewModel()
        {
            OpenPanelCommand = new RelayCommand<object>((p) => { return true; }, (p) => { Button a = p as Button; ShowPanel(a); });
            connectCommand = new RelayCommand<object>((p) => { return true; }, (p) => {  connecToServer(); });
            disconnectCommand = new RelayCommand<object>((p) => { return true; }, (p) => {  disconnecToServer(); });

            nodeList = new List<Node>();
            isConnected = false ;
            isnotConnected = !isConnected;

            EventSystem.Subscribe<string>(StatusChanged);


        }


        void ShowPanel(Button a)
        {
            if(a!=null)
            {
                //NodePanel panel = new NodePanel();
                //var point = a.PointToScreen(Mouse.GetPosition(a));
                //panel.Left = point.X;
                //panel.Top = point.Y;
                //panel.ShowDialog();
            }
            
        }


        List<long> getNodeIds(string s)
        {
            List<long> temp = new List<long>();
            // Split on one or more non-digit characters.
            List<string> numbers = Regex.Split(s, @"\D+").ToList();
            foreach (string value in numbers)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    long i = long.Parse(value);
                    temp.Add(i);
                }
            }
            return temp;
        }

        void connecToServer()
        {
            int port;
            if (int.TryParse(mqttPort, out port))
            {
                client = new MqttClient(mqttBroker, port, false, null, null, 0, null, null);
                while(client.Connect("MeshServer", null, null) != 0)
                {
                    client.Connect("MeshServer", null, null);
                }
                client.Subscribe(new string[] { "HCMUT/MeshNetwork/ESP8266/#" },
                    new byte[] { uPLibrary.Networking.M2Mqtt.Messages.MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
                client.MqttMsgPublishReceived += MqttReceivedHandler;
                isConnected = true;
                isnotConnected = !isConnected;
                client.Publish(getNodesTopic, Encoding.UTF8.GetBytes("GetNodes"));
            }
            else MessageBox.Show("Kiểm tra lại thông tin server");
            
        }

        void disconnecToServer()
        {
            client.Disconnect();
            isConnected = false;
            isnotConnected = true;
        }

        private void MqttReceivedHandler(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            var topic = e.Topic;
            var message = Encoding.UTF8.GetString(e.Message);
            if(topic == fromEspTopic)
            {
                var result = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(message);
                if(nodeList!=null)
                {
                    foreach (var item in nodeList)
                    {
                        if (item.nodeID == result["Node"]) item.status = result["Status"];
                        EventSystem.Publish<Node>(item);
                    }
                }
                
            }
            else if(topic == nodesTopic)
            {
                var nodeListResult = getNodeIds(message);
                var tempNodeList = new List<Node>();
                if(nodeListResult!=null)
                {
                    foreach (var item in nodeListResult)
                    {
                        Node _temp = new Node();
                        _temp.nodeID = item;
                        tempNodeList.Add(_temp);
                    }
                    nodeList = tempNodeList;
                    EventSystem.Publish<List<Node>>(tempNodeList);


                }
            }
        }

        void StatusChanged(string msg)
        {
            var nodeID = long.Parse(msg.Split('/')[0]);
            var status = msg.Split('/')[1];
            string messageON = "{\"Node\":" + nodeID.ToString() + ",\"Status\":" + "1}";
            string messageOFF = "{\"Node\":" + nodeID.ToString() + ",\"Status\":" + "0}";

            switch (status)
            {
                case "ON":
                    {
                        client.Publish(fromServerTopic, Encoding.UTF8.GetBytes(messageON));
                        break;
                    }
                case "OFF":
                    {
                        client.Publish(fromServerTopic, Encoding.UTF8.GetBytes(messageOFF));
                        break;
                    }
                case "STOP":
                    {
                        client.Publish(nodesControlTopic, Encoding.UTF8.GetBytes(messageOFF));
                        break;
                    }

                   
            }
        }

    }
}
