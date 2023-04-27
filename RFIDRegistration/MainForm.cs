using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RFIDRegistration.Public;
using System.Threading;
using System.IO;
using Microsoft.Win32;
using System.Data.SqlClient;
using RFIDRegistration.Models;

namespace RFIDRegistration
{

    public partial class MainForm : Form
    {
        public bool adminLogin = false;
        private const string requestAttachDeviceText = "Please Connect To Device";
        public string IuserId = string.Empty, IUsername = string.Empty, IUserType = string.Empty, IUserGroupID = string.Empty, IUserGroupName = string.Empty, IUserGroupDesc = string.Empty,
            IFullname = string.Empty, IEmployeeID = string.Empty, IEmail = string.Empty, IPassword = string.Empty, IMobileNo = string.Empty, ICardID = string.Empty;
        //private const string requestInputUIIText = "Please input UII (Hex)";

        // Address of SQL Server and Database
        SqlConnect sqlConnect = new SqlConnect();
        
        public MainForm()
        {
            InitializeComponent();
            sqlConnect.Connection();
            DemoPublic.number = 1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GetTagList();
            GetDeviceList();
            GetUserList();

            if (adminLogin == false)
            {
                tab_Control_Ad.TabPages.Remove(tab_ModifyUser);
                helloAdminToolStripMenuItem.Text = "Hello User";
            }

            string[] serials = new string[32];

            int hidCnt = PublicFunction.SearchHids(ref serials);

            if (hidCnt == 0)
            {
                return;
            }
            else
            {
                for (int i = 0; i < hidCnt; i++)
                {
                    cbCom.Items.Add(serials[i]);
                }
            }

            if (cbCom.Items.Count != 0)
            {
                cbCom.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Device Not Found");
            }

            DemoPublic.PublicDM = this;
            timer1.Interval = 200;

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            const string connectText = "Connect";
            try
            {
                if (btnConnect.Text == connectText)
                {
                    if (cbCom.Items.Count == 0)
                    {
                        MessageBox.Show("Error : Unable to find Device");
                        return;
                    }
                    string comStr = cbCom.SelectedItem.ToString();
                    if (comStr == string.Empty)
                    {
                        MessageBox.Show("Please Select Serial Port");
                        return;
                    }

                    if (PublicFunction.ConnectRLM(comStr))
                    {
                        MessageBox.Show("Successfully Connected");
                        //AddToShowModifyTag("Successfully Connected");
                        //AddToShowModifyDev("Successfully Connected");

                        btnConnect.Text = "Disconnect";
                        DemoPublic.Enabel_flg = true;
                        //TagListSection.Enabled = true;
                        //TagListSection.ForeColor = Color.Black;


                        tBox_ModTag_NewEPCNo.Enabled = true;
                        tBox_ModTag_NewEPCNo.ForeColor = Color.Black;
                        btn_ModTag_ScanNewTag.Enabled = true;
                        btn_ModTag_ScanNewTag.ForeColor = Color.Black;

                        btn_ModTag_ScanSearchTag.Enabled = true;
                        btn_ModTag_ScanSearchTag.ForeColor = Color.Black;
                    }
                    else
                    {
                        MessageBox.Show("Fail to Connect Device, Please try Again!");
                        //AddToShowModifyTag("Fail to Connect Device, Please try Again!");
                        //AddToShowModifyDev("Fail to Connect Device, Please try Again!");
                    }
                }
                else
                {
                    if (PublicFunction.DisConnectRLM())
                    {
                        btnConnect.Text = connectText;
                        DemoPublic.Enabel_flg = false;
                        //TagListSection.Enabled = false;
                        //TagListSection.ForeColor = Color.Gray;

                        tBox_ModTag_NewEPCNo.Enabled = false;
                        tBox_ModTag_NewEPCNo.ForeColor = Color.Gray;
                        btn_ModTag_ScanNewTag.Enabled = false;
                        btn_ModTag_ScanNewTag.ForeColor = Color.Gray;

                        btn_ModTag_ScanSearchTag.Enabled = false;
                        btn_ModTag_ScanSearchTag.ForeColor = Color.Gray;

                        MessageBox.Show("Disconnected to Device Successfully!");
                        //AddToShowModifyTag("Disconnected to Device Successfully!");
                        //AddToShowModifyDev("Disconnected to Device Successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Fail to Disconnect, Please try Again!");
                        //AddToShowModifyTag("Fail to Disconnect, Please try Again!");
                        //AddToShowModifyDev("Fail to Disconnect, Please try Again!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.Message);
            }
        }

        public void AddToShow(string str)
        {
            //TbShow_Reg.AppendText(DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + " : " + str + Environment.NewLine);
        }

        public void AddToShowModifyTag(string str)
        {
            TbShow_ModifyTag.AppendText(DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + " : " + str + Environment.NewLine);
        }

        public void AddToShowModifyDev(string str)
        {
            TbShow_ModifyDev.AppendText(DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + " : " + str + Environment.NewLine);
        }

        /*private void timer1_Tick(object sender, EventArgs e)
        {
            DemoPublic.TagThread tagThread = new DemoPublic.TagThread(Public.DemoPublic.ShowLoop);
            BeginInvoke(tagThread);
        }*/

        private void ToolSMIConfig_Click(object sender, EventArgs e)
        {
            if (!DemoPublic.Enabel_flg)
            {
                MessageBox.Show(requestAttachDeviceText);
                return;
            }

            
        }

        private void ToolSMIVersion_Click(object sender, EventArgs e)
        {
            if (!DemoPublic.Enabel_flg)
            {
                MessageBox.Show(requestAttachDeviceText);
                return;
            }

            string infoStr = string.Empty;
            byte[] bSerial = new byte[6];
            byte[] bVersion = new byte[3];
            if (PublicFunction.GetVersion(bSerial, bVersion))
            {
                infoStr += "Hardware Serial Number: " + DemoPublic.BytesToHexString(bSerial, 6) + "\n";
                infoStr += "Firmware Version: " + bVersion[0].ToString() + "." + bVersion[1].ToString() + "." + bVersion[2].ToString() + "\n";
            }
            else
            {
                infoStr += "Failed to read hardware information \n";
            }

            MessageBox.Show(infoStr, "Hardware Information");

        }

        private void ToolSMIExit_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Do you want to Exit?",
                "Warning",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1);

            if (dr == DialogResult.Yes)
            {
                if (DemoPublic.Enabel_flg)
                {
                    PublicFunction.Stop();
                }
                Application.Exit();
            }
        }

        public void DuplicateMessage(string DuplicateDataType, string DuplicateData)
        {
            MessageBox.Show(DuplicateDataType + " Has Been Duplicated!");
            AddToShow(DuplicateDataType + " Has Been Duplicated! ::" + DuplicateData);
        }

        public void ModifyTagTextRefresh()
        {
            tBox_ModTag_UserReg.Text = string.Empty;
            tBox_ModTag_EmpName.Text = string.Empty;

            tBox_ModTag_OldSN.Text = string.Empty;
            tBox_ModTag_NewSN.Text = string.Empty;

            tBox_ModTag_OldPN.Text = string.Empty;
            tBox_ModTag_NewPN.Text = string.Empty;

            tBox_ModTag_OldEPCNo.Text = string.Empty;
            tBox_ModTag_NewEPCNo.Text = string.Empty;
            tBox_ModTag_MoreDesc.Text = string.Empty;

            tBox_ModTag_SearchEPCNo.Text = string.Empty;

        }

        public void ModifyDevTextRefresh()
        {
            tBox_ModDev_UserReg.Text = string.Empty;
            tBox_ModDev_EmpName.Text = string.Empty;
            tBox_ModDev_OldDevID.Text = string.Empty;
            tBox_ModDev_NewDevID.Text = string.Empty;
            tBox_ModDev_OldModel.Text = string.Empty;
            tBox_ModDev_NewModel.Text = string.Empty;
            tBox_ModDev_OldProductName.Text = string.Empty;
            tBox_ModDev_NewProductName.Text = string.Empty;
            tBox_ModDev_OldCommType.Text = string.Empty;
            cBox_ModDev_NewCommType.Text = string.Empty;
            tBox_ModDev_OldDevIP.Text = string.Empty;
            tBox_ModDev_NewDevIP.Text = string.Empty;
            tBox_ModDev_MoreDesc.Text = string.Empty;

            tBox_ModDev_SearchDevID.Text = string.Empty;
        }


        // Get Tag List from Database
        public void GetTagList()
        {
            sqlConnect.con.Open();
            if (sqlConnect.con.State == System.Data.ConnectionState.Open)
            {
                SqlCommand sqlCommand = new SqlCommand("Select_RegTagList", sqlConnect.con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                sqlCommand.Parameters.AddWithValue("@Option", "GetFullTagList");
                sqlCommand.Parameters.AddWithValue("@TAG_epc_no", string.Empty);
                sqlCommand.Parameters.AddWithValue("@SERIAL_no", string.Empty);
                sqlCommand.Parameters.AddWithValue("@PART_no", string.Empty);

                SqlDataAdapter dataAdapter_GetTagList = new SqlDataAdapter(sqlCommand);
                DataTable dt = new DataTable();
                dataAdapter_GetTagList.Fill(dt);
                dataGridView_TagList.DataSource = dt;
            }
            sqlConnect.con.Close();
        }

        public void GetDeviceList()
        {
            sqlConnect.con.Open();
            if (sqlConnect.con.State == System.Data.ConnectionState.Open)
            {
                SqlCommand sqlCommand = new SqlCommand("Select_RegDeviceList", sqlConnect.con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                sqlCommand.Parameters.AddWithValue("@Option", "GetFullDeviceList");
                sqlCommand.Parameters.AddWithValue("@Dev_ID", string.Empty);
                sqlCommand.Parameters.AddWithValue("@Model_Name", string.Empty);
                sqlCommand.Parameters.AddWithValue("@Product_Name", string.Empty);

                SqlDataAdapter dataAdapter_GetDeviceList = new SqlDataAdapter(sqlCommand);
                DataTable dt2 = new DataTable();
                dataAdapter_GetDeviceList.Fill(dt2);
                dataGridView_DevList.DataSource = dt2;
            }
            sqlConnect.con.Close();
        }

        public void GetUserList()
        {
            sqlConnect.con.Open();
            if (sqlConnect.con.State == System.Data.ConnectionState.Open)
            {
                SqlCommand sqlCommand = new SqlCommand("Select_UsersTB", sqlConnect.con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                sqlCommand.Parameters.AddWithValue("@Option", "GetFullUsersList");
                sqlCommand.Parameters.AddWithValue("@Username", string.Empty);
                sqlCommand.Parameters.AddWithValue("@Password", string.Empty);
                sqlCommand.Parameters.AddWithValue("@Fullname", string.Empty);
                sqlCommand.Parameters.AddWithValue("@User_Type", string.Empty);
                sqlCommand.Parameters.AddWithValue("@Employee_ID", string.Empty);

                SqlDataAdapter dataAdapter_GetUserList = new SqlDataAdapter(sqlCommand);
                DataTable dt2 = new DataTable();
                dataAdapter_GetUserList.Fill(dt2);
                dataGridView_UserList.DataSource = dt2;
            }
            sqlConnect.con.Close();
        }


        private void btn_ModTag_ScanNewTag_Click(object sender, EventArgs e)
        {
            if (!DemoPublic.Enabel_flg)
            {
                MessageBox.Show(requestAttachDeviceText);
                return;
            }

            byte[] uiiLen = new byte[1];
            byte[] uii = new byte[64];

            if (!PublicFunction.InventorySingle(uiiLen, uii))
            {
                AddToShowModifyTag("Scan Failed, Please try Again!");
                return;
            }
            string uiiStr = DemoPublic.BytesToHexString(uii, uiiLen[0]);
            AddToShowModifyTag("Scan Successful, UII : " + uiiStr);

            DemoPublic.tagInfo.uii = uiiStr;
            tBox_ModTag_NewEPCNo.Text = uiiStr;
        }

        private void btn_ModTag_UpdateRecords_Click(object sender, EventArgs e)
        {
            // Update Tag Records
            sqlConnect.con.Open();

            string SerialNo, PartNo, EPCTagNo;

            if (tBox_ModTag_NewSN.Text == string.Empty)
            {
                SerialNo = tBox_ModTag_OldSN.Text;
            }
            else
            {
                SerialNo = tBox_ModTag_NewSN.Text;
            }

            if (tBox_ModTag_NewPN.Text == string.Empty)
            {
                PartNo = tBox_ModTag_OldPN.Text;
            }
            else
            {
                PartNo = tBox_ModTag_NewPN.Text;
            }

            if (tBox_ModTag_NewEPCNo.Text == string.Empty)
            {
                EPCTagNo = tBox_ModTag_OldEPCNo.Text;
            }
            else
            {
                EPCTagNo = tBox_ModTag_NewEPCNo.Text;
            }

            if (tBox_ModTag_UserReg.Text == string.Empty)
            {
                MessageBox.Show("User Registered Cannot Be Empty!");
                AddToShowModifyTag("User Registered Has A NULL Value, Please Try Again!");
            }
            else if(tBox_ModTag_EmpName.Text == string.Empty)
            {
                MessageBox.Show("Employee Name Cannot Be Empty!");
                AddToShowModifyTag("Employee Name Has A NULL Value, Please Try Again!");
            }
            else
            {
                SqlCommand c = new SqlCommand("exec Update_TagList" +
               "'" + tBox_ModTag_EmpName.Text + "'," +
               "'" + SerialNo + "'," +
               "'" + PartNo + "'," +
               "'" + EPCTagNo + "'," +
               "'" + tBox_ModTag_OldEPCNo.Text + "'", sqlConnect.con);

                SqlCommand c2 = new SqlCommand("exec Insert_MaintenanceTagList '" + tBox_ModTag_UserReg.Text + "', '" + tBox_ModTag_EmpName.Text + "'," +
                    " '" + tBox_ModTag_OldSN.Text + "', '" + tBox_ModTag_NewSN.Text + "', '" + tBox_ModTag_OldPN.Text + "', '" + tBox_ModTag_NewPN.Text + "'," +
                    " '" + tBox_ModTag_OldEPCNo.Text + "', '" + tBox_ModTag_NewEPCNo.Text + "', '" + tBox_ModTag_MoreDesc.Text + "' ", sqlConnect.con);

                c.ExecuteNonQuery();
                c2.ExecuteNonQuery();
                AddToShowModifyTag("Tag Successfully Updated!");

                GetTagList();
            }
            ModifyTagTextRefresh();
            sqlConnect.con.Close();
        }

        private void btn_ModTag_DeleteRecords_Click(object sender, EventArgs e)
        {
            // Delete
            if (MessageBox.Show("Are you sure to delete?", "Delete Document", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                sqlConnect.con.Open();
                SqlCommand c = new SqlCommand("exec Delete_TagList '" + tBox_ModTag_OldEPCNo.Text + "'", sqlConnect.con);
                c.ExecuteNonQuery();
                AddToShowModifyTag("Tag Successfully Deleted!");
                GetTagList();
                sqlConnect.con.Close();
            }
        }

        public void btn_ModTag_Search_Click(object sender, EventArgs e)
        {
            // Search
            sqlConnect.con.Open();

            if (tBox_ModTag_SearchEPCNo.Text != string.Empty)
            { 
                SqlCommand cmd = new SqlCommand("Select_RegTagList", sqlConnect.con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Option", "GetListSearching");
                cmd.Parameters.AddWithValue("@TAG_epc_no", tBox_ModTag_SearchEPCNo.Text);
                cmd.Parameters.AddWithValue("@SERIAL_no", tBox_ModTag_SearchEPCNo.Text);
                cmd.Parameters.AddWithValue("@PART_no", tBox_ModTag_SearchEPCNo.Text);
                SqlDataAdapter sd = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                sd.Fill(dt);
                dataGridView_TagList.DataSource = dt;
                AddToShowModifyTag("Data Founded Under Database");
            }
            else if (tBox_ModTag_SearchEPCNo.Text == string.Empty)
            {
                SqlCommand cmd = new SqlCommand("Select_RegTagList", sqlConnect.con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Option", "GetFullTagList");
                cmd.Parameters.AddWithValue("@TAG_epc_no", string.Empty);
                cmd.Parameters.AddWithValue("@SERIAL_no", string.Empty);
                cmd.Parameters.AddWithValue("@PART_no", string.Empty);
                SqlDataAdapter sd = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                sd.Fill(dt);
                dataGridView_TagList.DataSource = dt;
                AddToShowModifyTag("Data Founded Under Database");
            }
            /*else if (chBox_ModTag_SearchTag.Checked == false && chBox_ModTag_SearchSN.Checked && chBox_ModTag_SearchPN.Checked == false)
            {
                if (tBox_ModTag_SearchSN.Text != "")
                {
                    SqlCommand cmd = new SqlCommand("Select_RegTagList", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Option", "GetListSerialNo");
                    cmd.Parameters.AddWithValue("@TAG_epc_no", "");
                    cmd.Parameters.AddWithValue("@SERIAL_no", tBox_ModTag_SearchSN.Text);
                    cmd.Parameters.AddWithValue("@PART_no", "");
                    SqlDataAdapter sd = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable();
                    sd.Fill(dt);
                    dataGridView_TagList.DataSource = dt;
                    AddToShowModifyTag("Data Founded Under Database with Serial No!");
                }
                else if (tBox_ModTag_SearchSN.Text == "")
                {
                    MessageBox.Show("Serial No. is Empty! Please Fill In Something.");
                    AddToShowModifyTag("Unable to accept NULL value on Serial No!");
                }
            }
            else if (chBox_ModTag_SearchSN.Checked == false && chBox_ModTag_SearchTag.Checked == false && chBox_ModTag_SearchPN.Checked)
            {
                if (tBox_ModTag_SearchPN.Text != "")
                {
                    SqlCommand cmd = new SqlCommand("Select_RegTagList", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Option", "GetListPartNo");
                    cmd.Parameters.AddWithValue("@TAG_epc_no", "");
                    cmd.Parameters.AddWithValue("@SERIAL_no", "");
                    cmd.Parameters.AddWithValue("@PART_no", tBox_ModTag_SearchPN.Text);
                    SqlDataAdapter sd = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable();
                    sd.Fill(dt);
                    dataGridView_TagList.DataSource = dt;
                    AddToShowModifyTag("Data Founded Under Database with Part No!");
                }
                else if (tBox_ModTag_SearchPN.Text == "")
                {
                    MessageBox.Show("Part No. is Empty! Please Fill In Something.");
                    AddToShowModifyTag("Unable to accept NULL value on Part No!");
                }
            }
            else if (chBox_ModTag_SearchTag.Checked && chBox_ModTag_SearchSN.Checked && chBox_ModTag_SearchPN.Checked == false)
            {
                if (tBox_ModTag_SearchEPCNo.Text != "" && tBox_ModTag_SearchSN.Text != "")
                {
                    SqlCommand cmd = new SqlCommand("Select_RegTagList", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Option", "GetListTagEPCSerialNo");
                    cmd.Parameters.AddWithValue("@TAG_epc_no", tBox_ModTag_SearchEPCNo.Text);
                    cmd.Parameters.AddWithValue("@SERIAL_no", tBox_ModTag_SearchSN.Text);
                    cmd.Parameters.AddWithValue("@PART_no", "");
                    SqlDataAdapter sd = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable();
                    sd.Fill(dt);
                    dataGridView_TagList.DataSource = dt;
                    AddToShowModifyTag("Data Founded Under Database with combination of EPC No & Serial No!");
                }
                else if (tBox_ModTag_SearchEPCNo.Text == "")
                {
                    MessageBox.Show("EPC No. is Empty! Please Fill In Something.");
                    AddToShowModifyTag("Unable to accept NULL value on EPC No!");
                }
                else if (tBox_ModTag_SearchSN.Text == "")
                {
                    MessageBox.Show("Serial No. is Empty! Please Fill In Something.");
                    AddToShowModifyTag("Unable to accept NULL value on Serial No!");
                }
                else if (tBox_ModTag_SearchEPCNo.Text == "" && tBox_ModTag_SearchSN.Text == "")
                {
                    MessageBox.Show("Both Value is Empty! Please Fill In Something.");
                    AddToShowModifyTag("Unable to accept NULL value on EPC No & Serial No!");
                }
            }
            else if (chBox_ModTag_SearchTag.Checked && chBox_ModTag_SearchPN.Checked && chBox_ModTag_SearchSN.Checked == false)
            {
                if (tBox_ModTag_SearchEPCNo.Text != "" && tBox_ModTag_SearchPN.Text != "")
                {
                    SqlCommand cmd = new SqlCommand("Select_RegTagList", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Option", "GetListTagEPCPartNo");
                    cmd.Parameters.AddWithValue("@TAG_epc_no", tBox_ModTag_SearchEPCNo.Text);
                    cmd.Parameters.AddWithValue("@SERIAL_no", "");
                    cmd.Parameters.AddWithValue("@PART_no", tBox_ModTag_SearchPN.Text);
                    SqlDataAdapter sd = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable();
                    sd.Fill(dt);
                    dataGridView_TagList.DataSource = dt;
                    AddToShowModifyTag("Data Founded Under Database with combination of EPC No & Part No!");
                }
                else if (tBox_ModTag_SearchEPCNo.Text == "")
                {
                    MessageBox.Show("EPC No. is Empty! Please Fill In Something.");
                    AddToShowModifyTag("Unable to accept NULL value on EPC No!");
                }
                else if (tBox_ModTag_SearchPN.Text == "")
                {
                    MessageBox.Show("Part No. is Empty! Please Fill In Something.");
                    AddToShowModifyTag("Unable to accept NULL value on Part No!");
                }
                else if (tBox_ModTag_SearchEPCNo.Text == "" && tBox_ModTag_SearchPN.Text == "")
                {
                    MessageBox.Show("Both Value is Empty! Please Fill In Something.");
                    AddToShowModifyTag("Unable to accept NULL value on EPC No & Part No!");
                }
            }
            else if (chBox_ModTag_SearchSN.Checked && chBox_ModTag_SearchPN.Checked && chBox_ModTag_SearchTag.Checked == false)
            {
                if (tBox_ModTag_SearchSN.Text != "" && tBox_ModTag_SearchPN.Text != "")
                {
                    SqlCommand cmd = new SqlCommand("Select_RegTagList", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Option", "GetListSerialNoPartNo");
                    cmd.Parameters.AddWithValue("@TAG_epc_no", "");
                    cmd.Parameters.AddWithValue("@SERIAL_no", tBox_ModTag_SearchSN.Text);
                    cmd.Parameters.AddWithValue("@PART_no", tBox_ModTag_SearchPN.Text);
                    SqlDataAdapter sd = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable();
                    sd.Fill(dt);
                    dataGridView_TagList.DataSource = dt;
                    AddToShowModifyTag("Data Founded Under Database with combination of Serial No & Part No!");
                }
                else if (tBox_ModTag_SearchSN.Text == "")
                {
                    MessageBox.Show("Serial No. is Empty! Please Fill In Something.");
                    AddToShowModifyTag("Unable to accept NULL value on Serial No!");
                }
                else if (tBox_ModTag_SearchPN.Text == "")
                {
                    MessageBox.Show("Part No. is Empty! Please Fill In Something.");
                    AddToShowModifyTag("Unable to accept NULL value on Part No!");
                }
                else if (tBox_ModTag_SearchSN.Text == "" && tBox_ModTag_SearchPN.Text == "")
                {
                    MessageBox.Show("Both Value is Empty! Please Fill In Something.");
                    AddToShowModifyTag("Unable to accept NULL value on Serial No & Part No!");
                }
            }
            else if (chBox_ModTag_SearchTag.Checked && chBox_ModTag_SearchSN.Checked && chBox_ModTag_SearchPN.Checked)
            {
                if (tBox_ModTag_SearchEPCNo.Text != "" && tBox_ModTag_SearchSN.Text != "" && tBox_ModTag_SearchPN.Text != "")
                {
                    SqlCommand cmd = new SqlCommand("Select_RegTagList", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Option", "GetListFullCombo");
                    cmd.Parameters.AddWithValue("@TAG_epc_no", tBox_ModTag_SearchEPCNo.Text);
                    cmd.Parameters.AddWithValue("@SERIAL_no", tBox_ModTag_SearchSN.Text);
                    cmd.Parameters.AddWithValue("@PART_no", tBox_ModTag_SearchPN.Text);
                    SqlDataAdapter sd = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable();
                    sd.Fill(dt);
                    dataGridView_TagList.DataSource = dt;
                    AddToShowModifyTag("Data Founded Under Database with All Combination!");
                }
                else if (tBox_ModTag_SearchEPCNo.Text == "")
                {
                    MessageBox.Show("EPC No. is Empty! Please Fill In Something.");
                    AddToShowModifyTag("Unable to accept NULL value on EPC No!");
                }
                else if (tBox_ModTag_SearchSN.Text == "")
                {
                    MessageBox.Show("Serial No. is Empty! Please Fill In Something.");
                    AddToShowModifyTag("Unable to accept NULL value on Serial No!");
                }
                else if (tBox_ModTag_SearchPN.Text == "")
                {
                    MessageBox.Show("Part No. is Empty! Please Fill In Something.");
                    AddToShowModifyTag("Unable to accept NULL value on Part No!");
                }
                else if (tBox_ModTag_SearchEPCNo.Text == "" && tBox_ModTag_SearchSN.Text == "" && tBox_ModTag_SearchPN.Text == "")
                {
                    MessageBox.Show("All Value is Empty! Please Fill In Something.");
                    AddToShowModifyTag("Unable to accept NULL on all value!");
                }
            }
            else
            {
                MessageBox.Show("No CheckBox is select, Please Check before Proceed!");
                AddToShowModifyTag("Unable to run Search without check any of the box!");
            }
            */


            sqlConnect.con.Close();
        }

        private void btn_ModTag_Refresh_Click(object sender, EventArgs e)
        {
            GetTagList();
            AddToShowModifyTag("Datatable Has Been Refreshed!");
        }

        private void tBox_ModTag_Search_TextChanged(object sender, EventArgs e)
        {
            sqlConnect.con.Open();
            if (tBox_ModTag_SearchEPCNo.Text != string.Empty)
            {
                /*SqlCommand cmd = new SqlCommand("SELECT * FROM registration_tag_tb WHERE tag_epc_no = @tag_epc_no", con);
                cmd.Parameters.AddWithValue("@tag_epc_no", tBox_ModTag_SearchEPCNo.Text);
                SqlDataReader da = cmd.ExecuteReader();*/
                SqlCommand cmd = new SqlCommand("Select_RegTagList", sqlConnect.con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Option", "GetListTagEPC");
                cmd.Parameters.AddWithValue("@TAG_epc_no", tBox_ModTag_SearchEPCNo.Text);
                cmd.Parameters.AddWithValue("@SERIAL_no", string.Empty);
                cmd.Parameters.AddWithValue("@PART_no", string.Empty);
                SqlDataReader da = cmd.ExecuteReader();
                
                while (da.Read())
                {
                    tBox_ModTag_UserReg.Text = da.GetValue(1).ToString();
                    tBox_ModTag_OldSN.Text = da.GetValue(2).ToString();
                    tBox_ModTag_OldPN.Text = da.GetValue(3).ToString();
                    tBox_ModTag_OldEPCNo.Text = da.GetValue(4).ToString();
                }
            }
            sqlConnect.con.Close();
        }

        private void btn_ModTag_ScanSearchTag_Click(object sender, EventArgs e)
        {
            if (!DemoPublic.Enabel_flg)
            {
                MessageBox.Show(requestAttachDeviceText);
                return;
            }

            byte[] uiiLen = new byte[1];
            byte[] uii = new byte[64];

            if (!PublicFunction.InventorySingle(uiiLen, uii))
            {
                AddToShowModifyTag("Scan Failed, Please try Again!");
                return;
            }
            string uiiStr = DemoPublic.BytesToHexString(uii, uiiLen[0]);
            AddToShowModifyTag("Scan Successful, UII : " + uiiStr);

            DemoPublic.tagInfo.uii = uiiStr;
            tBox_ModTag_SearchEPCNo.Text = uiiStr;
        }

        private void btn_ModTag_ClearRecords_Click(object sender, EventArgs e)
        {
            ModifyTagTextRefresh();
            AddToShowModifyTag("Tag Modification List Text Cleared!");
        }

        private void dataGridView_TagList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dataGridView_TagList.Rows[e.RowIndex];
                tBox_ModTag_UserReg.Text = row.Cells[2].Value.ToString();
                tBox_ModTag_OldSN.Text = row.Cells[3].Value.ToString();
                tBox_ModTag_OldPN.Text = row.Cells[4].Value.ToString();
                tBox_ModTag_OldEPCNo.Text = row.Cells[5].Value.ToString();
            }
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login login = new Login();
            login.ShowDialog();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
        }

        private void registerUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegistrationUser registrationUser = new RegistrationUser();
            registrationUser.ShowDialog();
        }

        private void btn_ModDev_UpdateRecords_Click(object sender, EventArgs e)
        {
            // Update Tag Records
            sqlConnect.con.Open();

            string DeviceID, ModelName, ProductName, CommType, DeviceIPAddr;

            if (tBox_ModDev_NewDevID.Text == string.Empty)
            {
                DeviceID = tBox_ModDev_OldDevID.Text;
            }
            else
            {
                DeviceID = tBox_ModDev_NewDevID.Text;
            }

            if (tBox_ModDev_NewModel.Text == string.Empty)
            {
                ModelName = tBox_ModDev_OldModel.Text;
            }
            else
            {
                ModelName = tBox_ModDev_NewModel.Text;
            }

            if (tBox_ModDev_NewProductName.Text == string.Empty)
            {
                ProductName = tBox_ModDev_OldProductName.Text;
            }
            else
            {
                ProductName = tBox_ModDev_NewProductName.Text;
            }

            if (cBox_ModDev_NewCommType.Text == string.Empty)
            {
                CommType = tBox_ModDev_OldCommType.Text;
            }
            else
            {
                CommType = cBox_ModDev_NewCommType.Text;
            }

            if (tBox_ModDev_NewDevIP.Text == string.Empty)
            {
                DeviceIPAddr = tBox_ModDev_OldDevIP.Text;
            }
            else
            {
                DeviceIPAddr = tBox_ModDev_NewDevIP.Text;
            }

            if (tBox_ModDev_UserReg.Text == string.Empty)
            {
                MessageBox.Show("User Registered Cannot Be Empty!");
                AddToShowModifyDev("User Registered Has A NULL Value, Please Try Again!");
            }
            else if (tBox_ModDev_EmpName.Text == string.Empty)
            {
                MessageBox.Show("Employee Name Cannot Be Empty!");
                AddToShowModifyDev("Employee Name Has A NULL Value, Please Try Again!");
            }
            else
            {
                SqlCommand c = new SqlCommand("exec Update_DevList" +
               "'" + tBox_ModDev_EmpName.Text + "'," +
               "'" + DeviceID + "'," +
               "'" + ModelName + "'," +
               "'" + ProductName + "'," +
               "'" + CommType + "'," +
               "'" + DeviceIPAddr + "'," +
               "'" + tBox_ModDev_OldDevID.Text + "'", sqlConnect.con);

                SqlCommand c2 = new SqlCommand("exec Insert_MaintenanceDevList '" + tBox_ModDev_UserReg.Text + "', '" + tBox_ModDev_EmpName.Text + "'," +
                    " '" + tBox_ModDev_OldDevID.Text + "', '" + tBox_ModDev_NewDevID.Text + "', '" + tBox_ModDev_OldModel.Text + "', '" + tBox_ModDev_NewModel.Text + "'," +
                    " '" + tBox_ModDev_OldProductName.Text + "', '" + tBox_ModDev_NewProductName.Text + "', '" + tBox_ModDev_OldCommType.Text + "', '" + cBox_ModDev_NewCommType.Text + "'," +
                    " '" + tBox_ModDev_OldDevIP.Text + "', '" + tBox_ModDev_NewDevIP.Text + "', '" + tBox_ModDev_MoreDesc.Text + "' ", sqlConnect.con);

                c.ExecuteNonQuery();
                c2.ExecuteNonQuery();
                AddToShowModifyDev("Device Successfully Updated!");

                GetDeviceList();
            }
            ModifyDevTextRefresh();
            sqlConnect.con.Close();
        }

        private void tBox_ModUser_Search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btn_ModUser_Search_Click(this, new EventArgs());
            }
        }

        private void toolStripMenuItemIndicator_Click(object sender, EventArgs e)
        {
            IndicatorForm indicatorForm1 = new IndicatorForm
            {
                indicatorChoosen = true
            };
            indicatorForm1.btnRPCConfig_03.Enabled = false;
            indicatorForm1.Show();
        }

        private void toolStripMenuItemRFIDReader_Click(object sender, EventArgs e)
        {
            IndicatorForm indicatorForm2 = new IndicatorForm
            {
                indicatorChoosen = false
            };
            indicatorForm2.Show();
        }

        private void newPartNumberPNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddPartNo addPartNo = new AddPartNo();
            addPartNo.Show();
        }

        private void newTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegistrationTag registrationTag = new RegistrationTag();
            registrationTag.ShowDialog();
        }

        private void newWIRIO3DeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegistrationDevice registrationDevice = new RegistrationDevice();
            registrationDevice.ShowDialog();
        }

        private void newSerialNumberSNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSerialNo addSerialNo = new AddSerialNo();
            addSerialNo.ShowDialog();
        }

        private void btn_ModDev_DeleteRecords_Click(object sender, EventArgs e)
        {
            // Delete
            if (MessageBox.Show("Are you sure to delete?", "Delete Document", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                sqlConnect.con.Open();
                SqlCommand c = new SqlCommand("exec Delete_DevList '" + tBox_ModDev_OldDevID.Text + "'", sqlConnect.con);
                c.ExecuteNonQuery();
                AddToShowModifyDev("Device Successfully Deleted!");
                GetDeviceList();
                sqlConnect.con.Close();
            }
        }

        private void btn_ModDev_ClearRecords_Click(object sender, EventArgs e)
        {
            ModifyDevTextRefresh();
            AddToShowModifyDev("Device Modification List Text Cleared!");
        }

        private void btn_ModDev_Refresh_Click(object sender, EventArgs e)
        {
            GetDeviceList();
            AddToShowModifyDev("Datatable Has Been Refreshed!");
        }

        private void btn_ModDev_SearchDev_Click(object sender, EventArgs e)
        {
            // Search
            sqlConnect.con.Open();

            if (tBox_ModDev_SearchDevID.Text != string.Empty)
            {

                SqlCommand cmd = new SqlCommand("Select_RegDeviceList", sqlConnect.con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Option", "GetListSearchingDev");
                cmd.Parameters.AddWithValue("@Dev_ID", tBox_ModDev_SearchDevID.Text);
                cmd.Parameters.AddWithValue("@Model_Name", tBox_ModDev_SearchDevID.Text);
                cmd.Parameters.AddWithValue("@Product_Name", tBox_ModDev_SearchDevID.Text);
                SqlDataAdapter sd = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                sd.Fill(dt);
                dataGridView_DevList.DataSource = dt;
                AddToShowModifyDev("Data Founded Under Database!");
            }
            else if (tBox_ModDev_SearchDevID.Text == string.Empty)
            {
                SqlCommand cmd = new SqlCommand("Select_RegDeviceList", sqlConnect.con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Option", "GetFullDeviceList");
                cmd.Parameters.AddWithValue("@Dev_ID", string.Empty);
                cmd.Parameters.AddWithValue("@Model_Name", string.Empty);
                cmd.Parameters.AddWithValue("@Product_Name", string.Empty);
                SqlDataAdapter sd = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                sd.Fill(dt);
                dataGridView_DevList.DataSource = dt;
                AddToShowModifyDev("Data Founded Under Database!");
            }

            /*if (chBox_ModDev_SearchDevID.Checked && chBox_ModDev_SearchModel.Checked == false && chBox_ModDev_SearchProductName.Checked == false)
            {
                if (tBox_ModDev_SearchDevID.Text != "")
                {

                    SqlCommand cmd = new SqlCommand("Select_RegDeviceList", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Option", "GetListDeviceID");
                    cmd.Parameters.AddWithValue("@Dev_ID", tBox_ModDev_SearchDevID.Text);
                    cmd.Parameters.AddWithValue("@Model_Name", "");
                    cmd.Parameters.AddWithValue("@Product_Name", "");
                    SqlDataAdapter sd = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable();
                    sd.Fill(dt);
                    dataGridView_TagList.DataSource = dt;
                    AddToShowModifyDev("Data Founded Under Database with Device ID!");
                }
                else if (tBox_ModDev_SearchDevID.Text == "")
                {
                    MessageBox.Show("Device ID is Empty! Please Fill In Something.");
                    AddToShowModifyDev("Unable to accept NULL value on Device ID!");
                }

            }
            else if (chBox_ModDev_SearchDevID.Checked == false && chBox_ModDev_SearchModel.Checked && chBox_ModDev_SearchProductName.Checked == false)
            {
                if (tBox_ModDev_SearchModel.Text != "")
                {
                    SqlCommand cmd = new SqlCommand("Select_RegDeviceList", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Option", "GetListModelName");
                    cmd.Parameters.AddWithValue("@Dev_ID", "");
                    cmd.Parameters.AddWithValue("@Model_Name", tBox_ModDev_SearchModel.Text);
                    cmd.Parameters.AddWithValue("@Product_Name", "");
                    SqlDataAdapter sd = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable();
                    sd.Fill(dt);
                    dataGridView_TagList.DataSource = dt;
                    AddToShowModifyDev("Data Founded Under Database with Model Name!");
                }
                else if (tBox_ModDev_SearchModel.Text == "")
                {
                    MessageBox.Show("Model Name is Empty! Please Fill In Something.");
                    AddToShowModifyDev("Unable to accept NULL value on Model Name!");
                }
            }
            else if (chBox_ModDev_SearchDevID.Checked == false && chBox_ModDev_SearchModel.Checked == false && chBox_ModDev_SearchProductName.Checked)
            {
                if (tBox_ModDev_SearchProductName.Text != "")
                {
                    SqlCommand cmd = new SqlCommand("Select_RegDeviceList", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Option", "GetListProductName");
                    cmd.Parameters.AddWithValue("@Dev_ID", "");
                    cmd.Parameters.AddWithValue("@Model_Name", "");
                    cmd.Parameters.AddWithValue("@Product_Name", tBox_ModDev_SearchProductName.Text);
                    SqlDataAdapter sd = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable();
                    sd.Fill(dt);
                    dataGridView_TagList.DataSource = dt;
                    AddToShowModifyDev("Data Founded Under Database with Product Name!");
                }
                else if (tBox_ModDev_SearchProductName.Text == "")
                {
                    MessageBox.Show("Product Name is Empty! Please Fill In Something.");
                    AddToShowModifyDev("Unable to accept NULL value on Product Name!");
                }
                // HERE WOWOKOK
            }
            else if (chBox_ModDev_SearchDevID.Checked && chBox_ModDev_SearchModel.Checked && chBox_ModDev_SearchProductName.Checked == false)
            {
                if (tBox_ModDev_SearchDevID.Text != "" && tBox_ModDev_SearchModel.Text != "")
                {
                    SqlCommand cmd = new SqlCommand("Select_RegDeviceList", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Option", "GetListDeviceIDModelName");
                    cmd.Parameters.AddWithValue("@Dev_ID", tBox_ModDev_SearchDevID.Text);
                    cmd.Parameters.AddWithValue("@Model_Name", tBox_ModDev_SearchModel.Text);
                    cmd.Parameters.AddWithValue("@Product_Name", "");
                    SqlDataAdapter sd = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable();
                    sd.Fill(dt);
                    dataGridView_TagList.DataSource = dt;
                    AddToShowModifyDev("Data Founded Under Database with combination of Device ID & Model Name!");
                }
                else if (tBox_ModDev_SearchDevID.Text == "")
                {
                    MessageBox.Show("Device ID is Empty! Please Fill In Something.");
                    AddToShowModifyDev("Unable to accept NULL value on Device ID!");
                }
                else if (tBox_ModDev_SearchModel.Text == "")
                {
                    MessageBox.Show("Model Name is Empty! Please Fill In Something.");
                    AddToShowModifyDev("Unable to accept NULL value on Model Name!");
                }
                else if (tBox_ModDev_SearchDevID.Text == "" && tBox_ModDev_SearchModel.Text == "")
                {
                    MessageBox.Show("Both Value is Empty! Please Fill In Something.");
                    AddToShowModifyDev("Unable to accept NULL value on Device ID & Model Name!");
                }
            }
            else if (chBox_ModDev_SearchDevID.Checked && chBox_ModDev_SearchModel.Checked && chBox_ModDev_SearchProductName.Checked == false)
            {
                if (tBox_ModDev_SearchDevID.Text != "" && tBox_ModDev_SearchProductName.Text != "")
                {
                    SqlCommand cmd = new SqlCommand("Select_RegDeviceList", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Option", "GetListDeviceIDProductName");
                    cmd.Parameters.AddWithValue("@Dev_ID", tBox_ModDev_SearchDevID.Text);
                    cmd.Parameters.AddWithValue("@Model_Name", "");
                    cmd.Parameters.AddWithValue("@Product_Name", tBox_ModDev_SearchProductName.Text);
                    SqlDataAdapter sd = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable();
                    sd.Fill(dt);
                    dataGridView_TagList.DataSource = dt;
                    AddToShowModifyDev("Data Founded Under Database with combination of Device ID & Product Name!");
                }
                else if (tBox_ModDev_SearchDevID.Text == "")
                {
                    MessageBox.Show("Device ID is Empty! Please Fill In Something.");
                    AddToShowModifyDev("Unable to accept NULL value on Device ID!");
                }
                else if (tBox_ModDev_SearchProductName.Text == "")
                {
                    MessageBox.Show("Product Name is Empty! Please Fill In Something.");
                    AddToShowModifyDev("Unable to accept NULL value on Product Name!");
                }
                else if (tBox_ModDev_SearchDevID.Text == "" && tBox_ModDev_SearchProductName.Text == "")
                {
                    MessageBox.Show("Both Value is Empty! Please Fill In Something.");
                    AddToShowModifyDev("Unable to accept NULL value on Device ID & Product Name!");
                }
            }
            else if (chBox_ModDev_SearchDevID.Checked && chBox_ModDev_SearchModel.Checked && chBox_ModDev_SearchProductName.Checked == false)
            {
                if (tBox_ModDev_SearchModel.Text != "" && tBox_ModDev_SearchProductName.Text != "")
                {
                    SqlCommand cmd = new SqlCommand("Select_RegDeviceList", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Option", "GetListModelNameProductName");
                    cmd.Parameters.AddWithValue("@Dev_ID", "");
                    cmd.Parameters.AddWithValue("@Model_Name", tBox_ModDev_SearchModel.Text);
                    cmd.Parameters.AddWithValue("@Product_Name", tBox_ModDev_SearchProductName.Text);
                    SqlDataAdapter sd = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable();
                    sd.Fill(dt);
                    dataGridView_TagList.DataSource = dt;
                    AddToShowModifyDev("Data Founded Under Database with combination of Model Name & Product Name!");
                }
                else if (tBox_ModDev_SearchModel.Text == "")
                {
                    MessageBox.Show("Model Name is Empty! Please Fill In Something.");
                    AddToShowModifyDev("Unable to accept NULL value on Model Name!");
                }
                else if (tBox_ModDev_SearchProductName.Text == "")
                {
                    MessageBox.Show("Product Name is Empty! Please Fill In Something.");
                    AddToShowModifyDev("Unable to accept NULL value on Product Name!");
                }
                else if (tBox_ModDev_SearchModel.Text == "" && tBox_ModDev_SearchProductName.Text == "")
                {
                    MessageBox.Show("Both Value is Empty! Please Fill In Something.");
                    AddToShowModifyDev("Unable to accept NULL value on Model Name & Product Name!");
                }
            }
            else if (chBox_ModDev_SearchDevID.Checked && chBox_ModDev_SearchModel.Checked && chBox_ModDev_SearchProductName.Checked)
            {
                if (tBox_ModDev_SearchDevID.Text != "" && tBox_ModDev_SearchModel.Text != "" && tBox_ModDev_SearchProductName.Text != "")
                {
                    SqlCommand cmd = new SqlCommand("Select_RegDeviceList", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Option", "GetListFullCombo");
                    cmd.Parameters.AddWithValue("@Dev_ID", tBox_ModDev_SearchDevID.Text);
                    cmd.Parameters.AddWithValue("@Model_Name", tBox_ModDev_SearchModel.Text);
                    cmd.Parameters.AddWithValue("@Product_Name", tBox_ModDev_SearchProductName.Text);
                    SqlDataAdapter sd = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable();
                    sd.Fill(dt);
                    dataGridView_TagList.DataSource = dt;
                    AddToShowModifyDev("Data Founded Under Database with All Combination!");
                }
                else if (tBox_ModDev_SearchDevID.Text == "")
                {
                    MessageBox.Show("Device ID is Empty! Please Fill In Something.");
                    AddToShowModifyDev("Unable to accept NULL value on Device ID!");
                }
                else if (tBox_ModDev_SearchModel.Text == "")
                {
                    MessageBox.Show("Model Name is Empty! Please Fill In Something.");
                    AddToShowModifyDev("Unable to accept NULL value on Model Name!");
                }
                else if (tBox_ModDev_SearchProductName.Text == "")
                {
                    MessageBox.Show("Product Name is Empty! Please Fill In Something.");
                    AddToShowModifyDev("Unable to accept NULL value on Product Name!");
                }
                else if (tBox_ModDev_SearchDevID.Text == "" && tBox_ModDev_SearchModel.Text == "" && tBox_ModDev_SearchProductName.Text == "")
                {
                    MessageBox.Show("All Value is Empty! Please Fill In Something.");
                    AddToShowModifyDev("Unable to accept NULL on all value!");
                }
            }
            else
            {
                MessageBox.Show("No CheckBox is select, Please Check before Proceed!");
                AddToShowModifyDev("Unable to run Search without check any of the box!");
            }*/

            sqlConnect.con.Close();
        }

        private void dataGridView_DevList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dataGridView_DevList.Rows[e.RowIndex];
                
                tBox_ModDev_UserReg.Text = row.Cells[3].Value.ToString();
                tBox_ModDev_OldDevID.Text = row.Cells[1].Value.ToString();
                tBox_ModDev_OldModel.Text = row.Cells[4].Value.ToString();
                tBox_ModDev_OldProductName.Text = row.Cells[5].Value.ToString();
                tBox_ModDev_OldCommType.Text = row.Cells[6].Value.ToString();
                tBox_ModDev_OldDevIP.Text = row.Cells[7].Value.ToString();
            }
        }

        private void newUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegistrationUser registrationUser = new RegistrationUser();
            registrationUser.ShowDialog();
        }

        private void newAnthenaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegistrationAnthena registrationAnthena = new RegistrationAnthena();
            registrationAnthena.Show();
        }

        private void tBox_ModTag_SearchEPCNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btn_ModTag_Search_Click(this, new EventArgs());
            }
        }

        private void tBox_ModDev_SearchDevID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btn_ModDev_SearchDev_Click(this, new EventArgs());
            }
        }

        private void btn_ModUser_Search_Click(object sender, EventArgs e)
        {
            sqlConnect.con.Open();

            if (tBox_ModUser_Search.Text != string.Empty)
            {

                SqlCommand cmd = new SqlCommand("Select_UsersTB", sqlConnect.con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Option", "GetSearchingList");
                cmd.Parameters.AddWithValue("@Username", tBox_ModUser_Search.Text);
                cmd.Parameters.AddWithValue("@Password", string.Empty);
                cmd.Parameters.AddWithValue("@Fullname", string.Empty);
                cmd.Parameters.AddWithValue("@User_Type", tBox_ModUser_Search.Text);
                cmd.Parameters.AddWithValue("@Employee_ID", tBox_ModUser_Search.Text);
                SqlDataAdapter sd = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                sd.Fill(dt);
                dataGridView_UserList.DataSource = dt;
            }
            else if (tBox_ModUser_Search.Text == string.Empty)
            {
                SqlCommand cmd = new SqlCommand("Select_UsersTB", sqlConnect.con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Option", "GetFullUsersList");
                cmd.Parameters.AddWithValue("@Username", string.Empty);
                cmd.Parameters.AddWithValue("@Password", string.Empty);
                cmd.Parameters.AddWithValue("@Fullname", string.Empty);
                cmd.Parameters.AddWithValue("@User_Type", string.Empty);
                cmd.Parameters.AddWithValue("@Employee_ID", string.Empty);
                SqlDataAdapter sd = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                sd.Fill(dt);
                dataGridView_UserList.DataSource = dt;
            }

            sqlConnect.con.Close();
        }

        private void btn_ModUser_Update_Click(object sender, EventArgs e)
        {
            if (dataGridView_UserList.SelectedCells.Count > 0)
            {
                int selectedRowIndex = dataGridView_UserList.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = dataGridView_UserList.Rows[selectedRowIndex];
                IuserId = Convert.ToString(selectedRow.Cells["User_Id"].Value);
                IUsername = Convert.ToString(selectedRow.Cells["Username"].Value);
                IUserType = Convert.ToString(selectedRow.Cells["User_Type"].Value);
                IUserGroupID = Convert.ToString(selectedRow.Cells["User_Group_Id"].Value);
                IUserGroupName = Convert.ToString(selectedRow.Cells["User_Group_Name"].Value);
                IUserGroupDesc = Convert.ToString(selectedRow.Cells["User_Group_Description"].Value);
                IFullname = Convert.ToString(selectedRow.Cells["Fullname"].Value);
                IEmployeeID = Convert.ToString(selectedRow.Cells["Emp_Id"].Value);
                IEmail = Convert.ToString(selectedRow.Cells["Email"].Value);
                IPassword = Convert.ToString(selectedRow.Cells["Password"].Value);
                IMobileNo = Convert.ToString(selectedRow.Cells["Mobile_No"].Value);
                ICardID = Convert.ToString(selectedRow.Cells["Card_Id"].Value);
            }

            using (UserUpdate userUpdate = new UserUpdate())
            {
                userUpdate.UserId = IuserId;
                userUpdate.Username = IUsername;
                userUpdate.User_Type = IUserType;
                userUpdate.User_Group_Id = IUserGroupID;
                userUpdate.User_Group_Name = IUserGroupName;
                userUpdate.User_Group_Desc = IUserGroupDesc;
                userUpdate.Fullname = IFullname;
                userUpdate.Emp_Id = IEmployeeID;
                userUpdate.Email = IEmail;
                userUpdate.Password = IPassword;
                userUpdate.Mobile_No = IMobileNo;
                userUpdate.Card_Id = ICardID;
                userUpdate.ShowDialog();
            }
        }
    }
}
