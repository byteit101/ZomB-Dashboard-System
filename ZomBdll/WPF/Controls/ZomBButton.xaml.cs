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

namespace System451.Communication.Dashboard.WPF.Controls
{
    /// <summary>
    /// Interaction logic for ZomBButton.xaml
    /// </summary>
    [Design.ZomBDesignableProperty("Width", Dynamic = true, Category = "Layout")]
    [Design.ZomBDesignableProperty("Height", Dynamic = true, Category = "Layout")]
    [Design.ZomBDesignableProperty("Content", Dynamic = true, Category = "Appearance")]
    [Design.ZomBControl("ZomBButton", Description="Useful Button")]
    public partial class ZomBButton : Button
    {
        public ZomBButton()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.ContextMenu.PlacementTarget = this;
            this.ContextMenu.IsOpen = true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //Restart
            DashboardDataHub.RestartZomB();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            //Exit
            DashboardDataHub.ExitZomB();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            //DS restart
            DashboardDataHub.RestartDS();
        }
    }
}
