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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace System451.Communication.Dashboard.Net.DriverStation
{
    /// <summary>
    /// Interaction logic for AxisConfigPanel.xaml
    /// </summary>
    public partial class AxisConfigPanel : GroupBox
    {
        public AxisConfigPanel()
        {
            InitializeComponent();

            var nams = ZDesigner.GetChildren();
            foreach (var item in nams)
            {
                xsourcebox.Items.Add("Virtual " + (item as FrameworkElement).Name);
            }

            xsourcebox.SelectedIndex = 0;
        }

        public void SetConfig(string p)
        {
            try
            {
                if (p.Contains("Virtual"))
                {
                    string[] splt = p.Split('@');
                    splt[0] = splt[0].Replace("Virtual", "Virtual ");
                    xsourcebox.SelectedIndex = SelectBox(xsourcebox, splt[0]);
                    xdetailbox.SelectedIndex = SelectBox(xdetailbox, splt[1]);
                }
                else//Hardware
                {
                    p = p.Substring(8);
                    xsourcebox.SelectedIndex = int.Parse(p[0].ToString()) - 1;
                    xdetailbox.SelectedIndex = SelectBox(xdetailbox, p.Substring(1));
                }
            }
            catch { }
        }

        private int SelectBox(ComboBox bx, string splt)
        {
            for (int i = 0; i < bx.Items.Count; i++)
            {
                if (bx.Items[i].ToString() == splt)
                {
                    return i;
                }
            }
            return 0;
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
                var nams = ZDesigner.GetChildByName(xsourcebox.SelectedItem.ToString().Substring(8)).GetType().GetProperties();
                foreach (var item in nams)
                {
                    xdetailbox.Items.Add(item.Name);
                }
            }
            xdetailbox.SelectedIndex = 0;
        }

        public string GetConfigString()
        {
            if (xsourcebox.SelectedItem.ToString().Contains("Virtual"))
                return xsourcebox.SelectedItem.ToString().Replace(" ", "") + "@" + xdetailbox.SelectedItem.ToString();
            else
                return xsourcebox.SelectedItem.ToString().Replace(" ", "") + xdetailbox.SelectedItem.ToString();
        }
    }
}
