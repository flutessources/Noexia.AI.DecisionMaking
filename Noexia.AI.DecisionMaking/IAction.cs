using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.AI.DecisionMaking
{
	public interface IAction
	{
		public string Text { get; }
		public bool Execute();
	}
}
