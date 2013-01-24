using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace RemoteBackUp
{
    class WinApiFileReader
    {
        const uint GENERIC_READ = 0x80000000;
        const uint OPEN_EXISTING = 3;
        public IntPtr handle;

        [DllImport("kernel32", SetLastError = true)]
        static extern unsafe IntPtr CreateFile
        (
            string FileName,          // file name
            uint DesiredAccess,       // access mode
            uint ShareMode,           // share mode
            uint SecurityAttributes,  // Security Attributes
            uint CreationDisposition, // how to create
            uint FlagsAndAttributes,  // file attributes
            int hTemplateFile         // handle to template file
        );
        [DllImport("kernel32", SetLastError = true)]
        static extern unsafe bool CloseHandle
        (
            System.IntPtr hObject // handle to object
        );

        [DllImport("kernel32", SetLastError = true)]
        static extern unsafe bool ReadFile
        (
            System.IntPtr hFile,      // handle to file
            void* pBuffer,            // data buffer
            int NumberOfBytesToRead,  // number of bytes to read
            int* pNumberOfBytesRead,  // number of bytes read
            int Overlapped            // overlapped buffer
        );

        public Byte[] GetFileData(String filePath)
        {
           
            IntPtr dataP = CreateFile(
                filePath,
                GENERIC_READ,
                0,
                0,
                OPEN_EXISTING,
                0,
                0
            );


            return PointerToData(dataP);
        }

        private unsafe int Read(IntPtr ptr,byte[] buffer, int index, int count)
        {
            int n = 0;
            fixed (byte* p = buffer)
            {
                if (!ReadFile(ptr, p + index, count, &n, 0))
                {
                    return 0;
                }
            }
            return n;
        }

        private Byte[] PointerToData(IntPtr pointer)
        {
            int bufferSize = 128;
            Byte[] data = new Byte[bufferSize];
            List<Byte> bData = new List<byte>();
          
            int bytesRead;
            do
            {
                bytesRead = Read(pointer,data, 0, data.Length);
              
                bData.AddRange(data);
            }
            while (bytesRead > 0);

            //bData.RemoveRange(bData.Count - 1 - bufferSize, bufferSize);
            return bData.ToArray();
        }

        public bool Close()
        {
            return CloseHandle(handle);
        }
    }
}
