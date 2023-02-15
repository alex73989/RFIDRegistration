using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace RFIDRegistration.Public
{
    class DemoPublic
    {
        public struct SRECORD
        {
            public byte Sindex;
            public byte Slen;
            public byte Target;
            public byte Action;
            public byte bank;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] Ptr;
            public byte Len;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] Mask;
            public byte Truncate;
        }

        //标签信息
        public struct TagInfo
        {
            public string uii;
            //public string pc;
            //public string epc;
        }

        public struct Para
        {
            public byte[] uii;
            public byte bank;
            public int addr;
            public int len;
            public byte[] pwd;
            public byte[] writedata;
            public bool withuii;
        }

        public static bool Enabel_flg = false;
        public const string API_Path = "UhfReader_API_HID.dll";
        public static IntPtr hCom;

        public static byte flagCrc = 0x05;
        public static byte bQ = 0x03;

        public static MainForm PublicDM;

        public static Thread EPCThread;
        public static bool LoopEnable = false;

        public static TagInfo tagInfo = new TagInfo();
        public static List<TagInfo> tagList = new List<TagInfo>();
        public static int[] cnt = new int[500];
        public static List<string> uiiList = new List<string>();
        public static double interval = 0.2;

        public delegate void TagThread();
        public delegate void AddShow(string txt);


        #region 
        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //open and connect  
        public static extern int UhfSearchHids(StringBuilder serials);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //open and connect  
        public static extern bool UhfReaderConnect(ref IntPtr hCom, string cPort, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //close and disconnect
        public static extern bool UhfReaderDisconnect(ref IntPtr hCom, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]
        public static extern bool UhfOpenPort(ref IntPtr hCom, string cPort, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]
        public static extern bool UhfClosePort(ref IntPtr hCom);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //get status
        public static extern bool UhfGetPaStatus(IntPtr hCom, byte[] status, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //get power
        public static extern bool UhfGetPower(IntPtr hCom, byte[] power, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //set power
        public static extern bool UhfSetPower(IntPtr hCom, byte option, byte power, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //get fre
        public static extern bool UhfGetFrequency(IntPtr hCom, byte[] fremode, byte[] frebase, byte[] basefre, byte[] channnum, byte[] channspc, byte[] frehop, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //set fre
        public static extern bool UhfSetFrequency(IntPtr hCom, byte fremode, byte frebase, byte[] basefre, byte channnum, byte channspc, byte frehop, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //get version
        public static extern bool UhfGetVersion(IntPtr hCom, byte[] serial, byte[] version, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //get uid
        public static extern bool UhfGetReaderUID(IntPtr hCom, byte[] uid, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //inventory
        public static extern bool UhfStartInventory(IntPtr hCom, byte flagCrcAnti, byte initQ, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               // Get Received
        public static extern bool UhfReadInventory(IntPtr hCom, byte[] ulen, byte[] Uii);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //stop
        public static extern bool UhfStopOperation(IntPtr hCom, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //inventory single
        public static extern bool UhfInventorySingleTag(IntPtr hCom, byte[] ulen, byte[] Uii, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //read data
        public static extern bool UhfReadDataByEPC(IntPtr hCom, byte[] pwd, byte bank, byte[] ptr, byte cnt, byte[] Uii, byte[] readdata, byte[] error, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //write data
        public static extern bool UhfWriteDataByEPC(IntPtr hCom, byte[] pwd, byte bank, byte[] ptr, byte cnt, byte[] Uii, byte[] writedata, byte[] error, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //read data single
        public static extern bool UhfReadDataFromSingleTag(IntPtr hCom, byte[] pwd, byte bank, byte[] ptr, byte cnt, byte[] readdata, byte[] Uii, byte[] uiiLen, byte[] error, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //write data single
        public static extern bool UhfWriteDataToSingleTag(IntPtr hCom, byte[] pwd, byte bank, byte[] ptr, byte cnt, byte[] writedata, byte[] Uii, byte[] uiiLen, byte[] error, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //erase data
        public static extern bool UhfEraseDataByEPC(IntPtr hCom, byte[] pwd, byte bank, byte[] ptr, byte cnt, byte[] Uii, byte[] error, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //lock
        public static extern bool UhfLockMemByEPC(IntPtr hCom, byte[] pwd, byte[] lockdata, byte[] Uii, byte[] error, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //kill
        public static extern bool UhfKillTagByEPC(IntPtr hCom, byte[] pwd, byte[] Uii, byte[] error, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //mutil write data
        public static extern bool UhfBlockWriteDataByEPC(IntPtr hCom, byte[] pwd, byte bank, byte[] ptr, byte cnt, byte[] Uii, byte[] writedata, byte[] error, byte[] status, byte[] writelen, byte[] ruuii, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //read data for Cnt is zero
        public static extern bool UhfReadMaxDataByEPC(IntPtr hCom, byte[] pwd, byte bank, byte[] ptr, byte[] Uii, byte[] datalen, byte[] readdata, byte[] error, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //read data single for Cnt is zero
        public static extern bool UhfReadMaxDataFromSingleTag(IntPtr hCom, byte[] pwd, byte bank, byte[] ptr, byte[] datalen, byte[] readdata, byte[] Uii, byte[] uiiLen, byte[] error, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //read data single for Anti Q
        public static extern bool UhfStartReadDataFromMultiTag(IntPtr hCom, byte[] pwd, byte bank, byte[] ptr, byte cnt, byte option, byte[] playload, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //read data single for Anti Q
        public static extern bool UhfGetDataFromMultiTag(IntPtr hCom, byte[] status, byte[] ufData_len, byte[] ufReadData, byte[] usData_len, byte[] usReadData, byte[] uii, byte[] uiilen);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //erase data single
        public static extern bool UhfEraseDataFromSingleTag(IntPtr hCom, byte[] pwd, byte bank, byte[] ptr, byte cnt, byte[] Uii, byte[] error, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //lock single
        public static extern bool UhfLockMemFromSingleTag(IntPtr hCom, byte[] pwd, byte[] lockdata, byte[] Uii, byte[] error, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //kill single
        public static extern bool UhfKillSingleTag(IntPtr hCom, byte[] pwd, byte[] Uii, byte[] error, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //mutil write data single
        public static extern bool UhfBlockWriteDataToSingleTag(IntPtr hCom, byte[] pwd, byte bank, byte[] ptr, byte cnt, byte[] writedata, byte[] Uii, byte[] uiilen, byte[] status, byte[] error, byte[] writelen, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //read register
        public static extern bool UhfGetRegister(IntPtr hCom, int radd, int rlen, byte[] status, byte[] reg, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //write register
        public static extern bool UhfSetRegister(IntPtr hCom, int radd, int rlen, byte[] reg, byte[] status, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //reset register
        public static extern bool UhfResetRegister(IntPtr hCom, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //save register
        public static extern bool UhfSaveRegister(IntPtr hCom, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //add select
        public static extern bool UhfAddFilter(IntPtr hCom, ref SRECORD pSRecord, byte[] status, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //delete select
        public static extern bool UhfDeleteFilterByIndex(IntPtr hCom, byte sindex, byte[] status, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //get select
        public static extern bool UhfStartGetFilterByIndex(IntPtr hCom, byte sindex, byte snum, byte[] status, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //get select received
        public static extern bool UhfReadFilterByIndex(IntPtr hCom, byte[] status, ref SRECORD pSRecord);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //choose select
        public static extern bool UhfSelectFilterByIndex(IntPtr hCom, byte sindex, byte snum, byte[] status, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //sleep mode
        public static extern bool UhfEnterSleepMode(IntPtr hCom, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //start update
        public static extern bool UhfUpdateInit(ref IntPtr hCom, string cPort, byte[] status, byte[] RN32, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //send inverse
        public static extern bool UhfUpdateSendRN32(IntPtr hCom, byte[] RN32, byte[] status, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //start trans
        public static extern bool UhfUpdateSendSize(IntPtr hCom, byte[] status, byte[] filesize, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //tran package
        public static extern bool UhfUpdateSendData(IntPtr hCom, byte[] status, byte packnum, byte lastpack, int data_len, byte[] trandata, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]                               //end update
        public static extern bool UhfUpdateCommit(IntPtr hCom, byte[] status, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]
        public static extern void UhfCharToCString(byte[] byteinput, StringBuilder stroutput, int len);    //convert byte[] to string 

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]
        public static extern void UhfCStringToChar(StringBuilder strinput, byte[] byteoutput, int len);    //convert string to byte[]

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]
        public static extern bool UhfBlockWriteEPCToSingleTag(IntPtr hCom, byte[] uAccessPwd, byte uCnt, byte[] uWriteData, byte[] uUii, byte[] uLenUii, byte[] uStatus, byte[] uErrorCode, byte[] uWritedLen, byte flagCrc);

        [System.Runtime.InteropServices.DllImportAttribute(API_Path)]
        public static extern bool UhfBlockWriteEPCByEPC(IntPtr hCom, byte[] uAccessPwd, byte uCnt, byte[] uUii, byte[] uWriteData, byte[] uErrorCode, byte[] uStatus, byte[] uWritedLen, byte[] RuUii, byte flagCrc);

        #endregion

        public static bool CheckDigit(string sString)//check if is hex string
        {
            int i;

            bool Res = false;

            char[] c = sString.ToUpper().ToCharArray();
            for (i = 0; i < c.Length; i++)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch((c[i]).ToString(), "[0-9]") || System.Text.RegularExpressions.Regex.IsMatch((c[i]).ToString(), "[A-F]"))
                    Res = true;
                else
                {
                    Res = false;
                    break;
                }
            }
            return Res;
        }

        public static bool CheckDecimal(string sString)//check if is decimal string
        {
            int i;

            bool Res = false;

            char[] c = sString.ToUpper().ToCharArray();
            for (i = 0; i < c.Length; i++)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch((c[i]).ToString(), "[0-9]"))
                    Res = true;
                else
                {
                    Res = false;
                    break;
                }
            }
            return Res;
        }

        public static string StringAddPlace(string InputStr)
        {
            if (InputStr.Length == 0)
                return "";

            string P_str = "";

            for (int i = 0; i < InputStr.Length / 2; i++)
            {
                P_str += InputStr.Substring(2 * i, 2) + " ";
            }

            return P_str;
        }

        public static void HexStringToBytes(string InputStr, byte[] OutPutByte)
        {
            if (InputStr.Length == 0)
                return;

            for (int strLen = 0; strLen < InputStr.Length / 2; strLen++)
                OutPutByte[strLen] = Convert.ToByte(InputStr.Substring(strLen * 2, 2), 16);
        }

        public static byte HexStringToByte(string InputStr)
        {
            if (InputStr.Length != 2)
                return 0x00;

            return Convert.ToByte(InputStr, 16);
        }

        public static string BytesToHexString(byte[] InPutByte, int ConvertLen)
        {
            string OutPutStr = "";
            try
            {
                for (int i = 0; i < ConvertLen; i++)
                {
                    OutPutStr += Convert.ToString((InPutByte[i] >> 4), 16);
                    OutPutStr += Convert.ToString((InPutByte[i] & 0x0F), 16);
                }
                return OutPutStr.ToUpper();
            }
            catch (Exception)
            {
                return "";
            }
        }


        public static string ByteToHexString(byte InputByte)
        {
            string OutPutStr = "";

            try
            {
                OutPutStr += Convert.ToString((InputByte >> 4), 16);
                OutPutStr += Convert.ToString((InputByte & 0x0F), 16);
                return OutPutStr.ToUpper();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static int number = 1;
        public static int TagCount = 0;
        /*public static void ShowLoop()
        {
            PublicDM.DataGVTags.Rows.Clear();
            if (tagList.Count != 0)
            {
                for (int i = 0; i < tagList.Count; i++)
                {
                    String pc = "", epc = "";
                    if (tagList[i].uii.Length != 0)
                    {
                        pc = tagList[i].uii.Substring(0, 4);
                        epc = tagList[i].uii.Substring(4);
                    }
                    //PublicDM.DataGVTags.Rows.Add((i + 1) + "", tagList[i].type, pc, epc, tagList[i].uii.Length / 2, tagList[i].tid, tagList[i].user, tagList[i].err, cnt[i]);
                    PublicDM.DataGVTags.Rows.Add((i + 1) + "", pc, epc, tagList[i].uii.Length / 2, cnt[i]);
                }
                //PublicDM.TbTagCnt.Text = uiiList.Count.ToString();
            }
        }*/


        public static void CntToZero()
        {
            for (int i = 0; i < 500; i++)
            {
                cnt[i] = 0;
            }
        }

        public static void AddToTagList(string type, string uii, string tid, string user, string err)
        {
            DemoPublic.tagInfo.uii = uii;


            int index = DemoPublic.tagList.IndexOf(DemoPublic.tagInfo);
            if (index == -1)
            {
                DemoPublic.tagList.Add(DemoPublic.tagInfo);
                int indexs = DemoPublic.tagList.IndexOf(DemoPublic.tagInfo);
                DemoPublic.cnt[indexs] = 1;
            }
            else
            {
                DemoPublic.cnt[index]++;
            }
        }

        public static void AddToShow(string str)
        {
            //PublicDM.TbShow_Reg.AppendText(str + "\r\n");
        }

    }
}
