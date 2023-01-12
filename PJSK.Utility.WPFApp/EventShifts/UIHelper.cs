using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xceed.Wpf.Toolkit;

namespace PJSK.Utility.WPFApp.EventShifts
{
    internal static class UIHelper
    {
        internal static void DisplayMessage(BackgroundWorkerResult result)
        {
            if (result != null && !string.IsNullOrWhiteSpace(result.Message))
            {
                switch (result.LogLevel)
                {
                    case Common.Enums.LogLevelTypes.Info:
                        DisplayInfoMessage(result.Message);
                        break;
                    case Common.Enums.LogLevelTypes.Warn:
                        DisplayWarnMessage(result.Message);
                        break;
                    case Common.Enums.LogLevelTypes.Error:
                    case Common.Enums.LogLevelTypes.Fatal:
                        DisplayErrorMessage(result.Message);
                        break;
                    default:
                        DisplayMessage(result.Message);
                        break;
                }
            }
        }

        internal static void DisplayMessage(string msg)
        {
            if (!string.IsNullOrWhiteSpace(msg))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(msg, "", MessageBoxButton.OK, MessageBoxImage.None);
            }
        }
        internal static void DisplayInfoMessage(string msg)
        {
            if (!string.IsNullOrWhiteSpace(msg))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(msg, "INFORMATION", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        internal static void DisplayWarnMessage(string msg)
        {
            if (!string.IsNullOrWhiteSpace(msg))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(msg, "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        internal static void DisplayErrorMessage(string msg)
        {
            if (!string.IsNullOrWhiteSpace(msg))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(msg, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        internal static void DisplayErrorMessage(Exception ex)
        {
            if (ex != null)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
