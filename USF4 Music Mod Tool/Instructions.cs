using System;
using System.Windows.Forms;
using USF4_Music_Mod_Tool.Properties;

namespace USF4_Music_Mod_Tool
{
    public partial class Instructions : Form
    {
        public Instructions()
        {
            InitializeComponent();
        }

        private void Instructions_Load(object sender, EventArgs e)
        {
            string rtf = Resources.Instructions;
            rtfBox1.Rtf = rtf;
            rtfBox1.ReadOnly = true;
        }
    }
}
