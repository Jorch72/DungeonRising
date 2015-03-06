using System;
using C5;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonRising
{

    public class Entity
    {
        public string Name;
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public char Left, Right;
        public Dijkstra Seeker;
        public Entity(string Representation, int Y, int X)
        {
            this.Name = "";
            this.Left = Representation[0];
            this.Right = Representation[1];
            this.X = X;
            this.Y = Y;
        }
        public Entity(string Name, string Representation, int Y, int X)
        {
            this.Name = Name;
            this.Left = Representation[0];
            this.Right = Representation[1];
            this.X = X;
            this.Y = Y;
        }
        public Entity()
        {
            this.Name = "";
            this.Left = '.';
            this.Right = ' ';
            this.X = 0;
            this.Y = 0;
        }
        public void Move(int yMove, int xMove)
        {
            Y += yMove;
            X += xMove;
            Seeker.Reset();
            Seeker.SetGoal(Y, X);
            Seeker.Scan();
            //Seeker.GetPath(Y, X);
        }
        public int Step()
        {
            if (Seeker.Path == null || Seeker.Path.Count == 0)
                return Seeker.Path.Count;
            int nxt = Seeker.Path.Pop();
            this.Y = nxt / Seeker.Width;
            this.X = nxt % Seeker.Width;
            return Seeker.Path.Count;
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
