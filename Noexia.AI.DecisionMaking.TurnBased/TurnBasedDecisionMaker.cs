using Noexia.AI.DecisionMaking.TurnBased.Data;
using Noexia.AI.DecisionMaking.TurnBased.States;


namespace Noexia.AI.DecisionMaking.TurnBased
{
	public class TurnBasedDecisionMaker
	{
		private readonly MiniMaxDecisionMaker<GameState> m_minimax;

		public TurnBasedDecisionMaker()
		{
			m_minimax = new MiniMaxDecisionMaker<GameState>();
		}

		public GameState? MakeDecision(
			CharacterData a_player, int a_playerCellX, int a_playerCellY,
			IEnumerable<CharacterData> a_ennemies, CellData[,] a_cells,
			Dictionary<int, (int a_x, int a_y)> a_enemiesPositions, WeightConfiguration a_weightConfiguration,
			ref int a_totalIterations)
		{

			CharacterState playerState = new CharacterState(a_player);
			Dictionary<int, CharacterState> enemyStates = a_ennemies.ToDictionary(e => e.id, e => new CharacterState(e));
			Dictionary<int, CellState> cellsWithCharacters = new Dictionary<int, CellState>();
			CellState[,] cellStates = new CellState[a_cells.GetLength(0), a_cells.GetLength(1)];
			for (int i = 0; i < a_cells.GetLength(0); i++)
			{
				for (int j = 0; j < a_cells.GetLength(1); j++)
				{
					cellStates[i, j] = new CellState(a_cells[i, j]);
				}
			}
			
			cellsWithCharacters.Add(a_player.id, cellStates[a_playerCellX, a_playerCellY]);
			foreach(var enemyPosition in a_enemiesPositions)
			{
				cellsWithCharacters.Add(enemyPosition.Key, cellStates[enemyPosition.Value.a_x, enemyPosition.Value.a_y]);
			}
			

			MapState mapState = new MapState(cellStates, cellsWithCharacters, a_cells.GetLength(0), a_cells.GetLength(1));

			GameState root = new GameState(playerState, enemyStates, mapState, a_weightConfiguration);
			root.FinalizeScore();
			GameState? best = null;
			
			return m_minimax.MakeDecision(root, ref a_totalIterations, ref best);
		}
	}
}
