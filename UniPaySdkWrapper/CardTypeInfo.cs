namespace UniPaySdkWrapper
{
    /// <summary>
    ///     Constructor to initialize class
    /// </summary>
    public class CardTypeInfo
    {
        public CardTypeInfo(string regEx, int length, CardType type)
        {
            RegEx = regEx;
            Length = length;
            Type = type;
        }

        public string RegEx { get; set; }
        public int Length { get; set; }
        public CardType Type { get; set; }
    }
}