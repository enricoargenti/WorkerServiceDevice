using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerServiceDevice;

internal class Packet
{
    public string Property { get; set; }
    public string Value { get; set; }

    public Packet(string property, string value) 
    {
        this.Property = property;
        this.Value = value;
    }
}
