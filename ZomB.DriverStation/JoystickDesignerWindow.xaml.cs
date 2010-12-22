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
        Assembly a;
        public JoystickDesignerWindow()
        {
            InitializeComponent();

            ////TODO: rm this evil hack
            //a = (from asm in AppDomain.CurrentDomain.GetAssemblies() where asm.FullName.Contains("ViZ") select asm).First();
            //string[] nams = (string[])a.GetType("System451.Communication.Dashboard.ViZ.Designer").GetMethod("GetNames").Invoke(null, null);
            //foreach (var item in nams)
            //{
            //    xsourcebox.Items.Add("Virtual " + item);
            //}

            xsourcebox.SelectedIndex = 0;
        }

        private void Source_changed(object sender, SelectionChangedEventArgs e)
        {
            xdetailbox.Items.Clear();
            //0-3 are hw
            if (xsourcebox.SelectedIndex < 4)
            {
                xdetailbox.Items.Add("LeftX");
                xdetailbox.Items.Add("LeftY");
                xdetailbox.Items.Add("RightX");
                xdetailbox.Items.Add("RightY");
                xdetailbox.Items.Add("LeftTrigger");
                xdetailbox.Items.Add("RightTrigger");
            }
            else
            {
                //TODO: rm this evil hack
                string[] nams = (string[])a.GetType("System451.Communication.Dashboard.ViZ.Designer").GetMethod("GetProperties").Invoke(null, new[] { xsourcebox.SelectedItem.ToString().Substring(8) });
                foreach (var item in nams)
                {
                    xdetailbox.Items.Add(item);
                }
            }
            xdetailbox.SelectedIndex = 0;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (xsourcebox.SelectedItem.ToString().Contains("Virtual"))
                joy.XSource = xsourcebox.SelectedItem.ToString().Replace(" ", "") + "@" + xdetailbox.SelectedItem.ToString();
            else
                joy.XSource = xsourcebox.SelectedItem.ToString().Replace(" ", "") + xdetailbox.SelectedItem.ToString();
            this.DialogResult = true;
        }

        private void detail_changed(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
