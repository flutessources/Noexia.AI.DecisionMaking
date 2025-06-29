using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.AI.DecisionMaking
{
	public class MiniMaxDecisionMaker<T> : DecisionMaker<T>
		where T : MomentaryState
	{
		public override T? MakeDecision(T a_state, ref int a_totalIterations, ref T a_bestState)
		{
			T? bestState = a_bestState;

			IEnumerable<IAction> actions = a_state.GetNextLegalActions();
            foreach (var action in actions)
            {
				T state = (T)a_state.Clone();
				state.Apply(action);
				a_totalIterations++;

				if (bestState == null || state.TotalScore > bestState.TotalScore ||
					(state.TotalScore == bestState.TotalScore && state.Actions.Count < bestState.Actions.Count))
				{
					bestState = state;
				}

				MakeDecision(state, ref a_totalIterations, ref bestState);
			}

			return bestState;
		}
	}
}
