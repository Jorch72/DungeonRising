using System;
//using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using BearLib;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
namespace DungeonRising
{
    public enum WaitReason
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
                Color.FromArgb(174, 174, 174),
                Color.FromArgb(181, 181, 181),
                Color.FromArgb(188, 188, 188),
                Color.FromArgb(195, 195, 195),
                Color.FromArgb(202, 202, 202),
                Color.FromArgb(195, 195, 195),
                Color.FromArgb(188, 188, 188),
                Color.FromArgb(181, 181, 181),
            };
        private static Color LightGray = Color.FromArgb(0xdd, 0xdd, 0xdd), DarkGray = Color.FromArgb(0x17, 0x17, 0x17), BloodRed = Color.FromArgb(0xbb, 0x1c, 0x00);
        /*public Dungeon DungeonStart;
        public char[,] World, PairedWorld;
        public int[,] LogicMap;
        public EntityDictionary Entities;
        public string CurrentActor;
        public int StepsLeft = 0, StepsTaken = 0;
        public int OffsetX = 0, OffsetY = 0;
        public Position Cursor, Camera;
        public WaitReason CurrentState;
        public Schedule Initiative;
        public long TurnsLeft;*/
        public int OffsetX = 0, OffsetY = 0;
        public State S;
        public int Input = 0;
        public static string IconGlyphs = "ሀሁሂሃሄህሆሇለሉሊላሌልሎሏሐሑሒሓሔሕሖሗመሙሚማሜ";
        public Entry()
        {
            S = new State();
            S.Entities = new EntityDictionary();
            S.Initiative = new Schedule();
            S.TurnsLeft = 0;
            Position spawnPoint = new Position(-1, -1);
            S.DungeonStart = new Dungeon(TilesetType.ROUND_ROOMS_DIAGONAL_CORRIDORS, 60, 60);
            spawnPoint = S.DungeonStart.LogicWorld.RandomMatch(Dungeon.FLOOR);
            

            Entity Player = new Entity("Player", "@ሂ", Color.Indigo, spawnPoint.Y, spawnPoint.X, 5, 5, 0); // \u1202
            S.CurrentActor = "Player";
            S.Cursor = new Position(spawnPoint.Y, spawnPoint.X);
            S.Camera = new Position(spawnPoint.Y, spawnPoint.X);
            Player.Seeker = new Dijkstra(S.DungeonStart.LogicWorld);
            Player.Seeker.SetGoal(Player.Pos.Y, Player.Pos.X);
            Player.Seeker.Scan();
            S.Entities.Add("Player", Player);
            for (int i = 0; i < 25; i++)
            {
                spawnPoint = S.DungeonStart.LogicWorld.RandomMatch(Dungeon.FLOOR);
                if (spawnPoint.Y >= 0 && !S.Entities.Contains(spawnPoint))
                {
                    Entity baddie = new Entity("Baddie " + (char)(65 + i), "b" + IconGlyphs.RandomElement(), BloodRed, spawnPoint.Y, spawnPoint.X, 4, XSSR.Next(1, 5), -1);
                    baddie.Seeker = new Dijkstra(S.DungeonStart.LogicWorld);
                    baddie.Seeker.SetGoal(baddie.Pos.Y, baddie.Pos.X);
                    baddie.Seeker.Scan();
                    S.Entities.Add(baddie);
                }
            }
            ResetInitiative();
            Entity first = S.Entities[S.Initiative.PeekTurn().Actor];
            if (first.Faction == 0)
                S.CurrentReason = WaitReason.Receiving;
            //            Player.Seeker.GetPath(Player.Y, Player.X);
        }
        public Entry(State state)
        {
            S = state;
        }
        public static Entry Self = null;
        private void ResetInitiative()
        {
            foreach (Entity e in S.Entities)
            {

                S.Initiative.AddTurn(e.Name, e.Delay);
                S.TurnsLeft += e.ActSpeed;
            }
        }
        public static void Run()
        {
            bool initialized = false;
            if(File.Exists("Savefile.json"))
            {
                try
                {
                    Self = new Entry(JsonConvert.DeserializeObject<State>(File.ReadAllText("Savefile.json")));
                    XSSR.SetState(Self.S.XSSRState);
                    initialized = true;
                } catch(Exception)
                {
                    initialized = false;
                    //throw e;
                }
            }
            if (!initialized)
            {
                XSSR.Seed(System.DateTime.UtcNow.Ticks); //0x1337FEEDBEEFBA5E
                Self = new Entry();
            }
            Terminal.Open();
            Terminal.Set("log: level=trace");
            Terminal.Set("window: title='Dungeon Rising', size=90x30; font: Rogue-Zodiac-12x24.png, size=12x24, codepage=custom.txt;");
            Self.AnimationThread = new Thread(() => {
                while (true)
                {
                    Self.currentPlayerColor = (Self.currentPlayerColor + 1) % 12;
                    Self.currentHighlightColor = (Self.currentHighlightColor + 1) % 8;
                    Self.Render();
                    Thread.Sleep(70);
                }
            });
            Self.AnimationThread.IsBackground = true;
            Self.AnimationThread.Start();
            Self.RunEntry();
        }
        public void Render()
        {
            Position p = new Position(0, 0);
            Dijkstra seeker = S.Entities[S.CurrentActor].Seeker;
            int top = S.Camera.Y - 12, bottom = S.Camera.Y + 12, left = S.Camera.X - 12, right = S.Camera.X + 12;
            if (top < 0)
            {
                bottom -= top;
                top = 0;
            }
            if (bottom > S.DungeonStart.World.GetUpperBound(0))
            {
                top -= bottom - S.DungeonStart.World.GetUpperBound(0);
                bottom = S.DungeonStart.World.GetUpperBound(0);
            }
            if (left < 0)
            {
                right -= left;
                left = 0;
            }
            if (right > S.DungeonStart.World.GetUpperBound(1))
            {
                left -= right - S.DungeonStart.World.GetUpperBound(1);
                right = S.DungeonStart.World.GetUpperBound(1);
            }
            OffsetY = top;
            OffsetX = left;
            Terminal.Layer(0);
            for (int y = OffsetY, sy = 0; sy < 25; y++, sy++)
            {
                for (int x = OffsetX, sx = 0; sx < 25; x++, sx++)
                {
                    if (S.CurrentReason == WaitReason.Receiving && seeker.CombinedMap[y, x] <= S.Entities[S.CurrentActor].MoveSpeed)
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
                    Entity e = S.Entities[p];
                    if (e != null)
                    {
                        Terminal.Color(e.Coloring);
                        Terminal.Put(sx * 2 + 1, sy + 1, e.Left);
                        Terminal.Put(sx * 2 + 2, sy + 1, e.Right);
                        Terminal.Color(DarkGray);
                    }
                    else if (S.Cursor.Y == y && S.Cursor.X == x)
                    {

                        Terminal.Color(playerColors[(currentPlayerColor + 3) % playerColors.Length]);
                        Terminal.Put(sx * 2 + 1, sy + 1, 'X');
                        Terminal.Put(sx * 2 + 2, sy + 1, '!');
                        Terminal.Color(DarkGray);
                    }
                    else if (S.Entities[S.CurrentActor].Faction == 0 && seeker.Path.Contains(p))
                    {
                        Terminal.Color(playerColors[currentPlayerColor]);
                        Terminal.Put(sx * 2 + 1, sy + 1, S.DungeonStart.PairedWorld[y, x * 2]);
                        Terminal.Put(sx * 2 + 2, sy + 1, S.DungeonStart.PairedWorld[y, x * 2 + 1]);
                        Terminal.Color(DarkGray);
                    }
/*                    else if(seeker.CombinedMap[y, x] < 100)
                    {
                        Terminal.Print(sx * 2 + 1, sy + 1, "" + seeker.CombinedMap[y, x]);
                    }*/
                    else
                    {
                        Terminal.Put(sx * 2 + 1, sy + 1, S.DungeonStart.PairedWorld[y, x * 2]);
                        Terminal.Put(sx * 2 + 2, sy + 1, S.DungeonStart.PairedWorld[y, x * 2 + 1]);
                    }
                }
            }

            Terminal.Print(60,  9, "Acting: " + S.CurrentActor);
/*            Terminal.Print(60, 10, "Y: " + Cursor.Y + ", X: " + Cursor.X);
            Terminal.Print(60, 11, "Seeker Value: " + seeker.CombinedMap[Cursor.Y, Cursor.X]);
            Terminal.Print(60, 12, "Physical Value: " + seeker.PhysicalMap[Cursor.Y, Cursor.X]);*/

            Terminal.Refresh();
            Terminal.Clear();
            if (S.CurrentReason == WaitReason.Animating)
            {
                S.StepsLeft = S.Entities.Step(S.CurrentActor);
                ++S.StepsTaken;
                if (S.StepsLeft <= 0 || S.StepsTaken > S.Entities[S.CurrentActor].MoveSpeed)
                {
                    FinishMove();
                }
            }
            else if(S.CurrentReason == WaitReason.CameraMoving)
            {
                if (S.Camera.X < S.Entities[S.CurrentActor].Pos.X)
                {
                    S.Camera.X++;
                    if (S.Camera.X + 12 > S.DungeonStart.World.GetUpperBound(1))
                        S.Camera.X = S.Entities[S.CurrentActor].Pos.X;
                }
                else if (S.Camera.X > S.Entities[S.CurrentActor].Pos.X)
                {
                    S.Camera.X--;
                    if (S.Camera.X < 12)
                        S.Camera.X = S.Entities[S.CurrentActor].Pos.X;
                }
                if (S.Camera.Y < S.Entities[S.CurrentActor].Pos.Y)
                {
                    S.Camera.Y++;
                    if (S.Camera.Y + 12 > S.DungeonStart.World.GetUpperBound(0))
                        S.Camera.Y = S.Entities[S.CurrentActor].Pos.Y;
                }
                else if (S.Camera.Y > S.Entities[S.CurrentActor].Pos.Y)
                {
                    S.Camera.Y--;
                    if (S.Camera.Y < 12)
                        S.Camera.Y = S.Entities[S.CurrentActor].Pos.Y;
                }
                if (S.Camera.X == S.Entities[S.CurrentActor].Pos.X && S.Camera.Y == S.Entities[S.CurrentActor].Pos.Y)
                {
                    if (S.Entities[S.CurrentActor].Faction == 0)
                    {
                        S.CurrentReason = WaitReason.Receiving;
                    }
                    else
                    {
                        S.CurrentReason = WaitReason.Animating;
                        S.Entities[S.CurrentActor].Seeker.GetPath(S.Entities["Player"].Pos.Y, S.Entities["Player"].Pos.X);
                    }
                }
            }
        }

        private void FinishMove()
        {
            S.StepsTaken = 0;
            Entity e = S.Entities[S.CurrentActor];
            e.Seeker.Reset();
            e.Seeker.SetGoal(e.Pos.Y, e.Pos.X);
            e.Seeker.Scan();
            Entity next = S.Entities[S.Initiative.NextTurn().Actor];

            if (--S.TurnsLeft <= 0)
                ResetInitiative();
            S.Initiative.AddTurn(e.Name, e.Delay);
            S.CurrentActor = next.Name;

            S.CurrentReason = WaitReason.CameraMoving;

        }
        public void RunEntry()
        {
            do
            {
                Input = Terminal.Read();
                if(Input == Terminal.TK_S)
                {
                    S.XSSRState = XSSR.GetState();
                    File.WriteAllText("Savefile.json", JsonConvert.SerializeObject(S));
                }
                switch (S.CurrentReason)
                {
                    case WaitReason.Animating:
                        {
                        }
                        break;
                    case WaitReason.Receiving:
                        {
                            switch (Input)
                            {
                                case Terminal.TK_MOUSE_LEFT:
                                    {
                                        S.Cursor.Y = (Terminal.State(Terminal.TK_MOUSE_Y) - 1) + OffsetY;
                                        S.Cursor.X = (Terminal.State(Terminal.TK_MOUSE_X) - 1) / 2 + OffsetX;
                                        S.Cursor.MakeValid(S.DungeonStart.Height, S.DungeonStart.Width);
                                        if (S.Entities[S.CurrentActor].Seeker.CombinedMap[S.Cursor.Y, S.Cursor.X] <= S.Entities[S.CurrentActor].MoveSpeed)
                                        {
                                            S.CurrentReason = WaitReason.Animating;
                                        }
                                    }
                                    break;
                                case Terminal.TK_MOUSE_MOVE:
                                    {
                                        S.Cursor.Y = (Terminal.State(Terminal.TK_MOUSE_Y) - 1) + OffsetY;
                                        S.Cursor.X = (Terminal.State(Terminal.TK_MOUSE_X) - 1) / 2 + OffsetX;
                                        if (S.Cursor.Validate(S.DungeonStart.Height, S.DungeonStart.Width))
                                            S.Entities[S.CurrentActor].Seeker.GetPath(S.Cursor.Y, S.Cursor.X);
                                        S.Cursor.MakeValid(S.DungeonStart.Height, S.DungeonStart.Width);
                                    }
                                    break;
                                case Terminal.TK_LEFT:
                                case Terminal.TK_KP_4:
                                case Terminal.TK_H:
                                    {
                                        if (S.DungeonStart.LogicWorld[S.Entities[S.CurrentActor].Pos.Y, S.Entities[S.CurrentActor].Pos.X - 1] == Dungeon.FLOOR)
                                        {
                                            S.Entities.Move(S.CurrentActor, 0, -1);
                                            FinishMove();
                                        }
                                    }
                                    break;
                                case Terminal.TK_RIGHT:
                                case Terminal.TK_KP_6:
                                case Terminal.TK_L:
                                    {
                                        if (S.DungeonStart.LogicWorld[S.Entities[S.CurrentActor].Pos.Y, S.Entities[S.CurrentActor].Pos.X + 1] == Dungeon.FLOOR)
                                        {
                                            S.Entities.Move(S.CurrentActor, 0, 1);
                                            FinishMove();
                                        }
                                    }
                                    break;
                                case Terminal.TK_UP:
                                case Terminal.TK_KP_8:
                                case Terminal.TK_K:
                                    {
                                        if (S.DungeonStart.LogicWorld[S.Entities[S.CurrentActor].Pos.Y - 1, S.Entities[S.CurrentActor].Pos.X] == Dungeon.FLOOR)
                                        {
                                            S.Entities.Move(S.CurrentActor, -1, 0);
                                            FinishMove();
                                        }
                                    }
                                    break;
                                case Terminal.TK_DOWN:
                                case Terminal.TK_KP_2:
                                case Terminal.TK_J:
                                    {
                                        if (S.DungeonStart.LogicWorld[S.Entities[S.CurrentActor].Pos.Y + 1, S.Entities[S.CurrentActor].Pos.X] == Dungeon.FLOOR)
                                        {
                                            S.Entities.Move(S.CurrentActor, 1, 0);
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
