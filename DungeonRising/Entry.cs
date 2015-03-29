using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using BearLib;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;
using ProdutiveRage.UpdateWith;

//using C5;
namespace DungeonRising
{
    public enum WaitReason
    {
        CameraMoving, Receiving, WalkAnimating, AttackAnimating
    }
    public class Entry
    {
        
        private Thread _animationThread;
        private int _currentPlayerColor = 0, _currentHighlightColor = 0;

        private static Color[] _playerColors = new Color[] {
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
            }, _highlightColors = new Color[] {
                Color.FromArgb(174, 174, 174),
                Color.FromArgb(181, 181, 181),
                Color.FromArgb(188, 188, 188),
                Color.FromArgb(195, 195, 195),
                Color.FromArgb(202, 202, 202),
                Color.FromArgb(195, 195, 195),
                Color.FromArgb(188, 188, 188),
                Color.FromArgb(181, 181, 181),
            };
        private static Color _lightGray = Color.FromArgb(0xdd, 0xdd, 0xdd), _darkGray = Color.FromArgb(0x17, 0x17, 0x17), _bloodRed = Color.FromArgb(0xbb, 0x1c, 0x00);
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
        public Dictionary<string, bool> BeingDamaged;
        public string message = "";
        private Stopwatch _timer;
        private long _now;
        public static string IconGlyphs = "ሀሁሂሃሄህሆሇለሉሊላሌልሎሏሐሑሒሓሔሕሖሗመሙሚማሜ";
        public static string WeaponGlyphs = "ሂሕሗሙህሌልሐሆሊሁሃሇለሏሑሓመማ";
        public Entry()
        {
            State s = new State();
            s.Entities = new EntityDictionary();
            s.Initiative = new Schedule();
            s.TurnsLeft = 0;
            s.DungeonStart = new Dungeon(TilesetType.DEFAULT_DUNGEON, 40, 40); //ROUND_ROOMS_DIAGONAL_CORRIDORS
            var spawnPoint = s.DungeonStart.LogicWorld.RandomMatch(Dungeon.Floor);


            Seeker = new Dijkstra(s.DungeonStart.LogicWorld);
            VisualRandom = new XSRandom();
            BeingDamaged = new Dictionary<string, bool>();

//            Player.Seeker.SetGoal(Player.Pos.Y, Player.Pos.X);
//            Player.Seeker.Scan();
            
            for (int i = 0; i < 6; i++)
            {
                do
                {
                    if (s.Entities.Contains(spawnPoint))
                    {
                        spawnPoint = s.DungeonStart.LogicWorld.RandomMatch(Dungeon.Floor);
                        continue;    
                    }
                    Entity player =
                        new Entity("Hero " + ((char) (65 + i)), "@" + WeaponGlyphs[i], _playerColors[i], spawnPoint.Y,
                            spawnPoint.X, 5, 3 + XSSR.Next(4), 0).UpdateStats(health: new Gauge(10), damage: 3);
                        // \u1202
                    player.Seeker = new Dijkstra(s.DungeonStart.LogicWorld);
                    s.Entities.Add(player);
                    spawnPoint = s.DungeonStart.LogicWorld.RandomMatch(Dungeon.Floor);
                    break;
                } while (true);
            }
            
            for (int i = 0; i < 25; i++)
            {
                spawnPoint = s.DungeonStart.LogicWorld.RandomMatch(Dungeon.Floor);
                if (spawnPoint.Y >= 0 && !s.Entities.Contains(spawnPoint))
                {
                    Entity baddie = new Entity("Baddie " + (char)(65 + i), "b" + IconGlyphs.RandomElement(), _bloodRed, spawnPoint.Y, spawnPoint.X, 4, XSSR.Next(1, 5), -1);
                    baddie.Seeker = new Dijkstra(s.DungeonStart.LogicWorld);
//                    baddie.Seeker.SetGoal(baddie.Pos.Y, baddie.Pos.X);
//                    baddie.Seeker.Scan();
                    s.Entities.Add(baddie);
                }
            }
            foreach(var k in s.Entities.NameToEntity.Keys)
            {
                IEnumerable<Position> foes =
                    s.Entities.NameToEntity.Where(kv => kv.Value.Faction != s.Entities[k].Faction)
                        .Select(e => e.Value.Pos);
                s.Entities[k] = s.Entities[k].AddEnemies(foes);
                IEnumerable<Position> friends =
                    s.Entities.NameToEntity.Where(kv => kv.Value.Faction == s.Entities[k].Faction)
                        .Select(e => e.Value.Pos);
                s.Entities[k] = s.Entities[k].AddAllies(friends);

            }
            s.StepsTaken = 0;
            s.XSSRState = XSSR.GetState();
            H.S = s;

            H.ResetInitiative();
            Entity first = H.S.Entities[H.S.Initiative.PeekTurn().Actor];
            H.S.CurrentActor = first.Name;
            H.S.Cursor = first.Pos;
            H.S.Camera = first.Pos;
            if (first.Faction == 0)
            {

                Seeker.SetGoal(first.Pos.Y, first.Pos.X);
                Seeker.Scan(first);

                H.S.CurrentReason = WaitReason.Receiving;
                H.Remember();
            }


            //            Player.Seeker.GetPath(Player.Y, Player.X);
        }
        public Entry(State state)
        {
            H.S = state;
            Seeker = new Dijkstra(H.S.DungeonStart.LogicWorld);
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
            Terminal.Set("window: title='Dungeon Rising', size=90x30; font: Rogue-Zodiac-12x24.png, size=12x24, codepage=custom.txt; output: vsync=false");
            Self._animationThread = new Thread(() => {
                Self._timer = Stopwatch.StartNew();
                Self._now = Self._timer.ElapsedMilliseconds;
                while (true)
                {
                    Self.Render(Self._timer.ElapsedMilliseconds, Self._now);
                }
            });
            Self._animationThread.IsBackground = true;
            Self._animationThread.Start();
            Self.RunEntry();
        }
        public void Render(long currentTime, long previousTime)
        {
            long delta = currentTime - previousTime;
            
            if (Input == Terminal.TK_LBRACKET || Input == Terminal.TK_ESCAPE)
            {
                H.Backward();
                return;
            }
            _now = _timer.ElapsedMilliseconds;

            _currentPlayerColor = ((int)currentTime / 80) % 12;
            _currentHighlightColor = ((int)currentTime / 80) % 8;
            Position p = new Position(0, 0);
            Entity acting = H.S.Entities[H.S.CurrentActor];
            int top = H.S.Camera.Y - 12, bottom = H.S.Camera.Y + 12, left = H.S.Camera.X - 12, right = H.S.Camera.X + 12;
            if (top < 0)
            {
                bottom -= top;
                top = 0;
            }
            if (bottom > H.S.DungeonStart.World.GetUpperBound(0))
            {
                top -= bottom - H.S.DungeonStart.World.GetUpperBound(0);
                bottom = H.S.DungeonStart.World.GetUpperBound(0);
            }
            if (left < 0)
            {
                right -= left;
                left = 0;
            }
            if (right > H.S.DungeonStart.World.GetUpperBound(1))
            {
                left -= right - H.S.DungeonStart.World.GetUpperBound(1);
                right = H.S.DungeonStart.World.GetUpperBound(1);
            }
            OffsetY = top;
            OffsetX = left;
            Terminal.Layer(0);
            if (H.S.CurrentReason == WaitReason.Receiving)
            {
                for (int y = OffsetY, sy = 0; sy < 25; y++, sy++)
                {
                    for (int x = OffsetX, sx = 0; sx < 25; x++, sx++)
                    {
                        if (Seeker.CombinedMap[y, x] <= acting.Stats.MoveSpeed)
                        {
                            Terminal.BkColor(_highlightColors[(_currentHighlightColor + 100 - Seeker.CombinedMap[y, x]) % _highlightColors.Length]);
                            Terminal.Put(sx * 2 + 1, sy + 1, ' ');
                            Terminal.Put(sx * 2 + 2, sy + 1, ' ');
                        }
                        else
                        {
                            Terminal.BkColor(_lightGray);
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
                        Terminal.BkColor(_lightGray);
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
                    Entity e = H.S.Entities[p];
                    if (e != null)
                    {
                        Terminal.Color(e.Coloring);
                        if (H.S.CurrentReason == WaitReason.AttackAnimating && BeingDamaged.ContainsKey(e.Name))
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
                        Terminal.Color(_darkGray);
                    }
                    else if (H.S.Cursor.Y == y && H.S.Cursor.X == x)
                    {

                        Terminal.Color(_playerColors[(_currentPlayerColor + 3) % _playerColors.Length]);
                        Terminal.Put(sx * 2 + 1, sy + 1, '{');
                        Terminal.Put(sx * 2 + 2, sy + 1, '}');
                        Terminal.Color(_darkGray);
                    }
                    else if (acting.Faction == 0 && acting.Seeker.Path.Contains(p))
                    {
                        Terminal.Color(_playerColors[_currentPlayerColor]);
                        Terminal.Put(sx * 2 + 1, sy + 1, H.S.DungeonStart.PairedWorld[y, x * 2]);
                        Terminal.Put(sx * 2 + 2, sy + 1, H.S.DungeonStart.PairedWorld[y, x * 2 + 1]);
                        Terminal.Color(_darkGray);
                    }
/*                    else if(seeker.CombinedMap[y, x] < 100)
                    {
                        Terminal.Print(sx * 2 + 1, sy + 1, "" + seeker.CombinedMap[y, x]);
                    }*/
                    else
                    {
                        Terminal.Put(sx * 2 + 1, sy + 1, H.S.DungeonStart.PairedWorld[y, x * 2]);
                        Terminal.Put(sx * 2 + 2, sy + 1, H.S.DungeonStart.PairedWorld[y, x * 2 + 1]);
                    }
                }
            }

            Terminal.Print(60, 9, "Acting: " + H.S.CurrentActor);
            Gauge hp = H.S.Entities[H.S.CurrentActor].Stats.Health;
            Terminal.Print(60, 10, "Health: " + hp.Current + "/" + hp.Max);
            Terminal.Print(60, 12, message);
/*            Terminal.Print(60, 10, "Y: " + Cursor.Y + ", X: " + Cursor.X);
            Terminal.Print(60, 11, "Seeker Value: " + seeker.CombinedMap[Cursor.Y, Cursor.X]);
            Terminal.Print(60, 12, "Physical Value: " + seeker.PhysicalMap[Cursor.Y, Cursor.X]);*/

            Terminal.Refresh();
            Terminal.Clear();
            if (H.S.CurrentReason == WaitReason.WalkAnimating)
            {
                if (delta < 85)
                {
                    _now = previousTime; // This run didn't count toward advancing animations.
                    return;
                }
                H.S.StepsLeft = H.S.Entities.Step(H.S.CurrentActor);
                ++H.S.StepsTaken;
                if (H.S.StepsLeft <= 0 || H.S.StepsTaken > acting.Stats.MoveSpeed)
                {
                    FinishMove();
                    if(H.S.CurrentReason == WaitReason.AttackAnimating)
                    {
                        return;
                    }
                    else
                    {
                        NextTurn();
                    }
                }
            }
            else if (H.S.CurrentReason == WaitReason.AttackAnimating)
            {
                if (delta < 120)
                {
                    _now = previousTime; // This run didn't count toward advancing animations.
                    return;
                }
                NextTurn();
            }
            else if(H.S.CurrentReason == WaitReason.CameraMoving)
            {
                if (delta < 55)
                {
                    _now = previousTime; // This run didn't count toward advancing animations.
                    return;
                }
                if (H.S.Camera.X < acting.Pos.X)
                {
                    H.S.Camera.Move(0, 1);
                    if (H.S.Camera.X + 12 > H.S.DungeonStart.World.GetUpperBound(1))
                        H.S.Camera.X = acting.Pos.X;
                }
                else if (H.S.Camera.X > acting.Pos.X)
                {
                    H.S.Camera.Move(0, -1);
                    if (H.S.Camera.X < 12)
                        H.S.Camera.X = acting.Pos.X;
                }
                if (H.S.Camera.Y < acting.Pos.Y)
                {
                    H.S.Camera.Move(1, 0);
                    if (H.S.Camera.Y + 12 > H.S.DungeonStart.World.GetUpperBound(0))
                        H.S.Camera.Y = acting.Pos.Y;
                }
                else if (H.S.Camera.Y > acting.Pos.Y)
                {
                    H.S.Camera.Move(-1, 0);
                    if (H.S.Camera.Y < 12)
                        H.S.Camera.Y = acting.Pos.Y;
                }
                if (H.S.Camera.X == acting.Pos.X && H.S.Camera.Y == acting.Pos.Y)
                {
                    if (acting.Faction == 0)
                    {

                        Seeker.SetGoal(acting.Pos.Y, acting.Pos.X);
                        Seeker.Scan(acting);
                        H.S.CurrentReason = WaitReason.Receiving;
                        H.Remember();

                    }
                    else
                    {
                        H.S.CurrentReason = WaitReason.WalkAnimating;
                        acting.Seeker.GetPath(acting, acting.Pos, H.S.Entities.NameToEntity.Where(kv => kv.Value.Faction == 0).Select(kv => kv.Value.Pos), acting.Stats.MoveSpeed);
                    }
                }
            }
        }

        private void FinishMove()
        {
            
            H.S.StepsTaken = 0;
            Entity e = H.S.Entities[H.S.CurrentActor];
            
            List<Position> adj = e.AdjacentToEnemy(e.Pos);
            if(adj.Count > 0)
            {
                Position rpos = adj[XSSR.Next(adj.Count)];
                BeingDamaged.Add(H.S.Entities[rpos].Name, true);
                message = H.S.Entities.Attack(e.Pos, rpos);
                H.S.CurrentReason = WaitReason.AttackAnimating;
                
            }
            else
            {
                message = "";
            }
        }
        private void NextTurn()
        {
            BeingDamaged.Clear();
            Entity e = H.S.Entities[H.S.CurrentActor];

            e.Seeker.Reset();
            Entity next = H.S.Entities[H.S.Initiative.NextTurn().Actor];

            if (--H.S.TurnsLeft <= 0)
                H.ResetInitiative();
            H.S.Initiative.AddTurn(e.Name, e.Delay);

            H.S.CurrentReason = WaitReason.CameraMoving;

            H.S.CurrentActor = next.Name;


        }
        public void RunEntry()
        {
            do
            {
                Input = Terminal.Read();
                if(Input == Terminal.TK_S)
                {
                    H.S.XSSRState = XSSR.GetState();
                    using (FileStream fs = new FileStream("Savefile.sav", FileMode.Create, FileAccess.Write))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize(fs, H.S);
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
                switch (H.S.CurrentReason)
                {
                    case WaitReason.WalkAnimating:
                        {
                        }
                        break;
                    case WaitReason.Receiving:
                        {
                            Entity acting = H.S.Entities[H.S.CurrentActor];
                            switch (Input)
                            {
                                case Terminal.TK_MOUSE_LEFT:
                                    {
                                        Position temp = new Position((Terminal.State(Terminal.TK_MOUSE_Y) - 1) + OffsetY, (Terminal.State(Terminal.TK_MOUSE_X) - 1) / 2 + OffsetX);
                                        H.S.Cursor = temp;
                                        H.S.Cursor.MakeValid(H.S.DungeonStart.Height, H.S.DungeonStart.Width);
                                        if (acting.Seeker.Path.Contains(H.S.Cursor))
                                        {
                                            Seeker.Goals.Clear();
                                            H.S.CurrentReason = WaitReason.WalkAnimating;
                                        }
                                        else if (H.S.Cursor == acting.Pos)
                                        {
                                            acting.Seeker.Path.Add(acting.Pos); 
                                            Seeker.Goals.Clear();
                                            H.S.CurrentReason = WaitReason.WalkAnimating;
                                        }
                                    }
                                    break;
                                case Terminal.TK_MOUSE_MOVE:
                                    {
                                        Position temp = new Position((Terminal.State(Terminal.TK_MOUSE_Y) - 1) + OffsetY, (Terminal.State(Terminal.TK_MOUSE_X) - 1) / 2 + OffsetX);
                                        H.S.Cursor = temp;
                                        if (H.S.Cursor.Validate(H.S.DungeonStart.Height, H.S.DungeonStart.Width))
                                            acting.Seeker.GetPath(acting, acting.Pos, H.S.Cursor, acting.Stats.MoveSpeed);
                                        H.S.Cursor.MakeValid(H.S.DungeonStart.Height, H.S.DungeonStart.Width);
                                    }
                                    break;
                                case Terminal.TK_LEFT:
                                case Terminal.TK_KP_4:
                                case Terminal.TK_H:
                                    {
                                        if (H.S.DungeonStart.LogicWorld[H.S.Entities[H.S.CurrentActor].Pos.Y, H.S.Entities[H.S.CurrentActor].Pos.X - 1] == Dungeon.Floor)
                                        {
                                            H.S.Entities.Move(H.S.CurrentActor, 0, -1);
                                            FinishMove();
                                        }
                                    }
                                    break;
                                case Terminal.TK_RIGHT:
                                case Terminal.TK_KP_6:
                                case Terminal.TK_L:
                                    {
                                        if (H.S.DungeonStart.LogicWorld[H.S.Entities[H.S.CurrentActor].Pos.Y, H.S.Entities[H.S.CurrentActor].Pos.X + 1] == Dungeon.Floor)
                                        {
                                            H.S.Entities.Move(H.S.CurrentActor, 0, 1);
                                            FinishMove();
                                        }
                                    }
                                    break;
                                case Terminal.TK_UP:
                                case Terminal.TK_KP_8:
                                case Terminal.TK_K:
                                    {
                                        if (H.S.DungeonStart.LogicWorld[H.S.Entities[H.S.CurrentActor].Pos.Y - 1, H.S.Entities[H.S.CurrentActor].Pos.X] == Dungeon.Floor)
                                        {
                                            H.S.Entities.Move(H.S.CurrentActor, -1, 0);
                                            FinishMove();
                                        }
                                    }
                                    break;
                                case Terminal.TK_DOWN:
                                case Terminal.TK_KP_2:
                                case Terminal.TK_J:
                                    {
                                        if (H.S.DungeonStart.LogicWorld[H.S.Entities[H.S.CurrentActor].Pos.Y + 1, H.S.Entities[H.S.CurrentActor].Pos.X] == Dungeon.Floor)
                                        {
                                            H.S.Entities.Move(H.S.CurrentActor, 1, 0);
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
