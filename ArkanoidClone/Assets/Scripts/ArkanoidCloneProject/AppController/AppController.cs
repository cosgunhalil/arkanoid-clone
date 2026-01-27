using ArkanoidCloneProject.Factories.StateFactory;
using ArkanoidProject.State;
using UnityEngine;
using VContainer;

namespace ArkanoidCloneProject.Controllers.Scripts
{
    public class AppController : MonoBehaviour
    {
        [Inject] private IStateFactory _stateFactory;
        private AppState _appState;

        void Start()
        {
            Debug.Log("AppController.Start()");
            _appState = _stateFactory.Create<AppState>();
            _appState.Enter();
        }

        void Update()
        {
            _appState.Update();
        }

        void OnDestroy()
        {
            _appState?.Exit();
        }
    }
}