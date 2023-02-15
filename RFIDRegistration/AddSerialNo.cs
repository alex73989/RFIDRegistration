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

namespace RFIDRegistration
{
    public partial class AddSerialNo : Form
    {
        AutoCompleteStringCollection coll = new AutoCompleteStringCollection();
        SqlConnection con = new SqlConnection("Data Source=MXPACALEX;Initial Catalog=Dek_MachineDB;Integrated Security=True;MultipleActiveResultSets=true");

        public AddSerialNo()
        {
            InitializeComponent();
        }

        public void AutoCompleteTxtBox()
        {
            SqlCommand sqlCommand = new SqlCommand("Select_RegTagList", con)
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
                lbl_AddSN_WarnPN.Show();
                lbl_AddSN_WarnPN.Text = "Part Number Not Found";
                lbl_AddSN_WarnPN.ForeColor = Color.Red;
                var t = new Timer
                {
                    Interval = 3000 // it will Tick in 3 seconds
                };
                t.Tick += (s, eV) =>
                {
                    lbl_AddSN_WarnPN.Hide();
                    t.Stop();
                };
                t.Start();
            }

            tBox_AddSN_PartNo.AutoCompleteMode = AutoCompleteMode.Suggest;
            tBox_AddSN_PartNo.AutoCompleteSource = AutoCompleteSource.CustomSource;
            tBox_AddSN_PartNo.AutoCompleteCustomSource = coll;
        }

        public void ClearTxt()
        {
            tBox_AddSN_PartNo.Text = "";
            tBox_AddSN_SerialNo.Text = "";
        }

        private void AddSerialNo_Load(object sender, EventArgs e)
        {
            lbl_AddSN_WarnPN.Hide();
            lbl_AddSN_WarnSN.Hide();
            lbl_AddPart_Register.Hide();
            AutoCompleteTxtBox();
        }

        private void tBox_AddSN_SerialNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btn_AddPart_Add_Click(this, new EventArgs());
            }
        }

        private void btn_AddPart_Add_Click(object sender, EventArgs e)
        {
            if (tBox_AddSN_PartNo.Text != "" && tBox_AddSN_SerialNo.Text != "")
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Select_RegTagList", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Option", "InsertSerialNo");
                cmd.Parameters.AddWithValue("@TAG_epc_no", "");
                cmd.Parameters.AddWithValue("@SERIAL_no", tBox_AddSN_SerialNo.Text.ToString());
                cmd.Parameters.AddWithValue("@PART_no", tBox_AddSN_PartNo.Text.ToString());

                SqlParameter returnParameter = cmd.Parameters.Add("@RetVal_2", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();
                int counter = (int)returnParameter.Value;

                if (counter == 1)
                {
                    lbl_AddPart_Register.Show();
                    lbl_AddPart_Register.Text = "Insert Failed to Database!";
                    lbl_AddPart_Register.ForeColor = Color.Red;
                    Timer t = new Timer
                    {
                        Interval = 3000 // it will Tick in 3 seconds
                    };
                    t.Tick += (s, eV) =>
                    {
                        lbl_AddPart_Register.Hide();
                        t.Stop();
                    };
                    t.Start();
                }
                else
                {
                    lbl_AddPart_Register.Show();
                    lbl_AddPart_Register.Text = "Registered Successfully!";
                    lbl_AddPart_Register.ForeColor = Color.DarkSeaGreen;
                    Timer t = new Timer
                    {
                        Interval = 3000 // it will Tick in 3 seconds
                    };
                    t.Tick += (s, eV) =>
                    {
                        lbl_AddPart_Register.Hide();
                        t.Stop();
                    };
                    t.Start();
                    ClearTxt();
                }

                con.Close();
            }
            else if (tBox_AddSN_PartNo.Text == "" || tBox_AddSN_SerialNo.Text == "")
            {
                lbl_AddPart_Register.Show();
                lbl_AddPart_Register.Text = "Unable accept blank!";
                lbl_AddPart_Register.ForeColor = Color.Red;
                Timer t = new Timer
                {
                    Interval = 3000 // it will Tick in 3 seconds
                };
                t.Tick += (s, eV) =>
                {
                    lbl_AddPart_Register.Hide();
                    t.Stop();
                };
                t.Start();
            }
        }
    }
}
