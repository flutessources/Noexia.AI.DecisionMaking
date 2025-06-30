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

		public static PushResult GetPushDestination(CellState from, CellState pusher, MapState map, int distance)
		{
			int dx = Math.Sign(from.X - pusher.X);
			int dy = Math.Sign(from.Y - pusher.Y);

			// Diagonales ou direction nulle interdites
			if ((dx != 0 && dy != 0) || (dx == 0 && dy == 0))
				return new PushResult { Destination = from, CellsBlocked = 0 };

			CellState lastFree = from;
			int blocked = 0;
			int dist = 0;

			for (int step = 0; step < distance; ++step)
			{
				int nx = lastFree.X + dx;
				int ny = lastFree.Y + dy;

				// 1. Hors carte → stop une case avant le bord
				if (!map.InsideBounds(nx, ny))
				{
					blocked++;
					break;
				}

				CellState next = map.GetCell(nx, ny);

				// 2. Obstacle ou personnage → stop juste avant l’impact
				if (!next.IsWalkable)
				{
					blocked++;
					break;
				}

				lastFree = next;                          // on avance
				dist++;
			}

			return new PushResult
			{
				Destination = lastFree,
				CellsBlocked = blocked
			};
		}
	}
}
