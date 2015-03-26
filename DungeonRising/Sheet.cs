using System;
using System.Collections.Immutable;
using ProdutiveRage.UpdateWith;

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
        public Gauge Health { get; private set; }
        public int Damage { get; private set; }
        public int MoveSpeed { get; private set; }
        public int ActSpeed { get; private set; }
        public Action WhenKilled { get; private set; }
        private EventHandler Ozh = null;
        public Sheet()
        {

        }
        public Sheet(int maxHealth, int damage, int moveSpeed, int actSpeed)
        {
            WhenKilled = null;
            MoveSpeed = moveSpeed;
            ActSpeed = actSpeed;
            Ozh = OnZeroHealth;
            Health = new Gauge(maxHealth, maxHealth, Ozh);
            Damage = damage;
        }
        public Sheet(Gauge health, int damage, int moveSpeed, int actSpeed, Action whenKilled)
        {
            WhenKilled = whenKilled;
            MoveSpeed = moveSpeed;
            ActSpeed = actSpeed;
            Ozh = OnZeroHealth;
            Health = health;
            Damage = damage;
        }

        void OnZeroHealth(object sender, EventArgs e)
        {
            Action handler = WhenKilled;
            if (handler != null)
            {
                handler();
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
            ret.WhenKilled = WhenKilled;
            ret.ActSpeed = ActSpeed;
            ret.MoveSpeed = MoveSpeed;
            return ret;
        }

        public Sheet UpdateWith(
Optional<Gauge> health = new Optional<Gauge>(),
Optional<int> damage = new Optional<int>(),
Optional<int> moveSpeed = new Optional<int>(),
Optional<int> actSpeed = new Optional<int>())
        {
            Gauge hp = health.GetValue(this.Health);
            
            return new Sheet(new Gauge(hp.Max, hp.Current, this.Ozh, true), damage.GetValue(this.Damage), moveSpeed.GetValue(this.MoveSpeed), actSpeed.GetValue(this.ActSpeed), this.WhenKilled);
        }
        public Sheet UpdateKilledAction(Action whenKilled)
        {
            return new Sheet(Health, Damage, MoveSpeed, ActSpeed, whenKilled);
        }
    }
    [Serializable]
    public struct Gauge
    {
        public event EventHandler ReachedZero;
        private int _max;
        public int Max
        {
            get { return _max; }
            set
            {
                _max = value;
                if (_max > _current)
                    _current = _max;
            }
        }
        private int _current;
        public int Current
        {
            get
            {
                return _current;
            }
            set
            {
                if (value >= _max)
                {
                    _current = _max;
                }
                else if (value <= 0)
                {
                    _current = 0;
                    
                    EventHandler handler = ReachedZero;
                    /*if(handler != null)
                    {
                        handler(this, EventArgs.Empty);
                    }*/
                }
                else
                {
                    _current = value;
                }
            }
        }
        public Gauge(int max, int current)
        {
            _max = max;
            _current = (current >= _max) ? _max : current;
            ReachedZero = null;
        }
        public Gauge(int max, int current, EventHandler reachedZero)
        {
            _max = max;
            _current = (current >= _max) ? _max : current;
            ReachedZero = reachedZero;
            if (_current <= 0)
            {
                _current = 0;
                if (ReachedZero != null)
                {
                    ReachedZero(this, EventArgs.Empty);
                    ReachedZero = null;
                }
            }
        }
        public Gauge(int max, int current, EventHandler reachedZero, bool dormant)
        {
            _max = max;
            _current = (current >= _max) ? _max : current;
            ReachedZero = reachedZero;
            if (_current <= 0)
            {
                _current = 0;
                if (ReachedZero != null && !dormant)
                {
                    ReachedZero(this, EventArgs.Empty);
                    ReachedZero = null;
                }
            }
        }
        public Gauge(int max)
        {
            _max = max;
            _current = max;
            ReachedZero = null;
        }
        public static Gauge operator -(Gauge start, int amount)
        {
            Gauge g = new Gauge(start.Max, start.Current - amount, start.ReachedZero);
            return g;
        }
        public static Gauge operator +(Gauge start, int amount)
        {
            Gauge g = new Gauge(start.Max, start.Current + amount, start.ReachedZero);
            return g;
        }
        public override int GetHashCode()
        {
            return (_max << 16) | _current;
        }
    }
}
