namespace IAModelEditor.GUI.Controls
{
    partial class MIWInitControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MIWInitControl));
            this.MIWInitDNSAModeLabel = new System.Windows.Forms.Label();
            this.MIWInitModelNameLabel = new System.Windows.Forms.Label();
            this.MIWInitDSNAMode = new System.Windows.Forms.TextBox();
            this.MIWInitModelName = new System.Windows.Forms.TextBox();
            this.MIWIntroText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // MIWInitDNSAModeLabel
            // 
            this.MIWInitDNSAModeLabel.AutoSize = true;
            this.MIWInitDNSAModeLabel.Location = new System.Drawing.Point(124, 316);
            this.MIWInitDNSAModeLabel.Name = "MIWInitDNSAModeLabel";
            this.MIWInitDNSAModeLabel.Size = new System.Drawing.Size(75, 15);
            this.MIWInitDNSAModeLabel.TabIndex = 7;
            this.MIWInitDNSAModeLabel.Text = "DSNA Mode:";
            // 
            // MIWInitModelNameLabel
            // 
            this.MIWInitModelNameLabel.AutoSize = true;
            this.MIWInitModelNameLabel.Location = new System.Drawing.Point(124, 248);
            this.MIWInitModelNameLabel.Name = "MIWInitModelNameLabel";
            this.MIWInitModelNameLabel.Size = new System.Drawing.Size(79, 15);
            this.MIWInitModelNameLabel.TabIndex = 8;
            this.MIWInitModelNameLabel.Text = "Model Name:";
            // 
            // MIWInitDSNAMode
            // 
            this.MIWInitDSNAMode.Location = new System.Drawing.Point(124, 334);
            this.MIWInitDSNAMode.Name = "MIWInitDSNAMode";
            this.MIWInitDSNAMode.Size = new System.Drawing.Size(392, 23);
            this.MIWInitDSNAMode.TabIndex = 5;
            this.MIWInitDSNAMode.Text = "DRAW";
            // 
            // MIWInitModelName
            // 
            this.MIWInitModelName.Location = new System.Drawing.Point(124, 266);
            this.MIWInitModelName.Name = "MIWInitModelName";
            this.MIWInitModelName.Size = new System.Drawing.Size(392, 23);
            this.MIWInitModelName.TabIndex = 6;
            this.MIWInitModelName.Text = "COS_000";
            // 
            // MIWIntroText
            // 
            this.MIWIntroText.AutoSize = true;
            this.MIWIntroText.Location = new System.Drawing.Point(124, 123);
            this.MIWIntroText.Name = "MIWIntroText";
            this.MIWIntroText.Size = new System.Drawing.Size(392, 90);
            this.MIWIntroText.TabIndex = 4;
            this.MIWIntroText.Text = resources.GetString("MIWIntroText.Text");
            // 
            // MIWInitControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MIWInitDNSAModeLabel);
            this.Controls.Add(this.MIWInitModelNameLabel);
            this.Controls.Add(this.MIWInitDSNAMode);
            this.Controls.Add(this.MIWInitModelName);
            this.Controls.Add(this.MIWIntroText);
            this.Location = new System.Drawing.Point(72, 12);
            this.Name = "MIWInitControl";
            this.Size = new System.Drawing.Size(640, 480);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label MIWInitDNSAModeLabel;
        private Label MIWInitModelNameLabel;
        private Label MIWIntroText;
        public TextBox MIWInitDSNAMode;
        public TextBox MIWInitModelName;
    }
}
