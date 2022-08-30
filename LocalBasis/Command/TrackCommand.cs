using LocalBasis.Model;
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

        private InstanceViewModel ViewModel;
        public TrackCommand(InstanceViewModel vm) //пока сделаем только от инстанца
        {
            ViewModel = vm;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            ViewModel.CustomInstance.SetNewGlobalCoords();
            ViewModel.GlobalText = ViewModel.CustomInstance.GlobalText;
            return;
        }
    }
}
