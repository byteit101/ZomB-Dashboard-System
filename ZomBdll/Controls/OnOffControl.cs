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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System451.Communication.Dashboard.Controls
{
    [ToolboxBitmap(typeof(icofinds), "System451.Communication.Dashboard.TBB.OnOff.png")]
    [Designer(typeof(Design.OnOffControlDesigner))]
    public partial class OnOffControl : ZomBControl
    {
        bool speedval = false;

        public OnOffControl()
        {
            InitializeComponent();

            ControlName = "digital1";
        }
        [DefaultValue("0"), Category("ZomB"), Description("The Value of the bool")]
        public bool Value
        {
            get
            {
                return speedval;
            }
            set
            {
                speedval = value;
                this.Invalidate();
            }
        }

        public override void UpdateControl(string value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Utils.StringFunction(UpdateControl), value);
            }
            else
            {
                if (value == "0" || value == "1")
                    Value = int.Parse(value) == 0 ? false : true;
                else
                    Value = bool.Parse(value);
            }
        }

        private void OnOffControl_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(this.BackColor);
            e.Graphics.ScaleTransform((float)this.Width / 25f, (float)this.Height / 25f);
            if (Value)
            {
                e.Graphics.FillEllipse(Brushes.DarkGreen, 1, 1, 23, 23);
                e.Graphics.FillEllipse(Brushes.ForestGreen, 3, 3, 19, 19);
                e.Graphics.FillEllipse(Brushes.LawnGreen, 6, 6, 13, 13);
            }
            else
            {
                e.Graphics.FillEllipse(Brushes.DarkRed, 1, 1, 23, 23);
                e.Graphics.FillEllipse(Brushes.Crimson, 3, 3, 19, 19);
                e.Graphics.FillEllipse(Brushes.Red, 6, 6, 13, 13);
            }
        }
    }
    [ToolboxBitmap(typeof(icofinds), "System451.Communication.Dashboard.TBB.OnOff.png")]
    [Designer(typeof(Design.AlertControlDesigner))]
    public partial class AlertControl : ZomBControl
    {
        bool speedval = false;

        public AlertControl()
        {
            InitializeComponent();
            ControlName = "alert1";
        }
        [DefaultValue("0"), Category("ZomB"), Description("The Value of the bool")]
        public bool Value
        {
            get
            {
                return speedval;
            }
            set
            {
                speedval = value;
                this.Invalidate();
            }
        }

        public override void UpdateControl(string value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Utils.StringFunction(UpdateControl), value);
            }
            else
            {
                if (value == "0" || value == "1")
                    Value = int.Parse(value) == 0 ? false : true;
                else
                    Value = bool.Parse(value);
            }
        }

        private void OnOffControl_Paint(object sender, PaintEventArgs e)
        {
            if (Value)
            {
                e.Graphics.Clear(this.ForeColor);
            }
            else
            {
                e.Graphics.Clear(this.BackColor);
            }
        }
    }

    [ToolboxBitmap(typeof(icofinds), "System451.Communication.Dashboard.TBB.Spike.png")]
    [Designer(typeof(Design.SpikeControlDesigner))]
    public partial class SpikeControl : ZomBControl
    {
        SpikePositions speedval = SpikePositions.Off;

        public SpikeControl()
        {
            InitializeComponent();
            ControlName = "spike1";
        }
        [DefaultValue("Off"), Category("ZomB"), Description("The Value of the Spike")]
        public SpikePositions Value
        {
            get
            {
                return speedval;
            }
            set
            {
                speedval = value;
                this.Invalidate();
            }
        }

        public override void UpdateControl(string value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Utils.StringFunction(UpdateControl), value);
            }
            else
            {
                Value = (SpikePositions)int.Parse(value);
            }
        }

        private void SpikeControl_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(this.BackColor);
            e.Graphics.ScaleTransform((float)this.Width / 25f, (float)this.Height / 25f);
            if (Value == SpikePositions.Forward)
            {
                e.Graphics.FillRectangle(Brushes.DarkGreen, 1 - .5f, 1 - .5f, 23, 23);
                e.Graphics.FillRectangle(Brushes.ForestGreen, 3 - .5f, 3 - .5f, 19, 19);
                e.Graphics.FillRectangle(Brushes.LawnGreen, 6 - .5f, 6 - .5f, 13, 13);
            }
            else if (Value == SpikePositions.Reverse)
            {
                e.Graphics.FillRectangle(Brushes.DarkRed, 1 - .5f, 1 - .5f, 23, 23);
                e.Graphics.FillRectangle(Brushes.Crimson, 3 - .5f, 3 - .5f, 19, 19);
                e.Graphics.FillRectangle(Brushes.Red, 6 - .5f, 6 - .5f, 13, 13);
            }
            else
            {
                e.Graphics.FillRectangle(Brushes.Black, 1 - .5f, 1 - .5f, 23, 23);
            }
        }
    }

    namespace Design
    {
        internal class OnOffControlDesigner : ControlDesigner
        {
            OnOffControl vm;
            bool dragin = false;
            bool inadorn = false;
            int point = 0;

            private const int WM_MouseMove = 0x0200;
            private const int WM_LButtonDown = 0x0201;
            private const int WM_LButtonUp = 0x0202;
            private const int WM_LButtonDblClick = 0x0203;
            private const int WM_RButtonDown = 0x0204;
            private const int WM_RButtonUp = 0x0205;
            private const int WM_RButtonDblClick = 0x0206;

            public OnOffControlDesigner()
            {

            }
            public override void Initialize(IComponent component)
            {
                base.Initialize(component);
                vm = (OnOffControl)component;
            }
            protected override void OnSetCursor()
            {
                if (!inadorn && !dragin)
                    base.OnSetCursor();
            }

            public GraphicsPath GetValueRec()
            {
                GraphicsPath gp = new GraphicsPath();
                gp.AddEllipse((float)((.04) * vm.Width), (float)((.04) * vm.Height), (float)(.92) * vm.Width, (float)(.92) * vm.Height);
                return gp;
            }
            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case WM_LButtonDown:
                        if (GetValueRec().IsVisible(new Point(m.LParam.ToInt32())))
                        {
                            dragin = true;
                            Cursor.Current = Cursors.Hand;
                            point = m.LParam.ToInt32();
                        }
                        break;
                    case WM_MouseMove:
                        if (dragin)
                        {
                            Cursor.Current = Cursors.Hand;
                            if (point == m.LParam.ToInt32())
                                return;
                        }
                        else if (GetValueRec().IsVisible(new Point(m.LParam.ToInt32())))
                        {
                            inadorn = true;
                            Cursor.Current = Cursors.Hand;
                        }
                        else
                            inadorn = false;

                        break;
                    case WM_LButtonUp:
                        if (dragin)
                        {
                            dragin = false;
                            if (point == m.LParam.ToInt32())
                                vm.Value = !vm.Value;
                        }
                        break;
                }
                base.WndProc(ref m);
            }
        }
        internal class AlertControlDesigner : ControlDesigner
        {
            AlertControl vm;
            bool dragin = false;
            bool inadorn = false;
            int point = 0;

            private const int WM_MouseMove = 0x0200;
            private const int WM_LButtonDown = 0x0201;
            private const int WM_LButtonUp = 0x0202;
            private const int WM_LButtonDblClick = 0x0203;
            private const int WM_RButtonDown = 0x0204;
            private const int WM_RButtonUp = 0x0205;
            private const int WM_RButtonDblClick = 0x0206;

            public AlertControlDesigner()
            {

            }
            public override void Initialize(IComponent component)
            {
                base.Initialize(component);
                vm = (AlertControl)component;
            }
            protected override void OnSetCursor()
            {
                if (!inadorn && !dragin)
                    base.OnSetCursor();
            }
            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case WM_LButtonDown:
                        dragin = true;
                        Cursor.Current = Cursors.Hand;
                        point = m.LParam.ToInt32();
                        break;
                    case WM_MouseMove:
                        if (dragin)
                        {
                            Cursor.Current = Cursors.Hand;
                            if (point == m.LParam.ToInt32())
                                return;
                        }
                        else
                        {
                            inadorn = true;
                            Cursor.Current = Cursors.Hand;
                        }
                        break;
                    case WM_LButtonUp:
                        if (dragin)
                        {
                            dragin = false;
                            if (point == m.LParam.ToInt32())
                                vm.Value = !vm.Value;
                        }
                        break;
                }
                base.WndProc(ref m);
            }
        }
        internal class SpikeControlDesigner : ControlDesigner
        {
            SpikeControl vm;
            bool dragin = false;
            bool inadorn = false;
            int point = 0;

            private const int WM_MouseMove = 0x0200;
            private const int WM_LButtonDown = 0x0201;
            private const int WM_LButtonUp = 0x0202;
            private const int WM_LButtonDblClick = 0x0203;
            private const int WM_RButtonDown = 0x0204;
            private const int WM_RButtonUp = 0x0205;
            private const int WM_RButtonDblClick = 0x0206;

            public SpikeControlDesigner()
            {

            }
            public override void Initialize(IComponent component)
            {
                base.Initialize(component);
                vm = (SpikeControl)component;
            }
            protected override void OnSetCursor()
            {
                if (!inadorn && !dragin)
                    base.OnSetCursor();
            }
            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case WM_LButtonDown:
                        dragin = true;
                        point = m.LParam.ToInt32();
                        Cursor.Current = Cursors.Hand;
                        break;
                    case WM_MouseMove:
                        if (dragin)
                        {
                            Cursor.Current = Cursors.Hand;
                            if (point == m.LParam.ToInt32())
                                return;
                        }
                        else
                        {
                            inadorn = true;
                            Cursor.Current = Cursors.Hand;
                        }
                        break;
                    case WM_LButtonUp:
                        if (dragin)
                        {
                            dragin = false;
                            if (point == m.LParam.ToInt32())
                                switch (vm.Value)
                                {
                                    case SpikePositions.Forward:
                                        vm.Value = SpikePositions.Reverse;
                                        break;
                                    case SpikePositions.Off:
                                        vm.Value = SpikePositions.Forward;
                                        break;
                                    case SpikePositions.Reverse:
                                        vm.Value = SpikePositions.Off;
                                        break;
                                    default:
                                        vm.Value = SpikePositions.Forward;
                                        break;
                                }
                        }
                        break;
                }
                base.WndProc(ref m);
            }
        }
    }
}
namespace System451.Communication.Dashboard
{
    public enum SpikePositions
    {
        Forward = 1,
        Off = 0,
        Reverse = -1
    };
}