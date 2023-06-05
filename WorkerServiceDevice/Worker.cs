using System.Text.Json;
using System.Text;
using Microsoft.Azure.Devices.Client;

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
            //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //await Task.Delay(1000, stoppingToken);

            Console.WriteLine("Insert a random code consisting of five numbers: ");
            string picRandGeneratedCode = Console.ReadLine().ToString();

            string building = "A";
            string raspberryID = "1";
            string doorID = "1001";

            Console.WriteLine("Building: " + building);
            Console.WriteLine("RaspberryID: " + raspberryID);
            Console.WriteLine("PIC random generated code: " + picRandGeneratedCode);
            Console.WriteLine("Door ID: " + doorID);

            List<Packet> packets = new List<Packet>();

            packets.Add(new Packet("building", building));
            packets.Add(new Packet("raspberryID", raspberryID));
            packets.Add(new Packet("picRandGeneratedCode", picRandGeneratedCode));
            packets.Add(new Packet("doorID", doorID));


            // Create JSON message
            string messageBody = JsonSerializer.Serialize(
            new
            {
                openDoorRequest = packets
            });
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