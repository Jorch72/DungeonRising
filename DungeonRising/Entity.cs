using System;
using C5;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DungeonRising
{

    public class Entity
    {
        public string Name;
        public int X;
        public int Y;
        public char Left, Right;
        public Color Coloring;
        public int MoveSpeed, ActSpeed, Faction;
        public double Delay { get { return 36.0 / ActSpeed; } }
        public Dijkstra Seeker;
        public bool Equivalent(Entity other)
        {
            return other != null && X == other.X && Y == other.Y && Left == other.Left && Right == other.Right;
        }
        public Entity(string representation, int y, int x)
        {
            Name = "";
            Left = representation[0];
            Right = representation[1];
            Coloring = Color.DimGray;
            X = x;
            Y = y;
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
            X = x;
            Y = y;
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
            X = x;
            Y = y;
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
            this.X = 0;
            this.Y = 0;
            this.MoveSpeed = 0;
            this.ActSpeed = 1;
            this.Faction = 0;
        }
    }

    public class EntityDictionary : IEnumerable<Entity>
    {
        private HashDictionary<string, Entity> byName;
        private HashDictionary<Position, Entity> byPosition;
        public int Count { get { return byName.Count; } }
        public EntityDictionary()
        {
            byName = new HashDictionary<string, Entity>();
            byPosition = new HashDictionary<Position, Entity>();

        }
        public bool Contains(string key)
        {
            return byName.Contains(key);
        }
        public bool Contains(int y, int x)
        {
            return byPosition.Contains(new Position(y, x));
        }
        public bool Contains(Position p)
        {
            return byPosition.Contains(p);
        }
        public void Add(Entity val)
        {
            byPosition.Add(new Position(val.Y, val.X), val);
            byName.Add(val.Name, val);
        }
        public void Add(string key, Entity val)
        {
            val.Name = key;
            byPosition.Add(new Position(val.Y, val.X), val);
            byName.Add(key, val);
        }
        public void Remove(string key)
        {
            Entity e = byName[key];
            byPosition.Remove(new Position(e.Y, e.X));
            byName.Remove(key);
        }

        public void Move(string key, int yMove, int xMove)
        {
            if (!byName.Contains(key) || byPosition.Contains(new Position(byName[key].Y + yMove, byName[key].X + xMove)))
                return;
            Entity e = byName[key];
            Position pos = new Position(e.Y, e.X);
            byPosition.Remove(pos);
            byName.Remove(key);
            e.Y += yMove;
            e.X += xMove;
            Add(key, e);
            /*foreach(var kv in byName)
            {
                kv.Value.Seeker.ResetCell(pos.Y, pos.X);
                //kv.Value.Seeker.SetOccupied(e.Y, e.X);
            }*/
            e.Seeker.SetGoal(e.Y, e.X);

            foreach (var kv in byName)
            {
                kv.Value.Seeker.Scan();
            }
            //Seeker.GetPath(Y, X);
        }
        public int Step(string key)
        {
            if (!byName.Contains(key))
                return 0;
            Entity e = byName[key];
            if (e.Seeker.Path == null || e.Seeker.Path.Count == 0)
                return e.Seeker.Path.Count;
            int nxt = e.Seeker.Path.Pop();
            Move(key, (nxt / e.Seeker.Width) - e.Y, (nxt % e.Seeker.Width) - e.X);
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
                byPosition[new Position(v.Y, v.X)] = v;
            }
        }
        public Entity this[int y, int x]
        {
            get
            {
                Position p = new Position(y, x);
                if(byPosition.Contains(p))
                    return byPosition[p];
                return null;
            }
            set
            {
                if (value.Name == "") throw new ArgumentException("Entity.Name must not be empty.");
                byPosition[new Position(value.Y, value.X)] = value;
                byName[value.Name] = value;
            }
        }
        public Entity this[Position p]
        {
            get
            {
                if (byPosition.Contains(p))
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
