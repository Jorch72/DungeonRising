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
        public char[,] World, PairedWorld;
        public int[,] LogicMap;
        public EntityDictionary Entities;
        public string CurrentActor;
        public int StepsLeft = 0, StepsTaken = 0;
        public Position Cursor, Camera;
        public WaitReason CurrentState;
        public Schedule Initiative;
        public long TurnsLeft;
        public ulong[] XSSRState;
        public State()
        {

        }
    }
}
