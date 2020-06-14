using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities.Types
{
    public class StateMachine<T>
    {
        struct Transition
        {
            public static bool operator ==(Transition l, Transition r)
            {
                return l.From.Equals(r.From) && l.To.Equals(r.To);
            }
            public static bool operator !=(Transition l, Transition r)
            {
                return !(l == r);
            }

            public readonly T From, To;
            public Transition(T from, T to)
            {
                From = from;
                To = to;
            }

            public override bool Equals(object obj)
            {
                if (obj is Transition == false)
                {
                    return false;
                }
                Transition t = (Transition)obj;
                return t.From.Equals(From) && t.To.Equals(To);
            }
            public override int GetHashCode()
            {
                return new { From, To }.GetHashCode();
            }
        }

        T _currState;
        List<Transition> _allowedTransitions = new List<Transition>();

        public T State { get => _currState; }

        public StateMachine(T initialState)
        {
            _currState = initialState;
        }

        public void AddAllowedTransition(T stateFrom, T stateTo)
        {
            _allowedTransitions.Add(new Transition(stateFrom, stateTo));
        }
        public void AddAllowedTransitions(IEnumerable<T> statesFrom, T stateTo)
        {
            foreach (T stateFrom in statesFrom)
            {
                _allowedTransitions.Add(new Transition(stateFrom, stateTo));
            }
        }
        public void AddAllowedTransitions(T stateFrom, IEnumerable<T> statesTo)
        {
            foreach (T stateTo in statesTo)
            {
                _allowedTransitions.Add(new Transition(stateFrom, stateTo));
            }
        }

        public void GoTo(T newState)
        {
            if (newState.Equals(_currState))
                return;

            if (IsTransitionAllowed(newState))
                _currState = newState;
            else
                throw new ArgumentOutOfRangeException("Forbidden transition");
        }

        public void ResetTo(T newState)
        {
            _currState = newState;
        }

        public bool IsTransitionAllowed(T stateTo)
        {
            Transition curr = new Transition(_currState, stateTo);
            return _allowedTransitions.Contains(curr);
        }
    }
}
