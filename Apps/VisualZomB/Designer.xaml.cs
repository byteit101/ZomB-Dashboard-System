﻿/*
 * ZomB Dashboard System <http://firstforge.wpi.edu/sf/projects/zombdashboard>
 * Copyright (C) 2012, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System451.Communication.Dashboard.Net;
using System451.Communication.Dashboard.Utils;
using System451.Communication.Dashboard.ViZ.Properties;
using System451.Communication.Dashboard.WPF.Controls;
using System451.Communication.Dashboard.WPF.Controls.Designer;
using System451.Communication.Dashboard.WPF.Design;

namespace System451.Communication.Dashboard.ViZ
{
    public partial class Designer : Window, ISurfaceDesigner
    {
        #region init, variables, and basic controls
        enum CurrentDrag
        {
            None,
            Move,
            Resize
        }

        [Flags]
        public enum CurrentDragMove
        {
            None = 0x0,
            X = 0x1,
            Y = 0x2,
            Width = 0x4,
            Height = 0x8
        }


        #region Win32 SystemMenu

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem);

        [DllImport("user32.dll")]
        private static extern IntPtr CheckMenuItem(IntPtr hMenu, int uIDCheckItem, int uCheck);

        const int WM_SYSCOMMAND = 0x112;
        const int MF_SEPARATOR = 0x800;
        const int MF_BYPOSITION = 0x400;
        const int MF_CHECKED = 0x8;
        const int MF_UNCHECKED = 0x0;
        const int MF_STRING = 0x0;

        const int AlwaysOnTopMenuID = 9066;
        const int AboutMenuID = 9067;

        #endregion

        Point dndopoint, opoint, oxpoint;
        object origSrc;
        bool lbdragging = false;
        CurrentDrag cd = CurrentDrag.None;
        CurrentDragMove cdm = CurrentDragMove.None;
        List<FrameworkElement> designerProps = new List<FrameworkElement>(4);
        static Dictionary<string, string> xmlNSmappings = new Dictionary<string, string>();
        ZomBUrlCollection ZomBUrlSources { get; set; }

        SurfaceControl curObj;

        Toolbox tbx;
        ListBox listBox1;
        Panel propHolder;

        bool resizingform = false;
        Point orfPoing;
        Size orfSixe, wsize;
        string preFile = null;

        static Designer dsb = null;

        public Designer()
        {
            if (dsb != null)
                throw new InvalidOperationException("Only one ViZ designer can be active at a time in this App Domain");
            dsb = this;
            if (Settings.Default.LastTeamNumber == "0" || Settings.Default.LastTeamNumber == "" || Settings.Default.LastTeamNumber == null)
            {
                Process.Start(System.AppDomain.CurrentDomain.BaseDirectory+"\\ViZ.exe", "help");
                Thread.Sleep(3141);
                new FirstRun().ShowDialog();
            }
            ZDesigner.SetDesigner(this);
            InitializeComponent();

            //initialize ns's
            LoadNSs();

            tbx = new Toolbox();
            if (System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width > 1080 || Settings.Default.EmbeddedTbx)
            {
                //chromify, we have space
                this.AllowsTransparency = false;
                Scrlview.Background = Brushes.LightGray;
                ViZGrid.Background = Brushes.White;
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.Width = 1080;
                this.Height = 470;
                if (Settings.Default.EmbeddedTbx)
                {
                    tbx.Content = null;
                    ViZGrid.Children.Add(tbx.TbxGrid);
                    this.Height += 230;
                    ToolboxDefinition.Height = new GridLength(230);
                    SplitterDefinition.Height = GridLength.Auto;
                }
            }
            else if (System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width <= 1080)
            {
                Canvas.SetTop(MenuGrid, 0);
            }
            listBox1 = tbx.ToolListBox;
            listBox1.PreviewMouseLeftButtonDown += listBox1_PreviewMouseLeftButtonDown;
            listBox1.PreviewMouseUp += listBox1_PreviewMouseUp;
            listBox1.PreviewMouseMove += listBox1_PreviewMouseMove;
            propHolder = tbx.PropertyBox;

            double nsysheight = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
            double nsyswidth = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;

            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
            {
                nsyswidth *= (96.0 / g.DpiX);
                nsysheight *= (96.0 / g.DpiY);
            }
            this.Top = (nsysheight <= 600) ? -1 : (nsysheight - (this.Height + (Settings.Default.EmbeddedTbx ? 0 : tbx.Height))) / 2.0;
            this.Left = Math.Max(-1.0, (nsyswidth / 2.0) - this.Width / 2.0);

            DoubleAnimation VizLogoani = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 2)));
            VizLogoani.BeginTime = new TimeSpan(0, 0, 1);
            VizLogoani.Completed += delegate { LayoutCvs.Children.Remove(ViZLogo); };
            ViZLogo.BeginAnimation(Image.OpacityProperty, VizLogoani);

            //Create our properties
            //TODO: this is a mess, improve it
            var lb = new Label();
            lb.Content = "Dashboard Data Hub";
            lb.Style = (Style)lb.FindResource("PropCatStyle");
            designerProps.Add(lb);
            designerProps.Add(new Label());
            (designerProps[1] as Label).Style = (Style)lb.FindResource("PropCatStyle");
            designerProps.Add(new Label());
            (designerProps[2] as Label).Content = "Invalid Packets:";
            designerProps.Add(new ComboBox());
            var cb = (designerProps[3] as ComboBox);
            foreach (var item in Enum.GetNames(typeof(InvalidPacketActions)))
            {
                cb.Items.Add(item);
                if (item == "Ignore")
                {
                    cb.SelectedValue = item;
                }
            }
            designerProps.Add(new Label());
            (designerProps[4] as Label).Content = "Source:";
            ZomBUrlSources = "zomb://." + Settings.Default.LastTeamNumber + "/SmartNG";
            var dsnr = new ZomBUrlCollectionDesigner();
            dsnr.Initialize(this, this.GetType().GetProperty("ZomBUrlSources", BindingFlags.NonPublic | BindingFlags.Instance));
            designerProps.Add(dsnr.GetProperyField());
            (designerProps[5] as FrameworkElement).Tag = dsnr;
            designerProps.Add(new Label());
            (designerProps[6] as Label).Content = "Team #:";
            designerProps.Add(new TextBox());
            (designerProps[7] as TextBox).Text = Settings.Default.LastTeamNumber;
            //End of massivly wrong code
            //TODO: clean up the above code

            propHolder.Children.Clear();
            foreach (var item in designerProps)
            {
                propHolder.Children.Add(item);
            }

            this.Closing += delegate
            {
                Settings.Default.LastTeamNumber = (designerProps[7] as TextBox).Text;
                Settings.Default.Save();
                try
                {
                    tbx.Close();
                }
                catch { }
            };
        }

        private void LoadNSs()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var ns in asm.GetCustomAttributes(typeof(WPF.Design.ZomBZamlNamespaceAttribute), false))
                {
                    var zn = ns as WPF.Design.ZomBZamlNamespaceAttribute;
                    xmlNSmappings.Add(zn.ClrNamespace, zn.XmlNamespace);
                }
            }
        }

        IntPtr sysptr;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            if (Environment.OSVersion.Version.Major >= 6)
            {
                if (Utils.AeroGlass.GlassifyWindow(this))
                {
                    ViZGrid.Background = Brushes.Transparent;
                    Scrlview.Background = Brushes.Transparent;
                }
            }

            IntPtr handle = new WindowInteropHelper(this).Handle;
            sysptr = GetSystemMenu(handle, false);

            /// Create our new System Menu items just before the Close menu item
            AppendMenu(sysptr, MF_SEPARATOR, 0, string.Empty); // <-- Add a menu seperator
            AppendMenu(sysptr, 0, AlwaysOnTopMenuID, "Always on top");
            AppendMenu(sysptr, 0, AboutMenuID, "About...");

            // Attach our WndProc handler to this Window
            HwndSource source = HwndSource.FromHwnd(handle);
            source.AddHook(new HwndSourceHook(WndProc));

            //#ifdef Holloween
            if (DateTime.Today.Day == 31 && DateTime.Today.Month == 10)
            {
                Scrlview.Background = Brushes.Maroon;
            }
            //#endif /*Holloween*/
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_SYSCOMMAND)
            {
                switch (wParam.ToInt32())
                {
                    case AlwaysOnTopMenuID:
                        var self = getDesigner();
                        self.Topmost = !self.Topmost;
                        CheckMenuItem(self.sysptr, AlwaysOnTopMenuID, self.Topmost ? MF_CHECKED : MF_UNCHECKED);
                        handled = true;
                        break;
                    case AboutMenuID:
                        getDesigner().MenuItem_Click(null, null);
                        handled = true;
                        break;
                }
            }

            return IntPtr.Zero;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Settings.Default.EmbeddedTbx)
            {
                tbx.Top = this.Top + this.ActualHeight - 2.0;
                tbx.Left = this.Left + (this.ActualWidth / 2.0) - (tbx.Width / 2.0);
                tbx.Show();
                tbx.Owner = this;
            }
            tbx.Closed += new EventHandler(tbx_Closed);
            try
            {
                listBox1.ItemsSource = Reflector.GetZomBDesignableInfos(Reflector.GetZomBDesignableClasses());
            }
            catch (Exception ex)
            {
                ErrorDialog.PrcException(ex);
                return;
            }
            if (preFile != null)
            {
                LoadFile(preFile);
                preFile = null;
            }
            ThreadPool.QueueUserWorkItem(new WaitCallback(CheckForUpdates));
        }

        private void CheckForUpdates(object args)
        {
            Thread.Sleep(3141);//wait pi seconds before continuing
            UpdateData url = new UpdateData();
            try
            {
                url = Updater.Check();
            }
            catch { }

            if (url.UpdateAvailable == true)//whoa! updates!
            {
                try
                {
                    string updatepath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + Path.DirectorySeparatorChar + "ZomB Update.exe";
                    if (!Updater.Download(url, updatepath))
                        TSAlert("An update for ZomB is avalible, but automatic download failed. Please manually update ZomB:\r\n\r\n" + url.DownloadUrl + "\r\n\r\nChanges:\r\n" + url.Changes, false);
                    if (TSAlert("An update for ZomB is avalible, and has been downloaded & verified to your desktop.\r\nWould you like to update now?\r\nChanges:\r\n" + url.Changes, true))
                    {
                        Process.Start(updatepath);
                        TSClose();
                    }
                }
                catch
                {
                    TSAlert("An update for ZomB is avalible, but automatic download failed. Please manually update ZomB:\r\n\r\n" + url, false);
                }
            }
        }
        private delegate bool alerter(string message, bool yesno);
        private bool TSAlert(string message, bool yesno)
        {
            return (bool)this.Dispatcher.Invoke(new alerter(NTSAlert), message, yesno);
        }

        private void TSClose()
        {
            this.Dispatcher.Invoke(new Utils.VoidFunction(tbx.Close));
        }

        private bool NTSAlert(string message, bool yesno)
        {
            if (yesno)
            {
                return MessageBox.Show(message, "", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
            }
            else
                new ErrorDialog { Message = message, TopMessage = "", BottomMessage = "" }.ShowDialog();
            return false;
        }

        void tbx_Closed(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch { }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        #endregion

        #region AddControl

        #region DnD

        private void listBox1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dndopoint = e.GetPosition(null);
            origSrc = e.OriginalSource;
            lbdragging = true;
        }

        private void listBox1_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                // Get the current mouse position
                Point mousePos = e.GetPosition(null);
                Vector diff = dndopoint - mousePos;

                if (lbdragging && (e.LeftButton == MouseButtonState.Pressed &&
                    Math.Abs(diff.X) + Math.Abs(diff.Y) > 3))
                {
                    // Get the dragged ListViewItem
                    ListBoxItem listViewItem =
                        FindAnchestor<ListBoxItem, WrapPanel>((DependencyObject)origSrc);

                    //System.Diagnostics.Debug.Print("Moving...");
                    // Find the data behind the ListViewItem
                    if (listViewItem != null)
                    {
                        ZomBControlAttribute contact = (ZomBControlAttribute)listBox1.ItemContainerGenerator.
                            ItemFromContainer(listViewItem);
                        //System.Diagnostics.Debug.Print("Found...");
                        // Initialize the drag & drop operation
                        DataObject dragData = new DataObject("ZomBControl", contact);
                        DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Copy);
                    }
                    lbdragging = false;

                }
            }
            catch (Exception ex)
            {
                ErrorDialog.PrcException(ex);
            }
        }

        private void listBox1_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            lbdragging = false;
        }

        // Helper to search up the VisualTree
        public static T FindAnchestor<T, P>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T && VisualTreeHelper.GetParent(current) is P)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        public static T FindAnchestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        public static DependencyObject FindTopAnchestor(DependencyObject current)
        {
            do
            {
                if (VisualTreeHelper.GetParent(current) == null)
                {
                    return current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        private void ZDash_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("ZomBControl"))
            {
                ZomBControlAttribute info = (e.Data.GetData("ZomBControl") as ZomBControlAttribute);
                if (info != null)
                    AddControl(info, e.GetPosition(ZDash));
            }
            else if (e.Data.GetDataPresent("ZomBControl2"))
            {
                ZomBControlAttribute info = (e.Data.GetData("ZomBControl2") as ZomBControlAttribute);
                if (info != null)
                {
                    if (info.Name == "ZomB Name")
                    {
                        //Add Dragger!
                        var r = VisualTreeHelper.HitTest(ZDash, e.GetPosition(ZDash));
                        if (r.VisualHit != ZDash)
                        {
                            SurfaceControl sc = FindAnchestor<SurfaceControl>(r.VisualHit);
                            sc.Control.Name = ap.Name.Replace(" ", "Z").Replace("_", "__").Replace(".", "_") + Guid.NewGuid().ToString("N").Substring(0, 5);
                            if (sc.Control is ZomBGLControl)
                                (sc.Control as ZomBGLControl).ControlName = ap.Name;
                            else if (sc.Control is ZomBGLControlGroup)
                                (sc.Control as ZomBGLControlGroup).ControlName = ap.Name;
                            if (curObj != null)
                            {
                                var pco = curObj;
                                Deselect();
                                Select(pco);
                            }

                        }
                    }
                    else
                        AddControl(info, e.GetPosition(ZDash), ap);
                }
            }
        }

        private void ZDash_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("ZomBControl") && !e.Data.GetDataPresent("ZomBControl2"))
            {
                e.Effects = DragDropEffects.None;
            }
        }

        #endregion

        private void AddControl(ZomBControlAttribute info)
        {
            AddControl(info, new Point(5.0, 5.0));
        }

        private void AddControl(ZomBControlAttribute info, Point point, AutoPoint aup)
        {
            FrameworkElement fe = Reflector.Inflate(info.Type) as FrameworkElement;
            fe.Name = aup.Name.Replace(" ", "Z").Replace("_", "__").Replace(".", "_") + Guid.NewGuid().ToString("N").Substring(0, 5);
            if (fe is ZomBGLControl)
                (fe as ZomBGLControl).ControlName = aup.Name;
            else if (fe is ZomBGLControlGroup)
                (fe as ZomBGLControlGroup).ControlName = aup.Name;
            AddControl(fe, point);
        }

        private void AddControl(ZomBControlAttribute info, Point point)
        {
            FrameworkElement fe = Reflector.Inflate(info.Type) as FrameworkElement;
            fe.Name = "Z" + Guid.NewGuid().ToString("N").Substring(0, 16);
            AddControl(fe, point);
        }

        private void AddControl(FrameworkElement ctrl)
        {
            AddControl(ctrl, new Point(Canvas.GetLeft(ctrl), Canvas.GetTop(ctrl)));
        }

        private void AddControl(FrameworkElement fe, Point point)
        {
            var sc = new SurfaceControl();
            sc.Control = fe;
            sc.Width = fe.Width;
            sc.Height = fe.Height;
            if (Double.IsNaN(fe.Width))
            {
                sc.Width = 75;//fe.ActualWidth;
            }
            if (Double.IsNaN(fe.Height))
            {
                sc.Height = 25;//fe.ActualHeight;
            }
            ZDash.Children.Add(sc);
            Canvas.SetTop(sc, point.Y);
            Canvas.SetLeft(sc, point.X);
            if (fe.GetType() == typeof(ZomBGLControlGroup))
            {
                (fe as ZomBGLControlGroup).GroupDescriptor = (fe as ZomBGLControlGroup).GroupDescriptor;//re-load controls
            }
            Select(sc);
        }

        #endregion

        #region Designer Surface

        #region Move and Resize

        private void ScrollViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            cd = CurrentDrag.None;
            if (curObj != null)
            {
                curObj.ClearSnap();
                curObj.DrawSnaps();
            }
            ZDash.ReleaseMouseCapture();
        }

        private void ScrollViewer_MouseMove(object sender, MouseEventArgs e)
        {
            switch (cd)
            {
                case CurrentDrag.Move:
                    {
                        Vector mv = e.GetPosition(ZDash) - dndopoint;
                        Canvas.SetLeft((UIElement)origSrc, opoint.X + mv.X);
                        Canvas.SetTop((UIElement)origSrc, opoint.Y + mv.Y);
                        ShowSnaps(SnapGridDirections.All, (x) => Canvas.SetLeft(curObj, x), (y) => Canvas.SetTop(curObj, y), (r) => Canvas.SetLeft(curObj, r - SnapGridHelper.Right(curObj) + SnapGridHelper.Left(curObj)), (b) => Canvas.SetTop(curObj, b - SnapGridHelper.Bottom(curObj) + SnapGridHelper.Top(curObj)));
                    }
                    break;
                case CurrentDrag.Resize:
                    {
                        Vector mv = e.GetPosition(ZDash) - dndopoint;
                        var sc = (origSrc as SurfaceControl);
                        if (cdm.Flagged(CurrentDragMove.Width))
                            sc.Width = Math.Max(0, opoint.X + mv.X);
                        if (cdm.Flagged(CurrentDragMove.Height))
                            sc.Height = Math.Max(0, opoint.Y + mv.Y);
                        if (cdm.Flagged(CurrentDragMove.X))
                        {
                            Canvas.SetLeft(sc, oxpoint.X + mv.X);
                            sc.Width = Math.Max(0, opoint.X - mv.X);
                        }
                        if (cdm.Flagged(CurrentDragMove.Y))
                        {
                            Canvas.SetTop(sc, oxpoint.Y + mv.Y);
                            sc.Height = Math.Max(0, opoint.Y - mv.Y);
                        }
                        ShowSnaps(((SnapGridDirections)cdm), x => { sc.Width = Canvas.GetLeft(sc) + sc.Width - x; Canvas.SetLeft(sc, x); }, y => { sc.Height = Canvas.GetTop(sc) + sc.Height - y; Canvas.SetTop(sc, y); }, r => curObj.Width = r - SnapGridHelper.Left(curObj), b => curObj.Height = b - SnapGridHelper.Top(curObj));

                    }
                    break;
                case CurrentDrag.None:
                default:
                    break;
            }
        }

        private void ZDash_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Deselect();
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (e.OriginalSource is Button || ((FrameworkElement)e.OriginalSource).TemplatedParent is Button) //resize, TODO: add PART_xxx detection
                {
                    cd = CurrentDrag.Resize;
                    dndopoint = e.GetPosition(ZDash);
                    origSrc = FindAnchestor<SurfaceControl>((DependencyObject)e.OriginalSource);
                    cdm = (origSrc as SurfaceControl).GetResizeDirection(e.OriginalSource as FrameworkElement);
                    opoint = new Point((origSrc as FrameworkElement).Width, (origSrc as FrameworkElement).Height);
                    oxpoint = new Point(Canvas.GetLeft(origSrc as FrameworkElement), Canvas.GetTop(origSrc as FrameworkElement));
                    Select(origSrc as SurfaceControl);
                    ZDash.CaptureMouse();
                }
                else if (e.OriginalSource is SurfaceControl || VisualTreeHelper.GetParent((DependencyObject)e.OriginalSource) is SurfaceControl) //resize, TODO: add PART_xxx detection
                {
                    cd = CurrentDrag.Move;
                    dndopoint = e.GetPosition(ZDash);
                    origSrc = FindAnchestor<SurfaceControl>((DependencyObject)e.OriginalSource);
                    opoint = new Point(Canvas.GetLeft((UIElement)origSrc), Canvas.GetTop((UIElement)origSrc));
                    Select(origSrc as SurfaceControl);
                    ZDash.CaptureMouse();
                }
            }
        }

        #region Snapping

        /// <summary>
        /// Snaping code. Be careful, it has a bad bite.
        /// </summary>
        /// <param name="dir">Directions to look for the snapping code that could bite your finger off</param>
        private void ShowSnaps(SnapGridDirections dir, Action<double> leftModifier, Action<double> topModifier, Action<double> rightModifier, Action<double> bottomModifier)
        {
            curObj.ClearSnap();
            List<SnapGridDistance> dist = new List<SnapGridDistance>();
            double lastDistance;
            List<SnapLine> snaplines = new List<SnapLine>();

            SnapLine leftside = new SnapLine { x1 = -1, x2 = 0.5, color = SnapLine.EqualLineColor, y1 = 0, y2 = curObj.Height };
            SnapLine rightside = new SnapLine { x1 = -1, x2 = curObj.Width + .5, color = SnapLine.EqualLineColor, y1 = 0, y2 = curObj.Height };
            SnapLine topside = new SnapLine { x1 = 0, x2 = curObj.Width, color = SnapLine.EqualLineColor, y1 = -1, y2 = .5 };
            SnapLine bottomside = new SnapLine { x1 = 0, x2 = curObj.Width, color = SnapLine.EqualLineColor, y1 = -1, y2 = curObj.Height + .5 };

            //Main code //TODO: better comment
            foreach (Control other in ZDash.Children)
            {
                if (other == curObj)
                    continue;

                if ((dir & SnapGridDirections.X) == SnapGridDirections.X)
                {
                    if ((lastDistance = SnapGridHelper.SnapableForceLeft1(curObj, other)) < SnapGridHelper.SnapableForceDistance)
                    {
                        dist.Add(new SnapGridDistance { Distance = lastDistance, Location = SnapGridDirections.X, Type = SnapType.Equal1, other = other });
                    }
                    else if ((lastDistance = SnapGridHelper.SnapableForceLeft2(curObj, other)) < SnapGridHelper.SnapableForceDistance)
                    {
                        dist.Add(new SnapGridDistance { Distance = lastDistance, Location = SnapGridDirections.X, Type = SnapType.Equal2, other = other });
                    }
                    else if ((lastDistance = SnapGridHelper.SnapableForceDistanceLeft(curObj, other)) < SnapGridHelper.SnapableForceDistance)
                    {
                        dist.Add(new SnapGridDistance { Distance = lastDistance, Location = SnapGridDirections.X, Type = SnapType.Distance, other = other });
                    }
                }
                if ((dir & SnapGridDirections.Right) == SnapGridDirections.Right)
                {
                    if ((lastDistance = SnapGridHelper.SnapableForceRight1(curObj, other)) < SnapGridHelper.SnapableForceDistance)
                    {
                        dist.Add(new SnapGridDistance { Distance = lastDistance, Location = SnapGridDirections.Right, Type = SnapType.Equal1, other = other });
                    }
                    else if ((lastDistance = SnapGridHelper.SnapableForceRight2(curObj, other)) < SnapGridHelper.SnapableForceDistance)
                    {
                        dist.Add(new SnapGridDistance { Distance = lastDistance, Location = SnapGridDirections.Right, Type = SnapType.Equal2, other = other });
                    }
                    else if ((lastDistance = SnapGridHelper.SnapableForceDistanceRight(curObj, other)) < SnapGridHelper.SnapableForceDistance)
                    {
                        dist.Add(new SnapGridDistance { Distance = lastDistance, Location = SnapGridDirections.Right, Type = SnapType.Distance, other = other });
                    }
                }
                if ((dir & SnapGridDirections.Y) == SnapGridDirections.Y)
                {
                    if ((lastDistance = SnapGridHelper.SnapableForceTop1(curObj, other)) < SnapGridHelper.SnapableForceDistance)
                    {
                        dist.Add(new SnapGridDistance { Distance = lastDistance, Location = SnapGridDirections.Y, Type = SnapType.Equal1, other = other });
                    }
                    else if ((lastDistance = SnapGridHelper.SnapableForceTop2(curObj, other)) < SnapGridHelper.SnapableForceDistance)
                    {
                        dist.Add(new SnapGridDistance { Distance = lastDistance, Location = SnapGridDirections.Y, Type = SnapType.Equal2, other = other });
                    }
                    else if ((lastDistance = SnapGridHelper.SnapableForceDistanceTop(curObj, other)) < SnapGridHelper.SnapableForceDistance)
                    {
                        dist.Add(new SnapGridDistance { Distance = lastDistance, Location = SnapGridDirections.Y, Type = SnapType.Distance, other = other });
                    }
                }
                if ((dir & SnapGridDirections.Bottom) == SnapGridDirections.Bottom)
                {
                    if ((lastDistance = SnapGridHelper.SnapableForceBottom1(curObj, other)) < SnapGridHelper.SnapableForceDistance)
                    {
                        dist.Add(new SnapGridDistance { Distance = lastDistance, Location = SnapGridDirections.Bottom, Type = SnapType.Equal1, other = other });
                    }
                    else if ((lastDistance = SnapGridHelper.SnapableForceBottom2(curObj, other)) < SnapGridHelper.SnapableForceDistance)
                    {
                        dist.Add(new SnapGridDistance { Distance = lastDistance, Location = SnapGridDirections.Bottom, Type = SnapType.Equal2, other = other });
                    }
                    else if ((lastDistance = SnapGridHelper.SnapableForceDistanceBottom(curObj, other)) < SnapGridHelper.SnapableForceDistance)
                    {
                        dist.Add(new SnapGridDistance { Distance = lastDistance, Location = SnapGridDirections.Bottom, Type = SnapType.Distance, other = other });
                    }
                }
            }

            if ((dir & SnapGridDirections.X) == SnapGridDirections.X)
            {
                if ((lastDistance = Math.Abs(0 - SnapGridHelper.Left(curObj))) < SnapGridHelper.SnapableForceDistance)
                {
                    dist.Add(new SnapGridDistance { Distance = lastDistance, Location = SnapGridDirections.X, Type = SnapType.Equal1, other = ZDash });
                }
                else if ((lastDistance = Math.Abs(SnapGridHelper.SnapDistance - SnapGridHelper.Left(curObj))) < SnapGridHelper.SnapableForceDistance)
                {
                    dist.Add(new SnapGridDistance { Distance = lastDistance, Location = SnapGridDirections.X, Type = SnapType.Distance, other = ZDash });
                }
            }
            if ((dir & SnapGridDirections.Right) == SnapGridDirections.Right)
            {
                if ((lastDistance = Math.Abs(ZDash.ActualWidth - SnapGridHelper.Right(curObj))) < SnapGridHelper.SnapableForceDistance)
                {
                    dist.Add(new SnapGridDistance { Distance = lastDistance, Location = SnapGridDirections.Right, Type = SnapType.Equal1, other = ZDash });
                }
                else if ((lastDistance = Math.Abs(ZDash.ActualWidth - SnapGridHelper.Right(curObj) - SnapGridHelper.SnapDistance)) < SnapGridHelper.SnapableForceDistance)
                {
                    dist.Add(new SnapGridDistance { Distance = lastDistance, Location = SnapGridDirections.Right, Type = SnapType.Distance, other = ZDash });
                }
            }
            if ((dir & SnapGridDirections.Y) == SnapGridDirections.Y)
            {
                if ((lastDistance = Math.Abs(0 - SnapGridHelper.Top(curObj))) < SnapGridHelper.SnapableForceDistance)
                {
                    dist.Add(new SnapGridDistance { Distance = lastDistance, Location = SnapGridDirections.Y, Type = SnapType.Equal1, other = ZDash });
                }
                else if ((lastDistance = Math.Abs(SnapGridHelper.SnapDistance - SnapGridHelper.Top(curObj))) < SnapGridHelper.SnapableForceDistance)
                {
                    dist.Add(new SnapGridDistance { Distance = lastDistance, Location = SnapGridDirections.Y, Type = SnapType.Distance, other = ZDash });
                }
            }
            if ((dir & SnapGridDirections.Bottom) == SnapGridDirections.Bottom)
            {
                if ((lastDistance = Math.Abs(ZDash.ActualHeight - SnapGridHelper.Bottom(curObj))) < SnapGridHelper.SnapableForceDistance)
                {
                    dist.Add(new SnapGridDistance { Distance = lastDistance, Location = SnapGridDirections.Bottom, Type = SnapType.Equal1, other = ZDash });
                }
                else if ((lastDistance = Math.Abs(ZDash.ActualHeight - SnapGridHelper.SnapDistance - SnapGridHelper.Bottom(curObj))) < SnapGridHelper.SnapableForceDistance)
                {
                    dist.Add(new SnapGridDistance { Distance = lastDistance, Location = SnapGridDirections.Bottom, Type = SnapType.Distance, other = ZDash });
                }
            }

            //Snap to code
            bool top = false, left = false, bottom = false, right = false;
            double topdi = Double.NaN, leftdi = Double.NaN, bottomdi = Double.NaN, rightdi = Double.NaN;
            double ty = 0;
            dist.Sort();
            foreach (SnapGridDistance item in dist)
            {
                double otherleft = SnapGridHelper.Left(item.other);
                double otherright = SnapGridHelper.Right(item.other);
                double othertop = SnapGridHelper.Top(item.other);
                double otherbottom = SnapGridHelper.Bottom(item.other);
                if (item.other == ZDash)
                {
                    otherright = otherbottom = 0;
                    otherleft = ZDash.ActualWidth;
                    othertop = ZDash.ActualHeight;
                }
                switch (item.Location)
                {
                    case SnapGridDirections.X:
                        if (!left)
                        {
                            left = true;
                            leftdi = item.Distance;
                            if (leftModifier != null)
                            {
                                if (item.Type == SnapType.Distance)
                                {
                                    leftModifier(otherright + SnapGridHelper.SnapDistance);
                                }
                                else if (item.Type == SnapType.Equal1)
                                {
                                    leftModifier(otherright);
                                }
                                else
                                {
                                    leftModifier(otherleft);
                                }
                            }
                        }
                        if (left && Math.Abs(leftdi - item.Distance) < SnapGridHelper.SnapableWithinDistance)
                        {
                            if (item.Type == SnapType.Distance)
                            {
                                ty = Math.Round(SnapGridHelper.SnapableDistanceLeftRightY(curObj, item.other)) + .5;
                                snaplines.Add(new SnapLine { y1 = ty, y2 = ty, color = SnapLine.DistanceLineColor, x1 = -SnapGridHelper.SnapDistance, x2 = 0 });
                            }
                            else
                            {
                                leftside.x1 = leftside.x2;
                                leftside.y1 = Math.Min(leftside.y1, othertop - Canvas.GetTop(curObj));
                                leftside.y2 = Math.Max(leftside.y2, othertop + item.other.Height - (Canvas.GetTop(curObj)));
                            }
                        }
                        break;
                    case SnapGridDirections.Y:
                        if (!top)
                        {
                            top = true;
                            topdi = item.Distance;
                            if (topModifier != null)
                            {
                                if (item.Type == SnapType.Distance)
                                {
                                    topModifier(otherbottom + SnapGridHelper.SnapDistance);
                                }
                                else if (item.Type == SnapType.Equal1)
                                {
                                    topModifier(otherbottom);
                                }
                                else
                                {
                                    topModifier(othertop);
                                }
                            }
                        }
                        if (top && Math.Abs(topdi - item.Distance) < SnapGridHelper.SnapableWithinDistance)
                        {
                            if (item.Type == SnapType.Distance)
                            {
                                ty = Math.Round(SnapGridHelper.SnapableDistanceTopBottomX(curObj, item.other)) + .5;
                                snaplines.Add(new SnapLine { color = SnapLine.DistanceLineColor, x1 = ty, x2 = ty, y1 = 0, y2 = -SnapGridHelper.SnapDistance });
                            }
                            else
                            {
                                topside.y1 = topside.y2;
                                topside.x1 = Math.Min(topside.x1, otherleft - Canvas.GetLeft(curObj));
                                topside.x2 = Math.Max(topside.x2, otherleft + item.other.Width - (Canvas.GetLeft(curObj)));
                            }
                        }
                        break;
                    case SnapGridDirections.Right:
                        if (!right)
                        {
                            right = true;
                            rightdi = item.Distance;
                            if (rightModifier != null)
                            {
                                if (item.Type == SnapType.Distance)
                                {
                                    rightModifier(otherleft - SnapGridHelper.SnapDistance);
                                }
                                else if (item.Type == SnapType.Equal1)
                                {
                                    rightModifier(otherleft);
                                }
                                else
                                {
                                    rightModifier(otherright);
                                }
                            }
                        }
                        if (right && Math.Abs(rightdi - item.Distance) < SnapGridHelper.SnapableWithinDistance)
                        {
                            if (item.Type == SnapType.Distance)
                            {
                                ty = Math.Round(SnapGridHelper.SnapableDistanceLeftRightY(curObj, item.other)) + .5;
                                snaplines.Add(new SnapLine { y1 = ty, y2 = ty, color = SnapLine.DistanceLineColor, x1 = curObj.Width + SnapGridHelper.SnapDistance, x2 = curObj.Width });
                            }
                            else
                            {
                                rightside.x1 = rightside.x2 = curObj.Width + 0.5;
                                rightside.y1 = Math.Min(rightside.y1, othertop - Canvas.GetTop(curObj));
                                rightside.y2 = Math.Max(rightside.y2, othertop + item.other.Height - (Canvas.GetTop(curObj)));
                            }
                        }
                        break;
                    case SnapGridDirections.Bottom:
                        if (!bottom)
                        {
                            bottom = true;
                            bottomdi = item.Distance;
                            if (bottomModifier != null)
                            {
                                if (item.Type == SnapType.Distance)
                                {
                                    bottomModifier(othertop - SnapGridHelper.SnapDistance);
                                }
                                else if (item.Type == SnapType.Equal1)
                                {
                                    bottomModifier(othertop);
                                }
                                else
                                {
                                    bottomModifier(otherbottom);
                                }
                            }
                        }
                        if (bottom && Math.Abs(bottomdi - item.Distance) < SnapGridHelper.SnapableWithinDistance)
                        {
                            if (item.Type == SnapType.Distance)
                            {
                                ty = Math.Round(SnapGridHelper.SnapableDistanceTopBottomX(curObj, item.other)) + .5;
                                snaplines.Add(new SnapLine { color = SnapLine.DistanceLineColor, x1 = ty, x2 = ty, y1 = curObj.Height, y2 = curObj.Height + SnapGridHelper.SnapDistance });

                            }
                            else
                            {
                                bottomside.y1 = bottomside.y2 = curObj.Height + 0.5;
                                bottomside.x1 = Math.Min(bottomside.x1, otherleft - Canvas.GetLeft(curObj));
                                bottomside.x2 = Math.Max(bottomside.x2, otherleft + item.other.Width - (Canvas.GetLeft(curObj)));
                            }
                        }
                        break;
                    default:
                        throw new InvalidOperationException("Enum not it!");
                }
            }


            if (leftside.x1 == leftside.x2)
                curObj.SetSnap(leftside);
            if (rightside.x1 == rightside.x2)
                curObj.SetSnap(rightside);
            if (topside.y1 == topside.y2)
                curObj.SetSnap(topside);
            if (bottomside.y1 == bottomside.y2)
                curObj.SetSnap(bottomside);

            foreach (var item in snaplines)
            {
                curObj.SetSnap(item);
            }

            curObj.DrawSnaps();
        }

        #endregion

        #region Designer Resizing

        private void ResizeGrip_MouseDown(object sender, MouseButtonEventArgs e)
        {
            resizingform = true;
            ResizeGrip.CaptureMouse();
            orfPoing = e.GetPosition(this);
            orfSixe = new Size(ZDash.ActualWidth, ZDash.ActualHeight);
            wsize = new Size(this.ActualWidth, this.ActualHeight);
        }

        private void ResizeGrip_MouseMove(object sender, MouseEventArgs e)
        {
            if (!resizingform)
                return;
            var ofv = e.GetPosition(this) - orfPoing;
            var newInnerSize = (Size)((Point)orfSixe + ofv);
            var newOuterSize = (Size)((Point)wsize + ofv);
            this.Width = newOuterSize.Width;
            this.Height = newOuterSize.Height;
            LayoutCvs.Width = (ZDChrome.Width = ZDash.Width = newInnerSize.Width) + 4;
            LayoutCvs.Height = (ZDChrome.Height = ZDash.Height = newInnerSize.Height) + 4;
        }

        private void ResizeGrip_MouseUp(object sender, MouseButtonEventArgs e)
        {
            resizingform = false;
            ResizeGrip.ReleaseMouseCapture();
        }

        #endregion

        #endregion

        #region Selection

        private void Select(SurfaceControl sl)
        {
            curObj = sl;
            curObj.Focus();
            UpdateSelected();
        }

        private void UpdateSelected()
        {
            if (curObj != null)
            {
                propHolder.Children.Clear();
                curObj.PopulateProperties(propHolder);
            }
            else
            {
                propHolder.Children.Clear();
                foreach (var item in designerProps)
                {
                    propHolder.Children.Add(item);
                }
            }
        }

        private void Deselect()
        {
            if (curObj != null)
            {
                curObj.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));//defocus
                ZDash.Focus();
            }
            curObj = null;
            UpdateSelected();
        }

        #endregion

        #region AutoAdd

        #region AADnD

        AutoPoint ap;

        private void listBox2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dndopoint = e.GetPosition(null);
            origSrc = e.OriginalSource;
            lbdragging = true;
            ap = FindAnchestor<AutoPoint>(LogicalTreeHelper.GetParent(FindTopAnchestor((DependencyObject)origSrc)));
        }

        private void listBox2_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                // Get the current mouse position
                Point mousePos = e.GetPosition(null);
                Vector diff = dndopoint - mousePos;

                if (lbdragging && (e.LeftButton == MouseButtonState.Pressed &&
                    Math.Abs(diff.X) + Math.Abs(diff.Y) > 3))
                {
                    // Get the dragged ListViewItem
                    ListBoxItem listViewItem =
                        FindAnchestor<ListBoxItem, WrapPanel>((DependencyObject)origSrc);

                    //System.Diagnostics.Debug.Print("Moving...");
                    // Find the data behind the ListViewItem
                    ZomBControlAttribute contact = (ZomBControlAttribute)FindAnchestor<ListBox>(listViewItem).ItemContainerGenerator.
                        ItemFromContainer(listViewItem);
                    //System.Diagnostics.Debug.Print("Found...");
                    // Initialize the drag & drop operation
                    DataObject dragData = new DataObject("ZomBControl2", contact);
                    DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Copy);
                    lbdragging = false;

                }
            }
            catch { }//System.Diagnostics.Debug.Print("FAIL!"); }
        }

        private void listBox2_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            lbdragging = false;
        }

        #endregion

        bool aaing;//A-A-R-D-V-A-R-K!
        Collection<IDashboardPeekableDataSource> peeps = new Collection<IDashboardPeekableDataSource>();

        private void AutoListen_Click(object sender, RoutedEventArgs e)
        {
            if (!aaing)
            {
                aadict.Clear();
                peeps.Clear();
                AutoAddPanel.Children.Clear();
                try
                {
                    foreach (var item in ZomBUrlSources)
                    {
                        if (typeof(IDashboardPeekableDataSource).IsAssignableFrom(item.SourceType))
                        {
                            try
                            {
                                peeps.Add((IDashboardPeekableDataSource)item.Exec(null));
                            }
                            catch { }
                        }
                    }
                    if (peeps.Count < 1)
                        throw new Exception();//wee!
                    foreach (var item in peeps)
                    {
                        item.BeginNamePeek(AddAutoStubPeek);
                        item.NewDataRecieved += AutoAddControlTypeHook;
                    }
                    aaing = true;
                    AutoPlay.Visibility = AAListen.Visibility = System.Windows.Visibility.Collapsed;
                    StopAutoPlay.Visibility = AAdeListen.Visibility = System.Windows.Visibility.Visible;
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Error Initializing AutoListener for specified sources, are they Peekable?");
                }
            }
        }

        private void StopListening_Click(object sender, RoutedEventArgs e)
        {
            if (aaing && peeps != null)
            {
                foreach (var item in peeps)
                {
                    item.EndNamePeek();
                    item.NewDataRecieved -= AutoAddControlTypeHook;
                }
                aaing = false;
                AutoPlay.Visibility = AAListen.Visibility = System.Windows.Visibility.Visible;
                StopAutoPlay.Visibility = AAdeListen.Visibility = System.Windows.Visibility.Collapsed;
                aadict.Clear();
                peeps.Clear();
                AutoAddPanel.Children.Clear();
                AutoAddPanel.Children.Clear();
            }
        }

        private void bc(object sender, RoutedEventArgs e)
        {
            AddAutoStub(Guid.NewGuid().ToString().Substring(0, 5));
        }

        Dictionary<string, AutoPoint> aadict = new Dictionary<string, AutoPoint>();

        private void AddAutoStubPeek(string name)
        {
            if (aaing)
                AddAutoStub(name);
        }

        private void AutoAddControlTypeHook(object sender, NewDataRecievedEventArgs e)
        {
            if (!aaing)
                return;
            foreach (var neweddata in e.NewData)
            {
                if (aadict.ContainsKey(neweddata.Key))
                {
                    var dk = aadict[neweddata.Key];
                    lock (dk)
                    {
                        if (dk.TypeHint != neweddata.Value.TypeHint)
                        {
                            if (dk.TypeHint == ZomBDataTypeHint.All || neweddata.Value.TypeHint == ZomBDataTypeHint.Unknown || neweddata.Value.TypeHint == ZomBDataTypeHint.All)
                                dk.TypeHint = 0;
                            dk.TypeHint |= neweddata.Value.TypeHint;
                            if ((dk.TypeHint & ZomBDataTypeHint.Lookup) == ZomBDataTypeHint.Lookup)
                            {
                                if ((neweddata.Value.Value is ZomBDataLookup) && (neweddata.Value.Value as ZomBDataLookup).ContainsKey("~TYPE~"))
                                {
                                    dk.TypeHintTag = (neweddata.Value.Value as ZomBDataLookup)["~TYPE~"];
                                }
                            }
                            Dispatcher.Invoke(new Utils.StringFunction(AutoAddResortTyped), neweddata.Key);
                        }
                    }
                }
            }
        }

        private void AutoAddResortTyped(string name)
        {
            try
            {
                var dk = aadict[name];
                List<ZomBControlAttribute> ls = new List<ZomBControlAttribute>(dk.Toolbox.Items.Count);
                foreach (ZomBControlAttribute titem in dk.Toolbox.ItemsSource)
                {
                    if ((dk.TypeHint & ZomBDataTypeHint.Lookup) == ZomBDataTypeHint.Lookup && dk.TypeHintTag != null && titem.Type != null)
                    {
                        titem.Star = false;
                        foreach (var item in titem.Type.GetCustomAttributes(typeof(ZomBCompositeAttribute), false))
                        {
                            if ((item as ZomBCompositeAttribute).CompositeName.ToLower() == dk.TypeHintTag.ToString().ToLower())
                            {
                                titem.Star = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        titem.Star = (((dk.TypeHint & titem.TypeHints) != 0));
                    }
                    ls.Add(titem);
                }
                dk.Toolbox.ItemsSource = from ZomBControlAttribute item in ls orderby !item.Star, item.Name select item;
            }
            catch (Exception e)
            {
                ErrorDialog.PrcException(e);
            }
        }

        private void AddAutoStub(string name)
        {
            if (!aadict.ContainsKey(name))
            {
                aadict.Add(name, new AutoPoint { Name = name, Toolbox = GetAAToolBoxClone() });
                var autop = aadict[name];
                DockPanel.SetDock(autop, Dock.Top);
                AutoAddPanel.Children.Add(autop);
            }
        }

        private ListBox GetAAToolBoxClone()
        {
            var lb = DeepClonetb();
            var ls = (from ZomBControlAttribute item in listBox1.ItemsSource
                      where item.Type.IsSubclassOf(typeof(ZomBGLControl))
                          || typeof(IZomBCompositeDescriptor).IsAssignableFrom(item.Type)
                      select item.Clone()).ToList();
            ls.Add(new ZomBControlAttribute("ZomB Name") { Description = "Drag this to a control and it will set the name to this AutoPoints name.", TypeHints = ZomBDataTypeHint.Unknown, IconName = "ZomBNameAddIcon", Icon = (ImageSource)System.Windows.Application.Current.FindResource("ZomBNameAddIcon" ?? "DefaultControlImage") });
            lb.ItemsSource = ls;
            lb.PreviewMouseLeftButtonDown += listBox2_PreviewMouseLeftButtonDown;
            lb.PreviewMouseUp += listBox2_PreviewMouseUp;
            lb.PreviewMouseMove += listBox2_PreviewMouseMove;
            return lb;
        }

        private ListBox DeepClonetb()
        {
            var s = "<ListBox xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" Grid.Column=\"0\" VerticalAlignment=\"Stretch\" ScrollViewer.HorizontalScrollBarVisibility=\"Disabled\"><ListBox.ItemTemplate><DataTemplate><ListBoxItem Padding=\"3\" HorizontalAlignment=\"Stretch\" ToolTipService.InitialShowDelay=\"1000\" ToolTipService.ShowDuration=\"10000\"><ListBoxItem.ToolTip><StackPanel Width=\"200\"><Label FontWeight=\"Bold\" Background=\"Blue\" Foreground=\"White\" HorizontalAlignment=\"Stretch\" HorizontalContentAlignment=\"Center\" Content=\"{Binding Path=Name}\"></Label><TextBlock Padding=\"10\" TextWrapping=\"Wrap\" Text=\"{Binding Path=Description}\"></TextBlock></StackPanel></ListBoxItem.ToolTip><Canvas Width=\"32\" Height=\"32\" Margin=\"2\" SnapsToDevicePixels=\"True\"><Image Width=\"32\" Height=\"32\" Source=\"{Binding Path=Icon}\" VerticalAlignment=\"Center\" HorizontalAlignment=\"Center\" SnapsToDevicePixels=\"True\" /><Path Data=\"M 32.84375 -5.0625 L 30.53125 -2.34375 L 26.96875 -2.71875 L 28.84375 0.34375 L 27.40625 3.59375 L 30.875 2.78125 L 33.53125 5.15625 L 33.8125 1.59375 L 36.90625 -0.1875 L 33.59375 -1.5625 L 32.84375 -5.0625 z \"><Path.Style><Style TargetType=\"{x:Type Path}\"><Setter Property=\"Fill\" Value=\"Transparent\" /><Style.Triggers><DataTrigger Value=\"True\"><DataTrigger.Binding><Binding Path=\"Star\" x:Name=\"StarBnd\" /></DataTrigger.Binding><Setter Property=\"Fill\" Value=\"#FFFFBB00\" /></DataTrigger></Style.Triggers></Style></Path.Style></Path></Canvas></ListBoxItem></DataTemplate></ListBox.ItemTemplate><ListBox.ItemsPanel><ItemsPanelTemplate><WrapPanel /></ItemsPanelTemplate></ListBox.ItemsPanel></ListBox>";
            return (ListBox)XamlReader.Parse(s);
        }

        #endregion

        #region ZIndex

        private void CommandBinding_MoveTop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (curObj != null)
            {
                ZDash.Children.Remove(curObj);
                ZDash.Children.Insert(ZDash.Children.Count, curObj);
            }
        }

        private void CommandBinding_MoveUp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (curObj != null)
            {
                var i = ZDash.Children.IndexOf(curObj);
                ZDash.Children.Remove(curObj);
                if (i + 1 >= ZDash.Children.Count)
                    i = ZDash.Children.Count - 1;
                ZDash.Children.Insert(i + 1, curObj);
            }
        }

        private void CommandBinding_MoveDown_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (curObj != null)
            {
                var i = ZDash.Children.IndexOf(curObj);
                ZDash.Children.Remove(curObj);
                if (i <= 1)
                    i = 1;
                ZDash.Children.Insert(i - 1, curObj);
            }
        }

        private void CommandBinding_MoveBottom_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (curObj != null)
            {
                ZDash.Children.Remove(curObj);
                ZDash.Children.Insert(0, curObj);
            }
        }

        #endregion

        #region Menu

        public static readonly RoutedUICommand Deploy = new RoutedUICommand(
            "Deploy",
            "Deploy",
            typeof(Designer),
            new InputGestureCollection { new KeyGesture(Key.F6) });

        internal void CommandBinding_Deploy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new BuildSettings(Export()).ShowDialog();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            new About().ShowDialog();
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new SettingsManager().ShowDialog();
        }

        private void HelpMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + "\\ViZ.exe", "help");
        }

        #endregion

        #region Other

        private void ZDash_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (e.Source is Button || VisualTreeHelper.GetParent((DependencyObject)e.Source) is Button || VisualTreeHelper.GetParent(VisualTreeHelper.GetParent((DependencyObject)e.Source)) is Button) //resize, TODO: add PART_xxx detection
                {
                    origSrc = FindAnchestor<SurfaceControl>((DependencyObject)e.OriginalSource);
                    Deselect();
                    Select(origSrc as SurfaceControl);
                }
                else if (e.Source is SurfaceControl || VisualTreeHelper.GetParent((DependencyObject)e.Source) is SurfaceControl) //resize, TODO: add PART_xxx detection
                {
                    origSrc = FindAnchestor<SurfaceControl>((DependencyObject)e.OriginalSource);
                    Deselect();
                    Select(origSrc as SurfaceControl);
                }
            }
        }

        private void CommandBinding_Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (curObj != null)
            {
                ZDash.Children.Remove(curObj);
                curObj = null;
            }
        }

        public static Designer getDesigner()
        {
            return dsb;
        }

        #endregion

        #endregion

        #region Save and Run

        public static readonly RoutedUICommand Run = new RoutedUICommand(
            "Run",
            "Run",
            typeof(Designer),
            new InputGestureCollection { new KeyGesture(Key.F5) });

        public static readonly RoutedUICommand SaveAs = new RoutedUICommand(
            "SaveAs",
            "SaveAs",
            typeof(Designer),
            new InputGestureCollection { new KeyGesture(Key.S, ModifierKeys.Alt | ModifierKeys.Control) });


        internal void CommandBinding_Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            StopListening_Click(sender, e);
            RunApp();
        }

        private void CommandBinding_New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Deselect();
            ZDash.Children.Clear();
            Deselect();
            lastSave = "";
        }

        internal void CommandBinding_Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveApp();
        }

        internal void CommandBinding_SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveAppAs();
        }

        internal void CommandBinding_Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.DefaultExt = ".zaml";
            dlg.SupportMultiDottedExtensions = true;
            dlg.Filter = "ZomB App Markup Files (*.zaml)|*.zaml;*.xaml|All Files|*.*";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                lastSave = dlg.FileName;
                LoadFile(dlg.FileName);
            }
        }

        public void PreLoadFile(string fileName)
        {
            this.preFile = fileName;
        }

        public void LoadFile(string fileName)
        {
            try
            {
                StoppedDDHCVS cvs = XamlReader.Load(new MemoryStream(UTF8Encoding.UTF8.GetBytes(new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    .ReadToEnd().Replace("ZomB:DashboardDataCanvas ",
                    "ViZ:StoppedDDHCVS xmlns:ViZ=\"clr-namespace:System451.Communication.Dashboard.ViZ;assembly=ViZ\" ")
                    .Replace("/ZomB:DashboardDataCanvas",
                    "/ViZ:StoppedDDHCVS")))) as StoppedDDHCVS;
                if (cvs == null)
                    return;
                List<UIElement> lc = new List<UIElement>(cvs.Children.Count);

                //canvas size
                var ofv = new Point(cvs.Width, cvs.Height) - new Point(ZDash.ActualWidth, ZDash.ActualHeight);
                var newInnerSize = (Size)(new Point(ZDash.ActualWidth, ZDash.ActualHeight) + ofv);
                var newOuterSize = (Size)(new Point(this.ActualWidth, this.ActualHeight) + ofv);
                this.Width = newOuterSize.Width;
                this.Height = newOuterSize.Height;
                LayoutCvs.Width = (ZDChrome.Width = ZDash.Width = newInnerSize.Width) + 4;
                LayoutCvs.Height = (ZDChrome.Height = ZDash.Height = newInnerSize.Height) + 4;

                (designerProps[3] as ComboBox).SelectedIndex = ((int)cvs.InvalidPacketAction) - 1;//this hinges on the values
                ((designerProps[5] as FrameworkElement).Tag as ZomBUrlCollectionDesigner).Set((cvs.DefaultSources));
                (designerProps[7] as TextBox).Text = cvs.Team.ToString();

                foreach (UIElement item in cvs.Children)
                {
                    var type = item.GetType();
                    if (type.GetCustomAttributes(typeof(ZomBControlAttribute), true).Length > 0 || type == typeof(ZomBGLControlGroup))
                    {
                        lc.Add(item);
                    }
                }
                ZDash.Children.Clear();
                foreach (var item in lc)
                {
                    cvs.Children.Remove(item);
                    AddControl((FrameworkElement)item);
                }
                Deselect();
            }
            catch (Exception ex)
            {
                ErrorDialog.PrcException(ex);
            }
        }

        private void SaveAppAs()
        {
            try
            {
                var dlg = new System.Windows.Forms.SaveFileDialog();
                dlg.DefaultExt = ".zaml";
                dlg.AddExtension = true;
                dlg.SupportMultiDottedExtensions = true;
                dlg.Filter = "ZomB App Markup Files (*.zaml)|*.zaml|All Files|*.*";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    lastSave = dlg.FileName;
                    SaveApp();
                }
            }
            catch (Exception ex)
            {
                ErrorDialog.PrcException(ex);
            }
        }

        private string lastSave = "";
        private void SaveApp()
        {
            try
            {
                if (lastSave != "")
                {
                    string zaml = Export();
                    File.WriteAllText(lastSave, zaml);
                }
                else
                    SaveAppAs();
            }
            catch (Exception ex)
            {
                ErrorDialog.PrcException(ex);
            }
        }

        private void RunApp()
        {
            var pw = new WPF.ProgressDialog();

            pw.Status = "Initializing...";
            pw.Show();
            ZDesigner.SetDesigner(null);

            pw.Status = "Generating Zaml...";
            string zaml = Export();
            try
            {

                pw.Status = "Initializing process...";
                {
                    Run r = new Run(zaml);
                    try
                    {
                        pw.Close();
                        r.ShowDialog();
                    }
                    catch { }
                    try
                    {
                        r.DashboardDataHub.Stop();
                        r.StopAll();
                    }
                    catch { }
                    r = null;
                }
                GC.Collect();
                try
                {
                    ZDesigner.SetDesigner(this);
                }
                catch { }
            }
            catch (Exception ex)
            {
                ErrorDialog.PrcException(ex);
            }
        }

        private void StaRun(string zaml, AppRunner r, WPF.ProgressDialog pw)
        {
            Thread thr = new Thread(delegate()
            {
                r.Run(zaml);
                Dispatcher.Invoke(new VoidFunction(() => pw.Close()));
                r.Start();
                r = null;
            });
            thr.SetApartmentState(ApartmentState.STA);
            thr.Start();
            thr.Join();
        }

        public string Export()
        {
            StringBuilder sb = new StringBuilder("<ZomB:DashboardDataCanvas xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" ");
            sb.Append(GetZamlAllNSs());
            sb.Append("Height=\"" + ZDash.ActualHeight + "\" Width=\"" + ZDash.ActualWidth + "\" InvalidPacketAction=\"");
            sb.Append((designerProps[3] as ComboBox).SelectedValue);
            sb.Append("\" DefaultSources=\"");
            sb.Append(((designerProps[5] as FrameworkElement).Tag as ZomBUrlCollectionDesigner).GetValue());
            sb.Append("\" Team=\"");
            sb.Append((designerProps[7] as TextBox).Text);
            sb.Append("\">");
            List<KeyValuePair<string, PropertyElement>> attached = new List<KeyValuePair<string, PropertyElement>>();

            foreach (var item in LogicalTreeHelper.GetChildren(ZDash))
            {
                string xns = GetXmlNS(((SurfaceControl)item).Control);
                sb.Append("<");
                sb.Append(xns);
                sb.Append(":");
                sb.Append(((SurfaceControl)item).Control.GetType().Name);
                if (((SurfaceControl)item).Control.Name != "")
                {
                    sb.Append(" Name=\"");
                    sb.Append(((SurfaceControl)item).Control.Name);
                    sb.Append("\" Canvas.Top=\"");
                }
                else
                    sb.Append(" Canvas.Top=\"");
                sb.Append(Canvas.GetTop(item as UIElement));
                sb.Append("\" Canvas.Left=\"");
                sb.Append(Canvas.GetLeft(item as UIElement));
                foreach (KeyValuePair<string, PropertyElement> cprops in ((SurfaceControl)item).GetProps())
                {
                    if ((cprops.Value.Designer != null && cprops.Value.Designer.IsDefaultValue()) || cprops.Value.Value.ToString() == "")
                        continue;
                    if (cprops.Value.Designer != null && cprops.Value.Designer.IsExpanded())
                    {
                        attached.Add(cprops);
                        continue;
                    }
                    sb.Append("\" ");
                    sb.Append(cprops.Key);
                    sb.Append("=\"");
                    if (cprops.Value.Designer != null)
                        sb.Append(cprops.Value.Designer.GetValue());
                    else
                        sb.Append(cprops.Value.Value.ToString());
                }
                if (attached.Count > 0)
                {
                    string ctrlNom = ((SurfaceControl)item).Control.GetType().Name;
                    sb.Append("\">");
                    foreach (var aprop in attached)
                    {
                        sb.Append("<");
                        sb.Append(xns);
                        sb.Append(":");
                        sb.Append(ctrlNom);
                        sb.Append(".");
                        sb.Append(aprop.Key);
                        sb.Append(">");
                        sb.Append(aprop.Value.Designer.GetValue());
                        sb.Append("</");
                        sb.Append(xns);
                        sb.Append(":");
                        sb.Append(ctrlNom);
                        sb.Append(".");
                        sb.Append(aprop.Key);
                        sb.Append(">");
                    }
                    attached.Clear();
                    sb.Append("</");
                    sb.Append(xns);
                    sb.Append(":");
                    sb.Append(ctrlNom);
                    sb.Append(">");
                }
                else
                    sb.Append("\" />");
            }

            sb.Append("</ZomB:DashboardDataCanvas>");
            return sb.ToString();
        }

        private string GetXmlNS(object item)
        {
            return xmlNSmappings[item.GetType().Namespace];
        }

        private string GetZamlAllNSs()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var ns in asm.GetCustomAttributes(typeof(WPF.Design.ZomBZamlNamespaceAttribute), false))
                {
                    var zn = ns as WPF.Design.ZomBZamlNamespaceAttribute;
                    sb.Append("xmlns:");
                    sb.Append(zn.XmlNamespace);
                    sb.Append("=\"clr-namespace:");
                    sb.Append(zn.ClrNamespace);
                    sb.Append(";assembly=");
                    string an = asm.FullName;
                    if (an.Contains(","))
                        an = an.Substring(0, an.IndexOf(','));
                    sb.Append(an);
                    sb.Append("\" ");
                }
            }
            return sb.ToString();
        }

        #endregion

        public static string[] GetNames()
        {
            var th = getDesigner().ZDash.Children;
            return (from SurfaceControl child in th select child.Control.Name).ToArray();
        }

        public static string[] GetProperties(string controlname)
        {
            var th = getDesigner().ZDash.Children;
            return (from prop in
                        (from SurfaceControl child in th where child.Control.Name == controlname select child.Control).First().GetType().GetProperties()
                    select prop.Name).ToArray();
        }

        public static object GetByName(string controlname)
        {
            var th = getDesigner().ZDash.Children;
            return (from SurfaceControl child in th where child.Control.Name == controlname select child.Control).First();
        }

        #region ISurfaceDesigner Members

        public UIElement[] GetChildren()
        {
            var th = getDesigner().ZDash.Children;
            return (from SurfaceControl child in th select child.Control).ToArray();
        }

        public bool ChildrenContain(string name)
        {
            var th = getDesigner().ZDash.Children;
            return (from SurfaceControl child in th where child.Control.Name == name select child.Control).Count() > 0;
        }

        public bool ChildrenContain(UIElement element)
        {
            var th = getDesigner().ZDash.Children;
            return (from SurfaceControl child in th where child.Control == element select child.Control).Count() > 0;
        }

        public UIElement GetChildByName(string name)
        {
            var th = getDesigner().ZDash.Children;
            var rs = (from SurfaceControl child in th where child.Control.Name == name select child.Control);
            if (rs.Count() < 1)
                return null;
            return rs.First();
        }

        public int GetTeamNumber()
        {
            try
            {
                return int.Parse((designerProps[7] as TextBox).Text);
            }
            catch { return 0; }
        }

        void ISurfaceDesigner.Highlight(string name)
        {
            var th = getDesigner().ZDash.Children;
            var rs = (from SurfaceControl child in th where child.Control.Name == name select child);
            if (rs.Count() > 0)
                Highlight((Control)rs.First());
            else
                this.Highlight((Control)null);
        }

        #endregion

        Control lastlight = null;

        public void Highlight(Control result)
        {
            if (lastlight != null)
            {
                lastlight.BorderBrush = null;
                lastlight.BorderThickness = new Thickness(0);
            }
            lastlight = result;
            if (lastlight != null)
            {
                lastlight.BorderBrush = Brushes.Red;
                lastlight.BorderThickness = new Thickness(4);
            }
        }

        public void SaveAsProfile(string profileName)
        {
            //team#|Ignore/continue|urls
            // invalid packets: 3 source: 5 team: 7 (designerProps[2] as Label)
            string profileString = "";
            profileString += (designerProps[7] as TextBox).Text + "|";
            profileString += (designerProps[3] as ComboBox).Text + "|";
            profileString += ZomBUrlSources;
            if (Settings.Default.Profile == null)
                Settings.Default.Profile = new System.Collections.Specialized.StringDictionary();
            if (Settings.Default.Profile.ContainsKey(profileName))
            {
                Settings.Default.Profile[profileName] = profileString;
            }
            else
            {
                Settings.Default.Profile.Add(profileName, profileString);
            }
            Settings.Default.Save();
        }

        public void DeleteProfile(string profileName)
        {
            Settings.Default.Profile.Remove(profileName);
            Settings.Default.Save();
        }

        public void LoadProfile(string profileName)
        {
            string profileString = Settings.Default.Profile[profileName];

            string teamnumberp, sourcep, invalidactionp;
            teamnumberp = profileString.Substring(0, profileString.IndexOf('|'));
            profileString = profileString.Substring(profileString.IndexOf('|') + 1);
            invalidactionp = profileString.Substring(0, profileString.IndexOf('|'));
            sourcep = profileString.Substring(profileString.IndexOf('|') + 1);
            (designerProps[7] as TextBox).Text = teamnumberp;
            (designerProps[3] as ComboBox).Text = invalidactionp;
            ZomBUrlSources = sourcep;
            ((designerProps[5] as FrameworkElement).Tag as ZomBUrlCollectionDesigner).Set(ZomBUrlSources);
        }
    }
    public static class ExtensionsBit
    {
        public static bool Flagged(this Designer.CurrentDragMove ths, Designer.CurrentDragMove value)
        {
            return (ths & value) == value;
        }
    }

    public class StoppedDDHCVS : WPF.Controls.DashboardDataCanvas
    {
        public StoppedDDHCVS()
            : base(false)
        {
            this.AutoStart = false;
        }
    }
}
