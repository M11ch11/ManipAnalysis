using System;
using System.Windows.Forms;

namespace ManipAnalysis_v2
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