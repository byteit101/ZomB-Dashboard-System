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
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace System451.Communication.Dashboard.Utils
{
    public static class Updater
    {
        const string beginSection = "----BEGIN ZomB VERSION INFO:";
        const string endSection = ":END ZomB VERSION INFO----";
        public const string defaultUpdateUrl = "http://firstforge.wpi.edu/sf/wiki/do/viewPage/projects.zombdashboard/wiki/VersionNumbers";

        /// <summary>
        /// Checks for updates at default url. Returns download url or null if up to date
        /// </summary>
        /// <returns>Download url or null if up to date.</returns>
        public static string Check()
        {
            return Check(defaultUpdateUrl);
        }

        /// <summary>
        /// Checks for updates at given url. Returns download url or null if up to date
        /// </summary>
        /// <param name="updateUrl">Url to check at</param>
        /// <returns>Download url or null if up to date.</returns>
        public static string Check(string updateUrl)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(updateUrl);
            request.UserAgent = "ZomB-Updater/" + ZVersionMgr.FullNumber;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new InvalidDataException("Site did not return HTTP OK: " + response.StatusCode.ToString());
            }
            var resp = response.GetResponseStream();
            using (StreamReader sr = new StreamReader(resp))
            {
                var entirety = sr.ReadToEnd();
                if (!entirety.Contains(beginSection) || !entirety.Contains(endSection))
                {
                    throw new InvalidDataException("Site did not contain version info");
                }
                entirety = entirety.Substring(entirety.IndexOf(beginSection) + beginSection.Length);
                entirety = entirety.Substring(0, entirety.IndexOf(endSection));
                //entirety == "[00]0.[00]0.[00]0.[svn|[000]0][b0|a0|g0]=urltodownload" (svn == .0)
                var versionside = entirety.Substring(0, entirety.IndexOf("="));
                var urlside = entirety.Substring(entirety.IndexOf("=")+1);
                var current = VersionNumber.FromString(versionside);
                var thisinstance = VersionNumber.FromString(ZVersionMgr.FullNumber);
                if (current > thisinstance)
                    return urlside;
                return null;
            }
        }

        public static string Download(string url)
        {
            WebClient wc = new WebClient();
            string tmp = Path.GetTempFileName();
            wc.Headers.Add("User-Agent: ZomB-Updater/" + ZVersionMgr.FullNumber);
            wc.DownloadFile(url, tmp);
            return tmp;
        }
    }

    public struct VersionNumber
    {
        public enum Type
        {
            None=3,
            Alpha=1, 
            Beta=2, 
            Release=3
        }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Bug { get; set; }
        public int Build { get; set; }
        public VersionNumber.Type BuildType { get; set; }
        public int BuildTypeNumber { get; set; }

        public static bool operator >(VersionNumber left, VersionNumber right)
        {
            if (left.Major == right.Major)
            {
                if (left.Minor == right.Minor)
                {
                    if (left.Bug == right.Bug)
                    {
                        if (left.Build == right.Build)
                        {
                            if (left.BuildType == right.BuildType)
                            {
                                if (left.BuildTypeNumber == right.BuildTypeNumber)
                                {
                                    return false;
                                }
                                else
                                    return left.BuildTypeNumber > right.BuildTypeNumber;
                            }
                            else
                                return (int)left.BuildType > (int)right.BuildType;
                        }
                        else
                            return left.Build > right.Build;
                    }
                    else
                        return left.Bug > right.Bug;
                }
                else
                    return left.Minor > right.Minor;
            }
            else
                return left.Major > right.Major;
        }

        public static bool operator <(VersionNumber left, VersionNumber right)
        {
            return right > left;
        }

        public static bool operator >=(VersionNumber left, VersionNumber right)
        {
            return !(right > left);
        }

        public static bool operator <=(VersionNumber left, VersionNumber right)
        {
            return !(left > right);
        }

        public override string ToString()
        {
            return Major.ToString() + "." + Minor.ToString() + "." + Bug.ToString() + "." + Build.ToString() +
                (BuildType == Type.Release ? "g" : (BuildType == Type.Alpha ? "a" : "b")) + BuildTypeNumber.ToString();
        }

        public static VersionNumber FromString(string version)
        {
            string pattern = "([0-9]*)\\.([0-9]*)\\.([0-9]*)\\.([0-9svn]*)([a|g|b]?)([0-9]*)";
            Regex r = new Regex(pattern);
            Match m = r.Match(version);
            VersionNumber vn = new VersionNumber();
            vn.Major = int.Parse(m.Groups[1].Value);
            vn.Minor = int.Parse(m.Groups[2].Value);
            vn.Bug = int.Parse(m.Groups[3].Value);
            try
            {
                vn.Build = int.Parse(m.Groups[4].Value);
            }
            catch
            {
                if (m.Groups[4].Value == "svn")
                    vn.Build = int.MaxValue;
                else
                    vn.Build = 0;
            }

            switch (m.Groups[5].Value)
            {
                case "a":
                    vn.BuildType = Type.Alpha;
                    break;
                case "b":
                    vn.BuildType = Type.Beta;
                    break;
                case "g":
                    vn.BuildType = Type.Release;
                    break;
                default:
                    vn.BuildType = Type.None;
                    break;
            }
            try
            {
                vn.BuildTypeNumber = int.Parse(m.Groups[6].Value);
            }
            catch { vn.BuildTypeNumber = 0; }

            return vn;
        }
    }
}
