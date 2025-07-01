using Noexia.AI.DecisionMaking.TurnBased.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.AI.DecisionMaking.TurnBased.States
{
	public class CellState
	{
		private readonly CellData m_cellData;
		public int CharacterId { get; private set; } = 0;
		private bool m_isWalkable = true;
		public bool IsWalkable => m_cellData.isWalkable && m_cellData.isInteractive == false && m_isWalkable;
		public bool BlockVisionLine => m_cellData.blockVisionLine;
		public int X => m_cellData.x;
		public int Y => m_cellData.y;
		public int Id => m_cellData.id;

		public CellData GetData() => m_cellData;

		public CellState(CellData a_cellData)
		{
			m_cellData = a_cellData;
		}

		public CellState(CellData a_cellData, int a_characterId, bool a_isWalkable)
		{
			m_cellData = a_cellData;
			CharacterId = a_characterId;
			m_isWalkable = a_isWalkable;
		}

		public bool SetCharacter(int a_characterId)
		{
			if (CharacterId != 0)
			{
				throw new InvalidOperationException("Cell already has a character.");
			}

			CharacterId = a_characterId;
			m_isWalkable = false;

			return true;
		}

		public bool RemoveCharacter()
		{
			if (CharacterId == 0)
			{
				throw new InvalidOperationException("Cell does not have a character to remove.");
			}

			CharacterId = 0;
			m_isWalkable = true; // Cell is walkable again when no character is present

			return true;
		}

		public CellState Clone()
		{
			return new CellState(m_cellData, CharacterId, IsWalkable);
		}
	}
}
