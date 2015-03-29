using System;
//using C5;
using System.Collections.Immutable;
using ProdutiveRage.UpdateWith;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
namespace DungeonRising
{
    [Serializable]
    
    public class Entity : IEquatable<Entity>
    {
        public Sheet Stats { get; private set; }
        public string Name { get; private set; }
        public Position Pos { get; private set; }
        public char Left { get; private set; }
        public char Right { get; private set; }
        public Color Coloring { get; private set; }
        public int Faction { get; private set; }
        public ImmutableDictionary<Position, int> Enemies { get; private set; }
        public ImmutableDictionary<Position, int> Allies { get; private set; }
        public ImmutableDictionary<Position, int> Obstacles { get; private set; }
        public double Delay { get { return 36.0 / Stats.ActSpeed; } }

        public Dijkstra Seeker { get; set; }

        public bool Equivalent(Entity other)
        {
            return other != null && Pos == other.Pos && Name == other.Name;
        }

        public Entity(string name, Position pos, char left, char right, Color coloring, int faction, Sheet stats, Dijkstra seeker, ImmutableDictionary<Position, int> enemies,
            ImmutableDictionary<Position, int> allies, ImmutableDictionary<Position, int> obstacles)
        {
            Name = name;
            Pos = pos;
            Left = left;
            Right = right;
            Coloring = coloring;
            Faction = faction;
            Stats = stats;
            Seeker = seeker;
            Enemies = enemies;
            Allies = allies;
            Obstacles = obstacles;
        }
        public Entity(string representation, int y, int x)
        {
            Name = "";
            Left = representation[0];
            Right = representation[1];
            Coloring = Color.DimGray;
            Pos = new Position(y, x);
            Stats = new Sheet(5, 1, 5, 1);
            Faction = 0;
            Enemies = ImmutableDictionary<Position, int>.Empty;
            Allies = ImmutableDictionary<Position, int>.Empty.Add(Pos, Dungeon.Wall);
            Obstacles = ImmutableDictionary<Position, int>.Empty;
        }
        public Entity(string name, string representation, Color coloring, int y, int x, int moveSpeed, int actSpeed, int faction)
        {
            Name = name;
            Left = representation[0];
            Right = representation[1];
            Coloring = coloring;
            Pos = new Position(y, x);
            Stats = new Sheet(5, 1, moveSpeed, actSpeed);
            Faction = faction;
            Enemies = ImmutableDictionary<Position, int>.Empty;
            Allies = ImmutableDictionary<Position, int>.Empty.Add(Pos, Dungeon.Wall);
            Obstacles = ImmutableDictionary<Position, int>.Empty;
        }
        public Entity(string name, string representation, Color coloring, int y, int x, int faction, Sheet stats)
        {
            Name = name;
            Left = representation[0];
            Right = representation[1];
            Coloring = coloring;
            Pos = new Position(y, x);
            Stats = stats;
            Faction = faction;
            Enemies = ImmutableDictionary<Position, int>.Empty;
            Allies = ImmutableDictionary<Position, int>.Empty.Add(Pos, Dungeon.Wall);
            Obstacles = ImmutableDictionary<Position, int>.Empty;
        }
        public Entity(string name, string representation, int y, int x)
        {
            Name = name;
            Left = representation[0];
            Right = representation[1];
            Coloring = Color.DimGray;
            Pos = new Position(y, x);
            Stats = new Sheet(5, 1, 5, 1);
            Faction = 0;
            Enemies = ImmutableDictionary<Position, int>.Empty;
            Allies = ImmutableDictionary<Position, int>.Empty.Add(Pos, Dungeon.Wall);
            Obstacles = ImmutableDictionary<Position, int>.Empty;
        }
        public Entity()
        {
            this.Name = "";
            this.Left = '.';
            this.Right = ' ';
            Coloring = Color.DimGray;
            Pos = new Position(0, 0);
            Stats = new Sheet(5, 1, 0, 1);
            this.Faction = 0;
            Enemies = ImmutableDictionary<Position, int>.Empty;
            Allies = ImmutableDictionary<Position, int>.Empty.Add(Pos, Dungeon.Wall);
            Obstacles = ImmutableDictionary<Position, int>.Empty;
        }


        public override bool Equals(Object obj)
        {
            if (obj == null || !(obj is Entity))
                return false;
            else
                return Pos == ((Entity)obj).Pos && Name == ((Entity)obj).Name && Stats.Equals(((Entity)obj).Stats);
        }
        public bool Equals(Entity obj)
        {
            if (obj == null)
                return false;
            else
                return Pos == obj.Pos && Name == obj.Name &&
                    Stats.Equals(obj.Stats) && Allies == obj.Allies && Obstacles == obj.Obstacles && Enemies == obj.Enemies && Faction == obj.Faction;
        }

        public Entity RemoveAlly(Position pos)
        {
            return Allies.ContainsKey(pos) ? UpdateWith(allies: Allies.Remove(pos)) : this;
        }
        public Entity UpdateAlly(Position start, Position end)
        {
            ImmutableDictionary<Position, int>.Builder tmpa = Allies.ToBuilder();
            if (tmpa.ContainsKey(start))
            {
                tmpa.Remove(start);
            }
            tmpa[end] = Dungeon.Wall;
            return UpdateWith(obstacles: tmpa.ToImmutable());
        }
        public Entity AddAlly(Position pos)
        {
            return UpdateWith(allies: Allies.SetItem(pos, Dungeon.Wall));
        }
        public Entity AddAllies(IEnumerable<Position> friends)
        {
            return UpdateWith(allies: Allies.SetItems(friends.ToDictionary(e => e, e => Dungeon.Wall)));
        }
        public Entity RemoveAllies(IEnumerable<Position> friends)
        {
            return UpdateWith(allies: Allies.RemoveRange(friends));
        }




        public Entity RemoveEnemy(Position pos)
        {
            ImmutableDictionary<Position, int> tmpo = Obstacles, tmpe = Enemies;

            if (Obstacles.ContainsKey(pos))
            {
                tmpo = Obstacles.Remove(pos);
            }
            if (Enemies.ContainsKey(pos))
            {
                tmpe = Enemies.Remove(pos);
            }
            return UpdateWith(obstacles: tmpo, enemies: tmpe);
        }
        public Entity RemoveEnemies(IEnumerable<Position> blocks)
        {
            ImmutableDictionary<Position, int>.Builder tmpo = Obstacles.ToBuilder(), tmpe = Enemies.ToBuilder();

            foreach (Position pos in blocks)
            {
                if (tmpo.ContainsKey(pos))
                    tmpo.Remove(pos);
                if (tmpe.ContainsKey(pos))
                    tmpe.Remove(pos);
            }
            return UpdateWith(obstacles: tmpo.ToImmutable(), enemies: tmpe.ToImmutable());
        }
        public Entity UpdateEnemy(Position start, Position end)
        {
            ImmutableDictionary<Position, int>.Builder tmpo = Obstacles.ToBuilder(), tmpe = Enemies.ToBuilder();
            if (tmpo.ContainsKey(start))
            {
                tmpo.Remove(start);
            }

            if (tmpe.ContainsKey(start))
            {
                tmpe.Remove(start);
            }
            tmpo[end] = Dungeon.Wall;
            tmpe[end] = Dungeon.Wall;
            return UpdateWith(obstacles: tmpo.ToImmutable(), enemies: tmpe.ToImmutable());
        }
        public List<Position> AdjacentToEnemy(Position pos)
        {
            List<Position> adjacent = new List<Position>();
            if (Enemies.ContainsKey(new Position(pos.Y, pos.X + 1)))
                adjacent.Add(new Position(pos.Y, pos.X + 1));
            if (Enemies.ContainsKey(new Position(pos.Y, pos.X - 1)))
                adjacent.Add(new Position(pos.Y, pos.X - 1));
            if (Enemies.ContainsKey(new Position(pos.Y + 1, pos.X)))
                adjacent.Add(new Position(pos.Y + 1, pos.X));
            if (Enemies.ContainsKey(new Position(pos.Y - 1, pos.X)))
                adjacent.Add(new Position(pos.Y - 1, pos.X));

            return adjacent;
        }
        public Entity AddEnemy(Position pos)
        {
            return UpdateWith(obstacles: Obstacles.SetItem(pos, Dungeon.Wall),
                enemies: Enemies.SetItem(pos, Dungeon.Wall));

        }
        public Entity AddEnemies(IEnumerable<Position> blocks)
        {
            return UpdateWith(obstacles: Obstacles.SetItems(blocks.ToDictionary(e => e, e => Dungeon.Wall)), enemies: Enemies.SetItems(blocks.ToDictionary(e => e, e => Dungeon.Wall)));
        }
        public Entity SetEnemies(Dictionary<Position, int> other)
        {
            return UpdateWith(obstacles: other.ToImmutableDictionary(), enemies: other.ToImmutableDictionary());
        }
        public Entity SetEnemies(IEnumerable<Position> blocks)
        {
            return UpdateWith(obstacles: blocks.ToImmutableDictionary(e => e, e => Dungeon.Wall), enemies: blocks.ToImmutableDictionary(e => e, e => Dungeon.Wall));
        }



        public Entity RemoveObstacle(Position pos)
        {
            ImmutableDictionary<Position, int> tmpo = Obstacles;

            if (Obstacles.ContainsKey(pos))
            {
                tmpo = Obstacles.Remove(pos);
            }
            return UpdateWith(obstacles: tmpo);
        }
        public Entity RemoveObstacles(IEnumerable<Position> blocks)
        {
            ImmutableDictionary<Position, int>.Builder tmpo = Obstacles.ToBuilder();

            foreach (Position pos in blocks)
            {
                if (tmpo.ContainsKey(pos))
                    tmpo.Remove(pos);
            }
            return UpdateWith(obstacles: tmpo.ToImmutable());
        }
        public Entity UpdateObstacle(Position start, Position end)
        {
            ImmutableDictionary<Position, int>.Builder tmpo = Obstacles.ToBuilder();
            if (tmpo.ContainsKey(start))
            {
                tmpo.Remove(start);
            }
            tmpo[end] = Dungeon.Wall;
            return UpdateWith(obstacles: tmpo.ToImmutable());
        }

        public Entity AddObstacle(Position pos)
        {
            return UpdateWith(obstacles: Obstacles.SetItem(pos, Dungeon.Wall));
        }
        public Entity AddObstacles(IEnumerable<Position> blocks)
        {
            return UpdateWith(obstacles: Obstacles.SetItems(blocks.ToDictionary(e => e, e => Dungeon.Wall)));

        }
        public Entity SetObstacles(Dictionary<Position, int> other)
        {
            return UpdateWith(obstacles: other.ToImmutableDictionary());
        }
        public Entity SetObstacles(IEnumerable<Position> blocks)
        {
            return UpdateWith(obstacles: blocks.ToImmutableDictionary(e => e, e => Dungeon.Wall));
        }



        public Entity RemoveAll(Position pos)
        {
            ImmutableDictionary<Position, int> tmpo = Obstacles, tmpe = Enemies, tmpa = Allies;

            if (Obstacles.ContainsKey(pos))
            {
                tmpo = Obstacles.Remove(pos);
            }
            if (Enemies.ContainsKey(pos))
            {
                tmpe = Enemies.Remove(pos);
            }
            if (Allies.ContainsKey(pos))
            {
                tmpa = Allies.Remove(pos);
            }
            return UpdateWith(allies: tmpa, obstacles: tmpo, enemies: tmpe);
        }


        public Entity UpdateWith(
Optional<string> name = new Optional<string>(),
Optional<Position> pos = new Optional<Position>(),
Optional<char> left = new Optional<char>(),
Optional<char> right = new Optional<char>(),
Optional<Color> coloring = new Optional<Color>(),
Optional<int> faction = new Optional<int>(),
Optional<Sheet> stats = new Optional<Sheet>(),
Optional<Dijkstra> seeker = new Optional<Dijkstra>(),
Optional<ImmutableDictionary<Position, int>> enemies = new Optional<ImmutableDictionary<Position, int>>(),
Optional<ImmutableDictionary<Position, int>> allies = new Optional<ImmutableDictionary<Position, int>>(),
Optional<ImmutableDictionary<Position, int>> obstacles = new Optional<ImmutableDictionary<Position, int>>())
        {
            return new Entity(name.GetValue(Name), pos.GetValue((Pos)), left.GetValue(Left), right.GetValue(Right), coloring.GetValue(Coloring),
                faction.GetValue(Faction), stats.GetValue(Stats), seeker.GetValue(Seeker), enemies.GetValue(Enemies), allies.GetValue(Allies), obstacles.GetValue(Obstacles));
        }
        public Entity UpdateStats(
Optional<Gauge> health = new Optional<Gauge>(),
Optional<int> damage = new Optional<int>(),
Optional<int> moveSpeed = new Optional<int>(),
Optional<int> actSpeed = new Optional<int>())
        {
            return UpdateWith(stats: Stats.UpdateWith(health, damage, moveSpeed, actSpeed));
        }
    }

    [Serializable]
    public class EntityDictionary : IEnumerable<Entity>
    {
        public ImmutableDictionary<string, Entity>.Builder NameToEntity { get; set; }
        public ImmutableDictionary<Position, string>.Builder PosToName { get; set; }
        public int Count { get { return NameToEntity.Count; } }
        public EntityDictionary()
        {
            NameToEntity = ImmutableDictionary<string, Entity>.Empty.ToBuilder();
            PosToName = ImmutableDictionary<Position, string>.Empty.ToBuilder();

        }
        public EntityDictionary(IEnumerable<Entity> ents)
        {
            NameToEntity = ImmutableDictionary<string, Entity>.Empty.ToBuilder();
            NameToEntity.AddRange(ents.ToDictionary(e => e.Name));
            PosToName = ImmutableDictionary<Position, string>.Empty.ToBuilder();
            PosToName.AddRange(NameToEntity.ToDictionary(kv => kv.Value.Pos, kv => kv.Key));

        }
        public EntityDictionary(ImmutableDictionary<string, Entity> ents)
        {
            NameToEntity = ents.ToBuilder();
            PosToName = ImmutableDictionary<Position, string>.Empty.ToBuilder();
            PosToName.AddRange(NameToEntity.ToDictionary(kv => kv.Value.Pos, kv => kv.Key));

        }
        public bool Contains(Entity key)
        {
            if (key == null) return false;
            return NameToEntity.ContainsKey(key.Name);
        }
        public bool ContainsExactly(Entity key)
        {
            if (key == null) return false;
            return NameToEntity.ContainsKey(key.Name) && NameToEntity[key.Name].Equals(key);
        }
        public bool Contains(string key)
        {
            return NameToEntity.ContainsKey(key);
        }
        public bool Contains(int y, int x)
        {
            return PosToName.ContainsKey(new Position(y, x));
        }
        public bool Contains(Position p)
        {
            return PosToName.ContainsKey(p);
        }
        public void Add(Entity val)
        {
            PosToName.Add(val.Pos, val.Name);
            NameToEntity.Add(val.Name, val);
        }

        public void Add(string key, Entity val)
        {
            Entity v = val.UpdateWith(name: key);
            PosToName.Add(v.Pos, v.Name);
            NameToEntity.Add(key, v);
        }
        public void AddAll(IEnumerable<Entity> vals)
        {
            foreach (var val in vals)
            {
                Add(val);
            }
        }
        public void UpdateAll(IEnumerable<Entity> vals)
        {
            NameToEntity.UpdateAll(vals.ToImmutableDictionary(e => e.Name));
            PosToName.UpdateAll(vals.ToImmutableDictionary(e => e.Pos, e=> e.Name));
            
        }
        public void Remove(string key)
        {
            Entity e = NameToEntity[key];
            PosToName.Remove(e.Pos);
            NameToEntity.Remove(key);
            foreach(var k in NameToEntity.Keys)
            {
                NameToEntity[k] = NameToEntity[k].RemoveAll(e.Pos);
            }
            H.S.Initiative.CancelTurn(key);
        }
        public void Remove(Position key)
        {
            Entity e = NameToEntity[PosToName[key]];
            NameToEntity.Remove(e.Name);
            PosToName.Remove(key);
            foreach (var k in NameToEntity.Keys)
            {
                NameToEntity[k] = NameToEntity[k].RemoveAll(e.Pos);
            }
            H.S.Initiative.CancelTurn(e.Name);
        }

        public void Move(string key, int yMove, int xMove)
        {
            if (!NameToEntity.ContainsKey(key) || PosToName.ContainsKey(new Position(NameToEntity[key].Pos.Y + yMove, NameToEntity[key].Pos.X + xMove)))
                return;
            Entity e = NameToEntity[key];
            Position pos = e.Pos;
            PosToName.Remove(pos);
            NameToEntity.Remove(key);
            e.Pos.Move(yMove, xMove);
            Add(key, e);
            foreach (var k in NameToEntity.Keys)
            {
                if (NameToEntity[k].Faction == e.Faction)
                {
                    NameToEntity[k] = NameToEntity[k].UpdateAlly(pos, e.Pos);
                }
                else
                {
                    NameToEntity[k] = NameToEntity[k].UpdateEnemy(pos, e.Pos);
                }
            }

//            byName[key].Seeker.SetGoal(e.Pos.Y, e.Pos.X);
//            byPosition[e.Pos].Seeker.SetGoal(e.Pos.Y, e.Pos.X);

            //Seeker.GetPath(Y, X);
        }
        public void MoveDirectly(string key, Position dest)
        {
            if (!NameToEntity.ContainsKey(key) || PosToName.ContainsKey(dest))
                return;
            Entity e = NameToEntity[key];
            Position pos = e.Pos;
            PosToName.Remove(pos);
            NameToEntity.Remove(key);
            Add(key, e.UpdateWith(pos: dest));
            foreach (var k in NameToEntity.Keys)
            {
                if (NameToEntity[k].Faction == e.Faction)
                {
                    NameToEntity[k] = NameToEntity[k].UpdateAlly(pos, dest);
                }
                else
                {
                    NameToEntity[k] = NameToEntity[k].UpdateEnemy(pos, dest);
                }
            }
//            byName[key].Seeker.SetGoal(dest.Y, dest.X);
//            byPosition[dest].Seeker.SetGoal(dest.Y, dest.X);

            //Seeker.GetPath(Y, X);
        }
        public int Step(string key)
        {
            if (!NameToEntity.ContainsKey(key))
                return 0;
            Entity e = NameToEntity[key];
            if (e.Seeker.Path == null || e.Seeker.Path.Count == 0)
                return 0;
            Position nxt = e.Seeker.Path.First();
            e.Seeker.Path.RemoveAt(0);
            MoveDirectly(key, nxt);
            return e.Seeker.Path.Count;
        }

        public string Attack(Position attackerKey, Position defenderKey)
        {
            Entity attacker = NameToEntity[PosToName[attackerKey]], defender = NameToEntity[PosToName[defenderKey]];
            Entity result = defender.UpdateStats(health: defender.Stats.Health - attacker.Stats.Damage);
            string summary = (defender.Stats.Health.Current - result.Stats.Health.Current) + " DAMAGE";
            if (result.Stats.Health.Current <= 0)
            {
                Remove(defenderKey);
                return "WASTED";
            }
            else
            {
                PosToName[defenderKey] = result.Name;
                NameToEntity[defender.Name] = result;
            }
            return summary;
        }


        public Entity this[string key]
        {
            get
            {
                return NameToEntity[key];
            }
            set
            {
                Entity v = value;
                v = v.UpdateWith(name: key);
                NameToEntity[key] = v;
                PosToName[v.Pos] = v.Name;
            }
        }
        public Entity this[int y, int x]
        {
            get
            {
                Position p = new Position(y, x);
                if(PosToName.ContainsKey(p))
                    return NameToEntity[PosToName[p]];
                return null;
            }
            set
            {
                if (value.Name == "") throw new ArgumentException("Entity.Name must not be empty.");
                Position p = new Position(y, x);
                Entity v = value.UpdateWith(pos: p);
                PosToName[p] = v.Name;
                NameToEntity[v.Name] = v;
            }
        }
        public Entity this[Position p]
        {
            get
            {
                if (PosToName.ContainsKey(p))
                    return NameToEntity[PosToName[p]];
                return null;
            }
            set
            {
                if (value.Name == "") throw new ArgumentException("Entity.Name must not be empty.");
                PosToName[p] = value.Name;
                NameToEntity[value.Name] = value;
            }
        }
        IEnumerator<Entity> IEnumerable<Entity>.GetEnumerator()
        {
            return NameToEntity.Values.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return NameToEntity.Values.GetEnumerator();
        }
        public Entity[] ToArray()
        {
            return NameToEntity.Values.ToArray();
        }
        public ImmutableDictionary<string, Entity> ToImmutable()
        {
            return NameToEntity.ToImmutable();
        }

    }

}
