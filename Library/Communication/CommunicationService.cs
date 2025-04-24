using Library.Events;
using Library.Interfaces;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Library.Communication;

public class CommunicationService(IEventAggregator eventAggregator) : ICommunicationService
{
    private readonly IEventAggregator eventAggregator = eventAggregator;
    private bool isListening = true;
    private readonly int sourcePort = 12345;
    private readonly int httpPort = 8081;

    public async Task ListenAsync()
    {
        ListenTcpAsync();
        ListenHttpAsync();
    }

    private async Task ListenTcpAsync()
    {
        TcpListener tcpListener = new(IPAddress.Any, sourcePort);
        tcpListener.Start();

        try
        {
            while (isListening)
            {
                TcpClient client = await tcpListener.AcceptTcpClientAsync();
                _ = HandleTcpClientAsync(client);
            }
        }
        finally
        {
            tcpListener.Stop();
        }
    }

    private async Task HandleTcpClientAsync(TcpClient client)
    {
        using (client)
        {
            NetworkStream stream = client.GetStream();
            byte[] data = new byte[1024];
            int bytesRead = await stream.ReadAsync(data);
            string jsonString = Encoding.UTF8.GetString(data, 0, bytesRead);
            var payload = JsonSerializer.Deserialize<CommunicationPayload>(jsonString);
            eventAggregator.GetEvent<CommunicationEvent>().Publish(payload);
        }
    }

    private async Task ListenHttpAsync()
    {
        HttpListener httpListener = new();
        httpListener.Prefixes.Add($"http://localhost:{httpPort}/");
        httpListener.Start();

        try
        {
            while (isListening)
            {
                HttpListenerContext context = await httpListener.GetContextAsync();
                _ = HandleHttpContextAsync(context);
            }
        }
        finally
        {
            httpListener.Stop();
        }
    }

    private async Task HandleHttpContextAsync(HttpListenerContext context)
    {
        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;

        response.Headers.Add("Access-Control-Allow-Origin", "*");
        response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
        response.Headers.Add("Access-Control-Allow-Credentials", "true");

        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

        response.ContentType = "application/json";

        if (request.HttpMethod == "OPTIONS")
        {
            response.StatusCode = (int)HttpStatusCode.NoContent;
            response.Close();
            return;
        }

        if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/send")
        {
            using (var reader = new StreamReader(request.InputStream, Encoding.UTF8))
            {
                string jsonString = await reader.ReadToEndAsync();
                var payload = JsonSerializer.Deserialize<CommunicationPayload>(jsonString);
                eventAggregator.GetEvent<CommunicationEvent>().Publish(payload);
            }

            response.StatusCode = (int)HttpStatusCode.NoContent;
            response.Close();
        }
    }

    public async Task SendMessageAsync(string message, int port)
    {
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        using TcpClient client = new(ipAddress.ToString(), port);
        NetworkStream stream = client.GetStream();

        var payload = new CommunicationPayload
        {
            Message = message,
            Port = sourcePort
        };

        string serializedPayload = JsonSerializer.Serialize(payload);
        byte[] data = Encoding.UTF8.GetBytes(serializedPayload);
        await stream.WriteAsync(data);
    }

    public void StopListening()
    {
        isListening = false;
    }
}
