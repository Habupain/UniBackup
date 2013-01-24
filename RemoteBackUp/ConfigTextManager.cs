using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RemoteBackUp
{
    class ConfigTextManager
    {
        
        //Main Files Parameters..
        public static String[] strLines = File.ReadAllLines(@"backup.txt");//GetTextLines("http://www.hireageek.info/script/test/backup.txt");
        public static String Owner;
        public static String Company;
        public static bool Status;
        
        


        //Tag Corner : All the tags that are present in the Text are refered here.
        private static String ownerNameTag = "[OwnerName]";
        private static String companyTag = "[Company]";
        private static String backupTag = "[Backup]";
        private static String statusTag = "[Status]";
        private static String smtpTag = "[SMTP]";
        private static String usernameTag = "[Username]";
        private static String passwordTag = "[Password]";
        private static String emailTag = "[E-Mail]";
        private static String portTag = "[Port]";
        private static String reportTag = "[Report]";
        private static String backupTypeTag = "[Backup Type]";
        private static String compressionTag = "[Compression]";
        private static String subFolderTag = "[Sub Folder]";
        private static String setsToKeepTag = "[Sets to Keep]";
        private static String sourceTag = "[Source]";
        private static String destinationTag = "[Destination]";
        private static String fullTag = "[Full]";
        private static String incrementalTag = "[Incremental]";
        private static String differentialTag = "[Differential]";
        private static String backupPassTag = "[Use Password]";
        private static String useVssTag = "[Use VSS]";
        private static String freeSpaceTag = "[Free Space]";
        private static String backupNameTag = "[BackupName]";
        private static String backupStatusTag = "[BackupStatus]";
        private static String sourceEndTag = "[Source End]";
       
        //Gets Main Files Parameters..
         public static void GetParams()
        {
           
           Owner = ExtractTagData(ownerNameTag)[0];
           Company = ExtractTagData(companyTag)[0];
           Status = ExtractTagBool(statusTag)[0];
           
            
        }

         private static String[] GetTextLines(String webLink)
         {
             System.Net.WebClient wc = new System.Net.WebClient();
             byte[] raw = wc.DownloadData(webLink);
             
             string webData = System.Text.Encoding.UTF8.GetString(raw);
            String[] lines = webData.Split(new String[]{"\n"},StringSplitOptions.None);
            return lines;
         }
       
        //DebugBackUp Object...
        public static void DebugBackUp(Backup bck)
        {
                Console.WriteLine(" ");
                Console.WriteLine("Backup ID :" + bck.iD);
                Console.WriteLine("Backup Name :"+ bck.Name);
                Console.WriteLine("Backup Password :" + bck.password);
                Console.WriteLine("Backup Status :"+ bck.status);
                Console.WriteLine("Backup Vss :"+bck.isVss);
                Console.WriteLine("Backup Sub Folder :"+bck.isSubDir);
                Console.WriteLine("Backup Report :" +bck.isReport);
                Console.WriteLine("Backup Type :"+bck.backupLevel);
                Console.WriteLine("Sets to Keep :"+bck.setsToKeep);
                Console.WriteLine("Sources :");
                String[] values = bck.Sources;
                if (values != null)
                {
                    foreach (String val in values)
                    {
                        if(val!=String.Empty)
                            Console.WriteLine(val);
                    }
                }
                Console.WriteLine("Destination :" + bck.dest);
                Console.WriteLine("Compression Level :" + bck.compressLevel);
                Console.WriteLine("Monitor Free Space :"+bck.spaceFree);
                Console.WriteLine("Backup Type :" + bck.backupType);
               
        }

        //Extracts Data form  Given Tag...
        public static List<String> ExtractTagData(String Tag)
        {
            List<String> data = new List<string>();
            if (strLines.Length > 0)
            {
                foreach (String line in strLines)
                {
                  
                    if (line != String.Empty)
                    {
                        if (line.Contains(Tag))
                        {
                            int strlen = Tag.Length;
                           
                            data.Add(line.Substring(strlen, line.Length - strlen).Trim());
                        }
                    }
                }
                return data;
            }
            return data;
        }

        
        //Gets Number of Data on the Same Tags...
        public static int ExtractTagCount(String Tag)
        {
            int cnt = 0;
            if (strLines.Length > 0)
            {
                foreach (String line in strLines)
                {
                    if (line != String.Empty)
                    {
                        if (line.Contains(Tag))
                        {
                            cnt++;
                        }
                    }
                }
            }
            return cnt;
        }

        //Gets Number of Switches From the Same Tags..
        public static List<bool> ExtractTagBool(String Tag)
        {
            List<bool> data = new List<bool>();
            if (strLines.Length > 0)
            {
                foreach (String line in strLines)
                {
                    if (line != String.Empty)
                    {
                        if (line.Contains(Tag))
                        {
                            int strlen = Tag.Length;
                            String strdata = line.Substring(strlen, line.Length - strlen).Trim();
                            if (strdata == "True")
                                data.Add(true);
                            else
                                data.Add(false);
                        }
                    }
                }
                return data;
            }

            return data;
        
        }

        //Gets The Source Links On given BackUp Index
       public static String[] ExtractSources(int c)
       {
           int start = ExtractTagIndex(sourceTag)[c];
           int end = ExtractTagIndex(sourceEndTag)[c];
        
           if (end - start - 1 > 0)
           {
               String[] stringLines = new String[end - start - 1];
               int tt = 0;
               for (int x = start + 2; x <= end; x++)
               {

                   stringLines[tt++] = strLines[x].Trim().Replace("\\", "\\\\");
               }
               return stringLines;
           }
           else
           {
               return null;
           }
       }

        //Extracts Tag Index...
        public static List<int> ExtractTagIndex(String Tag)
        { 
            int x=-1;
            List<int> dataIndexes = new List<int>();

            if (strLines.Length > 0)
            {
                foreach (String line in strLines)
                {
                    if (line != String.Empty)
                    {
                        if (line.Contains(Tag))
                        {
                            dataIndexes.Add(x);
                        }
                    }
                    x++;
                }
                return dataIndexes;
            }
            return dataIndexes;
        }
        public static BackupType GetBackupType(string str)
        { 
            switch(str)
            {
                case "Full": return BackupType.Full;
                case "Differential": return BackupType.Differential;
                case "Incremental": return BackupType.Incremental;
                default: return BackupType.None;
            
            }
        
        }
        //Extracts BackupType From the Days Structure...
        public static BackupType GetBackupType(int index)
        {
           return SetupDays(index)[GetDayOfWeekIndex(DateTime.Today.DayOfWeek.ToString().Substring(0, 3))];
        }

        //Setups the BackupType according to Data from Text
        private static BackupType[] SetupDays(int index)
        {
            BackupType[] array = new BackupType[7];
            array = SetDayParams(array,BackupType.Full,index);
            array = SetDayParams(array,BackupType.Incremental,index);
            array = SetDayParams(array,BackupType.Differential,index);
            return array;

        }

        //Sets up the param For specific BackupType...
        private static BackupType[] SetDayParams(BackupType[] array,BackupType valu,int index)
        {

            String dat ;
            if(valu==BackupType.Full)
                dat = ExtractTagData(fullTag)[index];
            else if(valu==BackupType.Incremental)
                dat = ExtractTagData(incrementalTag)[index];
            else if(valu == BackupType.Differential)
                dat = ExtractTagData(differentialTag)[index];
            else
                dat=String.Empty;

            if(dat.Length>0)
            {
                if(dat.Length>3)
                {
                    int start = GetDayOfWeekIndex(dat.Substring(0,3));
                    int end = GetDayOfWeekIndex(dat.Substring(4,dat.Length-4));
                    for(int x=start;x<end;x++)
                        array[x]=valu;
                }
                else
                {
                    array[GetDayOfWeekIndex(dat)]= valu;
                }
            }
            return array;
        }

        //Gets the index number fromt he week..
        private static int GetDayOfWeekIndex(String dayOfWeek)
        {
            switch(dayOfWeek)
            {
                case "Sun": return 0;
                case "Mon" : return 1;
                case "Tue": return 2;
                case "Wed":return 3;
                case "Thu":return 4;
                case "Fri":return 5;
                case "Sat":return 6;
            }
            return 0;
        }

        //Gets THe BackUpList()
        public static List<Backup> GetBackUpList()
        { 
            List<Backup> backuplist = new List<Backup>();

            for (int x = 0; x < ExtractTagCount(backupTag); x++)
            {
                Backup backup = new Backup();
                backup.Name = ExtractTagData(backupNameTag)[x];
                backup.isVss = ExtractTagBool(useVssTag)[x];
                backup.status = ExtractTagBool(backupStatusTag)[x];
                backup.isReport = ExtractTagBool(reportTag)[x];
                backup.isSubDir = ExtractTagBool(subFolderTag)[x];
                try
                {
                    backup.iD = Convert.ToInt32(ExtractTagData(backupTag)[x]);
                }
                catch (Exception e)
                {
                    backup.iD = 0;
                }
                String backupLvl = ExtractTagData(backupTypeTag)[x];
                backup.backupLevel = GetBackupLevel(backupLvl);
                backup.setsToKeep = Convert.ToInt32(ExtractTagData(setsToKeepTag)[x]);
                backup.dest =  ExtractTagData(destinationTag)[x].Replace("\\","\\\\");
                String Comp = ExtractTagData(compressionTag)[x];
                switch (Comp)
                {
                    case "High": backup.compressLevel = Compression.High; break;
                    case "Fast": backup.compressLevel = Compression.Fast; break;
                    case "Normal": backup.compressLevel = Compression.Normal; break;
                    default: backup.compressLevel = Compression.Normal; break;
                }
                String space = ExtractTagData(freeSpaceTag)[x];
                backup.spaceFree = Convert.ToInt32(space.Substring(0,space.Length-1));
                backup.Sources = ExtractSources(x);
                backup.password = ExtractTagData(backupPassTag)[x];
                backup.backupType = GetBackupType(x);

                backuplist.Add(backup);
            }

                return backuplist;
        }
        public static BackupLevel GetBackupLevel(String str)
        {
            String backupLvl = str;
            switch (backupLvl)
            {
                case "Zip":
                case "zip":
                    return BackupLevel.Zip; 
                case "NTBackup": return BackupLevel.NtBackup;
                case "Reflect": return BackupLevel.Reflect;
                case "DriveImageXML": return BackupLevel.DriverXml;
                case "Wbadmin": return BackupLevel.Wbadmin;
                default: return BackupLevel.None;
            }
        }
     }
}
