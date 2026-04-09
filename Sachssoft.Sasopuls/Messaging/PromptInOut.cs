using System;
using System.Threading.Tasks;

namespace Sachssoft.Sasofly.Pulse.Messaging
{
    public sealed class PromptInOut<TIn, TOut> : PromptBase
    {
        private Func<TIn, Task<TOut>>? _handler;
        private Func<object?, TIn, Task<TOut>>? _handlerWithSender;
        private object? _sender;

        public PromptInOut() { }

        // ---------- Register ohne Sender ----------

        public void Register(Func<TIn, Task<TOut>> handler)
        {
            ThrowRegistrationException(handler);
            _handler = handler;
        }

        public void Register(Func<TIn, TOut> handler)
        {
            ThrowRegistrationException(handler);
            _handler = input => Task.FromResult(handler(input));
        }

        // ---------- Register mit Sender ----------

        public void Register(object? sender, Func<object?, TIn, Task<TOut>> handler)
        {
            ThrowRegistrationException(handler);
            _sender = sender;
            _handlerWithSender = handler;
        }

        public void Register(object? sender, Func<object?, TIn, TOut> handler)
        {
            ThrowRegistrationException(handler);
            _sender = sender;
            _handlerWithSender = (s, input) => Task.FromResult(handler(s, input));
        }

        // ---------- Execute ----------

        public Task<TOut> ExecuteAsync(TIn input)
        {
            if (_handlerWithSender != null)
                return _handlerWithSender(_sender, input);

            if (_handler != null)
                return _handler(input);

            return Task.FromException<TOut>(ThrowExecuteException());
        }
    }
}