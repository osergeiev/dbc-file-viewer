using DbcViewer.Core;

string filePath = Path.Combine(AppContext.BaseDirectory, "example.dbc");
DbcParser parser = new DbcParser();

var network = parser.Parse(filePath);

foreach (var message in network.Messages)
{
    Console.WriteLine($"Message {message.Id} : {message.Name}, Length = {message.Length}, Transmitter = {message.Transmitter}");

    foreach (var signal in message.Signals)
    {
        Console.WriteLine($"  Signal: {signal.Name}");
        Console.WriteLine($"    StartBit: {signal.StartBit}, Length: {signal.Length}");
        Console.WriteLine($"    Factor: {signal.Factor}, Offset: {signal.Offset}");
        Console.WriteLine($"    Unit: {(string.IsNullOrEmpty(signal.Unit) ? "(none)" : signal.Unit)}");

        if (signal.Attributes.Count > 0)
        {
            Console.WriteLine($"    Attributes:");
            foreach (var attr in signal.Attributes)
            {
                Console.WriteLine($"      {attr.Name} = {attr.Value}");
            }
        }
    }
}
