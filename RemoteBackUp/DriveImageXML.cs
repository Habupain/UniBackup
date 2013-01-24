using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace RemoteBackUp
{
    class DriveImageXML
    {
        private Process proc = new Process();

        public DriveImageXML(Backup bck)
        {
            this.proc.StartInfo.FileName = "dixml.exe";
            //this.proc.StartInfo.Arguments=;
            this.proc.Start();
            Thread.Sleep(1000);
            this.proc.Kill();
        }

        public DriveImageXML()
        {
            this.proc.StartInfo.FileName = "dixml";
            //this.proc.StartInfo.Arguments=;

            
            this.proc.Start();
            this.proc.EnableRaisingEvents = true;
            this.proc.Exited += new EventHandler(proc_Exited);
            Process[] procs = Process.GetProcessesByName("dixml");
            foreach (Process pr in procs)
            {
                pr.EnableRaisingEvents = true;
                pr.Exited += new EventHandler(proc_Exited);

            }
            proc= procs[0];
            while (!proc.HasExited);
        }

        private void proc_Exited(Object sender, EventArgs e)
        {
           
            MessageBox.Show("EXITED");
          
        }
    }
}
