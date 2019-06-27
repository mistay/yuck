namespace yuck
{
    partial class MatrixRoomProperties
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
            this.direct = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtoomID = new System.Windows.Forms.TextBox();
            this.txtRoomName = new System.Windows.Forms.TextBox();
            this.chkDirect = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // direct
            // 
            this.direct.AutoSize = true;
            this.direct.Location = new System.Drawing.Point(52, 56);
            this.direct.Name = "direct";
            this.direct.Size = new System.Drawing.Size(43, 17);
            this.direct.TabIndex = 0;
            this.direct.Text = "direct";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(52, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "id";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(52, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "name";
            // 
            // txtoomID
            // 
            this.txtoomID.Location = new System.Drawing.Point(143, 85);
            this.txtoomID.Name = "txtoomID";
            this.txtoomID.ReadOnly = true;
            this.txtoomID.Size = new System.Drawing.Size(509, 22);
            this.txtoomID.TabIndex = 4;
            // 
            // txtRoomName
            // 
            this.txtRoomName.Location = new System.Drawing.Point(143, 113);
            this.txtRoomName.Name = "txtRoomName";
            this.txtRoomName.ReadOnly = true;
            this.txtRoomName.Size = new System.Drawing.Size(509, 22);
            this.txtRoomName.TabIndex = 5;
            // 
            // chkDirect
            // 
            this.chkDirect.AutoSize = true;
            this.chkDirect.Enabled = false;
            this.chkDirect.Location = new System.Drawing.Point(143, 56);
            this.chkDirect.Name = "chkDirect";
            this.chkDirect.Size = new System.Drawing.Size(18, 17);
            this.chkDirect.TabIndex = 6;
            this.chkDirect.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(540, 174);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 42);
            this.button1.TabIndex = 7;
            this.button1.Text = "&Close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // MatrixRoomProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 264);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.chkDirect);
            this.Controls.Add(this.txtRoomName);
            this.Controls.Add(this.txtoomID);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.direct);
            this.Name = "MatrixRoomProperties";
            this.Text = "Matrix Room Properties";
            this.Load += new System.EventHandler(this.MatrixRoomProperties_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label direct;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtoomID;
        private System.Windows.Forms.TextBox txtRoomName;
        private System.Windows.Forms.CheckBox chkDirect;
        private System.Windows.Forms.Button button1;
    }
}