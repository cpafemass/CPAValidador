using System.Text.Json.Serialization;

namespace Aplicativo.Resources.Scaffolding;

public class QRCodePayload
{
    [JsonPropertyName("resumo")]
    public ResumoValidacao? Resumo { get; set; }
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

public class ResumoValidacao
{
    public long ValidacaoId { get; set; }
    public CampanhaValidacao? Campanha { get; set; }
    public FormularioValidacao? Formulario { get; set; }
    public List<AvaliacaoValidacao> Avaliacoes { get; set; } = new();

    [JsonIgnore]
    public bool HasContext => Campanha is not null || Formulario is not null || Avaliacoes.Count > 0;
}
public class CampanhaValidacao
{
    public string? Codigo { get; set; }
    public string? Nome { get; set; }
}
public class FormularioValidacao
{
    public string? Codigo { get; set; }
    public string? Nome { get; set; }
    public int Versao { get; set; }
    public string? Publico { get; set; }
    public string? Escopo { get; set; }
}
public class CursoValidacao
{
    public long Id { get; set; }
    public string? Nome { get; set; }
}
public class DisciplinaValidacao : CursoValidacao
{
    public string? Professor { get; set; }
}
public class AvaliacaoValidacao
{
    public long Id { get; set; }
    public CursoValidacao? Curso { get; set; }
    public DisciplinaValidacao? Disciplina { get; set; }
    public List<long> RespostaIds { get; set; } = new();
    [JsonPropertyName("formulario")]
    public FormularioValidacao? Formulario { get; set; }
}
