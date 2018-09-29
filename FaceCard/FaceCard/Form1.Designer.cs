namespace FaceCard
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
            this.pictureBoxCam = new System.Windows.Forms.PictureBox();
            this.labelScore = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.textBoxFInfo = new System.Windows.Forms.TextBox();
            this.textBox2Pic = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCam)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxCam
            // 
            this.pictureBoxCam.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pictureBoxCam.Location = new System.Drawing.Point(17, 24);
            this.pictureBoxCam.Name = "pictureBoxCam";
            this.pictureBoxCam.Size = new System.Drawing.Size(781, 557);
            this.pictureBoxCam.TabIndex = 0;
            this.pictureBoxCam.TabStop = false;
            // 
            // labelScore
            // 
            this.labelScore.AutoSize = true;
            this.labelScore.Location = new System.Drawing.Point(14, 617);
            this.labelScore.Name = "labelScore";
            this.labelScore.Size = new System.Drawing.Size(98, 18);
            this.labelScore.TabIndex = 2;
            this.labelScore.Text = "人脸搜索：";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(816, 24);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(288, 34);
            this.button1.TabIndex = 3;
            this.button1.Text = "显示face2.jpg的人脸信息";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBoxFInfo
            // 
            this.textBoxFInfo.Location = new System.Drawing.Point(816, 66);
            this.textBoxFInfo.Multiline = true;
            this.textBoxFInfo.Name = "textBoxFInfo";
            this.textBoxFInfo.Size = new System.Drawing.Size(563, 347);
            this.textBoxFInfo.TabIndex = 4;
            // 
            // textBox2Pic
            // 
            this.textBox2Pic.Location = new System.Drawing.Point(816, 517);
            this.textBox2Pic.Multiline = true;
            this.textBox2Pic.Name = "textBox2Pic";
            this.textBox2Pic.Size = new System.Drawing.Size(563, 99);
            this.textBox2Pic.TabIndex = 6;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(816, 472);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(288, 33);
            this.button2.TabIndex = 5;
            this.button2.Text = "比对两张图片face1.jpg和face2.jpg";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 661);
            this.Controls.Add(this.textBox2Pic);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBoxFInfo);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.labelScore);
            this.Controls.Add(this.pictureBoxCam);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCam)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxCam;
        private System.Windows.Forms.Label labelScore;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBoxFInfo;
        private System.Windows.Forms.TextBox textBox2Pic;
        private System.Windows.Forms.Button button2;
    }
}

