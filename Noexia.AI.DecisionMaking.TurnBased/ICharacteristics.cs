using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.AI.DecisionMaking.TurnBased
{
	public interface ICharacteristics
	{
		public int lp { get; }
		public int mp { get; }
		public int ap { get; }
		public int range { get; }
		public int fireResitances { get; }
		public int waterResitances { get; }
		public int earthResitances { get; }
		public int airResitances { get; }
		public int neutralResistances { get; }
		public int fireDamages { get; }
		public int waterDamages { get; }
		public int earthDamages { get; }
		public int airDamages { get; }
		public int neutralDamages { get; }
		public int firePercentDamages { get; }
		public int waterPercentDamages { get; }
		public int earthPercentDamages { get; }
		public int airPercentDamages { get; }
		public int neutralPercentDamages { get; }
	}
}
