using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics;
using System.Collections;
using System.Windows.Forms;

namespace Converter
{
    class HEXConverter
    {
        private readonly ValueTransfer vT = new ValueTransfer();
        public bool conversionStatus;
        public bool oneByte, twoBytes, threeBytes, fourBytes, fiveBytes, sixBytes, sevenBytes, eightBytes;
        public int startPos;
        public int xLength;

        List<string> outputListASCII, outputListHEX, outputListDEC, outputListBIN;

        public HEXConverter()
        {
            outputListASCII = new List<string>();
            outputListHEX = new List<string>();
            outputListDEC = new List<string>();
            outputListBIN = new List<string>();
        }

        /// <summary>
        /// Make the conversions and adds it to the list
        /// Conversion;
        /// HEX to ASCII
        /// HEX to DECIMAL
        /// HEX to BINARY
        /// </summary>
        /// <param name="vT"></param>
        public void ConvertFromHex(ValueTransfer vT)
        {
            string space = " ";
            int numOfChar = 0;

            //Converts HEX value to ASCII value
            StringBuilder builderAscii = new StringBuilder();

            foreach (string str in vT.Hex)
            {
                builderAscii.Clear();
                string[] split = new string[(str.Length / 2) + (str.Length % 2 == 0 ? 0 : 1)];

                for (int i = 0; i < split.Length; i++)
                {
                    split[i] = str.Substring(i * 2, i * 2 + 2 > str.Length ? 1 : 2);

                    if (Int64.TryParse(split[i], System.Globalization.NumberStyles.HexNumber,
                        System.Globalization.CultureInfo.InvariantCulture, out long parseNumber))
                    {
                        int min = 20;
                        int max = 255;

                        if (parseNumber >= min && parseNumber <= max)
                        {
                            int length = split[i].Length;
                            byte[] bytes = new byte[length / 2];

                            for (int y = 0; y < length; y += 2)
                            {
                                bytes[y / 2] = Convert.ToByte(split[i].Substring(y, 2), 16);
                            }
                            char[] chars = Encoding.GetEncoding(850).GetChars(bytes);   //Code Page: 850 - OEM Multilingual Latin 1; Western European (DOS) 
                                                                                        //https://docs.microsoft.com/en-us/windows/win32/intl/code-page-identifiers
                            builderAscii.Append(chars);
                            conversionStatus = true;
                        }
                        else
                        {
                            conversionStatus = false;
                        }
                    }
                    else
                    {
                        conversionStatus = false;
                    }
                }
                outputListASCII.Add(builderAscii.ToString());
            }

            //Converts HEX value to DEC value
            StringBuilder builderDec = new StringBuilder();
            StringBuilder builderX = new StringBuilder();

            foreach (string str in vT.Hex)
            {
                //Converts number of bytes to number of characters.
                //This way we only need one dynamic calculation instead of several calculations.
                if (oneByte) numOfChar = 2;
                if (twoBytes) numOfChar = 4;
                if (threeBytes) numOfChar = 6;
                if (fourBytes) numOfChar = 8;
                if (fiveBytes) numOfChar = 10;
                if (sixBytes) numOfChar = 12;
                if (sevenBytes) numOfChar = 14;
                if (eightBytes) numOfChar = 16;

                builderDec.Clear();
                if (str.Length >= numOfChar)
                {
                    string[] split = new string[str.Length / numOfChar + (str.Length % numOfChar == 0 ? 0 : 1)];

                    for (int i = 0; i < split.Length; i++)
                    {
                        int strInputMod = str.Length;

                        //For each index, this loop removes specified number of characters (numOfChar) from the input string.
                        //This is needed to count the number of characters at the last index.
                        for (int z = 0; z < split.Length - 1; z++)
                        {
                            strInputMod = strInputMod - numOfChar;
                        }

                        //This is a counter to know the start position of the first 'x' if there are any in the ouput string (it will later be colored RED).
                        //These values sends to MainForm.
                        startPos = str.Length - strInputMod + 1;
                        xLength = strInputMod;

                        split[i] = str.Substring(i * numOfChar, (i * numOfChar) + numOfChar > str.Length ? strInputMod : numOfChar);

                        int arrayLength = split.Length; //Gets the number of indexes.

                        //Checks if the last index is not "null" and that the first index length is greater than the last index.
                        if ((split[arrayLength - 1] != null) && (split[0].Length > split[arrayLength - 1].Length))
                        {
                            int firstIndexLength = split[0].Length; //Gets the length of the first index.
                            int lastIndexLength = split[arrayLength - 1].Length; //Gets the length of the last index.

                            //Compares the length of the element in the first index with the last index.
                            //If the length of the element in the last index is less than the first index,
                            //it will set these leftovers to 'x'. Just to show the user the these characters could not
                            //be converted due to the users selection of number of selected bytes.
                            if (firstIndexLength > lastIndexLength)
                            {
                                for (int x = 0; x < lastIndexLength; x++)
                                {
                                    builderDec.Append('x');
                                }
                            }
                        }
                        else
                        {
                            long decVal = Convert.ToInt64(split[i], 16);
                            builderDec.Append(decVal + space).ToString();
                        }
                    }
                }
                //If the string length is less than the selected bytes, it will set these to 'x'.
                else //(str.Length < numOfChar)
                {
                    for (int j = 0; j < str.Length; j++)
                    {
                        builderDec.Append('x');
                    }
                }
                outputListDEC.Add(builderDec.ToString().TrimEnd() + builderX.ToString().TrimEnd());
            }
            oneByte = false; twoBytes = false; threeBytes = false; fourBytes = false; fiveBytes = false; sixBytes = false; sevenBytes = false; eightBytes = false;

            //Converts HEX value to BINARY value
            StringBuilder builderBin = new StringBuilder();
            Dictionary<char, string> hexCharToBin = new Dictionary<char, string>
            {
                { '0', "0000" }, { '1', "0001" }, { '2', "0010" }, { '3', "0011" },
                { '4', "0100" }, { '5', "0101" }, { '6', "0110" }, { '7', "0111" },
                { '8', "1000" }, { '9', "1001" }, { 'A', "1010" }, { 'B', "1011" },
                { 'C', "1100" }, { 'D', "1101" }, { 'E', "1110" }, { 'F', "1111" }
            };
            foreach (string str in vT.Hex)
            {
                builderBin.Clear();
                foreach (char c in str.ToUpper())
                {
                    builderBin.Append(hexCharToBin[char.ToUpper(c)] + space);
                }
                outputListBIN.Add(builderBin.ToString().TrimEnd());
            }
        }

        /// Make the conversions and adds it to the list
        /// Conversion;
        /// ASCII to HEX
        /// ASCII to DECIMAL
        /// ASCII to BINARY
        /// </summary>
        /// <param name="vT"></param>
        public void ConvertFromAscii(ValueTransfer vT)
        {
            string space = " ";

            //Converts ASCII value to HEX value
            StringBuilder builderHex = new StringBuilder();
            for (int i = 0; i < vT.Ascii.Count; i++)
            {
                string format = "{0:X2} ";
                builderHex.Clear();
                foreach (char c in vT.Ascii[i])
                {
                    builderHex.AppendFormat(format, (int)c);
                }
                outputListHEX.Add(builderHex.ToString().TrimEnd());
            }
            //Converts ASCII value to DEC value
            StringBuilder builderDec = new StringBuilder();
            for (int i = 0; i < vT.Ascii.Count; i++)
            {
                builderDec.Clear();
                foreach (char c in vT.Ascii[i])
                {
                    string format = " ";
                    long dec = Convert.ToInt64(c);
                    builderDec.AppendFormat(dec.ToString() + format);
                }
                outputListDEC.Add(builderDec.ToString().TrimEnd());
            }

            //Converts ASCII to BIN
            StringBuilder builderBin = new StringBuilder();
            StringBuilder tempBuilder = new StringBuilder();
            List<string> tempList = new List<string>();
            for (int i = 0; i < vT.Ascii.Count; i++)
            {
                //Converts the ASCII string into HEX before converting it to BIN
                string format = "{0:X2}";
                foreach (char c in vT.Ascii[i])
                {
                    tempBuilder.AppendFormat(format, (int)c);
                }
                tempList.Add(tempBuilder.ToString());
                tempBuilder.Clear();
            }
            Dictionary<char, string> hexCharToBin = new Dictionary<char, string>
            {
                { '0', "0000" }, { '1', "0001" }, { '2', "0010" }, { '3', "0011" },
                { '4', "0100" }, { '5', "0101" }, { '6', "0110" }, { '7', "0111" },
                { '8', "1000" }, { '9', "1001" }, { 'A', "1010" }, { 'B', "1011" },
                { 'C', "1100" }, { 'D', "1101" }, { 'E', "1110" }, { 'F', "1111" }
            };
            foreach (string str in tempList)
            {
                builderBin.Clear();
                foreach (char c in str.ToUpper())
                {
                    builderBin.Append(hexCharToBin[char.ToUpper(c)] + space);
                }
                outputListBIN.Add(builderBin.ToString().TrimEnd());
            }
        }

        /// <summary>
        /// Make the conversions and adds it to the list
        /// Conversion;
        /// DEC to HEX
        /// DEC to ASCII
        /// DEC to BINARY
        /// </summary>
        /// <param name="vT"></param>
        public void ConvertFromDec(ValueTransfer vT)
        {
            string spaceChar = " ";
            StringBuilder builderHex = new StringBuilder();
            StringBuilder builderAscii = new StringBuilder();
            StringBuilder builderBin = new StringBuilder();
            List<string> tempList = new List<string>();

            //Converts DEC value to HEX value
            foreach (string str in vT.Dec)
            {
                builderHex.Clear();

                if (!string.IsNullOrEmpty(str))
                {
                    if (str.Contains(spaceChar))
                    {
                        StringBuilder tempStringBuild = new StringBuilder();

                        string[] splitString = str.Split(new char[0]);

                        foreach (string newString in splitString)
                        {
                            long stringToInt = Convert.ToInt64(newString);
                            string intToHex = stringToInt.ToString("X");

                            tempStringBuild.Append(intToHex.PadLeft(2, '0') + spaceChar);
                        }
                        tempList.Add(tempStringBuild.ToString()); //Saves the HEX-value for later use in DEC to BIN conversion.
                        builderHex.Append(tempStringBuild);
                    }
                    else
                    {
                        long stringToInt = Convert.ToInt64(str);
                        string intToHex = stringToInt.ToString("X");
                        builderHex.Append(intToHex.PadLeft(2, '0'));
                        tempList.Add(builderHex.ToString());
                    }
                }
                else
                {
                    builderHex.Append("");
                    tempList.Add(builderHex.ToString());
                }
                outputListHEX.Add(builderHex.ToString().TrimEnd());
            }

            //Converts DEC value to ASCII value
            foreach (string str in vT.Dec)
            {
                string[] decSplit = str.Split(new char[0]); //Split the string at every whitespace and sets each value into an array.
                builderAscii.Clear();

                if (!string.IsNullOrEmpty(str))
                {
                    for (long i = 0; i < decSplit.Length; i++)
                    {
                        long decValue = long.Parse(decSplit[i]);
                        string hexValue = decValue.ToString("X");

                        if (Int64.TryParse(hexValue, System.Globalization.NumberStyles.HexNumber,
                            System.Globalization.CultureInfo.InvariantCulture, out long parseNumber))
                        {
                            int min = 20;
                            int max = 255;

                            if (parseNumber >= min && parseNumber <= max)
                            {
                                int length = hexValue.Length;
                                byte[] bytes = new byte[length / 2];

                                for (int y = 0; y < length; y += 2)
                                {
                                    bytes[y / 2] = Convert.ToByte(hexValue.Substring(y, 2), 16);
                                }
                                char[] chars = Encoding.GetEncoding(850).GetChars(bytes);   //Code Page: 850 - OEM Multilingual Latin 1; Western European (DOS)
                                                                                            //https://docs.microsoft.com/en-us/windows/win32/intl/code-page-identifiers
                                builderAscii.Append(chars);
                                conversionStatus = true;
                            }
                            else
                            {
                                conversionStatus = false;
                            }
                        }
                    }
                }
                else
                {
                    builderAscii.Append("");
                }
                outputListASCII.Add(builderAscii.ToString());
            }

            //Converts DEC to BIN value
            //HEX to BIN dicionary cause it gets the already converted HEX value from "DEC to HEX" conversion
            Dictionary<char, string> hexCharToBin = new Dictionary<char, string>
            {
                { '0', "0000" }, { '1', "0001" }, { '2', "0010" }, { '3', "0011" },
                { '4', "0100" }, { '5', "0101" }, { '6', "0110" }, { '7', "0111" },
                { '8', "1000" }, { '9', "1001" }, { 'A', "1010" }, { 'B', "1011" },
                { 'C', "1100" }, { 'D', "1101" }, { 'E', "1110" }, { 'F', "1111" }
            };
            //Removes all the whitespaces in the list before entering the conversion
            List<string> modList = new List<string>();
            modList = tempList.Select(x => x.Replace(" ", "")).ToList();
            tempList = modList;
            foreach (string str in tempList) //tempList value comes from the DEC to HEX conversion
            {
                builderBin.Clear();
                if (!string.IsNullOrEmpty(str))
                {
                    foreach (char c in str.ToUpper())
                    {
                        builderBin.Append(hexCharToBin[char.ToUpper(c)] + spaceChar);
                    }
                }
                else
                {
                    builderBin.Append("");
                }
                outputListBIN.Add(builderBin.ToString().TrimEnd());
            }
        }

        /// <summary>
        /// Make the conversions and adds it to the list
        /// Conversion;
        /// BINARY to HEX
        /// BINARY to ASCII
        /// BINARY to DEC
        /// </summary>
        /// <param name="vT"></param>
        public void ConvertFromBin(ValueTransfer vT)
        {
            //Converts BIN value to HEX value
            StringBuilder builderHex = new StringBuilder();
            StringBuilder sbTemp = new StringBuilder();
            List<string> hexList = new List<string>();

            Dictionary<string, char> hexCharToBin = new Dictionary<string, char>
            {
                { "0000",'0'  }, { "0001", '1' }, { "0010", '2' }, { "0011", '3' },
                { "0100",'4'  }, { "0101", '5' }, { "0110", '6' }, { "0111", '7' },
                { "1000",'8'  }, { "1001", '9' }, { "1010", 'A' }, { "1011", 'B' },
                { "1100",'C'  }, { "1101", 'D' }, { "1110", 'E' }, { "1111", 'F' }
            };

            foreach (string str in vT.Bin)
            {
                builderHex.Clear();
                hexList.Clear();
                int strCharCount = str.Count();
                string tempValue = "";

                string[] split = new string[str.Length / 4 + (str.Length % 4 == 0 ? 0 : 1)];

                for (int i = 0; i < split.Length; i++)
                {
                    split[i] = str.Substring(i * 4, i * 4 + 4 > str.Length ? i : 4);
                    hexList.Add(hexCharToBin[split[i]].ToString());
                }

                for (int i = 0; i < 1; i++)
                {
                    foreach (var value in hexList)
                    {
                        sbTemp.Append(value[i].ToString());
                        tempValue = value[i].ToString();
                    }
                }

                //Adds a '0' (zero) before last character in the HEX conversion result,
                //if there is odd nibbles (not full byte of 8 bits), e.g. 1111 0000 1111 (F0 F) -> 1111 0000 0000 1111 (F0 0F).
                int modLen = sbTemp.Length % 2;
                if (modLen != 0 && tempValue != "")
                {
                    int sbLen = sbTemp.Length;
                    sbTemp.Insert(sbLen - 1, '0');
                }

                outputListHEX.Add(sbTemp.ToString().TrimEnd());
                sbTemp.Clear();
            }

            //Converts BIN (via HEX) value to ASCII value
            StringBuilder builderAscii = new StringBuilder();
            foreach (string str in outputListHEX)
            {
                builderAscii.Clear();

                string[] split = new string[(str.Length / 2) + (str.Length % 2 == 0 ? 0 : 1)];

                for (int i = 0; i < split.Length; i++)
                {
                    split[i] = str.Substring(i * 2, i * 2 + 2 > str.Length ? 1 : 2);

                    if (Int64.TryParse(split[i], System.Globalization.NumberStyles.HexNumber,
                        System.Globalization.CultureInfo.InvariantCulture, out long parseNumber))
                    {
                        int min = 20;
                        int max = 255;

                        if (parseNumber >= min && parseNumber <= max)
                        {
                            int length = split[i].Length;
                            byte[] bytes = new byte[length / 2];

                            for (int y = 0; y < length; y += 2)
                            {
                                bytes[y / 2] = Convert.ToByte(split[i].Substring(y, 2), 16);
                            }
                            char[] chars = Encoding.GetEncoding(850).GetChars(bytes);   //Code Page: 850 - OEM Multilingual Latin 1; Western European (DOS) 
                                                                                        //https://docs.microsoft.com/en-us/windows/win32/intl/code-page-identifiers
                            builderAscii.Append(chars);
                            conversionStatus = true;
                        }
                        else
                        {
                            conversionStatus = false;
                        }
                    }
                    else
                    {
                        conversionStatus = false;
                    }
                }
                outputListASCII.Add(builderAscii.ToString());
            }

            //Converts BIN (via HEX) value to DEC value
            StringBuilder builderDec = new StringBuilder();
            foreach (string str in outputListHEX)
            {
                builderDec.Clear();
                string[] split = new string[str.Length / 2 + (str.Length % 2 == 0 ? 0 : 1)];

                for (int i = 0; i < split.Length; i++)
                {
                    split[i] = str.Substring(i * 2, i * 2 + 2 > str.Length ? 1 : 2);
                    long decVal = Convert.ToInt64(split[i], 16);
                    builderDec.Append(decVal + " ").ToString();
                }
                outputListDEC.Add(builderDec.ToString().TrimEnd());
            }
        }

        /// <summary>
        /// Creates a copy of ASCII list and allows the MainForm get the values. 
        /// </summary>
        /// <returns></returns>
        public List<string> GetValueConvertedToAscii()
        {
            List<string> copyAsciiString = new List<string>();
            copyAsciiString.AddRange(outputListASCII);
            return copyAsciiString;
        }

        /// <summary>
        /// Creates a copy of HEX list and allows the MainForm get the values.
        /// </summary>
        /// <returns></returns>
        public List<string> GetValueConvertedToHex()
        {
            List<string> copyHexString = new List<string>();
            copyHexString.AddRange(outputListHEX);
            return copyHexString;
        }

        /// <summary>
        /// Creates a copy of DEC list and allows the MainForm get the values.
        /// </summary>
        /// <returns></returns>
        public List<string> GetValueConvertedToDec()
        {
            List<string> copyDecString = new List<string>();
            copyDecString.AddRange(outputListDEC);
            return copyDecString;
        }

        /// <summary>
        /// Creates a copy of BIN list and allows the MainForm get the values.
        /// </summary>
        /// <returns></returns>
        public List<string> GetValueConvertedToBin()
        {
            List<string> copyBinString = new List<string>();
            copyBinString.AddRange(outputListBIN);
            return copyBinString;
        }

        /// <summary>
        /// Clears all the list.
        /// </summary>
        public void ClearAllLists()
        {
            outputListASCII.Clear();
            outputListHEX.Clear();
            outputListDEC.Clear();
            outputListBIN.Clear();
        }
    }
}
