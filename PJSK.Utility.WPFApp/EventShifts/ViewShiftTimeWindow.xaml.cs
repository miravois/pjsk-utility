using PJSK.Utility.EventShifts.Models;
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

namespace PJSK.Utility.WPFApp.EventShifts
{
    /// <summary>
    /// Interaction logic for ViewShiftTimeWindow.xaml
    /// </summary>
    public partial class ViewShiftTimeWindow : Window
    {
        public bool IsDataUpdated { get; set; }
        public ViewShiftTimeModel ViewModel { get; set; }

        public ViewShiftTimeWindow(List<ShiftTableCell> allShifts)
        {
            InitializeComponent();
            ViewModel = new ViewShiftTimeModel();
            ViewModel.AllShifts = new ObservableCollection<ShiftTableCell>(allShifts);
            DataContext = ViewModel;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((Button)sender).Tag.ToString() == "OptimizeAndSave")
                {
                    ViewModel.AllShifts = new ObservableCollection<ShiftTableCell>(ShiftTableCell.SetOptimalPosition(ViewModel.AllShifts.ToList()));
                }

                foreach (var shift in ViewModel.AllShifts)
                {
                    shift.UpdateParentShiftWithCurrentShiftData();

                    var worker = new BackgroundWorker();
                    var workerArgument = new BackgroundWorkerArgument()
                    {
                        CurrentShift = shift.ParentShift
                    };
                    worker.DoWork += (sender, e) =>
                    {
                        var argument = (BackgroundWorkerArgument)e.Argument;
                        var result = new BackgroundWorkerResult();
                        result.HandleQueryResult(Data.EventShifts.ShiftHelper.UpsertShift(argument.CurrentShift, out var msg), msg);
                        e.Result = result;
                    };
                    worker.RunWorkerCompleted += (sender, e) =>
                    {
                        var result = (BackgroundWorkerResult)e.Result;
                        if (result.IsSuccess)
                        {
                            IsDataUpdated = true;
                            this.Close();
                        }
                        UIHelper.DisplayMessage(result);
                    };
                    worker.RunWorkerAsync(workerArgument);
                }
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var row = sender as ListViewItem;
                var currentShift = row.DataContext as ShiftTableCell;
                var window = new AddEditPlayerWindow(currentShift.CurrentPlayer.ShallowCopy(), "");
                window.Owner = this;
                window.ShowDialog();
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        private void ListViewItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var row = sender as ListViewItem;
                var currentShift = row.DataContext as ShiftTableCell;
                var window = new AddEditShiftWindow(currentShift.CurrentEvent, currentShift.CurrentPlayer, DateTime.Parse(currentShift.ParentShift.ShiftDate), "");
                window.Owner = this;
                window.ShowDialog();
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }
    }
}
