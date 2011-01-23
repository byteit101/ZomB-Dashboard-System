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
using System451.Communication.Dashboard.ViZ.Properties;

namespace System451.Communication.Dashboard.ViZ
{
    /// <summary>
    /// Interaction logic for FirstRun.xaml
    /// </summary>
    public partial class FirstRun : Window
    {
        public FirstRun()
        {
            InitializeComponent();
        }

        private void textBox1_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Go();
            }
        }

        private void Go()
        {
            int i;
            if (int.TryParse(textBox1.Text, out i) && i.ToString() == textBox1.Text)
            {
                Settings.Default.LastTeamNumber = i.ToString();
                Settings.Default.Save();
                this.DialogResult = true;
            }
            else
                System.Console.Beep();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Go();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBox1.Focus();
        }
    }
}
