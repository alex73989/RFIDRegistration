
namespace RFIDRegistration
{
    partial class RegistrationTag
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
            this.tBox_Tag_FunctionUsed = new System.Windows.Forms.TextBox();
            this.label40 = new System.Windows.Forms.Label();
            this.cBox_Tag_Location = new System.Windows.Forms.ComboBox();
            this.label42 = new System.Windows.Forms.Label();
            this.cBox_Tag_EmpName = new System.Windows.Forms.ComboBox();
            this.tBox_Tag_MoreDesc = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.btnScan = new System.Windows.Forms.Button();
            this.tBox_Tag_EPCNo = new System.Windows.Forms.TextBox();
            this.tBox_Tag_PartNo = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnTag_ClearRecords = new System.Windows.Forms.Button();
            this.btnTag_SaveRecords = new System.Windows.Forms.Button();
            this.cBox_Tag_SerialNo = new System.Windows.Forms.ComboBox();
            this.lbl_Status = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tBox_Tag_FunctionUsed
            // 
            this.tBox_Tag_FunctionUsed.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tBox_Tag_FunctionUsed.Location = new System.Drawing.Point(223, 287);
            this.tBox_Tag_FunctionUsed.Name = "tBox_Tag_FunctionUsed";
            this.tBox_Tag_FunctionUsed.Size = new System.Drawing.Size(300, 30);
            this.tBox_Tag_FunctionUsed.TabIndex = 37;
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label40.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label40.Location = new System.Drawing.Point(40, 290);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(149, 25);
            this.label40.TabIndex = 36;
            this.label40.Text = "Function Used :";
            // 
            // cBox_Tag_Location
            // 
            this.cBox_Tag_Location.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cBox_Tag_Location.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cBox_Tag_Location.FormattingEnabled = true;
            this.cBox_Tag_Location.Items.AddRange(new object[] {
            "None",
            "Front Door",
            "Back Door",
            "Left Door",
            "Right Door"});
            this.cBox_Tag_Location.Location = new System.Drawing.Point(223, 237);
            this.cBox_Tag_Location.Name = "cBox_Tag_Location";
            this.cBox_Tag_Location.Size = new System.Drawing.Size(300, 33);
            this.cBox_Tag_Location.TabIndex = 35;
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label42.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label42.Location = new System.Drawing.Point(40, 240);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(97, 25);
            this.label42.TabIndex = 34;
            this.label42.Text = "Location :";
            // 
            // cBox_Tag_EmpName
            // 
            this.cBox_Tag_EmpName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cBox_Tag_EmpName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cBox_Tag_EmpName.FormattingEnabled = true;
            this.cBox_Tag_EmpName.Location = new System.Drawing.Point(223, 37);
            this.cBox_Tag_EmpName.Name = "cBox_Tag_EmpName";
            this.cBox_Tag_EmpName.Size = new System.Drawing.Size(300, 33);
            this.cBox_Tag_EmpName.TabIndex = 33;
            // 
            // tBox_Tag_MoreDesc
            // 
            this.tBox_Tag_MoreDesc.Location = new System.Drawing.Point(223, 338);
            this.tBox_Tag_MoreDesc.Multiline = true;
            this.tBox_Tag_MoreDesc.Name = "tBox_Tag_MoreDesc";
            this.tBox_Tag_MoreDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tBox_Tag_MoreDesc.Size = new System.Drawing.Size(300, 185);
            this.tBox_Tag_MoreDesc.TabIndex = 32;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label13.Location = new System.Drawing.Point(40, 371);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(170, 25);
            this.label13.TabIndex = 31;
            this.label13.Text = "More Description :";
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(538, 87);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(86, 30);
            this.btnScan.TabIndex = 24;
            this.btnScan.Text = "Scan";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // tBox_Tag_EPCNo
            // 
            this.tBox_Tag_EPCNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tBox_Tag_EPCNo.Location = new System.Drawing.Point(223, 87);
            this.tBox_Tag_EPCNo.Name = "tBox_Tag_EPCNo";
            this.tBox_Tag_EPCNo.Size = new System.Drawing.Size(300, 30);
            this.tBox_Tag_EPCNo.TabIndex = 30;
            // 
            // tBox_Tag_PartNo
            // 
            this.tBox_Tag_PartNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tBox_Tag_PartNo.Location = new System.Drawing.Point(223, 137);
            this.tBox_Tag_PartNo.Name = "tBox_Tag_PartNo";
            this.tBox_Tag_PartNo.Size = new System.Drawing.Size(300, 30);
            this.tBox_Tag_PartNo.TabIndex = 29;
            this.tBox_Tag_PartNo.TextChanged += new System.EventHandler(this.tBox_Tag_PartNo_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label5.Location = new System.Drawing.Point(40, 90);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(134, 25);
            this.label5.TabIndex = 27;
            this.label5.Text = "Tag EPC No :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label4.Location = new System.Drawing.Point(40, 140);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 25);
            this.label4.TabIndex = 26;
            this.label4.Text = "Part No :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label3.Location = new System.Drawing.Point(40, 190);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 25);
            this.label3.TabIndex = 25;
            this.label3.Text = "Serial No :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label2.Location = new System.Drawing.Point(40, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(167, 25);
            this.label2.TabIndex = 23;
            this.label2.Text = "Employee Name :";
            // 
            // btnTag_ClearRecords
            // 
            this.btnTag_ClearRecords.ForeColor = System.Drawing.SystemColors.Desktop;
            this.btnTag_ClearRecords.Location = new System.Drawing.Point(368, 605);
            this.btnTag_ClearRecords.Name = "btnTag_ClearRecords";
            this.btnTag_ClearRecords.Size = new System.Drawing.Size(145, 55);
            this.btnTag_ClearRecords.TabIndex = 22;
            this.btnTag_ClearRecords.Text = "Clear Records";
            this.btnTag_ClearRecords.UseVisualStyleBackColor = true;
            this.btnTag_ClearRecords.Click += new System.EventHandler(this.btnTag_ClearRecords_Click);
            // 
            // btnTag_SaveRecords
            // 
            this.btnTag_SaveRecords.ForeColor = System.Drawing.SystemColors.Desktop;
            this.btnTag_SaveRecords.Location = new System.Drawing.Point(213, 605);
            this.btnTag_SaveRecords.Name = "btnTag_SaveRecords";
            this.btnTag_SaveRecords.Size = new System.Drawing.Size(128, 55);
            this.btnTag_SaveRecords.TabIndex = 21;
            this.btnTag_SaveRecords.Text = "Save Records";
            this.btnTag_SaveRecords.UseVisualStyleBackColor = true;
            this.btnTag_SaveRecords.Click += new System.EventHandler(this.btnTag_SaveRecords_Click);
            // 
            // cBox_Tag_SerialNo
            // 
            this.cBox_Tag_SerialNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cBox_Tag_SerialNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cBox_Tag_SerialNo.FormattingEnabled = true;
            this.cBox_Tag_SerialNo.Location = new System.Drawing.Point(223, 187);
            this.cBox_Tag_SerialNo.Name = "cBox_Tag_SerialNo";
            this.cBox_Tag_SerialNo.Size = new System.Drawing.Size(300, 33);
            this.cBox_Tag_SerialNo.TabIndex = 38;
            // 
            // lbl_Status
            // 
            this.lbl_Status.AutoSize = true;
            this.lbl_Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Status.Location = new System.Drawing.Point(210, 548);
            this.lbl_Status.Name = "lbl_Status";
            this.lbl_Status.Size = new System.Drawing.Size(53, 20);
            this.lbl_Status.TabIndex = 39;
            this.lbl_Status.Text = "label1";
            // 
            // RegistrationTag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 672);
            this.Controls.Add(this.lbl_Status);
            this.Controls.Add(this.cBox_Tag_SerialNo);
            this.Controls.Add(this.tBox_Tag_FunctionUsed);
            this.Controls.Add(this.label40);
            this.Controls.Add(this.cBox_Tag_Location);
            this.Controls.Add(this.label42);
            this.Controls.Add(this.cBox_Tag_EmpName);
            this.Controls.Add(this.tBox_Tag_MoreDesc);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.btnScan);
            this.Controls.Add(this.tBox_Tag_EPCNo);
            this.Controls.Add(this.tBox_Tag_PartNo);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnTag_ClearRecords);
            this.Controls.Add(this.btnTag_SaveRecords);
            this.Name = "RegistrationTag";
            this.Text = "RegistrationTag";
            this.Load += new System.EventHandler(this.RegistrationTag_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox tBox_Tag_FunctionUsed;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.ComboBox cBox_Tag_Location;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.ComboBox cBox_Tag_EmpName;
        private System.Windows.Forms.TextBox tBox_Tag_MoreDesc;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.TextBox tBox_Tag_EPCNo;
        private System.Windows.Forms.TextBox tBox_Tag_PartNo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnTag_ClearRecords;
        private System.Windows.Forms.Button btnTag_SaveRecords;
        private System.Windows.Forms.ComboBox cBox_Tag_SerialNo;
        private System.Windows.Forms.Label lbl_Status;
    }
}