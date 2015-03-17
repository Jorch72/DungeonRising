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
    public class Entity
    {
        public string Name;
        public Position Pos;
        public char Left, Right;
        public Color Coloring;
        public int MoveSpeed, ActSpeed, Faction;
        public double Delay { get { return 36.0 / ActSpeed; } }
        public Dijkstra Seeker;
        public bool Equivalent(Entity other)
        {
            return other != null && Pos == other.Pos && Left == other.Left && Right == other.Right;
        }
        public Entity(string representation, int y, int x)
        {
            Name = "";
            Left = representation[0];
            Right = representation[1];
            Coloring = Color.DimGray;
            Pos = new Position(y, x);
            MoveSpeed = 5;
            ActSpeed = 1;
            Faction = 0;
        }
        public Entity(string name, string representation, Color coloring, int y, int x, int moveSpeed, int actSpeed, int faction)
        {
            Name = name;
            Left = representation[0];
            Right = representation[1];
            Coloring = coloring;
            Pos = new Position(y, x); 
            MoveSpeed = moveSpeed;
            ActSpeed = actSpeed;
            Faction = faction;
        }
        public Entity(string name, string representation, int y, int x)
        {
            Name = name;
            Left = representation[0];
            Right = representation[1];
            Coloring = Color.DimGray;
            Pos = new Position(y, x);
            MoveSpeed = 5;
            ActSpeed = 1;
            Faction = 0;
        }
        public Entity()
        {
            this.Name = "";
            this.Left = '.';
            this.Right = ' ';
            Coloring = Color.DimGray;
            Pos = new Position(0, 0);
            this.MoveSpeed = 0;
            this.ActSpeed = 1;
            this.Faction = 0;
        }
    }
    [Serializable]
    [JsonObjectAttribute]
    public class EntityDictionary : IEnumerable<Entity>
    {
        public Dictionary<string, Entity> byName;
        public Dictionary<Position, Entity> byPosition;
        public int Count { get { return byName.Count; } }
        public EntityDictionary()
        {
            byName = new Dictionary<string, Entity>();
            byPosition = new Dictionary<Position, Entity>();

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
        public void Remove(string key)
        {
            Entity e = byName[key];
            byPosition.Remove(e.Pos);
            byName.Remove(key);
        }

        public void Move(string key, int yMove, int xMove)
        {
            if (!byName.ContainsKey(key) || byPosition.ContainsKey(new Position(byName[key].Pos.Y + yMove, byName[key].Pos.X + xMove)))
                return;
            Entity e = byName[key];
            Position pos = e.Pos;
            byPosition.Remove(pos);
            byName.Remove(key);
            e.Pos.Y += yMove;
            e.Pos.X += xMove;
            Add(key, e);
            /*foreach(var kv in byName)
            {
                kv.Value.Seeker.ResetCell(pos.Y, pos.X);
                //kv.Value.Seeker.SetOccupied(e.Y, e.X);
            }*/
            e.Seeker.SetGoal(e.Pos.Y, e.Pos.X);

            foreach (var kv in byName)
            {
                kv.Value.Seeker.Scan();
            }
            //Seeker.GetPath(Y, X);
        }
        public int Step(string key)
        {
            if (!byName.ContainsKey(key))
                return 0;
            Entity e = byName[key];
            if (e.Seeker.Path == null || e.Seeker.Path.Count == 0)
                return e.Seeker.Path.Count;
            int nxt = e.Seeker.Path.Pop();
            Move(key, (nxt / e.Seeker.Width) - e.Pos.Y, (nxt % e.Seeker.Width) - e.Pos.X);
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
                value.Pos.Y = y;
                value.Pos.X = x;
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
    }

}
