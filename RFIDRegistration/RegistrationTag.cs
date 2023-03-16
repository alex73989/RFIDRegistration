using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using RFIDRegistration.Public;
using RFIDRegistration.Models;

namespace RFIDRegistration
{
    public partial class RegistrationTag : Form
    {
        AutoCompleteStringCollection coll = new AutoCompleteStringCollection();
        private const string requestAttachDeviceText = "Please Connect To Device";

        // Address of SQL Server and Database
        SqlConnect sqlConnect = new SqlConnect();

        public RegistrationTag()
        {
            InitializeComponent();
            SetComboBoxValueEmpName();
            sqlConnect.Connection();
        }

        public void TagTextRefresh()
        {
            cBox_Tag_EmpName.SelectedItem = null;
            tBox_Tag_EPCNo.Text = "";
            tBox_Tag_PartNo.Text = "";
            cBox_Tag_SerialNo.SelectedItem = null;
            cBox_Tag_Location.SelectedItem = null;
            tBox_Tag_FunctionUsed.Text = "";
            tBox_Tag_MoreDesc.Text = "";
        }

        public void SetComboBoxValueEmpName()
        {
            sqlConnect.con.Open();
            if (sqlConnect.con.State == System.Data.ConnectionState.Open)
            {
                SqlCommand sqlCommand = new SqlCommand("Select_UsersTB", sqlConnect.con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                sqlCommand.Parameters.AddWithValue("@Option", "GetFullUsersList");
                sqlCommand.Parameters.AddWithValue("@Username", "");
                sqlCommand.Parameters.AddWithValue("@Password", "");
                sqlCommand.Parameters.AddWithValue("@Fullname", "");
                sqlCommand.Parameters.AddWithValue("@User_Type", "");
                sqlCommand.Parameters.AddWithValue("@Employee_ID", "");

                SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
                DataSet ds = new DataSet();
                da.Fill(ds);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    cBox_Tag_EmpName.Items.Add(ds.Tables[0].Rows[i][6].ToString());
                }
            }
            sqlConnect.con.Close();
        }

        public void AutoCompletePartNo()
        {
            SqlCommand sqlCommand = new SqlCommand("Select_RegTagList", sqlConnect.con)
            {
                CommandType = CommandType.StoredProcedure
            };
            sqlCommand.Parameters.AddWithValue("@Option", "GetPartList");
            sqlCommand.Parameters.AddWithValue("@TAG_epc_no", "");
            sqlCommand.Parameters.AddWithValue("@SERIAL_no", "");
            sqlCommand.Parameters.AddWithValue("@PART_no", "");

            SqlDataAdapter dataAdapter_GetDeviceList = new SqlDataAdapter(sqlCommand);
            DataTable dt2 = new DataTable();
            dataAdapter_GetDeviceList.Fill(dt2);

            if (dt2.Rows.Count > 0)
            {
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    coll.Add(dt2.Rows[i]["Part_No"].ToString());
                }
            }
            else
            {
                lbl_Status.Show();
                lbl_Status.Text = "Part Number Not Found";
                lbl_Status.ForeColor = Color.Red;
                var t = new Timer
                {
                    Interval = 3000 // it will Tick in 3 seconds
                };
                t.Tick += (s, eV) =>
                {
                    lbl_Status.Hide();
                    t.Stop();
                };
                t.Start();
            }

            tBox_Tag_PartNo.AutoCompleteMode = AutoCompleteMode.Suggest;
            tBox_Tag_PartNo.AutoCompleteSource = AutoCompleteSource.CustomSource;
            tBox_Tag_PartNo.AutoCompleteCustomSource = coll;
        }

        private void btnTag_ClearRecords_Click(object sender, EventArgs e)
        {
            TagTextRefresh();
        }

        private void btnTag_SaveRecords_Click(object sender, EventArgs e)
        {
            if (cBox_Tag_EmpName.Text != "" && tBox_Tag_EPCNo.Text != "" && tBox_Tag_PartNo.Text != "" && cBox_Tag_SerialNo.Text != ""  && cBox_Tag_Location.Text != "" && tBox_Tag_FunctionUsed.Text != "" && tBox_Tag_MoreDesc.Text != "")
            {
                if (PublicFunction.DuplicateTagChecking(tBox_Tag_EPCNo.Text, "tag_epc_no", "@tag_epc_no"))
                {
                    MessageBox.Show("Current Tag Have been duplicate!");
                }
                else
                {
                    // Open Connection
                    sqlConnect.con.Open();

                    if (sqlConnect.con.State == System.Data.ConnectionState.Open)
                    {
                        SqlCommand sqlCommand = new SqlCommand("Insert_RegisterTagList", sqlConnect.con)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        sqlCommand.Parameters.AddWithValue("@employee_name", cBox_Tag_EmpName.SelectedItem.ToString());
                        sqlCommand.Parameters.AddWithValue("@serial_no", cBox_Tag_SerialNo.SelectedItem.ToString());
                        sqlCommand.Parameters.AddWithValue("@part_no", tBox_Tag_PartNo.Text);
                        sqlCommand.Parameters.AddWithValue("@tag_epc_no", tBox_Tag_EPCNo.Text);
                        sqlCommand.Parameters.AddWithValue("@location", cBox_Tag_Location.SelectedItem.ToString());
                        sqlCommand.Parameters.AddWithValue("@function_used", tBox_Tag_FunctionUsed.Text);
                        sqlCommand.Parameters.AddWithValue("@more_desc", tBox_Tag_MoreDesc.Text);

                        SqlParameter returnParameter = sqlCommand.Parameters.Add("@RetVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;
                        SqlParameter returnParameter_2 = sqlCommand.Parameters.Add("@RetVal_2", SqlDbType.Int);
                        returnParameter_2.Direction = ParameterDirection.ReturnValue;

                        sqlCommand.ExecuteNonQuery();
                        int counterTag = (int)returnParameter.Value;
                        int counterPartNo = (int)returnParameter_2.Value;

                        if (counterTag == 1)
                        {
                            lbl_Status.Show();
                            lbl_Status.Text = "Tag Has Been Registered Before, Please Try Again with Diff Tag!";
                            lbl_Status.ForeColor = Color.Red;
                        }
                        else if (counterPartNo == 0)
                        {
                            lbl_Status.Show();
                            lbl_Status.Text = "Part No Haven't Registered!";
                            lbl_Status.ForeColor = Color.Yellow;
                        }
                        else
                        {
                            lbl_Status.Show();
                            lbl_Status.Text = "Tag Has Been Insert Successfully";
                            lbl_Status.ForeColor = Color.DarkSeaGreen;
                        }

                        Timer t = new Timer
                        {
                            Interval = 3000 // it will Tick in 3 seconds
                        };
                        t.Tick += (s, eV) =>
                        {
                            lbl_Status.Hide();
                            t.Stop();
                        };
                        t.Start();

                        TagTextRefresh();
                    }
                    else
                    {
                        MessageBox.Show("Connection Failure, Please Try Again!");
                    }

                    // Close Connection
                    sqlConnect.con.Close();
                }
            }
            else
            {
                MessageBox.Show("Please Fill In All The Blank Value! Thanks");
                //AddToShow("Not Accepted Value with Nulls, Please Try Again..");
            }
        }

        private void btnScan_Click(object sender, EventArgs e)
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
                MessageBox.Show("Scan Failed!");
                //AddToShow("Scan Failed, Please try Again!");
                return;
            }
            string uiiStr = DemoPublic.BytesToHexString(uii, uiiLen[0]);
            //AddToShow("Scan Successful, UII : " + uiiStr);
            MessageBox.Show("Scan Successful");

            DemoPublic.tagInfo.uii = uiiStr;
            tBox_Tag_EPCNo.Text = uiiStr;
        }

        private void RegistrationTag_Load(object sender, EventArgs e)
        {
            AutoCompletePartNo();
            lbl_Status.Hide();
        }

        public void PopulateComboBox()
        {
            sqlConnect.con.Open();
            if (sqlConnect.con.State == System.Data.ConnectionState.Open)
            {
                SqlCommand sqlCommand = new SqlCommand("Select_RegTagList", sqlConnect.con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                sqlCommand.Parameters.AddWithValue("@Option", "GetSerialNoBasedOnPartNo");
                sqlCommand.Parameters.AddWithValue("@TAG_epc_no", "");
                sqlCommand.Parameters.AddWithValue("@SERIAL_no", "");
                sqlCommand.Parameters.AddWithValue("@PART_no", tBox_Tag_PartNo.Text);

                SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
                DataSet ds = new DataSet();
                da.Fill(ds);
                cBox_Tag_SerialNo.Items.Clear();

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    cBox_Tag_SerialNo.Items.Add(ds.Tables[0].Rows[i]["Serial_No"].ToString());
                }
            }
            sqlConnect.con.Close();
        }

        private void tBox_Tag_PartNo_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(tBox_Tag_PartNo.Text))
            {
                PopulateComboBox();
            }
        }
    }
}
