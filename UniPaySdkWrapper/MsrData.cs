namespace UniPaySdkWrapper
{
    internal class MsrData
    {
        public CardReaderResponse Response { get; set; }
        public byte[] TrackData { get; set; }
        public int Length { get; set; }
    }
}