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
using System.Windows.Controls;

namespace System451.Communication.Dashboard.WPF.Controls
{
    [Design.ZomBControl("Image ", Description = "This will disply an image", IconName = "ImageIcon")]
    [Design.ZomBDesignableProperty("Source", Category = "Appearance")]
    [Design.ZomBDesignableProperty("Width", Dynamic = true, Category = "Layout")]
    [Design.ZomBDesignableProperty("Height", Dynamic = true, Category = "Layout")]
    [Design.ZomBDesignableProperty("RenderTransform", DisplayName = "Transform")]
    [Design.ZomBDesignableProperty("RenderTransformOrigin", DisplayName = "Transform Origin", Description = "The location the transform modifies about. In the range 0-1.")]
    public class ZImage : Image
    {
        public ZImage()
        {
            this.Width = 100;
            this.Height = 100;
            this.Stretch = System.Windows.Media.Stretch.Fill;
        }
    }
}
