using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbcViewer.Core.Models;

namespace DbcViewer.Core
{
    public interface IDbcParser
    {
        Network Parse(string filePath);
    }
}
