using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using C5;

namespace DungeonRising
{
    public static class Extensions
    {
        private static Random r = new Random();
        public static T RandomElement<T>(this IEnumerable<T> list)
        {
            if (list.Count() == 0)
                return default(T);
            int idx = 0, tgt = r.Next(list.Count());
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
            if (list.Count() == 0)
                return -1;
            int idx = 0;
            foreach (T t in list)
            {
                if (target.Equals(list[idx]))
                {
                    return idx;
                }
                idx++;
            }
            return -1;
        }
        public static T RandomElement<T>(this T[,] mat)
        {
            if (mat.Length == 0)
                return default(T);

            return mat[r.Next(mat.GetLength(0)), r.Next(mat.GetLength(1))];
        }
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
        public static T[,] Insert<T>(this T[,] mat, T[][] items, int coord1, int coord2)
        {
            if (mat.Length == 0 || items.Length == 0 || items[0].Length == 0)
                return mat;

            for (int i = coord1, i1 = 0; i1 < items.Length; i++, i1++)
            {
                for (int j = coord2, j2 = 0; j2 < items[0].Length; j++, j2++)
                {
                    if (i < 0 || j < 0 || i >= mat.GetLength(0) || j >= mat.GetLength(1))
                        continue;
                    mat[i, j] = items[i1][j2];
                }
            }
            return mat;
        }
        public static char[,] Insert(this char[,] mat, string[] items, int coord1, int coord2)
        {
            if (mat.Length == 0 || items.Length == 0 || items[0].Length == 0)
                return mat;

            for (int i = coord1, i1 = 0; i1 < items.Length; i++, i1++)
            {
                for (int j = coord2, j2 = 0; j2 < items[0].Length; j++, j2++)
                {
                    if (i < 0 || j < 0 || i >= mat.GetLength(0) || j >= mat.GetLength(1))
                        continue;
                    mat[i, j] = items[i1][j2];
                }
            }
            return mat;
        }
        public static Position RandomMatch<T>(this T[,] mat, T test)
        {
            if (mat.Length == 0)
                return new Position(-1, -1);
            int frustration = 0;

            int coord1 = r.Next(mat.GetLength(0)), coord2 = r.Next(mat.GetLength(1));
            while (frustration < 20 && !(System.Collections.Generic.EqualityComparer<T>.Default.Equals(mat[coord1, coord2], test)))
            {
                coord1 = r.Next(mat.GetLength(0));
                coord2 = r.Next(mat.GetLength(1));
                frustration++;
            }
            if(frustration >= 20)
                return new Position(-1, -1);

            return new Position(coord1, coord2);
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
        public static HashDictionary<K, V> Replicate<K, V>(this HashDictionary<K, V> dict)
        {
            HashDictionary<K, V> d = new HashDictionary<K, V>();
            d.AddAll(dict);
            return d;
        }

    }
}
