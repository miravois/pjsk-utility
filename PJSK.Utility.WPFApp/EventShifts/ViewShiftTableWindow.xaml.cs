using PJSK.Utility.EventShifts.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace PJSK.Utility.WPFApp.EventShifts
{
    /// <summary>
    /// Interaction logic for ViewShiftTableWindow.xaml
    /// </summary>
    public partial class ViewShiftTableWindow : Window
    {
        public ObservableCollection<ShiftTableRow> ShiftTableRows { get; set; }
        public ViewShiftTableWindow(ShiftTable shiftTable, DateTime? shiftDate, bool isExportImage)
        {
            InitializeComponent();
            ShiftTableRows = new ObservableCollection<ShiftTableRow>(shiftTable.ActiveShiftTableRows);
            ShiftTableRows.Insert(0, GetHeaderRow());
            DataContext = ShiftTableRows;

            txtShiftDate.Text = shiftDate.Value.ToString("yyyy-MM-dd (dddd)", System.Globalization.CultureInfo.GetCultureInfo("ja-JP"));

            Loaded += (sender, e) =>
            {
                try
                {
                    if (isExportImage)
                    {
                        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                        timer.Start();
                        timer.Tick += (sender, args) =>
                        {
                            timer.Stop();

                            double dpi = double.Parse(System.Configuration.ConfigurationManager.AppSettings["EventShifts_ImageDPI"].ToString());
                            double scale = dpi / 96;
                            int width = (int)((this.ActualWidth-15) * scale);
                            int height = (int)((this.ActualHeight-38) * scale);
                            RenderTargetBitmap bitmap = new RenderTargetBitmap(width, height, dpi, dpi, PixelFormats.Pbgra32);
                            bitmap.Render(this);

                            var fileName = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["EventShifts_ImageOutputDirPath"],
                            $"{shiftDate.Value.ToString(Common.Constants.DateTime.DateTimePatterns.Date)}-{DateTime.Now.ToString("yyyyMMddHHmmss")}.png");
                            using (FileStream stream = File.Create(fileName))
                            {
                                PngBitmapEncoder encoder = new PngBitmapEncoder();
                                //encoder.Interlace = PngInterlaceOption.On;
                                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                                encoder.Save(stream);
                            }
                            UIHelper.DisplayInfoMessage("IMAGE GENERATED!");
                        };
                    }
                }
                catch (Exception ex) { UIHelper.DisplayErrorMessage(ex); }
            };
        }

        private ShiftTableRow GetHeaderRow()
        {
            var headerRow = new ShiftTableRow();
            for (var i=0; i<6; i++)
            {
                var headerCell = new ShiftTableCell(new ShiftData("", true, false, (i+1)));
                if (i != 5)
                {
                    headerCell.CurrentPlayer.PlayerName = (i+1).ToString();
                }
                else
                {
                    //headerCell.CurrentPlayer.PlayerName = "待機枠";
                }
                headerRow.AllShifts.Add(headerCell);
            }
            return headerRow;
        }
    }
}
