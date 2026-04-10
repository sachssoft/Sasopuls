using System;
using System.Threading.Tasks;

namespace Sachssoft.Sasopuls.Messaging
{
    public sealed class Prompt : PromptBase
    {
        private Func<Task>? _handler;
        private Func<object?, Task>? _handlerWithSender;
        private object? _sender;

        public Prompt() { }

        // ---------- Register ohne Sender ----------

        public void Register(Func<Task> handler)
        {
            ThrowRegistrationException(handler);
            _handler = handler;
        }

        public void Register(Action handler)
        {
            ThrowRegistrationException(handler);
            _handler = () =>
            {
                handler();
                return Task.CompletedTask;
            };
        }

        // ---------- Register mit Sender ----------

        public void Register(object? sender, Func<object?, Task> handler)
        {
            ThrowRegistrationException(handler);
            _sender = sender;
            _handlerWithSender = handler;
        }

        public void Register(object? sender, Action<object?> handler)
        {
            ThrowRegistrationException(handler);
            _sender = sender;
            _handlerWithSender = s =>
            {
                handler(s);
                return Task.CompletedTask;
            };
        }

        // ---------- Execute ----------

        public Task ExecuteAsync()
        {
            if (_handlerWithSender != null)
                return _handlerWithSender(_sender);

            if (_handler != null)
                return _handler();

            return Task.FromException(ThrowExecuteException());
        }
    }
}