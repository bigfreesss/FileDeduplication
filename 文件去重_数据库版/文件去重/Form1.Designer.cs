namespace 文件去重
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.Multifile = new System.Windows.Forms.CheckBox();
            this.button_recovery = new System.Windows.Forms.Button();
            this.button_recoveryLog = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.checkBox_md5 = new System.Windows.Forms.CheckBox();
            this.checkBox_Hash = new System.Windows.Forms.CheckBox();
            this.checkBox_sha256 = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.checkBox_DeleteNullFile = new System.Windows.Forms.CheckBox();
            this.button_SQLempty = new System.Windows.Forms.Button();
            this.checkBox_sql = new System.Windows.Forms.CheckBox();
            this.button_SQLAuditFile = new System.Windows.Forms.Button();
            this.button_recoverySQL = new System.Windows.Forms.Button();
            this.checkBox_test = new System.Windows.Forms.CheckBox();
            this.radioButton_MoveRoot = new System.Windows.Forms.RadioButton();
            this.radioButton_Delete = new System.Windows.Forms.RadioButton();
            this.radioButton_Move = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 80);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "开始";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(329, 21);
            this.textBox1.TabIndex = 1;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(347, 12);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(196, 21);
            this.textBox2.TabIndex = 2;
            this.textBox2.Text = ".jpg.png.mp4.bmp.jpeg.gif.webp";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 39);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(531, 23);
            this.progressBar1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "0/0";
            // 
            // Multifile
            // 
            this.Multifile.AutoSize = true;
            this.Multifile.Checked = true;
            this.Multifile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Multifile.Location = new System.Drawing.Point(264, 84);
            this.Multifile.Name = "Multifile";
            this.Multifile.Size = new System.Drawing.Size(114, 16);
            this.Multifile.TabIndex = 0;
            this.Multifile.Text = "多文件（+连接）";
            this.Multifile.UseVisualStyleBackColor = true;
            // 
            // button_recovery
            // 
            this.button_recovery.Location = new System.Drawing.Point(387, 80);
            this.button_recovery.Name = "button_recovery";
            this.button_recovery.Size = new System.Drawing.Size(75, 23);
            this.button_recovery.TabIndex = 5;
            this.button_recovery.Text = "即时还原";
            this.button_recovery.UseVisualStyleBackColor = true;
            this.button_recovery.Click += new System.EventHandler(this.button_recovery_Click);
            // 
            // button_recoveryLog
            // 
            this.button_recoveryLog.Location = new System.Drawing.Point(468, 80);
            this.button_recoveryLog.Name = "button_recoveryLog";
            this.button_recoveryLog.Size = new System.Drawing.Size(75, 23);
            this.button_recoveryLog.TabIndex = 6;
            this.button_recoveryLog.Text = "日志还原";
            this.button_recoveryLog.UseVisualStyleBackColor = true;
            this.button_recoveryLog.Click += new System.EventHandler(this.button_recoveryLog_Click);
            // 
            // button2
            // 
            this.button2.AutoSize = true;
            this.button2.Location = new System.Drawing.Point(93, 80);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(87, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "选择文件夹";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // checkBox_md5
            // 
            this.checkBox_md5.AutoSize = true;
            this.checkBox_md5.Checked = true;
            this.checkBox_md5.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_md5.Location = new System.Drawing.Point(12, 110);
            this.checkBox_md5.Name = "checkBox_md5";
            this.checkBox_md5.Size = new System.Drawing.Size(42, 16);
            this.checkBox_md5.TabIndex = 8;
            this.checkBox_md5.Text = "md5";
            this.checkBox_md5.UseVisualStyleBackColor = true;
            // 
            // checkBox_Hash
            // 
            this.checkBox_Hash.AutoSize = true;
            this.checkBox_Hash.Checked = true;
            this.checkBox_Hash.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_Hash.Location = new System.Drawing.Point(60, 110);
            this.checkBox_Hash.Name = "checkBox_Hash";
            this.checkBox_Hash.Size = new System.Drawing.Size(84, 16);
            this.checkBox_Hash.TabIndex = 9;
            this.checkBox_Hash.Text = "Hash(sha1)";
            this.checkBox_Hash.UseVisualStyleBackColor = true;
            // 
            // checkBox_sha256
            // 
            this.checkBox_sha256.AutoSize = true;
            this.checkBox_sha256.Checked = true;
            this.checkBox_sha256.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_sha256.Location = new System.Drawing.Point(150, 110);
            this.checkBox_sha256.Name = "checkBox_sha256";
            this.checkBox_sha256.Size = new System.Drawing.Size(60, 16);
            this.checkBox_sha256.TabIndex = 10;
            this.checkBox_sha256.Text = "sha256";
            this.checkBox_sha256.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.AutoSize = true;
            this.button3.Location = new System.Drawing.Point(183, 80);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 12;
            this.button3.Text = "仅当前目录";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // checkBox_DeleteNullFile
            // 
            this.checkBox_DeleteNullFile.AutoSize = true;
            this.checkBox_DeleteNullFile.Location = new System.Drawing.Point(375, 137);
            this.checkBox_DeleteNullFile.Name = "checkBox_DeleteNullFile";
            this.checkBox_DeleteNullFile.Size = new System.Drawing.Size(96, 16);
            this.checkBox_DeleteNullFile.TabIndex = 13;
            this.checkBox_DeleteNullFile.Text = "删除空文件夹";
            this.checkBox_DeleteNullFile.UseVisualStyleBackColor = true;
            // 
            // button_SQLempty
            // 
            this.button_SQLempty.AutoSize = true;
            this.button_SQLempty.Location = new System.Drawing.Point(201, 133);
            this.button_SQLempty.Name = "button_SQLempty";
            this.button_SQLempty.Size = new System.Drawing.Size(75, 23);
            this.button_SQLempty.TabIndex = 15;
            this.button_SQLempty.Text = "清空数据库";
            this.button_SQLempty.UseVisualStyleBackColor = true;
            this.button_SQLempty.Click += new System.EventHandler(this.button_SQLempty_Click);
            // 
            // checkBox_sql
            // 
            this.checkBox_sql.AutoSize = true;
            this.checkBox_sql.Checked = true;
            this.checkBox_sql.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_sql.Location = new System.Drawing.Point(12, 137);
            this.checkBox_sql.Name = "checkBox_sql";
            this.checkBox_sql.Size = new System.Drawing.Size(120, 16);
            this.checkBox_sql.TabIndex = 16;
            this.checkBox_sql.Text = "启用数据库内数据";
            this.checkBox_sql.UseVisualStyleBackColor = true;
            // 
            // button_SQLAuditFile
            // 
            this.button_SQLAuditFile.AutoSize = true;
            this.button_SQLAuditFile.Location = new System.Drawing.Point(132, 133);
            this.button_SQLAuditFile.Name = "button_SQLAuditFile";
            this.button_SQLAuditFile.Size = new System.Drawing.Size(63, 23);
            this.button_SQLAuditFile.TabIndex = 14;
            this.button_SQLAuditFile.Text = "检查文件";
            this.button_SQLAuditFile.UseVisualStyleBackColor = true;
            this.button_SQLAuditFile.Click += new System.EventHandler(this.button_SQLAuditFile_Click);
            // 
            // button_recoverySQL
            // 
            this.button_recoverySQL.AutoSize = true;
            this.button_recoverySQL.Location = new System.Drawing.Point(282, 133);
            this.button_recoverySQL.Name = "button_recoverySQL";
            this.button_recoverySQL.Size = new System.Drawing.Size(87, 23);
            this.button_recoverySQL.TabIndex = 17;
            this.button_recoverySQL.Text = "按数据库还原";
            this.button_recoverySQL.UseVisualStyleBackColor = true;
            this.button_recoverySQL.Click += new System.EventHandler(this.button_recoverySQL_Click);
            // 
            // checkBox_test
            // 
            this.checkBox_test.AutoSize = true;
            this.checkBox_test.Location = new System.Drawing.Point(471, 137);
            this.checkBox_test.Name = "checkBox_test";
            this.checkBox_test.Size = new System.Drawing.Size(72, 16);
            this.checkBox_test.TabIndex = 18;
            this.checkBox_test.Text = "测试模式";
            this.checkBox_test.UseVisualStyleBackColor = true;
            // 
            // radioButton_MoveRoot
            // 
            this.radioButton_MoveRoot.AutoSize = true;
            this.radioButton_MoveRoot.Checked = true;
            this.radioButton_MoveRoot.Location = new System.Drawing.Point(222, 109);
            this.radioButton_MoveRoot.Name = "radioButton_MoveRoot";
            this.radioButton_MoveRoot.Size = new System.Drawing.Size(131, 16);
            this.radioButton_MoveRoot.TabIndex = 19;
            this.radioButton_MoveRoot.TabStop = true;
            this.radioButton_MoveRoot.Text = "移动到根目录文件夹";
            this.radioButton_MoveRoot.UseVisualStyleBackColor = true;
            // 
            // radioButton_Delete
            // 
            this.radioButton_Delete.AutoSize = true;
            this.radioButton_Delete.Location = new System.Drawing.Point(359, 109);
            this.radioButton_Delete.Name = "radioButton_Delete";
            this.radioButton_Delete.Size = new System.Drawing.Size(71, 16);
            this.radioButton_Delete.TabIndex = 20;
            this.radioButton_Delete.Text = "直接删除";
            this.radioButton_Delete.UseVisualStyleBackColor = true;
            // 
            // radioButton_Move
            // 
            this.radioButton_Move.AutoSize = true;
            this.radioButton_Move.Location = new System.Drawing.Point(436, 109);
            this.radioButton_Move.Name = "radioButton_Move";
            this.radioButton_Move.Size = new System.Drawing.Size(107, 16);
            this.radioButton_Move.TabIndex = 21;
            this.radioButton_Move.Text = "移动到相对目录";
            this.radioButton_Move.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(556, 165);
            this.Controls.Add(this.radioButton_Move);
            this.Controls.Add(this.radioButton_Delete);
            this.Controls.Add(this.radioButton_MoveRoot);
            this.Controls.Add(this.checkBox_test);
            this.Controls.Add(this.button_recoverySQL);
            this.Controls.Add(this.button_SQLAuditFile);
            this.Controls.Add(this.checkBox_sql);
            this.Controls.Add(this.button_SQLempty);
            this.Controls.Add(this.checkBox_DeleteNullFile);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.checkBox_sha256);
            this.Controls.Add(this.checkBox_Hash);
            this.Controls.Add(this.checkBox_md5);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button_recoveryLog);
            this.Controls.Add(this.button_recovery);
            this.Controls.Add(this.Multifile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "文件去重";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox Multifile;
        private System.Windows.Forms.Button button_recovery;
        private System.Windows.Forms.Button button_recoveryLog;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox checkBox_md5;
        private System.Windows.Forms.CheckBox checkBox_Hash;
        private System.Windows.Forms.CheckBox checkBox_sha256;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckBox checkBox_DeleteNullFile;
        private System.Windows.Forms.Button button_SQLempty;
        private System.Windows.Forms.CheckBox checkBox_sql;
        private System.Windows.Forms.Button button_SQLAuditFile;
        private System.Windows.Forms.Button button_recoverySQL;
        private System.Windows.Forms.CheckBox checkBox_test;
        private System.Windows.Forms.RadioButton radioButton_MoveRoot;
        private System.Windows.Forms.RadioButton radioButton_Delete;
        private System.Windows.Forms.RadioButton radioButton_Move;
    }
}

