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
using SharpPG;

namespace System451.Communication.Dashboard.Utils
{
    public static class Updater
    {
        const string beginSection = "-----BEGIN PGP SIGNED MESSAGE-----";
        const string endSection = "\n-----END PGP SIGNATURE-----";
        public const string defaultUpdateUrl = "http://firstforge.wpi.edu/sf/wiki/do/viewPage/projects.zombdashboard/wiki/VersionNumbers";
        public const string ZomBDevelopersPubKey = @"-----BEGIN PGP PUBLIC KEY BLOCK-----
Version: GnuPG v2.0.17 (MingW32)

mQENBE5lSNQBCACbQaBQFg3Jz0CnZP4rXWe8nhzBdoX9LHsOq7qiRvULdA5hCfDG
zBDyA+yUBH7Ge28O85vPzoRlW81C2eReKQZWLZmUYeH1OiEnIjtX5LT+7o0Rh3nS
/yJ2Q9kt5oOz59LBY0VkAiyl0Qg2dwVhGPc2O5BtM4oWAzAmuFiVuu+UzuSbkKD2
20hj9dKDZGBUfL4O7e/h1riUipUgm+OcwLIczX6JKWRMFtKxL12QcEfJXlFWZzhM
H/T38TqUT1wH6/oLHVOgUyt+q+cegvr4UYilIKilTSFYKgIzwpozY5YJm0fS6HuL
vhTikq/jT95R/PR109AfiC49kOeuXTf6w3xHABEBAAG0OFpvbUIgRGFzaGJvYXJk
IFN5c3RlbSBEZXZlbG9wZXJzIDxab21CQHRoZWNhdGF0dGFjay5vcmc+iQE4BBMB
AgAiBQJOZUjUAhsPBgsJCAcDAgYVCAIJCgsEFgIDAQIeAQIXgAAKCRAy0xosbNSj
Pk2SB/9Em0oWsTuq3wj+teOlrMLfUEcAhdI7JKQaq1IKT3Z2VxknmLD1ovlooPoh
WOIshtdQXaMVbaBSrge93NABdGEjdhFoBDqvSPEPbWogI6rzmTviYHi3/8rcZqvO
zCGkOgb7AmhDZ6CkdI9JgrElPndXLZhLSY8Hd+KFRBYPWQhgBF7fu+R2rJeea6Cp
8qLMycqAo9P8yrY7l5WfG4BYR2rzU6eOrSHX+3ME0gWh13GB9gmx+6QYU56JGv2y
MgVWLui4a02qd/IcJ1bmR9n1eTNO2hGL6gB1ElHTY1Vn7h+xRjmGU2aBcDZxAbt1
XCq0oqhAxhMjOxg6vaKHL/8+PED/iQEcBBABAgAGBQJOZVO7AAoJENUyr8jo4/9/
RvEH/223aauHBfw3GUO7rlJVb2w6ckGaev99VJF2TxU+4kzGMG8e0jptAJ6ocyzq
pt9t9qtWrUimSU9QExFBy4ZFgFbylmx3oP/8j95t+JU43fuKc29D2LYKxRA7q5fB
vxVeVkQCZ/ee3HQ+4E7T1XcLSAcTt9c4DfCl+d9OMRelX5cqmP9VcAugahE8K2ET
7LfKUPiYvxrcl9FrqYc/VAOQSrqjY26v+tZtBNEhntylLuSHjDxtzwtb11CdvONe
XHvvaVSx2FEvgmt+qFobk2aCKBLrnwCmdZrGs4pKvficAaFGSSQ3yl3XENV2MMGl
PCEEo/ht1O0NhQPIbe2mudIz374=
=Bns1
-----END PGP PUBLIC KEY BLOCK-----
";

        /// <summary>
        /// Checks for updates at default url. Returns download url or null if up to date
        /// </summary>
        /// <returns>Download url or null if up to date.</returns>
        public static UpdateData Check()
        {
            return Check(defaultUpdateUrl);
        }

        /// <summary>
        /// Checks for updates at given url. Returns download url or null if up to date
        /// </summary>
        /// <param name="updateUrl">Url to check at</param>
        /// <returns>Download url or null if up to date.</returns>
        public static UpdateData Check(string updateUrl)
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
                var entiretyall = sr.ReadToEnd();
                string entirety = "";
                if (!entiretyall.Contains(beginSection) || !entiretyall.Contains(endSection))
                {
                    throw new InvalidDataException("Site did not contain version info");
                }
                entiretyall = entiretyall.Substring(entiretyall.IndexOf(beginSection));
                entirety = entiretyall.Substring(0, entiretyall.IndexOf(endSection) + endSection.Length) + "\n";
                entirety = entirety.Replace("\r\n", "\n");
                var serres = EncryptionHelper.Verify(EncryptionHelper.PubKey(ZomBDevelopersPubKey), entirety);
                if (!serres.IsValid)
                    return new UpdateData { UpdateAvailable = null };//TODO: look for other keys
                entirety = serres.SignedData.Replace("\r\n", "\n");
                entirety = entirety.Split('\n')[0];
                var sigdata = serres.SignedData.Replace("\r\n", "\n").Substring(entirety.Length + 12);//\nsignature=\n
                //entirety == "[00]0.[00]0.[00]0.[svn|[000]0][b0|a0|g0]=urltodownload" (svn == .0)
                var versionside = entirety.Substring(0, entirety.IndexOf("="));
                var urlside = entirety.Substring(entirety.IndexOf("=") + 1);
                var current = VersionNumber.FromString(versionside);
                var thisinstance = VersionNumber.FromString(ZVersionMgr.FullNumber);
                if (current > thisinstance)
                    return new UpdateData { UpdateAvailable = true, DownloadUrl = urlside, NewVersion = current, SigntureData = sigdata };
                return new UpdateData { UpdateAvailable = false };
            }
        }

        public static void Download(string url, string topath)
        {
            if (File.Exists(topath))
                File.Delete(topath);
            WebClient wc = new WebClient();
            wc.Headers.Add("User-Agent: ZomB-Updater/" + ZVersionMgr.FullNumber);
            wc.DownloadFile(url, topath);
        }

        public static string Download(string url)
        {
            string tmp = Path.GetTempFileName();
            Download(url, tmp);
            return tmp;
        }

        public static bool Download(UpdateData updateData, string saveas)
        {
            Download(updateData.DownloadUrl, saveas);
            File.WriteAllText(saveas + ".asc", updateData.SigntureData);
            bool returnv =  EncryptionHelper.VerifyFile(EncryptionHelper.PubKey(ZomBDevelopersPubKey), saveas + ".asc").IsValid;
            File.Delete(saveas + ".asc");
            return returnv;
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

    public struct UpdateData
    {
        public bool? UpdateAvailable { get; set; }
        public string DownloadUrl { get; set; }
        public string SigntureData { get; set; }
        public VersionNumber NewVersion { get; set; }
    }
}
