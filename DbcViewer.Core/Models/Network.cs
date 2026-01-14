using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbcViewer.Core.Models
{
    public class Network
    {
        public List<Message> Messages { get; } = new List<Message>();
    }
}
