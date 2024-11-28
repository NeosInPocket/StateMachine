using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

class StateMachine
{
    private IState _current;
    
    private readonly IState[] _states;
    private readonly Transition[] _transitions;
    private readonly Dictionary<Type, List<Transition>> _transitionsPerState;

    public StateMachine(IState[] states, Transition[] transitions)
    {
        _states = states;
        _transitions = transitions;
        _current = states[0];

        foreach (Transition transition in transitions)
        {
            if (_transitionsPerState.ContainsKey(transition.From))
                _transitionsPerState[transition.From] = new List<Transition>() { transition };
            else
                _transitionsPerState[transition.From].Add(transition);
        }
    }

    public void Update()
    {
        if (_current is IUpdateState updateState)
        {
            updateState.Update();
        }

        foreach (Transition transition in _transitionsPerState[_current.GetType()])
        {
            if (transition.From == _current.GetType() && transition.Condition())
            {
                TranslateTo(transition.To);
            }
        }
    }

    private void TranslateTo(Type targetType)
    {
        if (_current is IExitState exitState)
            exitState.Exit();
        
        _current = _states.First(x => x.GetType() == targetType);

        if (_current is IEnterState enterState)
        {
            enterState.Enter(); 
        }
    }
}