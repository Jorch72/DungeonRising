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
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public char Left, Right;
        public Dijkstra Seeker;
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
        public void Move(int yMove, int xMove)
        {
            Y += yMove;
            X += xMove;
            Seeker.Reset();
            Seeker.SetGoal(Y, X);
            Seeker.Scan();
            //Seeker.GetPath(Y, X);

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
        private static Color LightGray = Color.FromArgb(211, 211, 211), BloodRed = Color.FromArgb(0xbb, 0x1c, 0);
        public Dungeon DungeonStart;
        public char[,] World, Display;
        public int[,] LogicMap;
        public Entity Player;
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

            Player.Seeker = new Dijkstra(LogicMap);
            Player.Seeker.SetGoal(Player.Y, Player.X);
            Player.Seeker.Scan();
            //            Player.Seeker.GetPath(Player.Y, Player.X);
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
                    else if (Terminal.State(Terminal.TK_MOUSE_Y) == y + 1 && (Terminal.State(Terminal.TK_MOUSE_X) - 1) / 2 == x)
                    {

                        Terminal.Color(playerColors[(currentColor + 3) % playerColors.Length]);
                        Terminal.Put(x * 2 + 1, y + 1, 'X');
                        Terminal.Put(x * 2 + 2, y + 1, '!');
                        Terminal.Color(LightGray);
                    }
                    else if(Player.Seeker.Path.Contains(y * Player.Seeker.Width + x))
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
                    case Terminal.TK_MOUSE_MOVE:
                        {
                            int ty = (Terminal.State(Terminal.TK_MOUSE_Y) - 1), tx = (Terminal.State(Terminal.TK_MOUSE_X) - 1) / 2;
                            if (ty >= 0 && ty < DungeonStart.Height && tx >= 0 && tx < DungeonStart.Width)
                                Player.Seeker.GetPath(ty, tx);
                        }
                        break;
                    case Terminal.TK_LEFT:
                    case Terminal.TK_KP_4:
                    case Terminal.TK_H:
                        {
                            if (LogicMap[Player.Y, Player.X - 1] == Dungeon.FLOOR)
                                Player.Move(0, -1);
                        }
                        break;
                    case Terminal.TK_RIGHT:
                    case Terminal.TK_KP_6:
                    case Terminal.TK_L:
                        {
                            if (LogicMap[Player.Y, Player.X + 1] == Dungeon.FLOOR)
                                Player.Move(0, 1);
                        }
                        break;
                    case Terminal.TK_UP:
                    case Terminal.TK_KP_8:
                    case Terminal.TK_K:
                        {
                            if (LogicMap[Player.Y - 1, Player.X] == Dungeon.FLOOR)
                                Player.Move(-1, 0);
                        }
                        break;
                    case Terminal.TK_DOWN:
                    case Terminal.TK_KP_2:
                    case Terminal.TK_J:
                        {
                            if (LogicMap[Player.Y + 1, Player.X] == Dungeon.FLOOR)
                                Player.Move(1, 0);
                        }
                        break;
                }

            } while (Input != Terminal.TK_CLOSE && Input != Terminal.TK_Q);


            Terminal.Close(); // terminal_close();
        }
    }
}
