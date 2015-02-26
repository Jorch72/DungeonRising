using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BearLib;
namespace DungeonRising
{
    class Entity
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
        public static void Run()
        {
            Entity player = new Entity('@', 6, 4);
            Terminal.Open(); // terminal_open();

            Entity[][] world = new Entity[8][]
            {
            MakeEntities(0, 0, " ###  ### "),
            MakeEntities(0, 2, "##.####.##"),
            MakeEntities(0, 2, "#........#"),
            MakeEntities(0, 3, "#........#"),
            MakeEntities(0, 4, "#........#"),
            MakeEntities(0, 5, "#........#"),
            MakeEntities(0, 6, "##.####.##"),
            MakeEntities(0, 7, " ###  ### "),
            };

            int input = 0; 
            // Wait until user close the window
            do
            {
                switch (input)
                {
                    case Terminal.TK_LEFT:
                    case Terminal.TK_KP_4:
                    case Terminal.TK_H:
                        {
                            if (world[player.Y][player.X - 1].Rep == '.')
                                player.X--;
                        }
                        break;
                    case Terminal.TK_RIGHT:
                    case Terminal.TK_KP_6:
                    case Terminal.TK_L:
                        {

                            if (world[player.Y][player.X + 1].Rep == '.')
                                player.X++;
                        }
                        break;
                    case Terminal.TK_UP:
                    case Terminal.TK_KP_8:
                    case Terminal.TK_K:
                        {
                            if (world[player.Y - 1][player.X].Rep == '.')
                                player.Y--;
                        }
                        break;
                    case Terminal.TK_DOWN:
                    case Terminal.TK_KP_2:
                    case Terminal.TK_J:
                        {
                            if (world[player.Y + 1][player.X].Rep == '.')
                                player.Y++;
                        }
                        break;
                }

                for (int y = 0; y < world.Length; y++)
                {
                    for (int x = 0; x < world[0].Length; x++)
                    {
                        if (x == player.X && y == player.Y)
                        {
                            Terminal.Put(x + 1, y + 1, player.Rep);
                        }
                        else
                        {
                            Terminal.Put(x + 1, y + 1, world[y][x].Rep);
                        }
                    }
                }
                Terminal.Refresh();
                input = Terminal.Read();

            } while (input != Terminal.TK_CLOSE && input != Terminal.TK_Q);


            Terminal.Close(); // terminal_close();
        }
    }
}
