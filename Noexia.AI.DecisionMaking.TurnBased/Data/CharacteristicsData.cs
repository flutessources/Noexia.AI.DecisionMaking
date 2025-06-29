using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.AI.DecisionMaking.TurnBased.Data
{
	public class CharacteristicsData : ICharacteristics
	{
		public int lp { get; set; }
		public int mp { get; set; }
		public int ap { get; set; }

		public int range { get; set; }

		public int fireResitances { get; set; }

		public int waterResitances { get; set; }

		public int earthResitances { get; set; }

		public int airResitances { get; set; }

		public int neutralResistances { get; set; }

		public int fireDamages { get; set; }

		public int waterDamages { get; set; }

		public int earthDamages { get; set; }

		public int airDamages { get; set; }

		public int neutralDamages { get; set; }

		public int firePercentDamages { get; set; }

		public int waterPercentDamages { get; set; }

		public int earthPercentDamages { get; set; }

		public int airPercentDamages { get; set; }

		public int neutralPercentDamages { get; set; }
	}
}
