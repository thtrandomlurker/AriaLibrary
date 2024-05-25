namespace IAModelEditor.GUI.Forms.ModelImportWizard
{
    partial class MRWShaderSelector
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
            this.MIWShaderFragmentSampList = new System.Windows.Forms.ListBox();
            this.MIWShaderFragmentConstList = new System.Windows.Forms.ListBox();
            this.MIWShaderVertexConstList = new System.Windows.Forms.ListBox();
            this.MIWShaderVertexAttributeList = new System.Windows.Forms.ListBox();
            this.MIWShaderFragmentConstLabel = new System.Windows.Forms.Label();
            this.MIWShaderFragmentSamplers = new System.Windows.Forms.Label();
            this.MIWShaderFragmentInfoLabel = new System.Windows.Forms.Label();
            this.MIWShaderVertexAttrLabel = new System.Windows.Forms.Label();
            this.MIWShaderVertexConstLabel = new System.Windows.Forms.Label();
            this.MIWShaderVertexInfoLabel = new System.Windows.Forms.Label();
            this.MIWShaderList = new System.Windows.Forms.ListBox();
            this.MIWShaderConfirmation = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // MIWShaderFragmentSampList
            // 
            this.MIWShaderFragmentSampList.FormattingEnabled = true;
            this.MIWShaderFragmentSampList.ItemHeight = 15;
            this.MIWShaderFragmentSampList.Location = new System.Drawing.Point(363, 55);
            this.MIWShaderFragmentSampList.Name = "MIWShaderFragmentSampList";
            this.MIWShaderFragmentSampList.Size = new System.Drawing.Size(219, 139);
            this.MIWShaderFragmentSampList.TabIndex = 10;
            // 
            // MIWShaderFragmentConstList
            // 
            this.MIWShaderFragmentConstList.FormattingEnabled = true;
            this.MIWShaderFragmentConstList.ItemHeight = 15;
            this.MIWShaderFragmentConstList.Location = new System.Drawing.Point(363, 237);
            this.MIWShaderFragmentConstList.Name = "MIWShaderFragmentConstList";
            this.MIWShaderFragmentConstList.Size = new System.Drawing.Size(219, 139);
            this.MIWShaderFragmentConstList.TabIndex = 11;
            // 
            // MIWShaderVertexConstList
            // 
            this.MIWShaderVertexConstList.FormattingEnabled = true;
            this.MIWShaderVertexConstList.ItemHeight = 15;
            this.MIWShaderVertexConstList.Location = new System.Drawing.Point(138, 237);
            this.MIWShaderVertexConstList.Name = "MIWShaderVertexConstList";
            this.MIWShaderVertexConstList.Size = new System.Drawing.Size(219, 139);
            this.MIWShaderVertexConstList.TabIndex = 12;
            // 
            // MIWShaderVertexAttributeList
            // 
            this.MIWShaderVertexAttributeList.FormattingEnabled = true;
            this.MIWShaderVertexAttributeList.ItemHeight = 15;
            this.MIWShaderVertexAttributeList.Location = new System.Drawing.Point(138, 55);
            this.MIWShaderVertexAttributeList.Name = "MIWShaderVertexAttributeList";
            this.MIWShaderVertexAttributeList.Size = new System.Drawing.Size(219, 139);
            this.MIWShaderVertexAttributeList.TabIndex = 13;
            // 
            // MIWShaderFragmentConstLabel
            // 
            this.MIWShaderFragmentConstLabel.AutoSize = true;
            this.MIWShaderFragmentConstLabel.Location = new System.Drawing.Point(363, 219);
            this.MIWShaderFragmentConstLabel.Name = "MIWShaderFragmentConstLabel";
            this.MIWShaderFragmentConstLabel.Size = new System.Drawing.Size(63, 15);
            this.MIWShaderFragmentConstLabel.TabIndex = 4;
            this.MIWShaderFragmentConstLabel.Text = "Constants:";
            // 
            // MIWShaderFragmentSamplers
            // 
            this.MIWShaderFragmentSamplers.AutoSize = true;
            this.MIWShaderFragmentSamplers.Location = new System.Drawing.Point(363, 37);
            this.MIWShaderFragmentSamplers.Name = "MIWShaderFragmentSamplers";
            this.MIWShaderFragmentSamplers.Size = new System.Drawing.Size(102, 15);
            this.MIWShaderFragmentSamplers.TabIndex = 5;
            this.MIWShaderFragmentSamplers.Text = "Texture Samplers: ";
            // 
            // MIWShaderFragmentInfoLabel
            // 
            this.MIWShaderFragmentInfoLabel.AutoSize = true;
            this.MIWShaderFragmentInfoLabel.Location = new System.Drawing.Point(363, 12);
            this.MIWShaderFragmentInfoLabel.Name = "MIWShaderFragmentInfoLabel";
            this.MIWShaderFragmentInfoLabel.Size = new System.Drawing.Size(124, 15);
            this.MIWShaderFragmentInfoLabel.TabIndex = 6;
            this.MIWShaderFragmentInfoLabel.Text = "Fragment Shader Info:";
            // 
            // MIWShaderVertexAttrLabel
            // 
            this.MIWShaderVertexAttrLabel.AutoSize = true;
            this.MIWShaderVertexAttrLabel.Location = new System.Drawing.Point(138, 37);
            this.MIWShaderVertexAttrLabel.Name = "MIWShaderVertexAttrLabel";
            this.MIWShaderVertexAttrLabel.Size = new System.Drawing.Size(97, 15);
            this.MIWShaderVertexAttrLabel.TabIndex = 7;
            this.MIWShaderVertexAttrLabel.Text = "Vertex Attributes:";
            // 
            // MIWShaderVertexConstLabel
            // 
            this.MIWShaderVertexConstLabel.AutoSize = true;
            this.MIWShaderVertexConstLabel.Location = new System.Drawing.Point(138, 219);
            this.MIWShaderVertexConstLabel.Name = "MIWShaderVertexConstLabel";
            this.MIWShaderVertexConstLabel.Size = new System.Drawing.Size(63, 15);
            this.MIWShaderVertexConstLabel.TabIndex = 8;
            this.MIWShaderVertexConstLabel.Text = "Constants:";
            // 
            // MIWShaderVertexInfoLabel
            // 
            this.MIWShaderVertexInfoLabel.AutoSize = true;
            this.MIWShaderVertexInfoLabel.Location = new System.Drawing.Point(138, 12);
            this.MIWShaderVertexInfoLabel.Name = "MIWShaderVertexInfoLabel";
            this.MIWShaderVertexInfoLabel.Size = new System.Drawing.Size(105, 15);
            this.MIWShaderVertexInfoLabel.TabIndex = 9;
            this.MIWShaderVertexInfoLabel.Text = "Vertex Shader Info:";
            // 
            // MIWShaderList
            // 
            this.MIWShaderList.FormattingEnabled = true;
            this.MIWShaderList.ItemHeight = 15;
            this.MIWShaderList.Location = new System.Drawing.Point(12, 12);
            this.MIWShaderList.Name = "MIWShaderList";
            this.MIWShaderList.Size = new System.Drawing.Size(120, 364);
            this.MIWShaderList.TabIndex = 3;
            this.MIWShaderList.SelectedIndexChanged += new System.EventHandler(this.MIWShaderList_SelectedIndexChanged);
            // 
            // MIWShaderConfirmation
            // 
            this.MIWShaderConfirmation.Location = new System.Drawing.Point(537, 406);
            this.MIWShaderConfirmation.Name = "MIWShaderConfirmation";
            this.MIWShaderConfirmation.Size = new System.Drawing.Size(75, 23);
            this.MIWShaderConfirmation.TabIndex = 14;
            this.MIWShaderConfirmation.Text = "Confirm";
            this.MIWShaderConfirmation.UseVisualStyleBackColor = true;
            this.MIWShaderConfirmation.Click += new System.EventHandler(this.MIWShaderConfirmation_Click);
            // 
            // MIWShaderSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.MIWShaderConfirmation);
            this.Controls.Add(this.MIWShaderFragmentSampList);
            this.Controls.Add(this.MIWShaderFragmentConstList);
            this.Controls.Add(this.MIWShaderVertexConstList);
            this.Controls.Add(this.MIWShaderVertexAttributeList);
            this.Controls.Add(this.MIWShaderFragmentConstLabel);
            this.Controls.Add(this.MIWShaderFragmentSamplers);
            this.Controls.Add(this.MIWShaderFragmentInfoLabel);
            this.Controls.Add(this.MIWShaderVertexAttrLabel);
            this.Controls.Add(this.MIWShaderVertexConstLabel);
            this.Controls.Add(this.MIWShaderVertexInfoLabel);
            this.Controls.Add(this.MIWShaderList);
            this.Name = "MIWShaderSelector";
            this.Text = "Shader Selector";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ListBox MIWShaderFragmentSampList;
        private ListBox MIWShaderFragmentConstList;
        private ListBox MIWShaderVertexConstList;
        private ListBox MIWShaderVertexAttributeList;
        private Label MIWShaderFragmentConstLabel;
        private Label MIWShaderFragmentSamplers;
        private Label MIWShaderFragmentInfoLabel;
        private Label MIWShaderVertexAttrLabel;
        private Label MIWShaderVertexConstLabel;
        private Label MIWShaderVertexInfoLabel;
        private ListBox MIWShaderList;
        private Button MIWShaderConfirmation;
    }
}