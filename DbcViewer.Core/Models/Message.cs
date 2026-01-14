namespace DbcViewer.Core.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Length { get; set; }
        public string Transmitter { get; set; } = string.Empty;
        public List<Signal> Signals { get; } = new List<Signal>();
    }
}
