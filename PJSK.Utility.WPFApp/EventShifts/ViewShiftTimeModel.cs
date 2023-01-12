using PJSK.Utility.EventShifts.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJSK.Utility.WPFApp.EventShifts
{
    public sealed class ViewShiftTimeModelBindingProxy : BindingProxy<ViewShiftTimeModel> { }
    public class ViewShiftTimeModel : BaseViewModel
    {
        public ObservableCollection<ShiftTableCell> AllShifts
        {
            get { return _allShifts; }
            set { if (value != _allShifts) { _allShifts = value; OnPropertyChanged(); } }
        }
        private ObservableCollection<ShiftTableCell> _allShifts = new ObservableCollection<ShiftTableCell>();
    }
}
