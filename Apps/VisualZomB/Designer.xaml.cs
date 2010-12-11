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
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System451.Communication.Dashboard.WPF.Design;

namespace System451.Communication.Dashboard.ViZ
{
    public partial class Designer : Window
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

        Point dndopoint, opoint, oxpoint;
        object origSrc;
        bool lbdragging = false;
        CurrentDrag cd = CurrentDrag.None;
        CurrentDragMove cdm = CurrentDragMove.None;
        List<FrameworkElement> designerProps = new List<FrameworkElement>(4);

        SurfaceControl curObj;

        Toolbox tbx;
        ListBox listBox1;
        Panel propHolder;

        bool resizingform = false;
        Point orfPoing;
        Size orfSixe, wsize;

        static Designer dsb = null;

        public Designer()
        {
            if (dsb != null)
                throw new InvalidOperationException("Only one ViZ designer can be active at a time in this App Domain");
            dsb = this;
            InitializeComponent();
            if (System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width > 1080)
            {
                //chromify, we have space
                this.AllowsTransparency = false;
                Scrlview.Background = Brushes.LightGray;
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.Width = 1080;
                this.Height = 470;
            }
            tbx = new Toolbox();
            listBox1 = tbx.ToolListBox;
            listBox1.PreviewMouseLeftButtonDown += listBox1_PreviewMouseLeftButtonDown;
            listBox1.PreviewMouseUp += listBox1_PreviewMouseUp;
            listBox1.PreviewMouseMove += listBox1_PreviewMouseMove;
            propHolder = tbx.PropertyBox;
            this.Top = (System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height <= 600) ? -1 : (System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - (this.Height + tbx.Height)) / 2.0;
            this.Left = Math.Max(-1.0, (System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 2.0) - this.Width / 2.0);
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
            designerProps.Add(new ComboBox());
            cb = (designerProps[5] as ComboBox);
            foreach (var item in Enum.GetNames(typeof(StartSources)))
            {
                cb.Items.Add(item);
                if (item == "DashboardPacket")
                {
                    cb.SelectedValue = item;
                }
            }
            designerProps.Add(new Label());
            (designerProps[6] as Label).Content = "Team #:";
            designerProps.Add(new TextBox());
            (designerProps[7] as TextBox).Text = "0";
            //End of massivly wrong code
            //TODO: clean up the above code

            propHolder.Children.Clear();
            foreach (var item in designerProps)
            {
                propHolder.Children.Add(item);
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            if (Environment.OSVersion.Version.Major >= 6)
            {
                if (Utils.AeroGlass.GlassifyWindow(this))
                    Scrlview.Background = Brushes.Transparent;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbx.Top = this.Top + this.ActualHeight - 2.0;
            tbx.Left = this.Left + (this.ActualWidth / 2.0) - (tbx.Width / 2.0);
            tbx.Show();
            tbx.Owner = this;
            tbx.Closed += new EventHandler(tbx_Closed);
            listBox1.ItemsSource = Reflector.GetZomBDesignableInfos(Reflector.GetZomBDesignableClasses());
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
            tbx.Close();
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
                    ZomBControlAttribute contact = (ZomBControlAttribute)listBox1.ItemContainerGenerator.
                        ItemFromContainer(listViewItem);
                    //System.Diagnostics.Debug.Print("Found...");
                    // Initialize the drag & drop operation
                    DataObject dragData = new DataObject("ZomBControl", contact);
                    DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Copy);
                    lbdragging = false;

                }
            }
            catch { }//System.Diagnostics.Debug.Print("FAIL!"); }
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

        private void ZDash_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("ZomBControl"))
            {
                ZomBControlAttribute info = (e.Data.GetData("ZomBControl") as ZomBControlAttribute);
                if (info != null)
                    AddControl((ZomBControlAttribute)info, e.GetPosition(ZDash));
            }
        }

        private void ZDash_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("ZomBControl"))
            {
                e.Effects = DragDropEffects.None;
            }
        }

        #endregion

        private void AddControl(ZomBControlAttribute info)
        {
            AddControl(info, new Point(5.0, 5.0));
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
                        Canvas.SetLeft((UIElement)origSrc, Math.Min(Math.Max(0, opoint.X + mv.X), ZDash.Width - (origSrc as SurfaceControl).Width));
                        Canvas.SetTop((UIElement)origSrc, Math.Min(Math.Max(0, opoint.Y + mv.Y), ZDash.Height - (origSrc as SurfaceControl).Height));
                        ShowSnaps(SnapGridDirections.All, (x) => Canvas.SetLeft(curObj, x), (y) => Canvas.SetTop(curObj, y), (r) => Canvas.SetLeft(curObj, r - SnapGridHelper.Right(curObj) + SnapGridHelper.Left(curObj)), (b) => Canvas.SetTop(curObj, b - SnapGridHelper.Bottom(curObj) + SnapGridHelper.Top(curObj)));
                    }
                    break;
                case CurrentDrag.Resize:
                    {
                        Vector mv = e.GetPosition(ZDash) - dndopoint;
                        var sc = (origSrc as SurfaceControl);
                        if (cdm.Flagged(CurrentDragMove.Width))
                            sc.Width = Math.Min(Math.Max(0, opoint.X + mv.X), ZDash.Width - Canvas.GetLeft((UIElement)origSrc));
                        if (cdm.Flagged(CurrentDragMove.Height))
                            sc.Height = Math.Min(Math.Max(0, opoint.Y + mv.Y), ZDash.Height - Canvas.GetTop((UIElement)origSrc));
                        if (cdm.Flagged(CurrentDragMove.X))
                        {
                            Canvas.SetLeft(sc, Math.Min(Math.Max(0, oxpoint.X + mv.X), Canvas.GetLeft(sc) + sc.Width));
                            sc.Width = Math.Min(Math.Max(0, opoint.X - mv.X), ZDash.Width - Canvas.GetLeft((UIElement)origSrc));
                        }
                        if (cdm.Flagged(CurrentDragMove.Y))
                        {
                            Canvas.SetTop(sc, Math.Min(Math.Max(0, oxpoint.Y + mv.Y), Canvas.GetTop(sc) + sc.Height));
                            sc.Height = Math.Min(Math.Max(0, opoint.Y - mv.Y), ZDash.Height - Canvas.GetTop((UIElement)origSrc));
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

            //Snap to code
            bool top = false, left = false, bottom = false, right = false;
            double topdi = Double.NaN, leftdi = Double.NaN, bottomdi = Double.NaN, rightdi = Double.NaN;
            double ty = 0;
            dist.Sort();
            foreach (SnapGridDistance item in dist)
            {
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
                                    leftModifier(SnapGridHelper.Right(item.other) + SnapGridHelper.SnapDistance);
                                }
                                else if (item.Type == SnapType.Equal1)
                                {
                                    leftModifier(SnapGridHelper.Right(item.other));
                                }
                                else
                                {
                                    leftModifier(SnapGridHelper.Left(item.other));
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
                                leftside.y1 = Math.Min(leftside.y1, Canvas.GetTop(item.other) - Canvas.GetTop(curObj));
                                leftside.y2 = Math.Max(leftside.y2, Canvas.GetTop(item.other) + item.other.Height - (Canvas.GetTop(curObj)));
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
                                    topModifier(SnapGridHelper.Bottom(item.other) + SnapGridHelper.SnapDistance);
                                }
                                else if (item.Type == SnapType.Equal1)
                                {
                                    topModifier(SnapGridHelper.Bottom(item.other));
                                }
                                else
                                {
                                    topModifier(SnapGridHelper.Top(item.other));
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
                                topside.x1 = Math.Min(topside.x1, Canvas.GetLeft(item.other) - Canvas.GetLeft(curObj));
                                topside.x2 = Math.Max(topside.x2, Canvas.GetLeft(item.other) + item.other.Width - (Canvas.GetLeft(curObj)));
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
                                    rightModifier(SnapGridHelper.Left(item.other) - SnapGridHelper.SnapDistance);
                                }
                                else if (item.Type == SnapType.Equal1)
                                {
                                    rightModifier(SnapGridHelper.Left(item.other));
                                }
                                else
                                {
                                    rightModifier(SnapGridHelper.Right(item.other));
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
                                rightside.y1 = Math.Min(rightside.y1, Canvas.GetTop(item.other) - Canvas.GetTop(curObj));
                                rightside.y2 = Math.Max(rightside.y2, Canvas.GetTop(item.other) + item.other.Height - (Canvas.GetTop(curObj)));
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
                                    bottomModifier(SnapGridHelper.Top(item.other) - SnapGridHelper.SnapDistance);
                                }
                                else if (item.Type == SnapType.Equal1)
                                {
                                    bottomModifier(SnapGridHelper.Top(item.other));
                                }
                                else
                                {
                                    bottomModifier(SnapGridHelper.Bottom(item.other));
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
                                bottomside.x1 = Math.Min(bottomside.x1, Canvas.GetLeft(item.other) - Canvas.GetLeft(curObj));
                                bottomside.x2 = Math.Max(bottomside.x2, Canvas.GetLeft(item.other) + item.other.Width - (Canvas.GetLeft(curObj)));
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

        private void CommandBinding_Deploy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (ZomBBuilder.BuildZomBString(Export(), @"C:\Program Files\FRC Dashboard\Dashboard.exe"))
            {
                MessageBox.Show(@"Success! Dashboard.exe written to C:\Program Files\FRC Dashboard\Dashboard.exe");
            }
            else
                MessageBox.Show("Error building exe");
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Utils.InstallUtils.Install();
            MessageBox.Show("Success!\r\n\r\nPlease restart this application to load the win32 modules.");
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

        private void CommandBinding_Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RunApp();
        }

        private void CommandBinding_Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveApp();
        }

        private void CommandBinding_Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.DefaultExt = ".zaml";
            dlg.SupportMultiDottedExtensions = true;
            dlg.Filter = "ZomB App Markup Files (*.zaml)|*.zaml;*.xaml|All Files|*.*";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadFile(dlg.FileName);
            }
        }

        public void LoadFile(string fileName)
        {
            StoppedDDHCVS cvs = XamlReader.Load(new MemoryStream(UTF8Encoding.UTF8.GetBytes(new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                .ReadToEnd().Replace("ZomB:DashboardDataCanvas ",
                "ViZ:StoppedDDHCVS xmlns:ViZ=\"clr-namespace:System451.Communication.Dashboard.ViZ;assembly=ViZ\" ")
                .Replace("/ZomB:DashboardDataCanvas",
                "/ViZ:StoppedDDHCVS")))) as StoppedDDHCVS;
            if (cvs == null)
                return;
            List<Control> lc = new List<Control>(cvs.Children.Count);

            //canvas size
            var ofv = new Point(cvs.Width, cvs.Height) - new Point(ZDash.ActualWidth, ZDash.ActualHeight);
            var newInnerSize = (Size)(new Point(ZDash.ActualWidth, ZDash.ActualHeight) + ofv);
            var newOuterSize = (Size)(new Point(this.ActualWidth, this.ActualHeight) + ofv);
            this.Width = newOuterSize.Width;
            this.Height = newOuterSize.Height;
            LayoutCvs.Width = (ZDChrome.Width = ZDash.Width = newInnerSize.Width) + 4;
            LayoutCvs.Height = (ZDChrome.Height = ZDash.Height = newInnerSize.Height) + 4;

            (designerProps[3] as ComboBox).SelectedIndex = ((int)cvs.InvalidPacketAction) - 1;//this hinges on the values
            (designerProps[5] as ComboBox).SelectedIndex = ((int)cvs.DefaultSources) - 1;//this hinges on the values
            (designerProps[7] as TextBox).Text = cvs.Team.ToString();

            foreach (UIElement item in cvs.Children)
            {
                if (item.GetType().GetCustomAttributes(typeof(ZomBControlAttribute), true).Length > 0)
                {
                    lc.Add((Control)item);
                }
            }
            ZDash.Children.Clear();
            foreach (var item in lc)
            {
                cvs.Children.Remove(item);
                AddControl(item);
            }
            Deselect();
        }

        private void SaveApp()
        {
            var dlg = new System.Windows.Forms.SaveFileDialog();
            dlg.DefaultExt = ".zaml";
            dlg.AddExtension = true;
            dlg.SupportMultiDottedExtensions = true;
            dlg.Filter = "ZomB App Markup Files (*.zaml)|*.zaml|All Files|*.*";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string zaml = Export();
                File.WriteAllText(dlg.FileName, zaml);
            }
        }

        private void RunApp()
        {
            string zaml = Export();
            new Run(zaml).ShowDialog();
        }

        public string Export()
        {
            StringBuilder sb = new StringBuilder("<ZomB:DashboardDataCanvas xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:ZomB=\"clr-namespace:System451.Communication.Dashboard.WPF.Controls;assembly=ZomB\" Height=\"" + ZDash.ActualHeight + "\" Width=\"" + ZDash.ActualWidth + "\" InvalidPacketAction=\"");
            sb.Append((designerProps[3] as ComboBox).SelectedValue);
            sb.Append("\" DefaultSources=\"");
            sb.Append((designerProps[5] as ComboBox).SelectedValue);
            sb.Append("\" Team=\"");
            sb.Append((designerProps[7] as TextBox).Text);
            sb.Append("\">");
            List<KeyValuePair<string, PropertyElement>> attached = new List<KeyValuePair<string, PropertyElement>>();

            foreach (var item in LogicalTreeHelper.GetChildren(ZDash))
            {
                sb.Append("<ZomB:");
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
                    if ((cprops.Value.Designer != null && cprops.Value.Designer.IsDefaultValue()) || cprops.Value.Value.ToString() == "" || (cprops.Value.Designer != null && cprops.Value.Designer.GetValue() == ""))
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
                        sb.Append("<ZomB:");
                        sb.Append(ctrlNom);
                        sb.Append(".");
                        sb.Append(aprop.Key);
                        sb.Append(">");
                        sb.Append(aprop.Value.Designer.GetValue());
                        sb.Append("</ZomB:");
                        sb.Append(ctrlNom);
                        sb.Append(".");
                        sb.Append(aprop.Key);
                        sb.Append(">");
                    }
                    attached.Clear();
                    sb.Append("</ZomB:");
                    sb.Append(ctrlNom);
                    sb.Append(">");
                }
                else
                    sb.Append("\" />");
            }

            sb.Append("</ZomB:DashboardDataCanvas>");
            return sb.ToString();
        }

        #endregion
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
