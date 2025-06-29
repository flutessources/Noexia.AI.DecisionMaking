using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.AI.DecisionMaking
{
	public abstract class MomentaryState
	{
		protected List<IAction> m_actions = new();
		public IReadOnlyList<IAction> Actions => m_actions;
		public int TotalScore { get; protected set; } = 0;
		public int Score { get; protected set; } = 0;

		public abstract IEnumerable<IAction> GetNextLegalActions();
		public abstract bool Apply(IAction a_action);
		public abstract MomentaryState Clone();
	}
}
