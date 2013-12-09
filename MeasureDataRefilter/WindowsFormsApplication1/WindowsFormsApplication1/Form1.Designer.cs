namespace WindowsFormsApplication1
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.textBox_ForceCutoff = new System.Windows.Forms.TextBox();
            this.textBox_filterOrder = new System.Windows.Forms.TextBox();
            this.textBox_PosCutoff = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_SampleRate = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_NewSampleRate = new System.Windows.Forms.TextBox();
            this.label_eta = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(132, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Filter and normalize data";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(3, 41);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(661, 23);
            this.progressBar1.TabIndex = 1;
            // 
            // textBox_ForceCutoff
            // 
            this.textBox_ForceCutoff.Location = new System.Drawing.Point(353, 17);
            this.textBox_ForceCutoff.Name = "textBox_ForceCutoff";
            this.textBox_ForceCutoff.Size = new System.Drawing.Size(100, 20);
            this.textBox_ForceCutoff.TabIndex = 3;
            this.textBox_ForceCutoff.Text = "10";
            // 
            // textBox_filterOrder
            // 
            this.textBox_filterOrder.Location = new System.Drawing.Point(141, 15);
            this.textBox_filterOrder.Name = "textBox_filterOrder";
            this.textBox_filterOrder.Size = new System.Drawing.Size(100, 20);
            this.textBox_filterOrder.TabIndex = 4;
            this.textBox_filterOrder.Text = "2";
            // 
            // textBox_PosCutoff
            // 
            this.textBox_PosCutoff.Location = new System.Drawing.Point(247, 16);
            this.textBox_PosCutoff.Name = "textBox_PosCutoff";
            this.textBox_PosCutoff.Size = new System.Drawing.Size(100, 20);
            this.textBox_PosCutoff.TabIndex = 5;
            this.textBox_PosCutoff.Text = "6";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(138, -1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Filter order:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(247, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Position cutoff:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(353, 1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Force cutoff:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(458, 1);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Sample rate:";
            // 
            // textBox_SampleRate
            // 
            this.textBox_SampleRate.Location = new System.Drawing.Point(458, 17);
            this.textBox_SampleRate.Name = "textBox_SampleRate";
            this.textBox_SampleRate.Size = new System.Drawing.Size(100, 20);
            this.textBox_SampleRate.TabIndex = 9;
            this.textBox_SampleRate.Text = "200";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(564, 1);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "New sample rate:";
            // 
            // textBox_NewSampleRate
            // 
            this.textBox_NewSampleRate.Location = new System.Drawing.Point(564, 17);
            this.textBox_NewSampleRate.Name = "textBox_NewSampleRate";
            this.textBox_NewSampleRate.Size = new System.Drawing.Size(100, 20);
            this.textBox_NewSampleRate.TabIndex = 11;
            this.textBox_NewSampleRate.Text = "101";
            // 
            // label_eta
            // 
            this.label_eta.AutoSize = true;
            this.label_eta.Location = new System.Drawing.Point(0, 70);
            this.label_eta.Name = "label_eta";
            this.label_eta.Size = new System.Drawing.Size(31, 13);
            this.label_eta.TabIndex = 13;
            this.label_eta.Text = "ETA:";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(3, 92);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(661, 95);
            this.listBox1.TabIndex = 14;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 199);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.label_eta);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox_NewSampleRate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_SampleRate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_PosCutoff);
            this.Controls.Add(this.textBox_filterOrder);
            this.Controls.Add(this.textBox_ForceCutoff);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox textBox_ForceCutoff;
        private System.Windows.Forms.TextBox textBox_filterOrder;
        private System.Windows.Forms.TextBox textBox_PosCutoff;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_SampleRate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_NewSampleRate;
        private System.Windows.Forms.Label label_eta;
        private System.Windows.Forms.ListBox listBox1;
    }
}

