namespace SQLRegex
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.dontSaveOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openXLSOnFinishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionStringTB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLB = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.label6 = new System.Windows.Forms.Label();
            this.whereTB = new System.Windows.Forms.TextBox();
            this.QueryB = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.headerColumnsInAZOrderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Silver;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(539, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.dontSaveOptionsToolStripMenuItem,
            this.openXLSOnFinishToolStripMenuItem,
            this.headerColumnsInAZOrderToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(220, 6);
            // 
            // dontSaveOptionsToolStripMenuItem
            // 
            this.dontSaveOptionsToolStripMenuItem.CheckOnClick = true;
            this.dontSaveOptionsToolStripMenuItem.Name = "dontSaveOptionsToolStripMenuItem";
            this.dontSaveOptionsToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.dontSaveOptionsToolStripMenuItem.Text = "Don\'t save options";
            // 
            // openXLSOnFinishToolStripMenuItem
            // 
            this.openXLSOnFinishToolStripMenuItem.Checked = true;
            this.openXLSOnFinishToolStripMenuItem.CheckOnClick = true;
            this.openXLSOnFinishToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.openXLSOnFinishToolStripMenuItem.Name = "openXLSOnFinishToolStripMenuItem";
            this.openXLSOnFinishToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.openXLSOnFinishToolStripMenuItem.Text = "Open XLS on finish";
            // 
            // connectionStringTB
            // 
            this.connectionStringTB.Location = new System.Drawing.Point(15, 56);
            this.connectionStringTB.Name = "connectionStringTB";
            this.connectionStringTB.Size = new System.Drawing.Size(470, 20);
            this.connectionStringTB.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Connection String";
            // 
            // tableLB
            // 
            this.tableLB.FormattingEnabled = true;
            this.tableLB.Location = new System.Drawing.Point(20, 98);
            this.tableLB.Name = "tableLB";
            this.tableLB.Size = new System.Drawing.Size(237, 121);
            this.tableLB.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Table";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 318);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(539, 22);
            this.statusStrip1.TabIndex = 14;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 232);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Where...";
            // 
            // whereTB
            // 
            this.whereTB.Location = new System.Drawing.Point(20, 248);
            this.whereTB.Name = "whereTB";
            this.whereTB.Size = new System.Drawing.Size(480, 20);
            this.whereTB.TabIndex = 15;
            this.whereTB.Text = "table id in x";
            // 
            // QueryB
            // 
            this.QueryB.Location = new System.Drawing.Point(20, 274);
            this.QueryB.Name = "QueryB";
            this.QueryB.Size = new System.Drawing.Size(75, 23);
            this.QueryB.TabIndex = 17;
            this.QueryB.Text = "Query";
            this.QueryB.UseVisualStyleBackColor = true;
            this.QueryB.Click += new System.EventHandler(this.QueryB_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(263, 98);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 18;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // headerColumnsInAZOrderToolStripMenuItem
            // 
            this.headerColumnsInAZOrderToolStripMenuItem.CheckOnClick = true;
            this.headerColumnsInAZOrderToolStripMenuItem.Name = "headerColumnsInAZOrderToolStripMenuItem";
            this.headerColumnsInAZOrderToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.headerColumnsInAZOrderToolStripMenuItem.Text = "Header columns in AZ order";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 340);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.QueryB);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.whereTB);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tableLB);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.connectionStringTB);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TextBox connectionStringTB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox tableLB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox whereTB;
        private System.Windows.Forms.Button QueryB;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dontSaveOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripMenuItem openXLSOnFinishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem headerColumnsInAZOrderToolStripMenuItem;
    }
}

