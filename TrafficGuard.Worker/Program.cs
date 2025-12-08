using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TrafficGuard.Worker;

class Program
{
    // MUDANÇA 1: O Main precisa ser 'async Task' para usar await
    static async Task Main(string[] args)
    {
        const string QUEUE_NAME = "radares";

        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "admin",
            Password = "admin123"
        };

        // MUDANÇA 2: Usamos 'await' para criar a conexão e o canal
        // Nota: Se der erro no 'await using', mude apenas para 'using' ou remova o using
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        // MUDANÇA 3: QueueDeclareAsync (Tudo agora é Async)
        await channel.QueueDeclareAsync(queue: QUEUE_NAME,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        Console.WriteLine(" [*] Aguardando infrações... Pressione ENTER para sair.");

        var consumer = new AsyncEventingBasicConsumer(channel);

        // MUDANÇA 4: O evento precisa retornar uma Task (Task.CompletedTask)
        consumer.ReceivedAsync += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                var dados = JsonConvert.DeserializeObject<DadosRadar>(message);
                ProcessarInfracao(dados);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" [!] Erro ao processar mensagem: {ex.Message}");
            }

            // Avisa o RabbitMQ que terminamos de processar essa mensagem
            return Task.CompletedTask;
        };

        // MUDANÇA 5: BasicConsumeAsync
        await channel.BasicConsumeAsync(queue: QUEUE_NAME,
                             autoAck: true,
                             consumer: consumer);

        Console.ReadLine();
    }

    static void ProcessarInfracao(DadosRadar? dados)
    {
        if (dados == null) return;

        Console.Write($"[Processando] {dados.Placa} a {dados.Velocidade}km/h... ");

        if (dados.Velocidade > 60)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("MULTADO! 🚨");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Dentro do limite.");
        }

        Console.ResetColor();
    }
}

public class DadosRadar
{
    public string Placa { get; set; } = "";
    public int Velocidade { get; set; }
    public string Via { get; set; } = "";
    [JsonProperty("data_hora")]
    public DateTime DataHora { get; set; }
}