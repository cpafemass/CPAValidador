using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Aplicativo.Resources.Scaffolding
{
    public class QRCodePayload
    {
        public QRCodePayload() { }
        public QRCodePayload(bool hasFailed) 
        {
            if (hasFailed)
            {
                this.cpf = "Falha";
                this.matricula = "Falha";
                this.cursos = new List<string>() { "Falha" };
                this.disciplinas = new List<string>() { "Falha" };
                this.identificador = "Falha";
                this.HasFailed = true;
            }
        }

        [JsonPropertyName("cpf")]
        public string? cpf { get; set; }
        // Esse Payload possui o campo de cpf por que ele espelha o mesmo dto que o front envia para o back para criação do hash, ele não é utilizado no aplicativo de validação
        [JsonPropertyName("matricula")]
        public string? matricula { get; set; }
        [JsonPropertyName("cursos")]
        public List<string>? cursos { get; set; }
        [JsonPropertyName("disciplinas")]
        public List<string>? disciplinas { get; set; }
        [JsonPropertyName("identificador")]
        public string? identificador { get; set; }

        public bool HasFailed { get; set; }
    }
}
