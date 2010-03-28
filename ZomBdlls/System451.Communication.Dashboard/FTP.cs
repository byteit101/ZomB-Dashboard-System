/*
 * Copyright (c) 2009-2010, Patrick Plenefisch and FIRST Robotics Team 451 "The Cat Attack"
 * 
 * Permission to use, copy, modify, and distribute this software, its source, and its documentation
 * for any purpose, without fee, and without a written agreement is hereby granted, 
 * provided this paragraph and the following paragraph appear in all copies, and all
 * software that uses this code is released under this license. All projects that use
 * this code MUST release their source without fee.
 * 
 * THIS SOFTWARE IS PROVIDED "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
 * AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
 * Patrick Plenefisch OR FIRST Robotics Team 451 "The Cat Attack" BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
//using AviFile;
using System.Threading;
/*
namespace System451.Communication.Dashboard
{
    class JpegFTPMove
    {
        string ftpServerIP = "10.4.51.2";
        string ftpUserID = "anonymous";
        string ftpPassword = "anonymous";
        public JpegFTPMove()
        {

        }

        /// <summary>
        /// Dowload all the files in pathoncRIO to dirtosaveto and optionaly deletes them
        /// TEST THIS CLASS!!! LOOK IN CODE
        /// </summary>
        /// <param name="dirtosaveto">the directory to save to  (no / at the end)</param>
        /// <param name="pathoncRIO">the folder on crio (myfold/otherfold)</param>
        /// <param name="delete">delete the files after download?</param>
        public void DoDowload(string dirtosaveto, string pathoncRIO, bool delete)
        {
            //TODO: TEST: the file name may not iclude the dir
            string[] files = GetFileList(pathoncRIO);
            foreach (string file in files)
                Download(dirtosaveto, file);
            if (delete)
                foreach (string file in files)
                    DeleteFTP(file);

        }

        private void Download(string localfilePath, string ftpfileName)
        {
            FtpWebRequest reqFTP;
            try
            {
                //filePath = <<The full path where the file is to be created.>>, 
                //fileName = <<Name of the file to be created(Need not be the name of the file on FTP server).>>
                FileStream outputStream = new FileStream(localfilePath + "\\" + ftpfileName, FileMode.Create);

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + ftpfileName));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];

                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }

                ftpStream.Close();
                outputStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string[] GetFileList(string pathoncRIO)
        {
            string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + pathoncRIO + "/"));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                //MessageBox.Show(reader.ReadToEnd());
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                reader.Close();
                response.Close();

                //MessageBox.Show(response.StatusDescription);
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                downloadFiles = null;
                return downloadFiles;
            }
        }

        private string DeleteFTP(string fileName)
        {
            try
            {
                string uri = "ftp://" + ftpServerIP + "/" + fileName;
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + fileName));

                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;

                string result = String.Empty;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                long size = response.ContentLength;
                Stream datastream = response.GetResponseStream();
                StreamReader sr = new StreamReader(datastream);
                result = sr.ReadToEnd();
                sr.Close();
                datastream.Close();
                response.Close();
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "FTP 2.0 Delete");
                return "EXVEPTION";
            }
        }

    }
    
    class JpegToAvi
    {
        private Bitmap _image;

        public Bitmap NextFrame
        {
            get
            {
                return _image;
            }

            set
            {
                    _image = value;
            }

        }
        //string folder;
        public JpegToAvi()
        {
        }
        /// <summary>
        /// Create an Avi at the specified framerate in the file
        /// Run this async, and put new images in the NextFrame property at real speed, and 
        /// avi will record it at the correct framerate
        /// </summary>
        /// <param name="framerate">the framerate</param>
        /// <param name="file">the path to the avi</param>
        public void CreateAvi(int framerate, string file)
        {
            //load the first image
            Bitmap bitmap = NextFrame;
            //create a new AVI file
            AviManager aviManager = new AviManager(file, false);
            //add a new video stream and one frame to the new file
            VideoStream aviStream = aviManager.AddVideoStream(true, framerate, bitmap);

            bitmap = NextFrame;
            while (bitmap != null)
            {

                aviStream.AddFrame(bitmap);
                ///bitmap.Dispose(); TODO: is this really nessacary?
                Thread.Sleep((1 / framerate)*1000);
                bitmap = NextFrame;
            }


            aviManager.Close();

        }
        //private string[] GetFileList()
        //{
        //    int loop = 0;
        //    try
        //    {
        //        while (true)
        //        {
        //            pos++;
        //            files = GetFiles();
        //            exploreControl1.txtFileNames.Text += files[0] + "\r\n";
        //            loop++;
        //        }
        //    }
        //    catch
        //    {

        //    }

        //}
        //private string[] GetFiles()
        //{
        //    return (Directory.GetFiles(lastdir, ("*RIO" + (pos).ToString() + ".*")));
        //}
    }
    
}
*/