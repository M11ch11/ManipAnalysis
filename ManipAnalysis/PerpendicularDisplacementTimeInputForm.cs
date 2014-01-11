using System;
using System.Windows.Forms;

namespace ManipAnalysis
{
    public partial class PerpendicularDisplacementTimeInputForm : Form
    {
        public PerpendicularDisplacementTimeInputForm()
        {
            InitializeComponent();
        }

        public int getMilliseconds()
        {
            return Convert.ToInt32(textBox_EnteredTime.Text);
        }
    }
}