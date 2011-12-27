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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media;

namespace System451.Communication.Dashboard
{
    namespace WPF.Design
    {
        public static class Reflector
        {
            public static IEnumerable<Type> GetZomBDesignableClasses()
            {
                try
                {
                    var retTypes = new List<Type>();
                    var asms = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (var asm in asms)
                    {
                        var types = asm.GetTypes();
                        foreach (var type in types)
                        {
                            foreach (var atr in type.GetCustomAttributes(typeof(ZomBControlAttribute), false))
                            {
                                retTypes.Add(type);
                                break;
                            }
                        }
                    }
                    return retTypes;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error searching for Controls. This is most likely caused by a missing dependency. Is SlimDX installed?", ex);
                }
            }

            public static IEnumerable<ZomBControlAttribute> GetZomBDesignableInfos(IEnumerable<Type> types)
            {
                return (from t in types
                        let info = (t.GetCustomAttributes(typeof(ZomBControlAttribute), false)[0] as ZomBControlAttribute)
                        where (((info.Type = t) != null) &&
                        ((info.Icon = SafeFind(info.IconName ?? "DefaultControlImage")) != null))
                        orderby info.Name
                        select info);
            }

            public static ImageSource SafeFind(string iconname)
            {
                try
                {
                    return (ImageSource)System.Windows.Application.Current.FindResource(iconname);
                }
                catch
                {
                    try
                    {
                        return (ImageSource)System.Windows.Application.Current.FindResource("DefaultControlImage");
                    }
                    catch
                    {
                        return (DrawingImage)XamlReader.Load(new MemoryStream(UTF8Encoding.UTF8.GetBytes(@"<DrawingImage xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
        <DrawingImage.Drawing>
            <GeometryDrawing Geometry=""M 0 0 16 0 16 16 0 16 z"">
                <GeometryDrawing.Brush>
                    <VisualBrush Stretch=""Uniform"">
                        <VisualBrush.Visual>
                            <Label Background=""LightGray"" Foreground=""Red"">!?!?</Label>
                        </VisualBrush.Visual>
                    </VisualBrush>
                </GeometryDrawing.Brush>
            </GeometryDrawing>
        </DrawingImage.Drawing>
    </DrawingImage>")));
                    }
                }
            }

            public static object Inflate(Type t)
            {
                return t.GetConstructor(Type.EmptyTypes).Invoke(null);
            }

            public static T CreateInstanceOf<T>(this AppDomain appDomain) where T : class
            {
                return Activator.CreateInstance(appDomain, Assembly.GetAssembly(typeof(T)).FullName, typeof(T).FullName).Unwrap() as T;
            }

            public static T CreateInstanceOf<T>(this AppDomain appDomain, params object[] args) where T : class
            {
                return Activator.CreateInstance(appDomain, Assembly.GetAssembly(typeof(T)).FullName, typeof(T).FullName, false, BindingFlags.Default, null, args, null, null, new System.Security.Policy.Evidence()).Unwrap() as T;
            }
        }
    }
}
