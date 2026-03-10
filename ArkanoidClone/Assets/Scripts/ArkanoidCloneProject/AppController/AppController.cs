using ArkanoidCloneProject.Factories.StateFactory;
using ArkanoidProject.State;
using UnityEngine;
using VContainer;

namespace ArkanoidCloneProject.Controllers.Scripts
{
    public class AppController : MonoBehaviour
    {
        private AppState _appState;
        [Inject] private IStateFactory _stateFactory;

        private void Start()
        {
            Debug.Log("AppController.Start()");
            _appState = _stateFactory.Create<AppState>();
            _appState.Enter();
        }

        private void Update()
        {
            _appState.Update();
        }

        private void OnDestroy()
        {
            _appState?.Exit();
        }
    }
}