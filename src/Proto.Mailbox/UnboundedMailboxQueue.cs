﻿using System.Collections.Concurrent;

namespace Proto
{
    public class UnboundedMailboxQueue : IMailboxQueue
    {
        private readonly ConcurrentQueue<object> _messages = new ConcurrentQueue<object>();

        public void Push(object message)
        {
            _messages.Enqueue(message);
        }

        public object Pop()
        {
            object message;
            return _messages.TryDequeue(out message) ? message : null;
        }

        public bool HasMessages => _messages.Count > 0;
    }
}