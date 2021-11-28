﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
    class CodePageList
    {
        public Dictionary<string, int> codePageDict = new Dictionary<string, int>
        {
            { "IBM EBCDIC US-Canada",37  },
            { "OEM United States",437  },
            { "IBM EBCDIC International",500  },
            { "Arabic (ASMO 708)",708  },
            { "(ASMO-449+, BCON V4)",709  },
            { "Transparent Arabic",710  },
            { "Arabic (Transparent ASMO): Arabic (DOS)",720  },
            { "OEM Greek (formerly 437G): Greek (DOS)",737  },
            { "OEM Baltic: Baltic (DOS)",775  },
            { "OEM Multilingual Latin 1: Western European (DOS)",850  },
            { "OEM Latin 2: Central European (DOS)",852  },
            { "OEM Cyrillic (primarily Russian)",855  },
            { "OEM Turkish: Turkish (DOS)",857  },
            { "OEM Multilingual Latin 1 + Euro symbol",858  },
            { "OEM Portuguese: Portuguese (DOS)",860  },
            { "OEM Icelandic: Icelandic (DOS)",861  },
            { "OEM Hebrew: Hebrew (DOS)",862  },
            { "OEM French Canadian: French Canadian (DOS)",863  },
            { "OEM Arabic: Arabic (864)",864  },
            { "OEM Nordic: Nordic (DOS)",865  },
            { "OEM Russian: Cyrillic (DOS)",866  },
            { "OEM Modern Greek: Greek, Modern (DOS)",869  },
            { "IBM EBCDIC Multilingual/ROECE (Latin 2): IBM EBCDIC Multilingual Latin 2",870  },
            { "ANSI/OEM Thai (ISO 8859-11): Thai (Windows)",874  },
            { "IBM EBCDIC Greek Modern",875  },
            { "ANSI/OEM Japanese: Japanese (Shift-JIS)",932  },
            { "ANSI/OEM Simplified Chinese (PRC, Singapore): Chinese Simplified (GB2312)",936  },
            { "ANSI/OEM Korean (Unified Hangul Code)",949  },
            { "ANSI/OEM Traditional Chinese (Taiwan: Hong Kong SAR, PRC): Chinese Traditional (Big5)",950  },
            { "IBM EBCDIC Turkish (Latin 5)",1026  },
            { "IBM EBCDIC Latin 1/Open System",1047  },
            { "IBM EBCDIC US-Canada (037 + Euro symbol): IBM EBCDIC (US-Canada-Euro)",1140  },
            { "IBM EBCDIC Germany (20273 + Euro symbol): IBM EBCDIC (Germany-Euro)",1141  },
            { "IBM EBCDIC Denmark-Norway (20277 + Euro symbol): IBM EBCDIC (Denmark-Norway-Euro)",1142  },
            { "IBM EBCDIC Finland-Sweden (20278 + Euro symbol): IBM EBCDIC (Finland-Sweden-Euro)",1143  },
            { "IBM EBCDIC Italy (20280 + Euro symbol): IBM EBCDIC (Italy-Euro)",1144  },
            { "IBM EBCDIC Latin America-Spain (20284 + Euro symbol): IBM EBCDIC (Spain-Euro)",1145  },
            { "IBM EBCDIC United Kingdom (20285 + Euro symbol): IBM EBCDIC (UK-Euro)",1146  },
            { "IBM EBCDIC France (20297 + Euro symbol): IBM EBCDIC (France-Euro)",1147  },
            { "IBM EBCDIC International (500 + Euro symbol): IBM EBCDIC (International-Euro)",1148  },
            { "IBM EBCDIC Icelandic (20871 + Euro symbol): IBM EBCDIC (Icelandic-Euro)",1149  },
            { "Unicode UTF-16, little endian byte order (BMP of ISO 10646):",1200  },
            { "Unicode UTF-16, big endian byte order:",1201  },
            { "ANSI Central European: Central European (Windows)",1250  },
            { "ANSI Cyrillic: Cyrillic (Windows)",1251  },
            { "ANSI Latin 1: Western European (Windows)",1252  },
            { "ANSI Greek: Greek (Windows)",1253  },
            { "ANSI Turkish: Turkish (Windows)",1254  },
            { "ANSI Hebrew: Hebrew (Windows)",1255  },
            { "ANSI Arabic: Arabic (Windows)",1256  },
            { "ANSI Baltic: Baltic (Windows)",1257  },
            { "ANSI/OEM Vietnamese: Vietnamese (Windows)",1258  },
            { "Korean (Johab)",1361  },
            { "MAC Roman: Western European (Mac)",10000  },
            { "Japanese (Mac)",10001  },
            { "MAC Traditional Chinese (Big5): Chinese Traditional (Mac)",10002  },
            { "Korean (Mac)",10003  },
            { "Arabic (Mac)",10004  },
            { "Hebrew (Mac)",10005  },
            { "Greek (Mac)",10006  },
            { "Cyrillic (Mac)",10007  },
            { "MAC Simplified Chinese (GB 2312): Chinese Simplified (Mac)",10008  },
            { "Romanian (Mac)",10010  },
            { "Ukrainian (Mac)",10017  },
            { "Thai (Mac)",10021  },
            { "MAC Latin 2: Central European (Mac)",10029  },
            { "Icelandic (Mac)",10079  },
            { "Turkish (Mac)",10081  },
            { "Croatian (Mac)",10082  },
            { "Unicode UTF-32, little endian byte order",12000  },
            { "Unicode UTF-32, big endian byte order:",12001  },
            { "CNS Taiwan: Chinese Traditional (CNS)",20000  },
            { "TCA Taiwan",20001  },
            { "Eten Taiwan: Chinese Traditional (Eten)",20002  },
            { "IBM5550 Taiwan",20003  },
            { "TeleText Taiwan",20004  },
            { "Wang Taiwan",20005  },
            { "IA5 (IRV International Alphabet No. 5, 7-bit): Western European (IA5)",20105  },
            { "IA5 German (7-bit)",20106  },
            { "IA5 Swedish (7-bit)",20107  },
            { "IA5 Norwegian (7-bit)",20108  },
            { "US-ASCII (7-bit)",20127  },
            { "T.61",20261  },
            { "ISO 6937 Non-Spacing Accent",20269  },
            { "IBM EBCDIC Germany",20273  },
            { "IBM EBCDIC Denmark-Norway",20277  },
            { "IBM EBCDIC Finland-Sweden",20278  },
            { "IBM EBCDIC Italy",20280  },
            { "IBM EBCDIC Latin America-Spain",20284  },
            { "IBM EBCDIC United Kingdom",20285  },
            { "IBM EBCDIC Japanese Katakana Extended",20290  },
            { "IBM EBCDIC France",20297  },
            { "IBM EBCDIC Arabic",20420  },
            { "IBM EBCDIC Greek",20423  },
            { "IBM EBCDIC Hebrew",20424  },
            { "IBM EBCDIC Korean Extended",20833  },
            { "IBM EBCDIC Thai",20838  },
            { "Russian (KOI8-R): Cyrillic (KOI8-R)",20866  },
            { "IBM EBCDIC Icelandic",20871  },
            { "IBM EBCDIC Cyrillic Russian",20880  },
            { "IBM EBCDIC Turkish",20905  },
            { "IBM EBCDIC Latin 1/Open System (1047 + Euro symbol)",20924  },
            { "Japanese (JIS 0208-1990 and 0212-1990)",20932  },
            { "Simplified Chinese (GB2312): Chinese Simplified (GB2312-80)",20936  },
            { "Korean Wansung",20949  },
            { "IBM EBCDIC Cyrillic Serbian-Bulgarian",21025  },
            { "Ukrainian (KOI8-U): Cyrillic (KOI8-U)",21866  },
            { "ISO 8859-1 Latin 1: Western European (ISO)",28591  },
            { "ISO 8859-2 Central European: Central European (ISO)",28592  },
            { "ISO 8859-3 Latin 3",28593  },
            { "ISO 8859-4 Baltic",28594  },
            { "ISO 8859-5 Cyrillic",28595  },
            { "ISO 8859-6 Arabic",28596  },
            { "ISO 8859-7 Greek",28597  },
            { "ISO 8859-8 Hebrew: Hebrew (ISO-Visual)",28598  },
            { "ISO 8859-9 Turkish",28599  },
            { "ISO 8859-13 Estonian",28603  },
            { "ISO 8859-15 Latin 9",28605  },
            { "Europa 3",29001  },
            { "ISO 8859-8 Hebrew: Hebrew (ISO-Logical)",38598  },
            { "ISO 2022 Japanese with no halfwidth Katakana: Japanese (JIS)",50220  },
            { "ISO 2022 Japanese with halfwidth Katakana: Japanese (JIS-Allow 1 byte Kana)",50221  },
            { "ISO 2022 Japanese JIS X 0201-1989: Japanese (JIS-Allow 1 byte Kana - SO/SI)",50222  },
            { "ISO 2022 Korean",50225  },
            { "ISO 2022 Simplified Chinese: Chinese Simplified (ISO 2022)",50227  },
            { "ISO 2022 Traditional Chinese",50229  },
            { "EBCDIC Japanese (Katakana) Extended",50930  },
            { "EBCDIC US-Canada and Japanese",50931  },
            { "EBCDIC Korean Extended and Korean",50933  },
            { "EBCDIC Simplified Chinese Extended and Simplified Chinese",50935  },
            { "EBCDIC Simplified Chinese",50936  },
            { "EBCDIC US-Canada and Traditional Chinese",50937  },
            { "EBCDIC Japanese (Latin) Extended and Japanese",50939  },
            { "EUC Japanese",51932  },
            { "EUC Simplified Chinese: Chinese Simplified (EUC)",51936  },
            { "EUC Korean",51949  },
            { "Traditional Chinese",51950  },
            { "HZ-GB2312 Simplified Chinese: Chinese Simplified (HZ)",52936  },
            { "Windows XP and later: GB18030 Simplified Chinese (4 byte): Chinese Simplified (GB18030)",54936  },
            { "ISCII Devanagari",57002  },
            { "ISCII Bangla",57003  },
            { "ISCII Tamil",57004  },
            { "ISCII Telugu",57005  },
            { "ISCII Assamese",57006  },
            { "ISCII Odia",57007  },
            { "ISCII Kannada",57008  },
            { "ISCII Malayalam",57009  },
            { "ISCII Gujarati",57010  },
            { "ISCII Punjabi",57011  },
            { "Unicode (UTF-7)",65000  },
            { "Unicode (UTF-8)",65001  },
        };
    }
}
