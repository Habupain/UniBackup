using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;



namespace RemoteBackUp
{
    internal class vShadow
    {

        private ISHADOW moShadowInterface;
        private Guid moSnapShotSet = new Guid();

        private Guid moSnapShot = new Guid();
        private string msLocalPath;

        private string msShadowPath;

        private void InitializeShadowInterface()
        {
            if (!(Environment.OSVersion.Platform == PlatformID.Win32NT))
            {
                throw new Exception("Volume Shadow Copy not supported on this O/S.");
            }
            else
            {
                switch (Environment.OSVersion.Version.Major)
                {
                    case 3:
                        //---NT 3.51

                        throw new Exception("Volume Shadow Copy not supported on this O/S.");
                    case 4:
                        //---NT 4.0

                        throw new Exception("Volume Shadow Copy not supported on this O/S.");
                    case 5:
                        //---Win2000/XP/2003
                        switch (Environment.OSVersion.Version.Minor)
                        {
                            case 0:
                                //---Win2000

                                throw new Exception("Volume Shadow Copy not supported on this O/S.");
                            case 1:
                                //---WinXP
                                
                                if (IntPtr.Size==8)
                                {
                                    //---64-bit
                                    moShadowInterface = new VSHADOW_XP64();
                                }
                                else if(IntPtr.Size==4)
                                {
                                    //--32-bit
                                    moShadowInterface = new VSHADOW_XP32();
                                }

                                break;
                            case 2:
                                //---Win2003
                                
                                if (IntPtr.Size==8)
                                {
                                    //---64-bit
                                    moShadowInterface = new VSHADOW_03SVR64();

                                }
                                else if(IntPtr.Size==4)
                                {
                                    //--32-bit
                                    moShadowInterface = new VSHADOW_03SVR32();
                                }

                                break;
                        }

                        break;
                    case 6:
                        //---Vista/Windows Server 2008
                       
                        if (IntPtr.Size==8)
                        {
                            //---64-bit
                            moShadowInterface = new VSHADOW_VISTA64();
                        }
                        else if(IntPtr.Size==4)
                        {
                            //--32-bit
                            moShadowInterface = new VSHADOW_VISTA32();
                        }

                        break;
                }
            }
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    break;
            }
        }

        public vShadow()
        {
            InitializeShadowInterface();
        }

        public vShadow(string sPath)
        {
            InitializeShadowInterface();

            msLocalPath = sPath;

            int iIndex = sPath.IndexOf("\\", 0);
            string sVolumeName = sPath.Substring(0, iIndex + 1);
            msLocalPath = sPath.Substring(iIndex + 1, sPath.Length - iIndex - 1);

            moShadowInterface.VSSInitializeForBackup();
            long lRet = moShadowInterface.VSSStartSnapshot(sVolumeName, ref moSnapShotSet, ref moSnapShot);
        }

        public void InitShadowVolume(string sPath)
        {

            msLocalPath = sPath;

            int iIndex = sPath.IndexOf("\\", 0);
            string sVolumeName = sPath.Substring(0, iIndex + 1);
            msLocalPath = sPath.Substring(iIndex + 1, sPath.Length - iIndex - 1);

            moShadowInterface.VSSInitializeForBackup();
            long lRet = moShadowInterface.VSSStartSnapshot(sVolumeName, ref moSnapShotSet, ref moSnapShot);
        }

        public string StartSnapShot()
        {
            string sDeviceName = new string(' ', 255);
            long lRet = moShadowInterface.VSSGetSnapShotDeviceName(ref sDeviceName, ref moSnapShot);

            return sDeviceName + "\\" + msLocalPath;
        }

        public void Dispose()
        {
            moShadowInterface.VSSDeleteAllSnapshots(ref moSnapShotSet);
            moShadowInterface.VSSCloseBackup();
        }

        private interface ISHADOW
        {
            int Version();
            System.UInt32 VSSInitializeForBackup();

            System.UInt32 VSSStartSnapshot(string pwszVolumeName, ref Guid pidSnapShotSet, ref Guid pidSnapShot);

            System.UInt32 VSSGetSnapShotDeviceName(ref string bstrDeviceName, ref Guid pidSnapShot);

            System.UInt32 VSSDeleteAllSnapshots(ref Guid pidSnapShotSet);

            System.UInt32 VSSCloseBackup();
        }

        private class VSHADOW_XP32 : ISHADOW
        {
            [DllImport("vShadow-xp32.dll", EntryPoint = "Version", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern int _Version();
            [DllImport("vShadow-xp32.dll", EntryPoint = "VSSInitializeForBackup", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSInitializeForBackup();
            [DllImport("vShadow-xp32.dll", EntryPoint = "VSSStartSnapshot", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSStartSnapshot(string pwszVolumeName, ref Guid pidSnapShotSet, ref Guid pidSnapShot);
            [DllImport("vShadow-xp32.dll", EntryPoint = "VSSGetSnapShotDeviceName", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSGetSnapShotDeviceName([In(),Out(), MarshalAs(UnmanagedType.BStr)]
ref string bstrDeviceName, ref Guid pidSnapShot);
            [DllImport("vShadow-xp32.dll", EntryPoint = "VSSDeleteAllSnapshots", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSDeleteAllSnapshots(ref Guid pidSnapShotSet);
            [DllImport("vShadow-xp32.dll", EntryPoint = "VSSCloseBackup", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSCloseBackup();

            public int Version()
            {
                return _Version();
            }

            public uint VSSCloseBackup()
            {
                return _VSSCloseBackup();
            }

            public uint VSSDeleteAllSnapshots(ref System.Guid pidSnapShotSet)
            {
                return _VSSDeleteAllSnapshots(ref pidSnapShotSet);
            }

            public uint VSSGetSnapShotDeviceName(ref string bstrDeviceName, ref System.Guid pidSnapShot)
            {
                return _VSSGetSnapShotDeviceName(ref bstrDeviceName, ref pidSnapShot);
            }

            public uint VSSInitializeForBackup()
            {
                return _VSSInitializeForBackup();
            }

            public uint VSSStartSnapshot(string pwszVolumeName, ref System.Guid pidSnapShotSet, ref System.Guid pidSnapShot)
            {
                return _VSSStartSnapshot(pwszVolumeName, ref pidSnapShotSet, ref pidSnapShot);
            }
        }

        private class VSHADOW_XP64 : ISHADOW
        {
            [DllImport("vShadow-xp64.dll", EntryPoint = "Version", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern int _Version();
            [DllImport("vShadow-xp64.dll", EntryPoint = "VSSInitializeForBackup", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSInitializeForBackup();
            [DllImport("vShadow-xp64.dll", EntryPoint = "VSSStartSnapshot", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSStartSnapshot(string pwszVolumeName, ref Guid pidSnapShotSet, ref Guid pidSnapShot);
            [DllImport("vShadow-xp64.dll", EntryPoint = "VSSGetSnapShotDeviceName", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSGetSnapShotDeviceName([In(),Out(), MarshalAs(UnmanagedType.BStr)]
ref string bstrDeviceName, ref Guid pidSnapShot);
            [DllImport("vShadow-xp64.dll", EntryPoint = "VSSDeleteAllSnapshots", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSDeleteAllSnapshots(ref Guid pidSnapShotSet);
            [DllImport("vShadow-xp64.dll", EntryPoint = "VSSCloseBackup", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSCloseBackup();

            public int Version()
            {
                return _Version();
            }

            public uint VSSCloseBackup()
            {
                return _VSSCloseBackup();
            }

            public uint VSSDeleteAllSnapshots(ref System.Guid pidSnapShotSet)
            {
                return _VSSDeleteAllSnapshots(ref pidSnapShotSet);
            }

            public uint VSSGetSnapShotDeviceName(ref string bstrDeviceName, ref System.Guid pidSnapShot)
            {
                return _VSSGetSnapShotDeviceName(ref bstrDeviceName, ref pidSnapShot);
            }

            public uint VSSInitializeForBackup()
            {
                return _VSSInitializeForBackup();
            }

            public uint VSSStartSnapshot(string pwszVolumeName, ref System.Guid pidSnapShotSet, ref System.Guid pidSnapShot)
            {
                return _VSSStartSnapshot(pwszVolumeName, ref pidSnapShotSet, ref pidSnapShot);
            }
        }

        private class VSHADOW_03SVR32 : ISHADOW
        {
            [DllImport("vShadow-03svr32.dll", EntryPoint = "Version", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern int _Version();
            [DllImport("vShadow-03svr32.dll", EntryPoint = "VSSInitializeForBackup", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSInitializeForBackup();
            [DllImport("vShadow-03svr32.dll", EntryPoint = "VSSStartSnapshot", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSStartSnapshot(string pwszVolumeName, ref Guid pidSnapShotSet, ref Guid pidSnapShot);
            [DllImport("vShadow-03svr32.dll", EntryPoint = "VSSGetSnapShotDeviceName", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSGetSnapShotDeviceName([In(),Out(), MarshalAs(UnmanagedType.BStr)]
ref string bstrDeviceName, ref Guid pidSnapShot);
            [DllImport("vShadow-03svr32.dll", EntryPoint = "VSSDeleteAllSnapshots", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSDeleteAllSnapshots(ref Guid pidSnapShotSet);
            [DllImport("vShadow-03svr32.dll", EntryPoint = "VSSCloseBackup", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSCloseBackup();

            public int Version()
            {
                return _Version();
            }

            public uint VSSCloseBackup()
            {
                return _VSSCloseBackup();
            }

            public uint VSSDeleteAllSnapshots(ref System.Guid pidSnapShotSet)
            {
                return _VSSDeleteAllSnapshots(ref pidSnapShotSet);
            }

            public uint VSSGetSnapShotDeviceName(ref string bstrDeviceName, ref System.Guid pidSnapShot)
            {
                return _VSSGetSnapShotDeviceName(ref bstrDeviceName, ref pidSnapShot);
            }

            public uint VSSInitializeForBackup()
            {
                return _VSSInitializeForBackup();
            }

            public uint VSSStartSnapshot(string pwszVolumeName, ref System.Guid pidSnapShotSet, ref System.Guid pidSnapShot)
            {
                return _VSSStartSnapshot(pwszVolumeName, ref pidSnapShotSet, ref pidSnapShot);
            }
        }

        private class VSHADOW_03SVR64 : ISHADOW
        {
            [DllImport("vShadow-03svr64.dll", EntryPoint = "Version", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern int _Version();
            [DllImport("vShadow-03svr64.dll", EntryPoint = "VSSInitializeForBackup", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSInitializeForBackup();
            [DllImport("vShadow-03svr64.dll", EntryPoint = "VSSStartSnapshot", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSStartSnapshot(string pwszVolumeName, ref Guid pidSnapShotSet, ref Guid pidSnapShot);
            [DllImport("vShadow-03svr64.dll", EntryPoint = "VSSGetSnapShotDeviceName", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSGetSnapShotDeviceName([In(),Out(), MarshalAs(UnmanagedType.BStr)]
ref string bstrDeviceName, ref Guid pidSnapShot);
            [DllImport("vShadow-03svr64.dll", EntryPoint = "VSSDeleteAllSnapshots", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSDeleteAllSnapshots(ref Guid pidSnapShotSet);
            [DllImport("vShadow-03svr64.dll", EntryPoint = "VSSCloseBackup", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSCloseBackup();

            public int Version()
            {
                return _Version();
            }

            public uint VSSCloseBackup()
            {
                return _VSSCloseBackup();
            }

            public uint VSSDeleteAllSnapshots(ref System.Guid pidSnapShotSet)
            {
                return _VSSDeleteAllSnapshots(ref pidSnapShotSet);
            }

            public uint VSSGetSnapShotDeviceName(ref string bstrDeviceName, ref System.Guid pidSnapShot)
            {
                return _VSSGetSnapShotDeviceName(ref bstrDeviceName, ref pidSnapShot);
            }

            public uint VSSInitializeForBackup()
            {
                return _VSSInitializeForBackup();
            }

            public uint VSSStartSnapshot(string pwszVolumeName, ref System.Guid pidSnapShotSet, ref System.Guid pidSnapShot)
            {
                return _VSSStartSnapshot(pwszVolumeName, ref pidSnapShotSet, ref pidSnapShot);
            }
        }

        private class VSHADOW_VISTA32 : ISHADOW
        {
            [DllImport("vShadow-Vista32.dll", EntryPoint = "Version", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern int _Version();
            [DllImport("vShadow-Vista32.dll", EntryPoint = "VSSInitializeForBackup", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSInitializeForBackup();
            [DllImport("vShadow-Vista32.dll", EntryPoint = "VSSStartSnapshot", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSStartSnapshot(string pwszVolumeName, ref Guid pidSnapShotSet, ref Guid pidSnapShot);
            [DllImport("vShadow-Vista32.dll", EntryPoint = "VSSGetSnapShotDeviceName", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSGetSnapShotDeviceName([In(),Out(), MarshalAs(UnmanagedType.BStr)]
ref string bstrDeviceName, ref Guid pidSnapShot);
            [DllImport("vShadow-Vista32.dll", EntryPoint = "VSSDeleteAllSnapshots", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSDeleteAllSnapshots(ref Guid pidSnapShotSet);
            [DllImport("vShadow-Vista32.dll", EntryPoint = "VSSCloseBackup", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSCloseBackup();

            public int Version()
            {
                return _Version();
            }

            public uint VSSCloseBackup()
            {
                return _VSSCloseBackup();
            }

            public uint VSSDeleteAllSnapshots(ref System.Guid pidSnapShotSet)
            {
                return _VSSDeleteAllSnapshots(ref pidSnapShotSet);
            }

            public uint VSSGetSnapShotDeviceName(ref string bstrDeviceName, ref System.Guid pidSnapShot)
            {
                return _VSSGetSnapShotDeviceName(ref bstrDeviceName, ref pidSnapShot);
            }

            public uint VSSInitializeForBackup()
            {
                return _VSSInitializeForBackup();
            }

            public uint VSSStartSnapshot(string pwszVolumeName, ref System.Guid pidSnapShotSet, ref System.Guid pidSnapShot)
            {
                return _VSSStartSnapshot(pwszVolumeName, ref pidSnapShotSet, ref pidSnapShot);
            }
        }

        private class VSHADOW_VISTA64 : ISHADOW
        {
            [DllImport("vShadow-Vista64.dll", EntryPoint = "Version", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern int _Version();
            [DllImport("vShadow-Vista64.dll", EntryPoint = "VSSInitializeForBackup", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSInitializeForBackup();
            [DllImport("vShadow-Vista64.dll", EntryPoint = "VSSStartSnapshot", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSStartSnapshot(string pwszVolumeName, ref Guid pidSnapShotSet, ref Guid pidSnapShot);
            [DllImport("vShadow-Vista64.dll", EntryPoint = "VSSGetSnapShotDeviceName", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSGetSnapShotDeviceName([In(),Out(), MarshalAs(UnmanagedType.BStr)]
ref string bstrDeviceName, ref Guid pidSnapShot);
            [DllImport("vShadow-Vista64.dll", EntryPoint = "VSSDeleteAllSnapshots", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSDeleteAllSnapshots(ref Guid pidSnapShotSet);
            [DllImport("vShadow-Vista64.dll", EntryPoint = "VSSCloseBackup", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]

            private static extern System.UInt32 _VSSCloseBackup();

            public int Version()
            {
                return _Version();
            }

            public uint VSSCloseBackup()
            {
                return _VSSCloseBackup();
            }

            public uint VSSDeleteAllSnapshots(ref System.Guid pidSnapShotSet)
            {
                return _VSSDeleteAllSnapshots(ref pidSnapShotSet);
            }

            public uint VSSGetSnapShotDeviceName(ref string bstrDeviceName, ref System.Guid pidSnapShot)
            {
                return _VSSGetSnapShotDeviceName(ref bstrDeviceName, ref pidSnapShot);
            }

            public uint VSSInitializeForBackup()
            {
                return _VSSInitializeForBackup();
            }

            public uint VSSStartSnapshot(string pwszVolumeName, ref System.Guid pidSnapShotSet, ref System.Guid pidSnapShot)
            {
                return _VSSStartSnapshot(pwszVolumeName, ref pidSnapShotSet, ref pidSnapShot);
            }

           
        }
    }
}