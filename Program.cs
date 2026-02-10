using System;
using System.Net.Http;               
using System.Text.Json;              
using System.Text.Json.Serialization;
using System.Threading.Tasks;        
using System.Collections.Generic;
using System.IO;
using System.Net;      
using System.Net.Mail;
using System.Globalization;

namespace DesafioInoa
{   public class ConfigSmtp
    {
        [JsonPropertyName("servidor")]
        public string Servidor { get; set; }

        [JsonPropertyName("porta")]
        public int Porta { get; set; }

        [JsonPropertyName("usuario")]
        public string Usuario { get; set; }

        [JsonPropertyName("senha")]
        public string Senha { get; set; }
    }

    public class Configuracao
    {
        [JsonPropertyName("emailDestino")]
        public string EmailDestino { get; set; }

        [JsonPropertyName("smtp")]
        public ConfigSmtp Smtp { get; set; }
    }
    public class Ativo
    {
        [JsonPropertyName("symbol")] 
        public string Simbolo { get; set; }

        [JsonPropertyName("regularMarketPrice")]
        public decimal Preco { get; set; }

        [JsonPropertyName("regularMarketTime")]
        public string Data { get; set; }
    }
    public class RespostaApi
    {
        [JsonPropertyName("results")]
        public List<Ativo> Resultados { get; set; }
    }

    class Program
    {
    static void EnviarEmail(Configuracao config, string assunto, string corpo)
    {
        try
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(config.Smtp.Usuario);
            mail.To.Add(config.EmailDestino);
            mail.Subject = assunto;
            mail.Body = corpo;
            mail.IsBodyHtml = false; // Texto simples para evitar spam

            SmtpClient smtpClient = new SmtpClient(config.Smtp.Servidor, config.Smtp.Porta);
            smtpClient.Credentials = new NetworkCredential(config.Smtp.Usuario, config.Smtp.Senha);
            smtpClient.EnableSsl = true;

            smtpClient.Send(mail);
            Console.WriteLine("--> E-mail enviado com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Falha ao enviar e-mail: {ex.Message}");
        }
    }
        static async Task Main(string[] args)
        {   if (args.Length < 3)
            {
                Console.WriteLine("Erro: Use: DesafioInoa.exe [ATIVO] [PRECO_VENDA] [PRECO_COMPRA]");
                Console.WriteLine("Exemplo: DesafioInoa.exe PETR4 22.70 22.50");
                return;
            }

            string ticker = args[0];
            string precoMaximo = args[1];
            string precoMinimo = args[2];

            decimal precoMinimoDecimal = decimal.Parse(precoMinimo, CultureInfo.InvariantCulture);
            decimal precoMaximoDecimal = decimal.Parse(precoMaximo, CultureInfo.InvariantCulture);

            if (precoMinimoDecimal <= 0 || precoMaximoDecimal <= 0)
            {
                Console.WriteLine("Erro: Os preços devem ser valores positivos (maiores que zero).");
                return;
            }

            if (precoMinimoDecimal >= precoMaximoDecimal)
            {
                Console.WriteLine("Erro: Lógica inválida. O preço de VENDA deve ser maior que o de COMPRA para você ter lucro.");
                return;
            }

            Configuracao config;
            try 
            {
                string jsonConfig = await File.ReadAllTextAsync("config.json");
                config = JsonSerializer.Deserialize<Configuracao>(jsonConfig);
            }
            catch (Exception error)
            {
                Console.WriteLine($"Erro ao ler config.json: {error.Message}");
                return;
            }

            string url = $"https://brapi.dev/api/quote/{ticker}";

            Console.WriteLine($"--- Buscando dados de {ticker} na internet... ---");

            // Cria o "navegador"
            using (HttpClient client = new HttpClient())
            {
                while (true)
                {
                    try
                    {
                        // 1. Faz a requisição e espera a resposta
                        string jsonResposta = await client.GetStringAsync(url);
                        
                        Console.WriteLine(jsonResposta);

                        // 2. Transforma o texto em Objetos C# (Deserialização)
                        RespostaApi dados = JsonSerializer.Deserialize<RespostaApi>(jsonResposta);

                        Ativo ativoEncontrado = dados.Resultados[0];

                        Console.WriteLine("--- DADOS RECEBIDOS ---");
                        Console.WriteLine($"Ativo: {ativoEncontrado.Simbolo}");
                        Console.WriteLine($"Preço Atual: {ativoEncontrado.Preco:C}");
                        Console.WriteLine($"Data/Hora: {ativoEncontrado.Data}");

                        if (ativoEncontrado.Preco <= precoMinimoDecimal)
                            {
                                Console.WriteLine("--> ALERTA DE COMPRA! Enviando e-mail...");
                                EnviarEmail(
                                    config, 
                                    $"COMPRA: {ativoEncontrado.Simbolo} atingiu o alvo!", 
                                    $"O preço caiu para {ativoEncontrado.Preco:C}. O seu alvo de compra era {precoMinimoDecimal:C}."
                                );
                            }
                            else if (ativoEncontrado.Preco >= precoMaximoDecimal)
                            {
                                Console.WriteLine("--> ALERTA DE VENDA! Enviando e-mail...");
                                EnviarEmail(
                                    config, 
                                    $"VENDA: {ativoEncontrado.Simbolo} atingiu o alvo!", 
                                    $"O preço subiu para {ativoEncontrado.Preco:C}. O seu alvo de venda era {precoMaximoDecimal:C}."
                                );
                            }
                    }

                    catch (HttpRequestException httpEx)
                    {
                        Console.WriteLine($" Erro: O ativo '{ticker}' não foi encontrado ou a API está fora do ar.");
                        Console.WriteLine($"Detalhe técnico: {httpEx.Message}");
                        return;
                    }

                    catch (Exception error)
                    {
                        Console.WriteLine($"Erro ao conectar: {error.Message}");
                    }
                    Console.WriteLine("Aguardando 30 segundos...");
                    await Task.Delay(30000); // Espera 30 segundos antes da próxima verificação
                }
            }
        }
    }

}
