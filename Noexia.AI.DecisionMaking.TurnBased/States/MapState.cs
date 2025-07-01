using Noexia.AI.DecisionMaking.TurnBased.Data;
using System.Reflection.Metadata.Ecma335;

namespace Noexia.AI.DecisionMaking.TurnBased.States
{
	public class MapState
	{
		private readonly Dictionary<int, CellState> m_cellsByIds = new();
		private readonly Dictionary<int, CellState> m_cellsWithCharacter = new();
		public readonly int Width;
		public readonly int Height;

		public MapState(Dictionary<int, CellState> a_cells, Dictionary<int, CellState> a_cellsWithCharacted, int a_width, int a_height, bool a_setCharacters = true)
		{
			Width = a_width;
			Height = a_height;

			m_cellsWithCharacter = a_cellsWithCharacted;

			if (a_setCharacters)
			{
				foreach (var cellWithCharacter in m_cellsWithCharacter)
				{
					cellWithCharacter.Value.SetCharacter(cellWithCharacter.Key);
				}
			}

			m_cellsByIds = a_cells;
		}

		public MapState Clone()
		{
			Dictionary<int, CellState> cellsWithCharacter = new Dictionary<int, CellState>();
			Dictionary<int, CellState> cellsByIds = new Dictionary<int, CellState>();

			foreach(var cell in m_cellsByIds)
			{
				cellsByIds.Add(cell.Key, cell.Value.Clone());
			}

			foreach (var cellWithCharacter in m_cellsWithCharacter)
			{
				// /!\ Important : On ne duplique pas les instances de CellState déjà clonés dans le tableau juste au dessus
				cellsWithCharacter.Add(cellWithCharacter.Key, cellsByIds[cellWithCharacter.Value.Id]);
			}


			return new MapState(cellsByIds, cellsWithCharacter, Width, Height, false);
		}

		public bool InsideBounds(int a_x, int a_y)
		{
			return m_cellsByIds.Any(cell => cell.Value.X == a_x && cell.Value.Y == a_y);
		}

		public void Move(int a_character, CellState a_cell)
		{
			a_cell.SetCharacter(a_character);

			if (m_cellsWithCharacter.ContainsKey(a_character))
			{
				CellState previousCell = m_cellsWithCharacter[a_character];
				previousCell.RemoveCharacter();
				m_cellsWithCharacter.Remove(a_character);
			}

			m_cellsWithCharacter.Add(a_character, a_cell);
		}

		public void RemoveCharacter(int a_characterId)
		{
			if (m_cellsWithCharacter.ContainsKey(a_characterId) == false)
			{
				throw new KeyNotFoundException($"No cell found for character with ID {a_characterId}.");
			}

			CellState cell = m_cellsWithCharacter[a_characterId];
			cell.RemoveCharacter();
			m_cellsWithCharacter.Remove(a_characterId);
		}

		public CellState GetCellWithCharacter(int a_characterId)
		{
			if (m_cellsWithCharacter.ContainsKey(a_characterId) == false)
			{
				throw new KeyNotFoundException($"No cell found for character with ID {a_characterId}.");
			}

			return m_cellsWithCharacter[a_characterId];
		}

		// Todo : éviter de l'utiliser
		public CellState GetCell(int a_x, int a_y)
		{
			var cell = m_cellsByIds.FirstOrDefault(cell => cell.Value.X == a_x && cell.Value.Y == a_y).Value;

			if (cell == null)
			{
				//foreach(var c in m_cellsByIds.Values)
				//{
				//	Console.WriteLine($"Cell ID: {c.Id}, X: {c.X}, Y: {c.Y}");
				//}

				//throw new KeyNotFoundException($"No cell found at coordinates ({a_x}, {a_y}). Cells count : " + m_cellsByIds.Count);
			}

			return cell;
		}

		public CellState? GetCell(int a_id)
		{
			if (m_cellsByIds.TryGetValue(a_id, out CellState? cell))
			{
				return cell;
			}

			return null;
		}

		public IEnumerable<CellState> GetNeighbors(int a_startId, bool a_walkable)
		{
			List<CellState> cells = new List<CellState>();

			if (m_cellsByIds.TryGetValue(a_startId + 14, out CellState cell))
			{
				if ((a_walkable == true && cell.IsWalkable) || a_walkable == false)
				{
					cells.Add(cell);
				}
			}

			if (m_cellsByIds.TryGetValue(a_startId - 14, out cell))
			{
				if ((a_walkable == true && cell.IsWalkable) || a_walkable == false)
				{
					cells.Add(cell);
				}
			}

			if (m_cellsByIds.TryGetValue(a_startId + 15, out cell))
			{
				if ((a_walkable == true && cell.IsWalkable) || a_walkable == false)
				{
					cells.Add(cell);
				}
			}

			if (m_cellsByIds.TryGetValue(a_startId - 15, out cell))
			{
				if ((a_walkable == true && cell.IsWalkable) || a_walkable == false)
				{
					cells.Add(cell);
				}
			}

			return cells;
		}
	}
}
