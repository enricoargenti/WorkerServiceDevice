using System.Text.Json;
using System.Text;
using Microsoft.Azure.Devices.Client;
using WorkerServiceDevice.Models;

namespace WorkerServiceDevice;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly IConfiguration _configuration;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var deviceClient =
            DeviceClient.CreateFromConnectionString(
            _configuration.GetConnectionString("Device"),
            TransportType.Mqtt);

        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine();
            Console.WriteLine("---------- New openDoorRequest -----------");
            OpenDoorRequest openDoorRequest = new OpenDoorRequest();

            Console.Write("DoorId: ");
            openDoorRequest.DoorId = Convert.ToInt32(Console.ReadLine());

            Console.Write("GatewayId: ");
            openDoorRequest.GatewayId = Convert.ToInt32(Console.ReadLine());

            Console.Write("DeviceGeneratedCode: ");
            openDoorRequest.DeviceGeneratedCode = Convert.ToInt32(Console.ReadLine());

            Console.Write("CloudGeneratedCode: ");
            openDoorRequest.CloudGeneratedCode = Convert.ToInt32(Console.ReadLine());

            openDoorRequest.AccessRequestTime = DateTime.Now;


            // Create JSON message
            string messageBody = JsonSerializer.Serialize(openDoorRequest);
            using var message = new Message(Encoding.ASCII.GetBytes(messageBody))
            {
                ContentType = "application/json",
                ContentEncoding = "utf-8",
            };
            // Send the telemetry message
            await deviceClient.SendEventAsync(message, stoppingToken);

            //Thread.Sleep(1000);
        }
    }
}