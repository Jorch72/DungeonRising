using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C5;
namespace DungeonRising
{
    [Serializable]
    public class Dijkstra
    {
        public static int[,] PhysicalMap;
        [NonSerialized]
        public int[,] CombinedMap;
        public static int Height, Width;
        public ArrayList<Position> Path;
        public static int[][] DirShuffledY = new int[][]
            {
                new int[]{ -1, 0, 1, 0},
                new int[]{ -1, 0, 0, 1},
                new int[]{ -1, 1, 0, 0},
                new int[]{ -1, 0, 0, 1},
                new int[]{ -1, 1, 0, 0},
                new int[]{ -1, 0, 1, 0},
                
                new int[]{ 1, 0, -1, 0},
                new int[]{ 1, 0, 0, -1},
                new int[]{ 1, -1, 0, 0},
                new int[]{ 1, 0, 0, -1},
                new int[]{ 1, -1, 0, 0},
                new int[]{ 1, 0, -1, 0},
                
                new int[]{ 0, 1, -1, 0},
                new int[]{ 0, 1, 0, -1},
                new int[]{ 0, -1, 1, 0},
                new int[]{ 0, 0, 1, -1},
                new int[]{ 0, -1, 0, 1},
                new int[]{ 0, 0, -1, 1},
                
                new int[]{ 0, 1, -1, 0},
                new int[]{ 0, 1, 0, -1},
                new int[]{ 0, -1, 1, 0},
                new int[]{ 0, 0, 1, -1},
                new int[]{ 0, -1, 0, 1},
                new int[]{ 0, 0, -1, 1},
                
            }, DirShuffledX = new int[][]
            {
                new int[]{ 0, 1, 0, -1},
                new int[]{ 0, 1, -1, 0},
                new int[]{ 0, 0, 1, -1},
                new int[]{ 0, -1, 1, 0},
                new int[]{ 0, 0, -1, 1},
                new int[]{ 0, -1, 0, 1},
                
                new int[]{ 0, 1, 0, -1},
                new int[]{ 0, 1, -1, 0},
                new int[]{ 0, 0, 1, -1},
                new int[]{ 0, -1, 1, 0},
                new int[]{ 0, 0, -1, 1},
                new int[]{ 0, -1, 0, 1},
                
                new int[]{ 1, 0, 0, -1},
                new int[]{ 1, 0, -1, 0},
                new int[]{ 1, 0, 0, -1},
                new int[]{ 1, -1, 0, 0},
                new int[]{ 1, 0, -1, 0},
                new int[]{ 1, -1, 0, 0},
                
                new int[]{ -1, 0, 0, 1},
                new int[]{ -1, 0, 1, 0},
                new int[]{ -1, 0, 0, 1},
                new int[]{ -1, 1, 0, 0},
                new int[]{ -1, 0, 1, 0},
                new int[]{ -1, 1, 0, 0},
                
            };
        private const int GOAL = 0;
        public HashDictionary<int, int> goals, allies, obstacles;
        private HashDictionary<int, int> fresh, closed, open;
        public static XSRandom Rand;
        private static int frustration = 0;

        public Dijkstra()
        {
            Rand = new XSRandom();
            Path = new ArrayList<Position>();

            goals = new HashDictionary<int, int>();
            fresh = new HashDictionary<int, int>();
            allies = new HashDictionary<int, int>();
            obstacles = new HashDictionary<int, int>();
            closed = new HashDictionary<int, int>();
            open = new HashDictionary<int, int>();
        }
        public Dijkstra(int[,] level)
        {
            Rand = new XSRandom();
            CombinedMap = level.Replicate();
            PhysicalMap = level.Replicate();
            Path = new ArrayList<Position>();
            
            Height = PhysicalMap.GetLength(0);
            Width = PhysicalMap.GetLength(1);
            goals = new HashDictionary<int, int>();
            fresh = new HashDictionary<int, int>();
            allies = new HashDictionary<int, int>();
            obstacles = new HashDictionary<int, int>();
            closed = new HashDictionary<int, int>();
            open = new HashDictionary<int, int>();
        }
        public void Reset()
        {
            CombinedMap = PhysicalMap.Replicate();
            goals.Clear();
            Path.Clear();
        }
        public void SetGoal(int y, int x)
        {
            if (PhysicalMap[y, x] > Dungeon.FLOOR)
            {
                return;
            }
            if (allies.Contains(y * Width + x))
                allies.Remove(y * Width + x);
            if (obstacles.Contains(y * Width + x))
                obstacles.Remove(y * Width + x);
            goals[y * Width + x] = GOAL;
        }
        public void SetOccupied(int y, int x)
        {
            CombinedMap[y, x] = Dungeon.WALL;
        }
        public void ResetCell(int y, int x)
        {
            CombinedMap[y, x] = PhysicalMap[y, x];
        }
        public void RemoveAlly(Position pos)
        {
            if (allies.Contains(pos.Y * Width + pos.X))
            {
                allies.Remove(pos.Y * Width + pos.X);
            }
        }
        public void UpdateAlly(Position start, Position end)
        {
            if (allies.Contains(start.Y * Width + start.X))
            {
                allies.Remove(start.Y * Width + start.X);
            }
            allies[end.Y * Width + end.X] = Dungeon.WALL;
        }
        public void AddAlly(Position pos)
        {
            allies[pos.Y * Width + pos.X] = Dungeon.WALL;
        }
        public void AddAlly(IEnumerable<Position> friends)
        {
            foreach (Position pos in friends)
            {
                allies[pos.Y * Width + pos.X] = Dungeon.WALL;
            }
        }
        public void RemoveObstacle(Position pos)
        {
            if (obstacles.Contains(pos.Y * Width + pos.X))
            {
                obstacles.Remove(pos.Y * Width + pos.X);
            }
        }
        public void RemoveObstacles(IEnumerable<Position> blocks)
        {
            foreach (Position pos in blocks)
            {
                obstacles.Remove(pos.Y * Width + pos.X);
            }
        }
        public void UpdateObstacle(Position start, Position end)
        {
            if (obstacles.Contains(start.Y * Width + start.X))
            {
                obstacles.Remove(start.Y * Width + start.X);
            }
            obstacles[end.Y * Width + end.X] = Dungeon.WALL;
        }
        public ArrayList<Position> AdjacentToObstacle(Position pos)
        {
            ArrayList<Position> adjacent = new ArrayList<Position>();
            if (obstacles.Contains((pos.Y) * Width + (pos.X + 1)))
                adjacent.Add(new Position(pos.Y, pos.X + 1));
            if (obstacles.Contains((pos.Y) * Width + (pos.X - 1)))
                adjacent.Add(new Position(pos.Y, pos.X - 1));
            if (obstacles.Contains((pos.Y + 1) * Width + (pos.X)))
                adjacent.Add(new Position(pos.Y + 1, pos.X));
            if (obstacles.Contains((pos.Y - 1) * Width + (pos.X)))
                adjacent.Add(new Position(pos.Y - 1, pos.X));

            return adjacent;
        }
        public void AddObstacle(Position pos)
        {
            obstacles[pos.Y * Width + pos.X] = Dungeon.WALL;
        }
        public void AddObstacles(IEnumerable<Position> blocks)
        {
            foreach (Position pos in blocks)
            {
                obstacles[pos.Y * Width + pos.X] = Dungeon.WALL;
            }
        }
        public void SetObstacles(HashDictionary<int, int> other)
        {
            obstacles = other.Replicate();
        }
        public void SetObstacles(IEnumerable<Position> blocks)
        {
            obstacles.Clear();
            foreach (Position pos in blocks)
            {
                obstacles[pos.Y * Width + pos.X] = Dungeon.WALL;
            }
        }
        protected void SetFresh(int y, int x, int counter)
        {
            CombinedMap[y, x] = counter;
            fresh.Add(y * Width + x, counter);
        }
        protected void SetFresh(int index, int counter)
        {
            CombinedMap[index / Width, index % Width] = counter;
            fresh.Add(index, counter);
        }
        public int[,] Scan()
        {
            CombinedMap = PhysicalMap.Replicate();
            closed.UpdateAll(obstacles);
//            closed.AddAll(allies);
            for(int y = 0; y < Height; y++)
            {
                for(int x = 0; x < Width; x++)
                {
                    if (CombinedMap[y, x] > Dungeon.FLOOR)
                        closed[y * Width + x] = PhysicalMap[y, x];
                }
            }
            foreach(var kv in goals)
            {
                CombinedMap[kv.Key / Width, kv.Key % Width] = GOAL;
            }
            int numAssigned = goals.Count;
            int iter = 0;
            open.AddAll(goals);
            int[] dirs = { -Width, 1, Width, -1 };
            while(numAssigned > 0)
            {
                ++iter;
                numAssigned = 0;

                foreach (var cell in open)
                {
                    for (int d = 0; d < 4; d++)
                    {
                        if (!closed.ContainsKey(cell.Key + dirs[d]) && !open.ContainsKey(cell.Key + dirs[d]) && CombinedMap.GetIndex(cell.Key, Width) + 1 < CombinedMap.GetIndex(cell.Key + dirs[d], Width))
                        {
                            SetFresh(cell.Key + dirs[d], iter);
                            ++numAssigned;
                        }
                    }
                }
                closed.AddAll(open);
                open = fresh.Replicate();
                fresh.Clear();
            }
            closed.Clear();
            open.Clear();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if(CombinedMap[y,x] == Dungeon.FLOOR)
                    {
                        CombinedMap[y, x] = Dungeon.DARK;
                    }
                }
            }

            return CombinedMap;
        }

        public ArrayList<Position> GetPath(Position start, Position goal, int length)
        {
            Path = new ArrayList<Position>();
            if (CombinedMap[goal.Y, goal.X] > Dungeon.FLOOR)
            {
                return Path;
            }
            SetGoal(goal.Y, goal.X);
            Scan();
            Position currentPos = start;
//            Path.Add(currentPos);
            while (CombinedMap[currentPos.Y, currentPos.X] > 0)
            {
                if (frustration > 500)
                {
                    Path = new ArrayList<Position>();
                    frustration = 0;
                    foreach (var kv in goals)
                    {
                        ResetCell(kv.Key / Width, kv.Key % Width);
                    }
                    goals.Clear();
                    return Path;
                } int best = 9999, choice = 0, whichOrder = Rand.Next(24);
                int[] dirsY = DirShuffledY[whichOrder], dirsX = DirShuffledX[whichOrder];
                for (int d = 0; d < 4; d++)
                {
                    if (CombinedMap[currentPos.Y + dirsY[d], currentPos.X + dirsX[d]] < best)
                    {
                        best = CombinedMap[currentPos.Y + dirsY[d], currentPos.X + dirsX[d]];
                        choice = d;
                    }
                }
                if(best >= 9999)
                {
                    frustration = 0;
                    Path = new ArrayList<Position>();
                    return Path;
                }
                currentPos.Y += dirsY[choice];
                currentPos.X += dirsX[choice];
                Path.Add(currentPos);
                frustration++;
                if (Path.Count >= length)
                {
                    if(allies.Contains(currentPos.Y * Width + currentPos.X))
                    {
                        closed.AddAll(allies);
                        Scan();
                        return GetPath(start, goal, length);
                    }
                    foreach (var kv in goals)
                    {
                        ResetCell(kv.Key / Width, kv.Key % Width);
                    }
                    goals.Clear();
                    return Path;
                }
            }
            frustration = 0;
            foreach (var kv in goals)
            {
                ResetCell(kv.Key / Width, kv.Key % Width);
            }
            goals.Clear();
            return Path;
        }
        /*
(defn find-cells [^doubles a cell-kind]
    (persistent! (areduce ^doubles a i ret (transient {})
                          (if (= (hiphip/aget ^doubles a i) cell-kind) (assoc! ret i cell-kind) ret))))

(defn find-goals [^doubles a]
  (find-cells a GOAL))

(defn find-walls [^doubles a]
    (persistent! (areduce ^doubles a i ret (transient {})
                          (if (>= (hiphip/aget ^doubles a i) (double wall)) (assoc! ret i wall) ret))))

(defn find-floors [^doubles a]
  (find-cells a floor))

(defn find-lowest [^doubles a]
  (let [low-val (hiphip/amin a)]
    (find-cells a low-val)))

(defn find-monsters [m]
    (into {} (for [mp (map #(:pos @%) m)] [mp 1.0])))

(defn dijkstra
  ([a]
     (dijkstra a (find-walls a) (find-lowest a)))
  ([dun _]
     (dijkstra (:dungeon dun) (merge (find-walls (:dungeon dun)) (find-monsters @(:monsters dun))) (find-lowest (:dungeon dun))))
  ([a closed open-cells]
     (loop [open open-cells]
       (when (seq open)
         (recur (reduce (fn [newly-open [^long i ^double v]]
                          (reduce (fn [acc dir]
                                    (if (or (closed dir) (open dir)
                                            (>= (+ 1.0 v) (hiphip/aget ^doubles a dir)))
                                      acc
                                      (do (hiphip/aset ^doubles a dir (+ 1.0 v))
                                          (assoc acc dir (+ 1.0 v)))))
                                  newly-open, [(- i wide)
                                               (+ i wide)
                                               (- i 1)
                                               (+ i 1)]))
                        {}, open))))
     a))
         */
    }
}
