using ArkanoidCloneProject.Factories.StateFactory;
using ArkanoidProject.State;
using VContainer.Unity;

namespace ArkanoidCloneProject.Controllers.Scripts
{
    public class AppControllerEntryPoint : IStartable, ITickable
    {
        private readonly IStateFactory _stateFactory;
        private AppState _appState;

        public AppControllerEntryPoint(IStateFactory stateFactory)
        {
            _stateFactory = stateFactory;
        }

        public void Start()
        {
            _appState = _stateFactory.Create<AppState>();
            _appState.Enter();
        }

        public void Tick()
        {
            _appState?.Update();
        }
    }
}
