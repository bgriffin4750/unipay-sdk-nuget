namespace UniPaySdkWrapper
{
    public class CardInfo
    {
        public string Cipher { get; set; }
        public string STX { get; set; }
        public string DataLengthLowbyte { get; set; }
        public string DataLengthHighbyte { get; set; }
        public int DataLength { get; set; }
        public string EncodeType { get; set; }
        public string TrackStatus { get; set; }
        public string T1DataLength { get; set; }
        public string T2DataLength { get; set; }
        public int T1Length { get; set; }
        public int T2Length { get; set; }
        public string FieldByte1 { get; set; }
        public string FieldByte2 { get; set; }
        public string Track1MaskedHex { get; set; }
        public string Track1MaskedString { get; set; }
        public string Track1Encrypted { get; set; }
        public string Serial { get; set; }
        public string KSN { get; set; }
        public string RawData { get; set; }
        public string Last4 { get; set; }
        public string First4 { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public CardType CardType { get; set; }
        public CardReaderResponse CardReaderResponse { get; set; }

        public bool IsValid => !string.IsNullOrEmpty(Last4) &&
                               !string.IsNullOrEmpty(Track1MaskedHex);
    }
}