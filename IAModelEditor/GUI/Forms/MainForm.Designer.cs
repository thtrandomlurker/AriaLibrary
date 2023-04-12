namespace IAModelEditor.GUI.Forms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStripOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStripSave = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStripSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStripExport = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStripExportGPR = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStripExportOBJ = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStripExportFBX = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStripEditMESH = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStripEditMESHVariEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStripEditGPR = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStripEditNODT = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStripOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.MenuStripSaveAsFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.CurrentlyLoadedLabel = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.MenuStripExportFBXFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.MenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStrip
            // 
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(402, 24);
            this.MenuStrip.TabIndex = 0;
            this.MenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStripOpen,
            this.MenuStripSave,
            this.MenuStripSaveAs,
            this.MenuStripExport});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // MenuStripOpen
            // 
            this.MenuStripOpen.Name = "MenuStripOpen";
            this.MenuStripOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.MenuStripOpen.Size = new System.Drawing.Size(186, 22);
            this.MenuStripOpen.Text = "Open";
            this.MenuStripOpen.Click += new System.EventHandler(this.MenuStripOpen_OnClick);
            // 
            // MenuStripSave
            // 
            this.MenuStripSave.Name = "MenuStripSave";
            this.MenuStripSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.MenuStripSave.Size = new System.Drawing.Size(186, 22);
            this.MenuStripSave.Text = "Save";
            this.MenuStripSave.Click += new System.EventHandler(this.MenuStripSave_OnClick);
            // 
            // MenuStripSaveAs
            // 
            this.MenuStripSaveAs.Name = "MenuStripSaveAs";
            this.MenuStripSaveAs.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.MenuStripSaveAs.Size = new System.Drawing.Size(186, 22);
            this.MenuStripSaveAs.Text = "Save As";
            this.MenuStripSaveAs.Click += new System.EventHandler(this.MenuStripSaveAs_OnClick);
            // 
            // MenuStripExport
            // 
            this.MenuStripExport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStripExportGPR,
            this.MenuStripExportOBJ,
            this.MenuStripExportFBX});
            this.MenuStripExport.Name = "MenuStripExport";
            this.MenuStripExport.Size = new System.Drawing.Size(186, 22);
            this.MenuStripExport.Text = "Export";
            // 
            // MenuStripExportGPR
            // 
            this.MenuStripExportGPR.Name = "MenuStripExportGPR";
            this.MenuStripExportGPR.Size = new System.Drawing.Size(180, 22);
            this.MenuStripExportGPR.Text = "GPR";
            this.MenuStripExportGPR.Click += new System.EventHandler(this.MenuStripExportGPR_OnClick);
            // 
            // MenuStripExportOBJ
            // 
            this.MenuStripExportOBJ.Name = "MenuStripExportOBJ";
            this.MenuStripExportOBJ.Size = new System.Drawing.Size(180, 22);
            this.MenuStripExportOBJ.Text = "OBJ/MTL/GRP";
            this.MenuStripExportOBJ.Click += new System.EventHandler(this.MenuStripExportOBJ_OnClick);
            // 
            // MenuStripExportFBX
            // 
            this.MenuStripExportFBX.Name = "MenuStripExportFBX";
            this.MenuStripExportFBX.Size = new System.Drawing.Size(180, 22);
            this.MenuStripExportFBX.Text = "FBX";
            this.MenuStripExportFBX.Click += new System.EventHandler(this.MenuStripExportFBX_OnClick);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStripEditMESH,
            this.MenuStripEditGPR,
            this.MenuStripEditNODT});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // MenuStripEditMESH
            // 
            this.MenuStripEditMESH.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStripEditMESHVariEditor});
            this.MenuStripEditMESH.Name = "MenuStripEditMESH";
            this.MenuStripEditMESH.Size = new System.Drawing.Size(210, 22);
            this.MenuStripEditMESH.Text = "MESH";
            // 
            // MenuStripEditMESHVariEditor
            // 
            this.MenuStripEditMESHVariEditor.Name = "MenuStripEditMESHVariEditor";
            this.MenuStripEditMESHVariEditor.Size = new System.Drawing.Size(98, 22);
            this.MenuStripEditMESHVariEditor.Text = "VARI";
            this.MenuStripEditMESHVariEditor.Click += new System.EventHandler(this.MenuStripEditMESHVariEditor_OnClick);
            // 
            // MenuStripEditGPR
            // 
            this.MenuStripEditGPR.Name = "MenuStripEditGPR";
            this.MenuStripEditGPR.Size = new System.Drawing.Size(210, 22);
            this.MenuStripEditGPR.Text = "GPR (Not Implemented)";
            // 
            // MenuStripEditNODT
            // 
            this.MenuStripEditNODT.Name = "MenuStripEditNODT";
            this.MenuStripEditNODT.Size = new System.Drawing.Size(210, 22);
            this.MenuStripEditNODT.Text = "NODT (Not Implemented)";
            // 
            // MenuStripOpenFileDialog
            // 
            this.MenuStripOpenFileDialog.Filter = "IA/VT Model File|*.mdl";
            // 
            // MenuStripSaveAsFileDialog
            // 
            this.MenuStripSaveAsFileDialog.Filter = "IA/VT Model File|*.mdl";
            // 
            // CurrentlyLoadedLabel
            // 
            this.CurrentlyLoadedLabel.AutoSize = true;
            this.CurrentlyLoadedLabel.Location = new System.Drawing.Point(12, 24);
            this.CurrentlyLoadedLabel.Name = "CurrentlyLoadedLabel";
            this.CurrentlyLoadedLabel.Size = new System.Drawing.Size(133, 15);
            this.CurrentlyLoadedLabel.TabIndex = 1;
            this.CurrentlyLoadedLabel.Text = "Currently Loaded: None";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "IA/VT Model File|*.mdl";
            // 
            // MenuStripExportFBXFileDialog
            // 
            this.MenuStripExportFBXFileDialog.Filter = "FBX|*.fbx";
            this.MenuStripExportFBXFileDialog.Title = "Select a destitination file";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 52);
            this.Controls.Add(this.CurrentlyLoadedLabel);
            this.Controls.Add(this.MenuStrip);
            this.MainMenuStrip = this.MenuStrip;
            this.Name = "MainForm";
            this.Text = "IA/VT Colorful Model Editor 0.01";
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip MenuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem MenuStripOpen;
        private OpenFileDialog MenuStripOpenFileDialog;
        private ToolStripMenuItem MenuStripSave;
        private ToolStripMenuItem MenuStripSaveAs;
        private SaveFileDialog MenuStripSaveAsFileDialog;
        private ToolStripMenuItem MenuStripExport;
        private ToolStripMenuItem MenuStripExportGPR;
        private ToolStripMenuItem MenuStripExportOBJ;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem MenuStripEditMESH;
        private ToolStripMenuItem MenuStripEditGPR;
        private ToolStripMenuItem MenuStripEditNODT;
        private Label CurrentlyLoadedLabel;
        private ToolStripMenuItem MenuStripEditMESHVariEditor;
        private ToolStripMenuItem MenuStripExportFBX;
        private OpenFileDialog openFileDialog1;
        private SaveFileDialog MenuStripExportFBXFileDialog;
    }
}