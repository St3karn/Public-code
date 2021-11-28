using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Globalization;
using Converter.Properties;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;


namespace Converter
{
    public partial class MainForm : Form
    {
        private HEXConverter hexConverter = new HEXConverter();
        private DTCconverter dtcConverter = new DTCconverter();
        private TextConverter textConverter = new TextConverter();
        private bool darkModeChecked;
        
        public MainForm()
        {
            InitializeComponent();
            InitializeGUI();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            GetSettings();
            ToolTipInfo();
            Conversion_table dgvData = new Conversion_table();
            dgvData.Modf(this.dgvData);
            SetFocus();
            tabController.SelectedIndexChanged += new EventHandler(TabController_SelectedIndexChanged);
            txtConverterTab.Enabled = false; // This tab is disabled cause it's not finished yet.

            //Events to disable the automatic horizontal scroll when adding text to the richtextboxes.
            rtbASCII.TextChanged += HandleRichTextBoxAdjustScroll;
            rtbHEX.TextChanged += HandleRichTextBoxAdjustScroll;
            rtbDEC.TextChanged += HandleRichTextBoxAdjustScroll;
            rtbBIN.TextChanged += HandleRichTextBoxAdjustScroll;
        }

        private const UInt32 SB_LEFT = 0x06;
        private const UInt32 WM_HSCROLL = 0x114;
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        private void HandleRichTextBoxAdjustScroll(Object sender, EventArgs e)
        {
            PostMessage(rtbASCII.Handle, WM_HSCROLL, (IntPtr)SB_LEFT, IntPtr.Zero);
            PostMessage(rtbHEX.Handle, WM_HSCROLL, (IntPtr)SB_LEFT, IntPtr.Zero);
            PostMessage(rtbDEC.Handle, WM_HSCROLL, (IntPtr)SB_LEFT, IntPtr.Zero);
            PostMessage(rtbBIN.Handle, WM_HSCROLL, (IntPtr)SB_LEFT, IntPtr.Zero);
        }

        private void TabController_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetFocus();
        }

        /// <summary>
        /// Starting parameters when initializing
        /// </summary>
        private void InitializeGUI()
        {
            //HEX Converter Tab
            tabController.SelectedIndex = 0;
            rtbASCII.MouseWheel += new MouseEventHandler(Disable_MouseWheel);
            rtbHEX.MouseWheel += new MouseEventHandler(Disable_MouseWheel);
            rtbDEC.MouseWheel += new MouseEventHandler(Disable_MouseWheel);
            rtbBIN.MouseWheel += new MouseEventHandler(Disable_MouseWheel);
            txtInput.MouseWheel += new MouseEventHandler(Disable_MouseWheel);
            rbtnASCII.Checked = true;
            rbtnByte1.Checked = true;
            //DTC Converter Tab
            dtcTbInput.MouseWheel += new MouseEventHandler(Disable_MouseWheel);
            dtcRtbOutput.MouseWheel += new MouseEventHandler(Disable_MouseWheel);
            rbtnDtcHexToDtc.Checked = true;
            dtcTbInput.Focus();
            //TextConverter Tab
            lblx1.ForeColor = Color.Red; lblx1.Visible = false;
            lblx2.ForeColor = Color.Red; lblx2.Visible = false;
            lblx3.ForeColor = Color.Red; lblx3.Visible = false;
            lblx4.ForeColor = Color.Red; lblx4.Visible = false;
            lblx5.ForeColor = Color.Red; lblx5.Visible = false;
            lblx6.ForeColor = Color.Red; lblx6.Visible = false;
            lblx7.ForeColor = Color.Red; lblx7.Visible = false;
            lblx8.ForeColor = Color.Red; lblx8.Visible = false;
            lblx9.ForeColor = Color.Red; lblx9.Visible = false;
            lblx10.ForeColor = Color.Red; lblx10.Visible = false; 
            lblx11.ForeColor = Color.Red; lblx11.Visible = false; 
            //SettingsTab
            rtbSettingsInfo.Text = AppInformation(out string appInfo);
            this.KeyPreview = true; // The entire form is listening for key events
        }

        /// <summary>
        /// Sets so the user can use shortcuts "ALT + 0-9" for changing tabs.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Alt && e.KeyValue > '0' && e.KeyValue <= '9')
            {
                tabController.SelectedIndex = (int)(e.KeyCode - '1');
                e.Handled = true;
            }
        }

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            ValueTransfer vT = ReadInput(out bool status);
            NumberOfInputBytes();

            if (status && rbtnASCII.Checked)
            {
                hexConverter.ConvertFromAscii(vT);
                UpdateGUI();
            }
            else if (status && rbtnHEX.Checked)
            {
                hexConverter.ConvertFromHex(vT);
                UpdateGUI();
            }
            else if (status && rbtnDEC.Checked)
            {
                hexConverter.ConvertFromDec(vT);
                UpdateGUI();
            }
            else if (status && rbtnBIN.Checked)
            {
                hexConverter.ConvertFromBin(vT);
                UpdateGUI();
            }
            else
                return;
            hexConverter.ClearAllLists();
        }

        private void BtnDtcConvert_Click(object sender, EventArgs e)
        {
            ValueTransfer vT = DtcReadInput();
            //DtcReadInput();
            dtcConverter.ConvertInput(vT);
            DtcUpdateGUI();
            DtcRowCounter();
        }

        /// <summary>
        /// Resets all boxes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnResetAll_Click(object sender, EventArgs e)
        {
            txtInput.Clear();
            rtbASCII.Clear();
            rtbHEX.Clear();
            rtbDEC.Clear();
            rtbBIN.Clear();
            hexConverter.ClearAllLists();
            SetFocus();
        }

        /// <summary>
        /// Resets the input box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnResetInput_Click(object sender, EventArgs e)
        {
            txtInput.Clear();
            SetFocus();
        }

        /// <summary>
        /// Reads the input. Checks which radio button that is selected and if any text is written into the textbox.
        /// It also returns the re-arranged (non-converted) value from the input to the selected RTB-box.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private ValueTransfer ReadInput(out bool status)
        {
            string newLine = "\n";
            status = false;
            ValueTransfer vT = new ValueTransfer();
            List<string> inputHEX = new List<string>();
            List<string> inputASCII = new List<string>();
            List<string> inputDEC = new List<string>();
            List<string> inputBIN = new List<string>();
            int countAscii = 0;
            int countHex = 0;
            int countDec = 0;
            int countBin = 0;
            int outputCountAscii = rtbASCII.Lines.Length;
            int outputCountHex = rtbHEX.Lines.Length;
            int outputCountDec = rtbDEC.Lines.Length;
            int outputCountBin = rtbBIN.Lines.Length;

            if (rtbASCII.Lines.Length != 0) { countAscii = outputCountAscii; }
            if (rtbHEX.Lines.Length != 0)   { countHex = outputCountHex; }
            if (rtbDEC.Lines.Length != 0)   { countDec = outputCountDec; }
            if (rtbBIN.Lines.Length != 0)   { countBin = outputCountBin; }

            if (txtInput.Lines.Length > 0 && rbtnASCII.Checked)
            {
                for (int i = 0; i < txtInput.Lines.Length; i++)
                {
                    //Removes if there are multiple whitespaces in the string and replaces it with one whitespace.
                    string newStr = txtInput.Lines[i];
                    newStr = string.Join(" ", newStr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

                    string charString = txtInput.Lines[i];

                    inputASCII.Add(charString);
                    vT.Ascii = inputASCII;

                    if (cbConverterAddRowNr.Checked)
                    {
                        countAscii = countAscii + 1;
                        if (rtbASCII.TextLength <= 0)
                        { rtbASCII.Text = countAscii + ".\t" + newStr; }
                        else
                        { rtbASCII.AppendText(newLine + countAscii + ".\t" + newStr); }
                    }
                    else
                    {
                        if (rtbASCII.TextLength <= 0)
                        { rtbASCII.Text = newStr; }    //Adding the input values into the ASCII-listbox without conversion
                        else
                        { rtbASCII.AppendText(newLine + newStr); }
                    }
                }
                status = true;
            }
            else if (txtInput.Lines.Length > 0 && rbtnHEX.Checked)
            {
                const string containsHex = "0123456789ABCDEF";

                for (int i = 0; i < txtInput.Lines.Length; i++)
                {
                    inputHEX.Add(txtInput.Lines[i].ToString().Trim().Replace(" ", "").Replace(":", " ").Replace("::", " ").ToUpper());
                    foreach (char c in inputHEX[i])
                    {
                        if (!containsHex.Contains(c))   //An information window will show if the input value contains any non-valid HEX characters.
                        {
                            MessageBox.Show("All the input characters are not in HEX-format.\nPlease check the input", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return vT;
                        }
                        else
                            vT.Hex = inputHEX;
                    }
                }

                //This calculation splits the input value every 2 character.
                StringBuilder builder = new StringBuilder();
                foreach (string str in inputHEX)
                {
                    builder.Clear();
                    string[] split = new string[str.Length / 2 + (str.Length % 2 == 0 ? 0 : 1)];
                    string format = "{0:X2} ";

                    for (int x = 0; x < split.Length; x++)
                    {
                        split[x] = str.Substring(x * 2, x * 2 + 2 > str.Length ? 1 : 2);
                        builder.AppendFormat(format, split[x].PadLeft(2, '0'));
                    }

                    if (cbConverterAddRowNr.Checked)
                    {
                        countHex = countHex + 1;
                        if (rtbHEX.TextLength <= 0)
                        { rtbHEX.Text = countHex + ".\t" + builder.ToString().TrimEnd(); }
                        else
                        { rtbHEX.AppendText(newLine + countHex + ".\t" + builder.ToString().TrimEnd());}
                    }
                    else
                    {
                        if (rtbHEX.TextLength <= 0)
                        { rtbHEX.Text = builder.ToString().TrimEnd(); }   //Adding the input values into the HEX-listbox without conversion
                        else
                        { rtbHEX.AppendText(newLine + builder.ToString().TrimEnd()); }
                    }
                }
                status = true;
            }
            else if (txtInput.Lines.Length > 0 && rbtnDEC.Checked)
            {
                const string containsDec = "0123456789 ";
                List<string> tempInputDEC = new List<string>();

                for (int i = 0; i < txtInput.Lines.Length; i++)
                {
                    //Removes if there are multiple whitespaces in the string and replaces it with one whitespace.
                    string newStr = txtInput.Lines[i];
                    newStr = string.Join(" ", newStr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

                    tempInputDEC.Add(newStr.Trim().Replace(",", " ").Replace(".", " ").Replace("-", " ").Replace("´", " ").Replace("'", " ").Replace(":", " ").Replace("::", " "));
                    foreach (char c in tempInputDEC[i])
                    {
                        if (!containsDec.Contains(c))   //An information window will show if the input value contains any non-valid DEC characters.
                        {
                            MessageBox.Show("All the input characters are not in DEC-format.\nPlease check the input", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return vT;
                        }
                    }

                    if (cbConverterAddRowNr.Checked)
                    {
                        countDec = countDec + 1;
                        if (rtbDEC.TextLength <= 0)
                        { rtbDEC.Text = countDec + ".\t" + newStr.TrimEnd().TrimEnd(); }
                        else
                        { rtbDEC.AppendText(newLine + countDec + ".\t" + newStr.TrimEnd()); }
                    }
                    else
                    {
                        if (rtbDEC.TextLength <= 0)
                        { rtbDEC.Text = newStr.TrimEnd(); }
                        else
                        { rtbDEC.AppendText(newLine + newStr.TrimEnd()); }
                    }
                }
                status = true;
                vT.Dec = tempInputDEC;
            }
            else if (txtInput.Lines.Length > 0 && rbtnBIN.Checked)
            {
                const string containsBin = "01 ";
                StringBuilder builder = new StringBuilder();
                List<string> binList = new List<string>();

                for (int i = 0; i < txtInput.Lines.Length; i++)
                {
                    StringBuilder sbTemp = new StringBuilder();
                    string tempStr = txtInput.Lines[i];
                    inputBIN.Add(txtInput.Lines[i].ToString().Trim().Replace(" ", ""));
                    builder.Clear();
                    sbTemp.Clear();

                    foreach (char c in tempStr)
                    {
                        if (!containsBin.Contains(c))   //An information window will show if the input value contains any non-valid DEC characters.
                        {
                            MessageBox.Show("All the input characters are not in BIN-format.\nPlease check the input", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return vT;
                        }
                    }

                    for (int x = 0; x < inputBIN[i].Length; x += 4)
                    {
                        if (x + 4 < inputBIN[i].Length)
                        {
                            builder.Append(inputBIN[i].Substring(x, 4) + " ");
                            sbTemp.Append(inputBIN[i].Substring(x, 4));
                        }
                        else
                        {
                            builder.Append(inputBIN[i].Substring(x).PadLeft(4, '0').ToString());
                            sbTemp.Append(inputBIN[i].Substring(x).PadLeft(4, '0').ToString());
                        }
                    }
                    binList.Add(sbTemp.ToString());
                    vT.Bin = binList;

                    if (cbConverterAddRowNr.Checked)
                    {
                        countBin = countBin + 1;
                        if (rtbBIN.TextLength <= 0)
                        { rtbBIN.Text = countBin + ".\t" + builder.ToString().TrimEnd(); }
                        else
                        { rtbBIN.AppendText(newLine + countBin + ".\t" + builder.ToString().TrimEnd()); }
                    }
                    else
                    {
                        if (rtbBIN.TextLength <= 0)
                        { rtbBIN.Text = builder.ToString() + " ".TrimEnd(); }  //Adding the input values into the BIN-listbox without conversion
                        else
                        { rtbBIN.AppendText(newLine + builder.ToString() + " ".TrimEnd()); }
                    }
                    status = true;
                }
            }
            else
            {
                MessageBox.Show("Please provide input of ASCII, HEX, DECIMAL or BINARY\nin the\"INPUT\"-box", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblInput.BackColor = Color.Red;
                SetFocus();
                return null;
            }
            return vT;
        }

        private ValueTransfer DtcReadInput()
        {
            ValueTransfer vT = new ValueTransfer();
            List<string> Input = new List<string>();
            List<string> Output = new List<string>();
            dtcConverter.rbtnDtcHexToDtc = false;
            dtcConverter.rbtnDtcDtcToHex = false;

            //Sends the radio buttons status to the DTCconverter class.
            if (rbtnDtcHexToDtc.Checked) 
            { dtcConverter.rbtnDtcHexToDtc = true; }
            else
            { dtcConverter.rbtnDtcDtcToHex = true; }

            if (dtcTbInput.Lines.Length > 0)
            {
                for (int i = 0; i < dtcTbInput.Lines.Length; i++)
                {
                    //Removes if there are multiple whitespaces in the string and replaces it with one whitespace
                    string newStr = dtcTbInput.Lines[i];
                    newStr = string.Join(" ", newStr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                    Input.Add(newStr);
                }
            }
            vT.DtcInput = Input;     //Add the input value to the "Converter" class for calculation.
            return vT;
        }

        /// <summary>
        /// Updates the DTC Converter GUI after the users input has been converted.
        /// </summary>
        private void DtcUpdateGUI()
        {
            bool invalidDTC = dtcConverter.invalidDTC;
            List<string> dtcList = new List<string>();
            dtcList = dtcConverter.GetConvertedDtc();
            string newLine = "\n";

            //In HEX to DTC is checks the first character if valid and int DTC to HEX it checks the first two characters.
            //It does not check the whole DTC code.
            if (invalidDTC)
            {
                MessageBox.Show("Invalid HEX-DTC !\n\nPlease check the input.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Adds row number before the DTC if "cbDtcAddRowNr" in the setting tab is checked.
            //Checks if there's any values in output.
            //If there are previous values in output box, the counter continues from that, otherwise it begins from one.
            int count = 0;
            int outputCounter = dtcRtbOutput.Lines.Length;
            if (dtcRtbOutput.Lines.Length != 0)
            {
                count = outputCounter;
            }

            foreach (var str in dtcList)
            {
                count = count + 1;

                if (cbDtcAddRowNr.Checked)
                {
                    if (dtcRtbOutput.TextLength <= 0)
                    { dtcRtbOutput.Text = count + ".\t" + str; }
                    else
                    { dtcRtbOutput.AppendText(newLine + count + ".\t" + str); }
                }
                else
                {
                    if (dtcRtbOutput.TextLength <= 0)
                    { dtcRtbOutput.Text = str; }
                    else
                    { dtcRtbOutput.AppendText(newLine + str); }
                }
            }
        }

        /// <summary>
        ///  Updates the Converter GUI after the users input has been converted.
        /// </summary>
        private void UpdateGUI()
        {
            string newLine = "\n";  //This is needed to set a new line in RichTextBox. Otherwise it continues after the last character on the same line.
            List<string> asciiList = new List<string>();
            List<string> hexList = new List<string>();
            List<string> decList = new List<string>();
            List<string> binList = new List<string>();
            asciiList = hexConverter.GetValueConvertedToAscii();
            hexList = hexConverter.GetValueConvertedToHex();
            decList = hexConverter.GetValueConvertedToDec();
            binList = hexConverter.GetValueConvertedToBin();
            bool conversionStatus = hexConverter.conversionStatus;
            

            //Removes all white space characters from the output string if the checkbox is ticked.
            //Overwrites the original list with the modified list.
            if ((!cbConverterWhiteSpace.Checked) && (rbtnASCII.Checked))
            {
                foreach (string str in hexList)
                {
                    List<string> modHexList = new List<string>();
                    modHexList = hexList.Select(x => x.Replace(" 20", "")).ToList();
                    hexList = modHexList;
                }
                foreach (string str in decList)
                {
                    List<string> modDecList = new List<string>();
                    modDecList = decList.Select(x => x.Replace(" 32", "")).ToList();
                    decList = modDecList;
                }
                foreach (string str in binList)
                {
                    List<string> modBinList = new List<string>();
                    modBinList = binList.Select(x => x.Replace(" 0010 0000", "")).ToList();
                    binList = modBinList;
                }
            }

            //Replaces all SPACE characters with TAB characters if user want to copy everything into seperately cells.
            if (cbExcelFormat.Checked)
            {
                foreach (string str in asciiList)
                {
                    List<string> modAsciiList = new List<string>();
                    modAsciiList = hexList.Select(x => x.Replace(" ", "\t")).ToList();
                    asciiList = modAsciiList;
                }
                foreach (string str in hexList)
                {
                    List<string> modHexList = new List<string>();
                    modHexList = hexList.Select(x => x.Replace(" ", "\t")).ToList();
                    hexList = modHexList;
                }
                foreach (string str in decList)
                {
                    List<string> modDecList = new List<string>();
                    modDecList = decList.Select(x => x.Replace(" ", "\t")).ToList();
                    decList = modDecList;
                }
                foreach (string str in binList)
                {
                    List<string> modBinList = new List<string>();
                    modBinList = binList.Select(x => x.Replace(" ", "\t")).ToList();
                    binList = modBinList;
                }
            }

            int countAscii = 0;
            int countHex = 0;
            int countDec = 0;
            int countBin = 0;
            int outputCountAscii = rtbASCII.Lines.Length;
            int outputCountHex = rtbHEX.Lines.Length;
            int outputCountDec = rtbDEC.Lines.Length;
            int outputCountBin = rtbBIN.Lines.Length;

            if (rtbASCII.Lines.Length != 0)
            { countAscii = outputCountAscii; }
            if (rtbHEX.Lines.Length != 0)
            { countHex = outputCountHex; }
            if (rtbDEC.Lines.Length != 0)
            { countDec = outputCountDec; }
            if (rtbBIN.Lines.Length != 0)
            { countBin = outputCountBin; }

            if (cbConverterAddRowNr.Checked)    //Display the converted values WITH row numbers
            {
                if (rbtnASCII.Checked)
                {
                    foreach (var str in hexList)
                    {
                        countHex = countHex + 1;
                        if (rtbHEX.TextLength <= 0)
                        { rtbHEX.Text = countHex + ".\t" + str; }
                        else
                        { rtbHEX.AppendText(newLine + countHex + ".\t" + str); }
                    }
                    foreach (var str in decList)
                    {
                        countDec = countDec + 1;
                        if (rtbDEC.TextLength <= 0)
                        { rtbDEC.Text = countDec + ".\t" + str; }
                        else
                        { rtbDEC.AppendText(newLine + countDec + ".\t" + str); }
                    }
                    foreach (var str in binList)
                    {
                        countBin = countBin + 1;
                        if (rtbBIN.TextLength <= 0)
                        { rtbBIN.Text = countBin + ".\t" + str; }
                        else
                        { rtbBIN.AppendText(newLine + countBin + ".\t" + str); }
                    }
                }
                //-----------------------------------------------------------------------------------
                else if (rbtnHEX.Checked)
                {
                    foreach (var str in asciiList)
                    {
                        countAscii = countAscii + 1;
                        if (conversionStatus)
                        {
                            if (rtbASCII.TextLength <= 0)
                            { rtbASCII.Text = countAscii + ".\t" + str; }
                            else
                            { rtbASCII.AppendText(newLine + countAscii + ".\t" + str); }
                        }
                        else
                        {
                            if (rtbASCII.TextLength <= 0)
                            { rtbASCII.Text = countAscii + ".\t" + "Conversion not valid"; }
                            else
                            { rtbASCII.AppendText(newLine + countAscii + ".\t" + "Conversion not valid"); }
                        }
                    }
                    foreach (var str in decList)
                    {
                        countDec = countDec + 1;
                        if (rtbDEC.TextLength <= 0)
                        { rtbDEC.Text = countDec + ".\t" + str; }
                        else
                        { rtbDEC.AppendText(newLine + countDec + ".\t" + str); }
                    }
                    foreach (var str in binList)
                    {
                        countBin = countBin + 1;
                        if (rtbBIN.TextLength <= 0)
                        { rtbBIN.Text = countBin + ".\t" + str; }
                        else
                        { rtbBIN.AppendText(newLine + countBin + ".\t" + str); }
                    }
                }
                //-----------------------------------------------------------------------------------
                else if (rbtnDEC.Checked)
                {
                    foreach (var str in asciiList)
                    {
                        countAscii = countAscii + 1;
                        if (conversionStatus)
                        {
                            if (rtbASCII.TextLength <= 0)
                            { rtbASCII.Text = countAscii + ".\t" + str; }
                            else
                            { rtbASCII.AppendText(newLine + countAscii + ".\t" + str); }
                        }
                        else
                        {
                            if (rtbASCII.TextLength <= 0)
                            { rtbASCII.Text = countAscii + ".\t" + "Conversion not valid"; }
                            else
                            { rtbASCII.AppendText(newLine + countAscii + ".\t" + "Conversion not valid"); }
                        }
                    }
                    foreach (var str in hexList)
                    {
                        countHex = countHex + 1;
                        if (rtbHEX.TextLength <= 0)
                        { rtbHEX.Text = countHex + ".\t" + str; }
                        else
                        { rtbHEX.AppendText(newLine + countHex + ".\t" + str); }
                    }
                    foreach (var str in binList)
                    {
                        countBin = countBin + 1;
                        if (rtbBIN.TextLength <= 0)
                        { rtbBIN.Text = countBin + ".\t" + str; }
                        else
                        { rtbBIN.AppendText(newLine + countBin + ".\t" + str); }
                    }
                }
                //-----------------------------------------------------------------------------------
                else // (rbtnBIN.Checked)
                {
                    foreach (var str in hexList)
                    {
                        countHex = countHex + 1;
                        if (rtbHEX.TextLength <= 0)
                        { rtbHEX.Text = countHex + ".\t" + str; }
                        else
                        { rtbHEX.AppendText(newLine + countHex + ".\t" + str); }
                    }
                    foreach (var str in asciiList)
                    {
                        countAscii = countAscii + 1;
                        if (conversionStatus)
                        {
                            if (rtbASCII.TextLength <= 0)
                            { rtbASCII.Text = countAscii + ".\t" + str; }
                            else
                            { rtbASCII.AppendText(newLine + countAscii + ".\t" + str); }
                        }
                        else
                        {
                            if (rtbASCII.TextLength <= 0)
                            { rtbASCII.Text = countAscii + ".\t" + "Conversion not valid"; }
                            else
                            { rtbASCII.AppendText(newLine + countAscii + ".\t" + "Conversion not valid"); }
                        }
                    }
                    foreach (var str in decList)
                    {
                        countDec = countDec + 1;
                        if (rtbDEC.TextLength <= 0)
                        { rtbDEC.Text = countDec + ".\t" + str; }
                        else
                        { rtbDEC.AppendText(newLine + countDec + ".\t" + str); }
                    }
                }
            }
            else    //Display the converted values WITHOUT row numbers
            {
                if (rbtnASCII.Checked)
                {
                    foreach (var str in hexList)
                    {
                        if (rtbHEX.TextLength <= 0)
                        { rtbHEX.Text = str; }
                        else
                        { rtbHEX.AppendText(newLine + str); }
                    }
                    foreach (var str in decList)
                    {
                        if (rtbDEC.TextLength <= 0)
                        { rtbDEC.Text = str; }
                        else
                        { rtbDEC.AppendText(newLine + str); }
                    }
                    foreach (var str in binList)
                    {
                        if (rtbBIN.TextLength <= 0)
                        { rtbBIN.Text = str; }
                        else
                        { rtbBIN.AppendText(newLine + str); }
                    }
                }
                //-----------------------------------------------------------------------------------
                else if (rbtnHEX.Checked)
                {
                    foreach (var str in asciiList)
                    {
                        if (conversionStatus)
                        {
                            if (rtbASCII.TextLength <= 0)
                            { rtbASCII.Text = str; }
                            else
                            { rtbASCII.AppendText(newLine + str); }
                        }
                        else
                        {
                            if (rtbASCII.TextLength <= 0)
                            { rtbASCII.Text = "Conversion not valid"; }
                            else
                            { rtbASCII.AppendText(newLine + "Conversion not valid"); }
                        }
                    }
                    foreach (var str in decList)
                    {
                        Color color;
                        if (cbCommonDarkMode.Checked)
                        { color = Color.Cyan; }
                        else
                        { color = Color.Black; }

                        if (rtbDEC.TextLength <= 0)
                        {
                            HighlightPhrase(rtbDEC, "x", color); //Resets the color. Otherwise the whole next row will become red.
                            rtbDEC.Text = str;
                            HighlightPhrase(rtbDEC, "x", Color.Red); // Sets all 'x' character to RED.
                        }
                        else
                        {
                            HighlightPhrase(rtbDEC, "x", color); //Resets the color. Otherwise the whole next row will become red.
                            rtbDEC.AppendText(newLine + str);
                            HighlightPhrase(rtbDEC, "x", Color.Red); // Sets all 'x' character to RED.
                        }

                    }
                    foreach (var str in binList)
                    {
                        if (rtbBIN.TextLength <= 0)
                        { rtbBIN.Text = str; }
                        else
                        { rtbBIN.AppendText(newLine + str); }
                    }
                }
                //-----------------------------------------------------------------------------------
                else if (rbtnDEC.Checked)
                {
                    foreach (var str in asciiList)
                    {
                        if (conversionStatus)
                        {
                            if (rtbASCII.TextLength <= 0)
                            { rtbASCII.Text = str; }
                            else
                            { rtbASCII.AppendText(newLine + str); }
                        }
                        else
                        {
                            if (rtbASCII.TextLength <= 0)
                            { rtbASCII.Text = "Conversion not valid"; }
                            else
                            { rtbASCII.AppendText(newLine + "Conversion not valid"); }
                        }
                    }
                    foreach (var str in hexList)
                    {
                        if (rtbHEX.TextLength <= 0)
                        { rtbHEX.Text = str; }
                        else
                        { rtbHEX.AppendText(newLine + str); }
                    }
                    foreach (var str in binList)
                    {
                        if (rtbBIN.TextLength <= 0)
                        { rtbBIN.Text = str; }
                        else
                        { rtbBIN.AppendText(newLine + str); }
                    }
                }
                //-----------------------------------------------------------------------------------
                else // (rbtnBIN.Checked)
                {
                    foreach (var str in hexList)
                    {
                        if (rtbHEX.TextLength <= 0)
                        { rtbHEX.Text = str; }
                        else
                        { rtbHEX.AppendText(newLine + str); }
                    }
                    foreach (var str in asciiList)
                    {
                        if (conversionStatus)
                        {
                            if (rtbASCII.TextLength <= 0)
                            { rtbASCII.Text = str; }
                            else
                            { rtbASCII.AppendText(newLine + str); }
                        }
                        else
                        {
                            if (rtbASCII.TextLength <= 0)
                            { rtbASCII.Text = "Conversion not valid"; }
                            else
                            { rtbASCII.AppendText("Conversion not valid"); }
                        }
                    }
                    foreach (var str in decList)
                    {
                        if (rtbDEC.TextLength <= 0)
                        { rtbDEC.Text = str; }
                        else
                        { rtbDEC.AppendText(newLine + str); }
                    }
                }
            }

            if (cbConverterEraseAfterConvert.Checked)
            {
                txtInput.Clear();
            }
            asciiList.Clear();
            hexList.Clear();
            decList.Clear();
            binList.Clear();
            SetFocus();
        }

        /// <summary>
        /// Marks non-conversible red.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="phrase"></param>
        /// <param name="color"></param>
        private static void HighlightPhrase(RichTextBox box, string phrase, Color color)
        {
            int pos = box.SelectionStart;
            string s = box.Text;
            for (int ix = 0; ;)
            {
                int jx = s.IndexOf(phrase, ix, StringComparison.CurrentCultureIgnoreCase);
                if (jx < 0) break;
                box.SelectionStart = jx;
                box.SelectionLength = phrase.Length;
                box.SelectionColor = color;
                ix = jx + 1;
            }
            box.SelectionStart = pos;
            box.SelectionLength = 0;
        }

        /// <summary>
        /// This method allows user to activate dark/light mode. Settings are saved autmatically when user checks/unchecks this checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbDarkMode_CheckedChanged_1(object sender, EventArgs e)
        {
            if (cbCommonDarkMode.Checked)
            {
                //----------------------- Backcolor -----------------------
                //HEX Converter
                txtInput.BackColor = ColorTranslator.FromHtml("#303030");
                rtbASCII.BackColor = ColorTranslator.FromHtml("#303030");
                rtbHEX.BackColor = ColorTranslator.FromHtml("#303030");
                rtbDEC.BackColor = ColorTranslator.FromHtml("#303030");
                rtbBIN.BackColor = ColorTranslator.FromHtml("#303030");
                btnConvert.BackColor = ColorTranslator.FromHtml("#303030");
                btnResetAll.BackColor = ColorTranslator.FromHtml("#303030");
                btnResetInput.BackColor = ColorTranslator.FromHtml("#303030");
                btnTextMinus.BackColor = ColorTranslator.FromHtml("#303030");
                btnTextPlus.BackColor = ColorTranslator.FromHtml("#303030");
                btnResetCharSize.BackColor = ColorTranslator.FromHtml("#303030");
                panelAux.BackColor = ColorTranslator.FromHtml("#303030");
                converterTab.BackColor = ColorTranslator.FromHtml("#303030");
                settingTab.BackColor = ColorTranslator.FromHtml("#303030");
                btnResetOutput.BackColor = ColorTranslator.FromHtml("#303030");
                //DTC Converter
                dtcTbInput.BackColor = ColorTranslator.FromHtml("#303030");
                dtcRtbOutput.BackColor = ColorTranslator.FromHtml("#303030");
                btnDtcConvert.BackColor = ColorTranslator.FromHtml("#303030");
                btnDtcResetInput.BackColor = ColorTranslator.FromHtml("#303030");
                btnDtcResetOutput.BackColor = ColorTranslator.FromHtml("#303030");
                btnDtcResetAll.BackColor = ColorTranslator.FromHtml("#303030");
                grbDtcInputFormat.BackColor = ColorTranslator.FromHtml("#303030");
                tlpDTC.BackColor = ColorTranslator.FromHtml("#303030");
                dtcTab.BackColor = ColorTranslator.FromHtml("#303030");
                //Text Converter
                txtConverterTab.BackColor = ColorTranslator.FromHtml("#303030");
                tbFindAndArrMsg1.BackColor = ColorTranslator.FromHtml("#303030");
                tbFindAndArrMsg2.BackColor = ColorTranslator.FromHtml("#303030");
                tbFindMsg.BackColor = ColorTranslator.FromHtml("#303030");
                tbFindMsgConvTo.BackColor = ColorTranslator.FromHtml("#303030");
                tbRemoveReqAns.BackColor = ColorTranslator.FromHtml("#303030");
                panel1.BackColor = ColorTranslator.FromHtml("#303030");
                panel2.BackColor = ColorTranslator.FromHtml("#303030");
                panel3.BackColor = ColorTranslator.FromHtml("#303030");
                panel4.BackColor = ColorTranslator.FromHtml("#303030");
                panel5.BackColor = ColorTranslator.FromHtml("#303030");
                panel6.BackColor = ColorTranslator.FromHtml("#303030");
                gbTxtSettings.BackColor = ColorTranslator.FromHtml("#303030");
                tlpTxtConverter.BackColor = ColorTranslator.FromHtml("#303030");
                rtbTxtInput.BackColor = ColorTranslator.FromHtml("#303030");
                rtbTxtOutput.BackColor = ColorTranslator.FromHtml("#303030");
                btnTxtConvert.BackColor = ColorTranslator.FromHtml("#303030");
                btnTxtResetInput.BackColor = ColorTranslator.FromHtml("#303030");
                btnTxtResetOutput.BackColor = ColorTranslator.FromHtml("#303030");
                btnTxtResetAll.BackColor = ColorTranslator.FromHtml("#303030");
                btnTxtFind.BackColor = ColorTranslator.FromHtml("#303030");
                //Conversion Table
                converterTableTab.BackColor = ColorTranslator.FromHtml("#303030");
                dgvData.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#303030");
                dgvData.BackgroundColor = ColorTranslator.FromHtml("#303030");
                dgvData.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#303030");
                //Settings
                rtbSettingsInfo.BackColor = ColorTranslator.FromHtml("#303030");

                //----------------------- Forecolor -----------------------
                //HEX Converter
                txtInput.ForeColor = Color.Cyan;
                rtbASCII.ForeColor = Color.Cyan;
                rtbHEX.ForeColor = Color.Cyan;
                rtbDEC.ForeColor = Color.Cyan;
                rtbBIN.ForeColor = Color.Cyan;
                grbInputFormat.ForeColor = Color.Cyan;
                cbCommonDarkMode.ForeColor = Color.Cyan;
                lblTextSize.ForeColor = Color.Cyan;
                cbConverterEraseAfterConvert.ForeColor = Color.Cyan;
                cbConverterWhiteSpace.ForeColor = Color.Cyan;
                btnTextPlus.ForeColor = Color.Black;
                btnTextMinus.ForeColor = Color.Black;
                btnConvert.ForeColor = Color.Orange;
                btnResetAll.ForeColor = Color.Orange;
                btnResetInput.ForeColor = Color.Orange;
                btnTextMinus.ForeColor = Color.Orange;
                btnTextPlus.ForeColor = Color.Orange;
                btnResetCharSize.ForeColor = Color.Orange;
                grpInputBytes.ForeColor = Color.Cyan;
                btnResetOutput.ForeColor = Color.Orange;
                //DTC Converter
                dtcTbInput.ForeColor = Color.Cyan;
                dtcRtbOutput.ForeColor = Color.Cyan;
                btnDtcConvert.ForeColor = Color.Orange;
                btnDtcResetInput.ForeColor = Color.Orange;
                btnDtcResetOutput.ForeColor = Color.Orange;
                btnDtcResetAll.ForeColor = Color.Orange;
                rbtnDtcHexToDtc.ForeColor = Color.Cyan;
                rbtnDtcDtcToHex.ForeColor = Color.Cyan;
                grbDtcInputFormat.ForeColor = Color.Cyan;
                lbDtcInputRowCount.ForeColor = Color.Orange;
                lbDtcOutputRowCount.ForeColor = Color.Orange;
                //Text Converter
                tbFindAndArrMsg1.ForeColor = Color.Cyan;
                tbFindAndArrMsg2.ForeColor = Color.Cyan;
                tbFindMsg.ForeColor = Color.Cyan;
                tbFindMsgConvTo.ForeColor = Color.Cyan;
                tbRemoveReqAns.ForeColor = Color.Cyan;
                panel1.ForeColor = Color.Cyan;
                panel2.ForeColor = Color.Cyan;
                panel3.ForeColor = Color.Cyan;
                panel4.ForeColor = Color.Cyan;
                panel5.ForeColor = Color.Cyan;
                panel6.ForeColor = Color.Cyan;
                gbTxtSettings.ForeColor = Color.Cyan;
                tlpTxtConverter.ForeColor = Color.Cyan;
                rtbTxtInput.ForeColor = Color.Cyan;
                rtbTxtOutput.ForeColor = Color.Cyan;
                btnTxtConvert.ForeColor = Color.Orange;
                btnTxtResetInput.ForeColor = Color.Orange;
                btnTxtResetOutput.ForeColor = Color.Orange;
                btnTxtResetAll.ForeColor = Color.Orange;
                lblCRInfo.ForeColor = Color.Cyan;
                lblLFInfo.ForeColor = Color.Cyan;
                btnTxtFind.ForeColor = Color.Orange;
                //Conversion Table
                dgvData.ColumnHeadersDefaultCellStyle.ForeColor = Color.Cyan;
                dgvData.DefaultCellStyle.ForeColor = Color.Cyan;
                //Settings
                gbCommonSettings.ForeColor = Color.Cyan;
                gbConverterSettings.ForeColor = Color.Cyan;
                gbDtcConverterSettings.ForeColor = Color.Cyan;
                rtbSettingsInfo.ForeColor = Color.Cyan;

                Properties.Settings.Default.DarkMode = cbCommonDarkMode.Checked = true;
                Properties.Settings.Default.Save();
                darkModeChecked = true;
            }
            else
            {
                //----------------------- Backcolor -----------------------
                //HEX Converter
                this.BackColor = Control.DefaultBackColor;
                txtInput.BackColor = Color.White;
                rtbASCII.BackColor = Color.White;
                rtbHEX.BackColor = Color.White;
                rtbDEC.BackColor = Color.White;
                rtbBIN.BackColor = Color.White;
                btnConvert.BackColor = Color.Empty;
                btnResetAll.BackColor = Color.Empty;
                btnResetInput.BackColor = Color.Empty;
                btnTextMinus.BackColor = Color.Empty;
                btnTextPlus.BackColor = Color.Empty;
                btnResetCharSize.BackColor = Color.Empty;
                panelAux.BackColor = Color.Empty;
                converterTab.BackColor = Color.Empty;
                btnResetOutput.BackColor = Color.Empty;
                //DTC Converter
                dtcTbInput.BackColor = Color.White;
                dtcRtbOutput.BackColor = Color.White;
                btnDtcConvert.BackColor = Color.Empty;
                btnDtcResetInput.BackColor = Color.Empty;
                btnDtcResetOutput.BackColor = Color.Empty;
                btnDtcResetAll.BackColor = Color.Empty;
                grbDtcInputFormat.BackColor = Color.Empty;
                tlpDTC.BackColor = Color.Empty;
                dtcTab.BackColor = Color.Empty;
                //Text Converter
                txtConverterTab.BackColor = Color.Empty;
                tbFindAndArrMsg1.BackColor = Color.White;
                tbFindAndArrMsg2.BackColor = Color.White;
                tbFindMsg.BackColor = Color.White;
                tbFindMsgConvTo.BackColor = Color.White;
                tbRemoveReqAns.BackColor = Color.White;
                panel1.BackColor = Color.Empty;
                panel2.BackColor = Color.Empty;
                panel3.BackColor = Color.Empty;
                panel4.BackColor = Color.Empty;
                panel5.BackColor = Color.Empty;
                panel6.BackColor = Color.Empty;
                gbTxtSettings.BackColor = Color.Empty;
                tlpTxtConverter.BackColor = Color.Empty;
                rtbTxtInput.BackColor = Color.White;
                rtbTxtOutput.BackColor = Color.White;
                btnTxtConvert.BackColor = Color.Empty;
                btnTxtResetInput.BackColor = Color.Empty;
                btnTxtResetOutput.BackColor = Color.Empty;
                btnTxtResetAll.BackColor = Color.Empty;
                btnTxtFind.BackColor = Color.Empty;
                //Conversion Table
                converterTableTab.BackColor = Color.White;
                dgvData.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
                dgvData.BackgroundColor = Color.White;
                dgvData.DefaultCellStyle.BackColor = Color.White;
                //Settings
                settingTab.BackColor = Color.Empty;
                rtbSettingsInfo.BackColor = Color.Empty;

                //----------------------- Forecolor -----------------------
                //HEX Converter
                txtInput.ForeColor = Color.Black;
                rtbASCII.ForeColor = Color.Black;
                rtbHEX.ForeColor = Color.Black;
                rtbDEC.ForeColor = Color.Black;
                rtbBIN.ForeColor = Color.Black;
                grbInputFormat.ForeColor = Color.Black;
                lblInput.ForeColor = Color.Black;
                cbCommonDarkMode.ForeColor = Color.Black;
                lblTextSize.ForeColor = Color.Black;
                cbConverterEraseAfterConvert.ForeColor = Color.Black;
                cbConverterWhiteSpace.ForeColor = Color.Black;
                btnTextPlus.ForeColor = Color.Black;
                btnTextMinus.ForeColor = Color.Black;
                btnResetCharSize.ForeColor = Color.Black;
                btnConvert.ForeColor = Color.Black;
                btnResetAll.ForeColor = Color.Black;
                btnResetInput.ForeColor = Color.Black;
                btnTextMinus.ForeColor = Color.Black;
                btnTextPlus.ForeColor = Color.Black;
                btnResetCharSize.ForeColor = Color.Black;
                grpInputBytes.ForeColor = Color.Black;
                btnResetOutput.ForeColor = Color.Black;
                //DTC Converter
                dtcTbInput.ForeColor = Color.Black;
                dtcRtbOutput.ForeColor = Color.Black;
                btnDtcConvert.ForeColor = Color.Black;
                btnDtcResetInput.ForeColor = Color.Black;
                btnDtcResetAll.ForeColor = Color.Black;
                rbtnDtcHexToDtc.ForeColor = Color.Black;
                rbtnDtcDtcToHex.ForeColor = Color.Black;
                grbDtcInputFormat.ForeColor = Color.Black;
                lbDtcInputRowCount.ForeColor = Color.Black;
                lbDtcOutputRowCount.ForeColor = Color.Black;
                //Text Converter
                tbFindAndArrMsg1.ForeColor = Color.Black;
                tbFindAndArrMsg2.ForeColor = Color.Black;
                tbFindMsg.ForeColor = Color.Black;
                tbFindMsgConvTo.ForeColor = Color.Black;
                tbRemoveReqAns.ForeColor = Color.Black;
                panel1.ForeColor = Color.Black;
                panel2.ForeColor = Color.Black;
                panel3.ForeColor = Color.Black;
                panel4.ForeColor = Color.Black;
                panel5.ForeColor = Color.Black;
                panel6.ForeColor = Color.Black;
                gbTxtSettings.ForeColor = Color.Black;
                tlpTxtConverter.ForeColor = Color.Black;
                rtbTxtInput.ForeColor = Color.Black;
                rtbTxtOutput.ForeColor = Color.Black;
                btnTxtConvert.ForeColor = Color.Black;
                btnTxtResetInput.ForeColor = Color.Black;
                btnTxtResetOutput.ForeColor = Color.Black;
                btnTxtResetAll.ForeColor = Color.Black;
                lblCRInfo.ForeColor = Color.Black;
                lblLFInfo.ForeColor = Color.Black;
                btnTxtFind.ForeColor = Color.Black;
                //Conversion Table
                dgvData.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                dgvData.DefaultCellStyle.ForeColor = Color.Black;
                //Settings
                gbCommonSettings.ForeColor = Color.Black;
                gbConverterSettings.ForeColor = Color.Black;
                gbDtcConverterSettings.ForeColor = Color.Black;
                rtbSettingsInfo.ForeColor = Color.Black;

                Properties.Settings.Default.DarkMode = cbCommonDarkMode.Checked = false;
                Properties.Settings.Default.Save();
                darkModeChecked = false;
            }
        }

        /// <summary>
        /// Gets users last saved settings.
        /// </summary>
        private void GetSettings()
        {
            float savedFontSize = Properties.Settings.Default.FontSize;
            Font currentFont = rtbASCII.SelectionFont;
            FontStyle newFontStyle = (FontStyle)(currentFont.Style | FontStyle.Regular);
            rtbASCII.Font = new Font(currentFont.FontFamily, savedFontSize, newFontStyle);
            rtbHEX.Font = new Font(currentFont.FontFamily, savedFontSize, newFontStyle);
            rtbDEC.Font = new Font(currentFont.FontFamily, savedFontSize, newFontStyle);
            rtbBIN.Font = new Font(currentFont.FontFamily, savedFontSize, newFontStyle);
            txtInput.Font = new Font(currentFont.FontFamily, savedFontSize, newFontStyle);
            dtcTbInput.Font = new Font(currentFont.FontFamily, savedFontSize, newFontStyle);
            dtcRtbOutput.Font = new Font(currentFont.FontFamily, savedFontSize, newFontStyle);

            cbCommonDarkMode.Checked = Properties.Settings.Default.DarkMode;
            cbConverterWhiteSpace.Checked = Properties.Settings.Default.Whitespace;
            cbConverterEraseAfterConvert.Checked = Properties.Settings.Default.EraseInput;
            cbConverterAddRowNr.Checked = Properties.Settings.Default.ConverterAddRowNr;
            cbDtcAddRowNr.Checked = Properties.Settings.Default.DtcAddRowNr;
            cbNotCaseSensitive.Checked = Properties.Settings.Default.nonCaseSensitive;
        }

        /// <summary>
        /// Increases the font size in "txtInput", "rtbASCII", "rtbHEX" and "rtbDEC".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnTextPlus_Click(object sender, EventArgs e)
        {
            //Gets the current font size and increases the value with "+5"..
            float size = rtbASCII.SelectionFont.Size + 5;

            Font currentFont = rtbASCII.SelectionFont;  //Uses "rtbASCII" as a reference value to know which font that is applied.
            FontStyle newFontStyle = (FontStyle)(currentFont.Style | FontStyle.Regular);
            rtbASCII.Font = new Font(currentFont.FontFamily, size, newFontStyle);
            rtbHEX.Font = new Font(currentFont.FontFamily, size, newFontStyle);
            rtbDEC.Font = new Font(currentFont.FontFamily, size, newFontStyle);
            rtbBIN.Font = new Font(currentFont.FontFamily, size, newFontStyle);
            txtInput.Font = new Font(currentFont.FontFamily, size, newFontStyle);
            dtcTbInput.Font = new Font(currentFont.FontFamily, size, newFontStyle);
            dtcRtbOutput.Font = new Font(currentFont.FontFamily, size, newFontStyle);

            Properties.Settings.Default.FontSize = size;
            Properties.Settings.Default.Save();
            SetFocus();
        }

        /// <summary>
        /// Decreases the font size in "txtInput", "rtbASCII", "rtbHEX" and "rtbDEC".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnTextMinus_Click(object sender, EventArgs e)
        {
            //Gets the current font size and decreasing the value with "-5".
            float size = rtbASCII.SelectionFont.Size - 5;

            //Font size cannot be smaller than "10". If this value gets lower than "0", it sets a error exception
            if (rtbASCII.SelectionFont.Size > 10)
            {
                Font currentFont = rtbASCII.SelectionFont;  //Uses "rtbASCII" as a reference value to know which font that is applied.
                FontStyle newFontStyle = (FontStyle)(currentFont.Style | FontStyle.Regular);
                rtbASCII.Font = new Font(currentFont.FontFamily, size, newFontStyle);
                rtbHEX.Font = new Font(currentFont.FontFamily, size, newFontStyle);
                rtbDEC.Font = new Font(currentFont.FontFamily, size, newFontStyle);
                rtbBIN.Font = new Font(currentFont.FontFamily, size, newFontStyle);
                txtInput.Font = new Font(currentFont.FontFamily, size, newFontStyle);
                dtcTbInput.Font = new Font(currentFont.FontFamily, size, newFontStyle);
                dtcRtbOutput.Font = new Font(currentFont.FontFamily, size, newFontStyle);

                Properties.Settings.Default.FontSize = size;
                Properties.Settings.Default.Save();
                SetFocus();
            }
            else
                return;
        }

        /// <summary>
        /// Reset the size of all the chars to default value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnResetCharSize_Click(object sender, EventArgs e)
        {
            float size = 15;

            Font currentFont = rtbASCII.SelectionFont;  //Uses "rtbASCII" as a reference value to know which font that is applied.
            FontStyle newFontStyle = (FontStyle)(currentFont.Style | FontStyle.Regular);
            rtbASCII.Font = new Font(currentFont.FontFamily, size, newFontStyle);
            rtbHEX.Font = new Font(currentFont.FontFamily, size, newFontStyle);
            rtbDEC.Font = new Font(currentFont.FontFamily, size, newFontStyle);
            rtbBIN.Font = new Font(currentFont.FontFamily, size, newFontStyle);
            txtInput.Font = new Font(currentFont.FontFamily, size, newFontStyle);
            dtcTbInput.Font = new Font(currentFont.FontFamily, size, newFontStyle);
            dtcRtbOutput.Font = new Font(currentFont.FontFamily, size, newFontStyle);

            Properties.Settings.Default.FontSize = size;
            Properties.Settings.Default.Save();
            SetFocus();
        }
        /// <summary>
        /// If the input text box is empty the "Input"-label turns red. This method restores the color to it's origial when the user
        /// types any character.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtInput_TextChanged(object sender, EventArgs e)
        {
            lblInput.BackColor = Color.Cyan;
        }

        /// <summary>
        /// Set the focus of the marker to the input box after pressing radio button of the input format selection (ASCII)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RbtnASCII_CheckedChanged(object sender, EventArgs e)
        {
            grpInputBytes.Visible = false;
            SetFocus();
        }
        /// <summary>
        /// Set the focus of the marker to the input box after pressing radio button of the input format selection (HEX)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RbtnHEX_CheckedChanged(object sender, EventArgs e)
        {
            grpInputBytes.Visible = true;
            SetFocus();
        }
        /// <summary>
        /// Set the focus of the marker to the input box after pressing radio button of the input format selection (DEC)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RbtnDEC_CheckedChanged(object sender, EventArgs e)
        {
            grpInputBytes.Visible = false;
            SetFocus();
        }
        /// <summary>
        /// Set the focus of the marker to the input box after pressing radio button of the input format selection (BIN)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RbtnBIN_CheckedChanged(object sender, EventArgs e)
        {
            grpInputBytes.Visible = false;
            SetFocus();
        }

        /// <summary>
        /// Event that show the About box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            About about = new About();
            {
                about.darkModeChecked = darkModeChecked;
            }
            about.Show();
        }

        private void CbConverterWhiteSpace_CheckedChanged(object sender, EventArgs e)
        {
            if (cbConverterWhiteSpace.Checked)
            {
                Properties.Settings.Default.Whitespace = cbConverterWhiteSpace.Checked = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.Whitespace = cbConverterWhiteSpace.Checked = false;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// Saves the "Erase after convert" checkbox setting after each check or uncheck.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbConverterEraseAfterConvert_CheckedChanged_1(object sender, EventArgs e)
        {
            if (cbConverterEraseAfterConvert.Checked)
            {
                Properties.Settings.Default.EraseInput = cbConverterEraseAfterConvert.Checked = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.EraseInput = cbConverterEraseAfterConvert.Checked = false;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// Showing tool tip info boxes.
        /// </summary>
        private void ToolTipInfo()
        {
            ttWhiteSpace.SetToolTip(this.cbConverterWhiteSpace, "When converting from ASCII, if this checkbox is ticked, it will\n" +
                                                       "include the whitespace value in HEX, DEC and BIN -boxes.\n" +
                                                       "HEX whitespace value = 20\n" +
                                                       "DEC whitespace value = 32\n" +
                                                       "BIN whitespace value = 0010 0000");

            ttEraseInput.SetToolTip(this.cbConverterEraseAfterConvert, "When pressing \"Convert\"-button, if this checkbox is ticked,\n" +
                                                              "it will erase the \"Input\"-box each time.");
        }

        /// <summary>
        /// Disable zooming with CTRL + mousewheel up/down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Disable_MouseWheel(object sender, MouseEventArgs e)
        {
            if (ModifierKeys.HasFlag(Keys.Control))
            {
                ((HandledMouseEventArgs)e).Handled = true;
            }
        }

        /// <summary>
        /// Checks which tab that is active and sets the focus to the defined areas.
        /// </summary>
        private void SetFocus()
        {
            if (tabController.SelectedIndex == 0)
            {
                txtInput.Select();
            }
            else if (tabController.SelectedTab == dtcTab)
            {
                dtcTbInput.Focus();
            }
            else if (tabController.SelectedTab == txtConverterTab)
            {
                rtbTxtInput.Select();
            }
            else
                return;
        }

        /// <summary>
        /// Counter that counts the rows/DTC's in Input and Output.
        /// </summary>
        private void DtcRowCounter()
        {
            var inputLines = dtcTbInput.Lines.Where(line => !String.IsNullOrWhiteSpace(line)).Count();
            var outputLines = dtcRtbOutput.Lines.Where(line => !String.IsNullOrWhiteSpace(line)).Count();

            lbDtcInputRowCount.Text = "Row count: " + inputLines;
            lbDtcOutputRowCount.Text = "Row count: " + outputLines;
        }

        private void BtnDtcResetInput_Click_1(object sender, EventArgs e)
        {
            dtcTbInput.Clear();
            dtcTbInput.Focus();
        }


        private void BtnDtcResetOutput_Click(object sender, EventArgs e)
        {
            dtcRtbOutput.Clear();
            dtcTbInput.Focus();
        }

        private void BtnDtcResetAll_Click_1(object sender, EventArgs e)
        {
            //dtcListMemory.Clear();  //The method DtcUpdateGUI uses this.
            dtcTbInput.Clear();
            dtcRtbOutput.Clear();
            dtcTbInput.Focus();
        }

        private void CbConverterAddRowNr_CheckedChanged(object sender, EventArgs e)
        {
            if (cbConverterAddRowNr.Checked)
            {
                Properties.Settings.Default.ConverterAddRowNr = cbConverterAddRowNr.Checked = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.ConverterAddRowNr = cbConverterAddRowNr.Checked = false;
                Properties.Settings.Default.Save();
            }
        }

        private void CbDtcAddRowNr_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDtcAddRowNr.Checked)
            {
                Properties.Settings.Default.DtcAddRowNr = cbDtcAddRowNr.Checked = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.DtcAddRowNr = cbDtcAddRowNr.Checked = false;
                Properties.Settings.Default.Save();
            }
        }

        private void NumberOfInputBytes()
        {
            bool oneByte, twoBytes, threeBytes, fourBytes, fiveBytes, sixBytes, sevenBytes, eightBytes = false;

            if (rbtnByte1.Checked) { oneByte = true; hexConverter.oneByte = oneByte; }
            if (rbtnByte2.Checked) { twoBytes = true; hexConverter.twoBytes = twoBytes; }
            if (rbtnByte3.Checked) { threeBytes = true; hexConverter.threeBytes = threeBytes; }
            if (rbtnByte4.Checked) { fourBytes = true; hexConverter.fourBytes = fourBytes; }
            if (rbtnByte5.Checked) { fiveBytes = true; hexConverter.fiveBytes = fiveBytes; }
            if (rbtnByte6.Checked) { sixBytes = true; hexConverter.sixBytes = sixBytes; }
            if (rbtnByte7.Checked) { sevenBytes = true; hexConverter.sevenBytes = sevenBytes; }
            if (rbtnByte8.Checked) { eightBytes = true; hexConverter.eightBytes = eightBytes; }
        }

        /// <summary>
        /// Adds information to the user about this application in the settings tab richtextbox.
        /// </summary>
        /// <param name="appInfo"></param>
        /// <returns></returns>
        private string AppInformation(out string appInfo)
        {
            appInfo = "\n- All settings are saved automatically when changes is done." +
                      "\n\n" +
                      "\n\n-------------------------------------------------------------HEX CONVERTER-------------------------------------------------------------" +
                      "\n\n- When converting to ASCII, it can be \"empty rows/values\". This is the conversion within codepage 850 translation." +
                      "\n\n- When the checkboxes \"Add row number\" is checked, it add a row number before each row with a TAB-space to set the number and the value in different cells if copying to Excel." +
                      "\n\n- HEX value \"00-1F\", DEC value \"0-31\" and BIN value \"0000 0000-0001 1111\" is not valid in this application when converting to ASCII, since these are unprintable control codes." +
                      "\n\n- When checkbox \"Excel output format\" is checked, TAB will be used instead of SPACE between all values. This to set values in seperately cells in Excel." +
                      "\n\n- The shortcut ALT + 0-9 can be used for changing tabs. First tab = ALT + 1, second tab = ALT + 2 etc" +
                      "\n\n- When converting from HEX -> DEC, for example, the selected bytes to convert is set on 3 bytes and the input string is 5 bytes á FF, the result will be \"16777215 xxxx\". This because it's only the first 3 bytes that ads up to the selected bytes." +
                      "\n\n" +
                      "\n\n-------------------------------------------------------------DTC CONVERTER-------------------------------------------------------------" +
                      "\n\n- Converts the first characters" +
                      "\n\n" +
                      "\n\n-------------------------------------------------------------TEXT CONVERTER-------------------------------------------------------------" +
                      "\n\n- If several boxes/panels are checked, the priority order will be from the top to the bottom.";
            return appInfo;
        }

        /// <summary>
        /// When pressing on the "Reset Output" -button, all output boxes will be cleared.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnResetOutput_Click(object sender, EventArgs e)
        {
            rtbASCII.Clear();
            rtbHEX.Clear();
            rtbDEC.Clear();
            rtbBIN.Clear();
        }

        /// <summary>
        /// When user press Convert in Text Converter Tab, the textbox values sends to TextConverter class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnTxtConvert_Click(object sender, EventArgs e)
        {
            lblx1.Visible = false; lblx2.Visible = false; lblx3.Visible = false;
            lblx4.Visible = false; lblx5.Visible = false; lblx6.Visible = false;
            lblx7.Visible = false; lblx8.Visible = false; lblx9.Visible = false;
            lblx10.Visible = false; lblx11.Visible = false;

            ValueTransfer vT = new ValueTransfer();
            TextConverter textConverter = new TextConverter();

            
            string strFindAndArrMsg1, strFindAndArrMsg2, strFindMsg, strFindMsgConvTo, strRemoveReqAns;

            strFindAndArrMsg1 = tbFindAndArrMsg1.Text;
            strFindAndArrMsg2 = tbFindAndArrMsg2.Text;
            strFindMsg = tbFindMsg.Text;
            strFindMsgConvTo = tbFindMsgConvTo.Text;
            strRemoveReqAns = tbRemoveReqAns.Text;

            vT.StrFindAndArrMsg1 = strFindAndArrMsg1;
            vT.StrFindAndArrMsg2 = strFindAndArrMsg2;
            vT.StrFindMsg = strFindMsg;
            vT.StrFindMsgConvTo = strFindMsgConvTo;
            vT.StrRemoveReqAns = strRemoveReqAns;

            if (cbNotCaseSensitive.Checked)
            { vT.NonCaseSensitive = true; }
            if (cbFindAndArrMsg.Checked)
            { vT.FindAndArrMsg = true; }
            if (cbRemoveReqAns.Checked)
            { vT.RemoveReqAns = true; }
            //if (cbFindMsg.Checked)
            //{ vT.FindMsg = true; }

            




            if (cbFindAndArrMsg.Checked)                            { textConverter.FindAndRearrangeMsg(vT); }
            if (cbRemoveReqAns.Checked)                             { vT.TesterID = tbTesterID.Text.ToUpper(); vT.EcuID = tbEcuUD.Text.ToUpper(); textConverter.RemoveReqAndAnswer(vT); }
            if (cbFindMsg.Checked)                                  { textConverter.FindMsg(); }
            if (cbLF1.Checked || cbCR1.Checked || cbCRLF1.Checked)  { textConverter.ReplaceCRLF(); }
            if (cbUnderlineToWhitespace.Checked)                    { textConverter.UnderlineToWhitespace(); }
            if (cbWhitespaceToUnderline.Checked)                    { textConverter.WhitespaceToUnderline(); }
            if (cbRemoveAnsBb.Checked)                              { textConverter.RemoveAnswerBusBuddy(); }
            if (cbRemoveExcessByte.Checked)                         { textConverter.RemoveExcessiveBytes(); }
            if (cbLogToBbRec.Checked)                               { textConverter.LogToBusBuddyRec(); }

            vT.Finish = true;
            textConverter.CheckStatus(vT);
            TextConverterUpdateGUI(vT);
        }

        private void RtbTxtInput_TextChanged(object sender, EventArgs e)
        {
            List<string> txtInput = new List<string>();
            ValueTransfer vT = new ValueTransfer();

            //Adds row by row into a List and then copy the List to TextConverter.
            for (int i = 0; i < rtbTxtInput.Lines.Length; i++)
            {
                txtInput.Add(rtbTxtInput.Lines[i]);
            }
            vT.TxtInput = txtInput;
        }

        private void TextConverterUpdateGUI(ValueTransfer vT)
        {
            CheckTextConverterError(vT);

            string newLine = "\n";
            List<string> outputListData = new List<string>();

            outputListData = vT.OutputListData;

            foreach (string str in outputListData)
            {
                if (rtbTxtOutput.TextLength <= 0)
                { rtbTxtOutput.Text = str; }
                else
                { rtbTxtOutput.AppendText(newLine + str); }
            }
            vT.Finish = false;
        }

        /// <summary>
        /// Dynamic error information builder. The information this popup window are based on what error/error's that might occur. 
        /// After each error message has been deployd into the stringbuilder it sets the error status back to false.
        /// </summary>
        /// <param name="vT"></param>
        private void CheckTextConverterError(ValueTransfer vT)
        {
            StringBuilder sbErrorInfo = new StringBuilder();
            bool showMessage = false;
            string findAndRearrangeMsg1Error = "Object: Find and re-arrange messages between...\n" + "\"" + tbFindAndArrMsg1.Text + "\"" + " could not be found in the Input.\n\n";
            string findAndRearrangeMsg2Error = "Object: Find and re-arrange messages between...\n" + "\"" + tbFindAndArrMsg2.Text + "\"" + " could not be found in the Input.\n\n";
            string removeReqAnsCharCountError = "Object: Find and re-arrange messages between...\nThere must be at least 8 character in the \"Request\" input.\n\n";
            string removeReqAnsError = "Object: Remove request and associated answer\n" + "\"" + tbRemoveReqAns.Text + "\"" + " could not be found in the Input.\n\n";
            string removeReqAnsOddNumber = "Object: Remove request and associated answer\nThere can't be uneven number of characters.\n\n";
            //string findMsgError = "Object: Find message...\n\n" + "\"" + tbFindMsg.Text + "\"" + " could not be found in the Input.\n\n";
            //string underLineError = "Object: Convert underline to whitespace\n\"No underline could be found\"";
            //string whiteSpaceError = "Object: Convert whitespace to underline\n\"No underline could be found\"";
            //string confirmIdError = "Object: Remove answers ==> confirmation ID:\n\"Confirmation ID could be found\"";
            string msgInfo = "Please check so the case sensitive checkbox is not faulty checked/unchecked.";

            if (vT.FindAndRearrangeMsg1Error)   { sbErrorInfo.Append(findAndRearrangeMsg1Error); vT.FindAndRearrangeMsg1Error = false; lblx1.Visible = true; showMessage = true; }
            if (vT.FindAndRearrangeMsg2Error)   { sbErrorInfo.Append(findAndRearrangeMsg2Error); vT.FindAndRearrangeMsg2Error = false; lblx2.Visible = true; showMessage = true; }
            if (vT.RemoveReqAnsCharCountError)  { sbErrorInfo.Append(removeReqAnsCharCountError); vT.RemoveReqAnsCharCountError = false; lblx3.Visible = true; showMessage = true; }
            if (vT.RemoveReqAnsError)           { sbErrorInfo.Append(removeReqAnsError); vT.RemoveReqAnsError = false; lblx3.Visible = true; showMessage = true; }
            if (vT.RemoveReqAnsOddNumber)       { sbErrorInfo.Append(removeReqAnsOddNumber); vT.RemoveReqAnsOddNumber = false; lblx3.Visible = true; showMessage = true; }
            //if (vT.FindMsgError)              { sbErrorInfo.Append(findMsgError); vT.FindMsgError = false; lblx4.Visible = true; showMessage = true; }
            //if (vT.UnderLineError)            { sbErrorInfo.Append(underLineError); vT.UnderLineError = false; lblx5.Visible = true; showMessage = true; }
            //if (vT.WhiteSpaceError)           { sbErrorInfo.Append(whiteSpaceError); vT.WhiteSpaceError = false; lblx6.Visible = true; showMessage = true; }
            //if (vT.ConfirmIdError)            { sbErrorInfo.Append(confirmIdError); vT.ConfirmIdError = false; lblx7.Visible = true; showMessage = true; }

            sbErrorInfo.Append("\n\n\n" + msgInfo);

            if (showMessage)
            { MessageBox.Show(sbErrorInfo.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            else return;
        }

        /// <summary>
        /// If this checkbox is checked, it will uncheck the other checkboxes.
        /// It will also send the checkbox status (boolean value) to Text Converter class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbLF1_CheckedChanged(object sender, EventArgs e)
        {
            if (cbLF1.Checked)
            {
                cbCR1.Checked = false;
                cbCRLF1.Checked = false;
            }
        }

        /// <summary>
        /// If this checkbox is checked, it will uncheck the other checkboxes.
        /// It will also send the checkbox status (boolean value) to Text Converter class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbCR1_CheckedChanged(object sender, EventArgs e)
        {
            if (cbCR1.Checked)
            {
                cbLF1.Checked = false;
                cbCRLF1.Checked = false;
            }
        }

        /// <summary>
        /// If this checkbox is checked, it will uncheck the other checkboxes.
        /// It will also send the checkbox status (boolean value) to Text Converter class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbCRLF1_CheckedChanged(object sender, EventArgs e)
        {
            if (cbCRLF1.Checked)
            {
                cbCR1.Checked = false;
                cbLF1.Checked = false;
            }
        }

        /// <summary>
        /// If this checkbox is checked, it will uncheck the other checkboxes.
        /// It will also send the checkbox status (boolean value) to Text Converter class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbLF2_CheckedChanged(object sender, EventArgs e)
        {
            if (cbLF2.Checked)
            {
                cbCR2.Checked = false;
                cbCRLF2.Checked = false;
            }
        }
        /// <summary>
        /// If this checkbox is checked, it will uncheck the other checkboxes.
        /// It will also send the checkbox status (boolean value) to Text Converter class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbCR2_CheckedChanged(object sender, EventArgs e)
        {
            if (cbCR2.Checked)
            {
                cbLF2.Checked = false;
                cbCRLF2.Checked = false;
            }
        }

        /// <summary>
        /// If this checkbox is checked, it will uncheck the other checkboxes.
        /// It will also send the checkbox status (boolean value) to Text Converter class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbCRLF2_CheckedChanged(object sender, EventArgs e)
        {
            if (cbCRLF2.Checked)
            {
                cbCR2.Checked = false;
                cbLF2.Checked = false;
            }
        }

        /// <summary>
        /// Saves the non-case sensitive checkbutton status so it will return to the same status as when program was shutdown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbNotCaseSensitive_CheckedChanged(object sender, EventArgs e)
        {
            if (cbNotCaseSensitive.Checked)
            {
                Properties.Settings.Default.nonCaseSensitive = cbNotCaseSensitive.Checked = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.nonCaseSensitive = cbNotCaseSensitive.Checked = false;
                Properties.Settings.Default.Save();
            }
        }

        private void CbExcelFormat_CheckedChanged(object sender, EventArgs e)
        {
            if (cbExcelFormat.Checked)
            {
                Properties.Settings.Default.Excelformat = cbExcelFormat.Checked = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.Excelformat = cbExcelFormat.Checked = false;
                Properties.Settings.Default.Save();
            }
        }
    }
}