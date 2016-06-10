using System;
//using UniPaySdkWrapper;

namespace CardReaderSample
{
    internal class Program
    {
        //private static CardReader _cardReader;

        private static void Main(string[] args)
        {
            //_cardReader = new CardReader();

            try
            {
                //_cardReader.Open();
                //_cardReader.CardSwiped += CardReaderOnCardSwiped;
                //_cardReader.EnableRead(TrackType.Track1Only);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.ReadKey();
        }

        //private static void CardReaderOnCardSwiped(object sender, CardSwipedEventArgs cardSwipedEventArgs)
        //{
        //    Console.WriteLine(cardSwipedEventArgs.CardInfo.Last4);
        //    _cardReader.CancelRead();
        //    _cardReader.Close();
        //}
    }
}