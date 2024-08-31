namespace NissayaEditor
{
    partial class MN_Catalog
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button_Done = new System.Windows.Forms.Button();
            this.button_Open = new System.Windows.Forms.Button();
            this.button_Files = new System.Windows.Forms.Button();
            this.label_Title = new System.Windows.Forms.Label();
            this.button_Save = new System.Windows.Forms.Button();
            this.button_UpArrow = new System.Windows.Forms.Button();
            this.button_DownArrow = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 35);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(534, 331);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.CellClick);
            // 
            // button_Done
            // 
            this.button_Done.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Done.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Done.Location = new System.Drawing.Point(473, 382);
            this.button_Done.Name = "button_Done";
            this.button_Done.Size = new System.Drawing.Size(75, 30);
            this.button_Done.TabIndex = 1;
            this.button_Done.Text = "Done";
            this.button_Done.UseVisualStyleBackColor = true;
            this.button_Done.Click += new System.EventHandler(this.button_Done_Click);
            // 
            // button_Open
            // 
            this.button_Open.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Open.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Open.Location = new System.Drawing.Point(375, 382);
            this.button_Open.Name = "button_Open";
            this.button_Open.Size = new System.Drawing.Size(75, 30);
            this.button_Open.TabIndex = 2;
            this.button_Open.Text = "Open";
            this.button_Open.UseVisualStyleBackColor = true;
            this.button_Open.Click += new System.EventHandler(this.button_Open_Click);
            // 
            // button_Files
            // 
            this.button_Files.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Files.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Files.Location = new System.Drawing.Point(12, 382);
            this.button_Files.Name = "button_Files";
            this.button_Files.Size = new System.Drawing.Size(75, 30);
            this.button_Files.TabIndex = 3;
            this.button_Files.Text = "Files";
            this.button_Files.UseVisualStyleBackColor = true;
            this.button_Files.Click += new System.EventHandler(this.button_Files_Click);
            // 
            // label_Title
            // 
            this.label_Title.AutoSize = true;
            this.label_Title.Font = new System.Drawing.Font("Myanmar Text", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Title.Location = new System.Drawing.Point(12, 5);
            this.label_Title.Name = "label_Title";
            this.label_Title.Size = new System.Drawing.Size(0, 29);
            this.label_Title.TabIndex = 4;
            // 
            // button_Save
            // 
            this.button_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Save.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Save.Location = new System.Drawing.Point(109, 382);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(75, 30);
            this.button_Save.TabIndex = 5;
            this.button_Save.Text = "Save";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // button_UpArrow
            // 
            this.button_UpArrow.Image = global::NissayaEditor.Properties.Resources.UpArrowHead;
            this.button_UpArrow.Location = new System.Drawing.Point(246, 382);
            this.button_UpArrow.Name = "button_UpArrow";
            this.button_UpArrow.Size = new System.Drawing.Size(32, 32);
            this.button_UpArrow.TabIndex = 6;
            this.button_UpArrow.UseVisualStyleBackColor = true;
            this.button_UpArrow.Click += new System.EventHandler(this.button_UpArrow_Click);
            // 
            // button_DownArrow
            // 
            this.button_DownArrow.Image = global::NissayaEditor.Properties.Resources.DownArrowHead;
            this.button_DownArrow.Location = new System.Drawing.Point(290, 382);
            this.button_DownArrow.Name = "button_DownArrow";
            this.button_DownArrow.Size = new System.Drawing.Size(32, 32);
            this.button_DownArrow.TabIndex = 7;
            this.button_DownArrow.UseVisualStyleBackColor = true;
            this.button_DownArrow.Click += new System.EventHandler(this.button_DownArrow_Click);
            // 
            // MN_Catalog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 424);
            this.Controls.Add(this.button_DownArrow);
            this.Controls.Add(this.button_UpArrow);
            this.Controls.Add(this.button_Save);
            this.Controls.Add(this.label_Title);
            this.Controls.Add(this.button_Files);
            this.Controls.Add(this.button_Open);
            this.Controls.Add(this.button_Done);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MN_Catalog";
            this.Text = "Majjhima Nikāya Nissaya Catalog";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button_Done;
        private System.Windows.Forms.Button button_Open;
        private System.Windows.Forms.Button button_Files;
        private System.Windows.Forms.Label label_Title;
        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.Button button_UpArrow;
        private System.Windows.Forms.Button button_DownArrow;
    }
}