using System;
using System.Text;
using System.Threading.Tasks;
using C5;
namespace DungeonRising
{
    public class Turn : IComparable<Turn>
    {
        public string Actor;
        public double Delay;
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
        
    }


    public class Schedule
    {
        protected SortedArray<Turn> scheduled;
        public Schedule()
        {
            scheduled = new SortedArray<Turn>(512);
        }
        public void AddTurn(string actor, double delay)
        {
            Turn t = new Turn(actor, delay);
            IPriorityQueueHandle<Turn> h = null;
            scheduled.Add(t);
            
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
    }
}
