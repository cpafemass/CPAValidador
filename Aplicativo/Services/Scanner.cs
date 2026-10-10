using ZXing.Net.Maui;

namespace Aplicativo.Services;

public partial class Scanner : ContentPage
{
    public string? Result { get; private set; }
    private int _isProcessing;
    private readonly TaskCompletionSource<string?> _completion = new();
    public Task<string?> Completion => _completion.Task;

    public Scanner()
    {
        InitializeComponent();
        barcodeReader.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormat.QrCode,
            AutoRotate = true,
            Multiple = false,
            TryHarder = true
        };
    }

    private void BarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
    {
        var result = e.Results.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Value));
        if (result == null || Interlocked.Exchange(ref _isProcessing, 1) != 0) return;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            barcodeReader.IsDetecting = false;
            Result = result.Value;
            try
            {
                if (Navigation.ModalStack.Contains(this))
                    await Navigation.PopModalAsync();
            }
            finally
            {
                _completion.TrySetResult(Result);
            }
        });
    }

    protected override void OnDisappearing()
    {
        barcodeReader.IsDetecting = false;
        _completion.TrySetResult(Result);
        base.OnDisappearing();
    }

    private async void CancelScan(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
