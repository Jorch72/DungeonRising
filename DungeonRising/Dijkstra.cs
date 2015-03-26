using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using C5;
namespace DungeonRising
{
    [Serializable]
    public class Dijkstra
    {
        public static int[,] PhysicalMap;
        [NonSerialized]
        public int[,] CombinedMap;
        public static int Height, Width;
        public List<Position> Path;
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
        private const int Goal = 0;
        public Dictionary<int, int> Goals, Allies, Obstacles, Enemies;
        private Dictionary<int, int> _fresh, _closed, _open;
        public static XSRandom Rand;
        private static int _frustration = 0;

        public Dijkstra()
        {
            Rand = new XSRandom();
            Path = new List<Position>();

            Goals = new Dictionary<int, int>();
            _fresh = new Dictionary<int, int>();
            Allies = new Dictionary<int, int>();
            Enemies = new Dictionary<int, int>();
            Obstacles = new Dictionary<int, int>();
            _closed = new Dictionary<int, int>();
            _open = new Dictionary<int, int>();
        }
        public Dijkstra(int[,] level)
        {
            Rand = new XSRandom();
            CombinedMap = level.Replicate();
            PhysicalMap = level.Replicate();
            Path = new List<Position>();
            
            Height = PhysicalMap.GetLength(0);
            Width = PhysicalMap.GetLength(1);
            Goals = new Dictionary<int, int>();
            _fresh = new Dictionary<int, int>();
            Allies = new Dictionary<int, int>();
            Enemies = new Dictionary<int, int>();
            Obstacles = new Dictionary<int, int>();
            _closed = new Dictionary<int, int>();
            _open = new Dictionary<int, int>();
        }
        public void Reset()
        {
            CombinedMap = PhysicalMap.Replicate();
            Goals.Clear();
            Path.Clear();
        }
        public void SetGoal(int y, int x)
        {
            if (PhysicalMap[y, x] > Dungeon.Floor)
            {
                return;
            }
            if (Allies.ContainsKey(y * Width + x))
                Allies.Remove(y * Width + x);
            if (Obstacles.ContainsKey(y * Width + x))
                Obstacles.Remove(y * Width + x);
            Goals[y * Width + x] = Goal;
        }
        public void SetOccupied(int y, int x)
        {
            CombinedMap[y, x] = Dungeon.Wall;
        }
        public void ResetCell(int y, int x)
        {
            CombinedMap[y, x] = PhysicalMap[y, x];
        }
        public void RemoveAlly(Position pos)
        {
            if (Allies.ContainsKey(pos.Y * Width + pos.X))
            {
                Allies.Remove(pos.Y * Width + pos.X);
            }
        }
        public void UpdateAlly(Position start, Position end)
        {
            if (Allies.ContainsKey(start.Y * Width + start.X))
            {
                Allies.Remove(start.Y * Width + start.X);
            }
            Allies[end.Y * Width + end.X] = Dungeon.Wall;
        }
        public void AddAlly(Position pos)
        {
            Allies[pos.Y * Width + pos.X] = Dungeon.Wall;
        }
        public void AddAllies(IEnumerable<Position> friends)
        {
            foreach (Position pos in friends)
            {
                Allies[pos.Y * Width + pos.X] = Dungeon.Wall;
            }
        }
        public void RemoveAllies(IEnumerable<Position> friends)
        {
            foreach (Position pos in friends)
            {
                Allies.Remove(pos.Y * Width + pos.X);
            }
        }




        public void RemoveEnemy(Position pos)
        {
            if (Obstacles.ContainsKey(pos.Y * Width + pos.X))
            {
                Obstacles.Remove(pos.Y * Width + pos.X);
            }
            if (Enemies.ContainsKey(pos.Y * Width + pos.X))
            {
                Enemies.Remove(pos.Y * Width + pos.X);
            }
        }
        public void RemoveEnemies(IEnumerable<Position> blocks)
        {
            foreach (Position pos in blocks)
            {
                if (Obstacles.ContainsKey(pos.Y * Width + pos.X))
                    Obstacles.Remove(pos.Y * Width + pos.X);
                if (Enemies.ContainsKey(pos.Y * Width + pos.X))
                    Enemies.Remove(pos.Y * Width + pos.X);
            }
        }
        public void UpdateEnemy(Position start, Position end)
        {
            if (Obstacles.ContainsKey(start.Y * Width + start.X))
            {
                Obstacles.Remove(start.Y * Width + start.X);
            }

            if (Enemies.ContainsKey(start.Y * Width + start.X))
            {
                Enemies.Remove(start.Y * Width + start.X);
            }
            Obstacles[end.Y * Width + end.X] = Dungeon.Wall;
            Enemies[end.Y * Width + end.X] = Dungeon.Wall;
        }
        public List<Position> AdjacentToEnemy(Position pos)
        {
            List<Position> adjacent = new List<Position>();
            if (Enemies.ContainsKey((pos.Y) * Width + (pos.X + 1)))
                adjacent.Add(new Position(pos.Y, pos.X + 1));
            if (Enemies.ContainsKey((pos.Y) * Width + (pos.X - 1)))
                adjacent.Add(new Position(pos.Y, pos.X - 1));
            if (Enemies.ContainsKey((pos.Y + 1) * Width + (pos.X)))
                adjacent.Add(new Position(pos.Y + 1, pos.X));
            if (Enemies.ContainsKey((pos.Y - 1) * Width + (pos.X)))
                adjacent.Add(new Position(pos.Y - 1, pos.X));

            return adjacent;
        }
        public void AddEnemy(Position pos)
        {
            Obstacles[pos.Y * Width + pos.X] = Dungeon.Wall;
            Enemies[pos.Y * Width + pos.X] = Dungeon.Wall;
        }
        public void AddEnemies(IEnumerable<Position> blocks)
        {
            foreach (Position pos in blocks)
            {
                Obstacles[pos.Y * Width + pos.X] = Dungeon.Wall;
                Enemies[pos.Y * Width + pos.X] = Dungeon.Wall;
            }
        }
        public void SetEnemies(Dictionary<int, int> other)
        {
            Enemies = other.Replicate();
            Obstacles = other.Replicate();
        }
        public void SetEnemies(IEnumerable<Position> blocks)
        {
            Obstacles.Clear();
            Enemies.Clear();
            foreach (Position pos in blocks)
            {
                Obstacles[pos.Y * Width + pos.X] = Dungeon.Wall;
                Enemies[pos.Y * Width + pos.X] = Dungeon.Wall;
            }
        }



        public void RemoveObstacle(Position pos)
        {
            if (Obstacles.ContainsKey(pos.Y * Width + pos.X))
            {
                Obstacles.Remove(pos.Y * Width + pos.X);
            }
        }
        public void RemoveObstacles(IEnumerable<Position> blocks)
        {
            foreach (Position pos in blocks)
            {
                Obstacles.Remove(pos.Y * Width + pos.X);
            }
        }
        public void UpdateObstacle(Position start, Position end)
        {
            if (Obstacles.ContainsKey(start.Y * Width + start.X))
            {
                Obstacles.Remove(start.Y * Width + start.X);
            }
            Obstacles[end.Y * Width + end.X] = Dungeon.Wall;
        }

        public void AddObstacle(Position pos)
        {
            Obstacles[pos.Y * Width + pos.X] = Dungeon.Wall;
        }
        public void AddObstacles(IEnumerable<Position> blocks)
        {
            foreach (Position pos in blocks)
            {
                Obstacles[pos.Y * Width + pos.X] = Dungeon.Wall;
            }
        }
        public void SetObstacles(Dictionary<int, int> other)
        {
            Obstacles = other.Replicate();
        }
        public void SetObstacles(IEnumerable<Position> blocks)
        {
            Obstacles.Clear();
            foreach (Position pos in blocks)
            {
                Obstacles[pos.Y * Width + pos.X] = Dungeon.Wall;
            }
        }
        protected void SetFresh(int y, int x, int counter)
        {
            CombinedMap[y, x] = counter;
            _fresh.Add(y * Width + x, counter);
        }
        protected void SetFresh(int index, int counter)
        {
            CombinedMap[index / Width, index % Width] = counter;
            _fresh.Add(index, counter);
        }
        public int[,] Scan()
        {
            CombinedMap = PhysicalMap.Replicate();
            _closed.UpdateAll(Enemies);
//            closed.AddAll(allies);
            for(int y = 0; y < Height; y++)
            {
                for(int x = 0; x < Width; x++)
                {
                    if (CombinedMap[y, x] > Dungeon.Floor)
                        _closed[y * Width + x] = PhysicalMap[y, x];
                }
            }
            foreach(var kv in Goals)
            {
                CombinedMap[kv.Key / Width, kv.Key % Width] = Goal;
            }
            int numAssigned = Goals.Count;
            int iter = 0;
            _open.UpdateAll(Goals);
            int[] dirs = { -Width, 1, Width, -1 };
            while(numAssigned > 0)
            {
                ++iter;
                numAssigned = 0;

                foreach (var cell in _open)
                {
                    for (int d = 0; d < 4; d++)
                    {
                        if (!_closed.ContainsKey(cell.Key + dirs[d]) && !_open.ContainsKey(cell.Key + dirs[d]) && CombinedMap.GetIndex(cell.Key, Width) + 1 < CombinedMap.GetIndex(cell.Key + dirs[d], Width))
                        {
                            SetFresh(cell.Key + dirs[d], iter);
                            ++numAssigned;
                        }
                    }
                }
                _closed.UpdateAll(_open);
                _open = _fresh.Replicate();
                _fresh.Clear();
            }
            _closed.Clear();
            _open.Clear();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if(CombinedMap[y,x] == Dungeon.Floor)
                    {
                        CombinedMap[y, x] = Dungeon.Dark;
                    }
                }
            }

            return CombinedMap;
        }

        public List<Position> GetPath(Position start, Position goal, int length)
        {
            Path = new List<Position>();
            if (CombinedMap[goal.Y, goal.X] > Dungeon.Floor)
            {
                return Path;
            }
            SetGoal(goal.Y, goal.X);
            Scan();
            Position currentPos = start;
//            Path.Add(currentPos);
            while (CombinedMap[currentPos.Y, currentPos.X] > 0)
            {
                if (_frustration > 500)
                {
                    Path = new List<Position>();
                    _frustration = 0;
                    foreach (var kv in Goals)
                    {
                        ResetCell(kv.Key / Width, kv.Key % Width);
                    }
                    Goals.Clear();
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
                    _frustration = 0;
                    Path = new List<Position>();
                    return Path;
                }
                currentPos.Y += dirsY[choice];
                currentPos.X += dirsX[choice];
                Path.Add(currentPos);
                _frustration++;
                if (Path.Count >= length)
                {
                    if(Allies.ContainsKey(currentPos.Y * Width + currentPos.X))
                    {
                        _closed.AddAll(Allies);
                        Scan();
                        return GetPath(start, goal, length);
                    }
                    foreach (var kv in Goals)
                    {
                        ResetCell(kv.Key / Width, kv.Key % Width);
                    }
                    Goals.Clear();
                    return Path;
                }
            }
            _frustration = 0;
            foreach (var kv in Goals)
            {
                ResetCell(kv.Key / Width, kv.Key % Width);
            }
            Goals.Clear();
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
