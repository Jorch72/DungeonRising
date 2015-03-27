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
        
        public Sheet()
        {

        }
        public Sheet(int maxHealth, int damage, int moveSpeed, int actSpeed)
        {
            MoveSpeed = moveSpeed;
            ActSpeed = actSpeed;
            Health = new Gauge(maxHealth, maxHealth);
            Damage = damage;
        }
        public Sheet(Gauge health, int damage, int moveSpeed, int actSpeed)
        {
            MoveSpeed = moveSpeed;
            ActSpeed = actSpeed;
            Health = health;
            Damage = damage;
        }


        public bool Equals(Sheet other)
        {
            return Health.Current == other.Health.Current && Damage == other.Damage && Health.Max == other.Health.Max && MoveSpeed == other.MoveSpeed && ActSpeed == other.ActSpeed;
        }

        public Sheet Replicate()
        {
            return new Sheet(Health, Damage, MoveSpeed, ActSpeed);
        }

        public Sheet UpdateWith(
Optional<Gauge> health = new Optional<Gauge>(),
Optional<int> damage = new Optional<int>(),
Optional<int> moveSpeed = new Optional<int>(),
Optional<int> actSpeed = new Optional<int>())
        {
            return new Sheet(health.GetValue(this.Health), damage.GetValue(this.Damage), moveSpeed.GetValue(this.MoveSpeed), actSpeed.GetValue(this.ActSpeed));
        }
    }
    [Serializable]
    public struct Gauge
    {
//        public event EventHandler ReachedZero;
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
            if (_current <= 0)
            {
                _current = 0;
                //if (ReachedZero != null)
                //{
                //    ReachedZero(this, EventArgs.Empty);
                //    ReachedZero = null;
                //}
            }
        }
        public Gauge(int max)
        {
            _max = max;
            _current = max;
        }
        public static Gauge operator -(Gauge start, int amount)
        {
            Gauge g = new Gauge(start.Max, start.Current - amount);
            return g;
        }
        public static Gauge operator +(Gauge start, int amount)
        {
            Gauge g = new Gauge(start.Max, start.Current + amount);
            return g;
        }

    }
}
