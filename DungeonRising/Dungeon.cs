using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonRising
{
    public class Dungeon
    {
        public int Height { get; protected set; }
        public int Width  { get; protected set; }
        public char[,] DLevel {get; set;}
        public Dungeon()
        {
            Height = 60;
            Width = 60;
            DLevel = new char[Height, Width];
            DLevel.Fill('#');
            PlaceBones();
        }
        public Dungeon(int height, int width)
        {
            Height = height;
            Width = width;
            DLevel = new char[Height, Width];
            DLevel.Fill('#');
            PlaceBones();
        }
        public void PlaceBones()
        {
            for (int sy = Height + 10; sy > -Width - 20; sy -= 40)
            {
                for (int x = -10, y = sy; x < Width + 20 && y < Height + 20; x += 10, y += 10)
                {
                    DLevel.Insert(Herringbone.Horizontal.RandomElement(), y, x);
                }
            }
            for (int sy = Height - 20; sy > -Width - 20; sy -= 40)
            {
                for (int x = -10, y = sy; x < Width + 20 && y < Height + 20; x += 10, y += 10)
                {
                    DLevel.Insert(Herringbone.Vertical.RandomElement(), y, x);
                }
            }
            for(int x = 0; x < Width; x++)
            {
                DLevel[0, x] = '#';
                DLevel[Height - 1, x] = '#';
            }
            for(int y = 0; y < Height; y++)
            {
                DLevel[y, 0] = '#';
                DLevel[y, Width - 1] = '#';
            }
        }

    }
}
