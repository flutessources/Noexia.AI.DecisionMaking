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

                        var actions = a_state.GetNextLegalActions().ToList();

                        if (actions.Count == 0)
                        {
                                a_state.FinalizeState();

                                if (bestState == null || a_state.TotalScore > bestState.TotalScore ||
                                        (a_state.TotalScore == bestState.TotalScore && a_state.Actions.Count < bestState.Actions.Count))
                                {
                                        bestState = a_state;
                                }

                                a_bestState = bestState ?? a_bestState;
                                return bestState;
                        }

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

                                T? result = MakeDecision(state, ref a_totalIterations, ref bestState);
                                if (result != null && (bestState == null || result.TotalScore > bestState.TotalScore ||
                                                (result.TotalScore == bestState.TotalScore && result.Actions.Count < bestState.Actions.Count)))
                                {
                                        bestState = result;
                                }
                        }

                        a_bestState = bestState ?? a_bestState;
                        return bestState;
                }
	}
}
