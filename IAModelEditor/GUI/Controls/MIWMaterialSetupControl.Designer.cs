namespace IAModelEditor.GUI.Controls
{
    partial class MIWMaterialSetupControl
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
            this.MIWMaterialListbox = new System.Windows.Forms.ListBox();
            this.MIWMaterialValidate = new System.Windows.Forms.Button();
            this.MIWMaterialValidateStatusLabel = new System.Windows.Forms.Label();
            this.MIWMaterialValidationStatus = new System.Windows.Forms.Label();
            this.MIWMaterialSelectShader = new System.Windows.Forms.Button();
            this.MIWMaterialOpenShaderDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // MIWMaterialListbox
            // 
            this.MIWMaterialListbox.FormattingEnabled = true;
            this.MIWMaterialListbox.ItemHeight = 15;
            this.MIWMaterialListbox.Location = new System.Drawing.Point(3, 3);
            this.MIWMaterialListbox.Name = "MIWMaterialListbox";
            this.MIWMaterialListbox.Size = new System.Drawing.Size(274, 469);
            this.MIWMaterialListbox.TabIndex = 0;
            // 
            // MIWMaterialValidate
            // 
            this.MIWMaterialValidate.Location = new System.Drawing.Point(283, 449);
            this.MIWMaterialValidate.Name = "MIWMaterialValidate";
            this.MIWMaterialValidate.Size = new System.Drawing.Size(354, 23);
            this.MIWMaterialValidate.TabIndex = 1;
            this.MIWMaterialValidate.Text = "Validate Material Data";
            this.MIWMaterialValidate.UseVisualStyleBackColor = true;
            this.MIWMaterialValidate.Click += new System.EventHandler(this.MIWMaterialValidate_Click);
            // 
            // MIWMaterialValidateStatusLabel
            // 
            this.MIWMaterialValidateStatusLabel.AutoSize = true;
            this.MIWMaterialValidateStatusLabel.Location = new System.Drawing.Point(283, 3);
            this.MIWMaterialValidateStatusLabel.Name = "MIWMaterialValidateStatusLabel";
            this.MIWMaterialValidateStatusLabel.Size = new System.Drawing.Size(143, 15);
            this.MIWMaterialValidateStatusLabel.TabIndex = 2;
            this.MIWMaterialValidateStatusLabel.Text = "Material Validation Status:";
            // 
            // MIWMaterialValidationStatus
            // 
            this.MIWMaterialValidationStatus.AutoSize = true;
            this.MIWMaterialValidationStatus.Location = new System.Drawing.Point(283, 34);
            this.MIWMaterialValidationStatus.Name = "MIWMaterialValidationStatus";
            this.MIWMaterialValidationStatus.Size = new System.Drawing.Size(0, 15);
            this.MIWMaterialValidationStatus.TabIndex = 2;
            // 
            // MIWMaterialSelectShader
            // 
            this.MIWMaterialSelectShader.Location = new System.Drawing.Point(283, 420);
            this.MIWMaterialSelectShader.Name = "MIWMaterialSelectShader";
            this.MIWMaterialSelectShader.Size = new System.Drawing.Size(354, 23);
            this.MIWMaterialSelectShader.TabIndex = 1;
            this.MIWMaterialSelectShader.Text = "Select Shader Package";
            this.MIWMaterialSelectShader.UseVisualStyleBackColor = true;
            this.MIWMaterialSelectShader.Click += new System.EventHandler(this.MIWMaterialSelectShader_Click);
            // 
            // MIWMaterialOpenShaderDialog
            // 
            this.MIWMaterialOpenShaderDialog.Filter = "Shader Package|*.csp";
            this.MIWMaterialOpenShaderDialog.Title = "Please select a shader package \"*.csp\"";
            // 
            // MIWMaterialSetupControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MIWMaterialValidationStatus);
            this.Controls.Add(this.MIWMaterialValidateStatusLabel);
            this.Controls.Add(this.MIWMaterialSelectShader);
            this.Controls.Add(this.MIWMaterialValidate);
            this.Controls.Add(this.MIWMaterialListbox);
            this.Location = new System.Drawing.Point(72, 12);
            this.Name = "MIWMaterialSetupControl";
            this.Size = new System.Drawing.Size(640, 480);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Button MIWMaterialValidate;
        private Label MIWMaterialValidateStatusLabel;
        private Label MIWMaterialValidationStatus;
        private Button MIWMaterialSelectShader;
        private OpenFileDialog MIWMaterialOpenShaderDialog;
        public ListBox MIWMaterialListbox;
    }
}
