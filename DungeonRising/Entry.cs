using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using BearLib;
using System.Threading;
namespace DungeonRising
{
    public class Entity
    {
        public int X, Y;
        public char Left, Right;
        public Entity(string Representation, int Y, int X)
        {
            this.Left = Representation[0];
            this.Right = Representation[1];
            this.X = X;
            this.Y = Y;
        }
        public Entity()
        {
            this.Left = '.';
            this.X = 0;
            this.Y = 0;
        }
    }

    public class Entry
    {
        static Entity[] MakeEntities(int startY, int startX, string reps)
        {
            Entity[] ret = new Entity[reps.Length];
            for(int i = 0; i < ret.Length; i++)
            {
                ret[i] = new Entity(reps[i] + ".", startY + i, startX);
            }
            return ret;
        }
        private Thread AnimationThread;
        private int currentColor = 0;

        private static Color[] playerColors = new Color[] {
                Color.FromArgb(0xff, 0, 0),
                Color.FromArgb(0xff, 0x77, 0),
                Color.FromArgb(0xff, 0xff, 0),
                Color.FromArgb(0x77, 0xff, 0),
                Color.FromArgb(0x00, 0xff, 0),
                Color.FromArgb(0x00, 0xff, 0x77),
                Color.FromArgb(0x00, 0xff, 0xff),
                Color.FromArgb(0x00, 0x77, 0xff),
                Color.FromArgb(0x00, 0x00, 0xff),
                Color.FromArgb(0x77, 0x00, 0xff),
                Color.FromArgb(0xff, 0x00, 0xff),
                Color.FromArgb(0xff, 0x00, 0x77),
            };
        private static Color LightGray = Color.FromArgb(211, 211, 211);
        public Dungeon DungeonStart;
        public char[,] World, Display;
        public int[,] LogicMap;
        public Entity Player;
        public List<int> Path;
        public Dijkstra Dijk;
        public static int Input = 0;
        Tuple<int, int> Goal;

        public Entry()
        {
            DungeonStart = new Dungeon(40, 50);
            World = DungeonStart.DLevel;
            Display = DungeonStart.PairLevel;
            LogicMap = DungeonStart.Level;
            Tuple<int, int> playerStart = World.RandomMatch('.');
            Goal = World.RandomMatch('.');
            while (playerStart.Item1 < 0)
            {
                playerStart = World.RandomMatch('.');
            }
            Player = new Entity("@}", playerStart.Item1, playerStart.Item2);

            Dijk = new Dijkstra(LogicMap);
            Dijk.SetGoal(Goal.Item1, Goal.Item2);
            Dijk.Scan();
            Path = Dijk.GetPath(Player.Y, Player.X);
        }

        private static Entry Self;
        public static void Run()
        {
            Self = new Entry();
            Terminal.Open();
            Terminal.Set("log: level=trace");
            Terminal.Set("window: title='Dungeon Rising', size=110x45; font: ./Zodiac-Narrow-6x12.png, size=6x12, codepage=./custom.txt;");
            Self.AnimationThread = new Thread(() => {
                while (true)
                {
                    Self.currentColor = (Self.currentColor + 1) % 12;
                    Self.Render();
                    Thread.Sleep(50);
                }
            });
            Self.AnimationThread.IsBackground = true;
            Self.AnimationThread.Start();
            Self.RunEntry();
        }
        public void Render()
        {

            for (int y = 0; y < World.GetLength(0); y++)
            {
                for (int x = 0; x < World.GetLength(1); x++)
                {
                    if (x == Player.X && y == Player.Y)
                    {
                        Terminal.Color(playerColors[currentColor]);
                        Terminal.Put(x * 2 + 1, y + 1, Player.Left);
                        Terminal.Put(x * 2 + 2, y + 1, Player.Right);
                        Terminal.Color(LightGray);
                    }
                    else if(Goal.Item1 == y && Goal.Item2 == x)
                    {

                        Terminal.Color(playerColors[(currentColor + 3) % playerColors.Length]);
                        Terminal.Put(x * 2 + 1, y + 1, 'X');
                        Terminal.Put(x * 2 + 2, y + 1, '!');
                        Terminal.Color(LightGray);
                    }
                    else if(Path.Contains(y * Dijk.Width + x))
                    {
                        Terminal.Color(playerColors[currentColor]);
                        Terminal.Put(x * 2+ 1, y + 1, Display[y, x*2]);
                        Terminal.Put(x * 2 + 2, y + 1, Display[y, x*2 + 1]);
                        Terminal.Color(LightGray);
                    }
                    else
                    {
                        Terminal.Put(x * 2 + 1, y + 1, Display[y, x*2]);
                        Terminal.Put(x * 2 + 2, y + 1, Display[y, x*2 + 1]);
                    }
                }
            }
            Terminal.Refresh();
        }
        public void RunEntry()
        {
            do
            {
                Input = Terminal.Read();

                switch (Input)
                {
                    case Terminal.TK_LEFT:
                    case Terminal.TK_KP_4:
                    case Terminal.TK_H:
                        {
                            if (LogicMap[Player.Y,Player.X - 1] == Dungeon.FLOOR)
                                Player.X--;
                        }
                        break;
                    case Terminal.TK_RIGHT:
                    case Terminal.TK_KP_6:
                    case Terminal.TK_L:
                        {
                            if (LogicMap[Player.Y, Player.X + 1] == Dungeon.FLOOR)
                                Player.X++;
                        }
                        break;
                    case Terminal.TK_UP:
                    case Terminal.TK_KP_8:
                    case Terminal.TK_K:
                        {
                            if (LogicMap[Player.Y - 1, Player.X] == Dungeon.FLOOR)
                                Player.Y--;
                        }
                        break;
                    case Terminal.TK_DOWN:
                    case Terminal.TK_KP_2:
                    case Terminal.TK_J:
                        {
                            if (LogicMap[Player.Y + 1, Player.X] == Dungeon.FLOOR)
                                Player.Y++;
                        }
                        break;
                }

            } while (Input != Terminal.TK_CLOSE && Input != Terminal.TK_Q);


            Terminal.Close(); // terminal_close();
        }
    }
}
