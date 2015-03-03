using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonRising
{
    public class Dungeon
    {
        public const int FLOOR = 5000, WALL = 9999, DARK = 11111;
        public int Height { get; protected set; }
        public int Width  { get; protected set; }
        public static Dictionary<char, int> Mapping = new Dictionary<char, int>(256);
        public char[,] DLevel { get; set; }
        public char[,] PairLevel { get; set; }
        public int[,] Level { get; set; }
        static Dungeon()
        {
            for(int c = 0; c < 128; c++)
            {
                Mapping.Add((char)c, c);
            }
            /*
0123456789ABCDEF
─━│┃┄┅┆┇┈┉┊┋┌┍┎┏ 2500
┐┑┒┓└┕┖┗┘┙┚┛├┝┞┟ 2510
┠┡┢┣┤┥┦┧┨┩┪┫┬┭┮┯ 2520
┰┱┲┳┴┵┶┷┸┹┺┻┼┽┾┿ 2530
╀╁╂╃╄╅╆╇╈╉╊╋╌╍╎╏ 2540
═║╒╓╔╕╖╗╘╙╚╛╜╝╞╟ 2550
╠╡╢╣╤╥╦╧╨╩╪╫╬╭╮╯ 2560
╰╱╲╳╴╵╶╷╸╹╺╻╼╽╾╿ 2570
             */
            for (int c = 0; c < 128; c++)
            {
                Mapping.Add((char)(c + 0x2500), c + 128);
            }
            
        }
        public Dungeon()
        {
            Height = 60;
            Width = 60;
            DLevel = new char[Height, Width];
            Level = new int[Height, Width];
            DLevel.Fill('#');
            PlaceBones();
        }
        public Dungeon(int height, int width)
        {
            Height = height;
            Width = width;
            DLevel = new char[Height, Width];
            PairLevel = new char[Height, Width * 2];
            Level = new int[Height, Width];
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
                DLevel[1, x] = '#';
                DLevel[Height - 1, x] = '#';
                DLevel[Height - 2, x] = '#';
            }
            for(int y = 0; y < Height; y++)
            {
                DLevel[y, 0] = '#';
                DLevel[y, 1] = '#';
                DLevel[y, Width - 1] = '#';
                DLevel[y, Width - 2] = '#';
            }

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (DLevel[y, x] == '#')
                    {
                        int q = 0;
                        q |= (y <= 0 || DLevel[y - 1, x] == '#') ? 1 : 0;
                        q |= (y <= 0 || x >= Width - 1 || DLevel[y - 1, x + 1] == '#') ? 2 : 0;
                        q |= (x >= Width - 1 || DLevel[y, x + 1] == '#') ? 4 : 0;
                        q |= (y >= Height - 1 || x >= Width - 1 || DLevel[y + 1, x + 1] == '#') ? 8 : 0;
                        q |= (y >= Height - 1 || DLevel[y + 1, x] == '#') ? 16 : 0;
                        q |= (y >= Height - 1 || x <= 0 || DLevel[y + 1, x - 1] == '#') ? 32 : 0;
                        q |= (x <= 0 || DLevel[y, x - 1] == '#') ? 64 : 0;
                        q |= (y <= 0 || x <= 0 || DLevel[y - 1, x - 1] == '#') ? 128 : 0;

                        if (q == 0xff)
                        {
                            Level[y, x] = DARK; //(int)' '
                        }
                        else
                        {
                            Level[y, x] = WALL;
                        }
                    }
                    else
                    {
                        Level[y, x] = FLOOR;
                    }
                }
            }
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (Level[y, x] == WALL)
                    {
                        bool n = (y <= 0 || Level[y - 1, x] == WALL);
                        bool ne = (y <= 0 || x >= Width - 1 || Level[y - 1, x + 1] == WALL || Level[y - 1, x + 1] == DARK);
                        bool e = (x >= Width - 1 || Level[y, x + 1] == WALL);
                        bool se = (y >= Height - 1 || x >= Width - 1 || Level[y + 1, x + 1] == WALL || Level[y + 1, x + 1] == DARK);
                        bool s = (y >= Height - 1 || Level[y + 1, x] == WALL);
                        bool sw = (y >= Height - 1 || x <= 0 || Level[y + 1, x - 1] == WALL || Level[y + 1, x - 1] == DARK);
                        bool w = (x <= 0 || Level[y, x - 1] == WALL);
                        bool nw = (y <= 0 || x <= 0 || Level[y - 1, x - 1] == WALL || Level[y - 1, x - 1] == DARK);

                        if (n)
                        {
                            if (e)
                            {
                                if (s)
                                {
                                    if (w)
                                    {
                                        DLevel[y, x] = '┼';
                                    }
                                    else
                                    {
                                        DLevel[y, x] = '├';
                                    }
                                }
                                else if (w)
                                {
                                    DLevel[y, x] = '┴';
                                }
                                else
                                {
                                    DLevel[y, x] = '└';
                                }
                            }
                            else if (s)
                            {
                                if (w)
                                {
                                    DLevel[y, x] = '┤';
                                }
                                else
                                {
                                    DLevel[y, x] = '│';
                                }
                            }
                            else if (w)
                            {
                                DLevel[y, x] = '┘';
                            }
                            else
                            {
                                DLevel[y, x] = '│';
                            }
                        }
                        else if (e)  // ┼ ├ ┤ ┴ ┬ ┌ ┐ └ ┘ │ ─
                        {
                            if (s)
                            {
                                if (w)
                                {
                                    DLevel[y, x] = '┬';
                                }
                                else
                                {
                                    DLevel[y, x] = '┌';
                                }
                            }
                            else if (w)
                            {
                                DLevel[y, x] = '─';
                            }
                            else
                            {
                                DLevel[y, x] = '─';
                            }
                        }
                        else if (s)
                        {
                            if (w)
                            {
                                DLevel[y, x] = '┐';
                            }
                            else
                            {
                                DLevel[y, x] = '│';
                            }
                        }
                        else if (w)
                        {
                            DLevel[y, x] = '─';
                        }
                        else
                        {
                            DLevel[y, x] = '─';
                        }

                    }
                    else
                    {
                        if (Level[y, x] == DARK)
                        {
                            DLevel[y, x] = ' ';
                        }
                    }
                }
            }
            //vertical crossbar removal
            for (int y = 1; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    // ┼ ├ ┤ ┴ ┬ ┌ ┐ └ ┘ │ ─
                    if (DLevel[y, x] == '┼' || DLevel[y, x] == '├' || DLevel[y, x] == '┤' || DLevel[y, x] == '┴')
                    {
                        if (DLevel[y - 1, x] == '┼' || DLevel[y - 1, x] == '├' || DLevel[y - 1, x] == '┤' || DLevel[y - 1, x] == '┬')
                        {
                            if ((y <= 0 || x >= Width - 1 || Level[y - 1, x + 1] == WALL || Level[y - 1, x + 1] == DARK) &&
                             (y <= 0 || x <= 0 || Level[y - 1, x - 1] == WALL || Level[y - 1, x - 1] == DARK) &&
                             (y <= 0 || x >= Width - 1 || Level[y, x + 1] == WALL || Level[y, x + 1] == DARK) &&
                             (y <= 0 || x <= 0 || Level[y, x - 1] == WALL || Level[y, x - 1] == DARK))
                            {
                                switch (DLevel[y, x])
                                {
                                    case '┼':
                                        DLevel[y, x] = '┬';
                                        break;
                                    case '├':
                                        DLevel[y, x] = '┌';
                                        break;
                                    case '┤':
                                        DLevel[y, x] = '┐';
                                        break;
                                    case '┴':
                                        DLevel[y, x] = '─';
                                        break;
                                }
                                switch (DLevel[y - 1, x])
                                {
                                    case '┼':
                                        DLevel[y-1, x] = '┴';
                                        break;
                                    case '├':
                                        DLevel[y-1, x] = '└';
                                        break;
                                    case '┤':
                                        DLevel[y-1, x] = '┘';
                                        break;
                                    case '┬':
                                        DLevel[y-1, x] = '─';
                                        break;

                                }
                            }
                        }
                    }
                }
            }
            //horizontal crossbar removal
            for (int y = 0; y < Height; y++)
            {
                for (int x = 1; x < Width; x++)
                {
                    // ┼ ├ ┤ ┴ ┬ ┌ ┐ └ ┘ │ ─
                    if (DLevel[y, x] == '┼' || DLevel[y, x] == '┤' || DLevel[y, x] == '┬' || DLevel[y, x] == '┴')
                    {
                        if (DLevel[y, x-1] == '┼' || DLevel[y, x-1] == '├' || DLevel[y, x-1] == '┬' || DLevel[y, x-1] == '┴')
                        {
                            if ((y >= Height - 1 || x >= Width - 1 || Level[y + 1, x -1] == WALL || Level[y + 1, x - 1] == DARK) &&
                             (y <= 0 || x <= 0 || Level[y - 1, x - 1] == WALL || Level[y - 1, x - 1] == DARK) &&
                             (y >= Height - 1 || Level[y + 1, x] == WALL || Level[y + 1, x] == DARK) &&
                             (y <= 0 || Level[y - 1, x] == WALL || Level[y - 1, x] == DARK))
                            {
                                switch (DLevel[y, x])
                                {
                                    case '┼':
                                        DLevel[y, x] = '├';
                                        break;
                                    case '┤':
                                        DLevel[y, x] = '│';
                                        break;
                                    case '┬':
                                        DLevel[y, x] = '┌';
                                        break;
                                    case '┴':
                                        DLevel[y, x] = '└';
                                        break;
                                }
                                switch (DLevel[y, x - 1])
                                {
                                    case '┼':
                                        DLevel[y, x - 1] = '┤';
                                        break;
                                    case '├':
                                        DLevel[y, x - 1] = '│';
                                        break;
                                    case '┬':
                                        DLevel[y, x - 1] = '┐';
                                        break;
                                    case '┴':
                                        DLevel[y, x - 1] = '┘';
                                        break;

                                }
                            }
                        }
                    }
                }
            }
            
            for(int y = 0; y < Height; y++)
            {
                for(int x = 0, px = 0; x < Width; x++, px += 2)
                {
                    PairLevel[y, px] = DLevel[y, x];
                    switch(PairLevel[y, px])
                    {
//                        case '┼ ├ ┤ ┴ ┬ ┌ ┐ └ ┘ │ ─'
                        case '┼':
                        case '├':
                        case '┴':
                        case '┬':
                        case '┌':
                        case '└':
                        case '─':
                            PairLevel[y, px + 1] = '─';
                            break;

                        default:
                            PairLevel[y, px + 1] = ' ';
                            break;
                        /*
                    case '.':
                    case '┤':
                    case '┐':
                    case '┘':
                    case '│':
                         */
                    }
                }
            }


        }

    }
}
