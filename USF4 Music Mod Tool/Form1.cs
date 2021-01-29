using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace USF4_Music_Mod_Tool
{
    public partial class Form1 : Form
    {
        private string WorkingStageName;
        private string StageMusicFolder;
        private string MainMenuMusicFolder;
        private bool NothingSelected = true;
        private PictureBox targetPB;
        private Label targetLB;
        private bool SelectedIsStage;
        private InputSimulator inputSimulator = new InputSimulator();

        public Form1()
        {
            InitializeComponent();
            ReadSettings();
            SetDirectory();
            pickFile.FileName = "";
        }

        #region Program Settings
        public void SaveSettings()
        {
            string settingString = "";
            settingString = $"{InstallLocation.Text},{tbSFAM.Text}";
            File.WriteAllText("./Settings.cfg", settingString.TrimEnd(null));
        }

        private void ReadSettings()
        {
            if (File.Exists("./Settings.cfg"))
            {
                string settingString = "";
                string[] settings;
                settingString = File.ReadAllText("./Settings.cfg").Replace(Environment.NewLine, "");
                settings = settingString.Split(',');
                InstallLocation.Text = settings[0];
                tbSFAM.Text = settings[1];
            }
        }
        #endregion Program Settings

        private void SetDirectory()
        {
            StageMusicFolder = InstallLocation.Text + "\\patch_ae2_tu1\\battle\\sound\\bgm";
            MainMenuMusicFolder = InstallLocation.Text + "\\patch_ae2_tu1\\ui\\sound\\bgm";
        }

        private void STAGECLICK(object sender, MouseEventArgs e)
        {
            PictureBox newPB = (PictureBox)sender;
            
            if (targetPB != null && newPB.Name == targetPB.Name) return;
            if (targetPB != null)
            {
                targetPB.BorderStyle = BorderStyle.FixedSingle;
                targetPB.BackColor = Color.Gray;
            }
            targetPB = (PictureBox)sender;
            targetPB.BorderStyle = BorderStyle.Fixed3D;
            targetPB.BackColor = Color.Red;
            WorkingStageName =targetPB.Name;

            string labelName = "lb" + WorkingStageName;
            if (targetLB != null) targetLB.BackColor = Color.Transparent;
            Label newLB = Controls.Find(labelName, true).FirstOrDefault() as Label;
            targetLB = newLB;
            targetLB.BackColor = Color.LimeGreen;
            SelectedIsStage = true;
            NothingSelected = false;
        }

        private void UICLICK(object sender, MouseEventArgs e)
        {
            PictureBox newPB = (PictureBox)sender;

            if (targetPB != null && newPB.Name == targetPB.Name) return;
            if (targetPB != null)
            {
                targetPB.BorderStyle = BorderStyle.FixedSingle;
                targetPB.BackColor = Color.Gray;
            }  
            targetPB = (PictureBox)sender;
            targetPB.BorderStyle = BorderStyle.Fixed3D;
            targetPB.BackColor = Color.Red;
            WorkingStageName = targetPB.Name;

            string labelName = "lb" + WorkingStageName;
            if (targetLB != null) targetLB.BackColor = Color.Transparent;
            Label newLB = Controls.Find(labelName, true).FirstOrDefault() as Label;
            targetLB = newLB;
            targetLB.BackColor = Color.LimeGreen;
            SelectedIsStage = false;
            NothingSelected = false;
        }
        
        private void SaveCSB ()
        {
            if (WorkingStageName == "") return;
            string FullFilename;
            if (SelectedIsStage)
            {
                Directory.CreateDirectory(StageMusicFolder);
                FullFilename = StageMusicFolder + "\\" + "BGM_" + WorkingStageName + ".csb";
            }
            else
            {
                Directory.CreateDirectory(MainMenuMusicFolder);
                FullFilename = MainMenuMusicFolder + "\\" + "BGM_" + WorkingStageName + ".csb";
            }
            
            if (File.Exists(FullFilename)) { File.Delete(FullFilename); }
            File.Copy(SourceCSB.Text, FullFilename);
            //MessageBox.Show("Install Done!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void InstallFile_Click(object sender, EventArgs e)
        {
           if (NothingSelected) { MessageBox.Show("Select a target Stage!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            if (InstallLocation.Text == "") {MessageBox.Show("Game Install Folder is empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
           if (SourceCSB.Text == "") {MessageBox.Show("No source CSB!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            SaveCSB();
        }

        private void PickGameDir_Click(object sender, EventArgs e)
        {
            folderPicker.ShowNewFolderButton = false;
            folderPicker.ShowDialog();
            if (folderPicker.SelectedPath != "") InstallLocation.Text = folderPicker.SelectedPath;
            SetDirectory();
            SaveSettings();
        }

        private void PickSourceCSB_Click(object sender, EventArgs e)
        {
            pickFile.RestoreDirectory = true;
            pickFile.Filter = "Capcom Sound Bank (*.csb)|*.csb"; //"Text files (*.txt)|*.txt

            pickFile.ShowDialog();
            if (pickFile.FileName != "") SourceCSB.Text = pickFile.FileName;
        }

        private void BtnPreview_Click(object sender, EventArgs e)
        {
            if (tbSFAM.Text == "") { MessageBox.Show("SFIVAM not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            if (SourceCSB.Text == "") { MessageBox.Show("Select CSB!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            var proc = Process.Start(new ProcessStartInfo(tbSFAM.Text));

            System.Threading.Thread.Sleep(100);
            inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.VK_F);
            inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
            System.Threading.Thread.Sleep(100);
            Clipboard.SetText(SourceCSB.Text);
            System.Threading.Thread.Sleep(100);
            inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
            System.Threading.Thread.Sleep(100);
            inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
        }

        private void BtnLocateSFAM_Click(object sender, EventArgs e)
        {
            pickFile.RestoreDirectory = true;
            pickFile.Filter = "SFIV Audio Manager (SFIVAM.exe)|*.exe"; //"Text files (*.txt)|*.txt

            pickFile.ShowDialog();
            if (pickFile.FileName != "") tbSFAM.Text = pickFile.FileName;
            SaveSettings();
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(linkLabel1.Text);
            Process.Start(sInfo);
        }

        private void InfoOnCSBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CSBExplained newForm = new CSBExplained();
            newForm.ShowDialog();
        }

        private void InstructionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Instructions newForm = new Instructions();
            newForm.ShowDialog();
        }
    }
}