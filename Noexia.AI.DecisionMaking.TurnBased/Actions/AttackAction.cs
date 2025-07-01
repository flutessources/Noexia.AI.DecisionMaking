using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.AI.DecisionMaking.TurnBased.Actions
{
	public class AttackAction : IAction
	{
		public readonly int CellId;
		public readonly int AttackId;
		public string Text { get; private set; }

		public AttackAction(int cellId, int attackId, string a_text)
		{
			CellId = cellId;
			AttackId = attackId;
			Text = a_text;
		}

		public bool Execute()
		{
			// Logic for executing the attack action
			return true; // Return true if the action was successful
		}
	}
}
