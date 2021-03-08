using System;
using DryIoc;

namespace MoqqerNamespace
{
    public interface IMoqqerObjectContext
    {
        void ForAllImplementedInterfaces();
    }

    class MoqqerObjectContext: IMoqqerObjectContext
    {
        private readonly Moqqer _moqqer;
        private readonly object _implementation;
        private readonly Type _type;

        public MoqqerObjectContext(Moqqer moqqer, object implementation, Type type)
        {
            _moqqer = moqqer;
            _implementation = implementation;
            _type = type;
        }

        public void ForAllImplementedInterfaces()
        {
            var interfaces = _type.GetInterfaces();
            foreach (var type in interfaces)
            {
                _moqqer.Container.RegisterInstance(type, _implementation, IfAlreadyRegistered.Replace);
            }
        }
    }
}