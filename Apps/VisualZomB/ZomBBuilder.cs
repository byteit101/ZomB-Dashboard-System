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
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System;

namespace System451.Communication.Dashboard.ViZ
{
    public static class ZomBBuilder
    {
        public static bool BuildZomBString(string zaml, string path)
        {
            File.Copy(Assembly.Load("ZomB").Location, Path.GetDirectoryName(path) + "\\ZomB.dll", true);
            if (Directory.Exists("plugins"))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)+"\\plugins");
                foreach (var item in Directory.GetFiles("plugins"))
                {
                    File.Copy(item, Path.GetDirectoryName(path) + "\\plugins\\"+Path.GetFileName(item), true);
                }
            }
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            var cs = CodeDomProvider.CreateProvider("CSharp");
            CompilerParameters pam = new CompilerParameters();
            pam.ReferencedAssemblies.Add("System.dll");
            pam.ReferencedAssemblies.Add(Assembly.Load("PresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35").Location);
            pam.ReferencedAssemblies.Add(Assembly.Load("System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089").Location);
            pam.ReferencedAssemblies.Add(Assembly.Load("PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35").Location);
            pam.ReferencedAssemblies.Add(Assembly.Load("WindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35").Location);
            pam.ReferencedAssemblies.Add(Path.GetDirectoryName(path) + "\\ZomB.dll");
            pam.OutputAssembly = path;
            pam.GenerateExecutable = true;
            pam.CompilerOptions = "/t:winexe \"/win32icon:"+
                Path.GetDirectoryName(Assembly.GetEntryAssembly().CodeBase).Replace("file:\\", "")+"\\Dashboardexe.ico\"";//hide console, set icon
            string src = @"using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System451.Communication.Dashboard.Utils;

namespace System451.g
{
    class ZApp : System451.Communication.Dashboard.WPF.Controls.DashboardDataHubWindow
    {
        public ZApp()
        {
            this.Content = System.Windows.Markup.XamlReader.Parse(""" + zaml.Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t") + @""");
        }
        [System.STAThread]
        static void Main()
        {
            System451.g.App app = new System451.g.App();
            app.Run(new ZApp());
        }
    }
    public class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AutoExtractor.AssemblyResolve);
            LoadPlugins();
            LoadAssembliesGeneric();
        }

        internal static void LoadPlugins()
        {
            if (Directory.Exists(""plugins""))
            {
                string[] plugins = Directory.GetFiles(""plugins"", ""*.dll"");
                foreach (string item in plugins)
                {
                    try
                    {
                        Assembly.LoadFrom(item);
                    }
                    catch { }
                }
            }
        }

        internal static void LoadAssembliesGeneric()
        {
            foreach (Assembly item in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    ResourceDictionary MyResourceDictionary = new ResourceDictionary();
                    MyResourceDictionary.Source = new Uri(""pack://application:,,,/"" + item.FullName + "";component/Themes/Generic.xaml"");
                    App.Current.Resources.MergedDictionaries.Add(MyResourceDictionary);
                }
                catch {}
            }
        }
    }
}";
            var res = cs.CompileAssemblyFromSource(pam, src);
            if (res.Errors.HasErrors)
                try
                {
                    var str = "ERROR BUILDING!";
                    foreach (CompilerError item in res.Errors)
                    {
                        str+=item.ToString();
                    }
                    System.Windows.Forms.MessageBox.Show("Generation failed: " + str);
                }
                catch
                {
                    //console?
                    Console.WriteLine("ERROR BUILDING!");
                    foreach (CompilerError item in res.Errors)
                    {
                        Console.WriteLine(item.ToString());
                    }
                }
            else
                return true;
            return false;
        }
    }
}
