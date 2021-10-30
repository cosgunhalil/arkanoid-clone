namespace ArkanoidProject.State 
{
    using Devkit.Base.Component;
    using Devkit.HSM;
    using System;

    public class AppState : StateMachine
    {
        private MainMenuState mainMenuState;
        private InGameState inGameState;
        private EndGameState endGameState;
        private PauseGameState pauseGameState;

        public AppState(ComponentContainer componentContainer) 
        {
            mainMenuState = new MainMenuState();
            inGameState = new InGameState();
            endGameState = new EndGameState();
            pauseGameState = new PauseGameState();

            AddSubState(mainMenuState);
            AddSubState(inGameState);
            AddSubState(endGameState);
            AddSubState(pauseGameState);

            SetupMainMenuTransitions();
            SetupInGameTransitions();
            SetupEndGameTransitions();
            SetupPauseGameTransitions();
            SetupPauseGameTransitions();
        }

        private void SetupMainMenuTransitions()
        {
            mainMenuState.AddTransition(mainMenuState, inGameState, (int)StateTriggers.START_GAME_REQUEST);
        }

        private void SetupInGameTransitions()
        {
            inGameState.AddTransition(inGameState, endGameState, (int)StateTriggers.GAME_OVER_REQUEST);
            inGameState.AddTransition(inGameState, pauseGameState, (int)StateTriggers.PAUSE_GAME_REQUEST);
        }

        private void SetupEndGameTransitions()
        {
            endGameState.AddTransition(endGameState, mainMenuState, (int)StateTriggers.RETURN_TO_MAIN_MENU_REQUEST);
        }

        private void SetupPauseGameTransitions()
        {
            pauseGameState.AddTransition(pauseGameState, mainMenuState, (int)StateTriggers.RETURN_TO_MAIN_MENU_REQUEST);
            pauseGameState.AddTransition(pauseGameState, inGameState, (int)StateTriggers.CONTINUE_GAME_REQUEST);
        }

        protected override void OnEnter()
        {
            //TODO: handle
        }

        protected override void OnExit()
        {
            //TODO: handle
        }
    }
}

