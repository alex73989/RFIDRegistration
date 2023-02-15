
namespace RFIDRegistration
{
    partial class RegistrationDevice
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cBox_Dev_EmpName = new System.Windows.Forms.ComboBox();
            this.cBox_Dev_CommType = new System.Windows.Forms.ComboBox();
            this.tBox_Dev_MoreDesc = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tBox_Dev_IPAddress = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.tBox_Dev_ProductName = new System.Windows.Forms.TextBox();
            this.tBox_Dev_Model = new System.Windows.Forms.TextBox();
            this.tBox_Dev_ID = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnDev_SaveRecords = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cBox_Dev_EmpName
            // 
            this.cBox_Dev_EmpName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cBox_Dev_EmpName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cBox_Dev_EmpName.FormattingEnabled = true;
            this.cBox_Dev_EmpName.Location = new System.Drawing.Point(213, 37);
            this.cBox_Dev_EmpName.Name = "cBox_Dev_EmpName";
            this.cBox_Dev_EmpName.Size = new System.Drawing.Size(360, 33);
            this.cBox_Dev_EmpName.TabIndex = 36;
            // 
            // cBox_Dev_CommType
            // 
            this.cBox_Dev_CommType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cBox_Dev_CommType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cBox_Dev_CommType.FormattingEnabled = true;
            this.cBox_Dev_CommType.Items.AddRange(new object[] {
            "WiFISTA",
            "ETHERNET"});
            this.cBox_Dev_CommType.Location = new System.Drawing.Point(213, 237);
            this.cBox_Dev_CommType.Name = "cBox_Dev_CommType";
            this.cBox_Dev_CommType.Size = new System.Drawing.Size(360, 33);
            this.cBox_Dev_CommType.TabIndex = 35;
            // 
            // tBox_Dev_MoreDesc
            // 
            this.tBox_Dev_MoreDesc.Location = new System.Drawing.Point(213, 344);
            this.tBox_Dev_MoreDesc.Multiline = true;
            this.tBox_Dev_MoreDesc.Name = "tBox_Dev_MoreDesc";
            this.tBox_Dev_MoreDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tBox_Dev_MoreDesc.Size = new System.Drawing.Size(360, 156);
            this.tBox_Dev_MoreDesc.TabIndex = 34;
            this.tBox_Dev_MoreDesc.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tBox_Dev_MoreDesc_KeyDown);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(40, 340);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(170, 25);
            this.label12.TabIndex = 33;
            this.label12.Text = "More Description :";
            // 
            // tBox_Dev_IPAddress
            // 
            this.tBox_Dev_IPAddress.Enabled = false;
            this.tBox_Dev_IPAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tBox_Dev_IPAddress.Location = new System.Drawing.Point(213, 287);
            this.tBox_Dev_IPAddress.Name = "tBox_Dev_IPAddress";
            this.tBox_Dev_IPAddress.Size = new System.Drawing.Size(360, 30);
            this.tBox_Dev_IPAddress.TabIndex = 32;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(40, 290);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(153, 25);
            this.label11.TabIndex = 31;
            this.label11.Text = "Device IP Addr :";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(40, 240);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(131, 25);
            this.label10.TabIndex = 30;
            this.label10.Text = "Comm Type :";
            // 
            // tBox_Dev_ProductName
            // 
            this.tBox_Dev_ProductName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tBox_Dev_ProductName.Location = new System.Drawing.Point(213, 187);
            this.tBox_Dev_ProductName.Name = "tBox_Dev_ProductName";
            this.tBox_Dev_ProductName.Size = new System.Drawing.Size(360, 30);
            this.tBox_Dev_ProductName.TabIndex = 29;
            // 
            // tBox_Dev_Model
            // 
            this.tBox_Dev_Model.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tBox_Dev_Model.Location = new System.Drawing.Point(213, 137);
            this.tBox_Dev_Model.Name = "tBox_Dev_Model";
            this.tBox_Dev_Model.Size = new System.Drawing.Size(360, 30);
            this.tBox_Dev_Model.TabIndex = 28;
            // 
            // tBox_Dev_ID
            // 
            this.tBox_Dev_ID.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tBox_Dev_ID.Location = new System.Drawing.Point(213, 87);
            this.tBox_Dev_ID.Name = "tBox_Dev_ID";
            this.tBox_Dev_ID.Size = new System.Drawing.Size(360, 30);
            this.tBox_Dev_ID.TabIndex = 27;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(40, 190);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(147, 25);
            this.label6.TabIndex = 26;
            this.label6.Text = "Product Name :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(40, 140);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 25);
            this.label7.TabIndex = 25;
            this.label7.Text = "Model :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(40, 90);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(107, 25);
            this.label8.TabIndex = 24;
            this.label8.Text = "Device ID :";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(40, 40);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(167, 25);
            this.label9.TabIndex = 23;
            this.label9.Text = "Employee Name :";
            // 
            // btnDev_SaveRecords
            // 
            this.btnDev_SaveRecords.Location = new System.Drawing.Point(213, 520);
            this.btnDev_SaveRecords.Name = "btnDev_SaveRecords";
            this.btnDev_SaveRecords.Size = new System.Drawing.Size(360, 38);
            this.btnDev_SaveRecords.TabIndex = 21;
            this.btnDev_SaveRecords.Text = "Save Records";
            this.btnDev_SaveRecords.UseVisualStyleBackColor = true;
            this.btnDev_SaveRecords.Click += new System.EventHandler(this.btnDev_SaveRecords_Click);
            // 
            // RegistrationDevice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 618);
            this.Controls.Add(this.cBox_Dev_EmpName);
            this.Controls.Add(this.cBox_Dev_CommType);
            this.Controls.Add(this.tBox_Dev_MoreDesc);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.tBox_Dev_IPAddress);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.tBox_Dev_ProductName);
            this.Controls.Add(this.tBox_Dev_Model);
            this.Controls.Add(this.tBox_Dev_ID);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnDev_SaveRecords);
            this.Name = "RegistrationDevice";
            this.Text = "RegistrationDevice";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cBox_Dev_EmpName;
        private System.Windows.Forms.ComboBox cBox_Dev_CommType;
        private System.Windows.Forms.TextBox tBox_Dev_MoreDesc;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox tBox_Dev_IPAddress;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tBox_Dev_ProductName;
        private System.Windows.Forms.TextBox tBox_Dev_Model;
        private System.Windows.Forms.TextBox tBox_Dev_ID;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnDev_SaveRecords;
    }
}