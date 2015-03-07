using System;
//using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using BearLib;
using System.Threading;
namespace DungeonRising
{

    public struct Position
    {
        public int X;
        public int Y;
        public Position(int y, int x)
        {
            X = x;
            Y = y;
        }
        public bool Validate(int Height, int Width)
        {
            return (Y >= 0 && Y < Height && X >= 0 && X < Width);
        }
        public static Position FromIndex(int index, int Width)
        {
            return new Position(index / Width, index % Width);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || !(obj is Position))
                return false;
            else
                return X == ((Position)obj).X && Y == ((Position)obj).Y;
        }

        public override int GetHashCode()
        {
            return (Y << 16) | (X & 0xffff);
        }
    }

    public enum GameState
    {
        Receiving, Animating
    }
    public class Entry
    {
        
        private Thread AnimationThread;
        private int currentPlayerColor = 0, currentHighlightColor = 0;

        private static Color[] playerColors = new Color[] {
                Color.FromArgb(0xff, 0x00, 0x00),
                Color.FromArgb(0xff, 0x77, 0x00),
                Color.FromArgb(0xff, 0xff, 0x00),
                Color.FromArgb(0x77, 0xff, 0x00),
                Color.FromArgb(0x00, 0xff, 0x00),
                Color.FromArgb(0x00, 0xff, 0x77),
                Color.FromArgb(0x00, 0xff, 0xff),
                Color.FromArgb(0x00, 0x77, 0xff),
                Color.FromArgb(0x00, 0x00, 0xff),
                Color.FromArgb(0x77, 0x00, 0xff),
                Color.FromArgb(0xff, 0x00, 0xff),
                Color.FromArgb(0xff, 0x00, 0x77),
            }, highlightColors = new Color[] {
                Color.FromArgb(0x33, 0x33, 0x33),
                Color.FromArgb(0x44, 0x44, 0x44),
                Color.FromArgb(0x55, 0x55, 0x55),
                Color.FromArgb(0x66, 0x66, 0x66),
                Color.FromArgb(0x77, 0x77, 0x77),
                Color.FromArgb(0x66, 0x66, 0x66),
                Color.FromArgb(0x55, 0x55, 0x55),
                Color.FromArgb(0x44, 0x44, 0x44),
            };
        private static Color LightGray = Color.FromArgb(211, 211, 211), BloodRed = Color.FromArgb(0xbb, 0x1c, 0);
        public Dungeon DungeonStart;
        public char[,] World, Display;
        public int[,] LogicMap;
        public EntityDictionary Entities;
        public string CurrentActor;
        public int Input = 0, StepsLeft = 0;
        public Position Cursor;
        public GameState CurrentState;
        private long Ticks;
        public Entry()
        {
            Entities = new EntityDictionary();
            Position playerStart = new Position(-1, -1);
            do
            {
                DungeonStart = new Dungeon(30, 40);
                World = DungeonStart.DLevel;
                playerStart = World.RandomMatch('.');
            } while (playerStart.Y < 0);

            Display = DungeonStart.PairLevel;
            LogicMap = DungeonStart.Level;
            Entity Player = new Entity("Player", "@\u1202", playerStart.Y, playerStart.X);
            CurrentActor = "Player";
            Cursor = new Position(playerStart.Y, playerStart.X);
            Player.Seeker = new Dijkstra(LogicMap);
            Player.Seeker.SetGoal(Player.Y, Player.X);
            Player.Seeker.Scan();
            Entities.Add("Player", Player);
            CurrentState = GameState.Receiving;
            //            Player.Seeker.GetPath(Player.Y, Player.X);
        }

        public static Entry Self = null;
        public static void Run()
        {
            Self = new Entry();
            Terminal.Open();
            Terminal.Set("log: level=trace");
            Terminal.Set("window: title='Dungeon Rising', size=90x33; font: Rogue-Zodiac-12x24.png, size=12x24, codepage=custom.txt;");
            Self.Ticks = 0;
            Self.AnimationThread = new Thread(() => {
                while (true)
                {
                    Self.currentPlayerColor = (Self.currentPlayerColor + 1) % 12;
                    Self.currentHighlightColor = (Self.currentHighlightColor + 1) % 8;
                    Self.Render();
                    Thread.Sleep(70);
                    Self.Ticks++;
                }
            });
            Self.AnimationThread.IsBackground = true;
            Self.AnimationThread.Start();
            Self.RunEntry();
        }
        public void Render()
        {
            Position p = new Position(0, 0);
            Dijkstra seeker = Entities[CurrentActor].Seeker;
            Terminal.Layer(0);
            for (int y = 0; y < World.GetLength(0); y++)
            {
                for (int x = 0; x < World.GetLength(1); x++)
                {
                    if (CurrentState == GameState.Receiving && seeker.CombinedMap[y, x] <= Entities[CurrentActor].MoveSpeed)
                    {
                        Terminal.BkColor(highlightColors[(currentHighlightColor + 100 - seeker.CombinedMap[y,x]) % highlightColors.Length]);
                        Terminal.Put(x * 2 + 1, y + 1, ' ');
                        Terminal.Put(x * 2 + 2, y + 1, ' ');
                    }
                    else
                    {
                        Terminal.BkColor(Color.Black);
                        Terminal.Put(x * 2 + 1, y + 1, ' ');
                        Terminal.Put(x * 2 + 2, y + 1, ' ');

                    }
                }
            }
            Terminal.Layer(1);
            for (int y = 0; y < World.GetLength(0); y++)
            {
                for (int x = 0; x < World.GetLength(1); x++)
                {
                    p.Y = y;
                    p.X = x;
                    Entity e = Entities[p];
                    if (e != null)
                    {
                        Terminal.Color(playerColors[currentPlayerColor]);
                        Terminal.Put(x * 2 + 1, y + 1, e.Left);
                        Terminal.Put(x * 2 + 2, y + 1, e.Right);
                        Terminal.Color(LightGray);
                    }
                    else if (Cursor.Y == y && Cursor.X == x)
                    {

                        Terminal.Color(playerColors[(currentPlayerColor + 3) % playerColors.Length]);
                        Terminal.Put(x * 2 + 1, y + 1, 'X');
                        Terminal.Put(x * 2 + 2, y + 1, '!');
                        Terminal.Color(LightGray);
                    }
                    else if (seeker.Path.Contains(y * seeker.Width + x))
                    {
                        Terminal.Color(playerColors[currentPlayerColor]);
                        Terminal.Put(x * 2 + 1, y + 1, Display[y, x * 2]);
                        Terminal.Put(x * 2 + 2, y + 1, Display[y, x * 2 + 1]);
                        Terminal.Color(LightGray);
                    }
                    else
                    {
                        Terminal.Put(x * 2 + 1, y + 1, Display[y, x * 2]);
                        Terminal.Put(x * 2 + 2, y + 1, Display[y, x * 2 + 1]);
                    }
                }
            }

            Terminal.Refresh();
            if (CurrentState == GameState.Animating)
            {
                StepsLeft = Entities.Step(CurrentActor);

                if (StepsLeft <= 0)
                {
                    FinishMove();
                    CurrentState = GameState.Receiving;
                }
            }
        }

        private void FinishMove()
        {
            Entities[CurrentActor].Seeker.Reset();
            Entities[CurrentActor].Seeker.SetGoal(Entities[CurrentActor].Y, Entities[CurrentActor].X);
            Entities[CurrentActor].Seeker.Scan();
        }
        public void RunEntry()
        {
            do
            {
                Input = Terminal.Read();
                switch (CurrentState)
                {
                    case GameState.Animating:
                        {
                        }
                        break;
                    case GameState.Receiving:
                        {
                            switch (Input)
                            {
                                case Terminal.TK_MOUSE_LEFT:
                                    {
                                        Cursor.Y = (Terminal.State(Terminal.TK_MOUSE_Y) - 1);
                                        Cursor.X = (Terminal.State(Terminal.TK_MOUSE_X) - 1) / 2;
                                        if (Entities[CurrentActor].Seeker.CombinedMap[Cursor.Y, Cursor.X] <= Entities[CurrentActor].MoveSpeed)
                                        {
                                            CurrentState = GameState.Animating;
                                        }
                                    }
                                    break;
                                case Terminal.TK_MOUSE_MOVE:
                                    {
                                        Cursor.Y = (Terminal.State(Terminal.TK_MOUSE_Y) - 1);
                                        Cursor.X = (Terminal.State(Terminal.TK_MOUSE_X) - 1) / 2;

                                        if (Cursor.Validate(DungeonStart.Height, DungeonStart.Width))
                                            Entities[CurrentActor].Seeker.GetPath(Cursor.Y, Cursor.X);
                                    }
                                    break;
                                case Terminal.TK_LEFT:
                                case Terminal.TK_KP_4:
                                case Terminal.TK_H:
                                    {
                                        if (LogicMap[Entities[CurrentActor].Y, Entities[CurrentActor].X - 1] == Dungeon.FLOOR)
                                        {
                                            Entities.Move(CurrentActor, 0, -1);
                                            FinishMove();
                                        }
                                    }
                                    break;
                                case Terminal.TK_RIGHT:
                                case Terminal.TK_KP_6:
                                case Terminal.TK_L:
                                    {
                                        if (LogicMap[Entities[CurrentActor].Y, Entities[CurrentActor].X + 1] == Dungeon.FLOOR)
                                        {
                                            Entities.Move(CurrentActor, 0, 1);
                                            FinishMove();
                                        }
                                    }
                                    break;
                                case Terminal.TK_UP:
                                case Terminal.TK_KP_8:
                                case Terminal.TK_K:
                                    {
                                        if (LogicMap[Entities[CurrentActor].Y - 1, Entities[CurrentActor].X] == Dungeon.FLOOR)
                                        {
                                            Entities.Move(CurrentActor, -1, 0);
                                            FinishMove();
                                        }
                                    }
                                    break;
                                case Terminal.TK_DOWN:
                                case Terminal.TK_KP_2:
                                case Terminal.TK_J:
                                    {
                                        if (LogicMap[Entities[CurrentActor].Y + 1, Entities[CurrentActor].X] == Dungeon.FLOOR)
                                        {
                                            Entities.Move(CurrentActor, 1, 0);
                                            FinishMove();
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                }
            } while (Input != Terminal.TK_CLOSE && Input != Terminal.TK_Q);


            Terminal.Close(); // terminal_close();
        }
    }
}
