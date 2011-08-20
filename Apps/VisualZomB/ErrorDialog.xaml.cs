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

namespace System451.Communication.Dashboard.ViZ
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
    }
}
