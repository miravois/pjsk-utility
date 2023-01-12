using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using PJSK.Utility.EventShifts;
using PJSK.Utility.EventShifts.Models;

namespace PJSK.Utility.WPFApp.EventShifts
{
    /// <summary>
    /// Interaction logic for AddEditShiftWindow.xaml
    /// </summary>
    public partial class AddEditShiftWindow : Window
    {
        public bool IsDataUpdated { get; set; }
        public AddEditShiftModel ViewModel { get; set; }
        public DiscordMessageParser DiscordMessageParser { get; set; }

        internal AddEditShiftWindow(Event currentEvent, Player currentPlayer, DateTime? shiftDate, string toBeParsedText)
        {
            InitializeComponent();
            InitializeData(currentEvent, currentPlayer, shiftDate, toBeParsedText);
        }

        #region Load Methods

        private void InitializeData(Event currentEvent, Player currentPlayer, DateTime? shiftDate, string toBeParsedText)
        {
            ViewModel = new AddEditShiftModel(currentEvent, currentPlayer);

            DiscordMessageParser = new DiscordMessageParser(toBeParsedText);
            ViewModel.CurrentShift.AddNotesAbove($"{DiscordMessageParser.OriginalMessage}");
            ViewModel.CurrentShift.ShiftDataList = DiscordMessageParser.GetShiftDataList(ViewModel.CurrentShift);
            ViewModel.ShiftDataList = new ObservableCollection<ShiftData>(ViewModel.CurrentShift.ShiftDataList);

            ViewModel.ShiftDate = shiftDate;

            DataContext = ViewModel;
        }

        #endregion Load Methods

        #region Event Methods

        private void dtpShiftDate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                ShowLoadingPanel();

                var dtp = sender as Xceed.Wpf.Toolkit.DateTimePicker;
                var shiftDate = dtp.Value?.ToString(Common.Constants.DateTime.DateTimePatterns.Date);
                ViewModel.CurrentShift.ShiftDate = shiftDate;

                if (dtp.Value == null)
                {
                    ViewModel.CanSave = false;
                    return;
                }

                var worker = new BackgroundWorker();
                var workerArgument = new BackgroundWorkerArgument()
                {
                    ShiftDate = shiftDate,
                    CurrentPlayer = ViewModel.CurrentPlayer,
                };
                worker.DoWork += (sender, e) =>
                {
                    var argument = (BackgroundWorkerArgument)e.Argument;
                    var result = new BackgroundWorkerResult() { Shifts = new List<Shift>() };
                    result.HandleQueryResult(Data.EventShifts.ShiftHelper.GetShiftsByPlayerId(argument.CurrentPlayer.PlayerId, out var shiftsDAL, out var msg), msg);
                    if (result.IsSuccess)
                    {
                        result.Shifts = shiftsDAL.ConvertAll(x => Shift.ConvertDAL(x));
                        result.CurrentShift = result.Shifts.SingleOrDefault(x => x.ShiftDate == argument.ShiftDate);
                        result.Status = result.CurrentShift != null ? Common.Enums.StatusTypes.Success : Common.Enums.StatusTypes.Failed;

                        if (result.CurrentShift == null)
                        {
                            result.CurrentShift = ViewModel.CurrentShift;
                            result.CurrentShift.ShiftId = 0;
                            result.CurrentShift.CanEncore = false;
                            result.CurrentShift.CanStandby = false;

                            ViewModel.CurrentShift.ShiftDataList = new List<ShiftData>();
                            ViewModel.ShiftDataList = new ObservableCollection<ShiftData>();
                        }

                        if (DiscordMessageParser.IsInUse)
                        {
                            result.CurrentShift.AddNotesAbove($"{DiscordMessageParser.OriginalMessage}");
                            result.CurrentShift.ShiftDataList = DiscordMessageParser.GetShiftDataList(result.CurrentShift);
                        }
                    }
                    
                    e.Result = result;
                };
                worker.RunWorkerCompleted += (sender, e) =>
                {
                    var result = (BackgroundWorkerResult)e.Result;
                    ViewModel.CurrentShift = result.CurrentShift;
                    UIHelper.DisplayMessage(result);

                    ViewModel.ShiftDataList = new ObservableCollection<ShiftData>(ViewModel.CurrentShift.ShiftDataList);
                    ViewModel.TodayActualShiftCount = ViewModel.CurrentPlayer.GetTodayActualShiftCount(ViewModel.CurrentShift);
                    ViewModel.TotalActualShiftCount = ViewModel.CurrentPlayer.GetTotalActualShiftCount(result.Shifts);
                    ViewModel.CanSave = true;

                    HideLoadingPanel();
                };
                worker.RunWorkerAsync(workerArgument);
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowLoadingPanel();

                ViewModel.CanSave = false;
                ViewModel.CurrentShift.ShiftDate = ViewModel.ShiftDate.Value.ToString(Common.Constants.DateTime.DateTimePatterns.Date);
                ViewModel.CurrentShift.ShiftDataList = ViewModel.ShiftDataList.ToList();

                var worker = new BackgroundWorker();
                var workerArgument = new BackgroundWorkerArgument()
                {
                    TagString = ((Button)sender).Tag.ToString(),
                    CurrentShift = ViewModel.CurrentShift
                };
                worker.DoWork += (sender, e) =>
                {
                    var argument = (BackgroundWorkerArgument)e.Argument;
                    var result = new BackgroundWorkerResult() { TagString = argument.TagString };
                    result.HandleQueryResult(Data.EventShifts.ShiftHelper.UpsertShift(argument.CurrentShift, out var msg), msg);
                    e.Result = result;
                };
                worker.RunWorkerCompleted += (sender, e) =>
                {
                    var result = (BackgroundWorkerResult)e.Result;
                    if (result.IsSuccess)
                    {
                        IsDataUpdated = true;

                        if (result.TagString == "SaveAndClose") { this.Close(); }
                    }
                    UIHelper.DisplayMessage(result);
                    ViewModel.CanSave = true;

                    HideLoadingPanel();
                };
                worker.RunWorkerAsync(workerArgument);
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        #endregion Event Methods

        #region Helper Methods

        private void ShowLoadingPanel()
        {
            ucLoadingPanel.Visibility = Visibility.Visible;
        }

        private void HideLoadingPanel()
        {
            ucLoadingPanel.Visibility = Visibility.Collapsed;
        }

        #endregion Helper Methods
    }
}
