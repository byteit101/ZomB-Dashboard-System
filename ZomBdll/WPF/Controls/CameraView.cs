/*
 * ZomB Dashboard System <http://firstforge.wpi.edu/sf/projects/zombdashboard>
 * Copyright (C) 2009-2010, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
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
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System451.Communication.Dashboard.Net.Video;
using System.Windows.Media.Imaging;
using System.IO;
using System.ComponentModel;

namespace System451.Communication.Dashboard.WPF.Controls
{
    /// <summary>
    /// Interaction logic for CameraView.xaml
    /// </summary>
    [Design.ZomBControl("Camera View Control", Description = "This shows you what the camera sees, and any targets its reporting", IconName="CameraViewIcon")]
    [TemplatePart(Name = "PART_img", Type = typeof(Image))]
    [TemplatePart(Name = "PART_refresh", Type = typeof(UIElement))]
    public class CameraView : ZomBGLControl
    {
        Image PART_img;
        IDashboardVideoDataSource videoSource;
        UIElement PART_refresh;
        static CameraView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CameraView),
            new FrameworkPropertyMetadata(typeof(CameraView)));
        }

        public CameraView()
        {
            this.SnapsToDevicePixels = true;
            this.Width = 320;
            this.Height = 240;
        }

        public override void UpdateControl(string value)
        {
            //TODO?
            //base.UpdateControl(value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_img = base.GetTemplateChild("PART_img") as Image;
            PART_refresh = base.GetTemplateChild("PART_refresh") as UIElement;
            PART_refresh.PreviewMouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(PART_refresh_PreviewMouseLeftButtonUp);
            PART_refresh.Visibility = ((ShowReset) ? Visibility.Visible : Visibility.Collapsed);
        }

        void PART_refresh_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            videoSource.Stop();
            videoSource.Start();
        }

        [Design.ZomBDesignable(DisplayName = "Team #"), Category("Misc"), Description("Your team number. This is used to create the IP address of the camera")]
        public int TeamNumber
        {
            get { return (int)GetValue(TeamNumberProperty); }
            set { SetValue(TeamNumberProperty, value); }
        }

        static void TeamUpdated(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CameraView am = (o as CameraView);
            am.videoSource = new WPILibTcpVideoSource((int)e.NewValue);
            am.videoSource.NewImageRecieved += new NewImageDataRecievedEventHandler(am.videoSource_NewImageRecieved);
            am.videoSource.Start();
        }

        static void ResetVischanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CameraView am = (o as CameraView);
            if (am.PART_refresh!=null)
            am.PART_refresh.Visibility = (((bool)e.NewValue)? Visibility.Visible: Visibility.Collapsed);
        }

        private void videoSource_NewImageRecieved(object sender, NewImageDataRecievedEventArgs e)
        {
            try
            {
                PART_img.Source = JpegBitmapDecoder.Create(e.NewDataStream, BitmapCreateOptions.None, BitmapCacheOption.None).Frames[0];
            }
            catch (System.Exception x)
            {
                System.Windows.Forms.MessageBox.Show(x.ToString());
            }
        }

        // Using a DependencyProperty as the backing store for TeamNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TeamNumberProperty =
            DependencyProperty.Register("TeamNumber", typeof(int), typeof(CameraView), new UIPropertyMetadata(0, TeamUpdated));


        [Design.ZomBDesignable(DisplayName = "Show Reset"), Category("Appearance"), Description("Should we show the reset button?")]
        public bool ShowReset
        {
            get { return (bool)GetValue(ShowResetProperty); }
            set { SetValue(ShowResetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowReset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowResetProperty =
            DependencyProperty.Register("ShowReset", typeof(bool), typeof(CameraView), new UIPropertyMetadata(true, ResetVischanged));
    }
}
