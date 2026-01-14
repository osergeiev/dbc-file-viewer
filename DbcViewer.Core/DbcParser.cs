using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    line = line.Trim();

                    if (line.StartsWith("BO_ "))
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

                        currentSignal = new Signal
                        {
                            Name = name,
                            StartBit = startBit,
                            Length = length,
                            Factor = factor,
                            Offset = offset,
                            Unit = unit
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

                        int valueStart = line.IndexOf('"', secondQuote + 1);
                        int valueEnd = line.IndexOf('"', valueStart + 1);
                        string value = line.Substring(valueStart + 1, valueEnd - valueStart - 1);

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