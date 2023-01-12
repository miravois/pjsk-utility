using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PJSK.Utility.EventShifts.Models;

namespace PJSK.Utility.WPFApp.EventShifts
{
    public sealed class AddEditShiftModelBindingProxy : BindingProxy<AddEditShiftModel> { }
    public class AddEditShiftModel : BaseViewModel
    {
        #region Properties

        public DateTime? ShiftDate
        {
            get { return _shiftDate; }
            set { if (value != _shiftDate) { _shiftDate = value; OnPropertyChanged(); } }
        }
        private DateTime? _shiftDate = default;

        public Event CurrentEvent
        {
            get { return _currentEvent; }
            set { if (value != _currentEvent) { _currentEvent = value; OnPropertyChanged(); } }
        }
        private Event _currentEvent = new Event();

        public Player CurrentPlayer
        {
            get { return _currentPlayer; }
            set { if (value != _currentPlayer) { _currentPlayer = value; OnPropertyChanged(); } }
        }
        private Player _currentPlayer = new Player();

        public Shift CurrentShift
        {
            get { return _currentShift; }
            set { if (value != _currentShift) { _currentShift = value; OnPropertyChanged(); } }
        }
        private Shift _currentShift = new Shift();

        public ObservableCollection<ShiftData> ShiftDataList
        {
            get { return _shiftDataList; }
            set { if (value != _shiftDataList) {  _shiftDataList = value; OnPropertyChanged(); } }
        }
        private ObservableCollection<ShiftData> _shiftDataList = new ObservableCollection<ShiftData>();

        public ObservableCollection<ShiftData> SelectedShiftDataList
        {
            get { return _selectedshiftDataList; }
            set { if (value != _selectedshiftDataList) { _selectedshiftDataList = value; OnPropertyChanged(); } }
        }
        private ObservableCollection<ShiftData> _selectedshiftDataList = new ObservableCollection<ShiftData>();

        public int TodayActualShiftCount
        {
            get { return _todayActualShiftCount; }
            set { if (value != _todayActualShiftCount) { _todayActualShiftCount = value; OnPropertyChanged(); } }
        }
        private int _todayActualShiftCount = default;

        public int TotalActualShiftCount
        {
            get { return _totalActualShiftCount; }
            set { if (value != _totalActualShiftCount) { _totalActualShiftCount = value; OnPropertyChanged(); } }
        }
        private int _totalActualShiftCount = default;

        public bool CanSave
        {
            get { return _canSave; }
            set { if (value != _canSave) { _canSave = value; OnPropertyChanged(); } }
        }
        private bool _canSave = default;

        #endregion Properties

        #region Constructors
        public AddEditShiftModel(Event currentEvent, Player currentPlayer)
        {
            ShiftDate = null;
            CurrentEvent = currentEvent;
            CurrentPlayer = currentPlayer;
            CurrentShift = new Shift()
            {
                EventId = CurrentEvent.EventId,
                PlayerId = CurrentPlayer.PlayerId,
            };
        }
        #endregion Constructors

        #region Methods

        #endregion Methods
    }
}
