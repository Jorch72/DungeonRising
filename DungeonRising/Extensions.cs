using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
//using C5;

namespace DungeonRising
{
    public static class Extensions
    {
        //private static XSRandom XSSR = new XSRandom();
        public static T RandomElement<T>(this IEnumerable<T> coll)
        {
            var list = coll.ToList();
            if (!list.Any())
                return default(T);
            int idx = 0, tgt = XSSR.Next(list.Count());
            foreach (T t in list)
            {
                if (tgt == idx)
                {
                    return t;
                }
                idx++;
            }
            return default(T);
        }
        public static int FindByIndex<T>(this List<T> list, T target)
        {
            if (!list.Any())
                return -1;
            var idx = 0;
            foreach (var t in list)
            {
                if (target.Equals(t))
                {
                    return idx;
                }
                idx++;
            }
            return -1;
        }
        public static T RandomElement<T>(this T[] arr)
        {
            if (arr.Length == 0)
                return default(T);

            return arr[XSSR.Next(arr.Length)];
        }
        public static T RandomElement<T>(this T[,] mat)
        {
            if (mat.Length == 0)
                return default(T);

            return mat[XSSR.Next(mat.GetLength(0)), XSSR.Next(mat.GetLength(1))];
        }
        /// <summary>
        /// Insert and replace a section of a larger 2D array with a smaller one.
        /// If coords are only partially in-bounds, only the section that is within the
        /// boundaries of the larger 'mat' will be replaced.
        /// </summary>
        /// <typeparam name="T">Any type.</typeparam>
        /// <param name="mat">A 2d array, will be modified.</param>
        /// <param name="items">A (possibly) smaller 2D array to be inserted.</param>
        /// <param name="coord1">The coordinate to be given first as an index to mat and items.</param>
        /// <param name="coord2">The coordnate to be given second as an index to mat and items.</param>
        /// <returns></returns>
        public static T[,] Insert<T>(this T[,] mat, T[,] items, int coord1, int coord2)
        {
            if (mat.Length == 0 || items.Length == 0)
                return mat;

            for (int i = coord1, i1 = 0; i1 < items.GetLength(0); i++, i1++)
            {
                for (int j = coord2, j2 = 0; j2 < items.GetLength(1); j++, j2++)
                {
                    if (i < 0 || j < 0 || i >= mat.GetLength(0) || j >= mat.GetLength(1))
                        continue;
                    mat[i, j] = items[i1, j2];
                }
            }
            return mat;
        }
        /// <summary>
        /// Insert and replace a section of a larger 2D array with a smaller one, here a jagged smaller array.
        /// If coords are only partially in-bounds, only the section that is within the
        /// boundaries of the larger 'mat' will be replaced.
        /// </summary>
        /// <typeparam name="T">Any type.</typeparam>
        /// <param name="mat">A 2d array, will be modified.</param>
        /// <param name="items">A (possibly) smaller jagged array to be inserted.</param>
        /// <param name="coord1">The coordinate to be given first as an index to mat and items.</param>
        /// <param name="coord2">The coordnate to be given second as an index to mat and items.</param>
        /// <returns></returns>
        public static T[,] Insert<T>(this T[,] mat, T[][] items, int coord1, int coord2)
        {
            if (mat.Length == 0 || items.Length == 0)
                return mat;

            for (int i = coord1, i1 = 0; i1 < items.Length; i++, i1++)
            {
                for (int j = coord2, j2 = 0; j2 < items[i1].Length; j++, j2++)
                {
                    if (i < 0 || j < 0 || i >= mat.GetLength(0) || j >= mat.GetLength(1))
                        continue;
                    mat[i, j] = items[i1][j2];
                }
            }
            return mat;
        }
        /// <summary>
        /// Insert and replace a section of a larger 2D char array with a smaller one here a 1D
        /// array of strings that is used as a 2D grid of chars.
        /// If coords are only partially in-bounds, only the section that is within the
        /// boundaries of the larger 'mat' will be replaced.
        /// </summary>
        /// <param name="mat">A 2d array, will be modified.</param>
        /// <param name="items">A (possibly) smaller (and possibly jagged) array of strings to be inserted.</param>
        /// <param name="coord1">The coordinate to be given first as an index to mat and items.</param>
        /// <param name="coord2">The coordnate to be given second as an index to mat and items.</param>
        /// <returns></returns>
        public static char[,] Insert(this char[,] mat, string[] items, int coord1, int coord2)
        {
            if (mat.Length == 0 || items.Length == 0)
                return mat;

            for (int i = coord1, i1 = 0; i1 < items.Length; i++, i1++)
            {
                for (int j = coord2, j2 = 0; j2 < items[i1].Length; j++, j2++)
                {
                    if (i < 0 || j < 0 || i >= mat.GetLength(0) || j >= mat.GetLength(1))
                        continue;
                    mat[i, j] = items[i1][j2];
                }
            }
            return mat;
        }
        public static T[,] Portion<T>(this T[,] mat, int coord1, int coord2, int span1, int span2)
        {
            T[,] portion = new T[span1, span2];
            for (int i = 0; i < span1; i++)
            {
                for (int j = 0; j < span2; j++)
                {
                    portion[i, j] = mat[coord1 + i, coord2 + j];
                }
            }
            return portion;
        }

        public static T[,] Surround<T>(this T[,] mat, T surrounder)
        {
            T[,] expanded = new T[mat.GetLength(0)+2, mat.GetLength(1)+2];
            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    expanded[i+1, j+1] = mat[i, j];
                }
            }
            for (int i = 0; i < mat.GetLength(0) + 2; i++)
            {
                expanded[i, 0] = surrounder;
                expanded[i, mat.GetLength(0) + 1] = surrounder;
            }
            for (int j = 1; j < mat.GetLength(1) + 1; j++)
            {
                expanded[0, j] = surrounder;
                expanded[mat.GetLength(1) + 1, j] = surrounder;
            }
            return expanded;
        }
        public static Position RandomMatch<T>(this T[,] mat, T test)
            where T : IEquatable<T>
        {
            if (mat.Length == 0)
                return new Position(-1, -1);
            int frustration = 0;
            int span0 = mat.GetLength(0), span1 = mat.GetLength(1);
            int coord0 = XSSR.Next(span0), coord1 = XSSR.Next(span1);
            while (frustration < 20 && !(test.Equals(mat[coord0, coord1]))) //System.Collections.Generic.EqualityComparer<T>.Default.Equals
            {
                coord0 = XSSR.Next(span0);
                coord1 = XSSR.Next(span1);
                frustration++;
            }
            if (frustration >= 20)
            {
                List<int> iii = new List<int>(span0 * span1), jjj = new List<int>(span0 * span1);
                for (int i = 0; i < span0; i++ )
                {
                    for(int j = 0; j < span1; j++)
                    {
                        if(test.Equals(mat[i,j]))
                        {
                            iii.Add(i);
                            jjj.Add(j);
                        }
                    }
                }
                if(iii.Count == 0)
                    return new Position(-1, -1);
                else
                {
                    int idx = XSSR.Next(iii.Count);
                    return new Position(iii[idx], jjj[idx]);
                }
            }
            return new Position(coord0, coord1);
        }


        public static T GetIndex<T>(this T[,] mat, int index, int width)
        {
            return mat[index / width, index % width];
        }
        public static void SetIndex<T>(this T[,] mat, int index, int width, T value)
        {
            mat[index / width, index % width] = value;
        }

        public static T[,] Fill<T>(this T[,] mat, T item)
        {
            if (mat.Length == 0)
                return mat;

            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    mat[i, j] = item;
                }
            }
            return mat;
        }
        public static byte[,] Fill(this byte[,] mat, byte item)
        {
            if (mat.Length == 0)
                return mat;

            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    mat[i, j] = item;
                }
            }
            return mat;
        }
        public static short[,] Fill(this short[,] mat, short item)
        {
            if (mat.Length == 0)
                return mat;

            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    mat[i, j] = item;
                }
            }
            return mat;
        }
        public static T[, ,] Fill<T>(this T[, ,] mat, T item)
        {
            if (mat.Length == 0)
                return mat;

            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    for (int k = 0; k < mat.GetLength(2); k++)
                    {
                        mat[i, j, k] = item;
                    }
                }
            }
            return mat;
        }
        public static T[] Fill<T>(this T[] arr, T item)
        {
            if (arr.Length == 0)
                return arr;

            for (int i = 0; i < arr.GetLength(0); i++)
            {
                arr[i] = item;
            }
            return arr;
        }

        public static T[, ,] Replicate<T>(this T[, ,] mat)
        {
            if (mat.Length == 0)
                return new T[0, 0, 0];
            int xs = mat.GetLength(0), ys = mat.GetLength(1), zs = mat.GetLength(2);
            T[, ,] dupe = new T[xs, ys, zs];

            for (int i = 0; i < xs; i++)
            {
                for (int j = 0; j < ys; j++)
                {
                    for (int k = 0; k < zs; k++)
                    {
                        dupe[i, j, k] = mat[i, j, k];
                    }
                }
            }
            return dupe;
        }
        public static T[,] Replicate<T>(this T[,] mat)
        {
            if (mat.Length == 0)
                return new T[0, 0];
            int xs = mat.GetLength(0), ys = mat.GetLength(1);
            T[,] dupe = new T[xs, ys];

            for (int i = 0; i < xs; i++)
            {
                for (int j = 0; j < ys; j++)
                {
                    dupe[i, j] = mat[i, j];
                }

            }
            return dupe;
        }
        public static T[] Replicate<T>(this T[] mat)
        {
            if (mat.Length == 0)
                return new T[0];
            int xs = mat.Length;
            T[] dupe = new T[xs];

            for (int i = 0; i < xs; i++)
            {
                dupe[i] = mat[i];
            }
            return dupe;
        }
        public static void AddAll<TK, TV>(this Dictionary<TK, TV> dict, Dictionary<TK, TV> items)
        {
            foreach (var kv in items)
            {
                dict.Add(kv.Key, kv.Value);
            }

        }
        public static void UpdateAll<TK, TV>(this IDictionary<TK, TV> dict, IEnumerable<KeyValuePair<TK, TV>> items)
        {
            foreach (var kv in items)
            {
                dict[kv.Key] = kv.Value;
            }
        }
        public static void UpdateWithPositions(this IDictionary<int, int> dict, IEnumerable<KeyValuePair<Position, int>> items, int width)
        {
            foreach (var kv in items)
            {
                dict[kv.Key.Y * width + kv.Key.X] = kv.Value;
            }
        }
        public static Dictionary<TK, TV> Replicate<TK, TV>(this Dictionary<TK, TV> dict)
        {
            Dictionary<TK, TV> d = new Dictionary<TK, TV>();
            d.AddAll(dict);
            return d;
        }

    }
}
