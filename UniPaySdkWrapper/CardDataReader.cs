using System;
using System.Text;
using System.Text.RegularExpressions;

namespace UniPaySdkWrapper
{
    public class CardDataReader
    {
        private static readonly CardTypeInfo[] CardTypeInfo =
        {
            new CardTypeInfo("^(51|52|53|54|55)", 4, CardType.MasterCard),
            new CardTypeInfo("^(4)", 4, CardType.VISA),
            new CardTypeInfo("^(4)", 4, CardType.VISA),
            new CardTypeInfo("^(34|37)", 4, CardType.Amex),
            new CardTypeInfo("^(6011)", 4, CardType.Discover),
            new CardTypeInfo("^(300|301|302|303|304|305|36|38)", 4, CardType.DinersClub),
            new CardTypeInfo("^(3)", 4, CardType.JCB),
            new CardTypeInfo("^(2131|1800)", 4, CardType.JCB),
            new CardTypeInfo("^(2014|2149)", 4, CardType.enRoute)
        };

        public static CardInfo GetCardInfo(byte[] data)
        {
            var hex = BitConverter.ToString(data).Replace("-", "");
            return Parse(hex);
        }

        private static CardInfo Parse(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;

            var ret = GetCardInfo(input);

            ret.Track1MaskedHex = GetTrack1MaskedHex(input);

            // create ascii string from track1 hex string
            ret.Track1MaskedString = HexToAscii(ret.Track1MaskedHex);

            var track1Mask = ret.Track1MaskedString;

            var firstCaretInx = track1Mask.IndexOf("^", StringComparison.Ordinal);
            var lastCaretInx = track1Mask.LastIndexOf("^", StringComparison.Ordinal);

            SetName(ret, firstCaretInx, lastCaretInx, track1Mask);

            ret.Last4 = GetLast4(firstCaretInx, track1Mask);

            ret.CardType = GetCardType(track1Mask, ret);

            return ret;
        }

        private static CardInfo GetCardInfo(string input)
        {
            var cInfo = new CardInfo {Cipher = input};

            input = GetRawData(input);
            cInfo.RawData = input;

            cInfo.STX = input.Substring(0, 2);
            cInfo.DataLengthLowbyte = input.Substring(2, 2);
            cInfo.DataLengthHighbyte = input.Substring(4, 2);
            cInfo.DataLength = Convert.ToInt32((cInfo.DataLengthHighbyte + cInfo.DataLengthLowbyte), 16);

            cInfo.EncodeType = input.Substring(6, 2);
            cInfo.TrackStatus = input.Substring(8, 2);
            cInfo.T1DataLength = input.Substring(10, 2);
            cInfo.T2DataLength = input.Substring(12, 2);
            cInfo.T1Length = Convert.ToInt32(cInfo.T1DataLength, 16);
            cInfo.T2Length = Convert.ToInt32(cInfo.T2DataLength, 16);

            cInfo.FieldByte1 = input.Substring(16, 2);
            cInfo.FieldByte2 = input.Substring(18, 2);

            return cInfo;
        }

        private static string GetTrack1MaskedHex(string input)
        {
            var currentPosition = 20;
            var trackMaskHex = string.Empty;

            while (true)
            {
                var t1MaskedTempStr = input.Substring(currentPosition, 2);

                if (currentPosition == 20)
                {
                    // value at position 20 should be '25' which represents the starting sentinel
                    if (string.Compare(t1MaskedTempStr, "25", StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        break;
                    }
                }

                // "3F" is the ending sentinel of the data we can access
                if (string.CompareOrdinal(t1MaskedTempStr, "3F") == 0)
                {
                    // end of track1 data detected
                    trackMaskHex += t1MaskedTempStr;
                    currentPosition += 2;

                    // read the next character
                    t1MaskedTempStr = input.Substring(currentPosition, 2);
                    trackMaskHex += t1MaskedTempStr;
                    break;
                }

                trackMaskHex += t1MaskedTempStr;
                currentPosition += 2;
            }

            return trackMaskHex;
        }

        private static void SetName(CardInfo cInfo, int firstCaretInx, int lastCaretInx, string trackMask)
        {
            if (firstCaretInx > -1 && lastCaretInx > -1 && firstCaretInx != lastCaretInx)
            {
                var nameLength = lastCaretInx - firstCaretInx - 1;

                var name = trackMask.Substring(firstCaretInx + 1, nameLength);
                var nameParts = name.Split('/');

                cInfo.LastName = nameParts[0].Trim();
                if (nameParts.Length > 1)
                {
                    cInfo.FirstName = nameParts[1].Trim();
                }
                cInfo.FullName = $"{cInfo.FirstName} {cInfo.LastName}";
            }
        }

        private static string GetLast4(int firstCaretInx, string trackMask)
        {
            //pull the last 4 from track1 even if we are not passing track1
            //track1 should always be formatted properly,but let's try it anyway
            if (firstCaretInx > -1 && firstCaretInx >= 3)
            {
                return trackMask.Substring(firstCaretInx - 4, 4);
            }

            return null;
        }

        private static string GetRawData(string input)
        {
            var hex = input;
            //remove Unipay specific header
            hex = hex.Substring(2);
            char[] zero = {'0'};
            //remove trailing nulls
            hex = hex.TrimEnd(zero);
            return hex;
        }

        private static CardType GetCardType(string trackMask, CardInfo cInfo)
        {
            var asterixInx = trackMask.IndexOf("*", StringComparison.Ordinal);
            var track1Length = trackMask.Length;

            string first4 = null;

            if (asterixInx > -1 && asterixInx + 5 < track1Length)
            {
                first4 = trackMask.Substring(asterixInx + 1, 4);
            }

            if (string.IsNullOrEmpty(first4)) return CardType.Unknown;

            cInfo.First4 = first4;

            foreach (var info in CardTypeInfo)
            {
                if (first4.Length == info.Length && Regex.IsMatch(first4, info.RegEx))
                    return info.Type;
            }

            return CardType.Unknown;
        }

        private static string HexToAscii(string input)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < input.Length; i += 2)
            {
                var hs = input.Substring(i, 2);
                sb.Append(Convert.ToChar(Convert.ToUInt32(hs, 16)));
            }

            return sb.ToString();
        }
    }
}