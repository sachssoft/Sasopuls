using System;
using System.Threading.Tasks;

namespace Sachssoft.Sasopuls.Messaging
{
    public sealed class PromptOut<TOut> : PromptBase
    {
        private Func<Task<TOut>>? _handler;
        private Func<object?, Task<TOut>>? _handlerWithSender;
        private object? _sender;

        public PromptOut() { }

        // ---------- Register ohne Sender ----------

        public void Register(Func<Task<TOut>> handler)
        {
            ThrowRegistrationException(handler);
            _handler = handler;
        }

        public void Register(Func<TOut> handler)
        {
            ThrowRegistrationException(handler);
            _handler = () => Task.FromResult(handler());
        }

        // ---------- Register mit Sender ----------

        public void Register(object? sender, Func<object?, Task<TOut>> handler)
        {
            ThrowRegistrationException(handler);
            _sender = sender;
            _handlerWithSender = handler;
        }

        public void Register(object? sender, Func<object?, TOut> handler)
        {
            ThrowRegistrationException(handler);
            _sender = sender;
            _handlerWithSender = s => Task.FromResult(handler(s));
        }

        // ---------- Execute ----------

        public Task<TOut> ExecuteAsync()
        {
            if (_handlerWithSender != null)
                return _handlerWithSender(_sender);

            if (_handler != null)
                return _handler();

            return Task.FromException<TOut>(ThrowExecuteException());
        }
    }
}