namespace DbcViewer.Core.Models
{
    public class Network
    {
        public List<String> Nodes { get; } = new List<String>();
        public List<Message> Messages { get; } = new List<Message>();
    }
}
