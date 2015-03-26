using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
//using C5;
namespace DungeonRising
{
    [Serializable]
    
    public class Turn : IComparable<Turn>
    {
        public string Actor { get; set; }
        public double Delay { get; set; }
        public Turn()
        {
            Actor = "";
            Delay = 1.0;
        }
        public Turn(string actor, double delay)
        {
            Actor = actor;
            Delay = delay;
        }
        public int CompareTo(Turn other)
        {
            return (this.Delay - other.Delay < 0.0) ? -1 : (this.Delay - other.Delay > 0.0) ? 1 : (this.Actor.CompareTo(other.Actor));
        }
        public Turn Replicate()
        {
            return new Turn(Actor, Delay);
        }
    }

    [Serializable]
    
    public class Schedule
    {
        public SortedList<double, string> Scheduled { get; set; }
        public Schedule()
        {
            Scheduled = new SortedList<double, string>(512);
        }
        public Schedule(SortedList<double, string> existing)
        {
            Scheduled = new SortedList<double, string>(existing.Count);
            foreach(var t in existing)
            {
                AddTurn(t.Value, t.Key);
            }
        }

        public void AddTurn(string actor, double delay)
        {
            double altDelay = delay;
            do
            {
                if (Scheduled.ContainsKey(altDelay))
                    altDelay += 0.001;
                else
                {
                    Scheduled.Add(altDelay, actor);
                    break;
                }
            } while (true);
        }

        public void AddTurn(Turn turn)
        {
            AddTurn(turn.Actor, turn.Delay);
        }
        public void RemoveTurn(Turn turn)
        {
            if(Scheduled.ContainsKey(turn.Delay))
            {
                Scheduled.Remove(turn.Delay);
            }
        }
        public Turn NextTurn()
        {
            double nxt = Scheduled.Keys[0];
            Turn tr = new Turn(Scheduled[nxt], nxt);
            Scheduled.RemoveAt(0);
            SortedList<double, string> altScheduled = new SortedList<double, string>(Scheduled.Count);
            foreach (var t in Scheduled)
            {
                altScheduled.Add(t.Key - nxt, t.Value);
            }
            Scheduled = altScheduled;
            return tr;
        }
        public Turn PeekTurn()
        {
            double nxt = Scheduled.Keys[0];
            return new Turn(Scheduled[nxt], nxt);
        }
        public void CancelTurn(string cancelled)
        {
            SortedList<double, string> sched2 = new SortedList<double, string>(Scheduled.Count);
            foreach (var t in Scheduled)
            {
                if (t.Value != cancelled)
                {
                    sched2.Add(t.Key, t.Value);
                }
            }
            Scheduled = sched2;
        }
        public void ReverseCancelTurn(IEnumerable<Turn> turns)
        {
            foreach (var t in turns)
            {
                AddTurn(t);
            }
        }
    }
}
