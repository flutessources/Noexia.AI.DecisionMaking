using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.AI.DecisionMaking.TurnBased.Data
{
	public class AttackData
	{
		public string name { get; set; }
		public int id { get; set; }
		public int apCost { get; set; }
		public int rangeMin { get; set; }
		public int rangeMax { get; set; }
		public bool isRangeBoostable { get; set; }
		public bool isRangeNeedLineOfSight { get; set; }
		public bool needsFreeCell { get; set; } 
		public bool isRangeLine { get; set; }
		public List<AttackEffectData> effects { get; set; } = new List<AttackEffectData>();
		public int usePerTurn { get; set; }
		public int usePerTarget { get; set; }
	}
}
