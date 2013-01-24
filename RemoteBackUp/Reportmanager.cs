using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteBackUp
{
    class Reportmanager
    {
        //TODO: GENERATE REPORT
        public static String GenerateReport(Report report)
        {
            String reportText=string.Empty;
            reportText += string.Format("Company:{0}\n", report.companyName);
            reportText += string.Format("Name:{0}\n", report.ownerName);
            reportText += string.Format("Backup User:\n");
            reportText += string.Format("Computer Name:\n");
            reportText += string.Format("Snapeshot Manager:\n");
            reportText += "\n";
            reportText += string.Format("Backup Label:  Backup ABC\n");
            reportText += string.Format("=========================\n");
            reportText += string.Format("Backup Source:\n");
            foreach(String lin in report.backupSources)
                if(lin!=string.Empty)
                     reportText += string.Format("{0}\n",lin);

            reportText += "\n";
            reportText += string.Format("Backup Destination:\n");
            reportText += string.Format("{0}\n",report.backupDestination);
            reportText += "\n";
            reportText += string.Format("Backup scheduled at time:{0} Completed {1}\n",report.backupTime,ReturnCondition(report.isSuccess));
            reportText += "\n";
            reportText += string.Format("Backup Start Time:{0}\n", report.backupStartTime);
            reportText += string.Format("Backup End Time:{0}\n", report.backupEndTime);

            return reportText;
            
        }

        public static String ReturnCondition(bool val)
        {
            if (val)
                return "SuccessFull";
            else
                return "UnSuccessFull";
        }

    }
}
