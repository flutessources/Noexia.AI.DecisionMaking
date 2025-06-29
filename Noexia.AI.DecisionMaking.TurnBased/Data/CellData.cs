using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.AI.DecisionMaking.TurnBased.Data
{
	public class CellData
	{
		public int id { get; set; }
		public int x { get; set; }
		public int y { get; set; }
		public bool isWalkable { get; set; }
	}
}
