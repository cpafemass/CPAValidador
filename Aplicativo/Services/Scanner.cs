using Aplicativo.Resources.Scaffolding;

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
        private readonly HttpService _httpService;
        public Scanner()
        {
            InitializeComponent();

            barcodeReader.Options = new BarcodeReaderOptions
            {
                Formats = BarcodeFormat.QrCode,
                AutoRotate = true,
                Multiple = false,
                TryHarder = false,
                TryInverted = false,
                
            };
        }
        public Scanner(HttpService httpService)
        {
            _httpService = httpService;
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
        /// <summary>
        /// Função responsável pela validação do código, ela irá chamar o DatabaseServices que terá acesso direto ao banco de dados e verificará o hash
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<QRCodePayload> ValidateQRCode(string content)
        {
            //Trocar a lógica depois, a lógica atual serve somente para testar a troca de cores dinamicas do "Hash scaneado" da home

            return await _httpService.CheckHash(content);
        }

    }
}
