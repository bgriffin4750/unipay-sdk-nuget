using System;

namespace UniPaySdkWrapper
{
    public class CardSwipedEventArgs : EventArgs
    {
        public CardReaderException Exception { get; set; }
        public CardInfo CardInfo { get; set; }
        public CardReaderResponse Response { get; set; }
        public bool HasError => Exception != null;
        public byte[] TrackData { get; set; }
        public int Length { get; set; }

        public CardSwipedEventArgs(CardReaderResponse response)
        {
            Response = response;
        }

        public CardSwipedEventArgs(CardReaderResponse response, CardReaderException exception)
        {
            Response = response;
            Exception = exception;
        }
    }
}