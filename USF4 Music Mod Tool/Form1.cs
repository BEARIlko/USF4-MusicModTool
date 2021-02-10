using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;
using NAudio.Wave;

namespace USF4_Music_Mod_Tool
{
    public partial class Form1 : Form
    {
        //File types filters

		public const string TXTFileFilter = "Text file (.txt)|*.txt";
		public const string WavFileFilter = "Wave  (.wav)|*.wav";
		public const string CSBFileFilter = "Sound Bank (.csb)|*.csb";
		public const string ADXFileFilter = "ADX (.adx)|*.adx";

        private string WorkingStageName;
        private string StageMusicFolder;
        private string MainMenuMusicFolder;
        private string CSBsSourceFolder;
        private bool NothingSelected = true;
        private PictureBox targetPB;
        private Label targetLB;
        private bool SelectedIsStage;
        private InputSimulator inputSimulator = new InputSimulator();
        //This will give us the full name path of the executable file:
        //i.e. C:\Program Files\MyApplication\MyApplication.exe
        string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        //This will strip just the working path name:
        //C:\Program Files\MyApplication
        string WorkPath;
        string ADXPath, CSBPath, TEMPPath;
        List<string> FilesToEncode =  new List<string>();
        
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
            WorkPath = Path.GetDirectoryName(strExeFilePath);
            ADXPath = WorkPath + "\\ADX";
            CSBPath = WorkPath + "\\CSB";
            TEMPPath = WorkPath + "\\NewCSB";
        }

        void EncodeLoadedFiles(List<string> wavs)
        {
	        for (int i = 0; i < wavs.Count; i++)
	        {
                Directory.CreateDirectory(ADXPath);
		        string wavPath = wavs[i];

                //Check if it's mp3
                if (wavPath.EndsWith(".mp3"))
				{
                    string mp3Path = wavPath.Replace(".mp3", ".wav");
                    ConvertMp3ToWav(wavPath, mp3Path);
                    wavPath = wavPath.Replace(".mp3", ".wav");
				}

                //Waved
		        string arguments = "\"" + wavPath + "\" \"" + ADXPath + "\"";
		        ProcessStartInfo sEncode = new ProcessStartInfo("adxencd.exe", arguments);
                Console.WriteLine("adxencd.exe " + arguments);
				var process = Process.Start(sEncode);
                process.WaitForExit();
			}
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
            pickFile.Filter = CSBFileFilter;

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

		private void TRL_Click(object sender, EventArgs e)
		{

		}

        void LoadWAVFiles()
        {
            diagPickWAVS.RestoreDirectory = true;
			diagPickWAVS.FileName = string.Empty;
			//diagPickWAVS.Filter = WavFileFilter;

            if (diagPickWAVS.ShowDialog() == DialogResult.OK)
            {
               listBox1.Items.Clear();
               FilesToEncode.Clear();
               string filepath = diagPickWAVS.FileName;
               string dir = Path.GetDirectoryName(filepath);
               string[] Files = diagPickWAVS.FileNames;
				foreach (string s in Files)
				{
                    string File =  s;
                    listBox1.Items.Add(File);
                    FilesToEncode.Add(File);
				}
            }
        }

        private static void ConvertMp3ToWav(string _inPath_, string _outPath_)
        {
            using (Mp3FileReader mp3 = new Mp3FileReader(_inPath_))
            {
                using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
                {
                    WaveFileWriter.CreateWaveFile(_outPath_, pcm);
                }
            }
        }

		private void btnConvertWav_Click(object sender, EventArgs e)
		{
            EncodeLoadedFiles(FilesToEncode);
		}

		private void pickADX1_Click(object sender, EventArgs e)
		{
            Button b = (Button)sender;
            PickADXFile(Convert.ToInt32(b.Tag));
		}

		private void btnCreateCSB_Click(object sender, EventArgs e)
		{
            //Create Directories
            Directory.CreateDirectory(TEMPPath);
            string StageBase = TEMPPath + "\\Synth\\STAGE\\USA";
            Directory.CreateDirectory(StageBase);
            string STG_EVENT_Normal = StageBase + "\\s_bgm_0f_wav";
            string STG_EVENT_Ultra = StageBase + "\\s_bgm_10_wav";
            string STG_EVENT_LowHP = StageBase + "\\s_bgm_11_wav";
            Directory.CreateDirectory(STG_EVENT_Normal);
            Directory.CreateDirectory(STG_EVENT_Ultra);
            Directory.CreateDirectory(STG_EVENT_LowHP);

            //Copy target ADXs to TEMP
            File.Copy(adxFileName1.Text, STG_EVENT_Normal + "\\Intro.adx", true);
            File.Copy(adxFileName2.Text, STG_EVENT_Normal + "\\Loop.adx", true);
            File.Copy(adxFileName3.Text, STG_EVENT_Ultra + "\\Intro.adx", true);
            File.Copy(adxFileName4.Text, STG_EVENT_Ultra + "\\Loop.adx", true);
            File.Copy(adxFileName5.Text, STG_EVENT_LowHP + "\\Intro.adx", true);
            File.Copy(adxFileName6.Text, STG_EVENT_LowHP + "\\Loop.adx", true);

            //Run CSBEditor
           //File.Copy(@"Resources\BGM_00.csb",  WorkPath + "\\NewCreatedCSB.csb", true);
            string arguments = string.Empty;
            arguments = "\""+TEMPPath + "\"";
            ProcessStartInfo sEncode = new ProcessStartInfo("CSBEditor.exe", arguments);
			var process = Process.Start(sEncode);
            process.WaitForExit();
            MessageBox.Show("NewCSB.csb is updated with the selected ADX files.");
		}

		void PickADXFile(int ADXID)
        {
            pickADXDialog.RestoreDirectory = true;
            pickADXDialog.FileName = string.Empty;
            pickADXDialog.Filter = ADXFileFilter;

            if (pickADXDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = pickADXDialog.FileName;
                Controls.Find("adxFileName" + ADXID,true)[0].Text = filename;
            }
        }

		private void btnLoadADX_Click(object sender, EventArgs e)
		{
            LoadWAVFiles();
		}
	}
}