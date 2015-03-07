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
            return (this.Delay - other.Delay < 0.0) ? -1 : (this.Delay - other.Delay > 0.0) ? 1 : 0;
        }
    }
    public class Schedule
    {
        protected IntervalHeap<Turn> scheduled;
        protected HashDictionary<string, ArrayList<IPriorityQueueHandle<Turn>>> lookups;
        public Schedule()
        {
            scheduled = new IntervalHeap<Turn>(512);
            lookups = new HashDictionary<string, ArrayList<IPriorityQueueHandle<Turn>>>();
        }
        public void AddTurn(string actor, double delay)
        {
            Turn t = new Turn(actor, delay);
            IPriorityQueueHandle<Turn> h = null;
            scheduled.Add(ref h, t);
            if(lookups.Contains(actor))
            {
                lookups[actor].Add(h);
            }
            else
            {
                lookups[actor] = new ArrayList<IPriorityQueueHandle<Turn>> { h };
            }
        }
        public Turn NextTurn()
        {
            IPriorityQueueHandle<Turn> h = null;
            Turn nxt = scheduled.DeleteMin(out h);
            if(lookups.Contains(nxt.Actor))
            {
                lookups[nxt.Actor].Remove(h);
                if(lookups[nxt.Actor].Count == 0)
                {
                    lookups.Remove(nxt.Actor);
                }
            }
            foreach (KeyValuePair<string, ArrayList<IPriorityQueueHandle<Turn>>> kv in lookups)
            {
                foreach(IPriorityQueueHandle<Turn> hand in kv.Value)
                {
                    scheduled[hand].Delay -= nxt.Delay;
                }
            }
            //scheduled.UpdateAll(-(nxt.ActSpeed));
            return nxt;
        }
        public Turn PeekTurn()
        {
            return scheduled.FindMin();
        }
        public void CancelTurn(string cancelled)
        {
            if(lookups.Contains(cancelled))
            {
                foreach(IPriorityQueueHandle<Turn> h in lookups[cancelled])
                {
                    scheduled.Delete(h);
                }
                lookups.Remove(cancelled);
            }
        }
    }
}
