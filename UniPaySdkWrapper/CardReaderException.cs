using System;

namespace UniPaySdkWrapper
{
    public class CardReaderException : Exception
    {
        public CardReaderException(string message) : base(message)
        {            
        }

        public CardReaderException(string message, Exception innerException) : base(message, innerException)
        {            
        }
    }
}