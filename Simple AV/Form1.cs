using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace Simple_AV
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string GetMD5FromFile(string filenPath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filenPath))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty).ToLower();
                }
            }
        }

        private void browseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "select a File, you think could be a virus";
            dlg.Filter = "ALL FILES | *.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtFile.Text = dlg.FileName;
                txtMD5.Text = GetMD5FromFile(dlg.FileName);
            }
            lblStatus.Text = "Status: N/A";
            lblScannedDocs.Text = "Scanned Docs:";
            lblScanning.Text = "Scanning:";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            Thread th = new Thread(() => {

            int num = 0;

            if (txtFile.Text != "")
            {
                again:
                    lblScannedDocs.Invoke(new Action(() => {
                        lblScannedDocs.Text = $"Scanned Docs: {num}";
                    }));

                int rest = 5 - num.ToString().Length;

                WebClient wc = new WebClient();

                switch (rest)
                {
                    case 4:
                        String md5Sig4 = wc.DownloadString("https://virusshare.com/hashfiles/VirusShare_" + "0000" + num.ToString() + ".md5");
                          
                        if (md5Sig4.Contains(txtMD5.Text))
                        {
                                lblStatus.Invoke(new Action(() => {
                                    lblStatus.Text = "Infected";
                                    lblStatus.ForeColor = Color.Red;
                                }));
                        }
                        else
                        {
                            num++;
                                lblScanning.Invoke(new Action(() => { lblScanning.Text = "Scanning: https://virusshare.com/hashfiles/VirusShare_" + "0000" + num.ToString() + ".md5"; }));
                                goto again;
                        }
                        break;

                    case 3:
                            String md5Sig3 = wc.DownloadString("https://virusshare.com/hashfiles/VirusShare_" + "000" + num.ToString() + ".md5");
                        
                            if (md5Sig3.Contains(txtMD5.Text))
                            {
                                lblStatus.Invoke(new Action(() => {
                                    lblStatus.Text = "Infected";
                                    lblStatus.ForeColor = Color.Red;
                                }));
                            }
                            else
                            {
                                num++;
                                lblScanning.Invoke(new Action(() => { lblScanning.Text = "Scanning: https://virusshare.com/hashfiles/VirusShare_" + "000" + num.ToString() + ".md5"; }));
                                goto again;
                            } 
                            break;

                    case 2:
                            String md5Sig2 = wc.DownloadString("https://virusshare.com/hashfiles/VirusShare_" + "00" + num.ToString() + ".md5");
                            
                            if (md5Sig2.Contains(txtMD5.Text))
                            {
                                lblStatus.Invoke(new Action(() => {
                                    lblStatus.Text = "Infected";
                                    lblStatus.ForeColor = Color.Red;
                                }));
                            }
                            else
                            {
                                num++;
                                lblScanning.Invoke(new Action(() => { lblScanning.Text = "Scanning: https://virusshare.com/hashfiles/VirusShare_" + "00" + num.ToString() + ".md5"; }));
                                if (num == 389)
                                {
                                    lblStatus.Invoke(new Action(() => {
                                        lblStatus.Text = "Clean";
                                        lblStatus.ForeColor = Color.Green;
                                    }));
                                }
                                else
                                {
                                    goto again;
                                }
                            }
                            break;                  
                }               
            }
            else
            {
                MessageBox.Show("Please select a File!","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            });
            th.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void toggleAdvancedModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool advanced = false;
            if (advanced)
            {
                advanced = false;
                lblScannedDocs.Visible = false;
                lblScanning.Visible = false;
            }
            else if (!advanced)
            {
                advanced = true;
                lblScannedDocs.Visible = true;
                lblScanning.Visible = true;
            }
        }

        bool advanced = false;
        private void toggleAdvancedModeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (advanced)
            {
                advanced = false;
                lblScannedDocs.Visible = false;
                lblScanning.Visible = false;
            }
            else if (!advanced)
            {
                advanced = true;
                lblScannedDocs.Visible = true;
                lblScanning.Visible = true;
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Status: N/A";
            lblStatus.ForeColor = Color.Black;

            lblScannedDocs.Text = "Scanned Docs:";
            lblScanning.Text = "Scanning:";

            txtFile.Text = "";
            txtMD5.Text = "";
        }
    }
}
