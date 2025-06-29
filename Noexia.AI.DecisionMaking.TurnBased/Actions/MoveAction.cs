using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.AI.DecisionMaking.TurnBased.Actions
{
	public class MoveAction : IAction
	{
		public readonly int TargetX;
		public readonly int TargetY;

		public string Text { get; private set; }

		public MoveAction(int targetX, int targetY, string text)
		{
			TargetX = targetX;
			TargetY = targetY;
			Text = text;
		}

		public bool Execute()
		{
			// Logic for executing the attack action
			return true; // Return true if the action was successful
		}
	}
}