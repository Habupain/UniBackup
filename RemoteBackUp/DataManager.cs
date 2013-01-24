using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;

namespace RemoteBackUp
{
    class DataManager
    {
        private static List<Set>[] Sets;

        public static void InitSets()
        {
            Sets = new List<Set>[BackUpManager.BackupList.Count];
            
        }

        public static int GetSetsCount(int backupId)
        {
            if (Sets.Length > 0)
            {
                if (Sets[backupId - 1] != null)
                {
                    if (Sets[backupId - 1].Count > 0)
                        return Sets[backupId - 1].Count();
                    else
                        return 0;
                }
                else
                    return 0;
            }
            else
                return 0;
        }

        

        public static bool IsSpaceAvailable(int percentSpace,String driveLetter)
        {
            DriveInfo c = new DriveInfo(driveLetter);
            long cAvailableSpace = c.AvailableFreeSpace;
            
            long cFullSpace = c.TotalSize;
         
            if ((int)(((float)cAvailableSpace /(float) cFullSpace) * 100) < percentSpace)
                return false;
            else
                return true;
            
        }

        public static bool DeleteLastSet(int BackupId)
        {
            
            Set set = Sets[BackupId-1][0];
            String SetFile = string.Format(
                "{0}\\{1}-{2}-{3}-Backup-{4}-{5}.{6}",
                set.setPath,set.dateYear,
                set.dateMonth,set.dateDay,
                set.backupType, BackupId, set.backupLevel
                );
            
            if (File.Exists(SetFile))
            {
                try
                {
                    File.Delete(SetFile);
                }
                catch (Exception e)
                {

                }
                Sets[BackupId-1].RemoveAt(0);
                return true;
            }
            else
                return false;
        }

        public static void PopulateSets()
        {
            InitSets();
            foreach (Backup bck in BackUpManager.BackupList)
            {
                int count = 0;
                if (bck.status)
                {
                    
                    DirectoryInfo dir = new DirectoryInfo(bck.dest);
                    try
                    {
                        FileInfo[] files = dir.GetFiles(string.Format("*-{0}.*", bck.iD));
                        if (files.Length > 0)
                        {
                            List<Set> values = new List<Set>();
                            foreach (FileInfo file in files)
                            {
                                string[] data = file.ToString().Split('-');

                                Set set = new Set();
                                set.backupSetID = bck.iD;
                                set.dateYear = data[0];
                                set.dateMonth = data[1];
                                set.dateDay = data[2];
                                set.backupType = ConfigTextManager.GetBackupType(data[4]);
                                data = data[5].Split('.');

                                set.backupLevel = ConfigTextManager.GetBackupLevel(data[1]);
                                set.setPath = bck.dest;
                                set.setRootPath = bck.dest.Substring(0, 3);
                                values.Add(set);

                            }
                            Sets[bck.iD - 1] = values;
                        }
                    }
                    catch (Exception e) { }

                }
            }
                            
        }

        private class Set
        {
            public int backupSetID;
            public String dateYear;
            public String dateMonth;
            public String dateDay;
            public BackupType backupType;
            public BackupLevel backupLevel;
            public String setPath;
            public String setRootPath;
        }

    }
   
}
