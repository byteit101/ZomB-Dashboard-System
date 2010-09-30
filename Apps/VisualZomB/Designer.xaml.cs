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
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System451.Communication.Dashboard.WPF;
using System451.Communication.Dashboard.WPF.Controls;
using System451.Communication.Dashboard.WPF.Design;

namespace System451.Communication.Dashboard.ViZ
{
    public partial class Designer : Window
    {
        enum CurrentDrag
        {
            None, Move, Resize
        }
        Point dndopoint, opoint;
        object origSrc;
        bool lbdragging = false;
        CurrentDrag cd = CurrentDrag.None;

        SurfaceControl curObj;

        public Designer()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            listBox1.ItemsSource = Reflector.GetZomBDesignableInfos(Reflector.GetZomBDesignableClasses());
        }

        #region AddControl

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
                        FindAnchestor<ListBoxItem, VirtualizingStackPanel>((DependencyObject)origSrc);

                    //System.Diagnostics.Debug.Print("Moving...");
                    // Find the data behind the ListViewItem
                    ZomBDesignableControlInfo contact = (ZomBDesignableControlInfo)listBox1.ItemContainerGenerator.
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

        // Helper to search up the VisualTree
        private static T FindAnchestor<T, P>(DependencyObject current) where T : DependencyObject
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
        private static T FindAnchestor<T>(DependencyObject current) where T : DependencyObject
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
                ZomBDesignableControlInfo? info = (e.Data.GetData("ZomBControl") as ZomBDesignableControlInfo?);
                if (info != null)
                    AddControl((ZomBDesignableControlInfo)info, e.GetPosition(ZDash));
            }
        }

        private void ZDash_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("ZomBControl"))
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void AddControl(ZomBDesignableControlInfo info)
        {
            AddControl(info, new Point(5.0, 5.0));
        }

        private void AddControl(ZomBDesignableControlInfo info, Point point)
        {
            Control fe = Reflector.Inflate(info.Type) as Control;
            var sc = new SurfaceControl();
            sc.Control = fe;
            sc.Width = fe.Width;
            sc.Height = fe.Height;
            ZDash.Children.Add(sc);
            //hider.Children.Add(fe);
            Canvas.SetTop(sc, point.Y);
            Canvas.SetLeft(sc, point.X);
        }

        private void listBox1_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            lbdragging = false;
        }

        #endregion

        #region Designer Surface

        private void ScrollViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            cd = CurrentDrag.None;
        }

        private void NameBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (curObj != null)
            {
                ((ZomBGLControl)curObj.Control).ControlName = NameBox.Text;
            }
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
                    }
                    break;
                case CurrentDrag.Resize:
                    {
                        Vector mv = e.GetPosition(ZDash) - dndopoint;
                        var sc = (origSrc as SurfaceControl);
                        sc.Width = Math.Min(Math.Max(0, opoint.X + mv.X), ZDash.Width - Canvas.GetLeft((UIElement)origSrc));
                        sc.Height = Math.Min(Math.Max(0, opoint.Y + mv.Y), ZDash.Height - Canvas.GetTop((UIElement)origSrc)); ;
                    }
                    break;
                case CurrentDrag.None:
                default:
                    break;
            }
        }

        private void ZDash_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Deselect();
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (e.OriginalSource is Button || VisualTreeHelper.GetParent((DependencyObject)e.OriginalSource) is Button || VisualTreeHelper.GetParent(VisualTreeHelper.GetParent((DependencyObject)e.OriginalSource)) is Button) //resize, TODO: add PART_xxx detection
                {
                    cd = CurrentDrag.Resize;
                    dndopoint = e.GetPosition(ZDash);
                    origSrc = FindAnchestor<SurfaceControl>((DependencyObject)e.OriginalSource);
                    opoint = new Point((origSrc as FrameworkElement).Width, (origSrc as FrameworkElement).Height);
                    Select(origSrc as SurfaceControl);
                }
                else if (e.OriginalSource is SurfaceControl || VisualTreeHelper.GetParent((DependencyObject)e.OriginalSource) is SurfaceControl) //resize, TODO: add PART_xxx detection
                {
                    System.Diagnostics.Debug.Print("st");
                    cd = CurrentDrag.Move;
                    dndopoint = e.GetPosition(ZDash);
                    origSrc = FindAnchestor<SurfaceControl>((DependencyObject)e.OriginalSource);
                    opoint = new Point(Canvas.GetLeft((UIElement)origSrc), Canvas.GetTop((UIElement)origSrc));
                    Select(origSrc as SurfaceControl);
                }
            }
        }

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
                NameBox.Text = ((IZomBControl)curObj.Control).ControlName;
                NameBox.Focusable = true;

            }
            else
            {
                NameBox.Text = "";
                NameBox.Focusable = false;
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

        #region Save and Run

        private void CommandBinding_Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RunApp();
        }

        private void CommandBinding_Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveApp();
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

        private string Export()
        {
            StringBuilder sb = new StringBuilder("<Canvas xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:ZomB=\"clr-namespace:System451.Communication.Dashboard.WPF.Controls;assembly=ZomB\" Height=\"400\" Width=\"1024\">");

            foreach (var item in LogicalTreeHelper.GetChildren(ZDash))
            {
                sb.Append("<ZomB:");
                sb.Append(((SurfaceControl)item).Control.GetType().Name);
                sb.Append(" Width=\"");
                sb.Append(((SurfaceControl)item).Width);
                sb.Append("\" Height=\"");
                sb.Append(((SurfaceControl)item).Height);
                sb.Append("\" Canvas.Top=\"");
                sb.Append(Canvas.GetTop(item as UIElement));
                sb.Append("\" Canvas.Left=\"");
                sb.Append(Canvas.GetLeft(item as UIElement));
                if ((((SurfaceControl)item).Control as ZomBGLControl).ControlName.Length > 0)
                {
                    sb.Append("\" Name=\"");
                    sb.Append((((SurfaceControl)item).Control as ZomBGLControl).ControlName);
                }
                sb.Append("\" />");
            }

            sb.Append("</Canvas>");
            return sb.ToString();
        }

        #endregion
    }
}
