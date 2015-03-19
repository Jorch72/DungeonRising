using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C5;

namespace DungeonRising
{
    [Serializable]
    
    public class State
    {
        public Dungeon DungeonStart { get; set; }
        public EntityDictionary Entities { get; set; }
        public string CurrentActor { get; set; }
        public int StepsLeft { get; set; }
        public int StepsTaken { get; set; }
        public Position Cursor { get; set; }
        public Position Camera { get; set; }
        public WaitReason CurrentReason { get; set; }
        public Schedule Initiative { get; set; }
        public long TurnsLeft { get; set; }
        public ulong[] XSSRState;
        public State()
        {

        }
        public State Fix()
        {
            XSSR.SetState(XSSRState);
            Dijkstra.Rand = new XSRandom();
            Dijkstra.Height = DungeonStart.Height;
            Dijkstra.Width = DungeonStart.Width;
            Dijkstra.PhysicalMap = DungeonStart.LogicWorld;

            foreach (Entity e in Entities)
            {
                Dijkstra dummy = e.Seeker;
                e.Seeker.CombinedMap = Dijkstra.PhysicalMap.Replicate();
                e.Seeker.SetGoal(e.Pos.Y, e.Pos.X);
                e.Seeker.Scan();
            }
            return this;
        }
        public State Clone()
        {
            State s = new State();
            s.DungeonStart = DungeonStart.Replicate();
            s.Entities = Entities.Replicate();
            s.CurrentActor = CurrentActor;
            s.StepsLeft = StepsLeft;
            s.StepsTaken = StepsTaken;
            s.Cursor = Cursor;
            s.Camera = Camera;
            s.CurrentReason = CurrentReason;
            s.Initiative = new Schedule(Initiative.scheduled);
            s.TurnsLeft = TurnsLeft;
            s.XSSRState = new ulong[] { XSSRState[0], XSSRState[1] };
            return s;
        }
    }

    public class Chariot
    {
        public static State S = null, Prior = null;
        public static int DiffIndex = 0;
        public static void ResetInitiative()
        {
            foreach (Entity e in S.Entities)
            {

                S.Initiative.AddTurn(e.Name, e.Delay);
                S.TurnsLeft += e.Stats.ActSpeed;
            }
        }
        public static void Remember()
        {
            if (S == null) return;
            Prior = S.Clone();
        }
        public static void Backward()
        {
            if (Prior == null) return;
            S = Prior.Clone();
            XSSR.SetState(S.XSSRState);
        }
    }
}
