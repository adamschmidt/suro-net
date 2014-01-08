using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suro.Net
{
    [Serializable]
    public class SuroException : Exception
    {
        public SuroException() { }
        public SuroException(string message) : base(message) { }
        public SuroException(string message, Exception inner) : base(message, inner) { }
        protected SuroException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
