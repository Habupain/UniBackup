using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace RemoteBackUp
{
    class Program
    {
        static void Main(string[] args)
        {
            //BackUpManager.DoBackUp();
            //new DriveImageXML();
            vShadow oShadow = new vShadow("c:\\");


            string sShadowPath = oShadow.StartSnapShot();

            WinApiFileReader wApi = new WinApiFileReader();


            Byte[] data = wApi.GetFileData(sShadowPath + "Gothic vamp.png");
            File.WriteAllBytes(@"you.png", data);


            wApi.Close();



            oShadow.Dispose();
            Console.ReadLine();
        }
    }
}
