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
    public partial class RegistrationUser : Form
    {
        string RequestFillInTheBlank = "Please Do Not Leave Blank at Column above!";
        // Address of SQL Server and Database
        SqlConnect sqlConnect = new SqlConnect();
        public SqlCommand cmd;
        SqlDataReader dr;

        public RegistrationUser()
        {
            InitializeComponent();
            sqlConnect.Connection();
        }

        private void RegistrationUser_Load(object sender, EventArgs e)
        { 
            lbl_RegUser_WarnUsername.Text = string.Empty;
            lbl_RegUser_WarnFullname.Text = string.Empty;
            lbl_RegUser_WarnPassword.Text = string.Empty;
            lbl_RegUser_WarnEmpID.Text = string.Empty;
            lbl_RegUser_WarnEmailAddr.Text = string.Empty;
            lbl_RegUser_WarnContactNo.Text = string.Empty;
            lbl_RegUser_WarnCardID.Text = string.Empty;
            lbl_RegUser_WarnUserType.Text = string.Empty;
            lbl_RegUser_WarnGroupID.Text = string.Empty;
            lbl_RegUser_WarnGroupName.Text = string.Empty;
            lbl_RegUser_WarnGroupDesc.Text = string.Empty;
        }

        public void TextRefreshRegUser()
        {
            tBox_RegUser_Username.Text = string.Empty;
            tBox_RegUser_Fullname.Text = string.Empty;
            tBox_RegUser_Password.Text = string.Empty;
            tBox_RegUser_EmpID.Text = string.Empty;
            tBox_RegUser_EmailAddr.Text = string.Empty;
            tBox_RegUser_ContactNo.Text = string.Empty;
            tBox_RegUser_CardID.Text = string.Empty;
            cBox_RegUser_UserType.Text = string.Empty;
            tBox_RegUser_GroupID.Text = string.Empty;
            tBox_RegUser_GroupName.Text = string.Empty;
            tBox_RegUser_GroupDesc.Text = string.Empty;
        }

        private void btn_RegisterNow_Click(object sender, EventArgs e)
        {
            if (tBox_RegUser_Username.Text != string.Empty)
            {
                if (tBox_RegUser_Fullname.Text != string.Empty)
                {
                    if (tBox_RegUser_Password.Text != string.Empty)
                    {
                        if (tBox_RegUser_EmpID.Text != string.Empty)
                        {
                            if (tBox_RegUser_EmailAddr.Text != string.Empty)
                            {
                                if (tBox_RegUser_ContactNo.Text != string.Empty)
                                {
                                    if (tBox_RegUser_CardID.Text != string.Empty)
                                    {
                                        if (cBox_RegUser_UserType.Text != string.Empty)
                                        {
                                            if (tBox_RegUser_GroupID.Text != string.Empty)
                                            {
                                                if (tBox_RegUser_GroupName.Text != string.Empty)
                                                {
                                                    if (tBox_RegUser_GroupDesc.Text != string.Empty)
                                                    {
                                                        Insert_RegistrationUser();
                                                        TextRefreshRegUser();
                                                    }
                                                    else if (tBox_RegUser_GroupDesc.Text == string.Empty)
                                                    {
                                                        lbl_RegUser_WarnGroupDesc.Text = RequestFillInTheBlank;
                                                    }
                                                }
                                                else if (tBox_RegUser_GroupName.Text == string.Empty)
                                                {
                                                    lbl_RegUser_WarnGroupName.Text = RequestFillInTheBlank;
                                                }
                                            }
                                            else if (tBox_RegUser_GroupID.Text == string.Empty)
                                            {
                                                lbl_RegUser_WarnGroupID.Text = RequestFillInTheBlank;
                                            }
                                        }
                                        else if (cBox_RegUser_UserType.Text == string.Empty)
                                        {
                                            lbl_RegUser_WarnUserType.Text = RequestFillInTheBlank;
                                        }
                                    }
                                    else if (tBox_RegUser_CardID.Text == string.Empty)
                                    {
                                        lbl_RegUser_WarnCardID.Text = RequestFillInTheBlank;
                                    }
                                }
                                else if (tBox_RegUser_ContactNo.Text == string.Empty)
                                {
                                    lbl_RegUser_WarnContactNo.Text = RequestFillInTheBlank;
                                }
                            }
                            else if (tBox_RegUser_EmailAddr.Text == string.Empty)
                            {
                                lbl_RegUser_WarnEmailAddr.Text = RequestFillInTheBlank;
                            }
                        }
                        else if (tBox_RegUser_EmpID.Text == string.Empty)
                        {
                            lbl_RegUser_WarnEmpID.Text = RequestFillInTheBlank;
                        }
                    }
                    else if (tBox_RegUser_Password.Text == string.Empty)
                    {
                        lbl_RegUser_WarnPassword.Text = RequestFillInTheBlank;
                    }
                }
                else if (tBox_RegUser_Fullname.Text == string.Empty)
                {
                    lbl_RegUser_WarnFullname.Text = RequestFillInTheBlank;
                }
            }
            else if (tBox_RegUser_Username.Text == string.Empty)
            {
                lbl_RegUser_WarnUsername.Text = RequestFillInTheBlank;
            }
        }

        public void Insert_RegistrationUser()
        {
            
            sqlConnect.con.Open();

            cmd = new SqlCommand("SELECT * FROM users_tb WHERE Username = '" + tBox_RegUser_Username + "'", sqlConnect.con);
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                dr.Close();
                MessageBox.Show("Username Already Exists, Please Try Again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                dr.Close();
                
                if(sqlConnect.con.State == System.Data.ConnectionState.Open)
                {
                    SqlDataAdapter dataAdapter_RegUser = new SqlDataAdapter("Insert_RegUserList", sqlConnect.con);

                    dataAdapter_RegUser.SelectCommand.CommandType = CommandType.StoredProcedure;
                    dataAdapter_RegUser.SelectCommand.Parameters.Add("@username", SqlDbType.VarChar).Value = tBox_RegUser_Username.Text;
                    dataAdapter_RegUser.SelectCommand.Parameters.Add("@fullname", SqlDbType.VarChar).Value = tBox_RegUser_Fullname.Text;
                    dataAdapter_RegUser.SelectCommand.Parameters.Add("@password", SqlDbType.VarChar).Value = tBox_RegUser_Password.Text;
                    dataAdapter_RegUser.SelectCommand.Parameters.Add("@emp_id", SqlDbType.VarChar).Value = tBox_RegUser_EmpID.Text;
                    dataAdapter_RegUser.SelectCommand.Parameters.Add("@email", SqlDbType.VarChar).Value = tBox_RegUser_EmailAddr.Text;
                    dataAdapter_RegUser.SelectCommand.Parameters.Add("@mobile_no", SqlDbType.VarChar).Value = tBox_RegUser_ContactNo.Text;
                    dataAdapter_RegUser.SelectCommand.Parameters.Add("@card_id", SqlDbType.VarChar).Value = tBox_RegUser_CardID.Text;
                    dataAdapter_RegUser.SelectCommand.Parameters.Add("@user_type", SqlDbType.VarChar).Value = cBox_RegUser_UserType.Text;
                    dataAdapter_RegUser.SelectCommand.Parameters.Add("@user_group_id", SqlDbType.VarChar).Value = tBox_RegUser_GroupID.Text;
                    dataAdapter_RegUser.SelectCommand.Parameters.Add("@user_group_name", SqlDbType.VarChar).Value = tBox_RegUser_GroupName.Text;
                    dataAdapter_RegUser.SelectCommand.Parameters.Add("@user_group_desc", SqlDbType.VarChar).Value = tBox_RegUser_GroupDesc.Text;

                    int cmdChecking = dataAdapter_RegUser.SelectCommand.ExecuteNonQuery();

                    if (cmdChecking > 0)
                    {
                        MessageBox.Show("Your Account Creation Successfully! Please Login Now.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }


            }
            sqlConnect.con.Close();
        }
    }
}
