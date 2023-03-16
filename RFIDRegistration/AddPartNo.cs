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
using RFIDRegistration.Models;

namespace RFIDRegistration
{
    public partial class AddPartNo : Form
    {
        public SqlConnect sqlConnect = new SqlConnect();

        public AddPartNo()
        {
            InitializeComponent();
            sqlConnect.Connection();
        }

        private void btn_AddPart_Add_Click(object sender, EventArgs e)
        {
            if (tBox_AddPart_PartNo.Text != "")
            {
                sqlConnect.con.Open();
                SqlCommand cmd = new SqlCommand("Select_RegTagList", sqlConnect.con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Option", "InsertPartNo");
                cmd.Parameters.AddWithValue("@TAG_epc_no", "");
                cmd.Parameters.AddWithValue("@SERIAL_no", "");
                cmd.Parameters.AddWithValue("@PART_no", tBox_AddPart_PartNo.Text.ToString());

                SqlParameter returnParameter = cmd.Parameters.Add("@RetVal", SqlDbType.Int);
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

                sqlConnect.con.Close();
            }
            else if (tBox_AddPart_PartNo.Text == "")
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

        private void AddPartNo_Load(object sender, EventArgs e)
        {
            lbl_AddPart_Register.Hide();
        }

        private void tBox_AddPart_PartNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btn_AddPart_Add_Click(this, new EventArgs());
            }
        }

        public void ClearTxt()
        {
            tBox_AddPart_PartNo.Text = "";
        }
    }
}
