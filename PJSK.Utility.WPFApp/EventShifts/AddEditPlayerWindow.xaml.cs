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
using PJSK.Utility.EventShifts;
using PJSK.Utility.EventShifts.Models;

namespace PJSK.Utility.WPFApp.EventShifts
{
    /// <summary>
    /// Interaction logic for AddEditPlayerWindow.xaml
    /// </summary>
    public partial class AddEditPlayerWindow : Window
    {
        public bool IsDataUpdated { get; set; }
        public Player CurrentPlayer { get; set; }
        public DiscordMessageParser DiscordMessageParser { get; set; }

        internal AddEditPlayerWindow(Player currentPlayer, string toBeParsedText)
        {
            InitializeComponent();
            InitializeData(currentPlayer, toBeParsedText);
        }

        #region Load Methods
        private void InitializeData(Player currentPlayer, string toBeParsedText)
        {
            try
            {
                CurrentPlayer = currentPlayer;
                DiscordMessageParser = new DiscordMessageParser(toBeParsedText);

                if (CurrentPlayer.PlayerId == 0)
                {
                    Title = $"Add New Player";
                }
                else
                {
                    Title = $"Edit Player {CurrentPlayer.PlayerName} for Event [{CurrentPlayer.EventId}]";
                }

                // add this message into Notes
                CurrentPlayer.AddNotesAbove($"{DiscordMessageParser.OriginalMessage}");

                // replace or add name
                var playerName = DiscordMessageParser.GetPlayerName();
                if (string.IsNullOrWhiteSpace(CurrentPlayer.PlayerName))
                {
                    CurrentPlayer.PlayerName = playerName;
                }
                else if (!CurrentPlayer.PlayerOtherNameList.Contains(playerName))
                {
                    CurrentPlayer.PlayerOtherNameList.Add(playerName);
                }

                // replace player data if necessary
                DiscordMessageParser.GetPlayerData(out var leaderSkill, out var internalValue, out long totalPower, out var encoreLeaderSkill, out var encoreInternalValue, out long encoreTotalPower);
                if (leaderSkill >= CurrentPlayer.PlayerLeaderSkill && internalValue >= CurrentPlayer.PlayerInternalValue)
                {
                    if (CurrentPlayer.PlayerData != "0/0/0")
                    {
                        CurrentPlayer.AddNotesAbove($"Data Replaced: {CurrentPlayer.PlayerData}");
                    }
                    CurrentPlayer.PlayerLeaderSkill = leaderSkill;
                    CurrentPlayer.PlayerInternalValue = internalValue;
                    CurrentPlayer.PlayerTotalPower = totalPower;
                }
                if (encoreLeaderSkill >= CurrentPlayer.EncorePlayerLeaderSkill && encoreTotalPower >= CurrentPlayer.EncorePlayerTotalPower)
                {
                    if (CurrentPlayer.EncorePlayerData != "0/0/0")
                    {
                        CurrentPlayer.AddNotesAbove($"Encore Data Replaced: {CurrentPlayer.EncorePlayerData}");
                    }
                    CurrentPlayer.EncorePlayerLeaderSkill = encoreLeaderSkill;
                    CurrentPlayer.EncorePlayerInternalValue = encoreInternalValue;
                    CurrentPlayer.EncorePlayerTotalPower = encoreTotalPower;
                }

                DataContext = CurrentPlayer;
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        #endregion Load Methods

        #region Event Methods

        private void txtBgColorHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = sender as TextBox;
            try
            {
                txt.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(txt.Text));
                CurrentPlayer.BgColorHex = txt.Text;
            }
            catch { }
        }

        private void btnGenerateRandomColor_Click(object sender, RoutedEventArgs e)
        {
            var random = new Random();
            double hue = random.Next(0, 359);
            double saturation = random.Next(40, 80) / 100.0;
            double brightness = 0; 
            
            if (saturation >=0.4 && saturation <= 0.55)
            {
                brightness = random.Next(80, 85) / 100.0;
            }
            else if (saturation > 0.55 && saturation <= 0.7)
            {
                brightness = random.Next(80, 90) / 100.0;
            }
            else
            {
                brightness = random.Next(85, 95) / 100.0;
            }

            var color = GetColorFromHSL(hue, saturation, brightness);
            txtBgColorHex.Text = "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowLoadingPanel();

                var worker = new BackgroundWorker();
                var workerArgument = new BackgroundWorkerArgument()
                {
                    CurrentPlayer = CurrentPlayer
                };
                worker.DoWork += (sender, e) =>
                {
                    var argument = (BackgroundWorkerArgument)e.Argument;
                    var result = new BackgroundWorkerResult();
                    result.HandleQueryResult(Data.EventShifts.PlayerHelper.UpsertPlayer(argument.CurrentPlayer, out var msg), msg);
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

        private void TextBox_PreviewTextInput_DigitsOnly(object sender, TextCompositionEventArgs e)
        {
            try
            {
                e.Handled = !long.TryParse(e.Text, out _);
            }
            catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
        }

        private void TextBox_Pasting_DigitsOnly(object sender, DataObjectPastingEventArgs e)
        {
            try
            {
                if (e.DataObject.GetDataPresent(typeof(string)))
                {
                    string text = (string)e.DataObject.GetData(typeof(string));
                    if (!long.TryParse(text, out _))
                    {
                        e.CancelCommand();
                    }
                }
                else
                {
                    e.CancelCommand();
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

        private Color GetColorFromHSL(double h, double s, double l)
        {
            // https://en.wikipedia.org/wiki/HSL_and_HSV#HSL_to_RGB
            double c = (1.0 - Math.Abs(2.0 * l - 1.0)) * s;
            double h1 = h / 60.0;
            double x = c * (1.0 - Math.Abs(h1 % 2.0 - 1.0));
            double m = l - (c / 2.0);

            double r1 = 0, g1 = 0, b1 = 0;
            if (h1 >= 0 && h1 < 1)
            {
                r1 = c;
                g1 = x;
                b1 = 0;
            }
            else if (h1 >= 1 && h1 < 2)
            {
                r1 = x;
                g1 = c;
                b1 = 0;
            }
            else if (h1 >= 2 && h1 < 3)
            {
                r1 = 0;
                g1 = c;
                b1 = x;
            }
            else if (h1 >= 3 && h1 < 4)
            {
                r1 = 0;
                g1 = x;
                b1 = c;
            }
            else if (h1 >= 4 && h1 < 5)
            {
                r1 = x;
                g1 = 0;
                b1 = c;
            }
            else if (h1 >= 5 && h1 < 6)
            {
                r1 = c;
                g1 = 0;
                b1 = x;
            }

            int r = Convert.ToInt32((r1 + m) * 255);
            int g = Convert.ToInt32((g1 + m) * 255);
            int b = Convert.ToInt32((b1 + m) * 255);

            return Color.FromRgb((byte)r, (byte)g, (byte)b);
        }
    }

    #endregion Helper Methods
}
