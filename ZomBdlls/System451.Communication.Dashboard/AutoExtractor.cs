using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System451.Communication.Dashboard.Properties;

namespace System451.Communication.Dashboard
{
    public class AutoExtractor
    {
        public enum Files
        {
            All = 0xffff,
            InTheHandManaged = 0x1,
            InTheHandNative = 0x2
        }
        public static void Extract(Files files)
        {
            if ((files & Files.InTheHandManaged) == Files.InTheHandManaged)
            {
                if (!File.Exists("InTheHand.Net.Personal.dll"))
                    File.WriteAllBytes("InTheHand.Net.Personal.dll", Resources.InTheHand_Net_Personal);
            }
            if ((files & Files.InTheHandNative) == Files.InTheHandNative)
            {
                if (!File.Exists("32feetWidcomm.dll"))
                    File.WriteAllBytes("32feetWidcomm.dll", Resources._32feetWidcomm);
            }
        }
    }
}
