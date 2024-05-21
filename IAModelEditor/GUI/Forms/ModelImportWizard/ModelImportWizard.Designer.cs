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
            this.MIWInitButtonNext = new System.Windows.Forms.Button();
            this.MIWInitButtonCancel = new System.Windows.Forms.Button();
            this.MIWActiveStageControl = new IAModelEditor.GUI.Controls.MIWInitControl();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // MIWInitButtonNext
            // 
            this.MIWInitButtonNext.Location = new System.Drawing.Point(697, 526);
            this.MIWInitButtonNext.Name = "MIWInitButtonNext";
            this.MIWInitButtonNext.Size = new System.Drawing.Size(75, 23);
            this.MIWInitButtonNext.TabIndex = 1;
            this.MIWInitButtonNext.Text = "Next";
            this.MIWInitButtonNext.UseVisualStyleBackColor = true;
            this.MIWInitButtonNext.Click += new System.EventHandler(this.MIWInitButtonNext_Click);
            // 
            // MIWInitButtonCancel
            // 
            this.MIWInitButtonCancel.Location = new System.Drawing.Point(16, 526);
            this.MIWInitButtonCancel.Name = "MIWInitButtonCancel";
            this.MIWInitButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.MIWInitButtonCancel.TabIndex = 1;
            this.MIWInitButtonCancel.Text = "Cancel";
            this.MIWInitButtonCancel.UseVisualStyleBackColor = true;
            // 
            // MIWActiveStageControl
            // 
            this.MIWActiveStageControl.Location = new System.Drawing.Point(72, 12);
            this.MIWActiveStageControl.Name = "MIWActiveStageControl";
            this.MIWActiveStageControl.Size = new System.Drawing.Size(640, 480);
            this.MIWActiveStageControl.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(97, 526);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Dump current contents";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ModelImportWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.MIWActiveStageControl);
            this.Controls.Add(this.MIWInitButtonCancel);
            this.Controls.Add(this.MIWInitButtonNext);
            this.Name = "ModelImportWizard";
            this.Text = "MIWInit";
            this.ResumeLayout(false);

        }

        #endregion
        private Button MIWInitButtonNext;
        private Button MIWInitButtonCancel;
        private Button button1;
        private UserControl MIWActiveStageControl;
    }
}