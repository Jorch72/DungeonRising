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
        public char Rep;
        public Entity(char Representation, int X, int Y)
        {
            this.Rep = Representation;
            this.X = X;
            this.Y = Y;
        }
        public Entity()
        {
            this.Rep = '.';
            this.X = 0;
            this.Y = 0;
        }
    }

    public class Entry
    {
        static Entity[] MakeEntities(int startX, int startY, string reps)
        {
            Entity[] ret = new Entity[reps.Length];
            for(int i = 0; i < ret.Length; i++)
            {
                ret[i] = new Entity(reps[i], startX + i, startY);
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
        public Dungeon DungeonStart;
        public char[,] World;
        public int[,] LogicMap;
        public Entity Player;
        public static int input = 0; 
            
        public Entry()
        {
            DungeonStart = new Dungeon(40, 100);
            World = DungeonStart.DLevel;
            LogicMap = DungeonStart.Level;
            Tuple<int, int> playerStart = World.RandomMatch('.');
            while (playerStart.Item1 < 0)
            {
                playerStart = World.RandomMatch('.');
            }
            Player = new Entity('@', playerStart.Item2, playerStart.Item1);
        

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
                        Terminal.Put(x + 1, y + 1, Player.Rep);
                        Terminal.Color(System.Drawing.Color.White);
                    }
                    else
                    {
                        Terminal.Put(x + 1, y + 1, World[y, x]);
                    }
                }
            }
            Terminal.Refresh();
        }
        public void RunEntry()
        {
            do
            {
                input = Terminal.Read();

                switch (input)
                {
                    case Terminal.TK_LEFT:
                    case Terminal.TK_KP_4:
                    case Terminal.TK_H:
                        {
                            if (LogicMap[Player.Y,Player.X - 1] != -1)
                                Player.X--;
                        }
                        break;
                    case Terminal.TK_RIGHT:
                    case Terminal.TK_KP_6:
                    case Terminal.TK_L:
                        {
                            if (LogicMap[Player.Y, Player.X + 1] != -1)
                                Player.X++;
                        }
                        break;
                    case Terminal.TK_UP:
                    case Terminal.TK_KP_8:
                    case Terminal.TK_K:
                        {
                            if (LogicMap[Player.Y - 1, Player.X] != -1)
                                Player.Y--;
                        }
                        break;
                    case Terminal.TK_DOWN:
                    case Terminal.TK_KP_2:
                    case Terminal.TK_J:
                        {
                            if (LogicMap[Player.Y + 1, Player.X] != -1)
                                Player.Y++;
                        }
                        break;
                }

            } while (input != Terminal.TK_CLOSE && input != Terminal.TK_Q);


            Terminal.Close(); // terminal_close();
        }
    }
}
