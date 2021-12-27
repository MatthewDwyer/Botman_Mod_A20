using System;
using System.Runtime.Serialization;

namespace Botman
{
    [Serializable]
    internal class FriendlyMessageException : Exception
    {
        private object errorChunkCacheNotReady;

        public FriendlyMessageException()
        {
        }

        public FriendlyMessageException(object errorChunkCacheNotReady)
        {
            this.errorChunkCacheNotReady = errorChunkCacheNotReady;
        }

        public FriendlyMessageException(string message) : base(message)
        {
        }

        public FriendlyMessageException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FriendlyMessageException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}