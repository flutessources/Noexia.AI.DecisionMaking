using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.AI.DecisionMaking
{
    public class TreeDecisionMaker<T> : DecisionMaker<T>
        where T : MomentaryState
    {
		public override T? MakeDecision(T state, ref int totalIterations, ref T bestState)
		{
			if (bestState == null) bestState = state;

			T? localBest = bestState;
			var actions = state.GetNextLegalActions().ToList();

			if (actions.Count == 0)            // base case: no more moves
				return localBest;

			foreach (var action in actions)
			{
				T child = (T)state.Clone();
				child.Apply(action);
				totalIterations++;

				if (localBest == null ||
					child.TotalScore > localBest.TotalScore ||
					(child.TotalScore == localBest.TotalScore &&
					 child.Actions.Count < localBest.Actions.Count))
				{
					localBest = child;
				}

				T? deeper = MakeDecision(child, ref totalIterations, ref localBest);
				if (deeper != null &&
					(localBest == null ||
					 deeper.TotalScore > localBest.TotalScore ||
					 (deeper.TotalScore == localBest.TotalScore &&
					  deeper.Actions.Count < localBest.Actions.Count)))
				{
					localBest = deeper;
				}
			}

			bestState = localBest ?? bestState;
			return localBest;
		}
	}
}
