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
    public partial class RegistrationUser : Form
    {
        string RequestFillInTheBlank = "Please Do Not Leave Blank at Column above!";
        SqlConnection con = new SqlConnection("Data Source=MXPACALEX;Initial Catalog=Dek_MachineDB;Integrated Security=True");
        public SqlCommand cmd;
        SqlDataReader dr;

        public RegistrationUser()
        {
            InitializeComponent();
        }

        private void RegistrationUser_Load(object sender, EventArgs e)
        { 
            lbl_RegUser_WarnUsername.Text = "";
            lbl_RegUser_WarnFullname.Text = "";
            lbl_RegUser_WarnPassword.Text = "";
            lbl_RegUser_WarnEmpID.Text = "";
            lbl_RegUser_WarnEmailAddr.Text = "";
            lbl_RegUser_WarnContactNo.Text = "";
            lbl_RegUser_WarnCardID.Text = "";
            lbl_RegUser_WarnUserType.Text = "";
            lbl_RegUser_WarnGroupID.Text = "";
            lbl_RegUser_WarnGroupName.Text = "";
            lbl_RegUser_WarnGroupDesc.Text = "";
        }

        public void TextRefreshRegUser()
        {
            tBox_RegUser_Username.Text = "";
            tBox_RegUser_Fullname.Text = "";
            tBox_RegUser_Password.Text = "";
            tBox_RegUser_EmpID.Text = "";
            tBox_RegUser_EmailAddr.Text = "";
            tBox_RegUser_ContactNo.Text = "";
            tBox_RegUser_CardID.Text = "";
            cBox_RegUser_UserType.Text = "";
            tBox_RegUser_GroupID.Text = "";
            tBox_RegUser_GroupName.Text = "";
            tBox_RegUser_GroupDesc.Text = "";
        }

        private void btn_RegisterNow_Click(object sender, EventArgs e)
        {
            if (tBox_RegUser_Username.Text != "")
            {
                if (tBox_RegUser_Fullname.Text != "")
                {
                    if (tBox_RegUser_Password.Text != "")
                    {
                        if (tBox_RegUser_EmpID.Text != "")
                        {
                            if (tBox_RegUser_EmailAddr.Text != "")
                            {
                                if (tBox_RegUser_ContactNo.Text != "")
                                {
                                    if (tBox_RegUser_CardID.Text != "")
                                    {
                                        if (cBox_RegUser_UserType.Text != "")
                                        {
                                            if (tBox_RegUser_GroupID.Text != "")
                                            {
                                                if (tBox_RegUser_GroupName.Text != "")
                                                {
                                                    if (tBox_RegUser_GroupDesc.Text != "")
                                                    {
                                                        Insert_RegistrationUser();
                                                        TextRefreshRegUser();
                                                    }
                                                    else if (tBox_RegUser_GroupDesc.Text == "")
                                                    {
                                                        lbl_RegUser_WarnGroupDesc.Text = RequestFillInTheBlank;
                                                    }
                                                }
                                                else if (tBox_RegUser_GroupName.Text == "")
                                                {
                                                    lbl_RegUser_WarnGroupName.Text = RequestFillInTheBlank;
                                                }
                                            }
                                            else if (tBox_RegUser_GroupID.Text == "")
                                            {
                                                lbl_RegUser_WarnGroupID.Text = RequestFillInTheBlank;
                                            }
                                        }
                                        else if (cBox_RegUser_UserType.Text == "")
                                        {
                                            lbl_RegUser_WarnUserType.Text = RequestFillInTheBlank;
                                        }
                                    }
                                    else if (tBox_RegUser_CardID.Text == "")
                                    {
                                        lbl_RegUser_WarnCardID.Text = RequestFillInTheBlank;
                                    }
                                }
                                else if (tBox_RegUser_ContactNo.Text == "")
                                {
                                    lbl_RegUser_WarnContactNo.Text = RequestFillInTheBlank;
                                }
                            }
                            else if (tBox_RegUser_EmailAddr.Text == "")
                            {
                                lbl_RegUser_WarnEmailAddr.Text = RequestFillInTheBlank;
                            }
                        }
                        else if (tBox_RegUser_EmpID.Text == "")
                        {
                            lbl_RegUser_WarnEmpID.Text = RequestFillInTheBlank;
                        }
                    }
                    else if (tBox_RegUser_Password.Text == "")
                    {
                        lbl_RegUser_WarnPassword.Text = RequestFillInTheBlank;
                    }
                }
                else if (tBox_RegUser_Fullname.Text == "")
                {
                    lbl_RegUser_WarnFullname.Text = RequestFillInTheBlank;
                }
            }
            else if (tBox_RegUser_Username.Text == "")
            {
                lbl_RegUser_WarnUsername.Text = RequestFillInTheBlank;
            }
        }

        public void Insert_RegistrationUser()
        {
            
            con.Open();

            cmd = new SqlCommand("SELECT * FROM users_tb WHERE Username = '" + tBox_RegUser_Username + "'", con);
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                dr.Close();
                MessageBox.Show("Username Already Exists, Please Try Again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                dr.Close();
                
                if(con.State == System.Data.ConnectionState.Open)
                {
                    SqlDataAdapter dataAdapter_RegUser = new SqlDataAdapter("Insert_RegUserList", con);

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
            con.Close();
        }
    }
}
