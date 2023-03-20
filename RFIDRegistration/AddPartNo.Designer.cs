
namespace RFIDRegistration
{
    partial class AddPartNo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddPartNo));
            this.tBox_AddPart_PartNo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_AddPart_Add = new System.Windows.Forms.Button();
            this.lbl_AddPart_Register = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tBox_AddPart_PartNo
            // 
            this.tBox_AddPart_PartNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tBox_AddPart_PartNo.Location = new System.Drawing.Point(161, 43);
            this.tBox_AddPart_PartNo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tBox_AddPart_PartNo.Name = "tBox_AddPart_PartNo";
            this.tBox_AddPart_PartNo.Size = new System.Drawing.Size(226, 26);
            this.tBox_AddPart_PartNo.TabIndex = 31;
            this.tBox_AddPart_PartNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tBox_AddPart_PartNo_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label4.Location = new System.Drawing.Point(91, 46);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 20);
            this.label4.TabIndex = 30;
            this.label4.Text = "Part No :";
            // 
            // btn_AddPart_Add
            // 
            this.btn_AddPart_Add.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_AddPart_Add.Location = new System.Drawing.Point(203, 132);
            this.btn_AddPart_Add.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btn_AddPart_Add.Name = "btn_AddPart_Add";
            this.btn_AddPart_Add.Size = new System.Drawing.Size(79, 38);
            this.btn_AddPart_Add.TabIndex = 32;
            this.btn_AddPart_Add.Text = "Add";
            this.btn_AddPart_Add.UseVisualStyleBackColor = true;
            this.btn_AddPart_Add.Click += new System.EventHandler(this.btn_AddPart_Add_Click);
            // 
            // lbl_AddPart_Register
            // 
            this.lbl_AddPart_Register.AutoSize = true;
            this.lbl_AddPart_Register.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_AddPart_Register.ForeColor = System.Drawing.Color.DarkSeaGreen;
            this.lbl_AddPart_Register.Location = new System.Drawing.Point(240, 79);
            this.lbl_AddPart_Register.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbl_AddPart_Register.Name = "lbl_AddPart_Register";
            this.lbl_AddPart_Register.Size = new System.Drawing.Size(162, 17);
            this.lbl_AddPart_Register.TabIndex = 33;
            this.lbl_AddPart_Register.Text = "Registered Successfully!";
            // 
            // AddPartNo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 210);
            this.Controls.Add(this.lbl_AddPart_Register);
            this.Controls.Add(this.btn_AddPart_Add);
            this.Controls.Add(this.tBox_AddPart_PartNo);
            this.Controls.Add(this.label4);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "AddPartNo";
            this.Text = "AddPartNo";
            this.Load += new System.EventHandler(this.AddPartNo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tBox_AddPart_PartNo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_AddPart_Add;
        private System.Windows.Forms.Label lbl_AddPart_Register;
    }
}