using System;
//using C5;
using System.Collections.Immutable;
using ProdutiveRage.UpdateWith;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Newtonsoft.Json;
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
        public double Delay { get { return 36.0 / Stats.ActSpeed; } }
        private Dijkstra _seeker = null;
        public Dijkstra Seeker
        {
            get
            {
                /*
                 if (_seeker == null)
                {
                    _seeker = new Dijkstra(Chariot.S.DungeonStart.LogicWorld);
                }
                 */
                return _seeker;
            }
            set
            {
                _seeker = value;
            }
        }
        public bool Equivalent(Entity other)
        {
            return other != null && Pos == other.Pos && Name == other.Name;
        }

        public Entity(string name, Position pos, char left, char right, Color coloring, int faction, Sheet stats, Dijkstra seeker)
        {
            Name = name;
            Pos = pos;
            Left = left;
            Right = right;
            Coloring = coloring;
            Faction = faction;
            Stats = stats;
            Seeker = seeker;
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
                    Stats.Equals(obj.Stats);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Pos.GetHashCode() ^ Stats.GetHashCode();
        }

        public Entity UpdateWith(
Optional<string> name = new Optional<string>(),
Optional<Position> pos = new Optional<Position>(),
Optional<char> left = new Optional<char>(),
Optional<char> right = new Optional<char>(),
Optional<Color> coloring = new Optional<Color>(),
Optional<int> faction = new Optional<int>(),
Optional<Sheet> stats = new Optional<Sheet>(),
Optional<Dijkstra> seeker = new Optional<Dijkstra>())
        {
            return DefaultUpdateWithHelper.GetGenerator<Entity>()(this, name, pos, left, right, coloring, faction, stats, seeker);
        }
        public Entity UpdateStats(
Optional<Gauge> health = new Optional<Gauge>(),
Optional<int> damage = new Optional<int>(),
Optional<int> moveSpeed = new Optional<int>(),
Optional<int> actSpeed = new Optional<int>())
        {
            return UpdateWith(stats: Stats.UpdateWith(health, damage, moveSpeed, actSpeed));
        }
        public Entity UpdateKilledAction(Action whenKilled)
        {
            return UpdateWith(stats: Stats.UpdateKilledAction(whenKilled));
        }
    }

    [Serializable]
    [JsonObjectAttribute]
    public class EntityDictionary : IEnumerable<Entity>
    {
        public ImmutableDictionary<string, Entity> ByName { get; set; }
        public ImmutableDictionary<Position, Entity> ByPosition { get; set; }
        public int Count { get { return ByName.Count; } }
        public EntityDictionary()
        {
            ByName = ImmutableDictionary<string, Entity>.Empty;
            ByPosition = ImmutableDictionary<Position, Entity>.Empty;

        }
        public bool Contains(Entity key)
        {
            if (key == null) return false;
            return ByName.ContainsKey(key.Name);
        }
        public bool ContainsExactly(Entity key)
        {
            if (key == null) return false;
            return ByName.ContainsKey(key.Name) && ByName[key.Name].Equals(key);
        }
        public bool Contains(string key)
        {
            return ByName.ContainsKey(key);
        }
        public bool Contains(int y, int x)
        {
            return ByPosition.ContainsKey(new Position(y, x));
        }
        public bool Contains(Position p)
        {
            return ByPosition.ContainsKey(p);
        }
        public void Add(Entity val)
        {
            Entity v = val.UpdateKilledAction(() =>
                Remove(val.Name));
            ByPosition = ByPosition.Add(v.Pos, v);
            ByName = ByName.Add(v.Name, v);
        }

        public void Add(string key, Entity val)
        {
            Entity v = val.UpdateKilledAction(() => 
                Remove(val.Name));
            v = v.UpdateWith(name: key);
            ByPosition = ByPosition.Add(v.Pos, v);
            ByName = ByName.Add(key, v);
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
            ByName = ByName.SetItems(vals.ToImmutableDictionary(e => e.Name));
            ByPosition = ByPosition.SetItems(vals.ToImmutableDictionary(e => e.Pos));
            
        }
        public void Remove(string key)
        {
            Entity e = ByName[key];
            ByPosition = ByPosition.Remove(e.Pos);
            ByName = ByName.Remove(key);
            foreach(var kv in ByName)
            {
                kv.Value.Seeker.RemoveEnemy(e.Pos);
                kv.Value.Seeker.RemoveAlly(e.Pos);
            }
            Chariot.S.Initiative.CancelTurn(e.Name);
        }
        public void Remove(Position key)
        {
            Entity e = ByPosition[key];
            ByName = ByName.Remove(e.Name);
            ByPosition = ByPosition.Remove(key);
            foreach (var kv in ByName)
            {
                kv.Value.Seeker.RemoveEnemy(e.Pos);
                kv.Value.Seeker.RemoveAlly(e.Pos);
            }
            Chariot.S.Initiative.CancelTurn(e.Name);
        }

        public void Move(string key, int yMove, int xMove)
        {
            if (!ByName.ContainsKey(key) || ByPosition.ContainsKey(new Position(ByName[key].Pos.Y + yMove, ByName[key].Pos.X + xMove)))
                return;
            Entity e = ByName[key];
            Position pos = e.Pos;
            ByPosition = ByPosition.Remove(pos);
            ByName = ByName.Remove(key);
            e.Pos.Move(yMove, xMove);
            Add(key, e);
            foreach (var kv in ByName)
            {
                if (kv.Value.Faction == e.Faction)
                {
                    kv.Value.Seeker.UpdateAlly(pos, e.Pos);
                }
                else
                {
                    kv.Value.Seeker.UpdateEnemy(pos, e.Pos);
                }
            }

//            byName[key].Seeker.SetGoal(e.Pos.Y, e.Pos.X);
//            byPosition[e.Pos].Seeker.SetGoal(e.Pos.Y, e.Pos.X);

            //Seeker.GetPath(Y, X);
        }
        public void MoveDirectly(string key, Position dest)
        {
            if (!ByName.ContainsKey(key) || ByPosition.ContainsKey(dest))
                return;
            Entity e = ByName[key];
            Position pos = e.Pos;
            ByPosition = ByPosition = ByPosition.Remove(pos);
            ByName = ByName = ByName.Remove(key);
            Add(key, e.UpdateWith(pos: dest));
            foreach (var kv in ByName)
            {
                if (kv.Value.Faction == e.Faction)
                {
                    kv.Value.Seeker.UpdateAlly(pos, dest);
                }
                else
                {
                    kv.Value.Seeker.UpdateEnemy(pos, dest);
                }
            }
//            byName[key].Seeker.SetGoal(dest.Y, dest.X);
//            byPosition[dest].Seeker.SetGoal(dest.Y, dest.X);

            //Seeker.GetPath(Y, X);
        }
        public int Step(string key)
        {
            if (!ByName.ContainsKey(key))
                return 0;
            Entity e = ByName[key];
            if (e.Seeker.Path == null || e.Seeker.Path.Count == 0)
                return 0;
            Position nxt = e.Seeker.Path.First();
            e.Seeker.Path.RemoveAt(0);
            MoveDirectly(key, nxt);
            return e.Seeker.Path.Count;
        }

        public void Attack(Position attackerKey, Position defenderKey)
        {
            Entity attacker = ByPosition[attackerKey], defender = ByPosition[defenderKey];
            Entity result = defender.UpdateStats(health: defender.Stats.Health - attacker.Stats.Damage);
            if (ByPosition.ContainsKey(defenderKey))
            {
                ByPosition = ByPosition.SetItem(defenderKey, result);
                ByName = ByName.SetItem(defender.Name, result);
            }
        }


        public Entity this[string key]
        {
            get
            {
                return ByName[key];
            }
            set
            {
                Entity v = value;
                v = v.UpdateWith(name: key);
                ByName = ByName.SetItem(key, v);
                ByPosition = ByPosition.SetItem(v.Pos, v);
            }
        }
        public Entity this[int y, int x]
        {
            get
            {
                Position p = new Position(y, x);
                if(ByPosition.ContainsKey(p))
                    return ByPosition[p];
                return null;
            }
            set
            {
                if (value.Name == "") throw new ArgumentException("Entity.Name must not be empty.");
                Position p = new Position(y, x);
                Entity v = value.UpdateWith(pos: p);
                ByPosition = ByPosition.SetItem(p, v);
                ByName = ByName.SetItem(v.Name, v);
            }
        }
        public Entity this[Position p]
        {
            get
            {
                if (ByPosition.ContainsKey(p))
                    return ByPosition[p];
                return null;
            }
            set
            {
                if (value.Name == "") throw new ArgumentException("Entity.Name must not be empty.");
                ByPosition = ByPosition.SetItem(p, value);
                ByName = ByName.SetItem(value.Name, value);
            }
        }
        IEnumerator<Entity> IEnumerable<Entity>.GetEnumerator()
        {
            return ByName.Values.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ByName.Values.GetEnumerator();
        }
        public Entity[] ToArray()
        {
            return ByName.Values.ToArray();
        }
        public ImmutableList<Entity> ToList()
        {
            return ByName.Values.ToImmutableList();
        }

    }

}
