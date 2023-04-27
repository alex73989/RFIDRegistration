using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RFIDRegistration
{
    public partial class TagConfig : Form
    {
        public static TagConfig instance;

        public TagConfig()
        {
            InitializeComponent();
            ResetFS();
            ResetART();
            ResetTR();
            ResetTW();
            ResetTS();
            instance = this;
        }

        public void ResetFS()
        {
            cBox_FS_TagSelectionMode.Items.Clear();

            cBox_FS_TagSelectionMode.Items.AddRange(new string[]
            {
                "0 = Disable Tag Selection",
                "1 = Tag Filter with Tag EPC",
                "2 = Tag Filter with TID Memory Bank",
                "3 = Tag Filter with USER Memory Bank",
                "4 = Tag Filter with EPC Memory Bank"
            });

            tBox_FS_BitAddress.Enabled= false;
            tBox_FS_LenBit.Enabled= false;
            tBox_FS_Data.Enabled= false;
            chBox_FS_InvertTrue.Enabled= false;
            chBox_FS_InvertFalse.Enabled= false;

            cBox_FS_TagSelectionMode.Text = string.Empty;
            tBox_FS_BitAddress.Text = string.Empty;
            tBox_FS_LenBit.Text = string.Empty;
            tBox_FS_Data.Text = string.Empty;
            chBox_FS_InvertTrue.Checked = false;
            chBox_FS_InvertFalse.Checked = false;
        }

        public void ResetART()
        {
            cBox_ART_MemoryBankType.Items.Clear();

            cBox_ART_MemoryBankType.Items.AddRange(new string[]
            {
                "0 = RESERVED",
                "1 = EPC",
                "2 = TID",
                "3 = USER"
            });

            cBox_ART_MemoryBankType.Enabled= false;
            tBox_ART_StartAddr.Enabled= false;
            tBox_ART_WordCount.Enabled= false;
            chBox_ART_EnableTagPassword.Enabled= false;
            chBox_ART_DisableTagPassword.Enabled = false;
            tBox_ART_TagPassword.Enabled = false;

            chBox_ART_EnableTagData.Checked = false;
            chBox_ART_DisableTagData.Checked = false;
            cBox_ART_MemoryBankType.Text = string.Empty;
            tBox_ART_StartAddr.Text = string.Empty;
            tBox_ART_WordCount.Text = string.Empty;
            chBox_ART_EnableTagPassword.Checked = false;
            chBox_ART_DisableTagPassword.Checked = false;
            tBox_ART_TagPassword.Text = string.Empty;
        }

        public void ResetTR()
        {
            cBox_TR_ReadOption.Items.Clear();
            cBox_TR_TSOption.Items.Clear();

            cBox_TR_ReadOption.Items.AddRange(new string[]
            {
                "0 = RESERVED Memory Bank",
                "1 = EPC Memory Bank",
                "2 = TID Memory Bank",
                "3 = USER Memory Bank"
            });

            cBox_TR_TSOption.Items.AddRange(new string[]
            {
                "0 = Disable Tag Selection",
                "1 = Tag Filter with Tag EPC",
                "2 = Tag Filter with TID Memory Bank",
                "3 = Tag Filter with USER Memory Bank",
                "4 = Tag Filter with EPC Memory Bank"
            });

            tBox_TR_Timeout.Enabled = false;
            tBox_TR_StartAddr.Enabled = false;
            tBox_TR_WordCount.Enabled = false;
            tBox_TR_Password.Enabled = false;
            cBox_TR_TSOption.Enabled = false;
            tBox_TR_TSStartAddr.Enabled = false;
            tBox_TR_TSLenBit.Enabled = false;
            tBox_TR_TSData.Enabled = false;
            chBox_TR_TSEnableInvert.Enabled = false;
            chBox_TR_TSDisableInvert.Enabled = false;

            cBox_TR_ReadOption.Text = string.Empty;
            tBox_TR_Timeout.Text = string.Empty;
            tBox_TR_StartAddr.Text = string.Empty;
            tBox_TR_WordCount.Text = string.Empty;
            tBox_TR_Password.Text = string.Empty;
            cBox_TR_TSOption.Text = string.Empty;
            tBox_TR_TSStartAddr.Text = string.Empty;
            tBox_TR_TSLenBit.Text = string.Empty;
            tBox_TR_TSData.Text = string.Empty;
            chBox_TR_TSEnableInvert.Checked = false;
            chBox_TR_TSDisableInvert.Checked = false;
        }

        public void ResetTW()
        {
            cBox_TW_WriteOption.Items.Clear();
            cBox_TW_TSOption.Items.Clear();

            cBox_TW_WriteOption.Items.AddRange(new string[]
            {
                "0 = RESERVED Memory Bank",
                "1 = EPC Memory Bank",
                "2 = TID Memory Bank",
                "3 = USER Memory Bank",
                "4 = Replace Tag EPC",
                "5 = Lock Tag Memory",
                "6 = Kill Tag"
            });

            cBox_TW_TSOption.Items.AddRange(new string[]
            {
                "0 = Disable Tag Selection",
                "1 = Tag Filter with Tag EPC",
                "2 = Tag Filter with TID Memory Bank",
                "3 = Tag Filter with USER Memory Bank",
                "4 = Tag Filter with EPC Memory Bank"
            });

            tBox_TW_Timeout.Enabled = false;
            tBox_TW_WriteAddress.Enabled = false;
            tBox_TW_WriteData.Enabled= false;
            tBox_TW_LockMask.Enabled = false;
            tBox_TW_LockAction.Enabled = false;
            tBox_TW_Password.Enabled = false;
            cBox_TW_TSOption.Enabled = false;
            tBox_TW_TSStartAddress.Enabled = false;
            tBox_TW_TSLenBit.Enabled = false;
            tBox_TW_TSData.Enabled = false;
            chBox_TW_InvertTrue.Enabled = false;
            chBox_TW_InvertFalse.Enabled = false;


            cBox_TW_WriteOption.Text = string.Empty;
            tBox_TW_Timeout.Text = string.Empty;
            tBox_TW_WriteAddress.Text = string.Empty;
            tBox_TW_WriteData.Text = string.Empty;
            tBox_TW_LockMask.Text = string.Empty;
            tBox_TW_LockAction.Text = string.Empty;
            tBox_TW_Password.Text = string.Empty;
            cBox_TW_TSOption.Text = string.Empty;
            tBox_TW_TSStartAddress.Text = string.Empty;
            tBox_TW_TSLenBit.Text = string.Empty;
            tBox_TW_TSData.Text = string.Empty;
            chBox_TW_InvertTrue.Checked = false;
            chBox_TW_InvertFalse.Checked = false;
        }

        public void ResetTS()
        {
            chBox_TS_Enable.Checked= false;
            chBox_TS_Disable.Checked= false;

            tBox_TS_RatePeriod.Enabled = false;
            tBox_TS_RatePeriod.Text = string.Empty;
        }

        public void SetLogText(string GetText)
        {
            tBox_log.Text += DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + " : " + Environment.NewLine + GetText + Environment.NewLine + Environment.NewLine;
        }

        private void btn_FS_Clear_Click(object sender, EventArgs e)
        {
            ResetFS();
            SetLogText("(Filter Settings) Reset Successfully!");
        }

        private void btn_ART_Clear_Click(object sender, EventArgs e)
        {
            ResetART();
            SetLogText("(Auto Read Tag) Reset Successfully!");
        }

        private void btn_TR_Clear_Click(object sender, EventArgs e)
        {
            ResetTR();
            SetLogText("(Tag Reading) Reset Successfully!");
        }

        private void btn_TW_Clear_Click(object sender, EventArgs e)
        {
            ResetTW();
            SetLogText("(Tag Writting) Reset Successfully!");
        }

        private void btn_TS_Clear_Click(object sender, EventArgs e)
        {
            ResetTS();
            SetLogText("(Temp Sensor) Reset Successfully!");
        }

        private void btn_FS_Proceed_Click(object sender, EventArgs e)
        {
            if(cBox_FS_TagSelectionMode.Text != null && tBox_FS_BitAddress.Text != null &&
                tBox_FS_LenBit.Text != null && tBox_FS_Data.Text != null &&
                (chBox_FS_InvertTrue.Checked != false || chBox_FS_InvertFalse.Checked != false))
            {
                tBox_Comm.Text = "{\"s.uhfrfid.selfilteroption\":\"" + cBox_FS_TagSelectionMode.Text.ToString() + "\"" +
                    "{\"s.uhfrfid.selfilteraddr\":\"" + tBox_FS_BitAddress.Text.ToString() + "\"" +
                    ",\"s.uhfrfid.selfilterlenbit\":\"" + tBox_FS_LenBit.Text.ToString() + "\"" +
                    ",\"s.uhfrfid.selfilterdata\":\"" + tBox_FS_Data.Text.ToString() + "\"";

                if (chBox_FS_InvertTrue.Checked == true)
                {
                    tBox_Comm.Text += ",\"s.uhfrfid.selfilterinvert\":1";
                }
                else if (chBox_FS_InvertFalse.Checked == true)
                {
                    tBox_Comm.Text += ",\"s.uhfrfid.selfilterinvert\":0";
                }

                tBox_Comm.Text += "}}";
            }
            else
            {
                SetLogText("(Filter Settings) Unable to proceed the data with null, Please try again!");
                tBox_Comm.Text = string.Empty;
            }
        }

        private void btn_ART_Proceed_Click(object sender, EventArgs e)
        {
            if (chBox_ART_EnableTagData.Checked == true && chBox_ART_DisableTagData.Checked == false)
            {
                if (cBox_ART_MemoryBankType.Text != null && tBox_ART_StartAddr.Text != null &&
                    tBox_ART_WordCount.Text != null)
                {
                    tBox_Comm.Text = "{\"s.uhfrfid.enbtagdata\":1" +
                    "{\"s.uhfrfid.tagdatamembank\":\"" + cBox_ART_MemoryBankType.Text.ToString() + "\" " +
                    ",\"s.uhfrfid.tagdatareadaddr\":\"" + tBox_ART_StartAddr.Text.ToString() + "\"" +
                    ",\"s.uhfrfid.tagdatawordcount\":\"" + tBox_ART_WordCount.Text.ToString() + "\"";

                    if (chBox_ART_EnableTagPassword.Checked == true && chBox_ART_DisableTagPassword.Checked == false)
                    {
                        if (tBox_ART_TagPassword.Text != null)
                        {
                            tBox_Comm.Text +=
                            ",\"s.uhfrfid.enbtagpass\":1" +
                            ",\"s.uhfrfid.tagpass\":\"" + tBox_ART_TagPassword.Text.ToString() + "\" }}";
                        }
                        else if (tBox_ART_TagPassword.Text == null)
                        {
                            SetLogText("(Auto Read Tag) Unable to proceed the data with null, Please try again!");
                            tBox_Comm.Text = string.Empty;
                        }

                    }
                    else if (chBox_ART_DisableTagPassword.Checked == true && chBox_ART_EnableTagPassword.Checked == false)
                    {
                        tBox_Comm.Text += ",\"s.uhfrfid.enbtagpass\":0 }}";
                    }
                }
                else if (cBox_ART_MemoryBankType.Text == null || tBox_ART_StartAddr.Text == null ||
                    tBox_ART_WordCount.Text == null)
                {
                    SetLogText("(Auto Read Tag) Unable to proceed the data with null, Please try again!");
                    tBox_Comm.Text = string.Empty;
                }
            }
            else if (chBox_ART_EnableTagData.Checked == false && chBox_ART_DisableTagData.Checked == true)
            {
                tBox_Comm.Text = "{\"s.uhfrfid.enbtagdata\":0}";
            }
        }

        private void btn_TR_Proceed_Click(object sender, EventArgs e)
        {
            if (cBox_TR_ReadOption.Text != null)
            {
                if(cBox_TR_ReadOption.Text == "0 = RESERVED Memory Bank")
                {
                    tBox_Comm.Text = "{\"r.uhfrfid.readtag\":{\"readoption\": 0";
                }
                else if (cBox_TR_ReadOption.Text == "1 = EPC Memory Bank")
                {
                    tBox_Comm.Text = "{\"r.uhfrfid.readtag\":{\"readoption\": 1";
                }
                else if (cBox_TR_ReadOption.Text == "2 = TID Memory Bank")
                {
                    tBox_Comm.Text = "{\"r.uhfrfid.readtag\":{\"readoption\": 2";
                }
                else if (cBox_TR_ReadOption.Text == "3 = USER Memory Bank")
                {
                    tBox_Comm.Text = "{\"r.uhfrfid.readtag\":{\"readoption\": 3";
                }

                if(tBox_TR_Timeout.Text != null && tBox_TR_StartAddr.Text != null && tBox_TR_WordCount.Text != null &&
                    tBox_TR_Password.Text != null)
                {
                    tBox_Comm.Text += ",\"timeout\":\"" + tBox_TR_Timeout.Text.ToString() + "\"" +
                        ",\"readaddr\":\"" + tBox_TR_StartAddr.Text.ToString() + "\"" +
                        ",\"wordcount\":\"" + tBox_TR_WordCount.Text.ToString() + "\"" +
                        ",\"password\":\"" + tBox_TR_Password.Text.ToString() + "\"";

                    if (cBox_TR_TSOption.Text != null)
                    {
                        if (cBox_TR_TSOption.Text == "0 = Disable Tag Selection")
                        {
                            tBox_Comm.Text += ",\"tagselection\":{\"option\": 0";
                        }
                        else if (cBox_TR_TSOption.Text == "1 = Tag Filter with Tag EPC")
                        {
                            tBox_Comm.Text += ",\"tagselection\":{\"option\": 1";
                        }
                        else if (cBox_TR_TSOption.Text == "2 = Tag Filter with TID Memory Bank")
                        {
                            tBox_Comm.Text += ",\"tagselection\":{\"option\": 2";
                        }
                        else if (cBox_TR_TSOption.Text == "3 = Tag Filter with USER Memory Bank")
                        {
                            tBox_Comm.Text += ",\"tagselection\":{\"option\": 3";
                        }
                        else if (cBox_TR_TSOption.Text == "4 = Tag Filter with EPC Memory Bank")
                        {
                            tBox_Comm.Text += ",\"tagselection\":{\"option\": 4";
                        }

                        if (tBox_TR_TSStartAddr.Text != null && tBox_TR_TSLenBit.Text != null && tBox_TR_TSData.Text != null)
                        {
                            tBox_Comm.Text += ",\"addr\":\"" + tBox_TR_TSStartAddr.Text.ToString() + "\"" +
                            ",\"lenbit\":\"" + tBox_TR_TSLenBit.Text.ToString() + "\"" +
                            ",\"data\":\"" + tBox_TR_TSData.Text.ToString() + "\"";

                            if (chBox_TR_TSEnableInvert.Checked == true || chBox_TR_TSDisableInvert.Checked == true)
                            {
                                if (chBox_TR_TSEnableInvert.Checked == true && chBox_TR_TSDisableInvert.Checked == false)
                                {
                                    tBox_Comm.Text += ",\"invert\": 1 }}}";
                                }
                                else if (chBox_TR_TSEnableInvert.Checked == true && chBox_TR_TSDisableInvert.Checked == false)
                                {
                                    tBox_Comm.Text += ",\"invert\": 0 }}}";
                                }
                            }
                            else
                            {
                                tBox_Comm.Text += "}}}";
                            }
                        }
                        else
                        {
                            SetLogText("(Tag Reading) Unable to proceed the data with null, Please try again!");
                            tBox_Comm.Text = string.Empty;
                        }
                    }
                    else if (cBox_TR_TSOption.Text == null)
                    {
                        tBox_Comm.Text += "}}";
                    }
                }
                else
                {
                    SetLogText("(Tag Reading) Unable to proceed the data with null, Please try again!");
                    tBox_Comm.Text = string.Empty;
                }
            }
            else
            {
                SetLogText("(Tag Reading) Unable to proceed the data with null, Please try again!");
                tBox_Comm.Text = string.Empty;
            }
        }

        private void btn_TW_Proceed_Click(object sender, EventArgs e)
        {
            if (cBox_TW_WriteOption.Text != null)
            {
                if (cBox_TW_WriteOption.Text == "0 = RESERVED Memory Bank")
                {
                    tBox_Comm.Text += "{\"r.uhfrfid.writetag\":" +
                    "{\"writeoption\": 0";
                }
                else if (cBox_TW_WriteOption.Text == "1 = EPC Memory Bank")
                {
                    tBox_Comm.Text += "{\"r.uhfrfid.writetag\":" +
                    "{\"writeoption\": 1";
                }
                else if (cBox_TW_WriteOption.Text == "2 = TID Memory Bank")
                {
                    tBox_Comm.Text += "{\"r.uhfrfid.writetag\":" +
                    "{\"writeoption\": 2";
                }
                else if (cBox_TW_WriteOption.Text == "3 = USER Memory Bank")
                {
                    tBox_Comm.Text += "{\"r.uhfrfid.writetag\":" +
                    "{\"writeoption\": 3";
                }
                else if (cBox_TW_WriteOption.Text == "4 = Replace Tag EPC")
                {
                    tBox_Comm.Text += "{\"r.uhfrfid.writetag\":" +
                    "{\"writeoption\": 4";
                }
                else if (cBox_TW_WriteOption.Text == "5 = Lock Tag Memory")
                {
                    tBox_Comm.Text += "{\"r.uhfrfid.writetag\":" +
                    "{\"writeoption\": 5";
                }
                else if (cBox_TW_WriteOption.Text == "6 = Kill Tag")
                {
                    tBox_Comm.Text += "{\"r.uhfrfid.writetag\":" +
                    "{\"writeoption\": 6";
                }

                if (tBox_TW_Timeout.Text != null && tBox_TW_WriteAddress.Text != null && tBox_TW_WriteData.Text != null &&
                    tBox_TW_LockMask.Text != null && tBox_TW_LockAction.Text != null && tBox_TW_Password.Text != null)
                {
                    tBox_Comm.Text += ",\"timeout\":\"" + cBox_TW_WriteOption.Text.ToString() + "\"" +
                        ",\"writeaddr\":\"" + tBox_TW_WriteAddress.Text.ToString() + "\"" +
                        ",\"writedata\":\"" + tBox_TW_WriteData.Text.ToString() + "\"" +
                        ",\"lockmask\":\"" + tBox_TW_LockMask.Text.ToString() + "\"" +
                        ",\"lockaction\":\"" + tBox_TW_LockAction.Text.ToString() + "\"" +
                        ",\"password\":\"" + tBox_TW_Password.Text.ToString() + "\"" ;

                    if (cBox_TW_TSOption.Text != null)
                    {
                        if (cBox_TW_TSOption.Text == "0 = Disable Tag Selection")
                        {
                            tBox_Comm.Text += ",\"tagselection\": {\"option\":0" ;
                        }
                        else if (cBox_TW_TSOption.Text == "1 = Tag Filter with Tag EPC")
                        {
                            tBox_Comm.Text += ",\"tagselection\": {\"option\":1";
                        }
                        else if (cBox_TW_TSOption.Text == "2 = Tag Filter with TID Memory Bank")
                        {
                            tBox_Comm.Text += ",\"tagselection\": {\"option\":2";
                        }
                        else if (cBox_TW_TSOption.Text == "3 = Tag Filter with USER Memory Bank")
                        {
                            tBox_Comm.Text += ",\"tagselection\": {\"option\":3";
                        }
                        else if (cBox_TW_TSOption.Text == "4 = Tag Filter with EPC Memory Bank")
                        {
                            tBox_Comm.Text += ",\"tagselection\": {\"option\":4";
                        }

                        if (tBox_TW_TSStartAddress.Text != null && tBox_TW_TSLenBit.Text != null && tBox_TW_TSData.Text != null &&
                            (chBox_TW_InvertTrue.Checked == true || chBox_TW_InvertFalse.Checked == true))
                        {
                            tBox_Comm.Text += ",\"addr\":\"" + tBox_TW_TSStartAddress.Text.ToString() + "\"" +
                                ",\"lenbit\":\"" + tBox_TW_TSLenBit.Text.ToString() + "\"" +
                                ",\"data\":\"" + tBox_TW_TSData.Text.ToString() + "\"" ;

                            if (chBox_TW_InvertTrue.Checked == true)
                            {
                                tBox_Comm.Text += ",\"invert\":1 }}}";
                            }
                            else if (chBox_TW_InvertFalse.Checked == true)
                            {
                                tBox_Comm.Text += ",\"invert\":0 }}}";
                            }
                        }
                        else
                        {
                            SetLogText("(Tag Writting) Unable to proceed the data with null, Please try again!");
                            tBox_Comm.Text = string.Empty;
                        }

                    }
                    else
                    {
                        tBox_Comm.Text += "}}";
                    }
                }
                else
                {
                    SetLogText("(Tag Writting) Unable to proceed the data with null, Please try again!");
                    tBox_Comm.Text = string.Empty;
                }
            }
            else
            {
                SetLogText("(Tag Writting) Unable to proceed the data with null, Please try again!");
                tBox_Comm.Text = string.Empty;
            }
        }

        private void btn_TS_Proceed_Click(object sender, EventArgs e)
        {
            if (chBox_TS_Enable.Checked == true && chBox_TS_Disable.Checked == false)
            {
                tBox_Comm.Text = "{\"s.envsensor.ratectrl\":1" +
                    ",\"s.envsensor.ratectrlperiod\":" + tBox_TS_RatePeriod.Text + "}}" ;
            }
            else if (chBox_TS_Disable.Checked == true && chBox_TS_Enable.Checked == false)
            {
                tBox_Comm.Text = "{\"s.envsensor.ratectrl\":0}";
            }
        }

        private void chBox_ART_EnableTagData_CheckedChanged(object sender, EventArgs e)
        {
            if (chBox_ART_EnableTagData.Checked == true)
            {
                chBox_ART_DisableTagData.Checked = false;
            }
            else
            {
                chBox_ART_DisableTagData.Checked = true;
            }

            if (chBox_ART_EnableTagData.Checked == true && chBox_ART_DisableTagData.Checked == false)
            {
                cBox_ART_MemoryBankType.Enabled = true;
                tBox_ART_StartAddr.Enabled = true;
                tBox_ART_WordCount.Enabled = true;
                chBox_ART_EnableTagPassword.Enabled = true;
                chBox_ART_DisableTagPassword.Enabled = true;
            }
        }

        private void chBox_ART_DisableTagData_CheckedChanged(object sender, EventArgs e)
        {
            if (chBox_ART_DisableTagData.Checked == true)
            {
                chBox_ART_EnableTagData.Checked = false;
            }
            else
            {
                chBox_ART_EnableTagData.Checked = true;
            }

            if (chBox_ART_EnableTagData.Checked == false && chBox_ART_DisableTagData.Checked == true)
            {
                cBox_ART_MemoryBankType.Text = string.Empty;
                cBox_ART_MemoryBankType.Enabled = false;

                tBox_ART_StartAddr.Text = string.Empty;
                tBox_ART_StartAddr.Enabled = false;

                tBox_ART_WordCount.Text = string.Empty;
                tBox_ART_WordCount.Enabled = false;

                chBox_ART_EnableTagPassword.Checked = false;
                chBox_ART_EnableTagPassword.Enabled = false;

                chBox_ART_DisableTagPassword.Checked = false;
                chBox_ART_DisableTagPassword.Enabled = false;

                tBox_ART_TagPassword.Text = string.Empty;
                tBox_ART_TagPassword.Enabled = false;
            }
        }

        private void chBox_ART_EnableTagPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (chBox_ART_EnableTagPassword.Checked == true)
            {
                chBox_ART_DisableTagPassword.Checked = false;
            }
            else
            {
                chBox_ART_DisableTagPassword.Checked = true;
            }

            if (chBox_ART_EnableTagPassword.Checked == true && chBox_ART_DisableTagPassword.Checked == false)
            {
                tBox_ART_TagPassword.Enabled = true;
            }
        }

        private void chBox_ART_DisableTagPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (chBox_ART_DisableTagPassword.Checked == true)
            {
                chBox_ART_EnableTagPassword.Checked = false;
            }
            else
            {
                chBox_ART_EnableTagPassword.Checked = true;
            }

            if (chBox_ART_EnableTagPassword.Checked == false && chBox_ART_DisableTagPassword.Checked == true)
            {
                tBox_ART_TagPassword.Text = string.Empty;
                tBox_ART_TagPassword.Enabled = false;
            }
        }

        private void cBox_FS_TagSelectionMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cBox_FS_TagSelectionMode.Text != null) 
            {
                tBox_FS_BitAddress.Enabled = true;
                tBox_FS_LenBit.Enabled = true;
                tBox_FS_Data.Enabled = true;
                chBox_FS_InvertTrue.Enabled = true;
                chBox_FS_InvertFalse.Enabled = true;
            }
            else
            {
                tBox_FS_BitAddress.Enabled = false;
                tBox_FS_LenBit.Enabled = false;
                tBox_FS_Data.Enabled = false;
                chBox_FS_InvertTrue.Enabled = false;
                chBox_FS_InvertFalse.Enabled = false;
            }
        }

        private void cBox_TR_ReadOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cBox_TR_ReadOption.Text != null)
            {
                tBox_TR_Timeout.Enabled = true;
                tBox_TR_StartAddr.Enabled = true;
                tBox_TR_WordCount.Enabled = true;
                tBox_TR_Password.Enabled = true;
                cBox_TR_TSOption.Enabled = true;
            }
            else
            {
                tBox_TR_Timeout.Enabled = false;
                tBox_TR_StartAddr.Enabled = false;
                tBox_TR_WordCount.Enabled = false;
                tBox_TR_Password.Enabled = false;
                cBox_TR_TSOption.Enabled = false;
            }
        }

        private void cBox_TR_TSOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cBox_TR_TSOption.Text != null)
            {
                tBox_TR_TSStartAddr.Enabled = true;
                tBox_TR_TSLenBit.Enabled = true;
                tBox_TR_TSData.Enabled = true;
                chBox_TR_TSEnableInvert.Enabled = true;
                chBox_TR_TSDisableInvert.Enabled = true;
            }
            else
            {
                tBox_TR_TSStartAddr.Enabled = false;
                tBox_TR_TSLenBit.Enabled = false;
                tBox_TR_TSData.Enabled = false;
                chBox_TR_TSEnableInvert.Enabled = false;
                chBox_TR_TSDisableInvert.Enabled = false;
            }
        }

        private void cBox_TW_WriteOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cBox_TW_WriteOption.Text != null)
            {
                tBox_TW_Timeout.Enabled = true;
                tBox_TW_WriteAddress.Enabled = true;
                tBox_TW_WriteData.Enabled = true;
                tBox_TW_LockMask.Enabled = true;
                tBox_TW_LockAction.Enabled = true;
                tBox_TW_Password.Enabled = true;
                cBox_TW_TSOption.Enabled = true;
            }
            else
            {
                tBox_TW_Timeout.Enabled = false;
                tBox_TW_WriteAddress.Enabled = false;
                tBox_TW_WriteData.Enabled = false;
                tBox_TW_LockMask.Enabled = false;
                tBox_TW_LockAction.Enabled = false;
                tBox_TW_Password.Enabled = false;
                cBox_TW_TSOption.Enabled = false;
            }
        }

        private void cBox_TW_TSOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cBox_TW_TSOption.Text != null)
            {
                tBox_TW_TSStartAddress.Enabled = true;
                tBox_TW_TSLenBit.Enabled = true;
                tBox_TW_TSData.Enabled = true;
                chBox_TW_InvertTrue.Enabled = true;
                chBox_TW_InvertFalse.Enabled = true;
            }
            else
            {
                tBox_TW_TSStartAddress.Enabled = false;
                tBox_TW_TSLenBit.Enabled = false;
                tBox_TW_TSData.Enabled = false;
                chBox_TW_InvertTrue.Enabled = false;
                chBox_TW_InvertFalse.Enabled = false;
            }
        }

        private void chBox_TR_TSEnableInvert_CheckedChanged(object sender, EventArgs e)
        {
            if (chBox_TR_TSEnableInvert.Checked == true)
            {
                chBox_TR_TSDisableInvert.Checked = false;
            }
            else
            {
                chBox_TR_TSDisableInvert.Checked = true;
            }
        }

        private void chBox_TR_TSDisableInvert_CheckedChanged(object sender, EventArgs e)
        {
            if (chBox_TR_TSDisableInvert.Checked == true)
            {
                chBox_TR_TSEnableInvert.Checked = false;
            }
            else
            {
                chBox_TR_TSEnableInvert.Checked = true;
            }
        }

        private void chBox_TW_InvertTrue_CheckedChanged(object sender, EventArgs e)
        {
            if (chBox_TW_InvertTrue.Checked == true)
            {
                chBox_TW_InvertFalse.Checked = false;
            }
            else
            {
                chBox_TW_InvertFalse.Checked = true;
            }
        }

        private void chBox_TW_InvertFalse_CheckedChanged(object sender, EventArgs e)
        {
            if (chBox_TW_InvertFalse.Checked == true)
            {
                chBox_TW_InvertTrue.Checked = false;
            }
            else
            {
                chBox_TW_InvertTrue.Checked = true;
            }
        }

        private void chBox_TS_Enable_CheckedChanged(object sender, EventArgs e)
        {
            if (chBox_TS_Enable.Checked == true)
            {
                chBox_TS_Disable.Checked = false;
                tBox_TS_RatePeriod.Enabled = true;
            }
            else
            {
                chBox_TS_Disable.Checked = true;
            }
        }

        private void chBox_TS_Disable_CheckedChanged(object sender, EventArgs e)
        {
            if (chBox_TS_Disable.Checked == true)
            {
                chBox_TS_Enable.Checked = false;
                tBox_TS_RatePeriod.Enabled = false;
            }
            else
            {
                chBox_TS_Enable.Checked = true;
            }
        }

        private void btn_Submit_Click(object sender, EventArgs e)
        {
            IndicatorForm.instance.RPCTxt.Text = tBox_Comm.Text.ToString();
            if (tBox_Comm.Text.Contains("r.uhfrfid.readtag") || tBox_Comm.Text.Contains("r.uhfrfid.writetag"))
            {
                IndicatorForm.instance.PubAttb.Enabled = false;
                IndicatorForm.instance.PubRPCReq.Enabled = true;
            }
            else
            {
                IndicatorForm.instance.PubAttb.Enabled = true;
                IndicatorForm.instance.PubRPCReq.Enabled = false;
            }
            
            SetLogText("Successfully Submit RPC Reply!");
        }
    }
}
