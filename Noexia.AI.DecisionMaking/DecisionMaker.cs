using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.AI.DecisionMaking
{
	public abstract class DecisionMaker<T> where T : MomentaryState
	{
		public abstract T? MakeDecision(T a_state, ref int a_totalIterations, ref T? a_bestState);
	}
}
