/*
 * ZomB Dashboard System <http://firstforge.wpi.edu/sf/projects/zombdashboard>
 * Copyright (C) 2011, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
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
using System.IO;

namespace System451.Communication.Dashboard
{
    /// <summary>
    /// Interaction logic for ErrorDialog.xaml
    /// </summary>
    public partial class ErrorDialog : Window
    {
        public ErrorDialog()
        {
            InitializeComponent();
        }

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(ErrorDialog), new UIPropertyMetadata(""));


        public string TopMessage
        {
            get { return (string)GetValue(TopMessageProperty); }
            set { SetValue(TopMessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TopMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopMessageProperty =
            DependencyProperty.Register("TopMessage", typeof(string), typeof(ErrorDialog), new UIPropertyMetadata("Ack! ZomB encountered an error. Please let the developers know about this."));



        public string BottomMessage
        {
            get { return (string)GetValue(BottomMessageProperty); }
            set { SetValue(BottomMessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BottomMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomMessageProperty =
            DependencyProperty.Register("BottomMessage", typeof(string), typeof(ErrorDialog), new UIPropertyMetadata("A copy of this report has been saved to C:\\ZomB.log"));



        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public static void PrcException(Exception ex)
        {
            string full = getFullException(ex);
            new ErrorDialog { Message = full }.ShowDialog();
            try
            {
                using (StreamWriter sw = new StreamWriter(File.Open("C:\\ZomB.log", FileMode.Append)))
                {
                    sw.Write(full+"\r\n\r\n");
                }
            }
            catch { }
        }

        public static string getFullException(Exception ex)
        {
            StringBuilder sb = new StringBuilder(ex.ToString());
            if (ex.InnerException != null)
                sb.Append("\r\nWith Inner:\r\n" + getFullException(ex.InnerException));
            return sb.ToString();
        }
    }
}
