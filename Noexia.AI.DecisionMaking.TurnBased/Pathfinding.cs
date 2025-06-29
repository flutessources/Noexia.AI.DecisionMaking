using Noexia.AI.DecisionMaking.TurnBased.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Noexia.AI.DecisionMaking.TurnBased
{
	public static class Pathfinding
	{
		public struct PushResult
		{
			public CellState Destination;     // dernière case réellement atteinte
			public int CellsBlocked;    // nb de cases « manquantes » avant l’obstacle
		}

		public static PushResult GetPushDestination(CellState a_from, CellState a_pusher, MapState a_map, int a_distance)
		{
			int dirX = Math.Sign(a_from.X - a_pusher.X);
			int dirY = Math.Sign(a_from.Y - a_pusher.Y);

			if ((dirX == 1 && dirY == 0)
				|| (dirX == -1 && dirY == 0)
				|| (dirX == 0 && dirY == 1)
				|| (dirX == 0 && dirY == -1)
				== false)
			{
				return new PushResult { Destination = a_from, CellsBlocked = 0 }; // pas de diagonale
			}

			// find the last cell and return PushResult
			CellState currentCell = a_from;
			int cellsBlocked = 0;
			for (int i = 0; i < a_distance; i++)
			{
				int nextX = currentCell.X + dirX;
				int nextY = currentCell.Y + dirY;

				CellState nextCell = a_map.GetCell(nextX, nextY);

				if (nextCell.IsWalabke)
				{
					currentCell = nextCell;
				}
				else
				{
					cellsBlocked++;
					break; // out of bounds, stop pushing
				}
			}

			return new PushResult { Destination = currentCell, CellsBlocked = cellsBlocked };
		}
	}
}
