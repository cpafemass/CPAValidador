using System;
using System.Collections.Generic;
using System.Text;
using ZXing.Net.Maui;

namespace Aplicativo.Services
{
    public partial class Scanner : ContentPage
    {
        public string Result { get; set; }
        private bool isProcessing = false;
        public Scanner()
        {
            InitializeComponent();

            barcodeReader.Options = new BarcodeReaderOptions
            {
                Formats = BarcodeFormat.QrCode,
                AutoRotate = true,
                Multiple = false,
                
            };
        }


        private void BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
        {
            if (isProcessing) return;
            var results = e.Results.FirstOrDefault();
            if (results != null)
            {
                isProcessing = true;

                barcodeReader.IsDetecting = false;

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    Result = results.Value;

                    if (Navigation.ModalStack.Count > 0)
                    {
                        await Navigation.PopModalAsync();
                    }

                });
            }
        }


    }
}
