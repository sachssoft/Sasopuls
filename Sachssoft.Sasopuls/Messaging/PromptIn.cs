using System;
using System.Threading.Tasks;

namespace Sachssoft.Sasofly.Pulse.Messaging
{
    public sealed class PromptIn<TIn> : PromptBase
    {
        private Func<TIn, Task>? _handler;
        private Func<object?, TIn, Task>? _handlerWithSender;
        private object? _sender;

        public PromptIn() { }

        // ---------- Register ohne Sender ----------

        public void Register(Func<TIn, Task> handler)
        {
            ThrowRegistrationException(handler);
            _handler = handler;
        }

        public void Register(Action<TIn> handler)
        {
            ThrowRegistrationException(handler);
            _handler = input =>
            {
                handler(input);
                return Task.CompletedTask;
            };
        }

        // ---------- Register mit Sender ----------

        public void Register(object? sender, Func<object?, TIn, Task> handler)
        {
            ThrowRegistrationException(handler);
            _sender = sender;
            _handlerWithSender = handler;
        }

        public void Register(object? sender, Action<object?, TIn> handler)
        {
            ThrowRegistrationException(handler);
            _sender = sender;
            _handlerWithSender = (s, input) =>
            {
                handler(s, input);
                return Task.CompletedTask;
            };
        }

        // ---------- Execute ----------

        public Task ExecuteAsync(TIn input)
        {
            if (_handlerWithSender != null)
                return _handlerWithSender(_sender, input);

            if (_handler != null)
                return _handler(input);

            return Task.FromException(ThrowExecuteException());
        }
    }
}