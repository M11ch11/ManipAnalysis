using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
