using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Data.SqlClient;

namespace RFIDRegistration.Public
{
    class PublicFunction
    {
        private static String errorExceedStorageSpace = "Exceeds Internal Storage Space or Unsupported PC value";
        private static String errorInternalStorageLocked = "Internal Storage is Locked";
        private static String errorNotEnoughPower = "Not Enough Power";
        private static String errorOtherError = "Other Errors";

        //Select baud and crc
        public static void SelectBaudAndCrc(long lBaud, byte bCrc)
        {
            byte bRes = 0x04;
            if (lBaud == 9600)
                bRes = 0x00;
            else if (lBaud == 19200)
                bRes = 0x02;
            else if (lBaud == 57600)
                bRes = 0x04;
            else
                bRes = 0x06;

            if (bCrc != 0)
                bRes = (byte)(bRes | 0x01);

            DemoPublic.flagCrc = bRes;
        }

        //Connect Function
        public static bool ConnectRLM(string sPort)
        {
            return DemoPublic.UhfReaderConnect(ref Public.DemoPublic.hCom, sPort, DemoPublic.flagCrc);
            //return DemoPublic.UhfOpenPort(ref DemoPublic.hCom, sPort, DemoPublic.flagCrc);
        }

        // DisConnect Function
        public static bool DisConnectRLM()
        {
            return DemoPublic.UhfReaderDisconnect(ref Public.DemoPublic.hCom, DemoPublic.flagCrc);
        }

        //Get the RLM Status of connect
        public static bool GetConnectStatus()
        {
            byte[] bStatus = new byte[1];
            return DemoPublic.UhfGetPaStatus(DemoPublic.hCom, bStatus, DemoPublic.flagCrc);
        }

        //Get the Power
        public static bool GetPower(byte[] bPower)
        {
            if (DemoPublic.UhfGetPower(DemoPublic.hCom, bPower, DemoPublic.flagCrc))
            {
                //DemoPublic.Power = (bAPower[0] & 0x7F);
                return true;
            }
            else
            {
                return false;
            }
        }

        //Set the Power
        public static bool SetPower(byte bPower)
        {
            byte bOption = 0x01;

            if (DemoPublic.UhfSetPower(DemoPublic.hCom, bOption, bPower, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        //Get the Frequency
        public static bool GetFrequency(byte[] bFreMode, byte[] bFreBase, byte[] bBaseFre, byte[] bFreChannel, byte[] bFreSpc, byte[] bFreHop)
        {
            if (DemoPublic.UhfGetFrequency(DemoPublic.hCom, bFreMode, bFreBase, bBaseFre, bFreChannel, bFreSpc, bFreHop, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        //Set the Frequency
        public static bool SetFrequency(byte FreMode, byte FreBase, byte[] bBaseFre, byte FreChannel, byte FreSpc, byte FreHop)
        {
            if (DemoPublic.UhfSetFrequency(DemoPublic.hCom, FreMode, FreBase, bBaseFre, FreChannel, FreSpc, FreHop, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        //Get the Version
        public static bool GetVersion(byte[] bSerial, byte[] bVersion)
        {
            if (DemoPublic.UhfGetVersion(DemoPublic.hCom, bSerial, bVersion, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        //Read the pruduct uid
        public static bool ReadUID(byte[] Uid)
        {
            if (DemoPublic.UhfGetReaderUID(DemoPublic.hCom, Uid, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        //Inventory Tags Single Function
        public static bool InventorySingle(byte[] UiiLen, byte[] Uii)
        {
            if (DemoPublic.UhfInventorySingleTag(DemoPublic.hCom, UiiLen, Uii, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        public static void GetUiiSingleLoopThread_Timer()
        {
            string uiiStr = "";
            byte[] uiiLen = new byte[1];
            byte[] uii = new byte[255];

            do
            {
                uiiStr = "";
                if (InventorySingle(uiiLen, uii))
                {

                    //System.Media.SystemSounds.Asterisk.Play();
                    uiiStr = DemoPublic.BytesToHexString(uii, uiiLen[0]);
                    DemoPublic.tagInfo.uii = uiiStr;

                    if (!DemoPublic.uiiList.Contains(uiiStr))
                    {
                        DemoPublic.uiiList.Add(uiiStr);
                    }

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
                else
                {
                    Console.WriteLine("Please Place the tag within the range of the antenna");
                }

            } while (DemoPublic.LoopEnable);

            //DemoPublic.TagThread tagThread = new DemoPublic.TagThread(DemoPublic.ShowLoop);
            //DemoPublic.PublicDM.BeginInvoke(tagThread);
        }

        //Stop command
        public static bool Stop()
        {
            if (DemoPublic.UhfStopOperation(DemoPublic.hCom, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }


        //Read data from tag
        public static bool ReadData(byte[] Pwd, byte bank, int addr, byte cnt, byte[] uii, byte[] readdata, byte[] error)
        {
            byte[] ptr = new byte[2];

            if (addr > 127)
            {
                ptr[0] = (byte)((addr >> 7) | 0x80);
                ptr[1] = (byte)(addr & 0x7F);
            }
            else
            {
                ptr[0] = (byte)addr;
            }

            if (DemoPublic.UhfReadDataByEPC(DemoPublic.hCom, Pwd, bank, ptr, cnt, uii, readdata, error, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        //Write data to tag
        public static bool WriteData(byte[] Pwd, byte bank, int addr, byte[] uii, byte[] writedata, byte[] error)
        {
            byte[] ptr = new byte[2];

            if (addr > 127)
            {
                ptr[0] = (byte)((addr >> 7) | 0x80);
                ptr[1] = (byte)(addr & 0x7F);
            }
            else
            {
                ptr[0] = (byte)addr;
            }

            if (DemoPublic.UhfWriteDataByEPC(DemoPublic.hCom, Pwd, bank, ptr, 1, uii, writedata, error, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        //Write Multi data to tag
        public static bool WriteMultiData(byte[] pwd, byte bank, int addr, byte cnt, byte[] Uii, byte[] writedata, byte[] error, byte[] writelen)
        {
            byte[] ruuii = new byte[256];
            byte[] status = new byte[1];
            byte[] ptr = new byte[2];

            if (addr > 127)
            {
                ptr[0] = (byte)((addr >> 7) | 0x80);
                ptr[1] = (byte)(addr & 0x7F);
            }
            else
            {
                ptr[0] = (byte)addr;
            }

            if (DemoPublic.UhfBlockWriteDataByEPC(DemoPublic.hCom, pwd, bank, ptr, cnt, Uii, writedata, error, status, writelen, ruuii, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        // Read data single from tag
        public static bool ReadDataSingle(byte[] Pwd, byte bank, int addr, byte cnt, byte[] uii, byte[] uiilen, byte[] readdata, byte[] error)
        {
            byte[] ptr = new byte[2];

            if (addr > 127)
            {
                ptr[0] = (byte)((addr >> 7) | 0x80);
                ptr[1] = (byte)(addr & 0x7F);
            }
            else
            {
                ptr[0] = (byte)addr;
            }

            if (DemoPublic.UhfReadDataFromSingleTag(DemoPublic.hCom, Pwd, bank, ptr, cnt, readdata, uii, uiilen, error, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        //Write data single to tag
        public static bool WriteDataSingle(byte[] Pwd, byte bank, int addr, byte[] uii, byte[] uiilen, byte[] writedata, byte[] error)
        {
            byte[] ptr = new byte[2];

            if (addr > 127)
            {
                ptr[0] = (byte)((addr >> 7) | 0x80);
                ptr[1] = (byte)(addr & 0x7F);
            }
            else
            {
                ptr[0] = (byte)addr;
            }

            if (DemoPublic.UhfWriteDataToSingleTag(DemoPublic.hCom, Pwd, bank, ptr, 1, writedata, uii, uiilen, error, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        // Write Multi data single to tag
        public static bool WriteMultiDataSingle(byte[] pwd, byte bank, int addr, byte cnt, byte[] writedata, byte[] Uii, byte[] uiilen, byte[] error, byte[] writelen)
        {
            byte[] status = new byte[1];
            byte[] ptr = new byte[2];

            if (addr > 127)
            {
                ptr[0] = (byte)((addr >> 7) | 0x80);
                ptr[1] = (byte)(byte)(addr & 0x7F);
            }
            else
            {
                ptr[0] = (byte)addr;
            }

            if (DemoPublic.UhfBlockWriteDataToSingleTag(DemoPublic.hCom, pwd, bank, ptr, cnt, writedata, Uii, uiilen, status, error, writelen, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        //Read data for cnt is zero from tag
        public static bool ReadDataNoCnt(byte[] Pwd, byte bank, int addr, byte[] uii, byte[] datalen, byte[] readdata, byte[] error)
        {
            byte[] ptr = new byte[2];

            if (addr > 127)
            {
                ptr[0] = (byte)((addr >> 7) | 0x80);
                ptr[1] = (byte)(addr & 0x7F);
            }
            else
            {
                ptr[0] = (byte)addr;
            }

            if (DemoPublic.UhfReadMaxDataByEPC(DemoPublic.hCom, Pwd, bank, ptr, uii, datalen, readdata, error, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        //Read data single for cnt is zero from tag
        public static bool ReadDataSingleNoCnt(byte[] Pwd, byte bank, int addr, byte[] datalen, byte[] readdata, byte[] uii, byte[] uiilen, byte[] error)
        {
            byte[] ptr = new byte[2];

            if (addr > 127)
            {
                ptr[0] = (byte)((addr >> 7) | 0x80);
                ptr[1] = (byte)(addr & 0x7F);
            }
            else
            {
                ptr[0] = (byte)addr;
            }

            if (DemoPublic.UhfReadMaxDataFromSingleTag(DemoPublic.hCom, Pwd, bank, ptr, datalen, readdata, uii, uiilen, error, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        //Read data single for Anti Q
        public static bool ReadDataAntiSingle(byte[] status, byte[] data1_len, byte[] data1, byte[] data2_len, byte[] data2, byte[] uii, byte[] uiilen)
        {
            if (DemoPublic.UhfGetDataFromMultiTag(DemoPublic.hCom, status, data1_len, data1, data2_len, data2, uii, uiilen))
                return true;
            else
                return false;
        }

        //Read data from tag Thread
        /*public static void ReadDataThread_Timer(Object obj)
        {
            byte[] uiiLen = new byte[1];
            byte[] dataLen = new byte[2];
            byte[] readData = new byte[1024];
            byte[] error = new byte[1];
            string uiiStr = "";
            int index = -1;

            DemoPublic.Para para = (DemoPublic.Para)obj;

            byte[] pwd = para.pwd;
            byte[] uii = para.uii;
            byte bank = para.bank;
            int addr = para.addr;
            byte cnt = (byte)para.len;
            bool withuii = para.withuii;

            DemoPublic.TagThread tagThread = new DemoPublic.TagThread(DemoPublic.ShowLoop);
            DemoPublic.AddShow addShow = new DemoPublic.AddShow(DemoPublic.AddToShow);

            do
            {
                //bool successful = withuii ? PublicFunction.ReadData(pwd, bank, addr, cnt, uii, readData, error) : PublicFunction.ReadDataSingle(pwd, bank, addr, cnt, uii, uiiLen, readData, error);
                bool successful = PublicFunction.ReadDataSingle(pwd, bank, addr, cnt, uii, uiiLen, readData, error);
                //uiiStr = DemoPublic.BytesToHexString(uii, withuii ? uiilen : uiiLen[0]);
                uiiStr = DemoPublic.BytesToHexString(uii, uiiLen[0]);

                if (successful)
                {
                    string dataStr = DemoPublic.BytesToHexString(readData, 2 * cnt);
                    if (!DemoPublic.uiiList.Contains(uiiStr))
                        DemoPublic.uiiList.Add(uiiStr);

                    DemoPublic.PublicDM.BeginInvoke(addShow, new object[] { "Successfully Read Data" });
                    DemoPublic.PublicDM.BeginInvoke(new Action<string>((string data) => DemoPublic.PublicDM.TbData.Text = data), new object[] { dataStr });

                }
                else
                {
                    switch (error[0])
                    {
                        case 0x03:
                            DemoPublic.PublicDM.BeginInvoke(addShow, new object[] { errorExceedStorageSpace });
                            break;
                        case 0x04:
                            DemoPublic.PublicDM.BeginInvoke(addShow, new object[] { errorInternalStorageLocked });
                            break;
                        case 0x0B:
                            DemoPublic.PublicDM.BeginInvoke(addShow, new object[] { errorNotEnoughPower });
                            break;
                        case 0xFF: //no tag found: ignore
                            DemoPublic.PublicDM.BeginInvoke(addShow, new object[] { "No Tag Found" });
                            continue;
                        default:
                            DemoPublic.PublicDM.BeginInvoke(addShow, new object[] { errorOtherError });
                            break;
                    }
                    DemoPublic.PublicDM.BeginInvoke(addShow, new object[] { "Failed To Read Data" });
                }

            } while (DemoPublic.LoopEnable);

            DemoPublic.PublicDM.BeginInvoke(tagThread);
        }

        public static void WriteDataThread_Timer(Object obj)
        {
            byte[] uiiLen = new byte[1];
            byte[] writelen = new byte[2];
            byte[] error = new byte[1];
            string uiiStr = "";
            int index = -1;

            DemoPublic.Para para = (DemoPublic.Para)obj;

            byte[] pwd = para.pwd;
            byte[] Uii = para.uii;
            byte len = (byte)para.len;
            byte[] writedata = para.writedata;
            int addr = para.addr;

            bool withuii = para.withuii;

            Console.WriteLine("writedata::" + writedata[0] + " " + writedata[1]);

            int uiilen = 2 * ((Uii[0] >> 3) + 1);

            DemoPublic.AddShow addShow = new DemoPublic.AddShow(DemoPublic.AddToShow);

            do
            {
                bool successful = withuii ? PublicFunction.WriteMultiData(pwd, 0x03, addr, len, Uii, writedata, error, writelen) : PublicFunction.WriteMultiDataSingle(pwd, 0x03, addr, len, writedata, Uii, uiiLen, error, writelen);
                uiiStr = DemoPublic.BytesToHexString(Uii, withuii ? uiilen : uiiLen[0]);
                string dataStr = DemoPublic.BytesToHexString(writedata, 2 * len);

                if (successful)
                {
                    DemoPublic.PublicDM.BeginInvoke(addShow, new object[] { "Successfully Write USER" });
                }
                else
                {
                    switch (error[0])
                    {
                        case 0x03:
                            DemoPublic.PublicDM.BeginInvoke(addShow, new object[] { errorExceedStorageSpace });
                            break;
                        case 0x04:
                            DemoPublic.PublicDM.BeginInvoke(addShow, new object[] { errorInternalStorageLocked });
                            break;
                        case 0x0B:
                            DemoPublic.PublicDM.BeginInvoke(addShow, new object[] { errorNotEnoughPower });
                            break;
                        case 0xFF: //no tag found: ignore
                            DemoPublic.PublicDM.BeginInvoke(addShow, new object[] { "No Tag Found" });
                            continue;
                        default:
                            DemoPublic.PublicDM.BeginInvoke(addShow, new object[] { errorOtherError });
                            break;
                    }
                    DemoPublic.PublicDM.BeginInvoke(addShow, new object[] { "Failed To Write USER" });
                }

            } while (DemoPublic.LoopEnable);

            if (DemoPublic.EPCThread != null)
            {
                DemoPublic.EPCThread.Abort();
            }
        }*/

        // Start read data from multi tags
        public static bool StartReadDataFromMultiTag(byte[] AccessPwd, byte bank1, int addr1, byte cnt1, byte option, byte bank2, int addr2, byte cnt2, byte bQ)
        {
            byte bank = bank1;
            byte[] ptr = new byte[2];
            byte cnt = cnt1;
            byte[] payload = new byte[6];
            int ptr_ebv = 0;

            if (addr1 > 127)
            {
                ptr[0] = (byte)((addr1 >> 7) | 0x80);
                ptr[1] = (byte)(addr1 & 0x7F);
            }
            else
            {
                ptr[0] = (byte)(addr1);
            }

            if (option == 1)
            {
                payload[0] = bank2;

                if (addr2 > 127)
                {
                    payload[1] = (byte)((addr2 >> 7) | 0x80);
                    payload[2] = (byte)(addr2 & 0x7F);
                    ptr_ebv = 1;
                }
                else
                {
                    payload[1] = (byte)(addr2);
                }

                payload[2 + ptr_ebv] = cnt2;
                payload[3 + ptr_ebv] = bQ;
                payload[4 + ptr_ebv] = 0x20;
            }
            else
            {
                payload[0] = bQ;
                payload[1] = 0x20;
            }

            if (DemoPublic.UhfStartReadDataFromMultiTag(DemoPublic.hCom, AccessPwd, bank, ptr, cnt, option, payload, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        //Erase data
        public static bool EraseData(byte[] Pwd, byte bank, int addr, byte cnt, byte[] uii, byte[] error)
        {
            byte[] ptr = new byte[2];

            if (addr > 127)
            {
                ptr[0] = (byte)((addr >> 7) | 0x80);
                ptr[1] = (byte)(addr & 0x7F);
            }
            else
            {
                ptr[0] = (byte)addr;
            }

            if (DemoPublic.UhfEraseDataByEPC(DemoPublic.hCom, Pwd, bank, ptr, cnt, uii, error, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        //Erase data Single (No specified Tag)
        public static bool EraseDataSingle(byte[] Pwd, byte bank, int addr, byte cnt, byte[] uii, byte[] error)
        {
            byte[] ptr = new byte[2];

            if (addr > 127)
            {
                ptr[0] = (byte)((addr >> 7) | 0x80);
                ptr[1] = (byte)(addr & 0x7F);
            }
            else
            {
                ptr[0] = (byte)addr;
            }

            if (DemoPublic.UhfEraseDataFromSingleTag(DemoPublic.hCom, Pwd, bank, ptr, cnt, uii, error, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        // Lock operation 
        public static bool LockMem(byte[] Pwd, byte[] lockdata, byte[] uii, byte[] error)
        {
            if (DemoPublic.UhfLockMemByEPC(DemoPublic.hCom, Pwd, lockdata, uii, error, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        //Lock operation single (no specified tag)
        public static bool LockMemSingle(byte[] Pwd, byte[] lockdata, byte[] uii, byte[] error)
        {
            if (DemoPublic.UhfLockMemFromSingleTag(DemoPublic.hCom, Pwd, lockdata, uii, error, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        // Kill operation
        public static bool KillTag(byte[] Pwd, byte[] uii, byte[] error)
        {
            if (DemoPublic.UhfKillTagByEPC(DemoPublic.hCom, Pwd, uii, error, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        //Kill operation single (no specified tag)
        public static bool KillTagSingle(byte[] Pwd, byte[] uii, byte[] error)
        {
            if (DemoPublic.UhfKillSingleTag(DemoPublic.hCom, Pwd, uii, error, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        // Read register
        public static bool ReadReg(int address, byte[] reg)
        {
            byte[] bStatus = new byte[1];

            if (DemoPublic.UhfGetRegister(DemoPublic.hCom, address, 1, bStatus, reg, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        // Write register
        public static bool WriteReg(int address, byte[] reg)
        {
            byte[] bStatus = new byte[1];

            if (DemoPublic.UhfSetRegister(DemoPublic.hCom, address, 1, reg, bStatus, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        // Reset register
        public static bool ResetReg()
        {
            if (DemoPublic.UhfResetRegister(DemoPublic.hCom, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        //Save register
        public static bool SaveReg()
        {
            if (DemoPublic.UhfSaveRegister(DemoPublic.hCom, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        //Beep Control
        public static bool SetBeep(bool bOpenClose)
        {
            byte[] bBeep = new byte[1];
            byte[] bStatus = new byte[1];

            if (bOpenClose)
                bBeep[0] = 1;
            else
                bBeep[0] = 0;
            if (DemoPublic.UhfSetRegister(DemoPublic.hCom, 288, 1, bBeep, bStatus, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        // Timer Control
        public static bool SetTimer(byte[] Time)
        {
            byte[] bStatus = new byte[1];

            if (DemoPublic.UhfSetRegister(DemoPublic.hCom, 289, 2, Time, bStatus, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        //Sleep mode
        public static bool EnterSleep()
        {
            if (DemoPublic.UhfEnterSleepMode(DemoPublic.hCom, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        //Add select record
        public static bool AddSelect(ref DemoPublic.SRECORD pSRcord)
        {
            byte[] bStatus = new byte[1];

            if (DemoPublic.UhfAddFilter(DemoPublic.hCom, ref pSRcord, bStatus, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        //convert byte[] to string
        public static string CharToCString(byte[] byteinput, int len)
        {
            StringBuilder OutputStr = new StringBuilder(1024);

            DemoPublic.UhfCharToCString(byteinput, OutputStr, len);

            return OutputStr.ToString(0, len * 2);
        }

        //convert string to byte[]

        public static void CStringToChar(string strinput, byte[] byteoutput)
        {
            StringBuilder InputStr = new StringBuilder(strinput);

            DemoPublic.UhfCStringToChar(InputStr, byteoutput, InputStr.Length);
        }

        // get COM count
        public static int GetComNum()
        {
            int num = 0;
            RegistryKey keyCom = Registry.LocalMachine.OpenSubKey("Hardware\\DeviceMap\\SerialComm");

            if (keyCom != null)
            {
                string[] sSubKeys = keyCom.GetValueNames();

                num = sSubKeys.Length;
            }

            return num;
        }

        // Write EPC to single tag (no specified tag)
        public static bool WriteEPCSingle(byte[] Pwd, byte cnt, byte[] uii, byte[] uiilen, byte[] writedata, byte[] error, byte[] writelen)
        {
            byte[] status = new byte[1];

            if (DemoPublic.UhfBlockWriteEPCToSingleTag(DemoPublic.hCom, Pwd, cnt, writedata, uii, uiilen, status, error, writelen, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        // Write EPC to tag
        public static bool WriteEPC(byte[] Pwd, byte cnt, byte[] uii, byte[] writedata, byte[] error, byte[] writelen)
        {
            byte[] ruuii = new byte[256];
            byte[] status = new byte[1];

            if (DemoPublic.UhfBlockWriteEPCByEPC(DemoPublic.hCom, Pwd, cnt, uii, writedata, error, status, writelen, ruuii, DemoPublic.flagCrc))
                return true;
            else
                return false;
        }

        public static int SearchHids(ref string[] serials)
        {
            StringBuilder serialStr = new StringBuilder(1024);

            int hidCnt = DemoPublic.UhfSearchHids(serialStr);

            if (hidCnt == 0)
            {
                return 0;
            }
            else
            {
                serials = serialStr.ToString().Split('|');
            }

            return hidCnt;
        }

        public static bool StarUpdate(string port, byte[] RN)
        {
            byte[] bStatus = new byte[1];
            byte[] bRN32 = new byte[4];

            if (DemoPublic.UhfUpdateInit(ref DemoPublic.hCom, port, bStatus, bRN32, DemoPublic.flagCrc))
            {
                for (int i = 0; i < 4; i++)
                    RN[i] = bRN32[i];

                return true;
            }
            else
            {
                return false;
            }
        }

        //Send RN32 function
        public static bool SendRN(byte[] RN)
        {
            byte[] bStatus = new byte[1];
            byte[] bRN32 = new byte[4];
            bRN32[0] = (byte)~RN[0];
            bRN32[1] = (byte)~RN[1];
            bRN32[2] = (byte)~RN[2];
            bRN32[3] = (byte)~RN[3];

            if (DemoPublic.UhfUpdateSendRN32(DemoPublic.hCom, bRN32, bStatus, DemoPublic.flagCrc))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Start send file function
        public static bool StarTrans(byte[] FILESIZE)
        {
            byte[] bStatus = new byte[1];

            if (DemoPublic.UhfUpdateSendSize(DemoPublic.hCom, bStatus, FILESIZE, DemoPublic.flagCrc))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Send Data Package function
        public static bool TranPackage(byte packnum, byte lastpack, int data_len, byte[] data)
        {
            byte[] bStatus = new byte[1];

            if (DemoPublic.UhfUpdateSendData(DemoPublic.hCom, bStatus, packnum, lastpack, data_len, data, DemoPublic.flagCrc))
            {
                return true;
            }
            else
            {

                return false;
            }
        }

        //End the update operate function
        public static bool EndUpdate()
        {
            byte[] bStatus = new byte[1];

            if (DemoPublic.UhfUpdateCommit(DemoPublic.hCom, bStatus, DemoPublic.flagCrc))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Check whether Data Exists Duplicate in Database of Tag
        public static bool DuplicateTagChecking(string CheckerData, string DatabaseColName, string DatabaseValue)
        {
            int Count = 0;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = @"Data Source=MXPACALEX;Initial Catalog=Dek_MachineDB;Integrated Security=True";
            con.Open();
            
            if (con.State == System.Data.ConnectionState.Open)
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM registration_tag_tb WHERE " + DatabaseColName +" =" + DatabaseValue,con);
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue(DatabaseValue, CheckerData);

                Count = Convert.ToInt32(cmd.ExecuteScalar());

            }
            con.Close();
            if (Count > 0) // Database having more than one data in current column
                return true;
            else
                return false;
        }

        // Check whether Data Exists Duplicate in Database of Device
        public static bool DuplicateDeviceChecking(string CheckerData, string DatabaseColName, string DatabaseValue)
        {
            int Count = 0;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = @"Data Source=MXPACALEX;Initial Catalog=Dek_MachineDB;Integrated Security=True";
            con.Open();

            if (con.State == System.Data.ConnectionState.Open)
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM registration_device_tb WHERE " + DatabaseColName + " =" + DatabaseValue, con);
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue(DatabaseValue, CheckerData);

                Count = Convert.ToInt32(cmd.ExecuteScalar());

            }
            con.Close();
            if (Count > 0) // Database having more than one data in current column
                return true;
            else
                return false;
        }

        // Check whether Part Number Duplicate in Database (Extra)
        public static bool CheckDuplicatePartNo(string PartNoChecker)
        {
            int PartNoCount = 0;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = @"Data Source=MXPACALEX;Initial Catalog=Dek_MachineDB;Integrated Security=True";
            con.Open();

            if (con.State == System.Data.ConnectionState.Open)
            {
                SqlCommand cmd = new SqlCommand("" +
                    "SELECT COUNT(*)" +
                    "FROM registration_tag_tb " +
                    "WHERE part_no = @part_no", con);
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@part_no", PartNoChecker);

                PartNoCount = Convert.ToInt32(cmd.ExecuteScalar());
            }

            con.Close();
            if (PartNoCount > 0)
                return true;
            else
                return false;
        }


    }
}
