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
                Color.FromArgb(0x77, 0x77, 0x77),
                Color.FromArgb(0x88, 0x88, 0x88),
                Color.FromArgb(0x99, 0x99, 0x99),
                Color.FromArgb(0xaa, 0xaa, 0xaa),
                Color.FromArgb(0xbb, 0xbb, 0xbb),
                Color.FromArgb(0xaa, 0xaa, 0xaa),
                Color.FromArgb(0x99, 0x99, 0x99),
                Color.FromArgb(0x88, 0x88, 0x88),
            };
        private static Color LightGray = Color.FromArgb(211, 211, 211), DarkGray = Color.FromArgb(0x17, 0x17, 0x17), BloodRed = Color.FromArgb(0xbb, 0x1c, 0x00);
        public Dungeon DungeonStart;
        public char[,] World, Display;
        public int[,] LogicMap;
        public EntityDictionary Entities;
        public string CurrentActor;
        public int Input = 0, StepsLeft = 0, StepsTaken = 0;
        public Position Cursor;
        public GameState CurrentState;
        public Schedule Initiative;
        private long Ticks, TurnsLeft;
        public static string IconGlyphs = "ሀሁሂሃሄህሆሇለሉሊላሌልሎሏሐሑሒሓሔሕሖሗመሙሚማሜ";
        public Entry()
        {
            Entities = new EntityDictionary();
            Initiative = new Schedule();
            TurnsLeft = 0;
            Position spawnPoint = new Position(-1, -1);
            do
            {
                DungeonStart = new Dungeon(30, 40);
                LogicMap = DungeonStart.Level;
                spawnPoint = LogicMap.RandomMatch(Dungeon.FLOOR);
            } while (spawnPoint.Y < 0);

            Display = DungeonStart.PairLevel;
            World = DungeonStart.DLevel;
            Entity Player = new Entity("Player", "@ሂ", spawnPoint.Y, spawnPoint.X, 5, 3, 0); // \u1202
            CurrentActor = "Player";
            Cursor = new Position(spawnPoint.Y, spawnPoint.X);
            Player.Seeker = new Dijkstra(LogicMap);
            Player.Seeker.SetGoal(Player.Y, Player.X);
            Player.Seeker.Scan();
            Entities.Add("Player", Player);

            spawnPoint = LogicMap.RandomMatch(Dungeon.FLOOR);
            if(spawnPoint.Y >= 0)
            {
                Entity baddie = new Entity("Baddie", "bሙ", spawnPoint.Y, spawnPoint.X, 6, 2, -1);
                baddie.Seeker = new Dijkstra(LogicMap);
                baddie.Seeker.SetGoal(baddie.Y, baddie.X);
                baddie.Seeker.Scan();
                Entities.Add("Baddie", baddie);
            }
            ResetInitiative();
            Entity first = Entities[Initiative.PeekTurn().Actor];
            if (first.Faction == 0)
                CurrentState = GameState.Receiving;
            //            Player.Seeker.GetPath(Player.Y, Player.X);
        }

        public static Entry Self = null;
        private void ResetInitiative()
        {
            foreach (Entity e in Entities)
            {
                Initiative.AddTurn(e.Name, e.Delay);
                TurnsLeft += e.ActSpeed;
            }
        }
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
                        Terminal.BkColor(LightGray);
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
                        if(e.Faction == 0)
                            Terminal.Color(playerColors[currentPlayerColor]);
                        else
                            Terminal.Color(BloodRed);
                        Terminal.Put(x * 2 + 1, y + 1, e.Left);
                        Terminal.Put(x * 2 + 2, y + 1, e.Right);
                        Terminal.Color(DarkGray);
                    }
                    else if (Cursor.Y == y && Cursor.X == x)
                    {

                        Terminal.Color(playerColors[(currentPlayerColor + 3) % playerColors.Length]);
                        Terminal.Put(x * 2 + 1, y + 1, 'X');
                        Terminal.Put(x * 2 + 2, y + 1, '!');
                        Terminal.Color(DarkGray);
                    }
                    else if (Entities[CurrentActor].Faction == 0 && seeker.Path.Contains(y * seeker.Width + x))
                    {
                        Terminal.Color(playerColors[currentPlayerColor]);
                        Terminal.Put(x * 2 + 1, y + 1, Display[y, x * 2]);
                        Terminal.Put(x * 2 + 2, y + 1, Display[y, x * 2 + 1]);
                        Terminal.Color(DarkGray);
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
                ++StepsTaken;
                if (StepsLeft <= 0 || StepsTaken > Entities[CurrentActor].MoveSpeed)
                {
                    FinishMove();
                }
            }
        }

        private void FinishMove()
        {
            StepsTaken = 0;
            Entity e = Entities[CurrentActor];
            e.Seeker.Reset();
            e.Seeker.SetGoal(e.Y, e.X);
            e.Seeker.Scan();
            Entity next = Entities[Initiative.NextTurn().Actor];

            if (--TurnsLeft <= 0)
                ResetInitiative();
            Initiative.AddTurn(e.Name, e.Delay);
            CurrentActor = next.Name;
            if (next.Faction == 0)
            {
                CurrentState = GameState.Receiving;
            }
            else
            {
                CurrentState = GameState.Animating;
                Entities[CurrentActor].Seeker.GetPath(Entities["Player"].Y, Entities["Player"].X);
            }
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
