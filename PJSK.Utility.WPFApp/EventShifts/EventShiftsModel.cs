using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using PJSK.Utility.EventShifts.Models;

namespace PJSK.Utility.WPFApp.EventShifts
{
    public sealed class EventShiftsModelBindingProxy : BindingProxy<EventShiftsModel> { }
    public class EventShiftsModel : BaseViewModel
    {
        #region Properties

        public List<Event> AllEvents
        {
            get { return _allEvents; }
            set { if (value != _allEvents) { _allEvents = value; OnPropertyChanged(); } }
        }
        private List<Event> _allEvents = new List<Event>();

        public Event CurrentEvent
        {
            get { return _currentEvent; }
            set { if (value != _currentEvent) { _currentEvent = value; OnPropertyChanged(); } }
        }
        private Event _currentEvent = new Event();

        public string EventCountdown
        {
            get { return _eventCountdown; }
            set { if (value != _eventCountdown) { _eventCountdown = value; OnPropertyChanged(); } }
        }
        private string _eventCountdown = default;

        public bool IsCurrentEventLoaded
        {
            get { return _isCurrentEventLoaded; }
            set { if (value != _isCurrentEventLoaded) { _isCurrentEventLoaded = value; OnPropertyChanged(); } }
        }
        private bool _isCurrentEventLoaded = default;

        public string ToBeParsedText
        {
            get { return _toBeParsedText; }
            set { if (value != _toBeParsedText) { _toBeParsedText = value; OnPropertyChanged(); } }
        }
        private string _toBeParsedText = default;

        public string ToBeSearchedPlayerName
        {
            get { return _toBeSearchedPlayerName; }
            set { if (value != _toBeSearchedPlayerName) { _toBeSearchedPlayerName = value; OnPropertyChanged(); } }
        }
        private string _toBeSearchedPlayerName = default;

        public List<Player> AllPlayers
        {
            get { return _allPlayers; }
            set { if (value != _allPlayers) { _allPlayers = value; OnPropertyChanged(); } }
        }
        private List<Player> _allPlayers = new List<Player>();

        public DateTime? ShiftDate
        {
            get { return _shiftDate; }
            set { if (value != _shiftDate) { _shiftDate = value; OnPropertyChanged(); } }
        }
        private DateTime? _shiftDate = default;

        public int ShiftTimeStartHour
        {
            get { return _shiftTimeStartHour; }
            set { if (value != _shiftTimeStartHour) { _shiftTimeStartHour = value; OnPropertyChanged(); } }
        }
        private int _shiftTimeStartHour = default;

        public ObservableCollection<ShiftTableRow> ShiftTableRows
        {
            get { return _shiftTableRows; }
            set { if (value != _shiftTableRows) { _shiftTableRows = value; OnPropertyChanged(); } }
        }
        private ObservableCollection<ShiftTableRow> _shiftTableRows = new ObservableCollection<ShiftTableRow>();

        public bool IsViewOrExportAllShifts
        {
            get { return _isViewOrExportAllShifts; }
            set { if (value != _isViewOrExportAllShifts) { _isViewOrExportAllShifts = value; OnPropertyChanged(); } }
        }
        private bool _isViewOrExportAllShifts = default;

        #endregion Properties

        public EventShiftsModel()
        {
            
        }
    }
}
