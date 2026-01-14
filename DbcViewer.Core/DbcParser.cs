using System.Xml.Linq;
using DbcViewer.Core.Models;

namespace DbcViewer.Core
{
    public class DbcParser : IDbcParser
    {
        public Network Parse(string filePath)
        {
            Network network = new Network();
            using (StreamReader reader = new StreamReader(filePath))
            {
                Message? currentMessage = null;
                Signal? currentSignal = null;
                Dictionary<int, Message> messagesById = new Dictionary<int, Message>();
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.TrimStart();

                    if (line.StartsWith("BU_:"))
                    {
                        var nodes = line.Substring(line.IndexOf(':') + 1)
                                        .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        foreach (var node in nodes)
                        {
                            network.Nodes.Add(node);
                            Console.WriteLine(node);
                        }
                    }
                    else if (line.StartsWith("BO_ "))
                    {
                        var parts = line.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);

                        int id = int.Parse(parts[1]);
                        string name = parts[2];
                        int length = int.Parse(parts[3]);
                        string transmitter = parts[4];

                        currentMessage = new Message
                        {
                            Id = id,
                            Name = name,
                            Length = length,
                            Transmitter = transmitter
                        };
                        network.Messages.Add(currentMessage);
                        messagesById[id] = currentMessage;
                    }
                    else if (line.StartsWith("SG_ "))
                    {
                        if (currentMessage == null) continue;

                        var parts = line.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
                        string name = parts[1];

                        var bitPart = parts[2].Split('@')[0];
                        var bitParts = bitPart.Split('|');

                        int startBit = int.Parse(bitParts[0]);
                        int length = int.Parse(bitParts[1]);

                        var scale = parts[3].Trim('(', ')').Split(',');
                        double factor = double.Parse(scale[0]);
                        double offset = double.Parse(scale[1]);

                        string unit = (parts.Length > 5) ? parts[5].Trim('"') : string.Empty;
                        unit = unit == "" ? string.Empty : unit;

                        string receiver = parts[6];

                        currentSignal = new Signal
                        {
                            Name = name,
                            StartBit = startBit,
                            Length = length,
                            Factor = factor,
                            Offset = offset,
                            Unit = unit,
                            Receiver = receiver
                        };

                        currentMessage.Signals.Add(currentSignal);
                    }
                    else if (line.StartsWith("BA_ "))
                    {
                        if (!line.Contains("SG_")) continue; //only sg attribures

                        int firstQuote = line.IndexOf('"');
                        int secondQuote = line.IndexOf('"', firstQuote + 1);
                        string name = line.Substring(firstQuote + 1, secondQuote - firstQuote - 1);

                        var remainder = line.Substring(secondQuote + 1).Trim();
                        var parts = remainder.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        int messageId = int.Parse(parts[1]);
                        string signalName = parts[2];

                        string value;
                        int valueIndex = remainder.IndexOf(parts[3]);
                        if (parts[3].StartsWith("\""))
                        {
                            int vStart = remainder.IndexOf('"', valueIndex);
                            int vEnd = remainder.IndexOf('"', vStart + 1);
                            value = remainder.Substring(vStart + 1, vEnd - vStart - 1);
                        }
                        else
                        {
                            value = parts[3];
                        }

                        if (messagesById.TryGetValue(messageId, out Message? message))
                        {
                            Signal signal = message.Signals.First(s => s.Name == signalName);
                            if (signal != null)
                            {
                                signal.Attributes.Add(new SignalAttribute
                                {
                                    Name = name,
                                    Value = value
                                });
                            }
                        }
                    }
                }
            }
            
            return network;
        }
    }
}