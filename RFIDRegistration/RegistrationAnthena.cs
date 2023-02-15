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
    public partial class RegistrationAnthena : Form
    {
        AutoCompleteStringCollection coll = new AutoCompleteStringCollection();
        SqlConnection con = new SqlConnection("Data Source=MXPACALEX;Initial Catalog=Dek_MachineDB;Integrated Security=True;MultipleActiveResultSets=true");

        public RegistrationAnthena()
        {
            InitializeComponent();
        }

        private void btn_CancelAnthena_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        public void Insert_RegistrationAnthena()
        {
            if (tBox_RegAnthena_DevID.Text != "" && tBox_RegAnthena_AntType.Text != "" && cBox_RegAnthena_ChnNo.Text != "" && tBox_RegAnthena_AntLocation.Text != "")
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("Select_RegAntList", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Option", "InsertAntList");
                cmd.Parameters.AddWithValue("@Device_ID", tBox_RegAnthena_DevID.Text);
                cmd.Parameters.AddWithValue("@Channel_No", cBox_RegAnthena_ChnNo.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@Anthena_Type", tBox_RegAnthena_AntType.Text);
                cmd.Parameters.AddWithValue("@Anthena_Location", tBox_RegAnthena_AntLocation.Text);

                SqlParameter returnParameter = cmd.Parameters.Add("@RetVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();
                int counter = (int)returnParameter.Value;

                if (counter == 1)
                {
                    lbl_RegAnthena_RegisterStatus.Show();
                    lbl_RegAnthena_RegisterStatus.Text = "Registration Failed! Channel No On this Device Has Been Registered!";
                    lbl_RegAnthena_RegisterStatus.ForeColor = Color.Red;
                    Timer t = new Timer
                    {
                        Interval = 3000 // it will Tick in 3 seconds
                    };
                    t.Tick += (s, eV) =>
                    {
                        lbl_RegAnthena_RegisterStatus.Hide();
                        t.Stop();
                    };
                    t.Start();
                }
                else
                {
                    lbl_RegAnthena_RegisterStatus.Show();
                    lbl_RegAnthena_RegisterStatus.Text = "Registered Successfully!";
                    lbl_RegAnthena_RegisterStatus.ForeColor = Color.DarkSeaGreen;
                    Timer t = new Timer
                    {
                        Interval = 3000 // it will Tick in 3 seconds
                    };
                    t.Tick += (s, eV) =>
                    {
                        lbl_RegAnthena_RegisterStatus.Hide();
                        t.Stop();
                    };
                    t.Start();
                    ClearTxt();
                }
                

                
                con.Close();
            }
            else
            {
                Validation();
            }
            
        }

        public void ClearTxt()
        {
            tBox_RegAnthena_DevID.Text = "";
            tBox_RegAnthena_AntType.Text = "";
            cBox_RegAnthena_ChnNo.SelectedItem = null;
            tBox_RegAnthena_AntLocation.Text = "";
        }

        public void Validation()
        {
            if (tBox_RegAnthena_DevID.Text == string.Empty)
            {
                lbl_RegAnthena_WarnDevID.Show();
            }
            else if (tBox_RegAnthena_AntType.Text == string.Empty)
            {
                lbl_RegAnthena_WarnChnNo.Show();
            }
            else if (cBox_RegAnthena_ChnNo.Text == string.Empty)
            {
                lbl_RegAnthena_WarnAnthenaType.Show();
            }
            else if (tBox_RegAnthena_AntLocation.Text == string.Empty)
            {
                lbl_RegAnthena_WarnAnthenaLocation.Show();
            }

            Timer t = new Timer
            {
                Interval = 3000 // it will Tick in 3 seconds
            };
            t.Tick += (s, eV) =>
            {
                lbl_RegAnthena_WarnDevID.Hide();
                lbl_RegAnthena_WarnChnNo.Hide();
                lbl_RegAnthena_WarnAnthenaType.Hide();
                lbl_RegAnthena_WarnAnthenaLocation.Hide();
                t.Stop();
            };
            t.Start();
        }

        private void btn_SaveAnthena_Click(object sender, EventArgs e)
        {
            Insert_RegistrationAnthena();
        }

        private void RegistrationAnthena_Load(object sender, EventArgs e)
        {
            lbl_RegAnthena_RegisterStatus.Hide();
            lbl_RegAnthena_WarnDevID.Hide();
            lbl_RegAnthena_WarnChnNo.Hide();
            lbl_RegAnthena_WarnAnthenaType.Hide();
            lbl_RegAnthena_WarnAnthenaLocation.Hide();
            AutoCompleteTxtBox();
        }

        public void AutoCompleteTxtBox()
        {
            SqlCommand sqlCommand = new SqlCommand("Select_RegDeviceList", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            sqlCommand.Parameters.AddWithValue("@Option", "GetFullDeviceList");
            sqlCommand.Parameters.AddWithValue("@Dev_ID", "");
            sqlCommand.Parameters.AddWithValue("@Model_Name", "");
            sqlCommand.Parameters.AddWithValue("@Product_Name", "");

            SqlDataAdapter dataAdapter_GetDeviceList = new SqlDataAdapter(sqlCommand);
            DataTable dt2 = new DataTable();
            dataAdapter_GetDeviceList.Fill(dt2);

            if (dt2.Rows.Count > 0)
            {
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    coll.Add(dt2.Rows[i]["Dev_ID"].ToString());
                }
            }
            else
            {
                lbl_RegAnthena_WarnDevID.Show();
                lbl_RegAnthena_WarnDevID.Text = "Device ID Not Found";
                lbl_RegAnthena_WarnDevID.ForeColor = Color.Red;
                var t = new Timer
                {
                    Interval = 3000 // it will Tick in 3 seconds
                };
                t.Tick += (s, eV) =>
                {
                    lbl_RegAnthena_WarnDevID.Hide();
                    t.Stop();
                };
                t.Start();
            }

            tBox_RegAnthena_DevID.AutoCompleteMode = AutoCompleteMode.Suggest;
            tBox_RegAnthena_DevID.AutoCompleteSource = AutoCompleteSource.CustomSource;
            tBox_RegAnthena_DevID.AutoCompleteCustomSource = coll;
        }
    }
}
