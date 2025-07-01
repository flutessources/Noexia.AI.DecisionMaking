using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.AI.DecisionMaking.TurnBased
{
	public class WeightConfiguration
	{
		public readonly float Kills = 1.35f;
		public readonly float Damages = 1f;
		public readonly float PotentialAveragesDamages = 0.6f;
		public readonly float PotentialKills = 0.85f;
		public readonly float PotentialReceivedDamages = 1.75f;
		public readonly int DistanceMin = 5;
		public readonly int DistanceMax = 8;
		public readonly int DistanceWeight = 1;
	}
}
