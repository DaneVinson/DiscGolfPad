using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public class ErrorMessage
    {
        public ErrorMessage()
        { }

        public ErrorMessage(string propertyName, string message)
        {
            Message = message;
            PropertyName = propertyName;
        }

        public string Message { get; set; }
        public string PropertyName { get; set; }

        public override string ToString()
        {
            return $"{PropertyName}:{Message}";
        }
    }
}
