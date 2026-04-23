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
            MIWMaterialListbox = new ListBox();
            MIWMaterialValidate = new Button();
            MIWMaterialValidateStatusLabel = new Label();
            MIWMaterialValidationStatus = new Label();
            MIWMaterialSelectShader = new Button();
            MIWMaterialOpenShaderDialog = new OpenFileDialog();
            MIWMaterialSelectShaderAll = new Button();
            MaterialProperties = new PropertyGrid();
            StringListViewer = new PropertyGrid();
            SuspendLayout();
            // 
            // MIWMaterialListbox
            // 
            MIWMaterialListbox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            MIWMaterialListbox.FormattingEnabled = true;
            MIWMaterialListbox.ItemHeight = 15;
            MIWMaterialListbox.Location = new Point(3, 3);
            MIWMaterialListbox.Name = "MIWMaterialListbox";
            MIWMaterialListbox.Size = new Size(274, 469);
            MIWMaterialListbox.TabIndex = 0;
            MIWMaterialListbox.SelectedIndexChanged += MIWMaterialListbox_SelectedIndexChanged;
            // 
            // MIWMaterialValidate
            // 
            MIWMaterialValidate.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            MIWMaterialValidate.Location = new Point(283, 449);
            MIWMaterialValidate.Name = "MIWMaterialValidate";
            MIWMaterialValidate.Size = new Size(354, 23);
            MIWMaterialValidate.TabIndex = 1;
            MIWMaterialValidate.Text = "Validate Material Data";
            MIWMaterialValidate.UseVisualStyleBackColor = true;
            MIWMaterialValidate.Click += MIWMaterialValidate_Click;
            // 
            // MIWMaterialValidateStatusLabel
            // 
            MIWMaterialValidateStatusLabel.AutoSize = true;
            MIWMaterialValidateStatusLabel.Location = new Point(283, 3);
            MIWMaterialValidateStatusLabel.Name = "MIWMaterialValidateStatusLabel";
            MIWMaterialValidateStatusLabel.Size = new Size(143, 15);
            MIWMaterialValidateStatusLabel.TabIndex = 2;
            MIWMaterialValidateStatusLabel.Text = "Material Validation Status:";
            // 
            // MIWMaterialValidationStatus
            // 
            MIWMaterialValidationStatus.AutoSize = true;
            MIWMaterialValidationStatus.Location = new Point(283, 34);
            MIWMaterialValidationStatus.Name = "MIWMaterialValidationStatus";
            MIWMaterialValidationStatus.Size = new Size(0, 15);
            MIWMaterialValidationStatus.TabIndex = 2;
            // 
            // MIWMaterialSelectShader
            // 
            MIWMaterialSelectShader.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            MIWMaterialSelectShader.Location = new Point(283, 420);
            MIWMaterialSelectShader.Name = "MIWMaterialSelectShader";
            MIWMaterialSelectShader.Size = new Size(354, 23);
            MIWMaterialSelectShader.TabIndex = 1;
            MIWMaterialSelectShader.Text = "Select Shader Package";
            MIWMaterialSelectShader.UseVisualStyleBackColor = true;
            MIWMaterialSelectShader.Click += MIWMaterialSelectShader_Click;
            // 
            // MIWMaterialOpenShaderDialog
            // 
            MIWMaterialOpenShaderDialog.Filter = "Shader Package|*.csp";
            MIWMaterialOpenShaderDialog.Title = "Please select a shader package \"*.csp\"";
            // 
            // MIWMaterialSelectShaderAll
            // 
            MIWMaterialSelectShaderAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            MIWMaterialSelectShaderAll.Location = new Point(283, 391);
            MIWMaterialSelectShaderAll.Name = "MIWMaterialSelectShaderAll";
            MIWMaterialSelectShaderAll.Size = new Size(354, 23);
            MIWMaterialSelectShaderAll.TabIndex = 1;
            MIWMaterialSelectShaderAll.Text = "Select Shader Package for All Materials";
            MIWMaterialSelectShaderAll.UseVisualStyleBackColor = true;
            MIWMaterialSelectShaderAll.Click += MIWMaterialSelectShaderAll_Click;
            // 
            // MaterialProperties
            // 
            MaterialProperties.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            MaterialProperties.HelpVisible = false;
            MaterialProperties.Location = new Point(283, 226);
            MaterialProperties.Name = "MaterialProperties";
            MaterialProperties.Size = new Size(354, 159);
            MaterialProperties.TabIndex = 3;
            // 
            // StringListViewer
            // 
            StringListViewer.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            StringListViewer.HelpVisible = false;
            StringListViewer.Location = new Point(283, 63);
            StringListViewer.Name = "StringListViewer";
            StringListViewer.Size = new Size(354, 157);
            StringListViewer.TabIndex = 3;
            // 
            // MIWMaterialSetupControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(StringListViewer);
            Controls.Add(MaterialProperties);
            Controls.Add(MIWMaterialValidationStatus);
            Controls.Add(MIWMaterialValidateStatusLabel);
            Controls.Add(MIWMaterialSelectShaderAll);
            Controls.Add(MIWMaterialSelectShader);
            Controls.Add(MIWMaterialValidate);
            Controls.Add(MIWMaterialListbox);
            Location = new Point(72, 12);
            Name = "MIWMaterialSetupControl";
            Size = new Size(640, 480);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button MIWMaterialValidate;
        private Label MIWMaterialValidateStatusLabel;
        private Label MIWMaterialValidationStatus;
        private Button MIWMaterialSelectShader;
        private OpenFileDialog MIWMaterialOpenShaderDialog;
        public ListBox MIWMaterialListbox;
        private Button MIWMaterialSelectShaderAll;
        private PropertyGrid MaterialProperties;
        private PropertyGrid StringListViewer;
    }
}
