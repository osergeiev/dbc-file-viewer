namespace DbcViewer.Core.Models
{
    public class Signal
    {
        public string Name { get; set; } = string.Empty;
        public int StartBit { get; set; }
        public int Length { get; set; }
        public double Factor { get; set; }
        public double Offset { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string Receiver { get; set; } = string.Empty;
        public List<SignalAttribute> Attributes { get; } = new List<SignalAttribute>();
    }
}
