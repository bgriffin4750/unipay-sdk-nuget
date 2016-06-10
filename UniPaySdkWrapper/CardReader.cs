using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace UniPaySdkWrapper
{
    public class CardReader
    {
        public bool IsOpen { get; private set; }
        internal BackgroundWorker MsrBackgroundWorker { get; private set; }

        [DllImport("UniPay_SDK.dll")]
        private static extern byte UniPay_OpenHID([In] uint auiVid, [In] uint auiPid);

        [DllImport("UniPay_SDK.dll")]
        private static extern byte UniPay_Close();

        [DllImport("UniPay_SDK.dll")]
        internal static extern byte UniPay_GetFirmware([In, Out] byte[] sFirmware, [Out] out int length);

        [DllImport("UniPay_SDK.dll")]
        internal static extern byte UniPay_GetSerialNumber([In, Out] byte[] sSerialNum, [Out] out int length);

        [DllImport("UniPay_SDK.dll")]
        internal static extern byte UniPay_GetModelNumber([In, Out] byte[] sModelNum, [Out] out int length);

        [DllImport("UniPay_SDK.dll")]
        private static extern byte UniPay_SmartGetEncryptionMode([In, Out] byte[] m_bEMode);

        [DllImport("UniPay_SDK.dll")]
        private static extern byte UniPay_EnableMSR([In] byte m_bTrack, [In, Out] byte[] m_bData, [Out] out int length);

        [DllImport("UniPay_SDK.dll")]
        private static extern byte UniPay_CancelMSR();

        public event EventHandler<CardSwipedEventArgs> CardSwiped;

        /// <summary>
        ///     Opens the instance.
        /// </summary>
        public CardReaderResponse Open()
        {
            if (MsrBackgroundWorker == null)
            {
                MsrBackgroundWorker = new BackgroundWorker {WorkerSupportsCancellation = true};
                MsrBackgroundWorker.DoWork += EnableMSR;
                MsrBackgroundWorker.RunWorkerCompleted += EnableMsrCompleted;
            }

            return OpenCardReader();
        }

        /// <summary>
        ///     Opens the card reader.
        /// </summary>
        private CardReaderResponse OpenCardReader()
        {
            var response = (CardReaderResponse) UniPay_OpenHID(CardReaderHardwareId.AuiVid, CardReaderHardwareId.AuiPid);
            IsOpen = response == CardReaderResponse.Success || response == CardReaderResponse.PortOpened;
            return response;
        }

        /// <summary>
        ///     Closes this instance.
        /// </summary>
        public CardReaderResponse Close()
        {
            try
            {
                if (MsrBackgroundWorker.IsBusy)
                {
                    MsrBackgroundWorker.CancelAsync();
                }

                return (CardReaderResponse) UniPay_Close();
            }
            finally
            {
                IsOpen = false;
            }
        }

        /// <summary>
        ///     Gets the firmware version.
        /// </summary>
        /// <returns>
        ///     The current firmware version as a string.
        /// </returns>
        public string GetFirmwareVersion()
        {
            if (!IsOpen)
            {
                throw new CardReaderException("Unable to get firmware version. Port is closed");
            }

            var version = new byte[128];
            var length = 0;

            var response = (CardReaderResponse) UniPay_GetFirmware(version, out length);

            if (response == CardReaderResponse.Success)
            {
                return Encoding.ASCII.GetString(version, 0, length);
            }

            throw new CardReaderException($"Unable to get firmware version. Response is {(byte) response}: {response}");
        }

        /// <summary>
        ///     Gets the serial number.
        /// </summary>
        /// <returns>
        ///     The serial number as a string.
        /// </returns>
        public string GetSerialNumber()
        {
            var number = new byte[128];
            var length = 0;

            var response = (CardReaderResponse) UniPay_GetSerialNumber(number, out length);

            if (response == CardReaderResponse.Success)
            {
                return Encoding.ASCII.GetString(number, 0, length);
            }


            throw new CardReaderException($"Unable to get serial number. Response is {(byte) response}: {response}");
        }

        /// <summary>
        ///     Gets the model number.
        /// </summary>
        /// <returns>
        ///     The model number as a string.
        /// </returns>
        public string GetModelNumber()
        {
            var model = new byte[128];
            var length = 0;

            var response = (CardReaderResponse)UniPay_GetModelNumber(model, out length);

            if (response == CardReaderResponse.Success)
            {
                return Encoding.ASCII.GetString(model, 0, length);
            }


            throw new CardReaderException($"Unable to get serial number. Response is {(byte)response}: {response}");
        }

        /// <summary>
        ///     Gets the encryption mode.
        /// </summary>
        /// <returns>
        ///     The current encryption mode
        /// </returns>
        public EncryptionType GetEncryptionMode()
        {
            var modeData = new byte[4];

            var response = (CardReaderResponse) UniPay_SmartGetEncryptionMode(modeData);

            if (response == CardReaderResponse.Success && modeData.Length > 0)
            { 
                switch (modeData[0])
                {
                    case 0xFF:
                        return EncryptionType.NoICCDuptKey;
                    case 0x00:
                        return EncryptionType.TDES;
                    case 0x01:
                        return EncryptionType.AES;
                }
            }

            return EncryptionType.Unknown;

        }

        /// <summary>
        ///     Enables the read.
        /// </summary>
        /// <param name="trackInfo">The track information.</param>
        public void EnableRead(TrackType trackInfo)
        {
            if (!MsrBackgroundWorker.IsBusy)
            {
                MsrBackgroundWorker.RunWorkerAsync(trackInfo);
            }
        }

        /// <summary>
        ///     Enables the MSR.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs" /> instance containing the event data.</param>
        private void EnableMSR(object sender, DoWorkEventArgs e)
        {
            var response = CardReaderResponse.Unknown;

            try
            {
                var trackData = new byte[512];
                var length = 0;

                var tracks = Convert.ToByte(e.Argument);                

                // yes, i did this and i know it is terrible and will eventually cause a problem
                // but, i am at a loss on what else to do, this card reader keeps spitting back 
                // a byte array that that has a length of 7 and there is no documentation that explains
                // why or how to prevent it, but doing this a couple of times fixes the problem
                // janky flush
                while (length <= 7)
                {
                    // this pause allows the card reader to catch up
                    // without it, two swipes are necessary
                    Thread.Sleep(500);
                    response = (CardReaderResponse)UniPay_EnableMSR(tracks, trackData, out length);

                    // get out if this is the case
                    if (response == CardReaderResponse.PortClosed || response == CardReaderResponse.Fail)
                    {
                        break;
                    }
                }

                e.Result = new CardSwipedEventArgs(response) { TrackData = trackData, Length = length };

            }
            catch (Exception ex)
            {
                var newEx = new CardReaderException($"Unable to read card reader. Response is {(byte)response}:{response}", ex);
                e.Result = new CardSwipedEventArgs(response, newEx);
            }

        }

        /// <summary>
        ///     Enables the MSR completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RunWorkerCompletedEventArgs" /> instance containing the event data.</param>
        private void EnableMsrCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var data = (CardSwipedEventArgs)e.Result;

            if (data.HasError || data.Response != CardReaderResponse.Success)
            {
                InvokeCardSwipedEvent(data);
                return;
            }

            try
            {                
                var cardInfo = CardDataReader.GetCardInfo(data.TrackData);
                cardInfo.CardReaderResponse = data.Response;
                data.CardInfo = cardInfo;

                InvokeCardSwipedEvent(data);
            }
            catch (Exception ex)
            {
                var newEx = new CardReaderException("Failed processing card data.", ex);
                data.Exception = newEx;
                InvokeCardSwipedEvent(data);
            }
        }

        /// <summary>
        ///     Processes the card.
        /// </summary>
        /// <param name="cardInfo">The card information.</param>
        private void InvokeCardSwipedEvent(CardSwipedEventArgs e)
        {
            if (CardSwiped != null)
            {
                CardSwiped.Invoke(this, e);
            }
        }

        /// <summary>
        ///     Cancels the read.
        /// </summary>
        public CardReaderResponse CancelRead()
        {
            return (CardReaderResponse)UniPay_CancelMSR();
        }

    }
}
