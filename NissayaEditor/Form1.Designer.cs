namespace NissayaEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newCtrlNToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printCtrlPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.paliTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plainTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullDocToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button_Find = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label_DataStatus = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.textBox_Find = new System.Windows.Forms.TextBox();
            this.button_X = new System.Windows.Forms.Button();
            this.button_FindNext = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_FindStatus = new System.Windows.Forms.TextBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button_FindPrev = new System.Windows.Forms.Button();
            this.button_jjha = new System.Windows.Forms.Button();
            this.button_kinsi = new System.Windows.Forms.Button();
            this.button_ddha = new System.Windows.Forms.Button();
            this.button_ttha = new System.Windows.Forms.Button();
            this.label_MN = new System.Windows.Forms.Label();
            this.textBox_MN = new System.Windows.Forms.TextBox();
            this.label_MNTitle = new System.Windows.Forms.Label();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.bookStyleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(774, 25);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newCtrlNToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.printCtrlPToolStripMenuItem,
            this.exitToolStripMenuItem,
            this.toolStripSeparator1});
            this.fileToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(39, 21);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newCtrlNToolStripMenuItem
            // 
            this.newCtrlNToolStripMenuItem.Name = "newCtrlNToolStripMenuItem";
            this.newCtrlNToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.newCtrlNToolStripMenuItem.Text = "New";
            this.newCtrlNToolStripMenuItem.Click += new System.EventHandler(this.newCtrlNToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.loadToolStripMenuItem.Text = "Open <Ctrl-O>";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.saveToolStripMenuItem.Text = "Save <Ctrl-S>";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.saveAsToolStripMenuItem.Text = "Save As <Ctrl-A>";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.closeToolStripMenuItem.Text = "Close <Ctrl-W>";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // printCtrlPToolStripMenuItem
            // 
            this.printCtrlPToolStripMenuItem.Name = "printCtrlPToolStripMenuItem";
            this.printCtrlPToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.printCtrlPToolStripMenuItem.Text = "Print <Ctrl-P>";
            this.printCtrlPToolStripMenuItem.Click += new System.EventHandler(this.printCtrlPToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.exitToolStripMenuItem.Text = "Exit <Ctrl-Q>";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(173, 6);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.paliTextToolStripMenuItem,
            this.plainTextToolStripMenuItem,
            this.bookStyleToolStripMenuItem,
            this.toolStripSeparator2,
            this.fullDocToolStripMenuItem,
            this.tableToolStripMenuItem});
            this.viewToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(47, 21);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // paliTextToolStripMenuItem
            // 
            this.paliTextToolStripMenuItem.Name = "paliTextToolStripMenuItem";
            this.paliTextToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.paliTextToolStripMenuItem.Text = "Pāḷi Text Only";
            this.paliTextToolStripMenuItem.Click += new System.EventHandler(this.paliTextToolStripMenuItem_Click);
            // 
            // plainTextToolStripMenuItem
            // 
            this.plainTextToolStripMenuItem.Name = "plainTextToolStripMenuItem";
            this.plainTextToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.plainTextToolStripMenuItem.Text = "Translation Only";
            this.plainTextToolStripMenuItem.Click += new System.EventHandler(this.plainTextToolStripMenuItem_Click);
            // 
            // fullDocToolStripMenuItem
            // 
            this.fullDocToolStripMenuItem.Name = "fullDocToolStripMenuItem";
            this.fullDocToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.fullDocToolStripMenuItem.Text = "Full Doc";
            this.fullDocToolStripMenuItem.Click += new System.EventHandler(this.fullDocToolStripMenuItem_Click);
            // 
            // tableToolStripMenuItem
            // 
            this.tableToolStripMenuItem.Name = "tableToolStripMenuItem";
            this.tableToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.tableToolStripMenuItem.Text = "Tabular";
            this.tableToolStripMenuItem.Click += new System.EventHandler(this.tableToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(55, 21);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(70, 70);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(694, 445);
            this.tabControl1.TabIndex = 2;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.SizeUpdate);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.richTextBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(686, 417);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "File1";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Enabled = false;
            this.richTextBox1.Font = new System.Drawing.Font("Myanmar Text", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(5, 8);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(674, 402);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            this.richTextBox1.Visible = false;
            // 
            // button_Find
            // 
            this.button_Find.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Find.Location = new System.Drawing.Point(242, 37);
            this.button_Find.Name = "button_Find";
            this.button_Find.Size = new System.Drawing.Size(58, 25);
            this.button_Find.TabIndex = 5;
            this.button_Find.Text = "Find";
            this.button_Find.UseVisualStyleBackColor = true;
            this.button_Find.Click += new System.EventHandler(this.button_Find_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Data Status :";
            // 
            // label_DataStatus
            // 
            this.label_DataStatus.AutoSize = true;
            this.label_DataStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_DataStatus.Location = new System.Drawing.Point(94, 41);
            this.label_DataStatus.Name = "label_DataStatus";
            this.label_DataStatus.Size = new System.Drawing.Size(116, 16);
            this.label_DataStatus.TabIndex = 6;
            this.label_DataStatus.Text = "Read-Only Ready";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Enabled = false;
            this.dataGridView1.Location = new System.Drawing.Point(35, 469);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(74, 40);
            this.dataGridView1.TabIndex = 7;
            this.dataGridView1.Visible = false;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            this.dataGridView1.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellLeave);
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            this.dataGridView1.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridView1_EditingControlShow);
            this.dataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
            this.dataGridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyDown);
            this.dataGridView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseClick);
            // 
            // textBox_Find
            // 
            this.textBox_Find.AcceptsReturn = true;
            this.textBox_Find.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_Find.Font = new System.Drawing.Font("Myanmar Text", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_Find.Location = new System.Drawing.Point(314, 38);
            this.textBox_Find.Name = "textBox_Find";
            this.textBox_Find.Size = new System.Drawing.Size(102, 25);
            this.textBox_Find.TabIndex = 8;
            this.textBox_Find.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_Find_KeyDown);
            // 
            // button_X
            // 
            this.button_X.BackColor = System.Drawing.Color.White;
            this.button_X.FlatAppearance.BorderSize = 0;
            this.button_X.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_X.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_X.Location = new System.Drawing.Point(522, 39);
            this.button_X.Name = "button_X";
            this.button_X.Size = new System.Drawing.Size(18, 23);
            this.button_X.TabIndex = 1;
            this.button_X.TabStop = false;
            this.button_X.Text = "X";
            this.button_X.UseVisualStyleBackColor = false;
            this.button_X.Click += new System.EventHandler(this.button_X_Click);
            // 
            // button_FindNext
            // 
            this.button_FindNext.BackColor = System.Drawing.Color.White;
            this.button_FindNext.FlatAppearance.BorderSize = 0;
            this.button_FindNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_FindNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_FindNext.Image = global::NissayaEditor.Properties.Resources.DownArrowHead;
            this.button_FindNext.Location = new System.Drawing.Point(500, 39);
            this.button_FindNext.Name = "button_FindNext";
            this.button_FindNext.Size = new System.Drawing.Size(23, 23);
            this.button_FindNext.TabIndex = 12;
            this.button_FindNext.TabStop = false;
            this.button_FindNext.UseVisualStyleBackColor = false;
            this.button_FindNext.Click += new System.EventHandler(this.button_FindNext_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label2.Location = new System.Drawing.Point(466, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 20);
            this.label2.TabIndex = 13;
            this.label2.Text = "|";
            // 
            // textBox_FindStatus
            // 
            this.textBox_FindStatus.AcceptsReturn = true;
            this.textBox_FindStatus.BackColor = System.Drawing.Color.White;
            this.textBox_FindStatus.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_FindStatus.Enabled = false;
            this.textBox_FindStatus.Font = new System.Drawing.Font("Myanmar Text", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_FindStatus.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBox_FindStatus.Location = new System.Drawing.Point(420, 43);
            this.textBox_FindStatus.Name = "textBox_FindStatus";
            this.textBox_FindStatus.ReadOnly = true;
            this.textBox_FindStatus.Size = new System.Drawing.Size(46, 23);
            this.textBox_FindStatus.TabIndex = 14;
            this.textBox_FindStatus.Text = "100/100";
            this.textBox_FindStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox_FindStatus.WordWrap = false;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(316, 495);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 15;
            this.textBox1.Visible = false;
            // 
            // button_FindPrev
            // 
            this.button_FindPrev.BackColor = System.Drawing.Color.White;
            this.button_FindPrev.FlatAppearance.BorderSize = 0;
            this.button_FindPrev.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_FindPrev.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_FindPrev.Image = global::NissayaEditor.Properties.Resources.UpArrowHead;
            this.button_FindPrev.Location = new System.Drawing.Point(484, 38);
            this.button_FindPrev.Name = "button_FindPrev";
            this.button_FindPrev.Size = new System.Drawing.Size(18, 23);
            this.button_FindPrev.TabIndex = 11;
            this.button_FindPrev.TabStop = false;
            this.button_FindPrev.UseVisualStyleBackColor = false;
            this.button_FindPrev.Click += new System.EventHandler(this.button_FindPrev_Click);
            // 
            // button_jjha
            // 
            this.button_jjha.Font = new System.Drawing.Font("Myanmar Text", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_jjha.Location = new System.Drawing.Point(561, 36);
            this.button_jjha.Name = "button_jjha";
            this.button_jjha.Size = new System.Drawing.Size(25, 28);
            this.button_jjha.TabIndex = 16;
            this.button_jjha.Text = "ဇ္ဈ";
            this.button_jjha.UseVisualStyleBackColor = true;
            this.button_jjha.Click += new System.EventHandler(this.HotKey);
            // 
            // button_kinsi
            // 
            this.button_kinsi.Font = new System.Drawing.Font("Myanmar Text", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_kinsi.Location = new System.Drawing.Point(584, 36);
            this.button_kinsi.Name = "button_kinsi";
            this.button_kinsi.Size = new System.Drawing.Size(25, 28);
            this.button_kinsi.TabIndex = 17;
            this.button_kinsi.Text = "င်္";
            this.button_kinsi.UseVisualStyleBackColor = true;
            this.button_kinsi.Click += new System.EventHandler(this.HotKey);
            // 
            // button_ddha
            // 
            this.button_ddha.Font = new System.Drawing.Font("Myanmar Text", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_ddha.Location = new System.Drawing.Point(630, 36);
            this.button_ddha.Name = "button_ddha";
            this.button_ddha.Size = new System.Drawing.Size(25, 28);
            this.button_ddha.TabIndex = 19;
            this.button_ddha.Text = "ဍ္ဎ";
            this.button_ddha.UseVisualStyleBackColor = true;
            this.button_ddha.Click += new System.EventHandler(this.HotKey);
            // 
            // button_ttha
            // 
            this.button_ttha.Font = new System.Drawing.Font("Myanmar Text", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_ttha.Location = new System.Drawing.Point(606, 36);
            this.button_ttha.Name = "button_ttha";
            this.button_ttha.Size = new System.Drawing.Size(25, 28);
            this.button_ttha.TabIndex = 18;
            this.button_ttha.Text = "ဋ္ဌ";
            this.button_ttha.UseVisualStyleBackColor = true;
            this.button_ttha.Click += new System.EventHandler(this.HotKey);
            // 
            // label_MN
            // 
            this.label_MN.AutoSize = true;
            this.label_MN.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_MN.Location = new System.Drawing.Point(669, 44);
            this.label_MN.Name = "label_MN";
            this.label_MN.Size = new System.Drawing.Size(30, 15);
            this.label_MN.TabIndex = 20;
            this.label_MN.Text = "MN:";
            // 
            // textBox_MN
            // 
            this.textBox_MN.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_MN.Location = new System.Drawing.Point(697, 41);
            this.textBox_MN.Name = "textBox_MN";
            this.textBox_MN.Size = new System.Drawing.Size(50, 22);
            this.textBox_MN.TabIndex = 21;
            this.textBox_MN.TextChanged += new System.EventHandler(this.MN_TextChanged);
            this.textBox_MN.Leave += new System.EventHandler(this.MN_Leave);
            this.textBox_MN.MouseLeave += new System.EventHandler(this.MN_Leave);
            this.textBox_MN.Validating += new System.ComponentModel.CancelEventHandler(this.MN_Validating);
            // 
            // label_MNTitle
            // 
            this.label_MNTitle.AutoSize = true;
            this.label_MNTitle.Font = new System.Drawing.Font("Myanmar Text", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_MNTitle.Location = new System.Drawing.Point(753, 39);
            this.label_MNTitle.Name = "label_MNTitle";
            this.label_MNTitle.Size = new System.Drawing.Size(0, 29);
            this.label_MNTitle.TabIndex = 22;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            // 
            // bookStyleToolStripMenuItem
            // 
            this.bookStyleToolStripMenuItem.Name = "bookStyleToolStripMenuItem";
            this.bookStyleToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.bookStyleToolStripMenuItem.Text = "Book Style";
            this.bookStyleToolStripMenuItem.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.OldLace;
            this.ClientSize = new System.Drawing.Size(774, 521);
            this.Controls.Add(this.label_MNTitle);
            this.Controls.Add(this.textBox_MN);
            this.Controls.Add(this.label_MN);
            this.Controls.Add(this.button_ddha);
            this.Controls.Add(this.button_ttha);
            this.Controls.Add(this.button_kinsi);
            this.Controls.Add(this.button_jjha);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.textBox_FindStatus);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button_FindNext);
            this.Controls.Add(this.button_FindPrev);
            this.Controls.Add(this.button_X);
            this.Controls.Add(this.textBox_Find);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label_DataStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_Find);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Nissaya Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_Closing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.SizeUpdate);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem paliTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plainTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        public System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button_Find;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_DataStatus;
        private System.Windows.Forms.ToolStripMenuItem tableToolStripMenuItem;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolStripMenuItem fullDocToolStripMenuItem;
        private System.Windows.Forms.TextBox textBox_Find;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button button_X;
        private System.Windows.Forms.Button button_FindPrev;
        private System.Windows.Forms.Button button_FindNext;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_FindStatus;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printCtrlPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newCtrlNToolStripMenuItem;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Button button_jjha;
        private System.Windows.Forms.Button button_kinsi;
        private System.Windows.Forms.Button button_ddha;
        private System.Windows.Forms.Button button_ttha;
        private System.Windows.Forms.Label label_MN;
        private System.Windows.Forms.TextBox textBox_MN;
        private System.Windows.Forms.Label label_MNTitle;
        private System.Windows.Forms.ToolStripMenuItem bookStyleToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}

