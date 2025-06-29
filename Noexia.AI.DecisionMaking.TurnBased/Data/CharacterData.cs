using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.AI.DecisionMaking.TurnBased.Data
{
	public class CharacterData
	{
		public int id { get; set; }
		public CharacteristicsData Characteristics = new(); 
		public List<AttackData> Attacks = new();
	}
}
