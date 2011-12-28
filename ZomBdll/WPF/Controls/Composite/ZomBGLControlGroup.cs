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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections;

namespace System451.Communication.Dashboard.WPF.Controls
{
    /// <summary>
    /// WPF Base ZomB Control
    /// </summary>
    [Design.ZomBDesignableProperty("Width", Dynamic = true, Category = "Layout")]
    [Design.ZomBDesignableProperty("Height", Dynamic = true, Category = "Layout")]
    [Design.ZomBDesignableProperty("RenderTransform", DisplayName = "Transform")]
    [Design.ZomBDesignableProperty("RenderTransformOrigin", DisplayName = "Transform Origin", Description = "The location the transform modifies about. In the range 0-1.")]
    public class ZomBGLControlGroup : ContentPresenter, IZomBControlGroup
    {
        DashboardDataHub localDDH;
        IZomBCompositeDescriptor descpt;
        string descptname = null;
        ZomBControlCollection remotes = new ZomBControlCollection();

        /// <summary>
        /// Creates the ZomBGLControl
        /// </summary>
        public ZomBGLControlGroup()
        {
            ControlAdded += new ControlAddedDelegate(ZomBGLControl_ControlAdded);
        }

        /// <summary>
        /// The control name
        /// </summary>
        [Browsable(false), Category("ZomB"), Description("What this control will prepend to each of its controls. Name.Data for SmartDashboard, empty for nothing")]
        [Design.ZomBDesignable(DisplayName = "GroupName", Index = 1)]
        public string ControlName
        {
            get
            {
                try
                {
                    return (string)GetValue(ControlNameProperty);
                }
                catch
                {/*wrong thread*/
                    return Dispatcher.Invoke(new Func<string>(() => ControlName)).ToString();
                }
            }
            set
            {
                try
                {
                    SetValue(ControlNameProperty, value);
                }
                catch
                {/*wrong thread*/
                    Dispatcher.Invoke(new Action<string>((s) => ControlName = value), value);
                }
            }
        }

        public static readonly DependencyProperty ControlNameProperty = DependencyProperty.Register("ControlName", typeof(string), typeof(ZomBGLControlGroup), new UIPropertyMetadata(""));

        /// <summary>
        /// The Descriptor name
        /// </summary>
        [Browsable(false), Category("ZomB"), Description("What this group will contain")]
        [Design.ZomBDesignable(DisplayName = "DescriptorName", Index = 1)]
        public string DescriptorName
        {
            get
            {
                try
                {
                    return (string)GetValue(DescriptorNameProperty);
                }
                catch
                {/*wrong thread*/
                    return Dispatcher.Invoke(new Func<string>(() => DescriptorName)).ToString();
                }
            }
            set
            {
                try
                {
                    SetValue(DescriptorNameProperty, value);
                }
                catch
                {/*wrong thread*/
                    Dispatcher.Invoke(new Action<string>((s) => DescriptorName = value), value);
                }
            }
        }

        public static readonly DependencyProperty DescriptorNameProperty = DependencyProperty.Register("DescriptorName", typeof(string), typeof(ZomBGLControlGroup), new UIPropertyMetadata(""));


        /// <summary>
        /// When this control is added to a DashboardDataHub
        /// </summary>
        public event ControlAddedDelegate ControlAdded;

        void IZomBControlGroup.ControlAdded(object sender, ZomBControlAddedEventArgs e)
        {
            if (ControlAdded != null)
                ControlAdded(sender, e);
        }

        void ZomBGLControl_ControlAdded(object sender, ZomBControlAddedEventArgs e)
        {
            localDDH = e.Controller.GetDashboardDataHub();
            if (this.Content == null)
            {
                this.Content = GroupDescriptor.InflateControls();
                Sizeify();
            }
            ResetControls();
        }

        private void ResetControls()
        {
            remotes.Clear();
            AddControls((DependencyObject)this.Content);
        }

        private void AddControls(DependencyObject controlCollection)
        {
            foreach (var item in LogicalTreeHelper.GetChildren(controlCollection))
            {
                if (item is IZomBControl)
                {
                    Add((IZomBControl)item);
                }

                if (item is IZTrigger)
                {
                    this.AddTriggerHandler((IZTrigger)item);
                }

                /*TODO:support nesting
                if (item is IZomBControlGroup)
                {
                    dashboardDataHub1.Add((IZomBControlGroup)item);
                    continue; //assume that the group manages all children
                }*/

                //If panel or has other controls, find those
                try
                {
                    foreach (var sub in LogicalTreeHelper.GetChildren((DependencyObject)item))
                    {
                        AddControls((DependencyObject)item);
                        break;
                    }
                }
                catch { /*No children or not a DepObj*/ }
            }
        }

        private void Add(IZomBControl item)
        {
            var rcl = new ZomBRemoteControl();
            var name = item.ControlName;
            if (this.ControlName != null && this.ControlName.Length > 0)
                name = this.ControlName + "." + name;
            if (item is IZomBDataControl)
            {
                rcl = new ZomBRemoteDataControl();
                (rcl as ZomBRemoteDataControl).DataControlEnabledChanged += new EventHandler(ZomBGLControlGroup_DataControlEnabledChanged);
                (item as IZomBDataControl).DataUpdated += delegate(object sender, ZomBDataControlUpdatedEventArgs e)
                {
                    (rcl as ZomBRemoteDataControl).SafeFireDataUpdated(name, e.Value);
                };
            }
            rcl.Tag = item;
            rcl.ControlName = name;
            rcl.ControlUpdated += new ControlUpdatedDelegate(rcl_ControlUpdated);
            remotes.Add(rcl);
            (rcl as IZomBControl).ControlAdded(this, new ZomBControlAddedEventArgs(LocalDashboardDataHub));
        }

        void ZomBGLControlGroup_DataControlEnabledChanged(object sender, EventArgs e)
        {
            ((sender as ZomBRemoteDataControl).Tag as IZomBDataControl).DataControlEnabled = (sender as ZomBRemoteDataControl).DataControlEnabled;
        }

        void rcl_ControlUpdated(object sender, ZomBControlUpdatedEventArgs e)
        {
            ((sender as ZomBRemoteControl).Tag as IZomBControl).UpdateControl(e.Value);
        }

        private void AddTriggerHandler(IZTrigger item)
        {
            item.Triggered += delegate
            {
                string[] trigs = item.TriggerListeners.Split();
                foreach (var trig in trigs)
                {
                    string[] nv = trig.Split(':');
                    if (nv.Length == 2)
                        CallTrigger(nv[0], nv[1]);
                }
            };
        }

        private void CallTrigger(string name, string methodname)
        {
            object unsuspectingVictim = this.FindName(name);
            if (unsuspectingVictim == null)
                return;
            var meth = unsuspectingVictim.GetType().GetMethod(methodname, new Type[] { });
            if (meth == null)
                return;
            meth.Invoke(unsuspectingVictim, null);
        }

        /// <summary>
        /// Gets the current DashboardDataHub
        /// </summary>
        [Browsable(false)]
        public DashboardDataHub LocalDashboardDataHub
        {
            get
            {
                return localDDH;
            }
        }

        /// <summary>
        /// Gets or sets the current Descriptor
        /// </summary>
        [Browsable(false)]
        public IZomBCompositeDescriptor GroupDescriptor
        {
            get
            {
                if (descpt == null && DescriptorName != null && DescriptorName != "")
                {
                    try
                    {
                        descpt = Activator.CreateInstance(GetDescriptorTypeByName(DescriptorName)) as IZomBCompositeDescriptor;
                    }
                    catch { }
                }
                return descpt;
            }
            set
            {
                descpt = value;
                this.Content = descpt.InflateControls();
                Sizeify();
            }
        }

        private void Sizeify()
        {
            if (double.IsNaN(this.Width))
                this.Width = (this.Content as FrameworkElement).Width;
            Binding bind = new Binding();
            bind.Mode = BindingMode.TwoWay;
            bind.Source = this;
            bind.Path = new PropertyPath(ZomBGLControlGroup.WidthProperty);
            (this.Content as FrameworkElement).SetBinding(FrameworkElement.WidthProperty, bind);

            if (double.IsNaN(this.Height))
                this.Height = (this.Content as FrameworkElement).Height;
            Binding bindh = new Binding();
            bindh.Mode = BindingMode.TwoWay;
            bindh.Source = this;
            bindh.Path = new PropertyPath(ZomBGLControlGroup.HeightProperty);
            (this.Content as FrameworkElement).SetBinding(FrameworkElement.HeightProperty, bindh);
        }

        public static Type GetDescriptorTypeByName(string name)
        {
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in asms)
            {
                var types = asm.GetTypes();
                foreach (var type in types)
                {
                    foreach (var atr in type.GetCustomAttributes(typeof(ZomBCompositeAttribute), false))
                    {
                        if ((atr as ZomBCompositeAttribute).CompositeName == name)
                            return type;
                    }
                }
            }
            throw new MissingMemberException("Type does not exist");
        }

        #region IZomBControlGroup Members

        public ZomBControlCollection GetControls()
        {
            return remotes;
        }

        #endregion
    }
}
