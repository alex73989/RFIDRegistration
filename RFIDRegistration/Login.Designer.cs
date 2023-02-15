
namespace RFIDRegistration
{
    partial class Login
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tBox_Login_Username = new System.Windows.Forms.TextBox();
            this.tBox_Login_Password = new System.Windows.Forms.TextBox();
            this.btn_LoginMain = new System.Windows.Forms.Button();
            this.lbl_LogUser_WarnUsername = new System.Windows.Forms.Label();
            this.lbl_LogUser_WarnPassword = new System.Windows.Forms.Label();
            this.cBox_ShowHide = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(54, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(164, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter Username :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(54, 154);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(160, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "Enter Password :";
            // 
            // tBox_Login_Username
            // 
            this.tBox_Login_Username.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tBox_Login_Username.Location = new System.Drawing.Point(234, 90);
            this.tBox_Login_Username.Name = "tBox_Login_Username";
            this.tBox_Login_Username.Size = new System.Drawing.Size(302, 30);
            this.tBox_Login_Username.TabIndex = 2;
            this.tBox_Login_Username.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tBox_Login_Username_KeyDown);
            // 
            // tBox_Login_Password
            // 
            this.tBox_Login_Password.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tBox_Login_Password.Location = new System.Drawing.Point(234, 151);
            this.tBox_Login_Password.Multiline = true;
            this.tBox_Login_Password.Name = "tBox_Login_Password";
            this.tBox_Login_Password.Size = new System.Drawing.Size(302, 30);
            this.tBox_Login_Password.TabIndex = 3;
            this.tBox_Login_Password.WordWrap = false;
            this.tBox_Login_Password.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tBox_Login_Password_KeyDown);
            // 
            // btn_LoginMain
            // 
            this.btn_LoginMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_LoginMain.Location = new System.Drawing.Point(219, 252);
            this.btn_LoginMain.Name = "btn_LoginMain";
            this.btn_LoginMain.Size = new System.Drawing.Size(127, 49);
            this.btn_LoginMain.TabIndex = 8;
            this.btn_LoginMain.Text = "Login";
            this.btn_LoginMain.UseVisualStyleBackColor = true;
            this.btn_LoginMain.Click += new System.EventHandler(this.btn_LoginMain_Click);
            // 
            // lbl_LogUser_WarnUsername
            // 
            this.lbl_LogUser_WarnUsername.AutoSize = true;
            this.lbl_LogUser_WarnUsername.Location = new System.Drawing.Point(231, 123);
            this.lbl_LogUser_WarnUsername.Name = "lbl_LogUser_WarnUsername";
            this.lbl_LogUser_WarnUsername.Size = new System.Drawing.Size(291, 17);
            this.lbl_LogUser_WarnUsername.TabIndex = 6;
            this.lbl_LogUser_WarnUsername.Text = "Please Do Not Leave Blank at Column above";
            // 
            // lbl_LogUser_WarnPassword
            // 
            this.lbl_LogUser_WarnPassword.AutoSize = true;
            this.lbl_LogUser_WarnPassword.Location = new System.Drawing.Point(231, 184);
            this.lbl_LogUser_WarnPassword.Name = "lbl_LogUser_WarnPassword";
            this.lbl_LogUser_WarnPassword.Size = new System.Drawing.Size(291, 17);
            this.lbl_LogUser_WarnPassword.TabIndex = 7;
            this.lbl_LogUser_WarnPassword.Text = "Please Do Not Leave Blank at Column above";
            // 
            // cBox_ShowHide
            // 
            this.cBox_ShowHide.AutoSize = true;
            this.cBox_ShowHide.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cBox_ShowHide.Location = new System.Drawing.Point(551, 159);
            this.cBox_ShowHide.Name = "cBox_ShowHide";
            this.cBox_ShowHide.Size = new System.Drawing.Size(18, 17);
            this.cBox_ShowHide.TabIndex = 4;
            this.cBox_ShowHide.UseVisualStyleBackColor = true;
            this.cBox_ShowHide.CheckedChanged += new System.EventHandler(this.cBox_ShowHide_CheckedChanged);
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(608, 368);
            this.Controls.Add(this.cBox_ShowHide);
            this.Controls.Add(this.lbl_LogUser_WarnPassword);
            this.Controls.Add(this.lbl_LogUser_WarnUsername);
            this.Controls.Add(this.btn_LoginMain);
            this.Controls.Add(this.tBox_Login_Password);
            this.Controls.Add(this.tBox_Login_Username);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Login_FormClosing);
            this.Load += new System.EventHandler(this.Login_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tBox_Login_Username;
        private System.Windows.Forms.TextBox tBox_Login_Password;
        private System.Windows.Forms.Button btn_LoginMain;
        private System.Windows.Forms.Label lbl_LogUser_WarnUsername;
        private System.Windows.Forms.Label lbl_LogUser_WarnPassword;
        private System.Windows.Forms.CheckBox cBox_ShowHide;
    }
}