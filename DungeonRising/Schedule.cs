using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using C5;
using PropertyChanged;
namespace DungeonRising
{
    [Serializable]
    [ImplementPropertyChanged]
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
    [ImplementPropertyChanged]
    public class Schedule
    {
        public SortedArray<Turn> scheduled { get; set; }
        public Schedule()
        {
            scheduled = new SortedArray<Turn>(512);
        }
        public Schedule(SortedArray<Turn> existing)
        {
            scheduled = new SortedArray<Turn>(existing.Count);
            foreach(Turn t in existing)
            {
                scheduled.Add(t.Replicate());
            }
        }
        public void AddTurn(string actor, double delay)
        {
            scheduled.Add(new Turn(actor, delay));
        }
        public void AddTurn(Turn turn)
        {
            scheduled.Add(turn);
        }
        public void RemoveTurn(Turn turn)
        {
            if(scheduled.Contains(turn))
            {
                scheduled.Remove(turn);
            }
        }
        public Turn NextTurn()
        {
            Turn nxt = scheduled.DeleteMin();

            foreach (Turn t in scheduled)
            {
                t.Delay -= nxt.Delay;
            }
            return nxt;
        }
        public Turn PreviousTurn(Turn undo)
        {
            Turn prev = scheduled.FindMin();
            foreach (Turn t in scheduled)
            {
                t.Delay += undo.Delay;
            }
            scheduled.Add(undo);
            return prev;
        }
        public Turn PeekTurn()
        {
            return scheduled.FindMin();
        }
        public void CancelTurn(string cancelled)
        {
            foreach (Turn t in scheduled)
            {
                if (t.Actor == cancelled)
                {
                    scheduled.Remove(t);
                }
            }
        }
        public void ReverseCancelTurn(IEnumerable<Turn> turns)
        {
            scheduled.AddAll(turns);
        }
    }
}
