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
using System.Windows.Threading;
using PJSK.Utility.EventShifts;
using PJSK.Utility.EventShifts.Models;

namespace PJSK.Utility.WPFApp.EventShifts
{
    /// <summary>
    /// Interaction logic for EventShiftsWindow.xaml
    /// </summary>
    public partial class EventShiftsWindow : Window
    {
        public EventShiftsModel ViewModel { get; set; }
        private DispatcherTimer _eventCountdownTimer { get; set; }

        public EventShiftsWindow()
        {
            InitializeComponent();
            ViewModel = new EventShiftsModel();
            DataContext = ViewModel;
            LoadEventComboBox();
        }

        #region Load Methods

        private void LoadEventComboBox()
        {
            try
            {
                ShowLoadingPanel();
                ViewModel.AllEvents = null;

                var worker = new BackgroundWorker();
                worker.DoWork += (sender, e) =>
                {
                    var result = new BackgroundWorkerResult();
                    result.HandleQueryResult(Data.EventShifts.EventHelper.GetEvents(out var events, out var msg), msg);
                    if (result.IsSuccess)
                    {
                        result.Events = events.ConvertAll(x => Event.ConvertDAL(x));
                    }
                    e.Result = result;
                };
                worker.RunWorkerCompleted += (sender, e) =>
                {
                    var result = (BackgroundWorkerResult)e.Result;
                    if (result.IsSuccess)
                    {
                        ViewModel.AllEvents = result.Events;
                    }
                    UIHelper.DisplayMessage(result);

                    ResetEventData();

                    HideLoadingPanel();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }


        private void ResetEventData()
        {
            ViewModel.CurrentEvent = new Event();
            ViewModel.IsCurrentEventLoaded = false;
            ViewModel.ToBeSearchedPlayerName = string.Empty;
            ViewModel.AllPlayers = new List<Player>();
        }

        private void LoadEventData(Event currentEvent)
        {
            try
            {
                ViewModel.CurrentEvent = currentEvent;
                if (ViewModel.CurrentEvent != null)
                {
                    if (ViewModel.CurrentEvent.EventEndDate_UTCDateTime != null)
                    {
                        LoadCountdownTimer();
                    }

                    LoadPlayerDataGrid();

                    ViewModel.IsCurrentEventLoaded = true;
                }
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        private void LoadCountdownTimer()
        {
            try
            {
                _eventCountdownTimer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
                  {
                      var nowUTC = DateTime.UtcNow;
                      var endDateUTC = ViewModel.CurrentEvent.EventEndDate_UTCDateTime;
                      if (endDateUTC != null && nowUTC < endDateUTC.Value)
                      {
                          ViewModel.EventCountdown = $"{(endDateUTC.Value - nowUTC).ToString(@"d\d\ h\h\ m\m\ s\s")}";
                      }
                      else if (endDateUTC == null)
                      {
                          ViewModel.EventCountdown = null;
                      }
                      else
                      {
                          ViewModel.EventCountdown = $"{(nowUTC - nowUTC).ToString(@"d\d\ h\h\ m\m\ s\s")}";
                      }
                  }, Application.Current.Dispatcher);
                _eventCountdownTimer.Start();
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        private void LoadPlayerDataGrid(string toBeSearchedPlayerName = "")
        {
            try
            {
                ShowLoadingPanel();
                ViewModel.AllPlayers = null;

                var worker = new BackgroundWorker();
                var workerArgument = new BackgroundWorkerArgument()
                {
                    EventId = ViewModel.CurrentEvent.EventId,
                    PlayerName = toBeSearchedPlayerName,
                };
                worker.DoWork += (sender, e) =>
                {
                    var argument = (BackgroundWorkerArgument)e.Argument;
                    var result = new BackgroundWorkerResult();

                    if (string.IsNullOrWhiteSpace(workerArgument.PlayerName))
                    {
                        result.HandleQueryResult(Data.EventShifts.PlayerHelper.GetPlayers(argument.EventId, out var playersDAL, out var msg), msg);
                        if (result.IsSuccess)
                        {
                            result.Players = playersDAL.ConvertAll(x => Player.ConvertDAL(x));
                        }
                    }
                    else
                    {
                        result.HandleQueryResult(Data.EventShifts.PlayerHelper.GetPlayersByName(argument.EventId, argument.PlayerName, out var playersDAL, out var msg), msg);
                        if (result.IsSuccess)
                        {
                            result.Players = playersDAL.ConvertAll(x => Player.ConvertDAL(x));
                        }
                    }

                    //if (ViewModel.ShiftDate != null)
                    //{
                    //    result.HandleQueryResult(Data.EventShifts.ShiftHelper.GetShiftsByEventIdAndDate(argument.EventId, ViewModel.ShiftDate.Value.ToString(Common.Constants.DateTime.DateTimePatterns.Date), out var shiftsDAL, out var msg), msg);
                    //    if (result.IsSuccess)
                    //    {
                    //        var shifts = shiftsDAL.ConvertAll(x => Shift.ConvertDAL(x));
                    //        result.Players.ForEach(x => x.SetShiftCount(shifts));
                    //    }
                    //}

                    e.Result = result;
                };
                worker.RunWorkerCompleted += (sender, e) =>
                {
                    var result = (BackgroundWorkerResult)e.Result;
                    if (result.IsSuccess)
                    {
                        ViewModel.AllPlayers = result.Players;
                    }
                    UIHelper.DisplayMessage(result);

                    HideLoadingPanel();
                };
                worker.RunWorkerAsync(workerArgument);
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        private void LoadShiftTable(ShiftTable shiftTable, int shiftTimeStartHour)
        {
            try
            {
                ViewModel.ShiftTableRows = new ObservableCollection<ShiftTableRow>();
                var startIndex = 0; // Math.Max(Math.Min(shiftTimeStartHour,18),0);
                var endIndex = 24; // shiftTimeStartHour+6;
                for (var i=startIndex; i<endIndex; i++)
                {
                    ViewModel.ShiftTableRows.Add(shiftTable.ShiftTableDict[PJSK.Utility.EventShifts.Constants.ShiftTimeMap.Forward[i]]);
                }
                HideLoadingPanel();
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        #endregion Load Methods

        #region Event Methods
        private void ddlEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ShowLoadingPanel();
                ResetEventData();

                var ddl = sender as ComboBox;

                if (ddl.SelectedIndex != -1)
                {
                    var worker = new BackgroundWorker();
                    var workerArgument = new BackgroundWorkerArgument()
                    {
                        EventId = (ddl.SelectedItem as Event).EventId
                    };
                    worker.DoWork += (sender, e) =>
                    {
                        var argument = (BackgroundWorkerArgument)e.Argument;
                        var result = new BackgroundWorkerResult() { TreatDataNotFoundAsFailed = true };
                        result.HandleQueryResult(Data.EventShifts.EventHelper.GetEventById(argument.EventId, out var eventDAL, out var msg), msg);
                        if (result.IsSuccess)
                        {
                            result.CurrentEvent = Event.ConvertDAL(eventDAL);
                        }
                        e.Result = result;
                    };
                    worker.RunWorkerCompleted += (sender, e) =>
                    {
                        var result = (BackgroundWorkerResult)e.Result;
                        if (result.IsSuccess)
                        {
                            LoadEventData(result.CurrentEvent);
                            ViewModel.IsCurrentEventLoaded = true;
                        }
                        UIHelper.DisplayMessage(result);
                    };
                    worker.RunWorkerAsync(workerArgument);
                }
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        private void btnAddEvent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new AddEditEventWindow(new Event());
                window.Owner = this;
                window.ShowDialog();
                if (window.IsDataUpdated)
                {
                    LoadEventComboBox();
                    ResetEventData();
                }
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }

        }

        private void btnEditEvent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new AddEditEventWindow(ViewModel.CurrentEvent.ShallowCopy());
                window.Owner = this;
                window.ShowDialog();
                if (window.IsDataUpdated)
                {
                    LoadEventComboBox();
                    ResetEventData();
                }
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        private void btnAddPlayer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parser = new DiscordMessageParser(ViewModel.ToBeParsedText);
                ViewModel.ToBeSearchedPlayerName = parser.GetPlayerName();

                var window = new AddEditPlayerWindow(new Player() { EventId = ViewModel.CurrentEvent.EventId }, ViewModel.ToBeParsedText);
                window.Owner = this;
                window.ShowDialog();
                if (window.IsDataUpdated)
                {
                    LoadPlayerDataGrid();
                }
                ViewModel.ToBeParsedText = string.Empty;
                ViewModel.ToBeSearchedPlayerName = string.Empty;
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        private void txtSearchPlayerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var txt = sender as TextBox;
                LoadPlayerDataGrid(txt.Text);
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        private void btnClearSearchPlayerName_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.ToBeSearchedPlayerName = string.Empty;
                LoadPlayerDataGrid(ViewModel.ToBeSearchedPlayerName);
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        private void btnParseData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parser = new DiscordMessageParser(ViewModel.ToBeParsedText);
                ViewModel.ToBeSearchedPlayerName = parser.GetPlayerName();
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        private void btnClearParseData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.ToBeParsedText = string.Empty;
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        private void dgPlayers_DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var row = sender as DataGridRow;
                var currentPlayer = row.DataContext as Player;
                var window = new AddEditPlayerWindow(currentPlayer.ShallowCopy(), ViewModel.ToBeParsedText);
                window.Owner = this;
                window.ShowDialog();
                if (window.IsDataUpdated)
                {
                    if (GetShiftTable(out var shiftTable))
                    {
                        LoadShiftTable(shiftTable, ViewModel.ShiftTimeStartHour);
                    }
                    LoadPlayerDataGrid();
                    ViewModel.ToBeParsedText = string.Empty;
                    ViewModel.ToBeSearchedPlayerName = string.Empty;
                }
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        private void dgPlayers_DataGridRow_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var row = sender as DataGridRow;
                var currentPlayer = row.DataContext as Player;
                var window = new AddEditShiftWindow(ViewModel.CurrentEvent, currentPlayer, ViewModel.ShiftDate, ViewModel.ToBeParsedText);
                window.Owner = this;
                window.ShowDialog();
                if (window.IsDataUpdated)
                {
                    if (GetShiftTable(out var shiftTable))
                    {
                        LoadShiftTable(shiftTable, ViewModel.ShiftTimeStartHour);
                    }
                    LoadPlayerDataGrid();
                    ViewModel.ToBeParsedText = string.Empty;
                    ViewModel.ToBeSearchedPlayerName = string.Empty;
                }
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        private void dtShiftDate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                var dtp = sender as Xceed.Wpf.Toolkit.DateTimePicker;
                if (dtp.Value != null && GetShiftTable(out var shiftTable, dtp.Value))
                {
                    LoadShiftTable(shiftTable, ViewModel.ShiftTimeStartHour);
                }
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        private void upShiftTimeStartHour_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                var up = sender as Xceed.Wpf.Toolkit.IntegerUpDown;
                //if (up.Value != null && GetShiftTable(out var shiftTable))
                //{
                //    LoadShiftTable(shiftTable, up.Value.Value);
                //}
                if (up.Value != null)
                {
                    svShiftTable.ScrollToVerticalOffset(((ItemsControl)svShiftTable.Content).ActualHeight * (up.Value.Value / 24.0));
                }
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        private void btnGenerateShiftTable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel.ShiftDate != null
                    && MessageBox.Show($"This will overwrite current saved data for {ViewModel.ShiftDate.Value.ToString(Common.Constants.DateTime.DateTimePatterns.DateTime)}.", 
                        "CONFIRM", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    if (GetShiftTable(out var shiftTable))
                    {
                        shiftTable.AutoGenerateShiftTable();
                        shiftTable.SaveToDB();
                        LoadShiftTable(shiftTable, ViewModel.ShiftTimeStartHour);
                    }
                }

            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        private void btnExportShiftTable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (GetShiftTable(out var shiftTable))
                {
                    if (ViewModel.IsViewOrExportAllShifts)
                    {
                        var window = new ViewShiftTableDetailWindow(shiftTable, ViewModel.ShiftDate, true);
                        window.Owner = this;
                        HideLoadingPanel();
                        window.Show();
                    }
                    else
                    {
                        var window = new ViewShiftTableWindow(shiftTable, ViewModel.ShiftDate, true);
                        window.Owner = this;
                        HideLoadingPanel();
                        window.Show();
                    }
                }
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        private void btnViewShiftTable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (GetShiftTable(out var shiftTable))
                {
                    if (ViewModel.IsViewOrExportAllShifts)
                    {
                        var window = new ViewShiftTableDetailWindow(shiftTable, ViewModel.ShiftDate, false);
                        window.Owner = this;
                        HideLoadingPanel();
                        window.Show();
                    }
                    else
                    {
                        var window = new ViewShiftTableWindow(shiftTable, ViewModel.ShiftDate, false);
                        window.Owner = this;
                        HideLoadingPanel();
                        window.Show();
                    }
                }
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        private void ShiftTableDetailRow_ViewShiftTimeWindowClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var row = sender as Controls.ShiftTableDetailRow;
                var window = new ViewShiftTimeWindow(row.CurrentShiftTableRow.AllShifts.Select(x => x.DeepCopy()).ToList());
                window.Owner = this;
                window.ShowDialog();
                if (window.IsDataUpdated)
                {
                    UIHelper.DisplayInfoMessage("SAVED!");

                    if (GetShiftTable(out var shiftTable))
                    {
                        LoadShiftTable(shiftTable, ViewModel.ShiftTimeStartHour);
                    }
                }
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

        private bool GetShiftTable(out ShiftTable shiftTable, DateTime? shiftDate = null)
        {
            shiftTable = null;
            shiftDate = shiftDate ?? ViewModel.ShiftDate;
            if (shiftDate == null) { return false; }

            ShowLoadingPanel();
            shiftTable = new ShiftTable(ViewModel.CurrentEvent.EventId, shiftDate.Value.ToString(Common.Constants.DateTime.DateTimePatterns.Date));
            return true;
        }

        #endregion Helper Methods
    }
}
