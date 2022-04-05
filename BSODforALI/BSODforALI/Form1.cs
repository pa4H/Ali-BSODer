using BSODforALI.Properties;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace BSODforALI
{
    public partial class Form1 : Form
    {
        [DllImport("ntdll.dll")]
        private static extern int NtSetInformationProcess(IntPtr process, int process_class, ref int process_value, int length);
        public Form1()
        {
            InitializeComponent();
        }

        private string hw;
        private string[] text = new string[8];
        private bool autorun = false;
        private int timerinterval = 0;
        private ulong mem;
        private bool debug = false;
        private int boom = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Get("Win32_Processor", "Name");
            this.textBox1.Text = this.hw;
            this.Get("Win32_VideoController", "Name");
            this.textBox2.Text = this.hw;
            ComputerInfo computerInfo = new ComputerInfo();
            this.mem = ulong.Parse(computerInfo.TotalPhysicalMemory.ToString());
            this.numericUpDown2.Value = Convert.ToDecimal(this.mem / 1048576uL);
            this.notifyIcon1.Visible = true;
            if (File.Exists("C:\\1.txt"))
            {
                base.WindowState = FormWindowState.Minimized;
                base.ShowInTaskbar = false;
                using (StreamReader streamReader = new StreamReader("C:\\1.txt"))
                {
                    for (int i = 0; i < 8; i++)
                    {
                        this.text[i] = streamReader.ReadLine();
                    }
                }

                //С какого раза срабатывать
                if (Convert.ToInt32(text[4]) > 1)
                {
                    int buf = Convert.ToInt32(text[4]) - 1;
                    text[4] = buf.ToString();
                    string fileName = "C:\\1.txt";
                    FileInfo fileInfo = new FileInfo(fileName);
                    using (StreamWriter streamWriter = fileInfo.CreateText())
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            streamWriter.WriteLine(text[i]);
                        }
                    }
                    Environment.Exit(1);
                }

                Random x = new Random();
                if (text[0] != text[1]) { timerinterval = x.Next(Convert.ToInt32(text[0]), Convert.ToInt32(text[1]) + 1); }
                else { this.timerinterval = Convert.ToInt32(this.text[0]); }
                this.notifyIcon1.Text = "BSOD через " + Convert.ToString(this.timerinterval - this.boom) + " секунд(ы)";
                this.button1.Text = "Убрать из автозагрузки";
                this.autorun = true;
                if (this.text[5] == this.textBox1.Text && this.text[6] == Convert.ToString(this.numericUpDown2.Value) && this.text[7] == this.textBox2.Text && Convert.ToInt32(this.text[3]) > 1)
                {
                    this.notifyIcon1.Text = "BSOD активирован";
                }

                if (this.text[3] == "1")//
                {
                    this.timer1.Enabled = true;
                }
                else if (this.text[3] == "2")//
                {
                    if (this.text[5] != this.textBox1.Text)//
                    {
                        if (this.text[2] == "1")//
                        {
                            this.timer1.Enabled = true;
                        }
                        else
                        {
                            this.killkillkill();
                        }
                    }
                }
                else if (this.text[3] == "3")//
                {
                    if (this.text[6] != Convert.ToString(this.numericUpDown2.Value))//
                    {
                        if (this.text[2] == "1")//
                        {
                            this.timer1.Enabled = true;
                        }
                        else
                        {
                            this.killkillkill();
                        }
                    }
                }
                else if (this.text[3] == "4")//
                {
                    if (this.text[7] != this.textBox2.Text)//
                    {
                        if (this.text[2] == "1")//
                        {
                            this.timer1.Enabled = true;
                        }
                        else
                        {
                            this.killkillkill();
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Время от
            //Время до
            //Галочка "таймер для условия"
            //Режим
            //С какого раза срабатывать
            //Проц
            //ОЗУ
            //Видео
            if (!this.autorun)
            {
                string fileName = "C:\\1.txt";
                FileInfo fileInfo = new FileInfo(fileName);
                using (StreamWriter streamWriter = fileInfo.CreateText())
                {
                    streamWriter.WriteLine(this.numericUpDown1.Value);
                    streamWriter.WriteLine(this.numericUpDown3.Value);
                    if (this.checkBox1.Checked)
                    {
                        streamWriter.WriteLine("1");
                    }
                    else
                    {
                        streamWriter.WriteLine("0");
                    }
                    if (this.radioButton1.Checked)
                    {
                        streamWriter.WriteLine("1");
                    }
                    else if (this.radioButton2.Checked)
                    {
                        streamWriter.WriteLine("2");
                    }
                    else if (this.radioButton3.Checked)
                    {
                        streamWriter.WriteLine("3");
                    }
                    else if (this.radioButton4.Checked)
                    {
                        streamWriter.WriteLine("4");
                    }
                    streamWriter.WriteLine(numericUpDown5.Value);
                    streamWriter.WriteLine(this.textBox1.Text);
                    streamWriter.WriteLine(this.numericUpDown2.Value);
                    streamWriter.WriteLine(this.textBox2.Text);
                    RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    registryKey.SetValue("REF", Application.ExecutablePath.ToString());
                }
                this.button1.BackColor = Color.LightGreen;
            }
            else
            {
                File.Delete("C:\\1.txt");
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                registryKey.DeleteValue("REF", false);
                this.notifyIcon1.Visible = false;
                Environment.Exit(1);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.notifyIcon1.Text = "BSOD через " + Convert.ToString(this.timerinterval - this.boom) + " секунд";
            this.boom++;
            if (this.boom > this.timerinterval)
            {
                this.timer1.Stop();
                this.killkillkill();
            }
        }
        private void Get(string hwclass, string syntax)
        {
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM " + hwclass);
            using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher.Get().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ManagementObject managementObject = (ManagementObject)enumerator.Current;
                    this.hw = Convert.ToString(managementObject[syntax]);
                }
            }
        }
        private void killkillkill()
        {
            if (!this.debug)
            {
                //new Process
                //{
                //    StartInfo =
                //    {
                //        FileName = "C:\\bsod.exe",
                //        Arguments = "0x10 0x1111 0x2222 0x3333 0x4444",
                //        WindowStyle = ProcessWindowStyle.Hidden
                //    }
                //}.Start();

                Process.EnterDebugMode();
                int status = 1;
                NtSetInformationProcess(Process.GetCurrentProcess().Handle, 0x1D, ref status, sizeof(int));
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                MessageBox.Show("BSOD");
            }
        }
        #region Интерфейс
        private void button2_Click(object sender, EventArgs e)
        {
            new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    Arguments = "/c shutdown -r -t 00"
                }
            }.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    Arguments = "/c shutdown -s -t 00"
                }
            }.Start();
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown3.Value < numericUpDown1.Value)
            {
                numericUpDown3.Value = numericUpDown1.Value;
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            numericUpDown3.Value = numericUpDown1.Value;
        }
        private void Form1_Deactivate(object sender, EventArgs e)
        {
            if (base.WindowState == FormWindowState.Minimized)
            {
                if (this.boom > 0)
                {
                    base.ShowInTaskbar = false;
                    this.timer1.Start();
                }
            }
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            

        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://vk.com/pa4h_official");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.killkillkill();
        }
        #endregion

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1.Enabled = false;
            numericUpDown1.Enabled = true;
            numericUpDown3.Enabled = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            checkboxChange();
            checkBox1.Enabled = true;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            checkboxChange();
            checkBox1.Enabled = true;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            checkboxChange();
            checkBox1.Enabled = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            checkboxChange();
        }
        void checkboxChange()
        {
            if (checkBox1.Checked)
            {
                numericUpDown1.Enabled = true;
                numericUpDown3.Enabled = true;
            }
            else
            {
                numericUpDown1.Enabled = false;
                numericUpDown3.Enabled = false;
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.boom > 0)
            {
                base.WindowState = FormWindowState.Normal;
                base.ShowInTaskbar = true;
                this.Text = "Таймер остановлен, сверните программу для возобновления";
                this.notifyIcon1.Text = "Пауза на " + Convert.ToString(this.timerinterval - this.boom) + " секунде";
                this.timer1.Stop();
            }
            if (notifyIcon1.Text == "BSOD активирован")
            {
                base.WindowState = FormWindowState.Normal;
                base.ShowInTaskbar = true;
            }
        }

    }
}
