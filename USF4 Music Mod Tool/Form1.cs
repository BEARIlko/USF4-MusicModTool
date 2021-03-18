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
using System.Timers;
using Timer = System.Windows.Forms.Timer;

namespace USF4_Music_Mod_Tool
{
    public partial class Form1 : Form
    {
        //File types filters
		public const string TXTFileFilter = "Text file (.txt)|*.txt";
		public const string WavFileFilter = "Wave  (.wav)|*.wav";
		public const string CSBFileFilter = "Sound Bank (.csb)|*.csb";
		public const string ADXFileFilter = "ADX (.adx)|*.adx";
        private string WorkingCSBName = "temp.csb";

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
        string exePath;
        string workingPath, TEMPPath;
        List<string> FilesToEncode =  new List<string>();

        private Timer RMTimer = new Timer();

        public string ScrollSelected = "\\";

        public Form1()
        {
            RMTimer.Tick += new EventHandler(RMTimer_Tick);
            InitializeComponent();
            ReadSettings();
            SetDirectory();
            pickFile.FileName = "";
        }

        #region Program Settings
        public void SaveSettings()
        {
            string settingString = "";
            settingString = $"{InstallLocation.Text},{tbSFAM.Text},{rmSource.Text}";
            File.WriteAllText("./Settings.cfg", settingString.TrimEnd(null));
        }

        private void ReadSettings()
        {
            if (File.Exists("./Settings.cfg"))
            {
                try
                {
                    string settingString = "";
                    string[] settings;
                    settingString = File.ReadAllText("./Settings.cfg").Replace(Environment.NewLine, "");
                    settings = settingString.Split(',');
                    InstallLocation.Text = settings[0];
                    tbSFAM.Text = settings[1];
                    rmSource.Text = settings[2];
                    CSBsSourceFolder = rmSource.Text;
                }
                catch
                {
                    Console.WriteLine("Oppsies Config!!");
                }
            }
        }
        #endregion Program Settings

        private void SetDirectory()
        {
            StageMusicFolder = InstallLocation.Text + "\\patch_ae2_tu1\\battle\\sound\\bgm";
            MainMenuMusicFolder = InstallLocation.Text + "\\patch_ae2_tu1\\ui\\sound\\bgm";
            exePath = Path.GetDirectoryName(strExeFilePath);
            workingPath = exePath + "\\working";
            TEMPPath = exePath + "\\temp";
        }

        void EncodeLoadedFiles(List<string> wavs)
        {
           string currentWav =exePath + "\\CurrentWav.wav";
	        for (int i = 0; i < wavs.Count; i++)
	        {
				string wavPath = wavs[i];

                //Check if it's mp3
                if (wavPath.EndsWith(".mp3"))
				{
                    string mp3Path = wavPath.Replace(".mp3", ".wav");
                    ConvertMp3ToWav(wavPath, mp3Path);
                    wavPath = wavPath.Replace(".mp3", ".wav");
				}

                //Waved
                File.Copy(wavPath, currentWav, true);
                string ADXName = wavPath.Replace(".wav", ".adx");
				string arguments = "CurrentWav.wav " + "\""+ ADXName  + "\"";
                Console.WriteLine("adxencd.exe " + arguments);
				ProcessStartInfo sEncode = new ProcessStartInfo("adxencd.exe", arguments);
				var process = Process.Start(sEncode);
                process.WaitForExit();
			}
            if (File.Exists(currentWav)) { File.Delete(currentWav); } //Cleanup
        }

        private void STAGECLICK(object sender, MouseEventArgs e)
        {
            PictureBox newPB = (PictureBox)sender;
            
            if (targetPB != null && newPB.Name == targetPB.Name) return;
            chRMActive.Checked = false; RMTimer.Enabled = false;
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
            try
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

                File.Copy(SourceCSB.Text, FullFilename, true);
                MessageBox.Show("Install Done!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Install Failed!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveCSB2 ()
        {
			    if (WorkingStageName == null || WorkingStageName == "") return;
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
                string csb = GetRandomCSB();
                if (csb == "") { Console.WriteLine("No CSB"); return; }
                File.Copy(csb, FullFilename, true);
                Console.WriteLine("Random CSB Swapped");
                Console.WriteLine("Timer interval " + RMTimer.Interval);
        }

        private void DeleteCustomCSB ()
        {
            try
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
                if (File.Exists(FullFilename))
                {
                    File.Delete(FullFilename);
                    MessageBox.Show("Uninstall Done! Stage Music should be back to default!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No Custom music found! Should be default!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {
                MessageBox.Show("Uninstall Failed!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InstallFile_Click(object sender, EventArgs e)
        {
            SaveSettings();
           if (NothingSelected) { MessageBox.Show("Select a target Stage!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            if (InstallLocation.Text == "") {MessageBox.Show("Game Install Folder is empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
           if (SourceCSB.Text == "") {MessageBox.Show("No source CSB!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
           SetDirectory();
            
            SaveCSB();
        }

        private void PickGameDir_Click(object sender, EventArgs e)
        {
			//folderPicker.RootFolder = Environment.SpecialFolder.Desktop;
			folderPicker.ShowNewFolderButton = false;
            if (InstallLocation.Text.Trim() != string.Empty)
			{
                folderPicker.SelectedPath = InstallLocation.Text;
				SendKeys.Send("{TAB}{TAB}{RIGHT}");
			}
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
            try
            {
                File.Copy(adxFileName1.Text, STG_EVENT_Normal + "\\Intro.adx", true);
                File.Copy(adxFileName2.Text, STG_EVENT_Normal + "\\Loop.adx", true);
                File.Copy(adxFileName3.Text, STG_EVENT_Ultra + "\\Intro.adx", true);
                File.Copy(adxFileName4.Text, STG_EVENT_Ultra + "\\Loop.adx", true);
                File.Copy(adxFileName5.Text, STG_EVENT_LowHP + "\\Intro.adx", true);
                File.Copy(adxFileName6.Text, STG_EVENT_LowHP + "\\Loop.adx", true);

                ////Write a target CSB
                //string emptyCSB = exePath + "\"" + "temp.csb";
                //File.WriteAllBytes

                //Run CSBEditor
                //File.Copy(@"Resources\BGM_00.csb",  WorkPath + "\\NewCreatedCSB.csb", true);
                string arguments = string.Empty;
                arguments = "\"" + TEMPPath + "\"";
                ProcessStartInfo sEncode = new ProcessStartInfo("CSBEditor.exe", arguments);
                var process = Process.Start(sEncode);
                process.WaitForExit();

                //Show where to save
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.FileName = "FreshNewCSB.csb";
                saveFileDialog1.Filter = CSBFileFilter;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string filename = saveFileDialog1.FileName;
                    File.Copy(WorkingCSBName, filename, true);
                }
            }
            catch
            {
                MessageBox.Show("Cannot Create CSB! Please fill all input ADX!");
            }
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

		private void UninstallMusicFile_Click(object sender, EventArgs e)
		{
            DeleteCustomCSB();
		}

		private void chRMActive_CheckedChanged(object sender, EventArgs e)
		{
            if (chRMActive.Checked)
            {
                RMTimer.Enabled = true;
                RMTimer.Interval = (int)numRMTime.Value * 60000;
            }
            else
            {
                RMTimer.Enabled = false;
            }
		}

        private void RMTimer_Tick(object sender, EventArgs e)
        {
            RandomizeMusic();
        }

        private void RandomizeMusic()
        {
            SaveCSB2();
        }

        public string GetRandomCSB()
        {
            string RandomCSB = "";
            List<string> CSBFiles = new List<string>();
            string[] Files = Directory.GetFiles(rmSource.Text);

            foreach (string file in Files)
            {
                string ext = Path.GetExtension(file);
                if (ext.ToLower().Equals(".csb"))
                {
                    CSBFiles.Add(file);
                }
            }

            if (CSBFiles.Count > 0)
            {
                var Rand = new Random();
                int RandomIndex = Rand.Next(0, CSBFiles.Count);
                RandomCSB = CSBFiles[RandomIndex];
            }
            return RandomCSB;
        }

		private void btnSetRMFolderSource_Click(object sender, EventArgs e)
		{
            folderPicker.ShowNewFolderButton = false;
            if (CSBsSourceFolder.Trim() != string.Empty)
			{
                folderPicker.SelectedPath = CSBsSourceFolder;
                SendKeys.Send("{TAB}{TAB}{RIGHT}");
			}
            folderPicker.ShowDialog();
            if (folderPicker.SelectedPath != "")
            {
                CSBsSourceFolder = folderPicker.SelectedPath;
                rmSource.Text = CSBsSourceFolder;
                SaveSettings();
            }
		}

		private void numRMTime_ValueChanged(object sender, EventArgs e)
		{
            RMTimer.Interval = (int)numRMTime.Value * 60000;
            if (RMTimer.Enabled)
            {
                RMTimer.Enabled = false;
                RMTimer.Enabled = true;
            }
		}

		private void btnLoadADX_Click(object sender, EventArgs e)
		{
            LoadWAVFiles();
		}
	}
}