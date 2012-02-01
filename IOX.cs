using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace L24CM
{
    public static class IOX
    {
        public static int CopyTo(this Stream src, Stream dest)
        {
            int size = (src.CanSeek) ? Math.Min((int)(src.Length - src.Position), 0x2000) : 0x2000;
            byte[] buffer = new byte[size];
            int n;
            int nBytes = 0;
            do
            {
                n = src.Read(buffer, 0, buffer.Length);
                dest.Write(buffer, 0, n);
                nBytes += n;
            } while (n != 0);

            return nBytes;
        }
    }
}
