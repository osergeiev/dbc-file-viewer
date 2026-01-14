using DbcViewer.Core;

namespace DbcViewer.Test
{
    public class DbcParserTests
    {
        [Fact]
        public void Parse_ShouldParseMessageSignalAndSignalAttributes()
        {
            string dbcContent = @"
                                BO_ 100 ExampleMessage: 8 ECU
                                 SG_ Speed : 0|16@1+ (0.01,0) [0|250] ""km/h"" ECU
                                BA_ ""GenSigComment"" SG_ 100 Speed ""Vehicle speed signal"" ;
                                BA_ ""Factor"" SG_ 100 Speed 0.01 ;
                                ";

            string tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile, dbcContent);

                var parser = new DbcParser();
                var network = parser.Parse(tempFile);

                Assert.NotNull(network);
                Assert.Single(network.Messages);

                var message = network.Messages.First();
                Assert.Equal(100, message.Id);
                Assert.Equal("ExampleMessage", message.Name);
                Assert.Single(message.Signals);

                var signal = message.Signals.First();
                Assert.Equal("Speed", signal.Name);
                Assert.Equal(0, signal.StartBit);
                Assert.Equal(16, signal.Length);
                Assert.Equal(0.01, signal.Factor);
                Assert.Equal(0, signal.Offset);
                Assert.Equal("km/h", signal.Unit);

                Assert.Equal(2, signal.Attributes.Count);

                var stringAttr = signal.Attributes.First(a => a.Name == "GenSigComment");
                Assert.Equal("Vehicle speed signal", stringAttr.Value);
                var numericAttr = signal.Attributes.First(a => a.Name == "Factor");
                Assert.Equal("0.01", numericAttr.Value);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}
