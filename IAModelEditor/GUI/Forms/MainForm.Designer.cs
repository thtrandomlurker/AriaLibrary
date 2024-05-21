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
            MenuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            MenuStripOpen = new ToolStripMenuItem();
            MenuStripSave = new ToolStripMenuItem();
            MenuStripSaveAs = new ToolStripMenuItem();
            MenuStripExport = new ToolStripMenuItem();
            MenuStripExportGPR = new ToolStripMenuItem();
            MenuStripExportFBX = new ToolStripMenuItem();
            MenuStripExportFBXBasic = new ToolStripMenuItem();
            MenuStripCreate = new ToolStripMenuItem();
            MenuStripReplace = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            MenuStripEditMESH = new ToolStripMenuItem();
            MenuStripEditMESHVariEditor = new ToolStripMenuItem();
            MenuStripEditGPR = new ToolStripMenuItem();
            MenuStripEditNODT = new ToolStripMenuItem();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            MenuStripToolsConvertGXT = new ToolStripMenuItem();
            MenuStripToolsConvertDDS = new ToolStripMenuItem();
            clearCSTSsForScienceToolStripMenuItem = new ToolStripMenuItem();
            unswizzleDDSInPlacToolStripMenuItem = new ToolStripMenuItem();
            MenuStripOpenFileDialog = new OpenFileDialog();
            MenuStripSaveAsFileDialog = new SaveFileDialog();
            CurrentlyLoadedLabel = new Label();
            MenuStripReplaceFileDialog = new OpenFileDialog();
            MenuStripExportFBXFileDialog = new SaveFileDialog();
            LoadedPlatformLabel = new Label();
            sanityCzechToolStripMenuItem = new ToolStripMenuItem();
            MenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // MenuStrip
            // 
            MenuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, toolsToolStripMenuItem });
            MenuStrip.Location = new Point(0, 0);
            MenuStrip.Name = "MenuStrip";
            MenuStrip.Size = new Size(402, 24);
            MenuStrip.TabIndex = 0;
            MenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { MenuStripOpen, MenuStripSave, MenuStripSaveAs, MenuStripExport, MenuStripCreate, MenuStripReplace, sanityCzechToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // MenuStripOpen
            // 
            MenuStripOpen.Name = "MenuStripOpen";
            MenuStripOpen.ShortcutKeys = Keys.Control | Keys.O;
            MenuStripOpen.Size = new Size(186, 22);
            MenuStripOpen.Text = "Open";
            MenuStripOpen.Click += MenuStripOpen_OnClick;
            // 
            // MenuStripSave
            // 
            MenuStripSave.Enabled = false;
            MenuStripSave.Name = "MenuStripSave";
            MenuStripSave.ShortcutKeys = Keys.Control | Keys.S;
            MenuStripSave.Size = new Size(186, 22);
            MenuStripSave.Text = "Save";
            MenuStripSave.Click += MenuStripSave_OnClick;
            // 
            // MenuStripSaveAs
            // 
            MenuStripSaveAs.Enabled = false;
            MenuStripSaveAs.Name = "MenuStripSaveAs";
            MenuStripSaveAs.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
            MenuStripSaveAs.Size = new Size(186, 22);
            MenuStripSaveAs.Text = "Save As";
            MenuStripSaveAs.Click += MenuStripSaveAs_OnClick;
            // 
            // MenuStripExport
            // 
            MenuStripExport.DropDownItems.AddRange(new ToolStripItem[] { MenuStripExportGPR, MenuStripExportFBX, MenuStripExportFBXBasic });
            MenuStripExport.Enabled = false;
            MenuStripExport.Name = "MenuStripExport";
            MenuStripExport.Size = new Size(186, 22);
            MenuStripExport.Text = "Export";
            // 
            // MenuStripExportGPR
            // 
            MenuStripExportGPR.Enabled = false;
            MenuStripExportGPR.Name = "MenuStripExportGPR";
            MenuStripExportGPR.Size = new Size(132, 22);
            MenuStripExportGPR.Text = "GPR";
            MenuStripExportGPR.Visible = false;
            MenuStripExportGPR.Click += MenuStripExportGPR_OnClick;
            // 
            // MenuStripExportFBX
            // 
            MenuStripExportFBX.Enabled = false;
            MenuStripExportFBX.Name = "MenuStripExportFBX";
            MenuStripExportFBX.Size = new Size(132, 22);
            MenuStripExportFBX.Text = "FBX";
            MenuStripExportFBX.Click += MenuStripExportFBX_OnClick;
            // 
            // MenuStripExportFBXBasic
            // 
            MenuStripExportFBXBasic.Enabled = false;
            MenuStripExportFBXBasic.Name = "MenuStripExportFBXBasic";
            MenuStripExportFBXBasic.Size = new Size(132, 22);
            MenuStripExportFBXBasic.Text = "FBX (Basic)";
            MenuStripExportFBXBasic.Click += MenuStripExportFBXBasic_Click;
            // 
            // MenuStripCreate
            // 
            MenuStripCreate.Name = "MenuStripCreate";
            MenuStripCreate.Size = new Size(186, 22);
            MenuStripCreate.Text = "Create";
            MenuStripCreate.Click += MenuStripCreate_Click;
            // 
            // MenuStripReplace
            // 
            MenuStripReplace.Enabled = false;
            MenuStripReplace.Name = "MenuStripReplace";
            MenuStripReplace.Size = new Size(186, 22);
            MenuStripReplace.Text = "Replace";
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { MenuStripEditMESH, MenuStripEditGPR, MenuStripEditNODT });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(39, 20);
            editToolStripMenuItem.Text = "Edit";
            // 
            // MenuStripEditMESH
            // 
            MenuStripEditMESH.DropDownItems.AddRange(new ToolStripItem[] { MenuStripEditMESHVariEditor });
            MenuStripEditMESH.Name = "MenuStripEditMESH";
            MenuStripEditMESH.Size = new Size(210, 22);
            MenuStripEditMESH.Text = "MESH";
            // 
            // MenuStripEditMESHVariEditor
            // 
            MenuStripEditMESHVariEditor.Name = "MenuStripEditMESHVariEditor";
            MenuStripEditMESHVariEditor.Size = new Size(98, 22);
            MenuStripEditMESHVariEditor.Text = "VARI";
            MenuStripEditMESHVariEditor.Click += MenuStripEditMESHVariEditor_OnClick;
            // 
            // MenuStripEditGPR
            // 
            MenuStripEditGPR.Name = "MenuStripEditGPR";
            MenuStripEditGPR.Size = new Size(210, 22);
            MenuStripEditGPR.Text = "GPR (Not Implemented)";
            // 
            // MenuStripEditNODT
            // 
            MenuStripEditNODT.Name = "MenuStripEditNODT";
            MenuStripEditNODT.Size = new Size(210, 22);
            MenuStripEditNODT.Text = "NODT (Not Implemented)";
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { MenuStripToolsConvertGXT, MenuStripToolsConvertDDS, clearCSTSsForScienceToolStripMenuItem, unswizzleDDSInPlacToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new Size(46, 20);
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // MenuStripToolsConvertGXT
            // 
            MenuStripToolsConvertGXT.Name = "MenuStripToolsConvertGXT";
            MenuStripToolsConvertGXT.Size = new Size(195, 22);
            MenuStripToolsConvertGXT.Text = "Convert GXT(s) to DDS";
            MenuStripToolsConvertGXT.Click += MenuStripToolsConvertGXT_Click;
            // 
            // MenuStripToolsConvertDDS
            // 
            MenuStripToolsConvertDDS.Name = "MenuStripToolsConvertDDS";
            MenuStripToolsConvertDDS.Size = new Size(195, 22);
            MenuStripToolsConvertDDS.Text = "Convert DDS(s) to GXT";
            MenuStripToolsConvertDDS.Click += MenuStripToolsConvertDDS_Click;
            // 
            // clearCSTSsForScienceToolStripMenuItem
            // 
            clearCSTSsForScienceToolStripMenuItem.Name = "clearCSTSsForScienceToolStripMenuItem";
            clearCSTSsForScienceToolStripMenuItem.Size = new Size(195, 22);
            clearCSTSsForScienceToolStripMenuItem.Text = "Clear CSTSs for science";
            clearCSTSsForScienceToolStripMenuItem.Click += clearCSTSsForScienceToolStripMenuItem_Click;
            // 
            // unswizzleDDSInPlacToolStripMenuItem
            // 
            unswizzleDDSInPlacToolStripMenuItem.Name = "unswizzleDDSInPlacToolStripMenuItem";
            unswizzleDDSInPlacToolStripMenuItem.Size = new Size(195, 22);
            unswizzleDDSInPlacToolStripMenuItem.Text = "Unswizzle DDS In Plac";
            unswizzleDDSInPlacToolStripMenuItem.Click += unswizzleDDSInPlacToolStripMenuItem_Click;
            // 
            // MenuStripOpenFileDialog
            // 
            MenuStripOpenFileDialog.Filter = "IA/VT Model File|*.mdl";
            // 
            // MenuStripSaveAsFileDialog
            // 
            MenuStripSaveAsFileDialog.Filter = "IA/VT Model File|*.mdl";
            // 
            // CurrentlyLoadedLabel
            // 
            CurrentlyLoadedLabel.AutoSize = true;
            CurrentlyLoadedLabel.Location = new Point(12, 24);
            CurrentlyLoadedLabel.Name = "CurrentlyLoadedLabel";
            CurrentlyLoadedLabel.Size = new Size(133, 15);
            CurrentlyLoadedLabel.TabIndex = 1;
            CurrentlyLoadedLabel.Text = "Currently Loaded: None";
            // 
            // MenuStripReplaceFileDialog
            // 
            MenuStripReplaceFileDialog.Filter = "FBX|*.fbx";
            // 
            // MenuStripExportFBXFileDialog
            // 
            MenuStripExportFBXFileDialog.Filter = "FBX|*.fbx|DAE|*.dae";
            MenuStripExportFBXFileDialog.Title = "Select a destitination file";
            // 
            // LoadedPlatformLabel
            // 
            LoadedPlatformLabel.AutoSize = true;
            LoadedPlatformLabel.Location = new Point(208, 24);
            LoadedPlatformLabel.Name = "LoadedPlatformLabel";
            LoadedPlatformLabel.Size = new Size(56, 15);
            LoadedPlatformLabel.TabIndex = 2;
            LoadedPlatformLabel.Text = "Platform:";
            // 
            // sanityCzechToolStripMenuItem
            // 
            sanityCzechToolStripMenuItem.Name = "sanityCzechToolStripMenuItem";
            sanityCzechToolStripMenuItem.Size = new Size(186, 22);
            sanityCzechToolStripMenuItem.Text = "Sanity Czech";
            sanityCzechToolStripMenuItem.Click += sanityCzechToolStripMenuItem_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(402, 52);
            Controls.Add(LoadedPlatformLabel);
            Controls.Add(CurrentlyLoadedLabel);
            Controls.Add(MenuStrip);
            MainMenuStrip = MenuStrip;
            Name = "MainForm";
            Text = "IA/VT Colorful Model Editor 0.1";
            MenuStrip.ResumeLayout(false);
            MenuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem MenuStripEditMESH;
        private ToolStripMenuItem MenuStripEditGPR;
        private ToolStripMenuItem MenuStripEditNODT;
        private Label CurrentlyLoadedLabel;
        private ToolStripMenuItem MenuStripEditMESHVariEditor;
        private ToolStripMenuItem MenuStripExportFBX;
        private OpenFileDialog MenuStripReplaceFileDialog;
        private SaveFileDialog MenuStripExportFBXFileDialog;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem MenuStripToolsConvertGXT;
        private ToolStripMenuItem MenuStripToolsConvertDDS;
        private ToolStripMenuItem MenuStripCreate;
        private ToolStripMenuItem MenuStripReplace;
        private ToolStripMenuItem clearCSTSsForScienceToolStripMenuItem;
        private ToolStripMenuItem unswizzleDDSInPlacToolStripMenuItem;
        private Label LoadedPlatformLabel;
        private ToolStripMenuItem MenuStripExportFBXBasic;
        private ToolStripMenuItem sanityCzechToolStripMenuItem;
    }
}