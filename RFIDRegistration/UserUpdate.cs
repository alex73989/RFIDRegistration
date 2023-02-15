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

    public partial class UserUpdate : Form
    {
        SqlConnection con = new SqlConnection("Data Source=MXPACALEX;Initial Catalog=Dek_MachineDB;Integrated Security=True;MultipleActiveResultSets=true");

        public string UserId { get; set; }
        public string Username { get; set; }
        public string User_Type { get; set; }
        public string User_Group_Id { get; set; }
        public string User_Group_Name { get; set; }
        public string User_Group_Desc { get; set; }
        public string Fullname { get; set; }
        public string Emp_Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Mobile_No { get; set; }
        public string Card_Id { get; set; }

        public UserUpdate()
        {
            InitializeComponent();
        }

        private void UserUpdate_Load(object sender, EventArgs e)
        {
            lbl_UpdateUser_UserID.Text = UserId;
            tBox_UpdateUser_Username.Text = Username;
            cBox_UpdateUser_UserType.Text = User_Type;
            tBox_UpdateUser_UserGroupID.Text = User_Group_Id;
            tBox_UpdateUser_UserGroupName.Text = User_Group_Name;
            tBox_UpdateUser_UserGroupDescription.Text = User_Group_Desc;
            tBox_UpdateUser_Fullname.Text = Fullname;
            tBox_UpdateUser_EmpID.Text = Emp_Id;
            tBox_UpdateUser_Email.Text = Email;
            tBox_UpdateUser_Password.Text = Password;
            tBox_UpdateUser_MobileNo.Text = Mobile_No;
            tBox_UpdateUser_CardID.Text = Card_Id;
            
        }

        private void btn_UpdateUser_Update_Click(object sender, EventArgs e)
        {
            if (tBox_UpdateUser_Username.Text != "" && cBox_UpdateUser_UserType.Text != "" && tBox_UpdateUser_UserGroupID.Text != "" && tBox_UpdateUser_UserGroupName.Text != ""
                && tBox_UpdateUser_UserGroupDescription.Text != "" && tBox_UpdateUser_Fullname.Text != "" && tBox_UpdateUser_EmpID.Text != ""
                && tBox_UpdateUser_Email.Text != "" && tBox_UpdateUser_Password.Text != "" && tBox_UpdateUser_MobileNo.Text != "" && tBox_UpdateUser_CardID.Text != "")
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("Update_UserList", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Option", "UpdateUserList");
                cmd.Parameters.AddWithValue("@User_Id", lbl_UpdateUser_UserID.Text);
                cmd.Parameters.AddWithValue("@Username", tBox_UpdateUser_Username.Text);
                cmd.Parameters.AddWithValue("@User_Type", cBox_UpdateUser_UserType.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@User_Group_ID", tBox_UpdateUser_UserGroupID.Text);
                cmd.Parameters.AddWithValue("@User_Group_Name", tBox_UpdateUser_UserGroupName.Text);
                cmd.Parameters.AddWithValue("@User_Group_Desc", tBox_UpdateUser_UserGroupDescription.Text);
                cmd.Parameters.AddWithValue("@Fullname", tBox_UpdateUser_Fullname.Text);
                cmd.Parameters.AddWithValue("@EmployeeID", tBox_UpdateUser_EmpID.Text);
                cmd.Parameters.AddWithValue("@Email", tBox_UpdateUser_Email.Text);
                cmd.Parameters.AddWithValue("@Password", tBox_UpdateUser_Password.Text);
                cmd.Parameters.AddWithValue("@Mobile_No", tBox_UpdateUser_MobileNo.Text);
                cmd.Parameters.AddWithValue("@Card_ID", tBox_UpdateUser_CardID.Text);

                SqlDataAdapter sd = new SqlDataAdapter(cmd);
                // Execute Query
                int cmdChecking = sd.SelectCommand.ExecuteNonQuery();

                if (cmdChecking > 0)
                {
                    MessageBox.Show("Updated Record Successfully");
                }

                this.Hide();
                con.Close();
            }
            else
            {
                MessageBox.Show("Make Sure All TextBox Contain Values", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_UpdateUser_Cancel_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Close();
        }
    }
}
