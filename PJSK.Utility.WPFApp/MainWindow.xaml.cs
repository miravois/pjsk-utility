using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PJSK.Utility.WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static EventShifts.EventShiftsWindow _eventShiftsWindow = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGetAverageMissedNotes_Click(object sender, RoutedEventArgs e)
        {
            int totalCount = 0;
            double totalGood = 0, totalBad = 0, totalMiss = 0;
            double last5Good = 0, last5Bad = 0, last5Miss = 0;

            for (var i=tbMissedNotesData.LineCount-1; i>=0; i--)
            {
                var lineData = tbMissedNotesData.GetLineText(i).Replace(Environment.NewLine, string.Empty);
                if (!string.IsNullOrWhiteSpace(lineData) && lineData.Contains("-"))
                {
                    if (!Regex.IsMatch(lineData, @"^\d+-\d+-\d+$"))
                    {
                        lblStatusBar.Content = @"Wrong Format (^\d+-\d+-\d+$)!";
                        return;
                    }
                    else
                    {
                        var splittedData = lineData.Split('-');
                        var lineGood = int.Parse(splittedData[0]);
                        var lineBad = int.Parse(splittedData[1]);
                        var lineMiss = int.Parse(splittedData[2]);

                        totalGood += lineGood;
                        totalBad += lineBad;
                        totalMiss += lineMiss;
                        totalCount++;

                        if (totalCount <= 5)
                        {
                            last5Good += lineGood;
                            last5Bad += lineBad;
                            last5Miss += lineMiss;
                        }
                    }
                }
            }

            double averageTotalGood = totalGood / totalCount;
            double averageTotalBad = totalBad / totalCount;
            double averageTotalMiss = totalMiss / totalCount;
            double averageTotalMissedNotes = (totalGood + totalBad + totalMiss) / totalCount;
            double averageLast5Good = last5Good / Math.Min(5, totalCount);
            double averageLast5Bad = last5Bad / Math.Min(5, totalCount);
            double averageLast5Miss = last5Miss / Math.Min(5, totalCount);
            double averageLast5MissedNotes = (last5Good + last5Bad + last5Miss) / Math.Min(5, totalCount);

            lblAverageMissedNotesData.Content = $"{averageTotalMissedNotes:0.00}"
                + $"{Environment.NewLine}{averageLast5MissedNotes:0.00}";
            lblFullMissedNotesData.Content = $"{averageTotalMissedNotes:0.00}\t{averageTotalGood:0.00}-{averageTotalBad:0.00}-{averageTotalMiss:0.00}"
                + $"{Environment.NewLine}{averageLast5MissedNotes:0.00}\t{averageLast5Good:0.00}-{averageLast5Bad:0.00}-{averageLast5Miss:0.00}";
            lblStatusBar.Content = "Calculated Data!";
        }

        private void btnCopyAverageMissedNotesData_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText((string)lblAverageMissedNotesData.Content);
            lblStatusBar.Content = "Copied Average Value!";
        }

        private void btnCopyFullMissedNotesData_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText((string)lblFullMissedNotesData.Content);
            lblStatusBar.Content = "Copied Full Data!";
        }

        private void lblEventShifts_Click(object sender, MouseButtonEventArgs e)
        {
            ShowLoadingPanel();
            if (_eventShiftsWindow == null)
            {
                _eventShiftsWindow = new EventShifts.EventShiftsWindow();
                _eventShiftsWindow.Closed += (sender, e) => _eventShiftsWindow = null;
            }
            _eventShiftsWindow.Show();
            HideLoadingPanel();
        }

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
