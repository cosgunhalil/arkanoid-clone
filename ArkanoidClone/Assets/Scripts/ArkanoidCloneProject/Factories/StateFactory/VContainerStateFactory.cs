using Devkit.HSM;
using VContainer;

namespace ArkanoidCloneProject.Factories.StateFactory
{
    public class VContainerStateFactory : IStateFactory
    {
        private readonly IObjectResolver _resolver;

        [Inject]
        public VContainerStateFactory(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public TState Create<TState>() where TState : StateMachine
        {
            return _resolver.Resolve<TState>();
        }
    }
}