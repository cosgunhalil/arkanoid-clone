using Devkit.HSM;

namespace ArkanoidCloneProject.Factories.StateFactory
{
    public interface IStateFactory
    {
        TState Create<TState>() where TState : StateMachine;
    }
}