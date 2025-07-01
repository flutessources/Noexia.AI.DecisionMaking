using Noexia.AI.DecisionMaking.TurnBased.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.AI.DecisionMaking.TurnBased.States
{
	public class CharacterState : ICharacteristics
	{
		private readonly CharacterData m_characterData;
		public readonly IEnumerable<AttackState> Attacks;
		public readonly IReadOnlyDictionary<int, AttackState> AttacksById;
		public int Id => m_characterData.id;
		public bool IsDead { get; private set; } = false;
		public int MaxMP => m_characterData.Characteristics.mp;
		public int MaxLP => m_characterData.Characteristics.lp;
		public int MaxAP => m_characterData.Characteristics.ap;

		#region Characteristics
		public int lp { get; private set; }
		public int mp { get; private set; }
		public int ap { get; private set; }

		public int range { get; private set; }

		public int fireResitances { get; private set; }

		public int waterResitances { get; private set; }

		public int earthResitances { get; private set; }

		public int airResitances { get; private set; }

		public int neutralResistances { get; private set; }

		public int fireDamages { get; private set; }

		public int waterDamages { get; private set; }

		public int earthDamages { get; private set; }

		public int airDamages { get; private set; }

		public int neutralDamages { get; private set; }

		public int firePercentDamages { get; private set; }

		public int waterPercentDamages { get; private set; }

		public int earthPercentDamages { get; private set; }

		public int airPercentDamages { get; private set; }

		public int neutralPercentDamages { get; private set; }
		#endregion

		public void UseAp(int a_value)
		{
			ap -= a_value;
		}

		public void UseMp(int a_value)
		{
			mp -= a_value;
		}

		public void TakeDamages(int a_value)
		{
			lp -= a_value;
			if (lp <= 0)
				IsDead = true;
		}

		public CharacterState(CharacterData a_characterData)
		{
			m_characterData = a_characterData;

			lp = a_characterData.Characteristics.lp;
			mp = a_characterData.Characteristics.mp;
			ap = a_characterData.Characteristics.ap;
			range = a_characterData.Characteristics.range;
			fireResitances = a_characterData.Characteristics.fireResitances;
			waterResitances = a_characterData.Characteristics.waterResitances;
			earthResitances = a_characterData.Characteristics.earthResitances;
			airResitances = a_characterData.Characteristics.airResitances;
			neutralResistances = a_characterData.Characteristics.neutralResistances;
			fireDamages = a_characterData.Characteristics.fireDamages;
			waterDamages = a_characterData.Characteristics.waterDamages;
			earthDamages = a_characterData.Characteristics.earthDamages;
			airDamages = a_characterData.Characteristics.airDamages;
			neutralDamages = a_characterData.Characteristics.neutralDamages;
			firePercentDamages = a_characterData.Characteristics.firePercentDamages;
			waterPercentDamages = a_characterData.Characteristics.waterPercentDamages;
			earthPercentDamages = a_characterData.Characteristics.earthPercentDamages;
			airPercentDamages = a_characterData.Characteristics.airPercentDamages;
			neutralPercentDamages = a_characterData.Characteristics.neutralPercentDamages;


			Attacks = a_characterData.Attacks.Select(a_attack => new AttackState(a_attack));
			AttacksById = Attacks.ToDictionary(a_attack => a_attack.Id, a_attack => a_attack);
		}

		public CharacterState(CharacterState a_clone)
		{
			m_characterData = a_clone.m_characterData;

			lp = a_clone.lp;
			mp = a_clone.mp;
			ap = a_clone.ap;
			range = a_clone.range;
			fireResitances = a_clone.fireResitances;
			waterResitances = a_clone.waterResitances;
			earthResitances = a_clone.earthResitances;
			airResitances = a_clone.airResitances;
			neutralResistances = a_clone.neutralResistances;
			fireDamages = a_clone.fireDamages;
			waterDamages = a_clone.waterDamages;
			earthDamages = a_clone.earthDamages;
			airDamages = a_clone.airDamages;
			neutralDamages = a_clone.neutralDamages;
			firePercentDamages = a_clone.firePercentDamages;
			waterPercentDamages = a_clone.waterPercentDamages;
			earthPercentDamages = a_clone.earthPercentDamages;
			airPercentDamages = a_clone.airPercentDamages;
			neutralPercentDamages = a_clone.neutralPercentDamages;

			List<AttackState> attacks = new List<AttackState>();
			Dictionary<int, AttackState> attacksById = new Dictionary<int, AttackState>();

			foreach (var attack in a_clone.Attacks)
			{
				AttackState state = attack.Clone();
				attacks.Add(state);
				attacksById[state.Id] = state;
			}

			Attacks = attacks;
			AttacksById = attacksById;
			IsDead = a_clone.IsDead;
		}

		public CharacterState Clone()
		{
			return new CharacterState(this);
		}
	}
}
