using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Net.Mime;
using System.Collections;
using VSS;
using EnumerateFile;

namespace RemoteBackUp
{
    public enum BackupType { 
        Full =0,
        Incremental=1,
        Differential = 2,
        None = 3
        
    }


    public enum BackupLevel { 
        Zip = 0,
        NtBackup = 1,
        Reflect = 2,
        DriverXml =3,
        Wbadmin = 4,
        None = 5
    }

    public enum Compression { 
        Fast =0,
        Normal = 1,
        High = 2,
        None = 3
    }

    

    static class BackUpManager
    {
        public static List<Backup> BackupList;
        
        public static void DoBackUp()
        {
            ConfigTextManager.GetParams();
            BackupList = ConfigTextManager.GetBackUpList();
            
            DataManager.PopulateSets();
            foreach (Backup bck in BackupList)
                if (bck.status)
                {
                    bck.backupTime = DateTime.Now.ToShortTimeString();
                Again:
                    if (DataManager.IsSpaceAvailable(bck.spaceFree, GetDriveLetter(bck.dest)))
                    {
                        if (DataManager.GetSetsCount(bck.iD) <= 0)
                            bck.backupType = BackupType.Full;

                            bck.backupStartTime = DateTime.Now.ToShortTimeString();
                            CreateBackUpFile(bck);
                            bck.backupEndTime = DateTime.Now.ToShortTimeString();
                            bck.isSuccess = true;
                            DataManager.PopulateSets();
                            if (bck.setsToKeep < DataManager.GetSetsCount(bck.iD))
                                DataManager.DeleteLastSet(bck.iD);
                    }
                    else if (DataManager.GetSetsCount(bck.iD) > 0)
                    {

                        DataManager.DeleteLastSet(bck.iD);
                        goto Again;
                    }
                    else
                    {
                        bck.isSuccess = false;
                    }
                    
                    if (bck.isReport)
                    {

                        String report = Reportmanager.GenerateReport(new Report(bck));
                        Console.Write(report);
                    }


                }

           
            
                
        }

        public static String GetDriveLetter(String path)
        {
            return path.Substring(0, 1);
        }

        private static void CreateBackUpFile(Backup backup)
        {
            
         
            switch(backup.backupLevel)
            {
                case BackupLevel.Zip:
                         
                         ZipManager zM = new ZipManager();
                         
                         zM.ZipFiles(backup);
                         
                         break;
                case BackupLevel.DriverXml:break;
                case BackupLevel.Reflect:break;
                case BackupLevel.NtBackup: break;
                case BackupLevel.Wbadmin: break;
                default: break;
            }
        }
    }
}

