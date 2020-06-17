using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace Utility.Standard
{
    public class RSpcFile
    {
        FileStream fileStream;
        spcdr m_SpcHdr;
        subhdr m_SubHdr;
        public string m_strError;

        // 打开spc文件
        public bool Open(string fileName)
        {
            try
            {
                fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                m_SpcHdr = new spcdr();
                m_SubHdr = new subhdr();
            }
            catch (System.Exception ex)
            {
                m_strError = ex.Message;
                return false;
            }


            return true;
        }

        // 关闭spc文件
        public bool Close()
        {
            try
            {
                fileStream.Close();
                fileStream = null;
            }
            catch (System.Exception ex)
            {
                m_strError = ex.Message;
                return false;
            }
            finally
            {

            }
            return true;
        }

        public const int G_RIGHTSPC_KEY_IS_FTFLGS = 0x80;// single file
        public const int G_RIGHTSPC_KEY_IS_FVERSN = 0x4b;// new format
        public const int G_RIGHTSPC_KEY_IS_FEXPER = 0x0b;// raman spectrum
        public const int G_RIGHTSPC_KEY_IS_FEXP = 0x80;// ieee 32-bit floats

        // 判断Spc是否正确
        public bool IsRightSpcFile(string sFileName)
        {
            bool bRe = true;
            try
            {
                FileStream fs = new FileStream(sFileName, FileMode.Open, FileAccess.Read);
                spcdr SpcHdr = new spcdr();

                byte[] bytData = new byte[Marshal.SizeOf(SpcHdr)];
                fs.Read(bytData, 0, bytData.Length);
                SpcHdr = (spcdr)BytesToStruct(bytData, typeof(spcdr));

                if (SpcHdr.ftflgs != G_RIGHTSPC_KEY_IS_FTFLGS)
                {
                    bRe = false;
                }

                if (SpcHdr.fversn != G_RIGHTSPC_KEY_IS_FVERSN)
                {
                    bRe = false;
                }
                if (SpcHdr.fexper != G_RIGHTSPC_KEY_IS_FEXPER)
                {
                    bRe = false;
                }
                if (SpcHdr.fexp != G_RIGHTSPC_KEY_IS_FEXP)
                {
                    bRe = false;
                }
                bytData = null;
                fs.Flush();
                fs.Close();
                fs = null;
            }
            catch (System.Exception ex)
            {
                bRe = false;
            }
            finally
            {

            }

            return bRe;
        }


        /// <summary>
        /// Read for single file type
        /// </summary>
        /// <param name="xdata"></param>
        /// <param name="ydata"></param>
        /// <returns></returns>
        public bool Read(out float[] xdata, out float[] ydata)
        {
            spcdr strsd = m_SpcHdr;
            subhdr strsh = m_SubHdr;

            try
            {
                ReadSpcHdr(out strsd);
                int count = Convert.ToInt32(strsd.fnpts);
                ReadData(out xdata, count);
                ReadSubHdr(out strsh);
                ReadData(out ydata, count);
            }
            catch (Exception)
            {
                xdata = new float[1];
                ydata = new float[1];
                return false;
            }

            return true;
        }

        /// <summary>
        /// write for single file type
        /// </summary>
        /// <param name="xdata"></param>
        /// <param name="ydata"></param>
        /// <returns></returns>
        public bool Write(float[] xdata, float[] ydata)
        {
            spcdr strsd = m_SpcHdr;
            subhdr strsh = m_SubHdr;

            if (xdata == null || ydata == null)
                return false;

            if (xdata.Length != ydata.Length)
                return false;

            // spchdr
            strsd.ftflgs = 0x80; // single file
            strsd.fversn = 0x4b; // new format
            strsd.fexper = 0x0b; // raman spectrum
            strsd.fexp = 0x80; // ieee 32-bit floats
            strsd.fnpts = Convert.ToUInt32(xdata.Length);
            strsd.ffirst = xdata[0];
            strsd.flast = xdata[xdata.Length - 1];
            strsd.fnsub = 1; // only one subfile
            strsd.fxtype = 13; // raman shift
            strsd.fytype = 4; // counts
            strsd.fztype = 0; // arbitrary
            strsd.fpost = 0; // 0
            strsd.fdate = 0;
            ArrayClear(out strsd.fres, 9); // 0
            ArrayClear(out strsd.fsource, 9);
            strsd.fpeakpt = 0; // not known
            ArrayClear(out strsd.fspare, 8);
            ArrayClear(out strsd.fcmnt, 130);
            ArrayClear(out strsd.fcatxt, 30); // 0
            strsd.flogoff = 0; // no log block
            strsd.fmods = 0; // 0
            strsd.fprocs = 0; // 0
            strsd.flevel = 0; // 0
            strsd.fsampin = 0; // 0
            strsd.ffactor = 0; // 0
            ArrayClear(out strsd.fmethod, 48); // 0
            strsd.fzinc = 0; // 0 for single file
            strsd.fwplanes = 0; // 0 for single file
            strsd.fwinc = 0; // 0 for single file
            strsd.fwtype = 0; // arbitrary
            ArrayClear(out strsd.freserv, 187); // 0

            // subhdr
            strsh.subflgs = 0; // 0
            strsh.subexp = 0; // fexp in spcdr is set
            strsh.subindx = 0; // only one sub file, 0 = start index
            strsh.subtime = 0; // 0
            strsh.subnext = 0; // 0
            strsh.subnois = 0; // 0
            strsh.subnpts = 0; // 0
            strsh.subscan = 0; // 0
            strsh.subwlevel = 0; // 0
            ArrayClear(out strsh.subresv, 4); // 0

            // write private params
            //strsd.fdate = DateTime2Uint(); 
            //ArraySet(ref strsd.fsource, deviceName);
            //strsd.fspare[0] = Convert.ToSingle(laserPower);
            //strsd.fspare[1] = Convert.ToSingle(intTime);
            //strsd.fspare[2] = Convert.ToSingle(reNum);
            //strsd.fspare[3] = Convert.ToSingle();
            //strsd.fspare[4] = Convert.ToSingle();
            //strsd.fspare[5] = Convert.ToSingle();
            //strsd.fspare[6] = Convert.ToSingle();
            //strsd.fspare[7] = Convert.ToSingle();

            //string[] pair = new string[8];float[] peakPos
            //pair[0] = 
            //pair[1] = 
            //pair[2] = 
            //pair[3] = 
            //pair[4] = 
            //pair[5] = 
            //pair[6] = 
            //pair[7] = 
            //ArraySet(ref strsd.fcmnt, String.Join("|", pair));

            WriteSpcHdr(strsd);
            WriteData(xdata);
            WriteSubHdr(strsh);
            WriteData(ydata);
            fileStream.Flush();
            return true;
        }

        private uint DateTime2Uint(DateTime date)
        {
            int year_s = 20;
            int month_s = 16;
            int day_s = 11;
            int hour_s = 6;
            int minute_s = 0;

            uint ret = 0;
            ret += (Convert.ToUInt32(date.Year) << year_s);
            ret += (Convert.ToUInt32(date.Month) << month_s);
            ret += (Convert.ToUInt32(date.Day) << day_s);
            ret += (Convert.ToUInt32(date.Hour) << hour_s);
            ret += (Convert.ToUInt32(date.Minute) << minute_s);

            return ret;
        }

        private DateTime Uint2DateTime(uint val)
        {
            int year_s = 20;
            int month_s = 16;
            int day_s = 11;
            int hour_s = 6;
            int minute_s = 0;

            uint year = (Convert.ToUInt32(0xfff) << year_s);
            uint month = (Convert.ToUInt32(0xf) << month_s);
            uint day = (Convert.ToUInt32(0x1f) << day_s);
            uint hour = (Convert.ToUInt32(0x1f) << hour_s);
            uint minute = (Convert.ToUInt32(0x3f) << minute_s);

            DateTime ret = new DateTime(
                Convert.ToInt32((val & year) >> year_s),
                Convert.ToInt32((val & month) >> month_s),
                Convert.ToInt32((val & day) >> day_s),
                Convert.ToInt32((val & hour) >> hour_s),
                Convert.ToInt32((val & minute) >> minute_s), 0);

            return ret;
        }

        private void ArrayClear(out byte[] array, int length)
        {
            array = new byte[length];
            Array.Clear(array, 0, array.Length);
        }

        private void ArrayClear(out float[] array, int length)
        {
            array = new float[length];
            Array.Clear(array, 0, array.Length);
        }

        private bool ArraySet(ref byte[] array, string str)
        {
            int length = array.Length > str.Length ? str.Length : array.Length - 1;
            Array.Clear(array, 0, array.Length);

            for (int i = 0; i < length; i++)
            {
                array[i] = Convert.ToByte(str[i]);
            }
            return true;
        }

        private string ArrayGet(byte[] array)
        {
            StringBuilder ret = new StringBuilder();

            for (int i = 0; i < array.Length - 1; i++)
            {
                if (array[i] == 0)
                    break;

                ret.Append(Convert.ToChar(array[i]));
            }
            return ret.ToString();
        }

        // 读取文件头
        private bool ReadSpcHdr(out spcdr SpcHdr)
        {
            SpcHdr = new spcdr();
            byte[] bytData = new byte[Marshal.SizeOf(SpcHdr)];
            fileStream.Read(bytData, 0, bytData.Length);
            SpcHdr = (spcdr)BytesToStruct(bytData, typeof(spcdr));
            /*
            bytData = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
			*/

            return true;
        }

        // 写入文件头
        private bool WriteSpcHdr(spcdr SpcHdr)
        {
            byte[] bytData = StructToBytes(SpcHdr);
            fileStream.Write(bytData, 0, bytData.Length);
            /*
            bytData = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
			*/
            return true;
        }

        // 读取副头文件
        private bool ReadSubHdr(out subhdr SubHdr)
        {
            SubHdr = new subhdr();
            byte[] bytData = new byte[Marshal.SizeOf(SubHdr)];
            fileStream.Read(bytData, 0, bytData.Length);
            SubHdr = (subhdr)BytesToStruct(bytData, typeof(subhdr));
            /*
            bytData = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
			*/
            return true;
        }

        // 写入副头文件
        private bool WriteSubHdr(subhdr SubHdr)
        {
            byte[] bytData = StructToBytes(SubHdr);
            fileStream.Write(bytData, 0, bytData.Length);
            /*
            bytData = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
			*/
            return true;
        }

        // 读取数据
        private bool ReadData(out float[] data, int count)
        {
            float a = new float();
            int size = Marshal.SizeOf(a);
            byte[] bytData = new byte[count * size];
            fileStream.Read(bytData, 0, bytData.Length);

            data = new float[count];
            for (int i = 0; i < count; i++)
            {
                data[i] = BitConverter.ToSingle(bytData, i * size);
            }
            /*
            bytData = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
			*/
            return true;
        }

        // 写入数据
        private bool WriteData(float[] fdata)
        {
            int size = Marshal.SizeOf(fdata[0]) * fdata.Length;

            IntPtr pnt = Marshal.AllocHGlobal(size);
            Marshal.Copy(fdata, 0, pnt, fdata.Length);

            byte[] bytData = new byte[size];
            Marshal.Copy(pnt, bytData, 0, size);

            fileStream.Write(bytData, 0, size);
            Marshal.FreeHGlobal(pnt);
            /*
            bytData = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
			*/
            return true;
        }

        private static byte[] StructToBytes(object structObj)
        {
            int size = Marshal.SizeOf(structObj);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structObj, buffer, false);
                byte[] bytes = new byte[size];
                Marshal.Copy(buffer, bytes, 0, size);
                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
                /*
                GC.Collect();
                GC.WaitForPendingFinalizers();
				*/
            }

        }

        private static object BytesToStruct(byte[] bytes, Type strcutType)
        {
            int size = Marshal.SizeOf(strcutType);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, buffer, size);
                return Marshal.PtrToStructure(buffer, strcutType);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
                /*
                GC.Collect();
                GC.WaitForPendingFinalizers();
				*/
            }
        }
    }
    //spc文件结构体
    public struct spcdr
    {
        public byte ftflgs;	    /* Flag bits defined below */
        public byte fversn;	    /* 0x4B=> new LSB 1st, 0x4C=> new MSB 1st, 0x4D=> old format */
        public byte fexper;	    /* Instrument technique code (see below) */
        public byte fexp; 	    /* Fraction scaling exponent integer (80h=>float) */
        public uint fnpts;	    /* Integer number of points (or TXYXYS directory position) */
        public double ffirst;	/* Floating X coordinate of first point */
        public double flast;	/* Floating X coordinate of last point */
        public uint fnsub; 	    /* Integer number of subfiles (1 if not TMULTI) */
        public byte fxtype;	    /* Type of X axis units (see definitions below) */
        public byte fytype;	    /* Type of Y axis units (see definitions below) */
        public byte fztype;	    /* Type of Z axis units (see definitions below) */
        public byte fpost;	    /* Posting disposition (see GRAMSDDE.H) */
        public uint fdate;	    /* Date/Time LSB: min=6b,hour=5b,day=5b,month=4b,year=12b */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public byte[] fres;	    /* Resolution description text (null terminated) */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public byte[] fsource;	/* Source instrument description text (null terminated) */
        public ushort fpeakpt;	/* Peak point number for interferograms (0=not known) */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public float[] fspare;	/* Used for Array Basic storage */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 130)]
        public byte[] fcmnt;	/* Null terminated comment ASCII text string */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
        public byte[] fcatxt;	/* X,Y,Z axis label strings if ftflgs=TALABS */
        public uint flogoff;	/* File offset to log block or 0 (see above) */
        public uint fmods;	    /* File Modification Flags (see below: 1=A,2=B,4=C,8=D..) */
        public byte fprocs;	    /* Processing code (see GRAMSDDE.H) */
        public byte flevel;	    /* Calibration level plus one (1 = not calibration data) */
        public ushort fsampin;	/* Sub-method sample injection number (1 = first or only ) */
        public float ffactor;	/* Floating data multiplier concentration factor (IEEE-32) */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] fmethod;	/* Method/program/data filename w/extensions comma list */
        public float fzinc;	    /* Z subfile increment (0 = use 1st subnext-subfirst) */
        public uint fwplanes;	/* Number of planes for 4D with W dimension (0=normal) */
        public float fwinc;	    /* W plane increment (only if fwplanes is not 0) */
        public byte fwtype;	    /* Type of W axis units (see definitions below) */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 187)]
        public byte[] freserv;  /* Reserved (must be set to zero) */
    }

    //[StructLayout(LayoutKind.Explicit)]
    public struct subhdr
    {
        public byte subflgs;	    /* Flags as defined above */
        public byte subexp;	        /* Exponent for sub-file's Y values (80h=>float) */
        public ushort subindx;	    /* Integer index number of trace subfile (0=first) */
        public float subtime;	    /* Floating time for trace (Z axis corrdinate) */
        public float subnext;	    /* Floating time for next trace (May be same as beg) */
        public float subnois;	    /* Floating peak pick noise level if high byte nonzero */
        public uint subnpts;	    /* Integer number of subfile points for TXYXYS type */
        public uint subscan;	    /* Integer number of co-added scans or 0 (for collect) */
        public float subwlevel;	    /* Floating W axis value (if fwplanes non-zero) */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] subresv;	    /* Reserved area (must be set to zero) */
    }
}
