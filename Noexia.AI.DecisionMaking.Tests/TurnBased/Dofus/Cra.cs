using Noexia.AI.DecisionMaking.TurnBased;
using Noexia.AI.DecisionMaking.TurnBased.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.AI.DecisionMaking.Tests.TurnBased.Dofus
{
	public static class Cra
	{
		public static void Test()
		{
			Console.WriteLine("Création de la data ...");

			TurnBasedDecisionMaker turnBasedDecisionMaker = new TurnBasedDecisionMaker();

			CharacterData player = new CharacterData
			{
				id = 1,
				Characteristics = new CharacteristicsData()
				{
					airDamages = 150,
					earthDamages = 50,
					fireDamages = 0,
					waterDamages = 25,
					airResitances = 20,
					earthResitances = 10,
					fireResitances = 15,
					waterResitances = 5,
					neutralResistances = 0,
					lp = 100,
					mp = 3,
					ap = 6,
					range = 5
				},
				Attacks = new List<AttackData>
				{
					new AttackData
					{
						name = "Flèche de recul",
						id = 1,
						apCost = 3,
						isRangeBoostable = true,
						isRangeLine = true,
						needsFreeCell = false,
						isRangeNeedLineOfSight = false,
						rangeMin = 1,
						rangeMax = 2,
						usePerTarget = 1,
						usePerTurn = 2,
						effects = new List<AttackEffectData>
						{
							new AttackPushEffect
							{
								pushDistance = 3,
							},
							new AttackElementDamageEffect
							{
								damagesMin = 10,
								damagesMax = 20,
								damagesAverage = 15,
								element = EElement.Air
							}
						}
					},
					new AttackData
					{
						name = "Flèche explosive",
						id = 2,
						apCost = 5,
						isRangeBoostable = true,
						isRangeLine = false,
						isRangeNeedLineOfSight = true,
						needsFreeCell = false,
						rangeMin = 1,
						rangeMax = 10,
						usePerTarget = 2,
						usePerTurn = 2,
						effects = new List<AttackEffectData>
						{
							new AttackElementDamageEffect
							{
								damagesMin = 8,
								damagesMax = 15,
								damagesAverage = 12,
								element = EElement.Fire
							}
						}
					}
				}
			};

			List<CharacterData> ennemies = new List<CharacterData>()
			{
				{
					new CharacterData
					{
						id = 2,
						Characteristics = new CharacteristicsData()
						{
							lp = 50,
							mp = 3,
							ap = 5,
							range = 4,
							airResitances = 0,
							earthResitances = 0,
							fireResitances = 0,
							waterResitances = 0,
							neutralResistances = 0
						},
						Attacks = new List<AttackData>
						{
							new AttackData
							{
								name = "Attaque de base",
								id = 3,
								apCost = 3,
								isRangeBoostable = false,
								isRangeLine = false,
								isRangeNeedLineOfSight = false,
								needsFreeCell = false,
								rangeMin = 1,
								rangeMax = 2,
								usePerTarget = 1,
								usePerTurn = 3,
								effects = new List<AttackEffectData>
								{
									new AttackElementDamageEffect
									{
										damagesMin = 100,
										damagesMax = 500,
										damagesAverage = 300,
										element = EElement.Neutral
									}
								}
							}
						}
					}
				},
				//{
				//	new CharacterData
				//	{
				//		id = 4,
				//		Characteristics = new CharacteristicsData()
				//		{
				//			lp = 600,
				//			mp = 3,
				//			ap = 4,
				//			range = 3,
				//			airResitances = 5,
				//			earthResitances = 15,
				//			fireResitances = 20,
				//			waterResitances = 10,
				//			neutralResistances = 0
				//		},
				//		Attacks = new List<AttackData>
				//		{
				//			new AttackData
				//			{
				//				name = "Attaque de base",
				//				id = 5,
				//				apCost = 2,
				//				isRangeBoostable = false,
				//				isRangeLine = true,
				//				isRangeNeedLineOfSight = true,
				//				rangeMin = 1,
				//				rangeMax = 3,
				//				usePerTarget = 1,
				//				usePerTurn = 3,
				//				effects = new List<AttackEffectData>
				//				{
				//					new AttackElementDamageEffect
				//					{
				//						damagesMin = 8,
				//						damagesMax = 12,
				//						damagesAverage = 10,
				//						element = EElement.Neutral
				//					}
				//				}
				//			}
				//		}
				//	}
				//}
			};


			// L'id des cellules fonctionne ainsi :
			// La case la plus en haut à gauche commence par 0, et la plus en haut à droite à le plus grand id
			// l'ID des cases suivantes fonctionne ainsi :
			// right +14, Left -14, up +15, down -15
			List<CellData> cells = new List<CellData>();
			for (int i = 0; i < 15; i++)
			{
				for (int j = 0; j < 15; j++)
				{
					cells.Add(new CellData
					{
						id = i * 15 + j,
						x = j,
						y = i,
						isWalkable = true,
						isInteractive = false,
						blockVisionLine = false
					});
				}
			}

			//Player au centre de la map, et ennemi collé à lui
			int playerCellId = 0; // Cellule (7, 7)
			int enemyCellId = 10 * 15 + 10;

			int totalIterations = 0;

			Console.WriteLine("Data crée");
			Console.WriteLine("Décision making ...");
			GameState? state = turnBasedDecisionMaker.MakeDecision(player, playerCellId, 15, 15, ennemies, cells,
				new Dictionary<int, int>()
				{
					{ 2, enemyCellId },
					//{ 4, (2, 2) }
				},
				new WeightConfiguration(), ref totalIterations);
			Console.WriteLine($"Décision making terminé avec {state.Actions.Count} actions et un score total de {state.TotalScore}, pour un total de {totalIterations} itérations");

			foreach (var action in state.History)
			{
				Console.WriteLine($"Action: {action}");
			}
		}
	}
}
