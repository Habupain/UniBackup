﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Packaging;
using System.Net.Mime;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;

namespace RemoteBackUp
{
    class ZipManager
    {
        private vShadow oShadow;
        private WinApiFileReader wApi;
        private String sShadowPath;

        internal void ZipFiles(Backup bck)
        {
           
            string strzipdir = bck.dest;  // where the zip file is to be created
            string strfilename = "";     // the final filename we want to create

            strfilename = string.Format("{0}-{1}-{2}-Backup-{3}-{4}.zip", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day,bck.backupType, bck.iD);
            SearchOption srchOption = SearchOption.TopDirectoryOnly;
            
            if(bck.isSubDir)
                srchOption = SearchOption.AllDirectories;
           

            strfilename = strzipdir + "\\" + strfilename;
            List<FileInfo> filess = new List<FileInfo>();
            if(bck.Sources.Length>0)
            {
                foreach(String srce in bck.Sources)
                 {
                     if (srce != String.Empty)
                     {
                         DirectoryInfo di = new DirectoryInfo(srce);
                         filess.AddRange(di.GetFiles("*.*",srchOption));

                     }
                 }
            }
            bck.numOfFileBackup = filess.Count;
            bck.numOfFileAlready = 0;
            bck.numOfFileBackedup = 0;
            FileAttributes fileAttributes;

            // Open the zip file if it exists, else create a new one 
            Package zip = ZipPackage.Open(strfilename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            Package oldZip;
            DateTime date = DateTime.Today;
            date = date.AddDays(-1);
            String oldZipFile = strzipdir + "\\" + string.Format("{0}-{1}-{2}-Backup-{3}-{4}.zip", date.Year, date.Month, date.Day, BackupType.Differential, bck.iD);

            if (File.Exists(oldZipFile)&&bck.backupType==BackupType.Differential)
            {
                oldZip = ZipPackage.Open(oldZipFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                PackagePartCollection ppc = oldZip.GetParts();
                foreach (PackagePart pp in ppc)
                {
                    PackagePart zipP =  zip.CreatePart(pp.Uri,MediaTypeNames.Application.Zip);
                    Byte[] byt = ReadFully(pp.GetStream());
                    zipP.GetStream().Write(byt, 0, byt.Length);

                }
                oldZip.Close();
            }

            if (bck.isVss)
                InitVssShadow();

            //Add as many files as you like
            for (int ii = 0; ii < filess.Count; ii++)
            {
                


                if ((bck.backupType == BackupType.Incremental||bck.backupType==BackupType.Differential))
                {
                    // get the attibutes
                    fileAttributes = File.GetAttributes(filess[ii].FullName);

                    // check whether a file has archive attribute
                    bool isArchive = ((File.GetAttributes(filess[ii].FullName) &
                         FileAttributes.Archive) == FileAttributes.Archive);


                    bck.numOfFileAlready++;
                    // if the archive bit is set then clear it
                    if (isArchive)
                    {
                        // add to the archive file
                        if(AddToArchive(zip, filess[ii].FullName, bck))
                             bck.numOfFileBackedup++;
                       
                    }
                }
                else
                {
                    // add to the archive file
                    if(AddToArchive(zip, filess[ii].FullName, bck))
                        bck.numOfFileBackedup++;
                   
                }
                
                // clear the bit we archived it 
                try
                {
                    File.SetAttributes(filess[ii].FullName, FileAttributes.Normal);
                }
                catch (Exception e)
                {
                    MessageBox.Show("ERROR OCCURED");
                }
            }
            wApi.Close();
            oShadow.Dispose();
           zip.Close();
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private void InitVssShadow()
        {
            oShadow = new vShadow("c:\\");
            sShadowPath = oShadow.StartSnapShot();
            wApi = new WinApiFileReader();
            
        }
       
        private bool AddToArchive(Package zip, string fileToAdd,Backup bck)
        {
            // Replace spaces with an underscore (_) 
            string uriFileName = fileToAdd.Replace(" ", "_");

            // A Uri always starts with a forward slash "/" 
            string zipUri = string.Concat("/", Path.GetFileName(uriFileName));

            Uri partUri = new Uri(zipUri, UriKind.Relative);
            string contentType = MediaTypeNames.Application.Zip;


            if (!zip.PartExists(partUri))
            {
                zip.DeletePart(partUri);
            }
            if (!zip.PartExists(partUri))
            {
                PackagePart pkgPart = zip.CreatePart(partUri, contentType, ConvertCompIntoCompressOption(bck.compressLevel));

                // Read all of the bytes from the file to add to the zip file 
                Byte[] bites = null;

                if (bck.isVss)
                {
                    MessageBox.Show(sShadowPath + fileToAdd);
                    //bites = wApi.GetFileData(sShadowPath + Path.GetFileName(fileToAdd));
                }
                else
                {
                    try
                    {
                        bites = File.ReadAllBytes(fileToAdd);
                    }
                    catch (Exception e)
                    {
                        bites = null;
                    }
                }


                //Compress and write the bytes to the zip file 
                
                if (bites != null)
                {
                   
                    pkgPart.GetStream().Write(bites, 0, bites.Length);
                    return true;
                }
                else
                    return false;

            }
            else
                return false;
        }


        public CompressionOption ConvertCompIntoCompressOption(Compression comp)
        {
            CompressionOption compress = CompressionOption.Normal;
            switch (comp)
            {
                case Compression.Normal: compress = CompressionOption.Normal; break;
                case Compression.High: compress = CompressionOption.Maximum; break;
                case Compression.Fast: compress = CompressionOption.Fast; break;
            }
            return compress;
        }

       
           
        
            
            
        
       
    }
}
