namespace USF4_Music_Mod_Tool
{
    partial class CSBExplained
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
            this.rtfBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // rtfBox1
            // 
            this.rtfBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtfBox1.HideSelection = false;
            this.rtfBox1.Location = new System.Drawing.Point(0, 0);
            this.rtfBox1.Name = "rtfBox1";
            this.rtfBox1.Size = new System.Drawing.Size(799, 452);
            this.rtfBox1.TabIndex = 0;
            this.rtfBox1.Text = "";
            this.rtfBox1.TextChanged += new System.EventHandler(this.RtfBox1_TextChanged);
            // 
            // CSBExplained
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.rtfBox1);
            this.Name = "CSBExplained";
            this.Text = "More information on CSB Files";
            this.Load += new System.EventHandler(this.CSBExplained_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtfBox1;
    }
}