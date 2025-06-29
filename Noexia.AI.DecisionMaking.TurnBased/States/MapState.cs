using Noexia.AI.DecisionMaking.TurnBased.Data;

namespace Noexia.AI.DecisionMaking.TurnBased.States
{
	public class MapState
	{
		private readonly CellState[,] m_cells;
		private readonly Dictionary<int, CellState> m_cellsWithCharacter = new();
		public readonly int Width;
		public readonly int Height;

		public MapState(CellState[,] a_cells, Dictionary<int, CellState> a_cellsWithCharacted, int a_width, int a_height, bool a_setCharacters = true)
		{
			m_cells = a_cells;
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
		}

		public MapState Clone()
		{
			CellState[,] cells = new CellState[m_cells.GetLength(0), m_cells.GetLength(1)];
			Dictionary<int, CellState> cellsWithCharacter = new Dictionary<int, CellState>();

			for (int i = 0; i < m_cells.GetLength(0); i++)
			{
				for (int j = 0; j < m_cells.GetLength(1); j++)
				{
					cells[i, j] = m_cells[i, j].Clone();
				}
			}

			foreach(var cellWithCharacter in m_cellsWithCharacter)
			{
				// /!\ Important : On ne duplique pas les instances de CellState déjà clonés dans le tableau juste au dessus
				cellsWithCharacter.Add(cellWithCharacter.Key, cells[cellWithCharacter.Value.X, cellWithCharacter.Value.Y]);
			}

			return new MapState(cells, cellsWithCharacter, Width, Height, false);
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

		public CellState GetCell(int a_x, int a_y)
		{
			if (a_x < 0 || a_x >= Width || a_y < 0 || a_y >= Height)
			{
				throw new ArgumentOutOfRangeException("Coordinates are out of bounds.");
			}

			return m_cells[a_x, a_y];
		}

		public IEnumerable<CellState> GetWalkableNeighbors(int a_startX, int a_startY)
			=> GetNeighbors(a_startX, a_startY, true);

		public IEnumerable<CellState> GetNeighbors(int a_startX, int a_startY, bool a_walkable)
		{
			List<CellState> cells = new List<CellState>();

			int left = a_startX - 1;
			if (left >= 0)
			{
                                CellState leftCell = m_cells[left, a_startY];
                                if (a_walkable == false || leftCell.IsWalkable)
				{
					cells.Add(m_cells[left, a_startY]);
				}
			}

			int right = a_startX + 1;
			if (right < Width)
			{
                                CellState rightCell = m_cells[right, a_startY];
                                if (a_walkable == false || rightCell.IsWalkable)
				{
					cells.Add(m_cells[right, a_startY]);
				}
			}

			int up = a_startY - 1;
			if (up >= 0)
			{
                                if (a_walkable == false || m_cells[a_startX, up].IsWalkable)
				{
					cells.Add(m_cells[a_startX, up]);
				}
			}

			int down = a_startY + 1;
			if (down < Height)
			{
                                if (a_walkable == false || m_cells[a_startX, down].IsWalkable)
				{
					cells.Add(m_cells[a_startX, down]);
				}
			}

			return cells;
		}
	}
}
