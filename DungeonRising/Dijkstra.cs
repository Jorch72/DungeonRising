using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C5;
namespace DungeonRising
{
    public class Dijkstra
    {
        public int[,] DMap;
        public int Height, Width;
        private int[][] DirShuffled;
        private const int GOAL = 0;
        private HashDictionary <int, int> goals, open, closed, fresh;
        private static XSRandom r;
        public Dijkstra(int[,] level)
        {
            r = new XSRandom();
            DMap = level.Replicate();
            Height = DMap.GetLength(0);
            Width = DMap.GetLength(1);
            goals = new HashDictionary<int, int>();
            open = new HashDictionary<int, int>();
            fresh = new HashDictionary<int, int>();
            closed = new HashDictionary<int, int>();

            DirShuffled = new int[][]
            {
                new int[]{ -Width, 1, Width, -1},
                new int[]{ -Width, 1, -1, Width},
                new int[]{ -Width, Width, 1, -1},
                new int[]{ -Width, -1, 1, Width},
                new int[]{ -Width, Width, -1, 1},
                new int[]{ -Width, -1, Width, 1},
                
                new int[]{ Width, 1, -Width, -1},
                new int[]{ Width, 1, -1, -Width},
                new int[]{ Width, -Width, 1, -1},
                new int[]{ Width, -1, 1, -Width},
                new int[]{ Width, -Width, -1, 1},
                new int[]{ Width, -1, -Width, 1},
                
                new int[]{ 1, Width, -Width, -1},
                new int[]{ 1, Width, -1, -Width},
                new int[]{ 1, -Width, Width, -1},
                new int[]{ 1, -1, Width, -Width},
                new int[]{ 1, -Width, -1, Width},
                new int[]{ 1, -1, -Width, Width},
                
                new int[]{ -1, Width, -Width, 1},
                new int[]{ -1, Width, 1, -Width},
                new int[]{ -1, -Width, Width, 1},
                new int[]{ -1, 1, Width, -Width},
                new int[]{ -1, -Width, 1, Width},
                new int[]{ -1, 1, -Width, Width},
                
            };
        }
        public void SetGoal(int y, int x)
        {
            if(DMap[y,x] > Dungeon.FLOOR)
            {
                return;
            }
            DMap[y, x] = GOAL;
            goals.Add(y * Width + x, GOAL);
        }
        public void SetFresh(int y, int x, int counter)
        {
            DMap[y, x] = counter;
            fresh.Add(y * Width + x, counter);
        }
        public void SetFresh(int index, int counter)
        {
            DMap[index / Width, index % Width] = counter;
            fresh.Add(index, counter);
        }
        public int[,] Scan()
        {
            for(int y = 0; y < Height; y++)
            {
                for(int x = 0; x < Width; x++)
                {
                    if (DMap[y, x] > Dungeon.FLOOR)
                        closed.Add(y * Width + x, DMap[y, x]);
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
                        if (!closed.Contains(cell.Key + dirs[d]) && !open.Contains(cell.Key + dirs[d]) && DMap.GetIndex(cell.Key, Width) + 1 < DMap.GetIndex(cell.Key + dirs[d], Width))
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
            return DMap;
        }
        public List<int> GetPath(int startY, int startX)
        {
            List<int> ls = new List<int>();
            if(DMap[startY, startX] > Dungeon.FLOOR)
            {
                return ls;
            }
            int currentPos = startY * Width + startX;
            ls.Add(currentPos);
            while (DMap.GetIndex(currentPos, Width) > 0)
            {
                int best = 9999, choice = 0;
                int[] dirs = DirShuffled[r.Next(24)];
                for (int d = 0; d < 4; d++)
                {
                    if (DMap.GetIndex(currentPos + dirs[d], Width) < best)
                    {
                        best = DMap.GetIndex(currentPos + dirs[d], Width);
                        choice = d;
                    }
                }
                currentPos += dirs[choice];
                ls.Add(currentPos);
            }
            return ls;
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
