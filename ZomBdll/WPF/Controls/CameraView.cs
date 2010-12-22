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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System451.Communication.Dashboard.Net.Video;
using System.IO;
using System451.Communication.Dashboard.Utils;

namespace System451.Communication.Dashboard.WPF.Controls
{
    /// <summary>
    /// Interaction logic for CameraView.xaml
    /// </summary>
    [Design.ZomBControl("Camera View Control", Description = "This shows you what the camera sees, and any targets its reporting", IconName = "CameraViewIcon")]
    [TemplatePart(Name = "PART_img", Type = typeof(Image))]
    [TemplatePart(Name = "PART_refresh", Type = typeof(UIElement))]
    [TemplatePart(Name = "PART_targets", Type = typeof(Panel))]
    public class CameraView : ZomBGLControl, IZomBControlGroup, ISavableZomBData
    {
        Image PART_img;
        IDashboardVideoDataSource videoSource;
        UIElement PART_refresh;
        CameraTargetUI tars;
        Stream laststream;
        VideoStreamSaver vss;
        MenuItem mi;
        MenuItem smi;
        ContextMenu enu;

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
            tars = new CameraTargetUI();
            tars.Width = 1;
            tars.Height = 1;
            this.ControlAdded += delegate { if (VideoSource == DefaultVideoSource.WPILibTcpStream) TeamUpdated(); };
            enu = new ContextMenu();
            mi = new MenuItem();
            mi.Header = "Start Saving";
            mi.FontSize = 20;
            mi.Click += new RoutedEventHandler(mi_Click);
            enu.Items.Add(mi);
            smi = new MenuItem();
            smi.Header = "Stop Saving";
            smi.FontSize = 20;
            smi.Visibility = Visibility.Collapsed;
            smi.Click += new RoutedEventHandler(smi_Click);
            enu.Items.Add(smi);
            this.ContextMenu = enu;
        }

        void smi_Click(object sender, RoutedEventArgs e)
        {
            vss.EndSave();
            smi.Visibility = Visibility.Collapsed;
            mi.Visibility = Visibility.Visible;
        }

        void mi_Click(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(BTZomBFingerFactory.DefaultSaveLocation);
            vss = new VideoStreamSaver(this);
            vss.FPS = 15;
            vss.StartSave(BTZomBFingerFactory.DefaultSaveLocation + "\\Capture" + (DateTime.Now.Ticks.ToString("x")) + ".webm");
            mi.Visibility = Visibility.Collapsed;
            smi.Visibility = Visibility.Visible;
        }

        public override void UpdateControl(string value)
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_img = base.GetTemplateChild("PART_img") as Image;
            PART_refresh = base.GetTemplateChild("PART_refresh") as UIElement;
            (base.GetTemplateChild("PART_targets") as Panel).Children.Add(tars);
            PART_refresh.PreviewMouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(PART_refresh_PreviewMouseLeftButtonUp);
            PART_refresh.Visibility = ((ShowReset) ? Visibility.Visible : Visibility.Collapsed);
        }

        void PART_refresh_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ZDesigner.IsRunMode)
            {
                videoSource.Stop();
                videoSource.Start();
            }
        }

        /// <summary>
        /// Gets the DDH's Team
        /// </summary>
        public int TeamNumber
        {
            get { return LocalDashboardDataHub.Team; }
        }

        static void sTeamUpdated(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as CameraView).TeamUpdated();
        }

        void TeamUpdated()
        {
            if (ZDesigner.IsDesignMode)
                return;
            try
            {
                videoSource = ((VideoSource == DefaultVideoSource.WPILibTcpStream) ? (IDashboardVideoDataSource)new WPILibTcpVideoSource(TeamNumber) : ((VideoSource == DefaultVideoSource.Webcam) ? (IDashboardVideoDataSource)new WebCamVideoSource() : null));
                videoSource.NewImageRecieved += new NewImageDataRecievedEventHandler(videoSource_NewImageRecieved);
                videoSource.Start();
            }
            catch { }
        }

        static void ResetVischanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CameraView am = (o as CameraView);
            if (am.PART_refresh != null)
                am.PART_refresh.Visibility = (((bool)e.NewValue) ? Visibility.Visible : Visibility.Collapsed);
        }

        private void videoSource_NewImageRecieved(object sender, NewImageDataRecievedEventArgs e)
        {
            try
            {
                PART_img.Source = JpegBitmapDecoder.Create(e.NewDataStream, BitmapCreateOptions.None, BitmapCacheOption.None).Frames[0];
                if (this.dataUpdatedEvent != null)
                {
                    laststream = e.NewDataStream;
                    dataUpdatedEvent(this, new EventArgs());
                }
            }
            catch (System.Exception x)
            {
                System.Windows.Forms.MessageBox.Show(x.ToString());
            }
        }

        [Design.ZomBDesignable(DisplayName = "Show Reset"), Category("Appearance"), Description("Should we show the reset button?")]
        public bool ShowReset
        {
            get { return (bool)GetValue(ShowResetProperty); }
            set { SetValue(ShowResetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowReset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowResetProperty =
            DependencyProperty.Register("ShowReset", typeof(bool), typeof(CameraView), new UIPropertyMetadata(true, ResetVischanged));


        [Design.ZomBDesignable(DisplayName = "Source"), Category("Behavior"), Description("What are we looking at?")]
        public DefaultVideoSource VideoSource
        {
            get { return (DefaultVideoSource)GetValue(VideoSourceProperty); }
            set { SetValue(VideoSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VideoSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VideoSourceProperty =
            DependencyProperty.Register("VideoSource", typeof(DefaultVideoSource), typeof(CameraView), new UIPropertyMetadata(DefaultVideoSource.WPILibTcpStream, sTeamUpdated));

        #region IZomBControlGroup Members

        public ZomBControlCollection GetControls()
        {
            if (tars == null)
            {
                return null;
            }
            else
            {
                return tars.Targets;
            }
        }

        #endregion

        #region ISavableZomBData Members

        TypeConverter ISavableZomBData.GetTypeConverter()
        {
            return new Net.Video.BitmapConverter();
        }

        string ISavableZomBData.DataValue
        {
            get
            {
                //TODO: figure out a better way to do this
                return Convert.ToBase64String((laststream as MemoryStream).ToArray());//I know this is a memory stream :-)
            }
        }
        private EventHandler dataUpdatedEvent;

        event EventHandler ISavableZomBData.DataUpdated
        {
            add { dataUpdatedEvent += value; }
            remove { dataUpdatedEvent -= value; }
        }
        #endregion

        [Design.ZomBDesignable(), Category("ZomB"), Description("What targets will we be looking for?")]
        public CameraTargetCollection Targets
        {
            get
            {
                try
                {
                    var cc = converttargs(tars.Targets);
                    cc.ItemAdded += delegate { tars.SetTargets(cc.Select(x => (x as IZomBControl))); };
                    return cc;
                }
                catch { }
                return null;
            }
            set
            {
                tars.SetTargets(value.Select(x => (x as IZomBControl)));
            }
        }

        private CameraTargetCollection converttargs(ZomBControlCollection inv)
        {
            CameraTargetCollection ctc = new CameraTargetCollection();
            foreach (var item in inv)
            {
                ctc.Add(item as CameraTarget);
            }
            return ctc;
        }

        public static void tchangecallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var t = o as CameraView;
            if (t.tars != null)
                t.tars.SetTargets(t.Targets);
        }
    }
    [Design.Designer(typeof(Designers.CameraTargetCollectionDesigner))]
    public class CameraTargetCollection : Collection<CameraTarget>
    {
        protected override void InsertItem(int index, CameraTarget item)
        {
            base.InsertItem(index, item);
            if (ItemAdded != null)
            {
                ItemAdded(this, new EventArgs());
            }
        }
        public event EventHandler ItemAdded;
    }

    public class CameraTargetUI : Panel
    {
        ZomBControlCollection targets = new ZomBControlCollection();
        DrawingVisual dv = new DrawingVisual();

        public CameraTargetUI()
        {
            using (DrawingContext dc = dv.RenderOpen())
            {
                dc.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, 1, 1));
            }
        }

        public ZomBControlCollection Targets { get { return targets; } }

        protected override int VisualChildrenCount
        {
            get
            {
                return targets.Count + 1;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index == targets.Count)
            {
                return dv;
            }
            return (CameraTarget)targets[index];
        }

        public void AddTarget(CameraTarget targ)
        {
            targets.Add(targ);

            AddVisualChild(targ);
            AddLogicalChild(targ);
        }

        public void RemoveTarget(CameraTarget targ)
        {
            targets.Remove(targ);

            RemoveVisualChild(targ);
            RemoveLogicalChild(targ);
        }

        public void SetTargets(CameraTargetCollection p)
        {
            foreach (var item in targets)
            {
                RemoveTarget(item as CameraTarget);
            }
            foreach (var item in p)
            {
                AddTarget(item);
            }
        }

        public void SetTargets(IEnumerable<IZomBControl> p)
        {
            foreach (var item in targets)
            {
                RemoveTarget(item as CameraTarget);
            }
            foreach (var item in p)
            {
                AddTarget(item as CameraTarget);
            }
        }
    }

    public class CameraTarget : DrawingVisual, IZomBControl
    {
        Pen border;
        public Pen Border
        {
            get { return border; }
            set
            {
                if (border != value)
                {
                    border = value;
                    Render();
                }
            }
        }

        Brush fill;
        public Brush Fill
        {
            get { return fill; }
            set
            {
                if (fill != value)
                {
                    fill = value;
                    Render();
                }
            }
        }

        Rect target;
        public Rect Target
        {
            get { return target; }
            set
            {
                if (target != value)
                {
                    target = value;
                    Render();
                }
            }
        }

        private void Render()
        {
            if (Dispatcher.Thread != System.Threading.Thread.CurrentThread)
                Dispatcher.Invoke(new Utils.VoidFunction(Render), null);
            using (DrawingContext dc = RenderOpen())
            {
                dc.DrawRectangle(Fill, Border, Target);
            }
        }

        private Rect RectParse(string value)
        {
            Rect rf = new Rect();
            if (value != null && value != "")
            {
                try
                {
                    //format widthxheigth+xpos,ypos
                    rf.Width = double.Parse(value.Substring(0, value.IndexOf('x')));
                    rf.Height = double.Parse(value.Substring(value.IndexOf('x') + 1, (value.IndexOf('+') - (value.IndexOf('x') + 1))));
                    rf.X = double.Parse(value.Substring(value.IndexOf('+') + 1, (value.IndexOf(',') - (value.IndexOf('+') + 1))));
                    rf.Y = double.Parse(value.Substring(value.IndexOf(',') + 1));
                }
                catch { }//bad, empty rect
            }
            return rf;
        }
        #region IZomBControl Members

        public bool IsMultiWatch
        {
            get { return false; }
        }

        public string ControlName { get; set; }

        public void UpdateControl(string value)
        {
            Target = RectParse(value);
        }

        public void ControlAdded(object sender, ZomBControlAddedEventArgs e)
        {
            //great, don't care
        }

        #endregion
    }
}
