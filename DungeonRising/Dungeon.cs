using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonRising
{
    [Serializable]
    [ImplementPropertyChanged]
    public class Dungeon
    {
        public const int FLOOR = 5000, WALL = 9999, DARK = 11111;
        public int Height { get; set; }
        public int Width { get; set; }
        public char[,] World { get; set; }
        public char[,] PairedWorld { get; set; }
        public int[,] LogicWorld { get; set; }
        public Position Entrance;
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

        public Dungeon()
        {
            /*
            do
            {
                Height = 60;
                Width = 60;
                bone = new BoneGen(XSSR.xsr);
                DLevel = BoneGen.WallWrap(bone.Generate(TilesetType.DEFAULT_DUNGEON, Height, Width));
                PairLevel = new char[Height, Width * 2];
                Level = new int[Height, Width];
            } while (!PlaceBones());*/
        }
        public Dungeon(int height, int width)
        {
            do
            {
                Height = height;
                Width = width;
                BoneGen Bone = new BoneGen(XSSR.xsr);
                World = BoneGen.WallWrap(Bone.Generate(TilesetType.DEFAULT_DUNGEON, Height, Width));
                PairedWorld = new char[Height, Width * 2];
                LogicWorld = new int[Height, Width];
            } while (!PlaceBones());
        }
        public Dungeon(TilesetType tt, int height, int width)
        {
            do
            {
                Height = height;
                Width = width;
                BoneGen Bone = new BoneGen(XSSR.xsr);
                World = BoneGen.WallWrap(Bone.Generate(tt, Height, Width));
                PairedWorld = new char[Height, Width * 2];
                LogicWorld = new int[Height, Width];
            } while (!PlaceBones());
        }
        public bool PlaceBones()
        {
            /*
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
            */
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (World[y, x] == '#')
                    {
                        int q = 0;
                        q |= (y <= 0 || World[y - 1, x] == '#') ? 1 : 0;
                        q |= (y <= 0 || x >= Width - 1 || World[y - 1, x + 1] == '#') ? 2 : 0;
                        q |= (x >= Width - 1 || World[y, x + 1] == '#') ? 4 : 0;
                        q |= (y >= Height - 1 || x >= Width - 1 || World[y + 1, x + 1] == '#') ? 8 : 0;
                        q |= (y >= Height - 1 || World[y + 1, x] == '#') ? 16 : 0;
                        q |= (y >= Height - 1 || x <= 0 || World[y + 1, x - 1] == '#') ? 32 : 0;
                        q |= (x <= 0 || World[y, x - 1] == '#') ? 64 : 0;
                        q |= (y <= 0 || x <= 0 || World[y - 1, x - 1] == '#') ? 128 : 0;

                        if (q == 0xff)
                        {
                            LogicWorld[y, x] = DARK; //(int)' '
                        }
                        else
                        {
                            LogicWorld[y, x] = WALL;
                        }
                    }
                    else
                    {
                        LogicWorld[y, x] = FLOOR;
                    }
                }
            }
            Entrance = LogicWorld.RandomMatch(FLOOR);
            if (Entrance.Y < 0) return false;

            LogicWorld = LogicWorld.Surround(DARK);
            World = World.Surround(' ');
            Height += 2;
            Width += 2;

            Dijkstra Scanner = new Dijkstra(LogicWorld);
            Scanner.Reset();
            Scanner.SetGoal(Entrance.Y + 1, Entrance.X + 1);
            Scanner.Scan();
            int floorCount = 0;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (Scanner.CombinedMap[y, x] == DARK)
                    {
                        LogicWorld[y, x] = DARK;
                        World[y, x] = ' ';
                    }
                    else if (Scanner.CombinedMap[y, x] < FLOOR)
                    {
                        ++floorCount;
                    }
                }
            }
            if (floorCount < Height * Width * 0.125)
                return false;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (LogicWorld[y, x] == WALL)
                    {
                        bool n = (y <= 0 || LogicWorld[y - 1, x] == WALL);
                        bool ne = (y <= 0 || x >= Width - 1 || LogicWorld[y - 1, x + 1] == WALL || LogicWorld[y - 1, x + 1] == DARK);
                        bool e = (x >= Width - 1 || LogicWorld[y, x + 1] == WALL);
                        bool se = (y >= Height - 1 || x >= Width - 1 || LogicWorld[y + 1, x + 1] == WALL || LogicWorld[y + 1, x + 1] == DARK);
                        bool s = (y >= Height - 1 || LogicWorld[y + 1, x] == WALL);
                        bool sw = (y >= Height - 1 || x <= 0 || LogicWorld[y + 1, x - 1] == WALL || LogicWorld[y + 1, x - 1] == DARK);
                        bool w = (x <= 0 || LogicWorld[y, x - 1] == WALL);
                        bool nw = (y <= 0 || x <= 0 || LogicWorld[y - 1, x - 1] == WALL || LogicWorld[y - 1, x - 1] == DARK);

                        if (n)
                        {
                            if (e)
                            {
                                if (s)
                                {
                                    if (w)
                                    {
                                        World[y, x] = '┼';
                                    }
                                    else
                                    {
                                        World[y, x] = '├';
                                    }
                                }
                                else if (w)
                                {
                                    World[y, x] = '┴';
                                }
                                else
                                {
                                    World[y, x] = '└';
                                }
                            }
                            else if (s)
                            {
                                if (w)
                                {
                                    World[y, x] = '┤';
                                }
                                else
                                {
                                    World[y, x] = '│';
                                }
                            }
                            else if (w)
                            {
                                World[y, x] = '┘';
                            }
                            else
                            {
                                World[y, x] = '│';
                            }
                        }
                        else if (e)  // ┼ ├ ┤ ┴ ┬ ┌ ┐ └ ┘ │ ─
                        {
                            if (s)
                            {
                                if (w)
                                {
                                    World[y, x] = '┬';
                                }
                                else
                                {
                                    World[y, x] = '┌';
                                }
                            }
                            else if (w)
                            {
                                World[y, x] = '─';
                            }
                            else
                            {
                                World[y, x] = '─';
                            }
                        }
                        else if (s)
                        {
                            if (w)
                            {
                                World[y, x] = '┐';
                            }
                            else
                            {
                                World[y, x] = '│';
                            }
                        }
                        else if (w)
                        {
                            World[y, x] = '─';
                        }
                        else
                        {
                            World[y, x] = '─';
                        }

                    }
                    else
                    {
                        if (LogicWorld[y, x] == DARK)
                        {
                            World[y, x] = ' ';
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
                    if (World[y, x] == '┼' || World[y, x] == '├' || World[y, x] == '┤' || World[y, x] == '┴')
                    {
                        if (World[y - 1, x] == '┼' || World[y - 1, x] == '├' || World[y - 1, x] == '┤' || World[y - 1, x] == '┬')
                        {
                            if ((y <= 0 || x >= Width - 1 || LogicWorld[y - 1, x + 1] == WALL || LogicWorld[y - 1, x + 1] == DARK) &&
                             (y <= 0 || x <= 0 || LogicWorld[y - 1, x - 1] == WALL || LogicWorld[y - 1, x - 1] == DARK) &&
                             (y <= 0 || x >= Width - 1 || LogicWorld[y, x + 1] == WALL || LogicWorld[y, x + 1] == DARK) &&
                             (y <= 0 || x <= 0 || LogicWorld[y, x - 1] == WALL || LogicWorld[y, x - 1] == DARK))
                            {
                                switch (World[y, x])
                                {
                                    case '┼':
                                        World[y, x] = '┬';
                                        break;
                                    case '├':
                                        World[y, x] = '┌';
                                        break;
                                    case '┤':
                                        World[y, x] = '┐';
                                        break;
                                    case '┴':
                                        World[y, x] = '─';
                                        break;
                                }
                                switch (World[y - 1, x])
                                {
                                    case '┼':
                                        World[y - 1, x] = '┴';
                                        break;
                                    case '├':
                                        World[y - 1, x] = '└';
                                        break;
                                    case '┤':
                                        World[y - 1, x] = '┘';
                                        break;
                                    case '┬':
                                        World[y - 1, x] = '─';
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
                    if (World[y, x] == '┼' || World[y, x] == '┤' || World[y, x] == '┬' || World[y, x] == '┴')
                    {
                        if (World[y, x - 1] == '┼' || World[y, x - 1] == '├' || World[y, x - 1] == '┬' || World[y, x - 1] == '┴')
                        {
                            if ((y >= Height - 1 || x >= Width - 1 || LogicWorld[y + 1, x - 1] == WALL || LogicWorld[y + 1, x - 1] == DARK) &&
                                                         (y <= 0 || x <= 0 || LogicWorld[y - 1, x - 1] == WALL || LogicWorld[y - 1, x - 1] == DARK) &&
                                                         (y >= Height - 1 || LogicWorld[y + 1, x] == WALL || LogicWorld[y + 1, x] == DARK) &&
                                                         (y <= 0 || LogicWorld[y - 1, x] == WALL || LogicWorld[y - 1, x] == DARK))
                            {
                                switch (World[y, x])
                                {
                                    case '┼':
                                        World[y, x] = '├';
                                        break;
                                    case '┤':
                                        World[y, x] = '│';
                                        break;
                                    case '┬':
                                        World[y, x] = '┌';
                                        break;
                                    case '┴':
                                        World[y, x] = '└';
                                        break;
                                }
                                switch (World[y, x - 1])
                                {
                                    case '┼':
                                        World[y, x - 1] = '┤';
                                        break;
                                    case '├':
                                        World[y, x - 1] = '│';
                                        break;
                                    case '┬':
                                        World[y, x - 1] = '┐';
                                        break;
                                    case '┴':
                                        World[y, x - 1] = '┘';
                                        break;

                                }
                            }
                        }
                    }
                }
            }
            Height -= 2;
            Width -= 2;
            LogicWorld = LogicWorld.Portion(1, 1, Height, Width);
            World = World.Portion(1, 1, Height, Width);


            for (int y = 0; y < Height; y++)
            {
                for (int x = 0, px = 0; x < Width; x++, px += 2)
                {
                    PairedWorld[y, px] = World[y, x];
                    switch (PairedWorld[y, px])
                    {
                        //                        case '┼ ├ ┤ ┴ ┬ ┌ ┐ └ ┘ │ ─'
                        case '┼':
                        case '├':
                        case '┴':
                        case '┬':
                        case '┌':
                        case '└':
                        case '─':
                            PairedWorld[y, px + 1] = '─';
                            break;

                        default:
                            PairedWorld[y, px + 1] = ' ';
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

            return true;
        }

        public Dungeon Replicate()
        {
            Dungeon d = new Dungeon();
            d.World = World.Replicate();
            d.PairedWorld = PairedWorld.Replicate();
            d.LogicWorld = LogicWorld.Replicate();
            d.Entrance = new Position(Entrance.Y, Entrance.X);
            d.Height = Height;
            d.Width = Width;
            return d;
        }
    }
}
