using System;
using C5;
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
        public Sheet Stats { get; set; }
        public string Name { get; set; }
        public Position Pos;
        public char Left { get; set; }
        public char Right { get; set; }
        public Color Coloring { get; set; }
        public int Faction { get; set; }
        public double Delay { get { return 36.0 / Stats.ActSpeed; } }
        private Dijkstra _Seeker = null;
        public Dijkstra Seeker
        {
            get
            {
                if (_Seeker == null)
                {
                    _Seeker = new Dijkstra(Chariot.S.DungeonStart.LogicWorld);
//                    _Seeker.SetGoal(Pos.Y, Pos.X);
//                    _Seeker.Scan();
                } return _Seeker;
            }
            set
            {
                _Seeker = value;
            }
        }
        public bool Equivalent(Entity other)
        {
            return other != null && Pos == other.Pos && Name == other.Name;
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

        public Entity Replicate()
        {
            return new Entity(Name, "" + Left + Right, Coloring, Pos.Y, Pos.X, Faction, Stats.Replicate());
        }
    }

    [Serializable]
    [JsonObjectAttribute]
    public class EntityDictionary : IEnumerable<Entity>
    {
        public HashDictionary<string, Entity> byName { get; set; }
        public HashDictionary<Position, Entity> byPosition { get; set; }
        public int Count { get { return byName.Count; } }
        public EntityDictionary()
        {
            byName = new HashDictionary<string, Entity>();
            byPosition = new HashDictionary<Position, Entity>();

        }
        public bool Contains(Entity key)
        {
            if (key == null) return false;
            return byName.ContainsKey(key.Name);
        }
        public bool ContainsExactly(Entity key)
        {
            if (key == null) return false;
            return byName.ContainsKey(key.Name) && byName[key.Name].Equals(key);
        }
        public bool Contains(string key)
        {
            return byName.ContainsKey(key);
        }
        public bool Contains(int y, int x)
        {
            return byPosition.ContainsKey(new Position(y, x));
        }
        public bool Contains(Position p)
        {
            return byPosition.ContainsKey(p);
        }
        public void Add(Entity val)
        {
            byPosition.Add(val.Pos, val);
            byName.Add(val.Name, val);
        }
        public void Add(string key, Entity val)
        {
            val.Name = key;
            byPosition.Add(val.Pos, val);
            byName.Add(key, val);
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
            foreach (var val in vals)
            {
                if(Contains(val))
                {
                    byName[val.Name] = val;
                    byPosition[val.Pos] = val;
                }
                else
                {
                    Add(val);
                }
            }
        }
        public void Remove(string key)
        {
            Entity e = byName[key];
            byPosition.Remove(e.Pos);
            byName.Remove(key);
        }
        public void Remove(Position key)
        {
            Entity e = byPosition[key];
            byName.Remove(e.Name);
            byPosition.Remove(key);
        }

        public void Move(string key, int yMove, int xMove)
        {
            if (!byName.ContainsKey(key) || byPosition.ContainsKey(new Position(byName[key].Pos.Y + yMove, byName[key].Pos.X + xMove)))
                return;
            Entity e = byName[key];
            Position pos = e.Pos;
            byPosition.Remove(pos);
            byName.Remove(key);
            e.Pos.Move(yMove, xMove);
            Add(key, e);
            foreach (var kv in byName)
            {
                if (kv.Value.Faction == e.Faction)
                {
                    kv.Value.Seeker.UpdateAlly(pos, e.Pos);
                }
            }

//            byName[key].Seeker.SetGoal(e.Pos.Y, e.Pos.X);
//            byPosition[e.Pos].Seeker.SetGoal(e.Pos.Y, e.Pos.X);

            //Seeker.GetPath(Y, X);
        }
        public void MoveDirectly(string key, Position dest)
        {
            if (!byName.ContainsKey(key) || byPosition.ContainsKey(dest))
                return;
            Entity e = byName[key];
            Position pos = e.Pos;
            byPosition.Remove(pos);
            byName.Remove(key);
            e.Pos = dest;
            Add(key, e);
            foreach (var kv in byName)
            {
                if (kv.Value.Faction == e.Faction)
                {
                    kv.Value.Seeker.UpdateAlly(pos, dest);
                }
            }
//            byName[key].Seeker.SetGoal(dest.Y, dest.X);
//            byPosition[dest].Seeker.SetGoal(dest.Y, dest.X);

            //Seeker.GetPath(Y, X);
        }
        public int Step(string key)
        {
            if (!byName.ContainsKey(key))
                return 0;
            Entity e = byName[key];
            if (e.Seeker.Path == null || e.Seeker.Path.Count == 0)
                return 0;
            Position nxt = e.Seeker.Path.RemoveFirst();
            MoveDirectly(key, nxt);
            return e.Seeker.Path.Count;
        }


        public Entity this[string key]
        {
            get
            {
                return byName[key];
            }
            set
            {
                Entity v = value;
                v.Name = key;
                byName[key] = v;
                byPosition[v.Pos] = v;
            }
        }
        public Entity this[int y, int x]
        {
            get
            {
                Position p = new Position(y, x);
                if(byPosition.ContainsKey(p))
                    return byPosition[p];
                return null;
            }
            set
            {
                if (value.Name == "") throw new ArgumentException("Entity.Name must not be empty.");
                value.Pos.SetAll(y, x);
                byPosition[value.Pos] = value;
                byName[value.Name] = value;
            }
        }
        public Entity this[Position p]
        {
            get
            {
                if (byPosition.ContainsKey(p))
                    return byPosition[p];
                return null;
            }
            set
            {
                if (value.Name == "") throw new ArgumentException("Entity.Name must not be empty.");
                byPosition[p] = value;
                byName[value.Name] = value;
            }
        }
        IEnumerator<Entity> IEnumerable<Entity>.GetEnumerator()
        {
            return byName.Values.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return byName.Values.GetEnumerator();
        }
        public Entity[] ToArray()
        {
            return byName.Values.ToArray();
        }
        public ArrayList<Entity> ToList()
        {
            ArrayList<Entity> ents = new ArrayList<Entity>();
            ents.AddAll(this.byName.Values);
            return ents;
        }

        public EntityDictionary Replicate()
        {
            EntityDictionary ed = new EntityDictionary();
            foreach(Entity e in byName.Values)
            {
                ed.Add(e.Replicate());
            }
            return ed;
        }
    }

}
