using DbcViewer.Core.Models;

namespace DbcViewer.Core
{
    public interface IDbcParser
    {
        Network Parse(string filePath);
    }
}
