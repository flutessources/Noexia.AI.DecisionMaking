using Noexia.AI.DecisionMaking.TurnBased.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Noexia.AI.DecisionMaking.TurnBased
{
	public class Formulas
	{
		public struct ComputedDamagesResult
		{
			public int totalDamagesMin;
			public int totalDamagesAverage;
			public int totalDamagesMax;
		}

		public static ComputedDamagesResult ComputeDamages(AttackData attack, ICharacteristics attackerCharacs, ICharacteristics targetCharacs)
		{
			ComputedDamagesResult result = new ComputedDamagesResult
			{
				totalDamagesMin = 0,
				totalDamagesAverage = 0,
				totalDamagesMax = 0
			};

			foreach (var effect in attack.effects)
			{
				if (effect is AttackElementDamageEffect damageEffect)
				{
					int resistance = 0;
					int damageBoostPercents = 0;
					int damageBoost = 0;

					if (damageEffect.element == EElement.Fire)
					{
						resistance = targetCharacs.fireResitances;
						damageBoost = attackerCharacs.fireDamages;
						damageBoostPercents = attackerCharacs.firePercentDamages;
					}
					else if (damageEffect.element == EElement.Water)
					{
						resistance = targetCharacs.waterResitances;
						damageBoost = attackerCharacs.waterDamages;
						damageBoostPercents = attackerCharacs.waterPercentDamages;
					}
					else if (damageEffect.element == EElement.Earth)
					{
						resistance = targetCharacs.earthResitances;
						damageBoost = attackerCharacs.earthDamages;
						damageBoostPercents = attackerCharacs.earthPercentDamages;
					}
					else if (damageEffect.element == EElement.Air)
					{
						resistance = targetCharacs.airResitances;
						damageBoost = attackerCharacs.airDamages;
						damageBoostPercents = attackerCharacs.airPercentDamages;
					}
					else if (damageEffect.element == EElement.Neutral)
					{
						resistance = targetCharacs.neutralResistances;
						damageBoost = attackerCharacs.neutralDamages;
						damageBoostPercents = attackerCharacs.neutralPercentDamages;
					}

					int spellDamagesMin = damageEffect.damagesMin;
					int spellDamagesAverage = damageEffect.damagesAverage;
					int spellDamagesMax = damageEffect.damagesMax;

					spellDamagesMin = (int)((float)spellDamagesMin + ((float)spellDamagesMin * ((float)damageBoostPercents / 100.0f))) + damageBoost;
					spellDamagesMin -= (int)((float)spellDamagesMin * ((float)resistance / 100.0f));

					result.totalDamagesMin += spellDamagesMin;

					spellDamagesAverage = (int)((float)spellDamagesAverage + ((float)spellDamagesAverage * ((float)damageBoostPercents / 100.0f))) + damageBoost;
					spellDamagesAverage -= (int)((float)spellDamagesAverage * ((float)resistance / 100.0f));
					result.totalDamagesAverage += spellDamagesAverage;

					spellDamagesMax = (int)((float)spellDamagesMax + ((float)spellDamagesMax * ((float)damageBoostPercents / 100.0f))) + damageBoost;
					spellDamagesMax -= (int)((float)spellDamagesMax * ((float)resistance / 100.0f));
					result.totalDamagesMax += spellDamagesMax;
				}
			}

			return result;
		}
	}
}
