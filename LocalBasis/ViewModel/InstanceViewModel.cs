﻿using LocalBasis.Command;
using LocalBasis.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LocalBasis.ViewModel
{
    public class InstanceViewModel : INotifyPropertyChanged
    {
        private string globalText;
        private string localText;
        public TrackCommand trackCommand { get; set; }
        public CustomInstance CustomInstance;

        public event PropertyChangedEventHandler PropertyChanged;
        public string GlobalText
        {
            get
            {
                return globalText;
            }
            set
            {
                globalText = value;
                OnPropertyChanged("GlobalText");
            }
        }
        public string LocalText
        {
            get
            {
                return localText;
            }
            set
            {
               localText = value;
               OnPropertyChanged("LocalText");
            }
        }
        public InstanceViewModel()
        {
            trackCommand = new TrackCommand(this);
        }


        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}
