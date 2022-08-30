using LocalBasis.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LocalBasis.Command
{
    public class TrackCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private InstanceViewModel InstanceViewModel;
        public TrackCommand(InstanceViewModel instanceViewModel) //пока сделаем только от инстанца
        {
            InstanceViewModel = instanceViewModel;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            InstanceViewModel.CustomInstance.SetNewGlobalCoords();
            InstanceViewModel.CustomInstance.SetNewLocalCoords();
            return;
        }
    }
}
