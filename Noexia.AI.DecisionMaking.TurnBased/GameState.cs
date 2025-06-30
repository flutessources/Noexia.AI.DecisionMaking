using Noexia.AI.DecisionMaking.TurnBased.Actions;
using Noexia.AI.DecisionMaking.TurnBased.States;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.AI.DecisionMaking.TurnBased
{
	public class GameState : MomentaryState
	{
		public readonly CharacterState Player;
		public readonly IReadOnlyDictionary<int, CharacterState> Ennemies;
		public readonly MapState Map;
		private List<string> m_history = new List<string>();

		private int m_totalEnnemiesHealth;
		private int m_totalEnnemiesAlive;
		private int m_receivedDamages;

		private WeightConfiguration m_weightConfiguration;
		public IEnumerable<string> History => m_history;

		public GameState(
			CharacterState a_player, IReadOnlyDictionary<int, CharacterState> a_ennemies, MapState a_map,
			WeightConfiguration a_weightConfiguration)
		{
			Player = a_player;
			Ennemies = a_ennemies;
			Map = a_map;
			m_weightConfiguration = a_weightConfiguration;

			m_totalEnnemiesHealth = Ennemies.Values.Sum(e => e.lp);
			m_totalEnnemiesAlive = Ennemies.Values.Count(e => e.lp > 0);
		}

		public GameState(
			CharacterState player, IReadOnlyDictionary<int, CharacterState> ennemies, MapState map,
			int totalEnnemiesHealth, int totalEnnemiesAlive, int a_receivedDamages, int totalScore, List<string> history, List<IAction> actions, WeightConfiguration weightConfiguration)
		{
			Player = player;
			m_actions = actions;
			Ennemies = ennemies;
			Map = map;
			TotalScore = totalScore;
			m_totalEnnemiesHealth = totalEnnemiesHealth;
			m_totalEnnemiesAlive = totalEnnemiesAlive;
			m_weightConfiguration = weightConfiguration;
			m_receivedDamages = a_receivedDamages;
			m_history = history ?? new List<string>();
		}

		public override IEnumerable<IAction> GetNextLegalActions()
		{
			List<IAction> actions = new List<IAction>();
			AppendNextMovesActions(ref actions);
			AppendNextAttackActions(ref actions);

			return actions;
		}

		protected virtual int AppendNextMovesActions(ref List<IAction> a_lst)
		{
			if (Player.mp <= 0)
				return 0;

			CellState currentCell = Map.GetCellWithCharacter(Player.Id);
			IEnumerable<CellState> neighbors = Map.GetNeighbors(currentCell.X, currentCell.Y, true);
			int added = 0;

			foreach (CellState neighbor in neighbors)
			{
				MoveAction moveAction = new MoveAction(neighbor.X, neighbor.Y,
					$"Move (from:{currentCell.X};{currentCell.Y}, to:{neighbor.X},{neighbor.Y})");
				a_lst.Add(moveAction);
				added++;
			}

			return added;
		}

		protected virtual int AppendNextAttackActions(ref List<IAction> a_lst)
		{
			if (Player.ap <= 0)
				return 0;

			int added = 0;

			foreach (var ennemiKvp in Ennemies)
			{
				var ennemi = ennemiKvp.Value;

				if (ennemi.lp <= 0)
					continue;

				foreach (var attack in Player.Attacks)
				{
					if (Player.ap < attack.ApCost)
						continue;

					if (attack.CanUse(ennemi.Id) == false)
						continue;

					CellState cell = Map.GetCellWithCharacter(ennemi.Id);
					AttackAction action = new AttackAction(cell.X, cell.Y, attack.Id,
						$"Attack : (targetHp:{ennemi.lp} spell:{attack.Id})");

					a_lst.Add(action);
					added++;
				}
			}

			return added;
		}

		public override bool Apply(IAction a_action)
		{
			// On incrémente le score uniquement lors de changemenents d'état
			// Par exemple, on ne va pas calculer les score de distance si aucun personnage n'a bouge, etc ...
			// Le but étant d'avoir un score représentatif de l'instant T

			int score = 0;

			m_history.Add("--Action--");
			m_history.Add($"Start with {TotalScore} score");

			if (a_action is MoveAction moveAction)
			{
				m_history.Add($"Player move to {moveAction.TargetX},{moveAction.TargetY}");

				CellState cellDest = Map.GetCell(moveAction.TargetX, moveAction.TargetY);
				Map.Move(Player.Id, cellDest);
				Player.UseMp(1);
			}
			else if (a_action is AttackAction attackAction)
			{
				m_history.Add($"Player attack {attackAction.AttackId} on {attackAction.X},{attackAction.Y}");

				AttackState attack = Player.AttacksById[attackAction.AttackId];
				CellState attackCellTarget = Map.GetCell(attackAction.X, attackAction.Y);
				CharacterState attackEnemyTarget = Ennemies[attackCellTarget.CharacterId];

				var damageResult = Formulas.ComputeDamages(attack.GetData, Player, attackEnemyTarget);

				Player.UseAp(attack.ApCost);
				attack.Use(attackEnemyTarget.Id);

				// Application des dommages:
				// On applique toujours avec la valeur mini
				// La valeur moyenne rentre tout de même dans le calcul de score dans le cas où on ne tue pas

				// Dans le cas où on ne tue pas :
				// On utilise les dégats moyens plutôt que les dégats minimum
				// On calcul ensuite morts potentiels avec les dégats moyens pour l'inclure dans le score
				if (attackEnemyTarget.lp > damageResult.totalDamagesMin)
				{
					m_history.Add($"Enemy {attackEnemyTarget.Id} takes {damageResult.totalDamagesMin} damages (min) and {damageResult.totalDamagesAverage} (average) from attack {attackAction.AttackId}");

					// On calcule le score avec les dégats moyens
					m_history.Add($"Score ({score}) += {ComputePotentialAveragesDamages(damageResult.totalDamagesAverage)}");
					score += ComputePotentialAveragesDamages(damageResult.totalDamagesAverage);

					// On calcul les morts potentiels
					if (attackEnemyTarget.lp <= damageResult.totalDamagesAverage)
					{
						m_history.Add($"Enemy {attackEnemyTarget.Id} is potentially killed by attack {attackAction.AttackId}");
						m_history.Add($"Score ({score}) += {ComputePotentialKillsScore(1)}");

						score += ComputePotentialKillsScore(1);
					}

					if (attack.IsPushableAttack)
					{
						CellState playerCell = Map.GetCellWithCharacter(Player.Id);
						Pathfinding.PushResult pushResult =
							Pathfinding.GetPushDestination(attackCellTarget, playerCell, Map, attack.TotalPushDistance);

						if (pushResult.Destination != attackCellTarget)
						{
							CellState destinationCell = pushResult.Destination;
							Map.Move(attackEnemyTarget.Id, destinationCell);

							m_history.Add($"Enemy {attackEnemyTarget.Id} is pushed to {destinationCell.X},{destinationCell.Y} by attack {attackAction.AttackId}");
						}					
					}

					// On appliques ls dégats minimaux pour ne pas fosser le reste de la simulation
					attackEnemyTarget.TakeDamages(damageResult.totalDamagesMin);
					m_totalEnnemiesHealth -= damageResult.totalDamagesMin;
				}
				else
				{
					m_history.Add($"Enemy {attackEnemyTarget.Id} is killed by attack {attackAction.AttackId}");

					int damages = damageResult.totalDamagesMin;
					attackEnemyTarget.TakeDamages(damages);
					damages += attackEnemyTarget.lp;

					Map.RemoveCharacter(attackEnemyTarget.Id);

					m_history.Add($"Enemy {attackEnemyTarget.Id} takes {damages} damages (min) and is killed by attack {attackAction.AttackId}");
					m_history.Add($"Score ({score}) += {ComputeKillsScore(1)}");
					score += ComputeKillsScore(1);

					m_history.Add($"Score ({score}) += {ComputeAveragesDamages(damages)}");
					score += ComputeAveragesDamages(damages);

					// On tue l'ennemi, donc on calcul la différence pour garder les dégats rééls (> 0)

					m_totalEnnemiesAlive--;
					m_totalEnnemiesHealth -= damages;
				}

				// Compute damages par PA utilisés
			}
			else
			{
				throw new InvalidOperationException($"Action type {a_action.GetType()} is not supported.");
			}

			m_actions.Add(a_action);

			Score = score;

			FinalizeScore();
			m_history.Add($"End with {TotalScore} ({TotalScore - Score} + {Score}) score (pa:{Player.ap}, pm:{Player.mp})");

			return true;
		}

		private int ComputeKillsScore(int a_kills)
		{
			float a = (float)((float)a_kills / (float)m_totalEnnemiesAlive);
			return (int)((a * 100f) * m_weightConfiguration.Kills);
		}

		private int ComputePotentialKillsScore(int a_kills)
		{
			float a = (float)((float)a_kills / (float)m_totalEnnemiesAlive);
			return (int)((a * 100f) * m_weightConfiguration.PotentialKills);
		}

		private int ComputePotentialAveragesDamages(int a_potentialAverageDamages)
		{
			float a = (float)((float)a_potentialAverageDamages / (float)m_totalEnnemiesHealth);
			return (int)((a * 100f) * m_weightConfiguration.PotentialAveragesDamages);
		}

		private int ComputeAveragesDamages(int a_damages)
		{
			float a = (float)((float)a_damages / (float)m_totalEnnemiesHealth);
			return (int)((a * 100f) * m_weightConfiguration.Damages);
		}

		private int ComputePotentialReceivedDamages(int a_damages)
		{
			float a = (float)a_damages / (float)Player.lp;
			return (int)((a * 100f) * m_weightConfiguration.PotentialReceivedDamages);
		}

		private int ComputeEnemiesPotentialAverageDamages()
		{
			CellState playerCell = Map.GetCellWithCharacter(Player.Id);
			int totalDamages = 0;

			foreach (var enemyKvp in Ennemies)
			{
				var enemy = enemyKvp.Value;
				if (enemy.IsDead || enemy.lp <= 0)
					continue;

				CellState enemyCell = Map.GetCellWithCharacter(enemy.Id);
				int distance = Math.Abs(enemyCell.X - playerCell.X) + Math.Abs(enemyCell.Y - playerCell.Y);

				int bestDamage = 0;
				foreach (var attack in enemy.Attacks)
				{
					if (enemy.ap < attack.ApCost)
						continue;

					if (attack.CanUse(Player.Id) == false)
						continue;

					int moveNeeded = 0;
					if (distance > attack.RangeMax)
						moveNeeded = distance - attack.RangeMax;
					else if (distance < attack.RangeMin)
						moveNeeded = attack.RangeMin - distance;

					if (moveNeeded > enemy.mp)
						continue;

					var dmg = Formulas.ComputeDamages(attack.GetData, enemy, Player);
					int uses = Math.Min(enemy.ap / attack.ApCost, attack.UsesPerturn);
					if (uses <= 0)
						continue;
					int potential = dmg.totalDamagesAverage * uses;
					if (potential > bestDamage)
						bestDamage = potential;
				}

				totalDamages += bestDamage;
			}

			return totalDamages;
		}

		public override void FinalizeScore()
		{
			int damages = ComputeEnemiesPotentialAverageDamages();
			int malus = ComputePotentialReceivedDamages(damages);
			int lastReceivedDamges = m_receivedDamages;
			m_receivedDamages = malus;

			int diff = malus - lastReceivedDamges;

			m_history.Add($"Score ({Score}) -= {diff} (potential received damages: {malus}, last received damages: {lastReceivedDamges})");
			Score -= diff;
			TotalScore += Score;

		}

		public override MomentaryState Clone()
		{
			return new GameState(
				Player.Clone(),
				Ennemies.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Clone()),
				Map.Clone(),
				m_totalEnnemiesHealth,
				m_totalEnnemiesAlive,
				m_receivedDamages,
				TotalScore,
				new List<string>(m_history),
				new List<IAction>(m_actions),
				m_weightConfiguration);
		}
	}
}
