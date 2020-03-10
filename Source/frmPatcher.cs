﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using static upatcher;
using System.Text.RegularExpressions;

using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace UniversalPatcher
{
    public partial class FrmPatcher : Form
    {
        public FrmPatcher()
        {
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            TextBoxTraceListener tbtl = new TextBoxTraceListener(txtDebug);
            Debug.Listeners.Add(tbtl);
            //Debug.WriteLine("Testing Testing 123");
        }

        private struct DetectGroup
        {
            public string Logic;
            public uint Hits;
            public uint Miss;
        }

        private frmSegmenList frmSL;
        private static List<uint> PatchData;
        private static List<uint> PatchAddr;
        private byte[] Basebuf;
        private byte[] Modbuf;
        private BinFile[] basefile;
        private BinFile[] modfile;
        private CheckBox[] chkSegments;
        private string LastXML = "";

        private void FrmPatcher_Load(object sender, EventArgs e)
        {
            this.Show();
            labelXML.Text = Path.GetFileName(XMLFile);
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && File.Exists(args[1]))
            {
                Logger(args[1]);
                frmSegmenList frmSL = new frmSegmenList();
                frmSL.LoadFile(args[1]);
            }
            addCheckBoxes();
            numSuppress.Value = Properties.Settings.Default.SuppressAfter;
            if (numSuppress.Value == 0)
                numSuppress.Value = 10;

            if (!File.Exists(Path.Combine(Application.StartupPath, "Patches")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Patches"));
            if (!File.Exists(Path.Combine(Application.StartupPath, "XML")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "XML"));

            Properties.Settings.Default.LastBINfolder = Properties.Settings.Default.LastBINfolder;
            Properties.Settings.Default.LastPATCHfolder = Properties.Settings.Default.LastPATCHfolder;
            Properties.Settings.Default.LastXMLfolder = Properties.Settings.Default.LastXMLfolder;

            if (Properties.Settings.Default.LastXMLfolder == "")
                Properties.Settings.Default.LastXMLfolder = Path.Combine(Application.StartupPath, "XML");
            if (Properties.Settings.Default.LastPATCHfolder == "")
                Properties.Settings.Default.LastPATCHfolder = Path.Combine(Application.StartupPath, "Patches");

            DetectRules = new List<DetectRule>();
            string AutoDetectFile = Path.Combine(Application.StartupPath, "XML", "autodetect.xml");
            if (File.Exists(AutoDetectFile))
            {
                Debug.WriteLine("Loading autodetect.xml");
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<DetectRule>));
                System.IO.StreamReader file = new System.IO.StreamReader(AutoDetectFile);
                DetectRules = (List<DetectRule>)reader.Deserialize(file);
                file.Close();
            }
        }

        public void addCheckBoxes()
        {
            if (LastXML == XMLFile && chkSegments != null && chkSegments.Length == Segments.Count)
                return;
            if (chkSegments != null)
            {
                for (int s = 0; s < chkSegments.Length; s++)
                {
                    chkSegments[s].Dispose();
                }
            }
            if (radioApply.Checked)
                return;
            int Left = 12;
            chkSegments = new CheckBox[Segments.Count];
            for (int s = 0; s < Segments.Count; s++)
            {
                CheckBox chk = new CheckBox();
                this.Controls.Add(chk);
                chk.Location = new Point(Left, 91);
                chk.Text = Segments[s].Name;
                chk.AutoSize = true;
                Left += chk.Width + 5;
                chk.Tag = s;
                chk.Checked = true;
                chkSegments[s] = chk;
            }
            LastXML = XMLFile;

        }

        private void CheckSegmentCompatibility()
        {
            if (!radioCreate.Checked || txtBaseFile.Text == "" || txtModifierFile.Text == "")
                return;

            labelXML.Text = Path.GetFileName(XMLFile);
            for (int s = 0; s < Segments.Count; s++)
            {
                CheckBox chk = null;
                for (int i = 0; i < this.Controls.Count; i++)
                {
                    if (this.Controls[i].Tag != null && (int)this.Controls[i].Tag == s)
                    {
                        chk = this.Controls[i] as CheckBox;
                    }
                }
                string BasePN = ReadInfo(Basebuf, basefile[s].PNaddr);
                string ModPN = ReadInfo(Modbuf, modfile[s].PNaddr);
                string BaseVer = ReadInfo(Basebuf, basefile[s].VerAddr);
                string ModVer = ReadInfo(Modbuf, modfile[s].VerAddr);

                if (BasePN != ModPN || BaseVer != ModVer)
                {
                    Logger(Segments[s].Name.PadLeft(11) + " differ: " + BasePN.ToString().PadRight(8) + " " + BaseVer + " <> " + ModPN.ToString().PadRight(8) + " " + ModVer);
                    chk.Enabled = false;
                }
                else
                {
                    chk.Enabled = true;
                }
            }
        }

        private void GetFileInfo(string FileName, ref BinFile[] binfile, ref byte[] buf)
        {
            try
            {
                uint fsize = (uint)new FileInfo(FileName).Length;
                buf = ReadBin(FileName, 0, fsize);
                if (chkAutodetect.Checked)
                {
                    string ConfFile = Autodetect(FileName, buf);
                    Logger("Autodetect: " + ConfFile);
                    if (ConfFile == "" || ConfFile.Contains(Environment.NewLine))
                    {
                        labelXML.Text = "";
                        XMLFile = "";
                        Segments.Clear();
                    }
                    else
                    { 
                        ConfFile = Path.Combine(Application.StartupPath,"XML", ConfFile);
                        if (File.Exists(ConfFile)) { 
                            frmSegmenList frmSL = new frmSegmenList();
                            frmSL.LoadFile(ConfFile);
                        }
                        else
                        {
                            Logger("XML File not found");
                            labelXML.Text = "";
                            XMLFile = "";
                            Segments.Clear();
                        }
                    }
                }
                labelXML.Text = Path.GetFileName(XMLFile);
                addCheckBoxes();
                Logger(Environment.NewLine + Path.GetFileName(FileName));
                txtFileinfo.AppendText(Environment.NewLine + Path.GetFileName(FileName) +" (" + labelXML.Text + ")" + Environment.NewLine);
                GetSegmentAddresses(buf, out binfile);
                if (Segments.Count > 0)
                    Logger("Segments:");
                for (int i = 0; i < Segments.Count; i++)
                {
                    SegmentConfig S = Segments[i];
                    if (S.Eeprom)
                    {
                        Logger(S.Name + " [4000 - 8000]");
                        Logger(GmEeprom.GetEEpromInfo(buf));
                    }
                    else
                    {
                        string tmp = "";
                        for (int s = 0; s < binfile[i].SegmentBlocks.Count; s++)
                        {
                            if (s > 0)
                                tmp += ", ";
                            tmp = binfile[i].SegmentBlocks[s].Start.ToString("X4") + " - " + binfile[i].SegmentBlocks[s].End.ToString("X4");
                        }
                        Logger(S.Name.PadRight(11) + (" [" + tmp + "]").PadRight(15), false);
                        string PN = ReadInfo(buf, binfile[i].PNaddr);
                        if (PN.Length > 1)
                            Logger(", PN: " + PN.PadRight(9), false);

                        string Ver = ReadInfo(buf, binfile[i].VerAddr);
                        if (Ver.Length > 1)
                            Logger(", Ver: " + Ver, false);

                        txtFileinfo.AppendText(S.Name.PadRight(11) + " PN: " + PN + " Ver: " + Ver);

                        string SNr = ReadInfo(buf, binfile[i].SegNrAddr);
                        if (SNr.Length > 0)
                            Logger(", Nr: " + SNr, false);
                        if (binfile[i].ExtraInfo != null && binfile[i].ExtraInfo.Count > 0)
                        {
                            string ExtraI = "";
                            for (int e = 0; e < binfile[i].ExtraInfo.Count; e++)
                            {
                                ExtraI += ", " + binfile[i].ExtraInfo[e].Name + ": " + ReadInfo(buf, binfile[i].ExtraInfo[e]);
                            }
                            Logger(ExtraI);
                        }
                        else
                            Logger("");
                    }
                    txtFileinfo.AppendText(Environment.NewLine);
                }
                Logger("Checksums:");
                for (int i = 0; i < Segments.Count; i++)
                {
                    SegmentConfig S = Segments[i];
                    Logger(S.Name.PadRight(11), false);
                    if (S.Eeprom)
                    {
                        Logger(GmEeprom.GetKeyStatus(buf));

                    }
                    else
                    {

                        if (S.CS1Method != CSMethod_None)
                        {
                            string CS1Calc = CalculateChecksum(buf, binfile[i].CS1Address, binfile[i].CS1Blocks, binfile[i].ExcludeBlocks, S.CS1Method, S.CS1Complement, binfile[i].CS1Address.Bytes, S.CS1SwapBytes).ToString("X4");
                            if (binfile[i].CS1Address.Bytes == 0)
                                Logger(" Checksum1: " + CS1Calc,false);
                            else
                            {
                                string CS1 = ReadInfo(buf, binfile[i].CS1Address);
                                if (CS1 == CS1Calc)
                                    Logger(" Checksum 1: " + CS1 + " [OK]", false);
                                else
                                { 
                                    Logger(" Checksum 1: " + CS1 + ", Calculated: " + CS1Calc + " [Fail]", false);
                                    txtFileinfo.AppendText(S.Name + " Checksum 1 Fail");
                                }
                            }
                        }

                        if (S.CS2Method != CSMethod_None)
                        {
                            string CS2Calc = CalculateChecksum(buf, binfile[i].CS2Address, binfile[i].CS2Blocks, binfile[i].ExcludeBlocks, S.CS2Method, S.CS2Complement, binfile[i].CS2Address.Bytes, S.CS2SwapBytes).ToString("X4");
                            if (binfile[i].CS2Address.Bytes == 0)
                                Logger(" Checksum1: " + CS2Calc, false);
                            else
                            {
                                string CS2 = ReadInfo(buf, binfile[i].CS2Address);
                                if (CS2 == CS2Calc)
                                    Logger(" Checksum 2: "  + CS2 + " [OK]", false);
                                else
                                { 
                                    Logger(" Checksum 2:" + CS2 + ", Calculated: " + CS2Calc + " [Fail]", false);
                                    txtFileinfo.AppendText(" " + S.Name + " Checksum 2 Fail");
                                }
                            }
                        }

                    }
                    if (!txtResult.Text.EndsWith(Environment.NewLine))
                        txtResult.AppendText(Environment.NewLine);
                    if (!txtFileinfo.Text.EndsWith(Environment.NewLine))
                        txtFileinfo.AppendText(Environment.NewLine);
                }
                addCheckBoxes();
                CheckSegmentCompatibility();
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }

        }
        private void btnOrgFile_Click(object sender, EventArgs e)
        {
            string Filename = SelectFile();
            if (Filename.Length > 1)
            {
                txtBaseFile.Text = Filename;
                uint fsize = (uint)new System.IO.FileInfo(txtBaseFile.Text).Length;
                labelBinSize.Text = fsize.ToString();
                basefile = new BinFile[Segments.Count];
                Basebuf = new byte[fsize];
                GetFileInfo(txtBaseFile.Text, ref basefile, ref Basebuf);
            }
        }

        private void btnModFile_Click(object sender, EventArgs e)
        {
            if (radioCreate.Checked)
            {
                string Filename = SelectFile();
                if (Filename.Length > 1)
                {
                    txtModifierFile.Text = Filename;
                    uint fsize = (uint)new System.IO.FileInfo(txtModifierFile.Text).Length;
                    modfile = new BinFile[Segments.Count];
                    Modbuf = new byte[fsize];
                    GetFileInfo(txtModifierFile.Text, ref modfile, ref Modbuf);
                }
            }
            else
            {
                try
                {
                    string Filename = SelectFile("PATCH files (*.patch)|*.patch|ALL files(*.*) | *.*");
                    if (Filename.Length > 1)
                    {
                        txtModifierFile.Text = Filename;
                        string line;
                        StreamReader sr = new StreamReader(txtModifierFile.Text);
                        line = sr.ReadLine();
                        Logger("Patch is for bin size: " + line);
                        line = sr.ReadLine();
                        Logger(line);
                        line = sr.ReadLine();
                        if (line.Contains(".xml"))
                        {
                            string tmpXML = "";
                            if (File.Exists(line))
                            {
                                tmpXML = line;
                            }
                            else
                            {
                                if (File.Exists(Path.Combine(Application.StartupPath, "XML", Path.GetFileName(line))))
                                    tmpXML = Path.Combine(Application.StartupPath, "XML", Path.GetFileName(line));
                            }

                            if (tmpXML != "")
                            {
                                if (frmSL == null)
                                    frmSL = new frmSegmenList();
                                frmSL.LoadFile(tmpXML);
                                labelXML.Text = Path.GetFileName(tmpXML);
                                if (txtBaseFile.Text.Length > 1)
                                    addCheckBoxes();
                            }
                        }
                        sr.Close();
                        btnShowPatch.Enabled = true;
                    }
                }
                catch (Exception ex)
                {
                    Logger("Error: " + ex.Message);
                }
            }

        }

        private bool ApplyPatch()
        {
            string line;

            try
            {
                StreamReader sr = new StreamReader(txtModifierFile.Text);
                uint LineNr = 0;
                uint fsize = (uint)new System.IO.FileInfo(txtBaseFile.Text).Length;
                Basebuf = new byte[fsize];
                basefile = new BinFile[Segments.Count];
                labelBinSize.Text = fsize.ToString();
                GetFileInfo(txtBaseFile.Text, ref basefile, ref Basebuf);
                while ((line = sr.ReadLine()) != null)
                {
                    if (LineNr == 0)
                    {
                        if (line != fsize.ToString())
                        {
                            Logger("File size doesn't match patch");
                            return false;
                        }
                    }
                    LineNr++;

                    string tmp = Regex.Replace(line, "[^0-9:]", "");
                    if (tmp != line)
                        Logger("(" + line + ")");
                    else
                    {
                        string[] LineParts = line.Split(':');
                        if (LineParts.Length > 1)
                        {
                            uint Addr = uint.Parse(LineParts[0]);
                            uint Data = uint.Parse(LineParts[1]);
                            if (Addr > Basebuf.Length)
                                throw new FileLoadException(String.Format("Address {0} out of range!", Addr.ToString("X4")));
                            if (Data > 0xff)
                                throw new FileLoadException(String.Format("Data {0} out of range!", Data.ToString("X4")));
                            //Apply patchrow:
                            if (LineNr <= 30)
                                Logger("Set address: ".PadRight(16) + Addr.ToString("X4").PadRight(10) + "Data:   " + Data.ToString("X4"));
                            Basebuf[Addr] = byte.Parse(LineParts[1]);
                        }
                    }
                }
                sr.Close();
                if (LineNr >= 30)
                {
                    Logger("(Suppressing output)");
                    Logger("Total: " + LineNr.ToString() + " modifications");
                }

            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
                return false;
            }
            Logger("[OK]");
            return true;
        }

        private void CompareBlock(byte[] OrgFile, byte[] ModFile, uint Start, uint End)
        {
            Logger(" [" + Start.ToString("X") + " - " + End.ToString("X") + "] ");
            uint ModCount = 0;
            for (uint i = Start; i < End; i++)
            {
                if (OrgFile[i] != ModFile[i])
                {
                    ModCount++;
                    PatchAddr.Add(i);
                    PatchData.Add(ModFile[i]);
                    if (ModCount < numSuppress.Value)
                        Logger(i.ToString("X6") + ": " + OrgFile[i].ToString("X2") + " => " + ModFile[i].ToString("X2"));
                }

            }
            if (ModCount >= numSuppress.Value)
            {
                Logger("(Suppressing output)");
                Logger("Total: " + ModCount.ToString() + " differences");
            }

        }
        public void CompareBins()
        {
            try
            {
                uint fsize = (uint)new System.IO.FileInfo(txtBaseFile.Text).Length;
                uint fsize2 = (uint)new System.IO.FileInfo(txtModifierFile.Text).Length;
                if (fsize != fsize2)
                {
                    Logger("Files are different size, will not compare!");
                    return;
                }
                basefile = new BinFile[Segments.Count];
                modfile = new BinFile[Segments.Count];
                Basebuf = new byte[fsize];
                Modbuf = new byte[fsize2];
                GetFileInfo(txtBaseFile.Text, ref basefile, ref Basebuf);
                GetFileInfo(txtModifierFile.Text, ref modfile, ref Modbuf);

                labelBinSize.Text = fsize.ToString();

                if (Segments.Count == 0)
                {
                    Logger("No segments defined, comparing complete file");
                    CompareBlock(Basebuf, Modbuf, 0, (uint)fsize);
                }
                else if (chkCompareAll.Checked)
                {
                    Logger("Comparing complete file");
                    CompareBlock(Basebuf, Modbuf, 0, (uint)fsize);
                }
                else
                {
                    for (int Snr = 0; Snr < Segments.Count; Snr++)
                    {
                        if (chkSegments[Snr].Enabled && chkSegments[Snr].Checked)
                        {
                            Logger("Comparing segment " + Segments[Snr].Name, false);
                            for (int p = 0; p < basefile[Snr].SegmentBlocks.Count; p++)
                            {
                                uint Start = basefile[Snr].SegmentBlocks[p].Start;
                                uint End = basefile[Snr].SegmentBlocks[p].End;
                                CompareBlock(Basebuf, Modbuf, Start, End);
                            }
                            Logger("");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }
        }
        private void btnCompare_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.SuppressAfter = (uint)numSuppress.Value;
            Properties.Settings.Default.Save();
            if (txtBaseFile.Text.Length == 0 || txtModifierFile.Text.Length == 0)
                return;
            labelXML.Text = Path.GetFileName(XMLFile);
            PatchData = new List<uint>();
            PatchAddr = new List<uint>();
            txtResult.Text = "";
            if (radioCreate.Checked)
            {
                CompareBins();
                if (PatchAddr.Count > 0)
                {
                    btnShowPatch.Enabled = true;
                    btnSave.Enabled = true;
                    btnSave.Text = "Save patch";
                    txtPatchName.Enabled = true;
                }
                else
                {
                    btnShowPatch.Enabled = false;
                    btnSave.Enabled = false;
                    txtPatchName.Enabled = false;
                }
            }
            else //Apply patch selected
            {
                btnShowPatch.Enabled = false;
                if (ApplyPatch())
                {
                    btnSave.Enabled = true;
                    btnSave.Text = "Save Bin";
                    txtPatchName.Enabled = true;
                }
                else
                {
                    btnSave.Enabled = false;
                    txtPatchName.Enabled = false;
                }
            }

        }

        private void SavePatch()
        {
            try
            {
                if (txtPatchName.Text.Length < 1)
                {
                    Logger("Supply patch description");
                    return;
                }
                string PatchName = labelBinSize.Text + "-" + txtPatchName.Text;
                string PatchFile = SelectSaveFile("PATCH files (*.patch)|*.patch|ALL files(*.*) | *.*");
                if (PatchFile.Length < 1)
                    return;
                Logger("Saving to file: " + PatchFile);
                StreamWriter sw = new StreamWriter(PatchFile);
                sw.WriteLine(labelBinSize.Text);
                sw.WriteLine(txtPatchName.Text);
                sw.WriteLine(XMLFile);
                for (int i = 0; i < PatchAddr.Count; i++)
                {
                    sw.WriteLine(PatchAddr[i].ToString() + ":" + PatchData[i].ToString());
                }
                sw.Close();
                Logger("Patch saved");
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }

        }

        private void SaveBin()
        {
            try
            {
                string FileName = SelectSaveFile();
                if (FileName.Length == 0)
                    return;
                Logger("Saving to file: " + FileName);
                WriteSegmentToFile(FileName, 0, (uint)Basebuf.Length, Basebuf);
                Logger("Done.");
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (radioCreate.Checked)
                SavePatch();
            else
                SaveBin();
        }

        public void Logger(string LogText, Boolean NewLine = true)
        {
            txtResult.AppendText(LogText);
            if (NewLine)
                txtResult.AppendText(Environment.NewLine);
            Application.DoEvents();
        }
        private void txtModifierFile_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioCreate_CheckedChanged(object sender, EventArgs e)
        {
            txtModifierFile.Text = "";
            btnSave.Enabled = false;
            btnShowPatch.Enabled = false;
            if (radioCreate.Checked)
            {
                btnOrgFile.Text = "Original file";
                btnModFile.Text = "Modified file";
                btnCompare.Text = "Compare";
                btnSave.Text = "Save Patch";
                txtPatchName.Visible = true;
                labelDescr.Visible = true;
                chkCompareAll.Visible = true;
                this.Text = "Create patch";
                addCheckBoxes();
                CheckSegmentCompatibility();
            }
            else
            {
                btnOrgFile.Text = "BIN file";
                btnModFile.Text = "Patch File";
                btnCompare.Text = "Apply Patch";
                btnSave.Text = "Save BIN";
                txtPatchName.Visible = false;
                labelDescr.Visible = false;
                chkCompareAll.Visible = false;
                this.Text = "Apply patch";
                if (chkSegments != null)
                {
                    for (int s = 0; s < chkSegments.Length; s++)
                    {
                        chkSegments[s].Dispose();
                    }
                }
                chkSegments = null;
            }
        }


        private void btnSegments_Click(object sender, EventArgs e)
        {
            if (frmSL != null && frmSL.Visible)
            {
                frmSL.BringToFront();
                return;
            }
            frmSL = new frmSegmenList();
            frmSL.InitMe();
            if (frmSL.ShowDialog() == DialogResult.OK)
            {
                //addCheckBoxes();
            }
        }

        private void btnCheckSums_Click(object sender, EventArgs e)
        {
            if (Segments.Count > 0)
                FixCheckSums();
            if (radioApply.Checked)
                btnSave.Enabled = true;
        }
        private void FixCheckSums()
        {
            try
            {
                Logger("Segments:");
                for (int i = 0; i < Segments.Count; i++)
                {
                    SegmentConfig S = Segments[i];
                    Logger(S.Name);
                    if (S.Eeprom)
                    {
                        Logger(GmEeprom.FixEepromKey(Basebuf));
                    }
                    else
                    {
                        if (S.CS1Method != CSMethod_None)
                        {
                            uint CS1 = 0;
                            uint CS1Calc = CalculateChecksum(Basebuf, basefile[i].CS1Address, basefile[i].CS1Blocks, basefile[i].ExcludeBlocks, S.CS1Method, S.CS1Complement, basefile[i].CS1Address.Bytes, S.CS1SwapBytes);
                            if (basefile[i].CS1Address.Bytes == 1)
                            {
                                CS1 = Basebuf[basefile[i].CS1Address.Address];
                            }
                            else if (basefile[i].CS1Address.Bytes == 2)
                            {
                                CS1 = BEToUint16(Basebuf, basefile[i].CS1Address.Address);
                            }
                            else if (basefile[i].CS1Address.Bytes == 4)
                            {
                                CS1 = BEToUint32(Basebuf, basefile[i].CS1Address.Address);
                            }
                            if (CS1 == CS1Calc)
                                Logger(" Checksum 1: " + CS1.ToString("X4") + " [OK]");
                            else
                            {
                                if (basefile[i].CS1Address.Bytes == 0)
                                {
                                    Logger(" Checksum 1: " + CS1Calc.ToString("X4") + " [Not saved]");
                                }
                                else
                                {
                                    if (basefile[i].CS1Address.Bytes == 1)
                                        Basebuf[basefile[i].CS1Address.Address] = (byte)CS1Calc;
                                    else if (basefile[i].CS1Address.Bytes == 2)
                                    {
                                        Basebuf[basefile[i].CS1Address.Address] = (byte)((CS1Calc & 0xFF00) >> 8);
                                        Basebuf[basefile[i].CS1Address.Address + 1] = (byte)(CS1Calc & 0xFF);
                                    }
                                    else if (basefile[i].CS1Address.Bytes == 4)
                                    {
                                        Basebuf[basefile[i].CS1Address.Address] = (byte)((CS1Calc & 0xFF000000) >> 24);
                                        Basebuf[basefile[i].CS1Address.Address + 1] = (byte)((CS1Calc & 0xFF0000) >> 16);
                                        Basebuf[basefile[i].CS1Address.Address + 2] = (byte)((CS1Calc & 0xFF00) >> 8);
                                        Basebuf[basefile[i].CS1Address.Address + 3] = (byte)(CS1Calc & 0xFF);

                                    }
                                    Logger(" Checksum 1: " + CS1.ToString("X") + " => " + CS1Calc.ToString("X4") + " [Fixed]");
                                }
                            }
                        }

                        if (S.CS2Method != CSMethod_None)
                        {
                            uint CS2 = 0;
                            uint CS2Calc = CalculateChecksum(Basebuf, basefile[i].CS2Address, basefile[i].CS2Blocks, basefile[i].ExcludeBlocks, S.CS2Method, S.CS2Complement, basefile[i].CS2Address.Bytes, S.CS2SwapBytes);
                            if (basefile[i].CS2Address.Bytes == 1)
                            {
                                CS2 = Basebuf[basefile[i].CS2Address.Address];
                            }
                            else if (basefile[i].CS2Address.Bytes == 2)
                            {
                                CS2 = BEToUint16(Basebuf, basefile[i].CS2Address.Address);
                            }
                            else if (basefile[i].CS2Address.Bytes == 4)
                            {
                                CS2 = BEToUint32(Basebuf, basefile[i].CS2Address.Address);
                            }
                            if (CS2 == CS2Calc)
                                Logger(" Checksum 2: " + CS2.ToString("X4") + " [OK]");
                            else
                            {
                                if (basefile[i].CS2Address.Bytes == 0)
                                {
                                    Logger(" Checksum 2: " + CS2Calc.ToString("X4") + " [Not saved]");
                                }
                                else
                                {
                                    if (basefile[i].CS2Address.Bytes == 1)
                                        Basebuf[basefile[i].CS2Address.Address] = (byte)CS2Calc;
                                    else if (basefile[i].CS2Address.Bytes == 2)
                                    {
                                        Basebuf[basefile[i].CS2Address.Address] = (byte)((CS2Calc & 0xFF00) >> 8);
                                        Basebuf[basefile[i].CS2Address.Address + 1] = (byte)(CS2Calc & 0xFF);
                                    }
                                    else if (basefile[i].CS2Address.Bytes == 4)
                                    {
                                        Basebuf[basefile[i].CS2Address.Address] = (byte)((CS2Calc & 0xFF000000) >> 24);
                                        Basebuf[basefile[i].CS2Address.Address + 1] = (byte)((CS2Calc & 0xFF0000) >> 16);
                                        Basebuf[basefile[i].CS2Address.Address + 2] = (byte)((CS2Calc & 0xFF00) >> 8);
                                        Basebuf[basefile[i].CS2Address.Address + 3] = (byte)(CS2Calc & 0xFF);

                                    }
                                    Logger(" Checksum 2: " + CS2.ToString("X") + " => " + CS2Calc.ToString("X4") + " [Fixed]");
                                }
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }

        }

        private void radioApply_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnShowPatch_Click(object sender, EventArgs e)
        {
            txtResult.Clear();
            Properties.Settings.Default.SuppressAfter = (uint)numSuppress.Value;
            Properties.Settings.Default.Save();

            if (radioApply.Checked)
            {
                int i = 0;
                StreamReader sr = new StreamReader(txtModifierFile.Text);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    i++;
                    if (i <= numSuppress.Value)
                        Logger(line);
                }
                if (i >= numSuppress.Value)
                    Logger("Total: " + i.ToString() + " rows, showing first " + numSuppress.Value.ToString());

            }
            else
            {

                Logger(labelBinSize.Text);
                Logger(txtPatchName.Text);
                if (XMLFile != null)
                    Logger(XMLFile);
                int i;
                for (i = 0; i < PatchAddr.Count && i <= numSuppress.Value; i++)
                {
                    Logger(PatchAddr[i].ToString() + ":" + PatchData[i].ToString());
                }
                if (i >= numSuppress.Value)
                    Logger("Total: " + PatchAddr.Count.ToString() + " rows, showing first " + numSuppress.Value.ToString());
            }

        }

        private void numSuppress_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            string FileName = SelectFile("XML files (*.xml)|*.xml|All files (*.*)|*.*");
            if (FileName.Length < 1)
                return;
            frmSegmenList frmSL = new frmSegmenList();
            frmSL.LoadFile(FileName);
            frmSL.Dispose();
            labelXML.Text = Path.GetFileName(XMLFile);
            addCheckBoxes();
        }

        private bool CheckRule(DetectRule DR, string FileName, byte[] buf)
        {
            try { 
            
                UInt64 Data = 0;
                uint Addr = 0;
                if (DR.address == "filesize")
                {
                    Data = (UInt64)new FileInfo(FileName).Length;
                }
                else
                {
                    string[] Parts = DR.address.Split(':');
                    HexToUint(Parts[0].Replace("@", ""),out Addr);
                    if (DR.address.StartsWith("@"))
                        Addr = BEToUint32(buf, Addr);
                    if (Parts[0].EndsWith("@"))
                        Addr = (uint)buf.Length - Addr;
                    if (Parts.Length == 1)
                        Data = BEToUint16(buf, Addr);
                    else
                    {
                        if (Parts[1] == "1")
                            Data = (uint)buf[Addr];
                        if (Parts[1] == "2")
                            Data = (uint)BEToUint16(buf, Addr);
                        if (Parts[1] == "4")
                            Data = BEToUint32(buf, Addr);
                        if (Parts[1] == "8")
                            Data = BEToUint64(buf, Addr);

                    }
                }

                //Logger(DR.xml + ": " + DR.address + ": " + DR.data.ToString("X") + DR.compare + "(" + DR.grouplogic + ") " + " [" + Addr.ToString("X") + ": " + Data.ToString("X") + "]");

                if (DR.compare == "==")
                {
                    if (Data == DR.data)
                        return true;
                }
                if (DR.compare == "<")
                {
                    if (Data < DR.data)
                        return true;
                }
                if (DR.compare == ">")
                {
                    if (Data > DR.data)
                        return true;
                }
                if (DR.compare == "!=")
                {
                    if (Data != DR.data)
                        return true;
                }
                //Logger("Not match");
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string Autodetect(string FileName, byte[] buf)
        {
            string Result = "";
            
            List<string> XmlList = new List<string>();
            XmlList.Add(DetectRules[0].xml.ToLower());
            for (int s = 0; s < DetectRules.Count; s++)
            {
                //Create list of XML files we know:
                bool Found = false;
                for (int x = 0; x < XmlList.Count; x++)
                {
                    if (XmlList[x] == DetectRules[s].xml.ToLower())
                        Found = true;
                }
                if (!Found)
                    XmlList.Add(DetectRules[s].xml.ToLower());
            }
            for (int x=0; x < XmlList.Count;x++)
            {
                Debug.WriteLine("Autodetect: " + XmlList[x]);
                uint MaxGroup = 0;
                
                //Check if compatible with THIS xml
                List<DetectRule> DRL = new List<DetectRule>();
                for (int s = 0; s < DetectRules.Count; s++)
                {                    
                    if (XmlList[x] == DetectRules[s].xml.ToLower())
                    {
                        DRL.Add(DetectRules[s]);
                        if (DetectRules[s].group > MaxGroup)
                            MaxGroup = DetectRules[s].group;
                    }
                }
                //Now all rules for this XML are in DRL (DetectRuleList)
                DetectGroup[] DG = new DetectGroup[MaxGroup + 1];
                for (int d = 1; d <= MaxGroup; d++)
                {
                    //Clear DG (needed?)
                    DG[d].Hits = 0;
                    DG[d].Miss = 0;
                }
                for (int d=0; d < DRL.Count; d++)
                {
                    //This list have only rules for one XML, lets go thru them
                    DG[DRL[d].group].Logic = DRL[d].grouplogic;
                    if (CheckRule(DRL[d], FileName, buf))
                        //This check matches
                        DG[DRL[d].group].Hits++;
                    else
                        DG[DRL[d].group].Miss++;
                }
                //Now we have array DG, where hits & misses are counted per group, for this XML
                bool Detection = true;
                for (int g = 1; g <= MaxGroup; g++)
                {
                    //If all groups match, then this XML, match.
                    if (DG[g].Logic == "And")
                    {
                        //Logic = and => if any Miss, not detection
                        if (DG[g].Miss > 0)
                            Detection = false;
                    }
                    if (DG[g].Logic == "Or")
                    {
                        if (DG[g].Hits == 0)
                            Detection = false;
                    }
                    if (DG[g].Logic == "Xor")
                    {
                        if (DG[g].Hits != 1)
                            Detection = false;
                    }
                }
                if (Detection)
                {
                    //All groups have hit (if grouplogic = or, only one hit per group is a hit)
                    if (Result != "")
                        Result += Environment.NewLine;
                    Result += XmlList[x];
                    Debug.WriteLine("Rules match");
                }
                else
                    Debug.WriteLine("No match");
            }
            return Result.ToLower();
        }

        private void btnAutodetect_Click(object sender, EventArgs e)
        {
            frmAutodetect frmAD = new frmAutodetect();
            frmAD.Show();
            frmAD.InitMe();
        }


        private void btnLoadFolder_Click(object sender, EventArgs e)
        {
            string FileName = SelectFile();
            if (FileName.Length == 0)
                return;
            txtFileinfo.Text = "";
            string Fldr = Path.GetDirectoryName(FileName);
            DirectoryInfo d = new DirectoryInfo(Fldr);
            FileInfo[] Files = d.GetFiles("*.bin");
            foreach (FileInfo file in Files)
            {
                uint fsize = (uint)new System.IO.FileInfo(file.FullName).Length;
                BinFile[] binfile = new BinFile[Segments.Count];
                byte[] buf = new byte[fsize];
                GetFileInfo(file.FullName, ref binfile, ref buf);
            }

        }

        private void btnSaveFileInfo_Click(object sender, EventArgs e)
        {
            try
            {
                string FileName = SelectSaveFile("Text files (*.txt)|*.txt|All files (*.*)|*.*");
                if (FileName.Length > 1)
                {
                    StreamWriter sw = new StreamWriter(FileName);
                    sw.WriteLine(txtFileinfo.Text);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error");
            }

        }
    }

    class TextBoxTraceListener : TraceListener
    {
        private TextBox tBox;

        public TextBoxTraceListener(TextBox box)
        {
            this.tBox = box;
        }

        public override void Write(string msg)
        {
            //allows tBox to be updated from different thread
            tBox.Parent.Invoke(new MethodInvoker(delegate ()
            {
                tBox.AppendText(msg);
            }));
        }

        public override void WriteLine(string msg)
        {
            Write(msg + "\r\n");
        }
    }
}
