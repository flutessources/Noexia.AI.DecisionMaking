using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.AI.DecisionMaking.TurnBased.Actions
{
	public class MoveAction : IAction
	{
		public readonly int TargetId;

		public string Text { get; private set; }

		public MoveAction(int targetId, string text)
		{
			TargetId = targetId;
			Text = text;
		}

		public bool Execute()
		{
			// Logic for executing the attack action
			return true; // Return true if the action was successful
		}
	}
}