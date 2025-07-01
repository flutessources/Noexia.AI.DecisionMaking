using Noexia.AI.DecisionMaking.TurnBased.Data;
using Noexia.AI.DecisionMaking.TurnBased.States;


namespace Noexia.AI.DecisionMaking.TurnBased
{
	public class TurnBasedDecisionMaker
	{
		private readonly TreeDecisionMaker<GameState> m_minimax;

		public TurnBasedDecisionMaker()
		{
			m_minimax = new TreeDecisionMaker<GameState>();
		}

		public GameState? MakeDecision(
			CharacterData a_player, int a_playerCellId, int a_mapWidth, int a_mapHeight,
			IEnumerable<CharacterData> a_ennemies, IEnumerable<CellData> a_cells,
			Dictionary<int, int> a_enemiesPositions, WeightConfiguration a_weightConfiguration,
			ref int a_totalIterations)
		{

			CharacterState playerState = new CharacterState(a_player);
			Dictionary<int, CharacterState> enemyStates = a_ennemies.ToDictionary(e => e.id, e => new CharacterState(e));
			Dictionary<int, CellState> cellsWithCharacters = new Dictionary<int, CellState>();
			Dictionary<int, CellState> cellStates = new ();
			foreach (var cellData in a_cells)
			{
				cellStates[cellData.id] = new CellState(cellData);
			}

			cellsWithCharacters.Add(a_player.id, cellStates[a_playerCellId]);
			foreach(var enemyPosition in a_enemiesPositions)
			{
				cellsWithCharacters.Add(enemyPosition.Key, cellStates[enemyPosition.Value]);
			}
			

			MapState mapState = new MapState(cellStates, cellsWithCharacters, a_mapWidth, a_mapHeight);

			GameState root = new GameState(playerState, enemyStates, mapState, a_weightConfiguration);
			root.FinalizeScore();
			GameState? best = null;
			
			return m_minimax.MakeDecision(root, ref a_totalIterations, ref best);
		}
	}
}
