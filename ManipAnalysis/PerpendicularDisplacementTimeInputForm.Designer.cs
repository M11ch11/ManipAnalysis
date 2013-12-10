namespace ManipAnalysis
{
    partial class PerpendicularDisplacementTimeInputForm
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
            this.label_PleaseEnterTime = new System.Windows.Forms.Label();
            this.textBox_EnteredTime = new System.Windows.Forms.TextBox();
            this.button_OK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label_PleaseEnterTime
            // 
            this.label_PleaseEnterTime.AutoSize = true;
            this.label_PleaseEnterTime.Location = new System.Drawing.Point(12, 9);
            this.label_PleaseEnterTime.Name = "label_PleaseEnterTime";
            this.label_PleaseEnterTime.Size = new System.Drawing.Size(122, 13);
            this.label_PleaseEnterTime.TabIndex = 0;
            this.label_PleaseEnterTime.Text = "Please enter a time (ms):";
            // 
            // textBox_EnteredTime
            // 
            this.textBox_EnteredTime.Location = new System.Drawing.Point(140, 6);
            this.textBox_EnteredTime.Name = "textBox_EnteredTime";
            this.textBox_EnteredTime.Size = new System.Drawing.Size(32, 20);
            this.textBox_EnteredTime.TabIndex = 1;
            this.textBox_EnteredTime.Text = "250";
            // 
            // button_OK
            // 
            this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_OK.Location = new System.Drawing.Point(12, 32);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(160, 23);
            this.button_OK.TabIndex = 2;
            this.button_OK.Text = "OK";
            this.button_OK.UseVisualStyleBackColor = true;
            // 
            // PerpendicularDisplacementTimeInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(182, 61);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.textBox_EnteredTime);
            this.Controls.Add(this.label_PleaseEnterTime);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PerpendicularDisplacementTimeInputForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PerpendicularDisplacementTimeInputForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_PleaseEnterTime;
        private System.Windows.Forms.TextBox textBox_EnteredTime;
        private System.Windows.Forms.Button button_OK;
    }
}