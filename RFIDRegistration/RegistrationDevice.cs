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
    public partial class RegistrationDevice : Form
    {
        // Address of SQL Server and Database
        SqlConnect sqlConnect = new SqlConnect();

        public RegistrationDevice()
        {
            InitializeComponent();
            sqlConnect.Connection();
        }

        public void DeviceTextRefresh()
        {
            tBox_Dev_ID.Text = string.Empty;
            cBox_Dev_EmpName.Text = string.Empty;
            tBox_Dev_Model.Text = string.Empty;
            tBox_Dev_ProductName.Text = string.Empty;
            cBox_Dev_CommType.Text = string.Empty;
            tBox_Dev_IPAddress.Text = string.Empty;
            tBox_Dev_MoreDesc.Text = string.Empty;
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
                sqlCommand.Parameters.AddWithValue("@Username", string.Empty);
                sqlCommand.Parameters.AddWithValue("@Password", string.Empty);
                sqlCommand.Parameters.AddWithValue("@Fullname", string.Empty);
                sqlCommand.Parameters.AddWithValue("@User_Type", string.Empty);
                sqlCommand.Parameters.AddWithValue("@Employee_ID", string.Empty);

                SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
                DataSet ds = new DataSet();
                da.Fill(ds);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    cBox_Dev_EmpName.Items.Add(ds.Tables[0].Rows[i][6].ToString());
                }
            }
            sqlConnect.con.Close();
        }

        private void btnDev_SaveRecords_Click(object sender, EventArgs e)
        {
            if (cBox_Dev_EmpName.Text != string.Empty && tBox_Dev_ID.Text != string.Empty && tBox_Dev_Model.Text != string.Empty
                && tBox_Dev_ProductName.Text != string.Empty && cBox_Dev_CommType.Text != string.Empty && tBox_Dev_IPAddress.Text != string.Empty && tBox_Dev_MoreDesc.Text != string.Empty)
            {
                if (PublicFunction.DuplicateDeviceChecking(tBox_Dev_ID.Text, "Dev_ID", "@Dev_ID"))
                {
                    //DuplicateMessage("Device ID", tBox_Dev_ID.Text);
                    MessageBox.Show("Device ID duplication, Please retry!");
                }
                else if (PublicFunction.DuplicateDeviceChecking(tBox_Dev_Model.Text, "Model_Name", "@Model_Name"))
                {
                    //DuplicateMessage("Model Name", tBox_Dev_Model.Text);
                    MessageBox.Show("Model Name duplication, Please retry!");
                }
                else if (PublicFunction.DuplicateDeviceChecking(tBox_Dev_ProductName.Text, "Product_Name", "@Product_Name"))
                {
                    //DuplicateMessage("Product Name", tBox_Dev_ProductName.Text);
                    MessageBox.Show("Product Name duplication, Please retry!");
                }
                else if (PublicFunction.DuplicateDeviceChecking(tBox_Dev_IPAddress.Text, "Device_IP_Address", "@Device_IP_Address"))
                {
                    //DuplicateMessage("Device IP Address", tBox_Dev_IPAddress.Text);
                    MessageBox.Show("Device IP duplication, Please retry!");
                }
                else
                {
                    sqlConnect.con.Open();

                    SqlCommand sqlCommand = new SqlCommand("Select_UsersTB", sqlConnect.con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    sqlCommand.Parameters.AddWithValue("@Option", "GetListFullname");
                    sqlCommand.Parameters.AddWithValue("@Username", string.Empty);
                    sqlCommand.Parameters.AddWithValue("@Password", string.Empty);
                    sqlCommand.Parameters.AddWithValue("@Fullname", cBox_Dev_EmpName.SelectedItem.ToString());
                    sqlCommand.Parameters.AddWithValue("@User_Type", string.Empty);
                    sqlCommand.Parameters.AddWithValue("@Employee_ID", string.Empty);

                    string UserIdRead = string.Empty;
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        UserIdRead = reader.GetValue(0).ToString();
                    }

                    if (sqlConnect.con.State == System.Data.ConnectionState.Open)
                    {
                        SqlDataAdapter dataAdapter_RegDev = new SqlDataAdapter("Insert_RegisterDeviceList", sqlConnect.con);

                        dataAdapter_RegDev.SelectCommand.CommandType = CommandType.StoredProcedure;
                        dataAdapter_RegDev.SelectCommand.Parameters.Add("@Device_ID", SqlDbType.VarChar).Value = tBox_Dev_ID.Text;
                        dataAdapter_RegDev.SelectCommand.Parameters.Add("@User_ID", SqlDbType.VarChar).Value = UserIdRead;
                        dataAdapter_RegDev.SelectCommand.Parameters.Add("@Employee_Name", SqlDbType.VarChar).Value = cBox_Dev_EmpName.SelectedItem.ToString();
                        dataAdapter_RegDev.SelectCommand.Parameters.Add("@Model_Name", SqlDbType.VarChar).Value = tBox_Dev_Model.Text;
                        dataAdapter_RegDev.SelectCommand.Parameters.Add("@Product_Name", SqlDbType.VarChar).Value = tBox_Dev_ProductName.Text;
                        dataAdapter_RegDev.SelectCommand.Parameters.Add("@Communication_Type", SqlDbType.VarChar).Value = cBox_Dev_CommType.SelectedItem.ToString();
                        dataAdapter_RegDev.SelectCommand.Parameters.Add("@Device_IP_Address", SqlDbType.VarChar).Value = tBox_Dev_IPAddress.Text;
                        dataAdapter_RegDev.SelectCommand.Parameters.Add("@More_Desc", SqlDbType.VarChar).Value = tBox_Dev_MoreDesc.Text;

                        int cmdChecking = dataAdapter_RegDev.SelectCommand.ExecuteNonQuery();

                        if (cmdChecking > 0)
                        {
                            //AddToShow("Device Data Insert Successfully!");
                            MessageBox.Show("Device Data Insert Successfully!");
                            sqlConnect.con.Close();
                            DeviceTextRefresh();
                        }
                    }
                    else
                    {
                        sqlConnect.con.Close();
                        //AddToShow("Connection Failure To Database, Please Try Again.");
                        MessageBox.Show("Connection Failure To Database, Please Try Again.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please Fill In All The blank value at Device List! Thanks");
                //AddToShow("Not Accepted Value with Nulls, Please Try Again..");
            }
        }

        private void tBox_Dev_MoreDesc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnDev_SaveRecords_Click(this, new EventArgs());
            }
        }
    }
}
