using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteBackUp
{
    class Report
    {
        public String subject;
        public String companyName;
        public String ownerName;
        public String backupUser;
        public String computerName;
        public String snapShotManager;
        public String[] backupSources;
        public String backupDestination;
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

        public Report(Backup bck)
        {
            ownerName =  ConfigTextManager.Owner;
            companyName = ConfigTextManager.Company;
            backupSources = bck.Sources;
            backupDestination = bck.dest;
            numOfFileAlready = bck.numOfFileAlready;
            numOfFileBackedup = bck.numOfFileBackedup;
            numOfFileBackup = bck.numOfFileBackup;
            backupTime = bck.backupTime;
            backupStartTime = bck.backupStartTime;
            backupEndTime = bck.backupEndTime;
            isSuccess = bck.isSuccess;
        }
        
       
    }
}
