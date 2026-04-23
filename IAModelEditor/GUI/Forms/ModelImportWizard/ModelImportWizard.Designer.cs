using IAModelEditor.GUI.Controls;

namespace IAModelEditor.GUI.Forms.ModelImportWizard
{
    partial class ModelImportWizard
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
            MIWInitButtonNext = new Button();
            MIWInitButtonCancel = new Button();
            MIWActiveStageControl = new MIWInitControl();
            button1 = new Button();
            button2 = new Button();
            SuspendLayout();
            // 
            // MIWInitButtonNext
            // 
            MIWInitButtonNext.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            MIWInitButtonNext.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            MIWInitButtonNext.Location = new Point(697, 526);
            MIWInitButtonNext.Name = "MIWInitButtonNext";
            MIWInitButtonNext.Size = new Size(75, 23);
            MIWInitButtonNext.TabIndex = 1;
            MIWInitButtonNext.Text = "Next";
            MIWInitButtonNext.UseVisualStyleBackColor = true;
            MIWInitButtonNext.Click += MIWInitButtonNext_Click;
            // 
            // MIWInitButtonCancel
            // 
            MIWInitButtonCancel.Location = new Point(16, 526);
            MIWInitButtonCancel.Name = "MIWInitButtonCancel";
            MIWInitButtonCancel.Size = new Size(75, 23);
            MIWInitButtonCancel.TabIndex = 1;
            MIWInitButtonCancel.Text = "Cancel";
            MIWInitButtonCancel.UseVisualStyleBackColor = true;
            // 
            // MIWActiveStageControl
            // 
            MIWActiveStageControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MIWActiveStageControl.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            MIWActiveStageControl.Location = new Point(72, 12);
            MIWActiveStageControl.Name = "MIWActiveStageControl";
            MIWActiveStageControl.Size = new Size(640, 480);
            MIWActiveStageControl.TabIndex = 2;
            // 
            // button1
            // 
            button1.Location = new Point(97, 526);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 3;
            button1.Text = "Dump current contents";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(178, 526);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 3;
            button2.Text = "Pause Execution";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // ModelImportWizard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 561);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(MIWActiveStageControl);
            Controls.Add(MIWInitButtonCancel);
            Controls.Add(MIWInitButtonNext);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MaximumSize = new Size(800, 600);
            MinimumSize = new Size(800, 600);
            Name = "ModelImportWizard";
            Text = "MIWInit";
            ResumeLayout(false);

        }

        #endregion
        private Button MIWInitButtonNext;
        private Button MIWInitButtonCancel;
        private Button button1;
        private Button button2;
        private UserControl MIWActiveStageControl;
    }
}