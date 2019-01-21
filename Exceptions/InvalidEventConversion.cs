using System;
using System.Collections.Generic;
using System.Text;

namespace AzureEventhubProtocol.Exceptions
{
    public class InvalidEventConversion : Exception
    {
        public InvalidEventConversion(string reason) : base(reason) {}
    }
}
