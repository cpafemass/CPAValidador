using Aplicativo.Services;
using System.Net;
using System.Text;

const string token = "AbCdEfGhIjKlMnOpQrSt_-";
var passed = 0;

await Check("Código opaco do frontend e contrato de sucesso", async () =>
{
    var handler = new StubHandler((request) =>
    {
        Assert(request.Method == HttpMethod.Put, "A validação deve usar PUT.");
        Assert(request.RequestUri!.AbsoluteUri == $"https://api.example.test/api/validacao/validar-hash?codigoValidacao={token}", "Endpoint ou código incorreto.");
        return Json(HttpStatusCode.OK, $$"""{"codigoValidacao":"{{token}}","validado":true,"status":"VALIDADO","mensagem":"Codigo validado com sucesso","codigoDigestFinal":"012345abcd"}""");
    });
    var result = await Service(handler).CheckHash($"  {token}\n");
    Assert(!result.HasFailed && result.CodigoValidacao == token, "A resposta atual deve ser aceita.");
    Assert(result.CodigoDigestFinal == "012345abcd", "O identificador retornado na validação deve ser preservado.");
    Assert(handler.Calls == 1, "O código não deve ser enviado mais de uma vez.");
});

await Check("Codificação de caracteres na query", async () =>
{
    var handler = new StubHandler(request =>
    {
        Assert(request.RequestUri!.Query == "?codigoValidacao=a%2Bb%26c%3D%3F%23", "Caracteres da query não foram escapados.");
        return Json(HttpStatusCode.NotFound, "{}");
    });
    await Service(handler).CheckHash("a+b&c=?#");
});

await Check("Cancelamento e conteúdo vazio não chamam a API", async () =>
{
    var handler = new StubHandler(_ => throw new Exception("Não deve chamar a API."));
    var service = Service(handler);
    foreach (var code in new string?[] { null, "", "   " })
        Assert((await service.CheckHash(code)).HasFailed, "Conteúdo vazio aceito.");
    Assert(handler.Calls == 0, "Conteúdo vazio chegou ao servidor.");
});

foreach (var (status, body, message) in new[]
{
    (HttpStatusCode.BadRequest, "{}", "inválido"),
    (HttpStatusCode.NotFound, "{}", "não encontrado"),
    (HttpStatusCode.Conflict, "{\"error\":\"Codigo ja foi validado e nao pode ser reutilizado\"}", "já foi validado"),
    (HttpStatusCode.Conflict, "{\"error\":\"Codigo expirado\"}", "expirou"),
    (HttpStatusCode.InternalServerError, "{}", "Tente novamente mais tarde")
})
    await Check($"Erro HTTP {(int)status}: {message}", async () =>
    {
        var handler = new StubHandler(_ => Json(status, body));
        var result = await Service(handler).CheckHash(token);
        Assert(result.HasFailed && result.Mensagem!.Contains(message), "Mensagem de erro incorreta.");
        Assert(handler.Calls == 1, "Não deve repetir a validação.");
    });

foreach (var body in new[] { "null", "{}", "{\"cursos\":[\"Curso antigo\"]}", "{\"validado\":false,\"status\":\"PENDENTE\"}", "{\"codigoValidacao\":\"outro-codigo\",\"validado\":true,\"status\":\"VALIDADO\"}", "invalid-json" })
    await Check($"Resposta inválida: {body}", async () =>
        Assert((await Service(new StubHandler(_ => Json(HttpStatusCode.OK, body))).CheckHash(token)).HasFailed, "Resposta incompatível aprovada."));

foreach (var exception in new Exception[] { new HttpRequestException(), new TaskCanceledException() })
    await Check($"Falha de conexão: {exception.GetType().Name}", async () =>
    {
        var handler = new StubHandler(_ => throw exception);
        var result = await Service(handler).CheckHash(token);
        Assert(result.HasFailed && result.Mensagem!.Contains("histórico"), "A falha deve informar resultado incerto.");
        Assert(handler.Calls == 1, "Não deve repetir um PUT após falha de conexão.");
    });

await Check("URL não configurada", async () =>
{
    var handler = new StubHandler(_ => throw new Exception("Não deve chamar a API."));
    var service = new HttpService(new HttpClient(handler));
    Assert((await service.CheckHash(token)).HasFailed && handler.Calls == 0, "URL vazia não tratada.");
});

await Check("Histórico sem dados pessoais", async () =>
{
    var service = Service(new StubHandler(request =>
    {
        Assert(request.Method == HttpMethod.Get && request.RequestUri!.AbsolutePath == "/api/validacao/historico", "Endpoint do histórico incorreto.");
        return Json(HttpStatusCode.OK, "[{\"codigoValidacao\":\"codigo\",\"validado\":true,\"status\":\"VALIDADO\",\"mensagem\":\"Codigo validado\",\"codigoDigestFinal\":\"012345abcd\"}]");
    }));
    var history = await service.GetHistory();
    Assert(history?.Count == 1 && !history[0].HasFailed, "Histórico incompatível.");
    Assert(history![0].CodigoDigestFinal == "012345abcd", "O histórico deve preservar o identificador da validação.");
});

Console.WriteLine($"{passed} testes passaram.");

async Task Check(string name, Func<Task> test)
{
    await test();
    passed++;
    Console.WriteLine($"PASS: {name}");
}

static HttpService Service(StubHandler handler) => new(new HttpClient(handler) { BaseAddress = new Uri("https://api.example.test/api/") });
static HttpResponseMessage Json(HttpStatusCode status, string body) => new(status) { Content = new StringContent(body, Encoding.UTF8, "application/json") };
static void Assert(bool condition, string message) { if (!condition) throw new Exception(message); }

sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
{
    public int Calls { get; private set; }
    public Uri? LastUri { get; private set; }
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Calls++;
        LastUri = request.RequestUri;
        return Task.FromResult(respond(request));
    }
}
