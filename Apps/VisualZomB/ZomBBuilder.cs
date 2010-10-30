using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using System.IO;
using System451.Communication.Dashboard.WPF;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

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
    class ZApp : System451.Communication.Dashboard.WPF.DashboardDataHubWindow
    {
        public ZApp()
        {
            this.Content = System.Windows.Markup.XamlReader.Parse(""" + zaml.Replace("\"", "\\\"") + @""");
        }
        [System.STAThread]
        static void Main()
        {
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
