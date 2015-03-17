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
        public int[,] PhysicalMap, CombinedMap;
        public int Height, Width;
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
        public Dictionary<int, int> goals;
        private Dictionary<int, int> open, closed, fresh;
        public static XSRandom Rand;
        public Dijkstra()
        {
            Rand = new XSRandom();

            goals = new Dictionary<int, int>();
            open = new Dictionary<int, int>();
            fresh = new Dictionary<int, int>();
            closed = new Dictionary<int, int>();
        }
        public Dijkstra(int[,] level)
        {
            Rand = new XSRandom();
            CombinedMap = level.Replicate();
            PhysicalMap = level.Replicate();
            Path = new ArrayList<Position>();
            
            Height = PhysicalMap.GetLength(0);
            Width = PhysicalMap.GetLength(1);
            goals = new Dictionary<int, int>();
            open = new Dictionary<int, int>();
            fresh = new Dictionary<int, int>();
            closed = new Dictionary<int, int>();

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
            CombinedMap[y, x] = GOAL;
            goals.Add(y * Width + x, GOAL);
        }
        public void SetOccupied(int y, int x)
        {
            CombinedMap[y, x] = Dungeon.WALL;
        }
        public void ResetCell(int y, int x)
        {
            if (CombinedMap[y, x] == GOAL)
                goals.Remove(y * Width + x);
                
            CombinedMap[y, x] = PhysicalMap[y, x];
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
            for(int y = 0; y < Height; y++)
            {
                for(int x = 0; x < Width; x++)
                {
                    if (CombinedMap[y, x] > Dungeon.FLOOR)
                        closed.Add(y * Width + x, PhysicalMap[y, x]);
                }
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
            open.Clear();
            closed.Clear();


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

        public ArrayList<Position> GetPath(int startY, int startX)
        {
            Path = new ArrayList<Position>();
            int frustration = 0;
            if (CombinedMap[startY, startX] > Dungeon.FLOOR)
            {
                return Path;
            }
            Position currentPos = new Position(startY, startX);
            Path.Add(currentPos);
            while (CombinedMap[currentPos.Y, currentPos.X] > 0)
            {
                if(frustration > 1000)
                    return new ArrayList<Position>();
                int best = 9999, choice = 0, whichOrder = Rand.Next(24);
                int[] dirsY = DirShuffledY[whichOrder], dirsX = DirShuffledX[whichOrder];
                for (int d = 0; d < 4; d++)
                {
                    if (CombinedMap[currentPos.Y + dirsY[d], currentPos.X + dirsX[d]] < best)
                    {
                        best = CombinedMap[currentPos.Y + dirsY[d], currentPos.X + dirsX[d]];
                        choice = d;
                    }
                }
                currentPos.Y += dirsY[choice];
                currentPos.X += dirsX[choice];
                Path.Add(currentPos);
                frustration++;
            }
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
