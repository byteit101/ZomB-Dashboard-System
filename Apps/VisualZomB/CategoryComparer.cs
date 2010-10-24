using System;
using System.Collections.Generic;
using System.Text;

namespace System451.Communication.Dashboard.ViZ
{
    public class CategoryComparer : IComparer<string>
    {
        public CategoryComparer()
        {

        }

        #region IComparer<string> Members

        public int Compare(string x, string y)
        {
            if (x == y)
                return 0;
            if (x == "ZomB" || y == "Misc")
                return -1;
            if (y == "ZomB" || x == "Misc")
                return 1;
            return string.Compare(x, y);
        }

        #endregion
    }
}
