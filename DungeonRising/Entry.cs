using System;
//using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using BearLib;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;
using C5;
namespace DungeonRising
{
    public enum WaitReason
    {
        CameraMoving, Receiving, WalkAnimating, AttackAnimating
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
        public int Input = 0;
        public Dijkstra Seeker;
        public XSRandom VisualRandom;
        public HashDictionary<string, bool> BeingDamaged;
        private Stopwatch timer;
        private long now;
        public static string IconGlyphs = "ሀሁሂሃሄህሆሇለሉሊላሌልሎሏሐሑሒሓሔሕሖሗመሙሚማሜ";
        public Entry()
        {
            State S = new State();
            S.Entities = new EntityDictionary();
            S.Initiative = new Schedule();
            S.TurnsLeft = 0;
            Position spawnPoint = new Position(-1, -1);
            S.DungeonStart = new Dungeon(TilesetType.DEFAULT_DUNGEON, 40, 40); //ROUND_ROOMS_DIAGONAL_CORRIDORS
            spawnPoint = S.DungeonStart.LogicWorld.RandomMatch(Dungeon.FLOOR);
            

            Entity Player = new Entity("Player", "@ሂ", Color.Indigo, spawnPoint.Y, spawnPoint.X, 5, 5, 0); // \u1202
            Player.Stats.Damage = 3;
            Player.Stats.Health.Max = 20;
            Player.Stats.Health.Current = 20;
            S.CurrentActor = "Player";
            S.Cursor = new Position(spawnPoint.Y, spawnPoint.X);
            S.Camera = new Position(spawnPoint.Y, spawnPoint.X);
            Player.Seeker = new Dijkstra(S.DungeonStart.LogicWorld);

            Seeker = new Dijkstra(S.DungeonStart.LogicWorld);
            VisualRandom = new XSRandom();
            BeingDamaged = new HashDictionary<string, bool>();

//            Player.Seeker.SetGoal(Player.Pos.Y, Player.Pos.X);
//            Player.Seeker.Scan();
            S.Entities.Add("Player", Player);
            for (int i = 0; i < 25; i++)
            {
                spawnPoint = S.DungeonStart.LogicWorld.RandomMatch(Dungeon.FLOOR);
                if (spawnPoint.Y >= 0 && !S.Entities.Contains(spawnPoint))
                {
                    Entity baddie = new Entity("Baddie " + (char)(65 + i), "b" + IconGlyphs.RandomElement(), BloodRed, spawnPoint.Y, spawnPoint.X, 4, XSSR.Next(1, 5), -1);
                    baddie.Seeker = new Dijkstra(S.DungeonStart.LogicWorld);
//                    baddie.Seeker.SetGoal(baddie.Pos.Y, baddie.Pos.X);
//                    baddie.Seeker.Scan();
                    S.Entities.Add(baddie);
                }
            }
            foreach(Entity ent in S.Entities)
            {
                ent.Seeker.AddObstacles(S.Entities.byPosition.Keys);
                ent.Seeker.RemoveObstacles(S.Entities.byPosition.Filter(kv => kv.Value.Faction == ent.Faction).Select(e => e.Key));
            }
            S.StepsTaken = 0;
            S.XSSRState = XSSR.GetState();
            Chariot.S = S;

            Chariot.ResetInitiative();
            Entity first = Chariot.S.Entities[Chariot.S.Initiative.PeekTurn().Actor];
            if (first.Faction == 0)
            {
                Seeker.SetGoal(first.Pos.Y, first.Pos.X);
                Seeker.SetObstacles(first.Seeker.obstacles);
                Seeker.Scan();

                Chariot.S.CurrentReason = WaitReason.Receiving;
                Chariot.Remember();
            }


            //            Player.Seeker.GetPath(Player.Y, Player.X);
        }
        public Entry(State state)
        {
            Chariot.S = state;
            Seeker = new Dijkstra(Chariot.S.DungeonStart.LogicWorld);
            VisualRandom = new XSRandom();
        }
        public static Entry Self = null;
        
        public static void Run()
        {
            bool initialized = false;
            if(File.Exists("Savefile.sav"))
            {
                try
                {
                    using (FileStream fs = new FileStream("Savefile.sav", FileMode.Open, FileAccess.Read))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        if (fs.Length < 64)
                        {
                            initialized = false;
                        }
                        else
                        {
                            Object obj = bf.Deserialize(fs);
                            Self = new Entry(((State)obj).Fix());
                            initialized = true;
                        }
                    }
                    if(!initialized)
                    {
                        File.Delete("Savefile.sav");
                    }
                        //                    Self = new Entry(JsonConvert.DeserializeObject<State>(File.ReadAllText("Savefile.json")));
                } catch(Exception e)
                {
                    initialized = false;
                    throw e;
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
                Self.timer = Stopwatch.StartNew();
                Self.now = Self.timer.ElapsedMilliseconds;
                while (true)
                {
                    Self.Render(Self.timer.ElapsedMilliseconds, Self.now);
                }
            });
            Self.AnimationThread.IsBackground = true;
            Self.AnimationThread.Start();
            Self.RunEntry();
        }
        public void Render(long currentTime, long previousTime)
        {
            long delta = currentTime - previousTime;
            
            if (Input == Terminal.TK_LBRACKET || Input == Terminal.TK_ESCAPE)
            {
                Chariot.Backward();
                return;
            }
            now = timer.ElapsedMilliseconds;

            currentPlayerColor = ((int)currentTime / 80) % 12;
            currentHighlightColor = ((int)currentTime / 80) % 8;
            Position p = new Position(0, 0);
            Entity acting = Chariot.S.Entities[Chariot.S.CurrentActor];
            int top = Chariot.S.Camera.Y - 12, bottom = Chariot.S.Camera.Y + 12, left = Chariot.S.Camera.X - 12, right = Chariot.S.Camera.X + 12;
            if (top < 0)
            {
                bottom -= top;
                top = 0;
            }
            if (bottom > Chariot.S.DungeonStart.World.GetUpperBound(0))
            {
                top -= bottom - Chariot.S.DungeonStart.World.GetUpperBound(0);
                bottom = Chariot.S.DungeonStart.World.GetUpperBound(0);
            }
            if (left < 0)
            {
                right -= left;
                left = 0;
            }
            if (right > Chariot.S.DungeonStart.World.GetUpperBound(1))
            {
                left -= right - Chariot.S.DungeonStart.World.GetUpperBound(1);
                right = Chariot.S.DungeonStart.World.GetUpperBound(1);
            }
            OffsetY = top;
            OffsetX = left;
            Terminal.Layer(0);
            if (Chariot.S.CurrentReason == WaitReason.Receiving)
            {
                for (int y = OffsetY, sy = 0; sy < 25; y++, sy++)
                {
                    for (int x = OffsetX, sx = 0; sx < 25; x++, sx++)
                    {
                        if (Seeker.CombinedMap[y, x] <= acting.Stats.MoveSpeed)
                        {
                            Terminal.BkColor(highlightColors[(currentHighlightColor + 100 - Seeker.CombinedMap[y, x]) % highlightColors.Length]);
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
            }
            else
            {
                for (int y = OffsetY, sy = 0; sy < 25; y++, sy++)
                {
                    for (int x = OffsetX, sx = 0; sx < 25; x++, sx++)
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
                    Entity e = Chariot.S.Entities[p];
                    if (e != null)
                    {
                        Terminal.Color(e.Coloring);
                        if (Chariot.S.CurrentReason == WaitReason.AttackAnimating && BeingDamaged.Contains(e.Name))
                        {
                            Terminal.Layer(3);
                            Terminal.PutExt(sx * 2 + 1, sy + 1, VisualRandom.Next(7) - 3, VisualRandom.Next(7) - 3, e.Left);
                            Terminal.PutExt(sx * 2 + 2, sy + 1, VisualRandom.Next(7) - 3, VisualRandom.Next(7) - 3, e.Right);
                            Terminal.Layer(1);
                            Terminal.Put(sx * 2 + 1, sy + 1, ' ');
                            Terminal.Put(sx * 2 + 2, sy + 1, ' ');
                        }
                        else
                        {
                            Terminal.Put(sx * 2 + 1, sy + 1, e.Left);
                            Terminal.Put(sx * 2 + 2, sy + 1, e.Right);
                        }
                        Terminal.Color(DarkGray);
                    }
                    else if (Chariot.S.Cursor.Y == y && Chariot.S.Cursor.X == x)
                    {

                        Terminal.Color(playerColors[(currentPlayerColor + 3) % playerColors.Length]);
                        Terminal.Put(sx * 2 + 1, sy + 1, '{');
                        Terminal.Put(sx * 2 + 2, sy + 1, '}');
                        Terminal.Color(DarkGray);
                    }
                    else if (acting.Faction == 0 && acting.Seeker.Path.Contains(p))
                    {
                        Terminal.Color(playerColors[currentPlayerColor]);
                        Terminal.Put(sx * 2 + 1, sy + 1, Chariot.S.DungeonStart.PairedWorld[y, x * 2]);
                        Terminal.Put(sx * 2 + 2, sy + 1, Chariot.S.DungeonStart.PairedWorld[y, x * 2 + 1]);
                        Terminal.Color(DarkGray);
                    }
/*                    else if(seeker.CombinedMap[y, x] < 100)
                    {
                        Terminal.Print(sx * 2 + 1, sy + 1, "" + seeker.CombinedMap[y, x]);
                    }*/
                    else
                    {
                        Terminal.Put(sx * 2 + 1, sy + 1, Chariot.S.DungeonStart.PairedWorld[y, x * 2]);
                        Terminal.Put(sx * 2 + 2, sy + 1, Chariot.S.DungeonStart.PairedWorld[y, x * 2 + 1]);
                    }
                }
            }

            Terminal.Print(60,  9, "Acting: " + Chariot.S.CurrentActor);
/*            Terminal.Print(60, 10, "Y: " + Cursor.Y + ", X: " + Cursor.X);
            Terminal.Print(60, 11, "Seeker Value: " + seeker.CombinedMap[Cursor.Y, Cursor.X]);
            Terminal.Print(60, 12, "Physical Value: " + seeker.PhysicalMap[Cursor.Y, Cursor.X]);*/

            Terminal.Refresh();
            Terminal.Clear();
            if (Chariot.S.CurrentReason == WaitReason.WalkAnimating)
            {
                if (delta < 85)
                {
                    now = previousTime; // This run didn't count toward advancing animations.
                    return;
                }
                Chariot.S.StepsLeft = Chariot.S.Entities.Step(Chariot.S.CurrentActor);
                ++Chariot.S.StepsTaken;
                if (Chariot.S.StepsLeft <= 0 || Chariot.S.StepsTaken > acting.Stats.MoveSpeed)
                {
                    FinishMove();
                    if(Chariot.S.CurrentReason == WaitReason.AttackAnimating)
                    {
                        return;
                    }
                    else
                    {
                        NextTurn();
                    }
                }
            }
            else if (Chariot.S.CurrentReason == WaitReason.AttackAnimating)
            {
                if (delta < 120)
                {
                    now = previousTime; // This run didn't count toward advancing animations.
                    return;
                }
                NextTurn();
            }
            else if(Chariot.S.CurrentReason == WaitReason.CameraMoving)
            {
                if (delta < 55)
                {
                    now = previousTime; // This run didn't count toward advancing animations.
                    return;
                }
                if (Chariot.S.Camera.X < acting.Pos.X)
                {
                    Chariot.S.Camera.Move(0, 1);
                    if (Chariot.S.Camera.X + 12 > Chariot.S.DungeonStart.World.GetUpperBound(1))
                        Chariot.S.Camera.X = acting.Pos.X;
                }
                else if (Chariot.S.Camera.X > acting.Pos.X)
                {
                    Chariot.S.Camera.Move(0, -1);
                    if (Chariot.S.Camera.X < 12)
                        Chariot.S.Camera.X = acting.Pos.X;
                }
                if (Chariot.S.Camera.Y < acting.Pos.Y)
                {
                    Chariot.S.Camera.Move(1, 0);
                    if (Chariot.S.Camera.Y + 12 > Chariot.S.DungeonStart.World.GetUpperBound(0))
                        Chariot.S.Camera.Y = acting.Pos.Y;
                }
                else if (Chariot.S.Camera.Y > acting.Pos.Y)
                {
                    Chariot.S.Camera.Move(-1, 0);
                    if (Chariot.S.Camera.Y < 12)
                        Chariot.S.Camera.Y = acting.Pos.Y;
                }
                if (Chariot.S.Camera.X == acting.Pos.X && Chariot.S.Camera.Y == acting.Pos.Y)
                {
                    if (acting.Faction == 0)
                    {

                        Seeker.SetGoal(acting.Pos.Y, acting.Pos.X);
                        Seeker.SetObstacles(acting.Seeker.obstacles);
                        Seeker.Scan();
                        Chariot.S.CurrentReason = WaitReason.Receiving;
                        Chariot.Remember();

                    }
                    else
                    {
                        Chariot.S.CurrentReason = WaitReason.WalkAnimating;
                        acting.Seeker.GetPath(acting.Pos, Chariot.S.Entities["Player"].Pos, acting.Stats.MoveSpeed);
                    }
                }
            }
        }

        private void FinishMove()
        {

            
            Chariot.S.StepsTaken = 0;
            Entity e = Chariot.S.Entities[Chariot.S.CurrentActor];
            ArrayList<Position> adj = e.Seeker.AdjacentToObstacle(e.Pos);
            if(adj.Count > 0)
            {
                Position rpos = adj[XSSR.Next(adj.Count)];
                BeingDamaged.Add(Chariot.S.Entities[rpos].Name, true);
                Chariot.S.Entities.Attack(e.Pos, rpos);
                Chariot.S.CurrentReason = WaitReason.AttackAnimating;
                
            }
        }
        private void NextTurn()
        {
            BeingDamaged.Clear();
            Entity e = Chariot.S.Entities[Chariot.S.CurrentActor];

            e.Seeker.Reset();
            Entity next = Chariot.S.Entities[Chariot.S.Initiative.NextTurn().Actor];

            if (--Chariot.S.TurnsLeft <= 0)
                Chariot.ResetInitiative();
            Chariot.S.Initiative.AddTurn(e.Name, e.Delay);

            Chariot.S.CurrentReason = WaitReason.CameraMoving;

            Chariot.S.CurrentActor = next.Name;


        }
        public void RunEntry()
        {
            do
            {
                Input = Terminal.Read();
                if(Input == Terminal.TK_S)
                {
                    Chariot.S.XSSRState = XSSR.GetState();
                    using (FileStream fs = new FileStream("Savefile.sav", FileMode.Create, FileAccess.Write))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize(fs, Chariot.S);
                    }
//                    File.WriteAllText("Savefile.json", JsonConvert.SerializeObject(S));
                }
                else if (Input == Terminal.TK_LBRACKET)
                {
                    continue;
                }
                else if (Input == Terminal.TK_RBRACKET)
                {
                    continue;
                }
                switch (Chariot.S.CurrentReason)
                {
                    case WaitReason.WalkAnimating:
                        {
                        }
                        break;
                    case WaitReason.Receiving:
                        {
                            Entity acting = Chariot.S.Entities[Chariot.S.CurrentActor];
                            switch (Input)
                            {
                                case Terminal.TK_MOUSE_LEFT:
                                    {
                                        Position temp = new Position((Terminal.State(Terminal.TK_MOUSE_Y) - 1) + OffsetY, (Terminal.State(Terminal.TK_MOUSE_X) - 1) / 2 + OffsetX);
                                        Chariot.S.Cursor = temp;
                                        Chariot.S.Cursor.MakeValid(Chariot.S.DungeonStart.Height, Chariot.S.DungeonStart.Width);
                                        if (acting.Seeker.Path.Contains(Chariot.S.Cursor))
                                        {
                                            Seeker.goals.Clear();
                                            Chariot.S.CurrentReason = WaitReason.WalkAnimating;
                                        }
                                        else if (Chariot.S.Cursor == acting.Pos)
                                        {
                                            acting.Seeker.Path.Add(acting.Pos); 
                                            Seeker.goals.Clear();
                                            Chariot.S.CurrentReason = WaitReason.WalkAnimating;
                                        }
                                    }
                                    break;
                                case Terminal.TK_MOUSE_MOVE:
                                    {
                                        Position temp = new Position((Terminal.State(Terminal.TK_MOUSE_Y) - 1) + OffsetY, (Terminal.State(Terminal.TK_MOUSE_X) - 1) / 2 + OffsetX);
                                        Chariot.S.Cursor = temp;
                                        if (Chariot.S.Cursor.Validate(Chariot.S.DungeonStart.Height, Chariot.S.DungeonStart.Width))
                                            acting.Seeker.GetPath(acting.Pos, Chariot.S.Cursor, acting.Stats.MoveSpeed);
                                        Chariot.S.Cursor.MakeValid(Chariot.S.DungeonStart.Height, Chariot.S.DungeonStart.Width);
                                    }
                                    break;
                                case Terminal.TK_LEFT:
                                case Terminal.TK_KP_4:
                                case Terminal.TK_H:
                                    {
                                        if (Chariot.S.DungeonStart.LogicWorld[Chariot.S.Entities[Chariot.S.CurrentActor].Pos.Y, Chariot.S.Entities[Chariot.S.CurrentActor].Pos.X - 1] == Dungeon.FLOOR)
                                        {
                                            Chariot.S.Entities.Move(Chariot.S.CurrentActor, 0, -1);
                                            FinishMove();
                                        }
                                    }
                                    break;
                                case Terminal.TK_RIGHT:
                                case Terminal.TK_KP_6:
                                case Terminal.TK_L:
                                    {
                                        if (Chariot.S.DungeonStart.LogicWorld[Chariot.S.Entities[Chariot.S.CurrentActor].Pos.Y, Chariot.S.Entities[Chariot.S.CurrentActor].Pos.X + 1] == Dungeon.FLOOR)
                                        {
                                            Chariot.S.Entities.Move(Chariot.S.CurrentActor, 0, 1);
                                            FinishMove();
                                        }
                                    }
                                    break;
                                case Terminal.TK_UP:
                                case Terminal.TK_KP_8:
                                case Terminal.TK_K:
                                    {
                                        if (Chariot.S.DungeonStart.LogicWorld[Chariot.S.Entities[Chariot.S.CurrentActor].Pos.Y - 1, Chariot.S.Entities[Chariot.S.CurrentActor].Pos.X] == Dungeon.FLOOR)
                                        {
                                            Chariot.S.Entities.Move(Chariot.S.CurrentActor, -1, 0);
                                            FinishMove();
                                        }
                                    }
                                    break;
                                case Terminal.TK_DOWN:
                                case Terminal.TK_KP_2:
                                case Terminal.TK_J:
                                    {
                                        if (Chariot.S.DungeonStart.LogicWorld[Chariot.S.Entities[Chariot.S.CurrentActor].Pos.Y + 1, Chariot.S.Entities[Chariot.S.CurrentActor].Pos.X] == Dungeon.FLOOR)
                                        {
                                            Chariot.S.Entities.Move(Chariot.S.CurrentActor, 1, 0);
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
