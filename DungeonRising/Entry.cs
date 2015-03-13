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
        public void MakeValid(int Height, int Width)
        {
            if (Y < 0)
                Y = 0;
            if (Y >= Height)
                Y = Height - 1;
            if (X < 0)
                X = 0;
            if (X >= Width)
                X = Width - 1;
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
        Receiving, Animating, CameraMoving
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
        public char[,] World, PairedWorld;
        public int[,] LogicMap;
        public EntityDictionary Entities;
        public string CurrentActor;
        public int Input = 0, StepsLeft = 0, StepsTaken = 0;
        public int OffsetX = 0, OffsetY = 0;
        public Position Cursor, Camera;
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
                DungeonStart = new Dungeon(TilesetType.ROUND_ROOMS_DIAGONAL_CORRIDORS, 60, 60);
                LogicMap = DungeonStart.Level;
                spawnPoint = LogicMap.RandomMatch(Dungeon.FLOOR);
            } while (spawnPoint.Y < 0);

            PairedWorld = DungeonStart.PairLevel;
            World = DungeonStart.DLevel;
            Entity Player = new Entity("Player", "@ሂ", Color.Indigo, spawnPoint.Y, spawnPoint.X, 5, 5, 0); // \u1202
            CurrentActor = "Player";
            Cursor = new Position(spawnPoint.Y, spawnPoint.X);
            Camera = new Position(spawnPoint.Y, spawnPoint.X);
            Player.Seeker = new Dijkstra(LogicMap);
            Player.Seeker.SetGoal(Player.Y, Player.X);
            Player.Seeker.Scan();
            Entities.Add("Player", Player);
            for (int i = 0; i < 25; i++)
            {
                spawnPoint = LogicMap.RandomMatch(Dungeon.FLOOR);
                if (spawnPoint.Y >= 0 && !Entities.Contains(spawnPoint))
                {
                    Entity baddie = new Entity("Baddie " + (char)(65 + i), "b" + IconGlyphs.RandomElement(), BloodRed, spawnPoint.Y, spawnPoint.X, 4, XSSR.Next(1, 5), -1);
                    baddie.Seeker = new Dijkstra(LogicMap);
                    baddie.Seeker.SetGoal(baddie.Y, baddie.X);
                    baddie.Seeker.Scan();
                    Entities.Add(baddie);
                }
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
            XSSR.Seed(System.DateTime.UtcNow.Ticks); //0x1337FEEDBEEFBA5E
            Self = new Entry();
            Terminal.Open();
            Terminal.Set("log: level=trace");
            Terminal.Set("window: title='Dungeon Rising', size=90x30; font: Rogue-Zodiac-12x24.png, size=12x24, codepage=custom.txt;");
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
            int top = Camera.Y - 12, bottom = Camera.Y + 12, left = Camera.X - 12, right = Camera.X + 12;
            if (top < 0)
            {
                bottom -= top;
                top = 0;
            }
            if (bottom > World.GetUpperBound(0))
            {
                top -= bottom - World.GetUpperBound(0);
                bottom = World.GetUpperBound(0);
            }
            if (left < 0)
            {
                right -= left;
                left = 0;
            }
            if (right > World.GetUpperBound(1))
            {
                left -= right - World.GetUpperBound(1);
                right = World.GetUpperBound(1);
            }
            OffsetY = top;
            OffsetX = left;
            Terminal.Layer(0);
            for (int y = OffsetY, sy = 0; sy < 25; y++, sy++)
            {
                for (int x = OffsetX, sx = 0; sx < 25; x++, sx++)
                {
                    if (CurrentState == GameState.Receiving && seeker.CombinedMap[y, x] <= Entities[CurrentActor].MoveSpeed)
                    {
                        Terminal.BkColor(highlightColors[(currentHighlightColor + 100 - seeker.CombinedMap[y, x]) % highlightColors.Length]);
                        Terminal.Put(sx * 2 + 1, sy + 1, ' ');
                        Terminal.Put(sx * 2 + 2, sy + 1, ' ');
                    }
                    else
                    {
                        Terminal.BkColor(LightGray);
                        Terminal.Put(sx * 2 + 1, sy + 1, ' ');
                        Terminal.Put(sx * 2 + 2, sy + 1, ' ');

                    }
                }
            }
            Terminal.Layer(1);

            for (int y = OffsetY, sy = 0; sy < 25; y++, sy++)
            {
                for (int x = OffsetX, sx = 0; sx < 25; x++, sx++)
                {
                    p.Y = y;
                    p.X = x;
                    Entity e = Entities[p];
                    if (e != null)
                    {
                        Terminal.Color(e.Coloring);
                        Terminal.Put(sx * 2 + 1, sy + 1, e.Left);
                        Terminal.Put(sx * 2 + 2, sy + 1, e.Right);
                        Terminal.Color(DarkGray);
                    }
                    else if (Cursor.Y == y && Cursor.X == x)
                    {

                        Terminal.Color(playerColors[(currentPlayerColor + 3) % playerColors.Length]);
                        Terminal.Put(sx * 2 + 1, sy + 1, 'X');
                        Terminal.Put(sx * 2 + 2, sy + 1, '!');
                        Terminal.Color(DarkGray);
                    }
                    else if (Entities[CurrentActor].Faction == 0 && seeker.Path.Contains(y * seeker.Width + x))
                    {
                        Terminal.Color(playerColors[currentPlayerColor]);
                        Terminal.Put(sx * 2 + 1, sy + 1, PairedWorld[y, x * 2]);
                        Terminal.Put(sx * 2 + 2, sy + 1, PairedWorld[y, x * 2 + 1]);
                        Terminal.Color(DarkGray);
                    }
/*                    else if(seeker.CombinedMap[y, x] < 100)
                    {
                        Terminal.Print(sx * 2 + 1, sy + 1, "" + seeker.CombinedMap[y, x]);
                    }*/
                    else
                    {
                        Terminal.Put(sx * 2 + 1, sy + 1, PairedWorld[y, x * 2]);
                        Terminal.Put(sx * 2 + 2, sy + 1, PairedWorld[y, x * 2 + 1]);
                    }
                }
            }

            Terminal.Print(60,  9, "Acting: " + CurrentActor);
/*            Terminal.Print(60, 10, "Y: " + Cursor.Y + ", X: " + Cursor.X);
            Terminal.Print(60, 11, "Seeker Value: " + seeker.CombinedMap[Cursor.Y, Cursor.X]);
            Terminal.Print(60, 12, "Physical Value: " + seeker.PhysicalMap[Cursor.Y, Cursor.X]);*/

            Terminal.Refresh();
            Terminal.Clear();
            if (CurrentState == GameState.Animating)
            {
                StepsLeft = Entities.Step(CurrentActor);
                ++StepsTaken;
                if (StepsLeft <= 0 || StepsTaken > Entities[CurrentActor].MoveSpeed)
                {
                    FinishMove();
                }
            }
            else if(CurrentState == GameState.CameraMoving)
            {
                if (Camera.X < Entities[CurrentActor].X)
                {
                    Camera.X++;
                    if (Camera.X + 12 > World.GetUpperBound(1))
                        Camera.X = Entities[CurrentActor].X;
                }
                else if (Camera.X > Entities[CurrentActor].X)
                {
                    Camera.X--;
                    if (Camera.X < 12)
                        Camera.X = Entities[CurrentActor].X;
                }
                if (Camera.Y < Entities[CurrentActor].Y)
                {
                    Camera.Y++;
                    if (Camera.Y + 12 > World.GetUpperBound(0))
                        Camera.Y = Entities[CurrentActor].Y;
                }
                else if (Camera.Y > Entities[CurrentActor].Y)
                {
                    Camera.Y--;
                    if (Camera.Y < 12)
                        Camera.Y = Entities[CurrentActor].Y;
                }
                if(Camera.X == Entities[CurrentActor].X && Camera.Y == Entities[CurrentActor].Y)
                {
                    if (Entities[CurrentActor].Faction == 0)
                    {
                        CurrentState = GameState.Receiving;
                    }
                    else
                    {
                        CurrentState = GameState.Animating;
                        Entities[CurrentActor].Seeker.GetPath(Entities["Player"].Y, Entities["Player"].X);
                    }
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

            CurrentState = GameState.CameraMoving;

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
                                        Cursor.Y = (Terminal.State(Terminal.TK_MOUSE_Y) - 1) + OffsetY;
                                        Cursor.X = (Terminal.State(Terminal.TK_MOUSE_X) - 1) / 2 + OffsetX;
                                        Cursor.MakeValid(DungeonStart.Height, DungeonStart.Width);
                                        if (Entities[CurrentActor].Seeker.CombinedMap[Cursor.Y, Cursor.X] <= Entities[CurrentActor].MoveSpeed)
                                        {
                                            CurrentState = GameState.Animating;
                                        }
                                    }
                                    break;
                                case Terminal.TK_MOUSE_MOVE:
                                    {
                                        Cursor.Y = (Terminal.State(Terminal.TK_MOUSE_Y) - 1) + OffsetY;
                                        Cursor.X = (Terminal.State(Terminal.TK_MOUSE_X) - 1) / 2 + OffsetX;
                                        if (Cursor.Validate(DungeonStart.Height, DungeonStart.Width))
                                            Entities[CurrentActor].Seeker.GetPath(Cursor.Y, Cursor.X);
                                        Cursor.MakeValid(DungeonStart.Height, DungeonStart.Width);
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
