using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using C5;

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
        public Position Cursor;
        public Position Camera;
        public WaitReason CurrentReason { get; set; }
        public Schedule Initiative { get; set; }
        public long TurnsLeft { get; set; }
        public ulong[] XSSRState;
        public State()
        {
            
        }
        public State(ImmutableState imm)
        {
            DungeonStart = imm.DungeonStart.Replicate();
            Entities = new EntityDictionary(imm.Entities);
            CurrentActor = imm.CurrentActor;
            StepsLeft = imm.StepsLeft;
            StepsTaken = imm.StepsTaken;
            Cursor = imm.Cursor;
            Camera = imm.Camera;
            CurrentReason = imm.CurrentReason;
            Initiative = new Schedule(imm.Initiative);
            TurnsLeft = imm.TurnsLeft;
            XSSRState = new ulong[] { imm.XSSRState[0], imm.XSSRState[1]};
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
                e.Seeker.CombinedMap = Dijkstra.PhysicalMap.Replicate();
//                e.Seeker.SetGoal(e.Pos.Y, e.Pos.X);
//                e.Seeker.Scan();
            }
            return this;
        }
        /*
         
        public State Clone()
        {
            State s = new State();
            s.DungeonStart = DungeonStart.Replicate();
            s.Entities = Entities;
            s.CurrentActor = CurrentActor;
            s.StepsLeft = StepsLeft;
            s.StepsTaken = StepsTaken;
            s.Cursor = Cursor;
            s.Camera = Camera;
            s.CurrentReason = CurrentReason;
            s.Initiative = new Schedule(Initiative.Scheduled);
            s.TurnsLeft = TurnsLeft;
            s.XSSRState = new ulong[] { XSSRState[0], XSSRState[1] };
            return s;
        }
         */
    }
    [Serializable]
    public class ImmutableState
    {
        public Dungeon DungeonStart { get; private set; }
        public ImmutableDictionary<string, Entity> Entities { get; private set; }
        public string CurrentActor { get; private set; }
        public int StepsLeft { get; private set; }
        public int StepsTaken { get; private set; }
        public Position Cursor { get; private set; }
        public Position Camera { get; private set; }
        public WaitReason CurrentReason { get; private set; }
        public ImmutableSortedDictionary<double, string> Initiative { get; private set; }
        public long TurnsLeft { get; private set; }
        public ulong[] XSSRState { get; private set; }

        public ImmutableState(Dungeon dungeon, EntityDictionary entities, string currentActor, int stepsLeft,
            int stepsTaken,
            Position cursor, Position camera, WaitReason currentReason, Schedule initiative, long turnsLeft,
            ulong[] xssrState)
        {
            DungeonStart = dungeon;
            Entities = entities.ToImmutable();
            CurrentActor = currentActor;
            StepsLeft = stepsLeft;
            StepsTaken = stepsTaken;
            Cursor = cursor;
            Camera = camera;
            CurrentReason = currentReason;
            Initiative = initiative.Scheduled.ToImmutableSortedDictionary();
            TurnsLeft = turnsLeft;
            XSSRState = xssrState;
        }

        public ImmutableState(State s)
        {
            DungeonStart = s.DungeonStart;
            Entities = s.Entities.ToImmutable();
            CurrentActor = s.CurrentActor;
            StepsLeft = s.StepsLeft;
            StepsTaken = s.StepsTaken;
            Cursor = s.Cursor;
            Camera = s.Camera;
            CurrentReason = s.CurrentReason;
            Initiative = s.Initiative.Scheduled.ToImmutableSortedDictionary();
            TurnsLeft = s.TurnsLeft;
            XSSRState = s.XSSRState;
        }
    }
    /// <summary>
    /// History class.
    /// </summary>
    public class H
    {
        public static State S = null;
        public  static ImmutableState Prior = null;
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
            S.XSSRState = XSSR.GetState();
            Prior = new ImmutableState(S);
        }
        public static void Backward()
        {
            if (Prior == null) return;
            S = new State(Prior);
            XSSR.SetState(S.XSSRState);
        }
    }
}
