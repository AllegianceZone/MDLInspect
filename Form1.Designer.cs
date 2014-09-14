namespace MDLUtility2
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnpickDirectory = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnOptions = new System.Windows.Forms.Button();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lblInternalReference = new System.Windows.Forms.Label();
            this.lblInternalName = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.lstInternalImages = new System.Windows.Forms.ListBox();
            this.txtBinaryHeader = new System.Windows.Forms.TextBox();
            this.Viewer1 = new System.Windows.Forms.Integration.ElementHost();
            this.mdlViewer1 = new MDLUtility2.MDLViewer();
            this.pbPreviewImage = new System.Windows.Forms.PictureBox();
            this.lblHeight = new System.Windows.Forms.Label();
            this.lblWidth = new System.Windows.Forms.Label();
            this.lblFormat = new System.Windows.Forms.Label();
            this.lblFileName = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblDetails = new System.Windows.Forms.ToolStripStatusLabel();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.contextMenuStripOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rememberZoomLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useQuickZoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreviewImage)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStripOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(2, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(110, 59);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // btnpickDirectory
            // 
            this.btnpickDirectory.Location = new System.Drawing.Point(4, 3);
            this.btnpickDirectory.Margin = new System.Windows.Forms.Padding(0);
            this.btnpickDirectory.Name = "btnpickDirectory";
            this.btnpickDirectory.Size = new System.Drawing.Size(63, 21);
            this.btnpickDirectory.TabIndex = 1;
            this.btnpickDirectory.Text = "Directory";
            this.btnpickDirectory.UseVisualStyleBackColor = true;
            this.btnpickDirectory.Click += new System.EventHandler(this.btnpickDirectory_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel1.Controls.Add(this.btnOptions);
            this.splitContainer1.Panel1.Controls.Add(this.btnSaveAs);
            this.splitContainer1.Panel1.Controls.Add(this.listBox1);
            this.splitContainer1.Panel1.Controls.Add(this.btnpickDirectory);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(807, 542);
            this.splitContainer1.SplitterDistance = 148;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 2;
            // 
            // btnOptions
            // 
            this.btnOptions.Location = new System.Drawing.Point(70, 3);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(60, 21);
            this.btnOptions.TabIndex = 4;
            this.btnOptions.Text = "Options";
            this.btnOptions.UseVisualStyleBackColor = true;
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            this.btnOptions.Leave += new System.EventHandler(this.btnOptions_Leave);
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSaveAs.Enabled = false;
            this.btnSaveAs.Location = new System.Drawing.Point(4, 510);
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(87, 20);
            this.btnSaveAs.TabIndex = 3;
            this.btnSaveAs.Text = "Save As";
            this.btnSaveAs.UseVisualStyleBackColor = true;
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(4, 25);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox1.Size = new System.Drawing.Size(137, 485);
            this.listBox1.TabIndex = 2;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(4, 6);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(649, 528);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.tabPage2.Controls.Add(this.lblInternalReference);
            this.tabPage2.Controls.Add(this.lblInternalName);
            this.tabPage2.Controls.Add(this.splitContainer2);
            this.tabPage2.Controls.Add(this.lblHeight);
            this.tabPage2.Controls.Add(this.lblWidth);
            this.tabPage2.Controls.Add(this.lblFormat);
            this.tabPage2.Controls.Add(this.lblFileName);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(641, 502);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Details";
            // 
            // lblInternalReference
            // 
            this.lblInternalReference.AutoSize = true;
            this.lblInternalReference.Location = new System.Drawing.Point(186, 20);
            this.lblInternalReference.Name = "lblInternalReference";
            this.lblInternalReference.Size = new System.Drawing.Size(65, 13);
            this.lblInternalReference.TabIndex = 1;
            this.lblInternalReference.Text = "References:";
            // 
            // lblInternalName
            // 
            this.lblInternalName.AutoSize = true;
            this.lblInternalName.Location = new System.Drawing.Point(186, 7);
            this.lblInternalName.Name = "lblInternalName";
            this.lblInternalName.Size = new System.Drawing.Size(76, 13);
            this.lblInternalName.TabIndex = 1;
            this.lblInternalName.Text = "Internal Name:";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer2.Location = new System.Drawing.Point(3, 62);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer2.Panel1.Controls.Add(this.lstInternalImages);
            this.splitContainer2.Panel1.Controls.Add(this.txtBinaryHeader);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer2.Panel2.Controls.Add(this.Viewer1);
            this.splitContainer2.Panel2.Controls.Add(this.pbPreviewImage);
            this.splitContainer2.Size = new System.Drawing.Size(635, 437);
            this.splitContainer2.SplitterDistance = 224;
            this.splitContainer2.SplitterWidth = 7;
            this.splitContainer2.TabIndex = 5;
            // 
            // lstInternalImages
            // 
            this.lstInternalImages.FormattingEnabled = true;
            this.lstInternalImages.Location = new System.Drawing.Point(36, 42);
            this.lstInternalImages.Name = "lstInternalImages";
            this.lstInternalImages.Size = new System.Drawing.Size(224, 641);
            this.lstInternalImages.TabIndex = 0;
            this.lstInternalImages.Visible = false;
            this.lstInternalImages.SelectedIndexChanged += new System.EventHandler(this.lstInternalImages_SelectedIndexChanged);
            // 
            // txtBinaryHeader
            // 
            this.txtBinaryHeader.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtBinaryHeader.Location = new System.Drawing.Point(68, 24);
            this.txtBinaryHeader.Multiline = true;
            this.txtBinaryHeader.Name = "txtBinaryHeader";
            this.txtBinaryHeader.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtBinaryHeader.Size = new System.Drawing.Size(313, 375);
            this.txtBinaryHeader.TabIndex = 1;
            this.txtBinaryHeader.Text = "Binary Header";
            this.txtBinaryHeader.Visible = false;
            this.txtBinaryHeader.WordWrap = false;
            // 
            // Viewer1
            // 
            this.Viewer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Viewer1.Location = new System.Drawing.Point(23, 191);
            this.Viewer1.Name = "Viewer1";
            this.Viewer1.Size = new System.Drawing.Size(374, 404);
            this.Viewer1.TabIndex = 4;
            this.Viewer1.Text = "elementHost1";
            this.Viewer1.Visible = false;
            this.Viewer1.Child = this.mdlViewer1;
            // 
            // pbPreviewImage
            // 
            this.pbPreviewImage.Location = new System.Drawing.Point(27, 21);
            this.pbPreviewImage.Name = "pbPreviewImage";
            this.pbPreviewImage.Size = new System.Drawing.Size(173, 164);
            this.pbPreviewImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbPreviewImage.TabIndex = 3;
            this.pbPreviewImage.TabStop = false;
            // 
            // lblHeight
            // 
            this.lblHeight.AutoSize = true;
            this.lblHeight.Location = new System.Drawing.Point(7, 46);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(41, 13);
            this.lblHeight.TabIndex = 0;
            this.lblHeight.Text = "Height:";
            // 
            // lblWidth
            // 
            this.lblWidth.AutoSize = true;
            this.lblWidth.Location = new System.Drawing.Point(7, 33);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(38, 13);
            this.lblWidth.TabIndex = 0;
            this.lblWidth.Text = "Width:";
            // 
            // lblFormat
            // 
            this.lblFormat.AutoSize = true;
            this.lblFormat.Location = new System.Drawing.Point(7, 20);
            this.lblFormat.Name = "lblFormat";
            this.lblFormat.Size = new System.Drawing.Size(42, 13);
            this.lblFormat.TabIndex = 0;
            this.lblFormat.Text = "Format:";
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Location = new System.Drawing.Point(7, 7);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(57, 13);
            this.lblFileName.TabIndex = 0;
            this.lblFileName.Text = "File Name:";
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.Controls.Add(this.hScrollBar1);
            this.tabPage1.Controls.Add(this.vScrollBar1);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.numericUpDown1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(641, 502);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "View";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollBar1.Location = new System.Drawing.Point(39, 488);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(586, 14);
            this.hScrollBar1.TabIndex = 4;
            this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vScrollBar1.Location = new System.Drawing.Point(625, 0);
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(14, 488);
            this.vScrollBar1.TabIndex = 3;
            this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(625, 488);
            this.panel1.TabIndex = 2;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numericUpDown1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown1.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown1.Location = new System.Drawing.Point(2, 488);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(36, 17);
            this.numericUpDown1.TabIndex = 5;
            this.numericUpDown1.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel1,
            this.lblDetails});
            this.statusStrip1.Location = new System.Drawing.Point(0, 532);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip1.Size = new System.Drawing.Size(807, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(116, 17);
            this.toolStripStatusLabel2.Text = "MDL Inspect by Astn";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // lblDetails
            // 
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(0, 17);
            // 
            // contextMenuStripOptions
            // 
            this.contextMenuStripOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rememberZoomLevelToolStripMenuItem,
            this.useQuickZoomToolStripMenuItem});
            this.contextMenuStripOptions.Name = "contextMenuStripOptions";
            this.contextMenuStripOptions.ShowCheckMargin = true;
            this.contextMenuStripOptions.ShowImageMargin = false;
            this.contextMenuStripOptions.Size = new System.Drawing.Size(244, 48);
            // 
            // rememberZoomLevelToolStripMenuItem
            // 
            this.rememberZoomLevelToolStripMenuItem.Name = "rememberZoomLevelToolStripMenuItem";
            this.rememberZoomLevelToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.rememberZoomLevelToolStripMenuItem.Text = "Remember Zoom Level";
            this.rememberZoomLevelToolStripMenuItem.Click += new System.EventHandler(this.rememberZoomLevelToolStripMenuItem_Click);
            // 
            // useQuickZoomToolStripMenuItem
            // 
            this.useQuickZoomToolStripMenuItem.Name = "useQuickZoomToolStripMenuItem";
            this.useQuickZoomToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.useQuickZoomToolStripMenuItem.Text = "Use Quick Zoom (lower Quality)";
            this.useQuickZoomToolStripMenuItem.Click += new System.EventHandler(this.useQuickZoomToolStripMenuItem_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 1;
            this.toolTip1.AutoPopDelay = 20000;
            this.toolTip1.InitialDelay = 1;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 554);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "MDL Inspect";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbPreviewImage)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStripOptions.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnpickDirectory;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btnSaveAs;
        private System.Windows.Forms.Button btnOptions;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripOptions;
        private System.Windows.Forms.ToolStripMenuItem rememberZoomLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useQuickZoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel lblDetails;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.Label lblFormat;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.ListBox lstInternalImages;
        private System.Windows.Forms.Label lblInternalReference;
        private System.Windows.Forms.Label lblInternalName;
        private System.Windows.Forms.TextBox txtBinaryHeader;
        private System.Windows.Forms.PictureBox pbPreviewImage;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Integration.ElementHost Viewer1;
        private MDLViewer mdlViewer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
    }
}

