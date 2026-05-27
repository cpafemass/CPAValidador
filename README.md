# ValidadorDeCPA

Simples aplicativo de verificação de QRCode que pode ser compilado e publicado para Windows, Android, Ios e MacOs. <br>
É recomendado utilizar a IDE [Visual Studio da Microsoft](https://visualstudio.microsoft.com/pt-br/), ela possui todas as ferramentas necessárias para publicação e debug. 

## Tecnologias
- Linguagem de programação: C# (CSharp) + .NET 10 
- Framework multiplataforma: Maui Blazor Hybrid
- Leitor de QRCode: Zxing
- Componentes da interface: Mudblazor

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
