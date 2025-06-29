using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.AI.DecisionMaking.TurnBased.Data
{
	public class AttackElementDamageEffect : AttackEffectData
	{
		public EElement element { get; set; }
		public int damagesMax { get; set; }
		public int damagesMin { get; set; }
		public int damagesAverage { get; set; }
	}
}
