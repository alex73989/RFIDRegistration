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
    public partial class Login : Form
    {
        string RequestFillInTheBlank = "Please Do Not Leave Blank at Column above!";

        // Address of SQL Server and Database
        SqlConnect sqlConnect = new SqlConnect();
        public SqlCommand cmd;
        SqlDataReader dr;

        public Login()
        {
            InitializeComponent();
            sqlConnect.Connection();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            lbl_LogUser_WarnPassword.Hide();
            lbl_LogUser_WarnUsername.Hide();
            tBox_Login_Password.PasswordChar = '*';
        }

        private void btn_LoginMain_Click(object sender, EventArgs e)
        {
            if (tBox_Login_Username.Text != "")
            {
                if (tBox_Login_Password.Text != "")
                {
                    sqlConnect.con = new SqlConnection("Data Source=MXPACALEX;Initial Catalog=Dek_MachineDB;Integrated Security=True");
                    sqlConnect.con.Open();

                    cmd = new SqlCommand("SELECT * FROM users_tb WHERE Username = '" + tBox_Login_Username.Text + "' AND Password = '" + tBox_Login_Password.Text + "' ", sqlConnect.con);
                    dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        if (dr.Read())
                        {
                            if (Convert.ToString(dr["User_type"]) == "Admin")
                            {
                                dr.Close();
                                this.Hide();
                                MessageBox.Show("You are logged in as an Admin");
                                MainForm form1 = new MainForm
                                {
                                    adminLogin = true
                                };
                                form1.ShowDialog();
                            }
                            else if (Convert.ToString(dr["User_type"]) == "User")
                            {
                                dr.Close();
                                this.Hide();
                                MessageBox.Show("You are logged in as an User");
                                MainForm form2 = new MainForm();
                                form2.ShowDialog();
                            }
                        }
                        else
                        {
                            dr.Close();
                            MessageBox.Show("Account Not Found, Please Register!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    sqlConnect.con.Close();
                }
                else if (tBox_Login_Password.Text == "")
                {
                    lbl_LogUser_WarnPassword.Text = RequestFillInTheBlank;
                }
                                            }
            else if (tBox_Login_Username.Text == "")
            {
                lbl_LogUser_WarnUsername.Text = RequestFillInTheBlank;
            }

        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
        }

        private void cBox_ShowHide_CheckedChanged(object sender, EventArgs e)
        {
            if (cBox_ShowHide.Checked)
                                {
                tBox_Login_Password.PasswordChar = '\0';
            }
            else
            {
                tBox_Login_Password.PasswordChar = '*';
            }
        }

        private void tBox_Login_Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btn_LoginMain_Click(this, new EventArgs());
            }

            if (Control.IsKeyLocked(Keys.CapsLock))
            {
                lbl_LogUser_WarnUsername.Show();
                lbl_LogUser_WarnUsername.Text = "The Caps Lock is ON";
                lbl_LogUser_WarnUsername.ForeColor = Color.Red;
                lbl_LogUser_WarnPassword.Show();
                lbl_LogUser_WarnPassword.Text = "The Caps Lock is ON";
                lbl_LogUser_WarnPassword.ForeColor = Color.Red;
            }
            else
            {
                lbl_LogUser_WarnUsername.Hide();
                lbl_LogUser_WarnPassword.Hide();
            }
        }

        private void tBox_Login_Username_KeyDown(object sender, KeyEventArgs e)
        {
            if (Control.IsKeyLocked(Keys.CapsLock))
            {
                lbl_LogUser_WarnUsername.Show();
                lbl_LogUser_WarnUsername.Text = "The Caps Lock is ON";
                lbl_LogUser_WarnUsername.ForeColor = Color.Red;
                lbl_LogUser_WarnPassword.Show();
                lbl_LogUser_WarnPassword.Text = "The Caps Lock is ON";
                lbl_LogUser_WarnPassword.ForeColor = Color.Red;
            }
            else
            {
                lbl_LogUser_WarnUsername.Hide();
                lbl_LogUser_WarnPassword.Hide();
            }
        }
    }
}
