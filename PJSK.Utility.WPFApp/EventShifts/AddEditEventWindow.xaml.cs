using System;
using System.Collections.Generic;
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
using Xceed.Wpf.Toolkit;
using PJSK.Utility.EventShifts.Models;

namespace PJSK.Utility.WPFApp.EventShifts
{
    /// <summary>
    /// Interaction logic for AddEditEventWindow.xaml
    /// </summary>
    public partial class AddEditEventWindow : Window
    {
        public bool IsDataUpdated { get; set; }
        public Event CurrentEvent { get; set; }

        internal AddEditEventWindow(Event currentEvent)
        {
            InitializeComponent();
            InitializeData(currentEvent);
        }

        #region Load Methods
        private void InitializeData(Event currentEvent)
        {
            try
            {
                CurrentEvent = currentEvent;
                if (CurrentEvent.EventId == 0)
                {
                    Title = $"Add New Event";
                }
                else
                {
                    Title = $"Edit Event [{CurrentEvent.EventId}] - {CurrentEvent.EventName}";
                }
                DataContext = CurrentEvent;
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        #endregion Load Methods

        #region Event Methods
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowLoadingPanel();

                var worker = new BackgroundWorker();
                var workerArgument = new BackgroundWorkerArgument()
                {
                    CurrentEvent = CurrentEvent
                };
                worker.DoWork += (sender, e) =>
                {
                    var argument = (BackgroundWorkerArgument)e.Argument;
                    var result = new BackgroundWorkerResult();
                    result.HandleQueryResult(Data.EventShifts.EventHelper.UpsertEvent(argument.CurrentEvent, out var msg), msg);
                    e.Result = result;
                };
                worker.RunWorkerCompleted += (sender, e) =>
                {
                    var result = (BackgroundWorkerResult)e.Result;
                    if (result.IsSuccess)
                    {
                        this.IsDataUpdated = true;
                    }
                    UIHelper.DisplayMessage(result);

                    HideLoadingPanel();
                    this.Close();
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
