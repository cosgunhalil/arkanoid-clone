using ArkanoidCloneProject.Factories.StateFactory;
using ArkanoidCloneProject.GameStates.PrepareGameState;
using Devkit.HSM;
using VContainer;

namespace ArkanoidProject.State
{
    public class AppState : StateMachine
    {
        private IStateFactory _stateFactory;

        [Inject]
        public AppState(IStateFactory stateFactory)
        {
            _stateFactory = stateFactory;
        }

        protected override void OnEnter()
        {
            BuildHierarchy();
        }

        private void BuildHierarchy()
        {
            var mainMenuState = _stateFactory.Create<MainMenuState>();
            var prepareGameState = _stateFactory.Create<PrepareGameState>();
            var inGameState = _stateFactory.Create<InGameState>();
            var pauseState = _stateFactory.Create<PauseGameState>();
            var endGameState = _stateFactory.Create<EndGameState>();

            AddSubState(mainMenuState);
            AddSubState(prepareGameState);
            AddSubState(inGameState);
            AddSubState(pauseState);
            AddSubState(endGameState);

            AddTransition(mainMenuState, prepareGameState, (int)StateTriggers.START_GAME_REQUEST);
            AddTransition(prepareGameState, inGameState, (int)StateTriggers.PREPARE_COMPLETE);
            AddTransition(inGameState, pauseState, (int)StateTriggers.PAUSE_GAME_REQUEST);
            AddTransition(pauseState, inGameState, (int)StateTriggers.CONTINUE_GAME_REQUEST);
            AddTransition(inGameState, endGameState, (int)StateTriggers.GAME_OVER_REQUEST);
        }
    }
}