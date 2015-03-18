using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DungeonRising
{
    [ImplementPropertyChanged]
    /// <summary>
    /// Character Sheet of statistics and stuff.
    /// </summary>
    public class Sheet
    {
        public Gauge Health { get; set; }
        public int Damage { get; set; }
        public int MoveSpeed { get; set; }
        public int ActSpeed { get; set; }
        public event EventHandler Killed;
        public Sheet()
        {

        }
        public Sheet(int maxHealth, int damage, int moveSpeed, int actSpeed)
        {
            Killed = null;
            MoveSpeed = moveSpeed;
            ActSpeed = actSpeed;
            Health = new Gauge(maxHealth);
            Damage = damage;
            Health.ReachedZero += OnZeroHealth;
        }

        void OnZeroHealth(object sender, EventArgs e)
        {
            EventHandler handler = Killed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
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
            ret.Killed = Killed;
            ret.ActSpeed = ActSpeed;
            ret.MoveSpeed = MoveSpeed;
            return ret;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

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
        public override int GetHashCode()
        {
            return (privMax << 16) | privCurrent;
        }
    }
}
