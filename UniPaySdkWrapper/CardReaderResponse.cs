namespace UniPaySdkWrapper
{
    public enum CardReaderResponse
    {
        Fail = 0,
        Success = 1,
        ParameterErr = 99,
        KeysNotLoaded = 100,
        PanError = 101,
        EncryptionDecryptionError = 102,
        NoAdminKey = 103,
        AdminKeyStop = 104,
        KsnError = 105,
        GetAuthenticationCodeError = 106,
        ValidateAuthenticationCodeError = 107,
        EncryptDecryptDataError = 108,
        NotSupportKeyType = 109,
        KeyIndexError = 110,
        StepError = 111,
        NoSerialNumber = 112,
        InvalidCommand = 113,
        NotSupportCommand = 114,
        PinDukptStop = 115,
        TimeOut = 116,
        NoCardData = 117,
        MagiiNoResponse = 118,
        NoCardSeated = 119,
        IccPowerUpError = 120,
        IccCommunicationTimeout = 121,
        IccCommunicationError = 122,
        CapduFormatError = 123,
        LowerPower = 124,
        IoLineLow = 125,
        PortOpened = 200,
        PortClosed = 201,
        MsrModel = 202,
        Unknown = 255
    }
}