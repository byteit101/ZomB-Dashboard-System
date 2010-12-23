using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;

namespace System451.Communication.Dashboard.Net.DriverStation
{
    /// <summary>
    /// Interaction logic for JoystickDesignerWindow.xaml
    /// </summary>
    public partial class JoystickDesignerWindow : Window
    {
        public Joystick joy { get; set; }
        public JoystickDesignerWindow()
        {
            InitializeComponent();
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            joy.XSource = XBox.GetConfigString();
            joy.YSource = YBox.GetConfigString();
            joy.ZSource = ZBox.GetConfigString();
            //if (xsourcebox.SelectedItem.ToString().Contains("Virtual"))
            //    joy.XSource = xsourcebox.SelectedItem.ToString().Replace(" ", "") + "@" + xdetailbox.SelectedItem.ToString();
            //else
            //    joy.XSource = xsourcebox.SelectedItem.ToString().Replace(" ", "") + xdetailbox.SelectedItem.ToString();
            this.DialogResult = true;
        }

        private void detail_changed(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
