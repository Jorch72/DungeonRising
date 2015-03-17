using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonRising
{
    [Serializable]
    public class State
    {
        public Dungeon DungeonStart;
        public EntityDictionary Entities;
        public string CurrentActor;
        public int StepsLeft = 0, StepsTaken = 0;
        public Position Cursor, Camera;
        public WaitReason CurrentReason;
        public Schedule Initiative;
        public long TurnsLeft;
        public ulong[] XSSRState;
        public State()
        {

        }
    }

    [Serializable]
    public class Diff
    {
        // public Dungeon DungeonStart;
        public EntityDictionary Entities;
        public string CurrentActor;
        public int? StepsLeft = 0, StepsTaken = 0;
        public Position? Cursor, Camera;
        public WaitReason? CurrentReason;
        public Schedule Initiative;
        public long? TurnsLeft;
        public ulong[] XSSRState;
        public Diff()
        {

        }
        public Diff(State state)
        {
            // DungeonStart = state.DungeonStart;
            Entities = state.Entities;
            CurrentActor = state.CurrentActor;
            StepsLeft = state.StepsLeft;
            StepsTaken = state.StepsTaken;
            Cursor = state.Cursor;
            Camera = state.Camera;
            CurrentReason = state.CurrentReason;
            Initiative = state.Initiative;
            TurnsLeft = state.TurnsLeft;
            XSSRState = state.XSSRState;
        }
    }
}
