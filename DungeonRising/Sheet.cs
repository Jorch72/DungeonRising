using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DungeonRising
{
    
    /// <summary>
    /// Character Sheet of statistics and stuff.
    /// </summary>
    [Serializable]
    public class Sheet
    {
        public Gauge Health;
        public int Damage;
        public int MoveSpeed;
        public int ActSpeed;
        private event EventHandler WhenKilled;
        
        public Sheet()
        {

        }
        public Sheet(int maxHealth, int damage, int moveSpeed, int actSpeed)
        {
            WhenKilled = null;
            MoveSpeed = moveSpeed;
            ActSpeed = actSpeed;
            Health = new Gauge(maxHealth);
            Damage = damage;
            Health.ReachedZero += OnZeroHealth;
        }

        void OnZeroHealth(object sender, EventArgs e)
        {
            EventHandler handler = WhenKilled;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public void SetKilledHandler(EventHandler handler)
        {
            WhenKilled = null;
            WhenKilled += handler;
        }

        public bool Equals(Sheet other)
        {
            return Health.Current == other.Health.Current && Damage == other.Damage && Health.Max == other.Health.Max && MoveSpeed == other.MoveSpeed && ActSpeed == other.ActSpeed;
        }

        public Sheet Replicate()
        {
            Sheet ret = new Sheet();
            ret.Health = Health;
            ret.Damage = Damage;
            ret.WhenKilled = WhenKilled;
            ret.ActSpeed = ActSpeed;
            ret.MoveSpeed = MoveSpeed;
            return ret;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    [Serializable]
    public struct Gauge
    {
        public event EventHandler ReachedZero;
        private int privMax;
        public int Max
        {
            get { return privMax; }
            set
            {
                privMax = value;
                if (privMax > privCurrent)
                    privCurrent = privMax;
            }
        }
        private int privCurrent;
        public int Current
        {
            get
            {
                return privCurrent;
            }
            set
            {
                if (value >= privMax)
                {
                    privCurrent = privMax;
                }
                else if (value <= 0)
                {
                    privCurrent = 0;
                    EventHandler handler = ReachedZero;
                    if(handler != null)
                    {
                        handler(this, EventArgs.Empty);
                    }
                }
                else
                {
                    privCurrent = value;
                }
            }
        }
        public Gauge(int max, int current)
        {
            privMax = max;
            privCurrent = (current >= privMax) ? privMax : current;
            ReachedZero = null;
        }
        public Gauge(int max)
        {
            privMax = max;
            privCurrent = max;
            ReachedZero = null;
        }
        public static Gauge operator -(Gauge start, int amount)
        {
            Gauge g = new Gauge(start.Max, start.Current - amount);
            g.ReachedZero = start.ReachedZero;
            return g;
        }
        public static Gauge operator +(Gauge start, int amount)
        {
            Gauge g = new Gauge(start.Max, start.Current + amount);
            g.ReachedZero = start.ReachedZero;
            return g;
        }
        public override int GetHashCode()
        {
            return (privMax << 16) | privCurrent;
        }
    }
}
