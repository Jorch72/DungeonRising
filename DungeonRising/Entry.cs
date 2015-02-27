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
        public Dungeon dungeon;
        public char[,] world;
        public Entity player;
        public static int input = 0; 
            
        public Entry()
        {
            dungeon = new Dungeon(40, 100);
            world = dungeon.DLevel;
            Tuple<int, int> playerStart = world.RandomMatch('.');
            while (playerStart.Item1 < 0)
            {
                playerStart = world.RandomMatch('.');
            }
            player = new Entity('@', playerStart.Item2, playerStart.Item1);
        

        }

        private static Entry Self;
        public static void Run()
        {
            Self = new Entry();
            Terminal.Open();
            Terminal.Set("window: title='Dungeon Rising', size=120x50;");
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

            for (int y = 0; y < world.GetLength(0); y++)
            {
                for (int x = 0; x < world.GetLength(1); x++)
                {
                    if (x == player.X && y == player.Y)
                    {
                        Terminal.Color(playerColors[currentColor]);
                        Terminal.Put(x + 1, y + 1, player.Rep);
                        Terminal.Color(System.Drawing.Color.White);
                    }
                    else
                    {
                        Terminal.Put(x + 1, y + 1, world[y, x]);
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
                            if (world[player.Y,player.X - 1] != '#')
                                player.X--;
                        }
                        break;
                    case Terminal.TK_RIGHT:
                    case Terminal.TK_KP_6:
                    case Terminal.TK_L:
                        {
                            if (world[player.Y, player.X + 1] != '#')
                                player.X++;
                        }
                        break;
                    case Terminal.TK_UP:
                    case Terminal.TK_KP_8:
                    case Terminal.TK_K:
                        {
                            if (world[player.Y - 1, player.X] != '#')
                                player.Y--;
                        }
                        break;
                    case Terminal.TK_DOWN:
                    case Terminal.TK_KP_2:
                    case Terminal.TK_J:
                        {
                            if (world[player.Y + 1, player.X] != '#')
                                player.Y++;
                        }
                        break;
                }

            } while (input != Terminal.TK_CLOSE && input != Terminal.TK_Q);


            Terminal.Close(); // terminal_close();
        }
    }
}
