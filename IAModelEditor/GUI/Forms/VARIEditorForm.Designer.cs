namespace IAModelEditor.GUI.Forms
{
    partial class VARIEditorForm
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
            this.VARIEditorPrimitiveList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // VARIEditorPrimitiveList
            // 
            this.VARIEditorPrimitiveList.FormattingEnabled = true;
            this.VARIEditorPrimitiveList.ItemHeight = 15;
            this.VARIEditorPrimitiveList.Location = new System.Drawing.Point(12, 27);
            this.VARIEditorPrimitiveList.Name = "VARIEditorPrimitiveList";
            this.VARIEditorPrimitiveList.Size = new System.Drawing.Size(237, 409);
            this.VARIEditorPrimitiveList.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Primitives";
            // 
            // VARIEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 473);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.VARIEditorPrimitiveList);
            this.Name = "VARIEditorForm";
            this.Text = "IA/VT Model Editor 0.01: VARI Editor";
            this.Load += new System.EventHandler(this.VARIEditorForm_OnLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ListBox VARIEditorPrimitiveList;
        private Label label1;
    }
}