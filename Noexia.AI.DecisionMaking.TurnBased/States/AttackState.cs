using Noexia.AI.DecisionMaking.TurnBased.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.AI.DecisionMaking.TurnBased.States
{
	public class AttackState
	{
		private readonly AttackData m_attackData;
		private Dictionary<int, int> m_usePerTargets = new Dictionary<int, int>(); // id target, nb of uses
		private int m_usesPerTurn = 0; 

		public int Id => m_attackData.id;
		public int ApCost => m_attackData.apCost;
		public int RangeMin => m_attackData.rangeMin;
		public int RangeMax => m_attackData.rangeMax;
		public int UsesPerturn => m_usesPerTurn;

		public readonly IReadOnlyDictionary<EElement, int> TotalDamagesMin;
		public readonly IReadOnlyDictionary<EElement, int> TotalDamagesMax;
		public readonly IReadOnlyDictionary<EElement, int> TotalDamagesAverage;

		public readonly bool IsPushableAttack;
		public readonly int TotalPushDistance;

		public AttackData GetData => m_attackData;

		public AttackState(AttackData attackData)
		{
			m_attackData = attackData;

			Dictionary<EElement, int> totalDamagesMin = new Dictionary<EElement, int>();
			Dictionary<EElement, int> totalDamagesMax = new Dictionary<EElement, int>();
			Dictionary<EElement, int> totalDamagesAverage = new Dictionary<EElement, int>();

			m_usesPerTurn = m_attackData.usePerTurn;
			foreach (var effect in m_attackData.effects)
			{
				if (effect is AttackElementDamageEffect damageEffect)
				{
					if (totalDamagesMin.ContainsKey(damageEffect.element))
					{
						totalDamagesMin[damageEffect.element] += damageEffect.damagesMin;
					}
					else
					{
						totalDamagesMin[damageEffect.element] = damageEffect.damagesMin;
					}

					if (totalDamagesMax.ContainsKey(damageEffect.element))
					{
						totalDamagesMax[damageEffect.element] += damageEffect.damagesMax;
					}
					else
					{
						totalDamagesMax[damageEffect.element] = damageEffect.damagesMax;
					}

					if (totalDamagesAverage.ContainsKey(damageEffect.element))
					{
						totalDamagesAverage[damageEffect.element] += damageEffect.damagesAverage;
					}
					else
					{
						totalDamagesAverage[damageEffect.element] = damageEffect.damagesAverage;
					}
				}
				else if (effect is AttackPushEffect pushEffect)
				{
					IsPushableAttack = true;
					TotalPushDistance += pushEffect.pushDistance;
				}
			}

			TotalDamagesMin = totalDamagesMin;
			TotalDamagesMax = totalDamagesMax;
			TotalDamagesAverage = totalDamagesAverage;
		}

		public AttackState(AttackData a_attackData, Dictionary<int, int> a_usePerTarget, int a_usePerTurn,
			Dictionary<EElement, int> totalDamagesMin, Dictionary<EElement, int> totalDamagesMax, Dictionary<EElement, int> totalDamagesAverage,
			bool isPushable, int totalPushDistance)
		{
			m_attackData = a_attackData;
			m_usePerTargets = a_usePerTarget;
			m_usesPerTurn = a_usePerTurn;

			TotalDamagesMin = totalDamagesMin;
			TotalDamagesMax = totalDamagesMax;
			TotalDamagesAverage = totalDamagesAverage;

			IsPushableAttack = isPushable;
			TotalPushDistance = totalPushDistance;
		}

		public bool CanUse(CellState a_cellStart, CellState a_cellDest, MapState a_mapState, int aditionnalRange)
		{
			if (m_usesPerTurn <= 0 && m_attackData.usePerTurn != 0)
			{
				return false;
			}

			if (m_attackData.usePerTarget != 0)
			{
				if (a_cellDest.CharacterId != 0)
				{
					if (m_usePerTargets.ContainsKey(a_cellDest.CharacterId) && m_usePerTargets[a_cellDest.CharacterId] <= 0)
					{
						return false;
					}
				}
			}


			var range = Pathfinding.GetSpellRange(
				a_cellStart, m_attackData.rangeMin, m_attackData.rangeMax, m_attackData.isRangeBoostable,
				m_attackData.isRangeLine, m_attackData.needsFreeCell, m_attackData.isRangeNeedLineOfSight, a_mapState, aditionnalRange);
			// Line of sight check
			// Line launch check

			return range.Contains(a_cellDest.Id);
		}

		public void Use(int a_characterId)
		{
			m_usesPerTurn--;

			if (m_usePerTargets.ContainsKey(a_characterId))
			{
				m_usePerTargets[a_characterId]--;
			}
			else
			{
				m_usePerTargets.Add(a_characterId, m_attackData.usePerTarget - 1);
			}
		}

		public AttackState Clone()
		{
			return new AttackState(m_attackData, new Dictionary<int, int>(m_usePerTargets), m_usesPerTurn,
				TotalDamagesMin.ToDictionary(), TotalDamagesMax.ToDictionary(), TotalDamagesAverage.ToDictionary(), IsPushableAttack, TotalPushDistance);
		}
	}
}
