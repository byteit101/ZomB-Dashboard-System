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
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;

namespace System451.Communication.Dashboard.WPF.Controls
{
    /// <summary>
    /// Interaction logic for StatusTextBox.xaml
    /// </summary>
    [Design.ZomBControl("Text Box",
        Description = "This shows an editable raw value for a control",
        IconName = "TextBoxIcon",
        TypeHints = ZomBDataTypeHint.String | ZomBDataTypeHint.Number)]
    [Design.ZomBDesignableProperty("Foreground")]
    [Design.ZomBDesignableProperty("StringValue", DisplayName = "Text")]
    [Design.ZomBDesignableProperty("FontSize")]
    public class StatusTextBox : ZomBGLControl, IZomBDataControl
    {
        TextBox PART_txt;
        static StatusTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusTextBox),
            new FrameworkPropertyMetadata(typeof(StatusTextBox)));
            StringValueProperty.OverrideMetadata(typeof(StatusTextBox), new FrameworkPropertyMetadata(""));
        }

        public StatusTextBox()
        {
            this.Foreground = Brushes.Black;
            this.Width = 50;
            this.Height = 25;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_txt = base.GetTemplateChild("PART_txt") as TextBox;
            PART_txt.KeyUp += new System.Windows.Input.KeyEventHandler(PART_txt_KeyUp);
            PART_txt.IsReadOnly = DataControlEnabled;
        }

        public override void UpdateControl(ZomBDataObject value)
        {
            StringValue = value;
        }

        void PART_txt_KeyUp(object sender, KeyEventArgs e)
        {
            StringValue = PART_txt.Text;
            DataUpdated(this, new ZomBDataControlUpdatedEventArgs(ControlName, StringValue));
        }

        public event ZomBDataControlUpdatedEventHandler DataUpdated;
        bool dce = false;
        public bool DataControlEnabled
        {
            get
            {
                return dce;
            }
            set
            {
                dce = value;
                if (PART_txt != null)
                {
                    PART_txt.IsReadOnly = !dce;
                }
            }
        }
    }
}
