namespace Devkit.HSM
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class TransitionContext
    {
        public StateMachine FromState { get; set; }
        public StateMachine ToState { get; set; }
        public int Trigger { get; set; }
    }

    public class Transition
    {
        public StateMachine TargetState { get; set; }
        public Func<bool> Guard { get; set; }
        public Action OnTransition { get; set; }
    }

    public abstract class StateMachine
    {
        private Dictionary<Type, StateMachine> subStates = new Dictionary<Type, StateMachine>();
        private Dictionary<int, Transition> transitions = new Dictionary<int, Transition>();
        private Stack<StateMachine> stateHistory = new Stack<StateMachine>();

        private StateMachine parent;
        private StateMachine defaultSubState;
        private StateMachine currentSubState;
        private int maxHistorySize = 10;

        public StateMachine Parent => parent;
        public StateMachine CurrentSubState => currentSubState;
        public StateMachine DefaultSubState => defaultSubState;

        protected virtual void OnEnter() { }
        protected virtual void OnEnter(TransitionContext context) { OnEnter(); }
        protected virtual void OnUpdate() { }
        protected virtual void OnExit() { }
        protected virtual void OnExit(TransitionContext context) { OnExit(); }

        public void Enter()
        {
            Enter(null);
        }

        public void Enter(TransitionContext context)
        {
            OnEnter(context);

            if (currentSubState == null && defaultSubState != null)
            {
                currentSubState = defaultSubState;
            }

            if (currentSubState != null)
            {
                currentSubState.Enter(context);
            }
        }

        public void Update()
        {
            OnUpdate();

            if (currentSubState != null)
            {
                currentSubState.Update();
            }
        }

        public void Exit()
        {
            Exit(null);
        }

        public void Exit(TransitionContext context)
        {
            if (currentSubState != null)
            {
                currentSubState.Exit(context);
            }

            OnExit(context);
        }

        public void AddTransition(StateMachine sourceState, StateMachine targetState, int trigger, Func<bool> guard = null, Action onTransition = null)
        {
            if (sourceState.transitions.ContainsKey(trigger))
            {
                Debug.LogWarning("Duplicated transition! : " + trigger);
                return;
            }

            var transition = new Transition
            {
                TargetState = targetState,
                Guard = guard,
                OnTransition = onTransition
            };

            sourceState.transitions.Add(trigger, transition);
        }

        public void AddSubState(StateMachine subState)
        {
            if (subStates.Count == 0)
            {
                defaultSubState = subState;
            }

            subState.parent = this;

            if (subStates.ContainsKey(subState.GetType()))
            {
                Debug.LogWarning("Duplicated sub state : " + subState.GetType());
                return;
            }

            subStates.Add(subState.GetType(), subState);
        }

        public void SetDefaultState(StateMachine state)
        {
            if (!subStates.ContainsValue(state))
            {
                Debug.LogWarning("State is not a substate of this state machine");
                return;
            }

            defaultSubState = state;
        }

        public void SetMaxHistorySize(int size)
        {
            maxHistorySize = size;
        }

        public void SendTrigger(int trigger)
        {
            var root = GetRoot();
            StateMachine stateWithTransition = null;
            Transition transition = null;

            var current = root;
            while (current != null)
            {
                if (current.transitions.TryGetValue(trigger, out var t))
                {
                    stateWithTransition = current;
                    transition = t;
                }
                current = current.currentSubState;
            }

            if (stateWithTransition == null || transition == null)
            {
                return;
            }

            if (transition.Guard != null && !transition.Guard())
            {
                return;
            }

            var fromState = GetDeepestActiveState();
            var toState = transition.TargetState;

            var context = new TransitionContext
            {
                FromState = fromState,
                ToState = toState,
                Trigger = trigger
            };

            var commonAncestor = FindCommonAncestor(stateWithTransition, toState);

            var exitPath = BuildExitPath(stateWithTransition, commonAncestor);
            for (int i = 0; i < exitPath.Count; i++)
            {
                PushHistory(exitPath[i].currentSubState);
                exitPath[i].currentSubState?.Exit(context);
                exitPath[i].currentSubState = null;
            }

            transition.OnTransition?.Invoke();

            var enterPath = BuildEnterPath(toState, commonAncestor);
            for (int i = enterPath.Count - 1; i >= 0; i--)
            {
                var state = enterPath[i];
                var parentState = state.parent;

                if (parentState != null)
                {
                    parentState.currentSubState = state;
                }

                state.OnEnter(context);
            }

            var deepest = toState;
            while (deepest.defaultSubState != null)
            {
                deepest.currentSubState = deepest.defaultSubState;
                deepest.currentSubState.OnEnter(context);
                deepest = deepest.currentSubState;
            }
        }

        public void PopState()
        {
            if (stateHistory.Count == 0)
            {
                return;
            }

            var previousState = stateHistory.Pop();
            if (previousState == null)
            {
                return;
            }

            var fromState = GetDeepestActiveState();
            var context = new TransitionContext
            {
                FromState = fromState,
                ToState = previousState,
                Trigger = -1
            };

            var commonAncestor = FindCommonAncestor(this, previousState);

            var current = GetDeepestActiveState();
            while (current != null && current != commonAncestor)
            {
                current.OnExit(context);
                if (current.parent != null)
                {
                    current.parent.currentSubState = null;
                }
                current = current.parent;
            }

            var enterPath = BuildEnterPath(previousState, commonAncestor);
            for (int i = enterPath.Count - 1; i >= 0; i--)
            {
                var state = enterPath[i];
                if (state.parent != null)
                {
                    state.parent.currentSubState = state;
                }
                state.OnEnter(context);
            }
        }

        public bool IsInState<T>() where T : StateMachine
        {
            var current = this;
            while (current != null)
            {
                if (current is T)
                {
                    return true;
                }
                current = current.currentSubState;
            }
            return false;
        }

        public bool IsInState(StateMachine state)
        {
            var current = this;
            while (current != null)
            {
                if (current == state)
                {
                    return true;
                }
                current = current.currentSubState;
            }
            return false;
        }

        public T GetState<T>() where T : StateMachine
        {
            if (subStates.TryGetValue(typeof(T), out var state))
            {
                return state as T;
            }
            return null;
        }

        public List<StateMachine> GetCurrentStatePath()
        {
            var path = new List<StateMachine>();
            var root = GetRoot();
            var current = root;

            while (current != null)
            {
                path.Add(current);
                current = current.currentSubState;
            }

            return path;
        }

        public StateMachine GetDeepestActiveState()
        {
            var current = this;
            while (current.currentSubState != null)
            {
                current = current.currentSubState;
            }
            return current;
        }

        public StateMachine GetRoot()
        {
            var root = this;
            while (root.parent != null)
            {
                root = root.parent;
            }
            return root;
        }

        public void SetToDefaultState()
        {
            if (defaultSubState == null)
            {
                return;
            }

            var context = new TransitionContext
            {
                FromState = currentSubState,
                ToState = defaultSubState,
                Trigger = -1
            };

            if (currentSubState != null)
            {
                currentSubState.Exit(context);
            }

            currentSubState = defaultSubState;
            currentSubState.Enter(context);
        }

        private void PushHistory(StateMachine state)
        {
            if (state == null)
            {
                return;
            }

            stateHistory.Push(state);

            while (stateHistory.Count > maxHistorySize)
            {
                var tempStack = new Stack<StateMachine>();
                for (int i = 0; i < maxHistorySize; i++)
                {
                    tempStack.Push(stateHistory.Pop());
                }
                stateHistory.Clear();
                while (tempStack.Count > 0)
                {
                    stateHistory.Push(tempStack.Pop());
                }
            }
        }

        private StateMachine FindCommonAncestor(StateMachine stateA, StateMachine stateB)
        {
            var ancestorsA = new HashSet<StateMachine>();
            var current = stateA;

            while (current != null)
            {
                ancestorsA.Add(current);
                current = current.parent;
            }

            current = stateB;
            while (current != null)
            {
                if (ancestorsA.Contains(current))
                {
                    return current;
                }
                current = current.parent;
            }

            return null;
        }

        private List<StateMachine> BuildExitPath(StateMachine from, StateMachine toAncestor)
        {
            var path = new List<StateMachine>();
            var current = from;

            while (current != null && current != toAncestor)
            {
                path.Add(current);
                current = current.parent;
            }

            return path;
        }

        private List<StateMachine> BuildEnterPath(StateMachine target, StateMachine fromAncestor)
        {
            var path = new List<StateMachine>();
            var current = target;

            while (current != null && current != fromAncestor)
            {
                path.Add(current);
                current = current.parent;
            }

            return path;
        }
    }
}