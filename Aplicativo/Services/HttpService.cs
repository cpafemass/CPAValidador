using Aplicativo.Resources.Scaffolding;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Aplicativo.Services;

public class HttpService
{
    private readonly HttpClient _httpClient;
    public bool IsConfigured => _httpClient.BaseAddress is not null;

    public HttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<QRCodePayload> CheckHash(string? codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            return Failure("O QR Code não contém um código de validação.");

        // O frontend codifica diretamente o código opaco retornado por /formulario.
        // Não repetir o PUT: a validação consome o código uma única vez.
        try
        {
            using var response = await _httpClient.PutAsync(
                Endpoint($"validacao/validar-hash?codigoValidacao={Uri.EscapeDataString(codigo.Trim())}"), null);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = await response.Content.ReadFromJsonAsync<QRCodePayload>();
                if (result is null)
                    return Failure("O servidor retornou uma resposta vazia.");
                if (result.HasFailed || result.CodigoValidacao != codigo.Trim())
                    return Failure("O servidor retornou uma resposta inesperada. Confira o histórico antes de tentar novamente.");
                return result;
            }

            var message = response.StatusCode switch
            {
                HttpStatusCode.BadRequest => "Código de validação inválido.",
                HttpStatusCode.NotFound => "Código de validação não encontrado.",
                HttpStatusCode.Conflict => "Este código já foi validado ou expirou.",
                _ => "Não foi possível validar o código. Tente novamente mais tarde."
            };
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                var error = await response.Content.ReadFromJsonAsync<ApiError>();
                if (error?.Error?.Contains("expirado", StringComparison.OrdinalIgnoreCase) == true)
                    message = "Este código de validação expirou.";
                else if (error?.Error?.Contains("ja foi validado", StringComparison.OrdinalIgnoreCase) == true)
                    message = "Este código já foi validado e não pode ser reutilizado.";
            }
            return Failure(message);
        }
        catch (OperationCanceledException)
        {
            return Failure("O servidor demorou para responder. A validação pode ter sido registrada; confira o histórico antes de tentar novamente.");
        }
        catch (HttpRequestException)
        {
            return Failure("Não foi possível confirmar a validação. Confira a conexão e o histórico antes de tentar novamente.");
        }
        catch (JsonException)
        {
            return Failure("O servidor retornou uma resposta inesperada. Confira o histórico antes de tentar novamente.");
        }
        catch (InvalidOperationException)
        {
            return Failure("O endereço da API não foi configurado na publicação do aplicativo.");
        }
    }

    public async Task<List<QRCodePayload>?> GetHistory()
    {
        using var response = await _httpClient.GetAsync(Endpoint("validacao/historico"));
        return response.StatusCode == HttpStatusCode.OK
            ? await response.Content.ReadFromJsonAsync<List<QRCodePayload>>()
            : null;
    }

    private static QRCodePayload Failure(string message) => new() { Mensagem = message };

    private Uri Endpoint(string path) => _httpClient.BaseAddress is null
        ? throw new InvalidOperationException("Backend não configurado.")
        : new Uri(_httpClient.BaseAddress, path);

    private sealed class ApiError
    {
        public string? Error { get; set; }
    }
}
