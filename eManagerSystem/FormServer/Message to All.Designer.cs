namespace FormServer
{
    partial class frmMessageToAll
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
            this.lvMessage = new System.Windows.Forms.ListView();
            this.txtContentMessage = new System.Windows.Forms.TextBox();
            this.btnSendMessage = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lvMessage
            // 
            this.lvMessage.HideSelection = false;
            this.lvMessage.Location = new System.Drawing.Point(12, 12);
            this.lvMessage.Name = "lvMessage";
            this.lvMessage.Size = new System.Drawing.Size(460, 220);
            this.lvMessage.TabIndex = 0;
            this.lvMessage.UseCompatibleStateImageBehavior = false;
            // 
            // txtContentMessage
            // 
            this.txtContentMessage.Location = new System.Drawing.Point(12, 238);
            this.txtContentMessage.Multiline = true;
            this.txtContentMessage.Name = "txtContentMessage";
            this.txtContentMessage.Size = new System.Drawing.Size(344, 49);
            this.txtContentMessage.TabIndex = 1;
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Location = new System.Drawing.Point(373, 238);
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Size = new System.Drawing.Size(81, 49);
            this.btnSendMessage.TabIndex = 2;
            this.btnSendMessage.Text = "Gửi";
            this.btnSendMessage.UseVisualStyleBackColor = true;
            this.btnSendMessage.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // frmMessageToAll
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 299);
            this.Controls.Add(this.btnSendMessage);
            this.Controls.Add(this.txtContentMessage);
            this.Controls.Add(this.lvMessage);
            this.Name = "frmMessageToAll";
            this.Text = "Message to All";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvMessage;
        private System.Windows.Forms.TextBox txtContentMessage;
        private System.Windows.Forms.Button btnSendMessage;
    }
}