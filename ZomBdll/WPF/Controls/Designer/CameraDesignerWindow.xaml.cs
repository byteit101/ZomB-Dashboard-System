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
using System.Reflection;
using System451.Communication.Dashboard.WPF.Design.DesignUtils;

namespace System451.Communication.Dashboard.WPF.Controls.Designer
{
    /// <summary>
    /// Interaction logic for CameraDesignerWindow.xaml
    /// </summary>
    public partial class CameraDesignerWindow : Window
    {
        CameraView Object { get; set; }
        ZomBControlCollection zcc;
        int lastid;
        bool haltevents = false;

        public CameraDesignerWindow(CameraView obj)
        {
            Object = obj;
            zcc=obj.GetControls();
            lastid = zcc.Count;
            InitializeComponent();
            if (zcc.Count > 0)
            {
                PopulateList();
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            zcc.Add(new CameraTarget());
            (zcc.Last() as CameraTarget).ControlName = "target" + ++lastid;
            (zcc.Last() as CameraTarget).Border = new Pen(Brushes.Lime, (1.0 / ((Object.Width + Object.Height) / 2.0)));
            (zcc.Last() as CameraTarget).Fill = new SolidColorBrush(Color.FromArgb(127,0,255,0));
            PopulateList();
            ListItems.SelectedIndex = zcc.Count - 1;
        }

        private void PopulateList()
        {
            ListItems.Items.Clear();
            foreach (var item in zcc)
            {
                ListItems.Items.Add(item as CameraTarget);
            }
        }

        private void removeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ListItems.SelectedItem != null)
            {
                zcc.Remove(ListItems.SelectedItem as IZomBControl);
                PopulateList();
            }
        }

        private void ListItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (haltevents)
                return;
            try
            {
                haltevents = true;
                nameBox.Text = (ListItems.SelectedItem as CameraTarget).ControlName;
                fillsp.Children.Clear();
                fillsp.Children.Add(DesignUtils.GetDesignerField(ListItems.SelectedItem, typeof(CameraTarget).GetProperty("Fill")));
                pensp.Children.Clear();
                pensp.Children.Add(DesignUtils.GetDesignerField((ListItems.SelectedItem as CameraTarget).Border, typeof(Pen).GetProperty("Brush")));
                WidthBox.Text = ((ListItems.SelectedItem as CameraTarget).Border.Thickness*((Object.Width+Object.Height)/2.0)).ToString();
                haltevents = false;
            }
            catch { nameBox.Text = ""; haltevents = false; }
        }

        private void nameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (haltevents)
                return;
            if (ListItems.SelectedItem != null)
            {
                haltevents = true;
                int si = ListItems.SelectedIndex;
                (zcc[zcc.IndexOf(ListItems.SelectedItem as CameraTarget)] as CameraTarget).ControlName = nameBox.Text;
                PopulateList();
                ListItems.SelectedIndex = si;
                haltevents = false;
            }
        }

        private void WidthBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (haltevents)
                return;
            if (ListItems.SelectedItem != null)
            {
                try
                {
                    (zcc[zcc.IndexOf(ListItems.SelectedItem as CameraTarget)] as CameraTarget).Border.Thickness = (double.Parse(WidthBox.Text)/((Object.Width+Object.Height)/2.0));
                }
                catch { }
            }
        }
    }
}
