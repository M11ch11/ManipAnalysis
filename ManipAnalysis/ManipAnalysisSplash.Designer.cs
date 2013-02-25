namespace ManipAnalysis
{
    partial class ManipAnalysisSplash
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManipAnalysisSplash));
            this.label_ManipAnalysis = new System.Windows.Forms.Label();
            this.label_Loading = new System.Windows.Forms.Label();
            this.pictureBox_Impressum_KITLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Impressum_KITLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // label_ManipAnalysis
            // 
            this.label_ManipAnalysis.AutoSize = true;
            this.label_ManipAnalysis.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_ManipAnalysis.Location = new System.Drawing.Point(180, 249);
            this.label_ManipAnalysis.Name = "label_ManipAnalysis";
            this.label_ManipAnalysis.Size = new System.Drawing.Size(204, 33);
            this.label_ManipAnalysis.TabIndex = 0;
            this.label_ManipAnalysis.Text = "ManipAnalysis";
            this.label_ManipAnalysis.UseWaitCursor = true;
            // 
            // label_Loading
            // 
            this.label_Loading.AutoSize = true;
            this.label_Loading.Location = new System.Drawing.Point(242, 291);
            this.label_Loading.Name = "label_Loading";
            this.label_Loading.Size = new System.Drawing.Size(54, 13);
            this.label_Loading.TabIndex = 1;
            this.label_Loading.Text = "Loading...";
            this.label_Loading.UseWaitCursor = true;
            // 
            // pictureBox_Impressum_KITLogo
            // 
            this.pictureBox_Impressum_KITLogo.Image = global::ManipAnalysis.Properties.Resources.KITLogo;
            this.pictureBox_Impressum_KITLogo.Location = new System.Drawing.Point(12, 12);
            this.pictureBox_Impressum_KITLogo.Name = "pictureBox_Impressum_KITLogo";
            this.pictureBox_Impressum_KITLogo.Size = new System.Drawing.Size(502, 234);
            this.pictureBox_Impressum_KITLogo.TabIndex = 2;
            this.pictureBox_Impressum_KITLogo.TabStop = false;
            this.pictureBox_Impressum_KITLogo.UseWaitCursor = true;
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(526, 311);
            this.Controls.Add(this.pictureBox_Impressum_KITLogo);
            this.Controls.Add(this.label_Loading);
            this.Controls.Add(this.label_ManipAnalysis);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SplashScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SplashScreen";
            this.TransparencyKey = System.Drawing.SystemColors.Control;
            this.UseWaitCursor = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Impressum_KITLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_ManipAnalysis;
        private System.Windows.Forms.Label label_Loading;
        private System.Windows.Forms.PictureBox pictureBox_Impressum_KITLogo;
    }
}