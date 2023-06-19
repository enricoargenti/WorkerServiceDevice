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

            openDoorRequest.TypeOfMessage = "newOpenDoorRequest";

            Console.Write("DoorId: ");
            openDoorRequest.DoorId = Convert.ToInt32(Console.ReadLine());

            Console.Write("DeviceId: ");
            openDoorRequest.DeviceId = Console.ReadLine();

            Console.Write("DeviceGeneratedCode: ");
            openDoorRequest.DeviceGeneratedCode = Console.ReadLine();

            //Console.Write("CloudGeneratedCode: ");
            //openDoorRequest.CloudGeneratedCode = Convert.ToInt32(Console.ReadLine());

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

            Console.WriteLine();
            Console.WriteLine("---------- New openDoorRequestStep2 -----------");
            OpenDoorRequest openDoorRequestStep2 = new OpenDoorRequest();

            openDoorRequestStep2.TypeOfMessage = "secondMessageFromDoor";

            Console.Write("DoorId: ");
            openDoorRequestStep2.DoorId = Convert.ToInt32(Console.ReadLine());

            Console.Write("DeviceId: ");
            openDoorRequestStep2.DeviceId = Console.ReadLine();

            // Bisognerà imporre al PIC di mandare questo codice anche nel secondo messaggio
            openDoorRequestStep2.DeviceGeneratedCode = openDoorRequest.DeviceGeneratedCode;

            Console.Write("CodeInsertedOnDoorByUser: ");
            openDoorRequestStep2.CodeInsertedOnDoorByUser = Console.ReadLine();

            //Console.Write("CloudGeneratedCode: ");
            //openDoorRequest.CloudGeneratedCode = Convert.ToInt32(Console.ReadLine());

            openDoorRequestStep2.AccessRequestTime = DateTime.Now;


            // Create JSON message
            string messageBodyStep2 = JsonSerializer.Serialize(openDoorRequestStep2);
            using var messageStep2 = new Message(Encoding.ASCII.GetBytes(messageBodyStep2))
            {
                ContentType = "application/json",
                ContentEncoding = "utf-8",
            };
            // Send the telemetry message
            await deviceClient.SendEventAsync(messageStep2, stoppingToken);

        }
    }
}