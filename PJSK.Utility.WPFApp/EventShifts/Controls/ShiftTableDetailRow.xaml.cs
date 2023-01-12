using PJSK.Utility.EventShifts.Models;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PJSK.Utility.WPFApp.EventShifts.Controls
{
    /// <summary>
    /// Interaction logic for ShiftTableDetailRow.xaml
    /// </summary>
    public partial class ShiftTableDetailRow : UserControl
    {
        public ShiftTableDetailRow()
        {
            InitializeComponent();
        }

        public static DependencyProperty ShiftTimeProperty =
            DependencyProperty.Register("ShiftTime", typeof(string), typeof(ShiftTableDetailRow));

        public static DependencyProperty CurrentShiftTableRowProperty =
            DependencyProperty.Register("CurrentShiftTableRow", typeof(ShiftTableRow), typeof(ShiftTableDetailRow));

        public string ShiftTime
        {
            get { return (string)GetValue(ShiftTimeProperty); }
            set { SetValue(ShiftTimeProperty, value); }
        }

        public ShiftTableRow CurrentShiftTableRow
        {
            get { return (ShiftTableRow)GetValue(CurrentShiftTableRowProperty); }
            set { SetValue(CurrentShiftTableRowProperty, value); }
        }

        public event RoutedEventHandler ViewShiftTimeWindowClick;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ViewShiftTimeWindowClick != null)
            {
                ViewShiftTimeWindowClick(this, new RoutedEventArgs());
            }
        }
    }
}
