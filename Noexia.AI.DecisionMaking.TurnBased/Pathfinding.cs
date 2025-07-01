using Microsoft.VisualBasic;
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

		public static List<CellState> GetFarthestReachableCells(int a_mp, CellState start, MapState a_map)
		{
			// Aucun déplacement possible
			if (a_mp <= 0)
				return new List<CellState>();

			// (x,y) -> coût pour atteindre la case
			Dictionary<(int x, int y), int> visited = new();
			Queue<(CellState cell, int cost)> open = new();

			visited[(start.X, start.Y)] = 0;
			open.Enqueue((start, 0));

			int maxDistance = 0;
			List<CellState> farthest = new() { start };

			while (open.Count > 0)
			{
				var (current, cost) = open.Dequeue();

				// Si on vient de trouver une distance plus grande,
				// on réinitialise la liste des « plus éloignées »
				if (cost > maxDistance)
				{
					maxDistance = cost;
					farthest.Clear();
					farthest.Add(current);
				}
				else if (cost == maxDistance)
				{
					farthest.Add(current);
				}

				// On ne propage plus si on a déjà dépensé tous les PM
				if (cost == a_mp)
					continue;

				foreach (CellState neigh in a_map.GetNeighbors(current.Id, true))
				{
					// Case inaccessible : mur, obstacle ou occupée
					if (!neigh.IsWalkable || neigh.CharacterId != 0)
						continue;

					int nextCost = cost + 1;
					var key = (neigh.X, neigh.Y);

					// On mémorise uniquement le meilleur coût trouvé pour la case
					if (!visited.TryGetValue(key, out int prev) || nextCost < prev)
					{
						visited[key] = nextCost;
						open.Enqueue((neigh, nextCost));
					}
				}
			}

			// Souvent on ne veut pas inclure la case de départ si des PM>0
			if (a_mp > 0)
				farthest.RemoveAll(c => c.X == start.X && c.Y == start.Y);

			return farthest;
		}

		public static List<int> GetSpellRange(CellState characterCell, int minRange, int maxRange, bool modifiableRange, bool inLine, bool needsFreecell, bool needsLineOfSight, MapState map, int aditionnalRange)
		{
			List<int> range = new List<int>();

			foreach (CellState cell in SpellShape.GetCellsInSpellRange(characterCell, minRange, maxRange, modifiableRange, inLine, map, aditionnalRange))
			{
				if (cell == null || range.Contains(cell.Id))
					continue;

				if (needsFreecell && cell.CharacterId != 0)
					continue;

				if (cell.GetData().isWalkable && cell.GetData().isInteractive == false)
					range.Add(cell.Id);
			}

			if (needsLineOfSight)
			{
				for (int i = range.Count - 1; i >= 0; i--)
				{
					if (LineIsObstruct(characterCell, map.GetCell(range[i]), map))
						range.RemoveAt(i);
				}
			}

			return range;
		}


		public static bool LineIsObstruct(CellState startCell, CellState destCell, MapState map)
		{
			double x = startCell.X + 0.5;
			double y = startCell.Y + 0.5;
			double destX = destCell.X + 0.5;
			double destY = destCell.Y + 0.5;
			double prevX = startCell.X;
			double prevY = startCell.Y;

			double pad_x = 0;
			double pad_y = 0;
			double steps = 0;
			int type = 0;

			if (Math.Abs(x - destX) == Math.Abs(y - destY))
			{
				steps = Math.Abs(x - destX);
				pad_x = (destX > x) ? 1 : -1;
				pad_y = (destY > y) ? 1 : -1;
				type = 1;
			}
			else if (Math.Abs(x - destX) > Math.Abs(y - destY))
			{
				steps = Math.Abs(x - destX);
				pad_x = (destX > x) ? 1 : -1;
				pad_y = (destY - y) / steps;
				pad_y = pad_y * 100;
				pad_y = Math.Ceiling(pad_y) / 100;
				type = 2;
			}
			else
			{
				steps = Math.Abs(y - destY);
				pad_x = (destX - x) / steps;
				pad_x = pad_x * 100;
				pad_x = Math.Ceiling(pad_x) / 100;
				pad_y = (destY > y) ? 1 : -1;
				type = 3;
			}

			int error_superior = Convert.ToInt32(Math.Round(Math.Floor(Convert.ToDouble((3 + (steps / 2))))));
			int error_info = Convert.ToInt32(Math.Round(Math.Floor(Convert.ToDouble((97 - (steps / 2))))));

			for (int i = 0; i < steps; i++)
			{
				double cellX, cellY;
				double xPadX = x + pad_x;
				double yPadY = y + pad_y;

				switch (type)
				{
					case 2:
						double beforeY = Math.Ceiling(y * 100 + pad_y * 50) / 100;
						double afterY = Math.Floor(y * 100 + pad_y * 150) / 100;
						double diffBeforeCenterY = Math.Floor(Math.Abs(Math.Floor(beforeY) * 100 - beforeY * 100)) / 100;
						double diffCenterAfterY = Math.Ceiling(Math.Abs(Math.Ceiling(afterY) * 100 - afterY * 100)) / 100;

						cellX = Math.Floor(xPadX);

						if (Math.Floor(beforeY) == Math.Floor(afterY))
						{
							cellY = Math.Floor(yPadY);
							if ((beforeY == cellY && afterY < cellY) || (afterY == cellY && beforeY < cellY))
							{
								cellY = Math.Ceiling(yPadY);
							}
							if (CellIsObstruct(cellX, cellY, destCell.Id, prevX, prevY, map)) return true;
							prevX = cellX;
							prevY = cellY;
						}
						else if (Math.Ceiling(beforeY) == Math.Ceiling(afterY))
						{
							cellY = Math.Ceiling(yPadY);
							if ((beforeY == cellY && afterY < cellY) || (afterY == cellY && beforeY < cellY))
							{
								cellY = Math.Floor(yPadY);
							}

							if (CellIsObstruct(cellX, cellY, destCell.Id, prevX, prevY, map))
								return true;

							prevX = cellX;
							prevY = cellY;
						}
						else if (Math.Floor(diffBeforeCenterY * 100) <= error_superior)
						{
							if (CellIsObstruct(cellX, Math.Floor(afterY), destCell.Id, prevX, prevY, map)) return true;
							prevX = cellX;
							prevY = Math.Floor(afterY);
						}
						else if (Math.Floor(diffCenterAfterY * 100) >= error_info)
						{
							if (CellIsObstruct(cellX, Math.Floor(beforeY), destCell.Id, prevX, prevY, map)) return true;
							prevX = cellX;
							prevY = Math.Floor(beforeY);
						}
						else
						{
							if (CellIsObstruct(cellX, Math.Floor(beforeY), destCell.Id, prevX, prevY, map)) return true;
							prevX = cellX;
							prevY = Math.Floor(beforeY);
							if (CellIsObstruct(cellX, Math.Floor(afterY), destCell.Id, prevX, prevY, map)) return true;
							prevY = Math.Floor(afterY);
						}
						break;

					case 3:
						double beforeX = Math.Ceiling(x * 100 + pad_x * 50) / 100;
						double afterX = Math.Floor(x * 100 + pad_x * 150) / 100;
						double diffBeforeCenterX = Math.Floor(Math.Abs(Math.Floor(beforeX) * 100 - beforeX * 100)) / 100;
						double diffCenterAfterX = Math.Ceiling(Math.Abs(Math.Ceiling(afterX) * 100 - afterX * 100)) / 100;

						cellY = Math.Floor(yPadY);

						if (Math.Floor(beforeX) == Math.Floor(afterX))
						{
							cellX = Math.Floor(xPadX);
							if ((beforeX == cellX && afterX < cellX) || (afterX == cellX && beforeX < cellX))
							{
								cellX = Math.Ceiling(xPadX);
							}
							if (CellIsObstruct(cellX, cellY, destCell.Id, prevX, prevY, map))
								return true;
							prevX = cellX;
							prevY = cellY;
						}
						else if (Math.Ceiling(beforeX) == Math.Ceiling(afterX))
						{
							cellX = Math.Ceiling(xPadX);

							if ((beforeX == cellX && afterX < cellX) || (afterX == cellX && beforeX < cellX))
								cellX = Math.Floor(xPadX);

							if (CellIsObstruct(cellX, cellY, destCell.Id, prevX, prevY, map))
								return true;

							prevX = cellX;
							prevY = cellY;
						}
						else if (Math.Floor(diffBeforeCenterX * 100) <= error_superior)
						{
							if (CellIsObstruct(Math.Floor(afterX), cellY, destCell.Id, prevX, prevY, map))
								return true;

							prevX = Math.Floor(afterX);
							prevY = cellY;
						}
						else if (Math.Floor(diffCenterAfterX * 100) >= error_info)
						{
							if (CellIsObstruct(Math.Floor(beforeX), cellY, destCell.Id, prevX, prevY, map))
								return true;

							prevX = Math.Floor(beforeX);
							prevY = cellY;
						}
						else
						{
							if (CellIsObstruct(Math.Floor(beforeX), cellY, destCell.Id, prevX, prevY, map)) return true;
							prevX = Math.Floor(beforeX);
							prevY = cellY;
							if (CellIsObstruct(Math.Floor(afterX), cellY, destCell.Id, prevX, prevY, map)) return true;
							prevX = Math.Floor(afterX);
						}
						break;

					default:
						if (CellIsObstruct(Math.Floor(xPadX), Math.Floor(yPadY), destCell.Id, prevX, prevY, map))
							return true;
						prevX = Math.Floor(xPadX);
						prevY = Math.Floor(yPadY);
						break;
				}

				x = (x * 100 + pad_x * 100) / 100;
				y = (y * 100 + pad_y * 100) / 100;
			}
			return false;
		}

		public static bool CellIsObstruct(double x, double y, int targetCellId, double lastX, double lastY, MapState map)
		{
			CellState cell = map.GetCell((int)x, (int)y);

			return cell.BlockVisionLine || (cell.Id != targetCellId && cell.CharacterId != 0);
		}
	}

	public static class SpellShape
	{
		public static IEnumerable<CellState> GetCellsInSpellRange(CellState cell, int minRange, int maxRange, bool modifiableRange, bool isLine, MapState map, int aditionalRange = 0)
		{
			int maximumRange = maxRange + (modifiableRange ? aditionalRange : 0);

			if (isLine)
				return Shaper.Cross(cell, minRange, maximumRange, map);
			else
				return Shaper.Ring(cell.X, cell.Y, minRange, maximumRange, map);
		}
	}

	public static class Shaper
	{
		public static IEnumerable<CellState> Circle(int x, int y, int minRadius, int maxRadius, MapState map)
		{
			List<CellState> range = new List<CellState>();

			if (minRadius == 0)
				range.Add(map.GetCell(x, y));

			for (int radio = minRadius == 0 ? 1 : minRadius; radio <= maxRadius; radio++)
			{
				for (int i = 0; i < radio; i++)
				{
					int r = radio - i;
					range.Add(map.GetCell(x + i, y - r));
					range.Add(map.GetCell(x + r, y + i));
					range.Add(map.GetCell(x - i, y + r));
					range.Add(map.GetCell(x - r, y - i));
				}
			}

			return range.Where(c => c != null);
		}

		public static IEnumerable<CellState> Line(int x, int y, int minRadius, int maxRadius, MapState map)
		{
			List<CellState> range = new List<CellState>();

			for (int i = minRadius; i <= maxRadius; i++)
				range.Add(map.GetCell(x * i, y * i));

			return range.Where(c => c != null);
		}

		public static IEnumerable<CellState> Cross(CellState cell, int minRadius, int maxRadius, MapState map)
		{
			List<CellState> range = new List<CellState>();
			int startJ = minRadius;
			int stopJ = maxRadius - minRadius;

			if (minRadius == 0)
			{
				range.Add(cell);
				startJ++;
			}

			int step = 0;
			int start = cell.Id;

			for (int i = 0; i < 4; i++)
			{
				if (i == 0)
					step = 14;
				else if (i == 1)
					step = 15;
				else if (i == 2)
					step = -14;
				else if (i == 3)
					step = -15;

				for (int j = startJ; j < stopJ; j++)
				{
					int cellDest = start + (step * j);
					CellState? c = map.GetCell(cellDest);

					if (c != null)
					{
						range.Add(c);
					}
				}
			}

			return range;
		}

		public static IEnumerable<CellState> Ring(int x, int y, int minRadius, int maxRadius, MapState map)
		{
			List<CellState> range = new List<CellState>();

			if (minRadius == 0)
				range.Add(map.GetCell(x, y));

			for (int radius = minRadius == 0 ? 1 : minRadius; radius <= maxRadius; radius++)
			{
				for (int i = 0; i < radius; i++)
				{
					int r = radius - i;

					CellState? cell = map.GetCell(x + i, y - r);
					if (cell != null)
						range.Add(cell);

					cell = map.GetCell(x + r, y + i);
					if (cell != null)
						range.Add(cell);

					cell = map.GetCell(x - i, y + r);
					if (cell != null)
						range.Add(cell);

					cell = map.GetCell(x - r, y - i);
					if (cell != null)
						range.Add(cell);
				}
			}

			return range.Where(c => c != null);
		}
	}
}
