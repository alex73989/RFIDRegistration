
namespace RFIDRegistration
{
    partial class AddSerialNo
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
            this.lbl_AddSN_WarnSN = new System.Windows.Forms.Label();
            this.lbl_AddSN_WarnPN = new System.Windows.Forms.Label();
            this.tBox_AddSN_SerialNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_AddPart_Register = new System.Windows.Forms.Label();
            this.btn_AddPart_Add = new System.Windows.Forms.Button();
            this.tBox_AddSN_PartNo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbl_AddSN_WarnSN
            // 
            this.lbl_AddSN_WarnSN.AutoSize = true;
            this.lbl_AddSN_WarnSN.Location = new System.Drawing.Point(302, 215);
            this.lbl_AddSN_WarnSN.Name = "lbl_AddSN_WarnSN";
            this.lbl_AddSN_WarnSN.Size = new System.Drawing.Size(46, 17);
            this.lbl_AddSN_WarnSN.TabIndex = 49;
            this.lbl_AddSN_WarnSN.Text = "label3";
            // 
            // lbl_AddSN_WarnPN
            // 
            this.lbl_AddSN_WarnPN.AutoSize = true;
            this.lbl_AddSN_WarnPN.Location = new System.Drawing.Point(302, 148);
            this.lbl_AddSN_WarnPN.Name = "lbl_AddSN_WarnPN";
            this.lbl_AddSN_WarnPN.Size = new System.Drawing.Size(46, 17);
            this.lbl_AddSN_WarnPN.TabIndex = 48;
            this.lbl_AddSN_WarnPN.Text = "label2";
            // 
            // tBox_AddSN_SerialNo
            // 
            this.tBox_AddSN_SerialNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tBox_AddSN_SerialNo.Location = new System.Drawing.Point(305, 177);
            this.tBox_AddSN_SerialNo.Name = "tBox_AddSN_SerialNo";
            this.tBox_AddSN_SerialNo.Size = new System.Drawing.Size(300, 30);
            this.tBox_AddSN_SerialNo.TabIndex = 47;
            this.tBox_AddSN_SerialNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tBox_AddSN_SerialNo_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label1.Location = new System.Drawing.Point(196, 180);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 25);
            this.label1.TabIndex = 46;
            this.label1.Text = "Serial No :";
            // 
            // lbl_AddPart_Register
            // 
            this.lbl_AddPart_Register.AutoSize = true;
            this.lbl_AddPart_Register.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_AddPart_Register.ForeColor = System.Drawing.Color.DarkSeaGreen;
            this.lbl_AddPart_Register.Location = new System.Drawing.Point(197, 257);
            this.lbl_AddPart_Register.Name = "lbl_AddPart_Register";
            this.lbl_AddPart_Register.Size = new System.Drawing.Size(195, 20);
            this.lbl_AddPart_Register.TabIndex = 45;
            this.lbl_AddPart_Register.Text = "Registered Successfully!";
            // 
            // btn_AddPart_Add
            // 
            this.btn_AddPart_Add.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_AddPart_Add.Location = new System.Drawing.Point(467, 293);
            this.btn_AddPart_Add.Name = "btn_AddPart_Add";
            this.btn_AddPart_Add.Size = new System.Drawing.Size(138, 47);
            this.btn_AddPart_Add.TabIndex = 44;
            this.btn_AddPart_Add.Text = "Add";
            this.btn_AddPart_Add.UseVisualStyleBackColor = true;
            this.btn_AddPart_Add.Click += new System.EventHandler(this.btn_AddPart_Add_Click);
            // 
            // tBox_AddSN_PartNo
            // 
            this.tBox_AddSN_PartNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tBox_AddSN_PartNo.Location = new System.Drawing.Point(305, 110);
            this.tBox_AddSN_PartNo.Name = "tBox_AddSN_PartNo";
            this.tBox_AddSN_PartNo.Size = new System.Drawing.Size(300, 30);
            this.tBox_AddSN_PartNo.TabIndex = 43;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label4.Location = new System.Drawing.Point(196, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 25);
            this.label4.TabIndex = 42;
            this.label4.Text = "Part No :";
            // 
            // AddSerialNo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lbl_AddSN_WarnSN);
            this.Controls.Add(this.lbl_AddSN_WarnPN);
            this.Controls.Add(this.tBox_AddSN_SerialNo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbl_AddPart_Register);
            this.Controls.Add(this.btn_AddPart_Add);
            this.Controls.Add(this.tBox_AddSN_PartNo);
            this.Controls.Add(this.label4);
            this.Name = "AddSerialNo";
            this.Text = "AddSerialNo";
            this.Load += new System.EventHandler(this.AddSerialNo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_AddSN_WarnSN;
        private System.Windows.Forms.Label lbl_AddSN_WarnPN;
        private System.Windows.Forms.TextBox tBox_AddSN_SerialNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_AddPart_Register;
        private System.Windows.Forms.Button btn_AddPart_Add;
        private System.Windows.Forms.TextBox tBox_AddSN_PartNo;
        private System.Windows.Forms.Label label4;
    }
}