using System;
using USF4_Music_Mod_Tool.Properties;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USF4_Music_Mod_Tool
{
    public partial class CSBExplained : Form
    {
        public CSBExplained()
        {
            InitializeComponent();
        }

        private void CSBExplained_Load(object sender, EventArgs e)
        {
            string rtf = Resources.Capcom_Sound_Bank ;
            rtfBox1.Rtf = rtf;
            rtfBox1.ReadOnly = true;
        }

        private void RtfBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}