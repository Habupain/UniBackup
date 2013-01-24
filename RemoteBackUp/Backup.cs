using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteBackUp
{
     class Backup
    {
           public bool status;
           public bool isReport;
           public int iD;
           public int spaceFree;
           public BackupLevel backupLevel;
           public String Name;
           public String password; 
           public bool isVss;
           public int setsToKeep;
           public BackupType backupType;
           public String[] Sources;
           public bool isSubDir;
           public Compression compressLevel;
           public String dest;
           public String backupTime;
           public String backupStartTime;
           public String backupEndTime;
           public long numOfFileBackup;
           public long numOfFileAlready;
           public long numOfFileBackedup;
           public String backupDrive;
           public String backupDriveSpaceUsed;
           public String backupDriveSpaceFree;
           public String backupDriveTotalSpace;
           public bool isSuccess;
    }
}
