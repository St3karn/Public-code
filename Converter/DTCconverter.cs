using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Converter
{
    class DTCconverter
    {
        private readonly ValueTransfer converter = new ValueTransfer();
        List<string> outputList = new List<string>();
        public bool invalidDTC;
        public bool rbtnDtcHexToDtc;
        public bool rbtnDtcDtcToHex;

        public void ConvertInput(ValueTransfer converter)
        {
            string firstCharStr;
            string modStr;
            outputList.Clear();

            List<string> dtcSplitString = new List<string>();
            StringBuilder dtcFirstCharBuilder = new StringBuilder();
            StringBuilder builder = new StringBuilder();

            Dictionary<char, string> hexToDtcMap = new Dictionary<char, string>
            {
                { '0', "P0" }, { '4', "C0" }, { '8', "B0" }, { 'C', "U0" },
                { '1', "P1" }, { '5', "C1" }, { '9', "B1" }, { 'D', "U1" },
                { '2', "P2" }, { '6', "C2" }, { 'A', "B2" }, { 'E', "U2" },
                { '3', "P3" }, { '7', "C3" }, { 'B', "B3" }, { 'F', "U3" }
            };

            Dictionary<string, char> dtcToHexMap = new Dictionary<string, char>
            {
                { "P0", '0' }, { "C0", '4' }, { "B0", '8' }, { "U0", 'C' },
                { "P1", '1' }, { "C1", '5' }, { "B1", '9' }, { "U1", 'D' },
                { "P2", '2' }, { "C2", '6' }, { "B2", 'A' }, { "U2", 'E' },
                { "P3", '3' }, { "C3", '7' }, { "B3", 'B' }, { "U3", 'F' }
            };

            if (rbtnDtcHexToDtc == true && converter.DtcInput.Count != 0)
            {
                foreach (string str in converter.DtcInput)
                {
                    invalidDTC = false; //Resets the status of "invalidDTC" since each "str" is a new DTC.

                    if (!string.IsNullOrEmpty(str))
                    {
                        char firstChar;
                        modStr = str.ToString().Trim().Replace(" ", "").ToUpper();
                        dtcSplitString.Clear();

                        foreach (char c in modStr)
                        {
                            dtcSplitString.Add(c.ToString()); //Splits the DTC-code so one element contains one character 
                        }

                        builder.Clear();
                        firstChar = Convert.ToChar(dtcSplitString[0]);   //Adds the first character
                        dtcSplitString.RemoveAt(0);

                        if (!hexToDtcMap.ContainsKey(firstChar))
                        {
                            invalidDTC = true;
                        }
                        else
                        {
                            firstCharStr = hexToDtcMap[char.ToUpper(firstChar)];                                        //Dictionary HEX to DTC lookup.
                            builder.Append(firstCharStr);
                            dtcSplitString.ForEach(s => builder.Append(s));
                            outputList.Add(builder.ToString());
                        }
                    }
                    else
                    {
                        outputList.Add("");
                    }
                }
            }
            else if (rbtnDtcDtcToHex == true && converter.DtcInput.Count != 0)
            {
                foreach (string str in converter.DtcInput)
                {
                    invalidDTC = false; //Resets the status of "invalidDTC" since each "str" is a new DTC.

                    if (!string.IsNullOrEmpty(str))
                    {
                        string firstTwoChar;
                        modStr = str.ToString().Trim().Replace(" ", "").ToUpper();
                        dtcSplitString.Clear();
                        builder.Clear();

                        foreach (char c in modStr)
                        {
                            dtcSplitString.Add(c.ToString()); //Splits the DTC-code so one element contains one character 
                        }

                        builder.Clear();

                        firstTwoChar = dtcSplitString[0] + dtcSplitString[1];    //Adds the first two characters.
                        dtcSplitString.RemoveAt(0);
                        dtcSplitString.RemoveAt(0);

                        if (!dtcToHexMap.ContainsKey(firstTwoChar))
                        {
                            MessageBox.Show("Invalid DTC !\n\nPlease check the input.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                        else
                        {
                            firstCharStr = dtcToHexMap[firstTwoChar].ToString();    //Dictionary DTC to HEX lookup.
                            builder.Append(firstCharStr);
                            dtcSplitString.ForEach(s => builder.Append(s));
                        }
                        outputList.Add(builder.ToString());
                    }
                    else
                    {
                        outputList.Add("");
                    }
                }
            }
        }

        /// <summary>
        /// Creates a copy of DTC list and allows the MainForm get the values.
        /// </summary>
        /// <returns></returns>
        public List<string> GetConvertedDtc()
        {
            List<string> copyDtcList = new List<string>();
            copyDtcList.AddRange(outputList);
            return copyDtcList;
        }
    }
}
