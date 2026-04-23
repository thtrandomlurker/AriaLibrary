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
            MIWInitDNSAModeLabel = new Label();
            MIWInitModelNameLabel = new Label();
            MIWInitDSNAMode = new TextBox();
            MIWInitModelName = new TextBox();
            MIWIntroText = new Label();
            SuspendLayout();
            // 
            // MIWInitDNSAModeLabel
            // 
            MIWInitDNSAModeLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MIWInitDNSAModeLabel.AutoSize = true;
            MIWInitDNSAModeLabel.Location = new Point(124, 316);
            MIWInitDNSAModeLabel.Name = "MIWInitDNSAModeLabel";
            MIWInitDNSAModeLabel.Size = new Size(75, 15);
            MIWInitDNSAModeLabel.TabIndex = 7;
            MIWInitDNSAModeLabel.Text = "DSNA Mode:";
            // 
            // MIWInitModelNameLabel
            // 
            MIWInitModelNameLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MIWInitModelNameLabel.AutoSize = true;
            MIWInitModelNameLabel.Location = new Point(124, 248);
            MIWInitModelNameLabel.Name = "MIWInitModelNameLabel";
            MIWInitModelNameLabel.Size = new Size(79, 15);
            MIWInitModelNameLabel.TabIndex = 8;
            MIWInitModelNameLabel.Text = "Model Name:";
            // 
            // MIWInitDSNAMode
            // 
            MIWInitDSNAMode.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MIWInitDSNAMode.Location = new Point(124, 334);
            MIWInitDSNAMode.Name = "MIWInitDSNAMode";
            MIWInitDSNAMode.Size = new Size(392, 23);
            MIWInitDSNAMode.TabIndex = 5;
            MIWInitDSNAMode.Text = "DRAW";
            // 
            // MIWInitModelName
            // 
            MIWInitModelName.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MIWInitModelName.Location = new Point(124, 266);
            MIWInitModelName.Name = "MIWInitModelName";
            MIWInitModelName.Size = new Size(392, 23);
            MIWInitModelName.TabIndex = 6;
            MIWInitModelName.Text = "COS_000";
            // 
            // MIWIntroText
            // 
            MIWIntroText.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MIWIntroText.AutoSize = true;
            MIWIntroText.Location = new Point(124, 123);
            MIWIntroText.Name = "MIWIntroText";
            MIWIntroText.Size = new Size(394, 90);
            MIWIntroText.TabIndex = 4;
            MIWIntroText.Text = resources.GetString("MIWIntroText.Text");
            // 
            // MIWInitControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(MIWInitDNSAModeLabel);
            Controls.Add(MIWInitModelNameLabel);
            Controls.Add(MIWInitDSNAMode);
            Controls.Add(MIWInitModelName);
            Controls.Add(MIWIntroText);
            Location = new Point(72, 12);
            Name = "MIWInitControl";
            Size = new Size(640, 480);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private Label MIWInitDNSAModeLabel;
        private Label MIWInitModelNameLabel;
        private Label MIWIntroText;
        public TextBox MIWInitDSNAMode;
        public TextBox MIWInitModelName;
    }
}
