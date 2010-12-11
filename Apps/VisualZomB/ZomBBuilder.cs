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

namespace System451.Communication.Dashboard.ViZ
{
    public static class ZomBBuilder
    {
        public static bool BuildZomBString(string zaml, string path)
        {
            File.Copy(Assembly.Load("ZomB").Location, Path.GetDirectoryName(path) + "\\ZomB.dll", true);
            if (!File.Exists(path))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            var cs = CodeDomProvider.CreateProvider("CSharp");
            CompilerParameters pam = new CompilerParameters();
            pam.ReferencedAssemblies.Add("System.dll");
            pam.ReferencedAssemblies.Add(Assembly.Load("PresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35").Location);
            pam.ReferencedAssemblies.Add(Assembly.Load("PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35").Location);
            pam.ReferencedAssemblies.Add(Assembly.Load("WindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35").Location);
            pam.ReferencedAssemblies.Add(Path.GetDirectoryName(path) + "\\ZomB.dll");
            pam.OutputAssembly = path;
            pam.GenerateExecutable = true;
            pam.CompilerOptions = "/t:winexe";//hide console
            string src = @"namespace System451.g
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
            System.AppDomain.CurrentDomain.AssemblyResolve += System451.Communication.Dashboard.Utils.AutoExtractor.AssemblyResolve;
            new System.Windows.Application().Run(new ZApp());
        }
    }
}";
            var res = cs.CompileAssemblyFromSource(pam, src);
            if (res.Errors.HasErrors)
                System.Windows.Forms.MessageBox.Show("Generation failed: " + res.Errors.ToString());
            else
                return true;
            return false;
        }
    }
}
