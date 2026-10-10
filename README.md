# ValidadorDeCPA

Simples aplicativo de verificação de QRCode que pode ser compilado e publicado para Windows, Android, Ios e MacOs. <br>
É recomendado utilizar a IDE [Visual Studio da Microsoft](https://visualstudio.microsoft.com/pt-br/), ela possui todas as ferramentas necessárias para publicação e debug. 

## Tecnologias
- Linguagem de programação: C# (CSharp) + .NET 10 
- Framework multiplataforma: Maui Blazor Hybrid
- Leitor de QRCode: Zxing
- Componentes da interface: Mudblazor

## Leitura do QR Code do frontend

O frontend transforma o valor `qrCode` recebido de `POST /formulario` em uma imagem QR. O conteúdo dessa imagem é um código opaco, sem dados pessoais. O aplicativo lê esse texto e envia uma única requisição `PUT /validacao/validar-hash?codigoValidacao=...`, com o código escapado para a URL.

A resposta utiliza `codigoValidacao`, `validado`, `status`, `mensagem` e `codigoDigestFinal`. Após validar um QR Code, o aplicativo exibe o identificador formado pelos 10 últimos caracteres do digest armazenado no backend. Esse mesmo identificador aparece em cada item do histórico das 10 últimas validações, para permitir conferir a leitura. Por ser hexadecimal, ele pode conter números e letras de `a` a `f`. Cursos e disciplinas não são retornados pela API atual.

O endereço da API é definido pelo responsável pela publicação em `Aplicativo/appsettings.json`, no campo `Configs:BaseUrl`, e é incorporado ao aplicativo. O usuário não configura nem altera esse endereço pelo aplicativo.

- Produção: utilize o endereço HTTPS da API, incluindo um eventual prefixo de caminho.
- Emulador Android: para uma API executando no computador, utilize `http://10.0.2.2:8080/`.
- Celular físico: utilize um endereço da API acessível pelo celular; `localhost` aponta para o próprio celular. Para desenvolvimento na rede local, exponha a API na interface de rede e confira o firewall. HTTP sem criptografia é permitido no Android somente em builds Debug.

Antes de gerar o pacote de publicação, preencha o campo com a URL da API, por exemplo:

```json
{
  "Configs": {
    "BaseUrl": "https://api.exemplo.edu.br/"
  }
}
```

O aplicativo informa que a API não foi configurada caso o pacote seja publicado com esse campo vazio ou inválido.

A leitura requer permissão de câmera. Fechar ou cancelar o scanner não envia uma validação. Códigos desconhecidos, expirados e já utilizados exibem mensagens específicas. A validação é de uso único: em caso de falha de conexão ou timeout, ela pode ter sido registrada no servidor, por isso confira o histórico antes de repetir.

### Verificação

Testes do contrato HTTP, sem dependências de MAUI ou pacotes de teste externos:

```powershell
dotnet run --project Tests/ContractTests.csproj
```

Compilação Android:

```powershell
dotnet build Aplicativo/Aplicativo.csproj -f net10.0-android -p:TargetFrameworks=net10.0-android
```

No dispositivo, envie uma avaliação pelo frontend e escaneie o comprovante: a primeira leitura deve confirmar a validação e a segunda deve informar que o código já foi utilizado. Confira também a permissão negada, o cancelamento da câmera, a API indisponível e o histórico.

## Como publicar para Android
1. Abrir a IDE através do ValidadorDeCPA.slnx ou através da própria IDE.
2. Colocar a solução em modo [Release](https://drive.google.com/file/d/1cpTKRCv0eNzAXOZBWGAB-Nmrurs6P8e-/view?usp=sharing) .
3. Apertar botão direito do mouse em "Aplicativo" e apertar na opção "Publicar..."
4. O IDE começara a empacotar o aplicativo, esse processo pode demorar algums minutos.
5. Quando terminar, aperte no botão "Distribuir..."
6. Aperte em "Ad Hoc"
7. Nessa parte, será necessário criar uma **Chave de Assinatura**, Aperte no +
8. Em seguida, irá abrir uma janela de criação de chave de assinatuira, nela, você irá preencher os campos com as informações necessárias e apertar em "Criar".
9. Com a chave criada e selecionada, aperte em salvar como e escolha um local de salvamento de fácil acesso
10. Após isso, você poderá instalar o aplicativo no seu android (lembre-se de permitir instalação de aplicativo de terceiros no celular).

## Estrutura de pastas
`Aplicativo/Platforms`: Permissões de plataforma, modificar somente se necessário. <br>
`Aplicativo/Components`: Componentes do aplicativo, como telas com **rota** e componentes soltos.  <br>
`Aplicativo/Components/Layout`: Pasta onde se localiza o `MainLayout.razor` e `MainLayout.razor.css`, são esses arquivos que "seguram" a renderização das páginas.  <br>
`Aplicativo/Components/Pages`: Páginas com rotas.  <br>
`Aplicativo/Resources`: Recursos do aplicativo.  <br>
`Aplicativo/Services`: Serviços injetados via injeção de dependencia dentro dos componentes com/sem rota.  <br>
`Aplicativo`: Arquivos de inicialização do aplicativo e configuração da solução (csproj).  <br> 

