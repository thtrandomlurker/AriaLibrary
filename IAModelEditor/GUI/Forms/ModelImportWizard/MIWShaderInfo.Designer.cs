namespace IAModelEditor.GUI.Forms
{
    partial class MIWShaderInfo
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
            MIWShaderFragmentSampList = new ListBox();
            MIWShaderFragmentConstList = new ListBox();
            MIWShaderVertexConstList = new ListBox();
            MIWShaderVertexAttributeList = new ListBox();
            MIWShaderFragmentConstLabel = new Label();
            MIWShaderFragmentSamplers = new Label();
            MIWShaderFragmentInfoLabel = new Label();
            MIWShaderVertexAttrLabel = new Label();
            MIWShaderVertexConstLabel = new Label();
            MIWShaderVertexInfoLabel = new Label();
            MIWShaderList = new ListBox();
            MIWShaderConfirmation = new Button();
            MIWShaderInfoInputInfoLabel = new Label();
            MIWShaderInfoVariableNameLabel = new Label();
            SuspendLayout();
            // 
            // MIWShaderFragmentSampList
            // 
            MIWShaderFragmentSampList.FormattingEnabled = true;
            MIWShaderFragmentSampList.ItemHeight = 15;
            MIWShaderFragmentSampList.Location = new Point(363, 55);
            MIWShaderFragmentSampList.Name = "MIWShaderFragmentSampList";
            MIWShaderFragmentSampList.Size = new Size(219, 139);
            MIWShaderFragmentSampList.TabIndex = 10;
            // 
            // MIWShaderFragmentConstList
            // 
            MIWShaderFragmentConstList.FormattingEnabled = true;
            MIWShaderFragmentConstList.ItemHeight = 15;
            MIWShaderFragmentConstList.Location = new Point(363, 290);
            MIWShaderFragmentConstList.Name = "MIWShaderFragmentConstList";
            MIWShaderFragmentConstList.Size = new Size(219, 139);
            MIWShaderFragmentConstList.TabIndex = 11;
            // 
            // MIWShaderVertexConstList
            // 
            MIWShaderVertexConstList.FormattingEnabled = true;
            MIWShaderVertexConstList.ItemHeight = 15;
            MIWShaderVertexConstList.Location = new Point(138, 290);
            MIWShaderVertexConstList.Name = "MIWShaderVertexConstList";
            MIWShaderVertexConstList.Size = new Size(219, 139);
            MIWShaderVertexConstList.TabIndex = 12;
            // 
            // MIWShaderVertexAttributeList
            // 
            MIWShaderVertexAttributeList.FormattingEnabled = true;
            MIWShaderVertexAttributeList.ItemHeight = 15;
            MIWShaderVertexAttributeList.Location = new Point(138, 55);
            MIWShaderVertexAttributeList.Name = "MIWShaderVertexAttributeList";
            MIWShaderVertexAttributeList.Size = new Size(219, 139);
            MIWShaderVertexAttributeList.TabIndex = 13;
            MIWShaderVertexAttributeList.SelectedIndexChanged += MIWShaderVertexAttributeList_SelectedIndexChanged;
            // 
            // MIWShaderFragmentConstLabel
            // 
            MIWShaderFragmentConstLabel.AutoSize = true;
            MIWShaderFragmentConstLabel.Location = new Point(363, 272);
            MIWShaderFragmentConstLabel.Name = "MIWShaderFragmentConstLabel";
            MIWShaderFragmentConstLabel.Size = new Size(63, 15);
            MIWShaderFragmentConstLabel.TabIndex = 4;
            MIWShaderFragmentConstLabel.Text = "Constants:";
            // 
            // MIWShaderFragmentSamplers
            // 
            MIWShaderFragmentSamplers.AutoSize = true;
            MIWShaderFragmentSamplers.Location = new Point(363, 37);
            MIWShaderFragmentSamplers.Name = "MIWShaderFragmentSamplers";
            MIWShaderFragmentSamplers.Size = new Size(102, 15);
            MIWShaderFragmentSamplers.TabIndex = 5;
            MIWShaderFragmentSamplers.Text = "Texture Samplers: ";
            // 
            // MIWShaderFragmentInfoLabel
            // 
            MIWShaderFragmentInfoLabel.AutoSize = true;
            MIWShaderFragmentInfoLabel.Location = new Point(363, 12);
            MIWShaderFragmentInfoLabel.Name = "MIWShaderFragmentInfoLabel";
            MIWShaderFragmentInfoLabel.Size = new Size(124, 15);
            MIWShaderFragmentInfoLabel.TabIndex = 6;
            MIWShaderFragmentInfoLabel.Text = "Fragment Shader Info:";
            // 
            // MIWShaderVertexAttrLabel
            // 
            MIWShaderVertexAttrLabel.AutoSize = true;
            MIWShaderVertexAttrLabel.Location = new Point(138, 37);
            MIWShaderVertexAttrLabel.Name = "MIWShaderVertexAttrLabel";
            MIWShaderVertexAttrLabel.Size = new Size(97, 15);
            MIWShaderVertexAttrLabel.TabIndex = 7;
            MIWShaderVertexAttrLabel.Text = "Vertex Attributes:";
            // 
            // MIWShaderVertexConstLabel
            // 
            MIWShaderVertexConstLabel.AutoSize = true;
            MIWShaderVertexConstLabel.Location = new Point(138, 272);
            MIWShaderVertexConstLabel.Name = "MIWShaderVertexConstLabel";
            MIWShaderVertexConstLabel.Size = new Size(63, 15);
            MIWShaderVertexConstLabel.TabIndex = 8;
            MIWShaderVertexConstLabel.Text = "Constants:";
            // 
            // MIWShaderVertexInfoLabel
            // 
            MIWShaderVertexInfoLabel.AutoSize = true;
            MIWShaderVertexInfoLabel.Location = new Point(138, 12);
            MIWShaderVertexInfoLabel.Name = "MIWShaderVertexInfoLabel";
            MIWShaderVertexInfoLabel.Size = new Size(105, 15);
            MIWShaderVertexInfoLabel.TabIndex = 9;
            MIWShaderVertexInfoLabel.Text = "Vertex Shader Info:";
            // 
            // MIWShaderList
            // 
            MIWShaderList.FormattingEnabled = true;
            MIWShaderList.ItemHeight = 15;
            MIWShaderList.Location = new Point(12, 12);
            MIWShaderList.Name = "MIWShaderList";
            MIWShaderList.Size = new Size(120, 514);
            MIWShaderList.TabIndex = 3;
            MIWShaderList.SelectedIndexChanged += MIWShaderList_SelectedIndexChanged;
            // 
            // MIWShaderConfirmation
            // 
            MIWShaderConfirmation.Location = new Point(537, 499);
            MIWShaderConfirmation.Name = "MIWShaderConfirmation";
            MIWShaderConfirmation.Size = new Size(75, 23);
            MIWShaderConfirmation.TabIndex = 14;
            MIWShaderConfirmation.Text = "Confirm";
            MIWShaderConfirmation.UseVisualStyleBackColor = true;
            MIWShaderConfirmation.Click += MIWShaderConfirmation_Click;
            // 
            // MIWShaderInfoInputInfoLabel
            // 
            MIWShaderInfoInputInfoLabel.AutoSize = true;
            MIWShaderInfoInputInfoLabel.Location = new Point(138, 197);
            MIWShaderInfoInputInfoLabel.Name = "MIWShaderInfoInputInfoLabel";
            MIWShaderInfoInputInfoLabel.Size = new Size(62, 15);
            MIWShaderInfoInputInfoLabel.TabIndex = 15;
            MIWShaderInfoInputInfoLabel.Text = "Input Info:";
            // 
            // MIWShaderInfoVariableNameLabel
            // 
            MIWShaderInfoVariableNameLabel.AutoSize = true;
            MIWShaderInfoVariableNameLabel.Location = new Point(139, 212);
            MIWShaderInfoVariableNameLabel.Name = "MIWShaderInfoVariableNameLabel";
            MIWShaderInfoVariableNameLabel.Size = new Size(54, 15);
            MIWShaderInfoVariableNameLabel.TabIndex = 15;
            MIWShaderInfoVariableNameLabel.Text = "Variable: ";
            // 
            // MIWShaderInfo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(624, 534);
            Controls.Add(MIWShaderInfoVariableNameLabel);
            Controls.Add(MIWShaderInfoInputInfoLabel);
            Controls.Add(MIWShaderConfirmation);
            Controls.Add(MIWShaderFragmentSampList);
            Controls.Add(MIWShaderFragmentConstList);
            Controls.Add(MIWShaderVertexConstList);
            Controls.Add(MIWShaderVertexAttributeList);
            Controls.Add(MIWShaderFragmentConstLabel);
            Controls.Add(MIWShaderFragmentSamplers);
            Controls.Add(MIWShaderFragmentInfoLabel);
            Controls.Add(MIWShaderVertexAttrLabel);
            Controls.Add(MIWShaderVertexConstLabel);
            Controls.Add(MIWShaderVertexInfoLabel);
            Controls.Add(MIWShaderList);
            Name = "MIWShaderInfo";
            Text = "Shader Selector";
            ResumeLayout(false);
            PerformLayout();
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
        private Label MIWShaderInfoInputInfoLabel;
        private Label MIWShaderInfoVariableNameLabel;
    }
}