using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Mesh_Network_Manager.ViewModel
{
   public class NodePanelViewModel: BaseViewModel
    {
       

        #region COMMAND
        public ICommand MoveCommand { get; set; }
        public ICommand AcceptCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand CheckChangedCommand { get; set; }
        #endregion

        private long _nodeID;
        public long nodeID { get=> _nodeID; set { _nodeID = value;OnPropertyChanged(); } }

        private Brush _LedColor;
        public Brush LedColor { get=>_LedColor; set { _LedColor = value;OnPropertyChanged(); } }

        private bool _isLEDChecked;
        public bool isLEDChecked { get => _isLEDChecked; set { _isLEDChecked = value; OnPropertyChanged(); } }

        private bool _isStopChecked;
        public bool isStopChecked { get => _isStopChecked; set { _isStopChecked = value; OnPropertyChanged(); } }





        public NodePanelViewModel()
        {

            MoveCommand = new RelayCommand<object>((p) => { return true; }, (p) => { Window a = p as Window; a.DragMove(); });
            AcceptCommand = new RelayCommand<object>((p) => { return true; }, (p) => { Accept(p as Window); });
            CancelCommand = new RelayCommand<object>((p) => { return true; }, (p) => { Window a = p as Window; a.Close(); });
            CheckChangedCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CheckBox a = p as CheckBox; CheckFee(a); });
            isLEDChecked = isStopChecked = false;

        }

        void Accept(Window a)
        {
            if (isLEDChecked)
            {
                if (LedColor == Brushes.Green) EventSystem.Publish<string>(nodeID.ToString() +"/" + "OFF");
                else EventSystem.Publish<string>(nodeID.ToString() + "/"+"ON");
            }

            if (isStopChecked) EventSystem.Publish<string>(nodeID.ToString() + "/"+"STOP");
            a.Close();
        }

        void CheckFee(CheckBox a)
        {
            

        }
    }
}
