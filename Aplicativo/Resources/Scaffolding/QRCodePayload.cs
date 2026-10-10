using System.Text.Json.Serialization;

namespace Aplicativo.Resources.Scaffolding;

public class QRCodePayload
{
    [JsonPropertyName("codigoValidacao")]
    public string? CodigoValidacao { get; set; }

    [JsonPropertyName("codigoDigestFinal")]
    public string? CodigoDigestFinal { get; set; }

    [JsonPropertyName("validado")]
    public bool Validado { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("mensagem")]
    public string? Mensagem { get; set; }

    [JsonIgnore]
    public bool HasFailed => !Validado || Status != "VALIDADO";
}
