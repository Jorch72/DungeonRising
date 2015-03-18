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
        public Dungeon DungeonStart;
        public EntityDictionary Entities;
        public string CurrentActor;
        public int StepsLeft = 0;
        public int StepsTaken = 0;
        public Position Cursor;
        public Position Camera;
        public WaitReason CurrentReason;
        public Schedule Initiative;
        public long TurnsLeft;
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

    public interface IUpdate<T>
    {
        T Backward(T original);
        T Forward(T original);
    }
    [Serializable]
    public class RefUpdate<T> : IUpdate<T>
        where T : class
    {
        private T[] C = null, F = null, D = null, RC = null, B = null, RD = null;
        public RefUpdate()
        {

        }
        public RefUpdate(T create)
        {
            if (create != null)
            {
                C = new T[] { create };
//                RD = new T[] { create };
            }
        }
        public RefUpdate(T create, T forward, T delete, T backward)
        {
            if (create != null)
            {
                C = new T[] { create };
//                RD = new T[] { create };
            }
            if (forward != null)
            {
                F = new T[] { forward };
            }
            if (backward != null)
            {
                B = new T[] { backward };
            }
            if (delete != null)
            {
                D = new T[] { delete };
                RC = new T[] { delete };
            }
        }
        public T Forward(T original)
        {
            if (D != null) return default(T);
            if (C != null) return C[0];
            if (F != null) return F[0];

            return original;
        }
        public T Backward(T original)
        {
            if (RD != null) return default(T);
            if (RC != null) return RC[0];
            if (B != null) return B[0];

            return original;
        }
    }
    [Serializable]
    public class ValUpdate<T> : IUpdate<T>
        where T : struct
    {
        private T[] C = null, F = null, D = null, RC = null, B = null, RD = null;
        public ValUpdate()
        {

        }
        public ValUpdate(T create)
        {
            C = new T[] { create };
//            RD = new T[] { create };
        }
        public ValUpdate(T? create, T? forward, T? delete, T? backward)
        {
            if (create != null)
            {
                C = new T[] { create.Value };
//                RD = new T[] { create.Value };
            }
            if (forward != null)
            {
                F = new T[] { forward.Value };
            }
            if (backward != null)
            {
                B = new T[] { backward.Value };
            }
            if (delete != null)
            {
                D = new T[] { delete.Value };
                RC = new T[] { delete.Value };
            }
        }
        public T Forward(T original)
        {
            if (D != null) return default(T);
            if (C != null) return C[0];
            if (F != null) return F[0];

            return original;
        }
        public T Backward(T original)
        {
            if (RD != null) return default(T);
            if (RC != null) return RC[0];
            if (B != null) return B[0];

            return original;
        }
    }
    [Serializable]
    public class EntitiesUpdate : IUpdate<EntityDictionary>
    {
        private Entity[] C = null, F = null, D = null, RC = null, B = null, RD = null;
        public EntitiesUpdate()
        {

        }
        public EntitiesUpdate(Entity create, Entity forward, Entity delete, Entity backward)
        {
            if (create != null)
            {
                C = new Entity[] { create };
                RD = new Entity[] { create };
            }
            if (forward != null)
            {
                F = new Entity[] { forward };
            }
            if (backward != null)
            {
                B = new Entity[] { backward };
            }
            if (delete != null)
            {
                D = new Entity[] { delete };
                RC = new Entity[] { delete };
            }
        }
        public EntitiesUpdate(IEnumerable<Entity> create, IEnumerable<Entity> forward, IEnumerable<Entity> delete, IEnumerable<Entity> backward)
        {
            if (create != null)
            {
                C = create.ToArray();
                RD = create.ToArray();
            }
            if (forward != null)
            {
                F = forward.ToArray();
            }
            if (backward != null)
            {
                B = backward.ToArray();
            }
            if (delete != null)
            {
                D = delete.ToArray();
                RC = delete.ToArray();
            }
        }
        public EntitiesUpdate(EntityDictionary val)
        {
            if (val != null)
            {
                C = val.ToArray();
                RD = val.ToArray();
            }
        }
        public EntityDictionary Forward(EntityDictionary original)
        {
            if (D != null)
            {
                if (original == null) return null;
                foreach (Entity e in D)
                {
                    original.Remove(e.Name);
                }
            }
            if (C != null)
            {
                if (original == null) original = new EntityDictionary();
                original.AddAll(C);
            }
            if (F != null)
            {
                if (original == null) original = new EntityDictionary();
                original.UpdateAll(F);
            }
            return original;
        }
        public EntityDictionary Backward(EntityDictionary original)
        {
            if (RD != null)
            {
                if (original == null) return null;
                foreach (Entity e in RD)
                {
                    original.Remove(e.Name);
                }
            }
            if (RC != null)
            {
                if (original == null) original = new EntityDictionary();
                original.AddAll(RC);
            }
            if (B != null)
            {
                if (original == null) original = new EntityDictionary();
                original.UpdateAll(B);
            }
            return original;
        }
    }
    [Serializable]
    public class ScheduleUpdate : IUpdate<Schedule>
    {
        private SortedArray<Turn> C = null, F = null, D = null, RC = null, B = null, RD = null;

        public ScheduleUpdate()
        {

        }
        public ScheduleUpdate(Schedule val)
        {
            if (val != null)
            {
                C = val.scheduled;
                RD = val.scheduled;
            }
        }
        public ScheduleUpdate(IEnumerable<Turn> create, IEnumerable<Turn> forward, IEnumerable<Turn> delete, IEnumerable<Turn> backward)
        {
            if (create != null)
            {
                C = new SortedArray<Turn>();
                C.AddAll(create);
                RD = new SortedArray<Turn>();
                RD.AddAll(create);
            }
            if (forward != null)
            {
                F = new SortedArray<Turn>();
                F.AddAll(forward);
            }
            if (backward != null)
            {
                B = new SortedArray<Turn>();
                B.AddAll(backward);
            }
            if (delete != null)
            {
                D = new SortedArray<Turn>();
                D.AddAll(delete);
                RC = new SortedArray<Turn>();
                RC.AddAll(delete);
            }
        }
        public Schedule Forward(Schedule original)
        {

            if (D != null)
            {
                if (original == null) return null;
                foreach (Turn t in D)
                {
                    original.RemoveTurn(t);
                }
            }
            if (C != null)
            {
                original = new Schedule(C);
            }
            if (F != null)
            {
                original = new Schedule(F);
            }
            return original;
        }
        public Schedule Backward(Schedule original)
        {

            if (RD != null)
            {
                if (original == null) return null;
                foreach (Turn t in RD)
                {
                    original.RemoveTurn(t);
                }
            }
            if (RC != null)
            {
                original = new Schedule(RC);
            }
            if (B != null)
            {
                original = new Schedule(B);
            }
            return original;
        }
    }
    [Serializable]
    public class Diff
    {
        // public Dungeon DungeonStart;
        public IUpdate<EntityDictionary> Entities = null;
        public IUpdate<string> CurrentActor = null;
        public IUpdate<int> StepsLeft = null, StepsTaken = null;
        public IUpdate<Position> Cursor = null, Camera = null;
        public IUpdate<WaitReason> CurrentReason = null;
        public IUpdate<Schedule> Initiative = null;
        public IUpdate<long> TurnsLeft = null;
        public IUpdate<ulong[]> XSSRState = null;
        public Diff()
        {

        }
        public Diff(State state)
        {
            // DungeonStart = state.DungeonStart;
            Entities = new EntitiesUpdate(state.Entities);
            CurrentActor = new RefUpdate<string>(state.CurrentActor);
            StepsLeft = new ValUpdate<int>(state.StepsLeft);
            StepsTaken = new ValUpdate<int>(state.StepsTaken);
            Cursor = new ValUpdate<Position>(state.Cursor);
            Camera = new ValUpdate<Position>(state.Camera);
            CurrentReason = new ValUpdate<WaitReason>(state.CurrentReason);
            Initiative = new ScheduleUpdate(state.Initiative);
            TurnsLeft = new ValUpdate<long>(state.TurnsLeft);
            XSSRState = new RefUpdate<ulong[]>(state.XSSRState);
        }
        public Diff(State before, State after)
        {
            // DungeonStart = state.DungeonStart;
            if (before.Entities.SequenceEqual(after.Entities))
            {
                Entities = null;
            }
            else
            {
                List<Entity> c = new List<Entity>(after.Entities.Count), f = new List<Entity>(after.Entities.Count), d = new List<Entity>(after.Entities.Count), b = new List<Entity>(after.Entities.Count);
                foreach (Entity afterE in after.Entities)
                {
                    if (before.Entities.ContainsExactly(afterE))
                        continue;
                    if (before.Entities.Contains(afterE))
                    {
                        f.Add(afterE);
                        b.Add(before.Entities[afterE.Name]);
                    }
                    else
                    {
                        c.Add(afterE);
                    }
                } foreach (Entity beforeE in before.Entities)
                {
                    if (after.Entities.Contains(beforeE))
                        continue;
                    d.Add(beforeE);
                }
                Entities = new EntitiesUpdate(c, f, d, b);
            }
            if (before.CurrentActor == after.CurrentActor)
            {
                CurrentActor = null;
            }
            else
            {
                CurrentActor = new RefUpdate<string>(null, after.CurrentActor, null, before.CurrentActor);
            }

            if (before.StepsLeft == after.StepsLeft)
            {
                StepsLeft = null;
            }
            else
            {
                StepsLeft = new ValUpdate<int>(null, after.StepsLeft, null, before.StepsLeft);
            }

            if (before.StepsTaken == after.StepsTaken)
            {
                StepsTaken = null;
            }
            else
            {
                StepsTaken = new ValUpdate<int>(null, after.StepsTaken, null, before.StepsTaken);
            }

            if (before.Cursor == after.Cursor)
            {
                Cursor = null;
            }
            else
            {
                Cursor = new ValUpdate<Position>(null, after.Cursor, null, before.Cursor);
            }

            if (before.Camera == after.Camera)
            {
                Camera = null;
            }
            else
            {
                Camera = new ValUpdate<Position>(null, after.Camera, null, before.Camera);
            }

            if (before.CurrentReason == after.CurrentReason)
            {
                CurrentReason = null;
            }
            else
            {
                CurrentReason = new ValUpdate<WaitReason>(null, after.CurrentReason, null, before.CurrentReason);
            }


            if (before.Initiative.scheduled.SequenceEqual(after.Initiative.scheduled))
            {
                Initiative = null;
            }
            else
            {
                Initiative = new ScheduleUpdate(null, after.Initiative.scheduled, null, before.Initiative.scheduled);
            }


            if (before.TurnsLeft == after.TurnsLeft)
            {
                TurnsLeft = null;
            }
            else
            {
                TurnsLeft = new ValUpdate<long>(null, after.TurnsLeft, null, before.TurnsLeft);
            }


            if (before.XSSRState == after.XSSRState)
            {
                XSSRState = null;
            }
            else
            {
                XSSRState = new RefUpdate<ulong[]>(null, after.XSSRState, null, before.XSSRState);
            }

        }

        public State Forward(State s)
        {
            State ret = new State();
            if (XSSRState != null)
                ret.XSSRState = XSSRState.Forward(s.XSSRState);
            ret.DungeonStart = s.DungeonStart.Replicate();
            if (Entities != null)
                ret.Entities = Entities.Forward(s.Entities);
            if (Camera != null)
                ret.Camera = Camera.Forward(s.Camera);
            if (CurrentActor != null)
                ret.CurrentActor = CurrentActor.Forward(s.CurrentActor);
            if (CurrentReason != null)
                ret.CurrentReason = CurrentReason.Forward(s.CurrentReason);
            if (Cursor != null)
                ret.Cursor = Cursor.Forward(s.Cursor);
            if (Initiative != null)
                ret.Initiative = Initiative.Forward(s.Initiative);
            if (StepsLeft != null)
                ret.StepsLeft = StepsLeft.Forward(s.StepsLeft);
            if (StepsTaken != null)
                ret.StepsTaken = StepsTaken.Forward(s.StepsTaken);
            ret.Fix();
            return ret;
        }

        public State Backward(State s)
        {
            State ret = new State();
            if (XSSRState != null)
                ret.XSSRState = XSSRState.Backward(s.XSSRState);
            ret.DungeonStart = s.DungeonStart.Replicate();
            if (Entities != null)
                ret.Entities = Entities.Backward(s.Entities);
            if (Camera != null)
                ret.Camera = Camera.Backward(s.Camera);
            if (CurrentActor != null)
                ret.CurrentActor = CurrentActor.Backward(s.CurrentActor);
            if (CurrentReason != null)
                ret.CurrentReason = CurrentReason.Backward(s.CurrentReason);
            if (Cursor != null)
                ret.Cursor = Cursor.Backward(s.Cursor);
            if (Initiative != null)
                ret.Initiative = Initiative.Backward(s.Initiative);
            if (StepsLeft != null)
                ret.StepsLeft = StepsLeft.Backward(s.StepsLeft);
            if (StepsTaken != null)
                ret.StepsTaken = StepsTaken.Backward(s.StepsTaken);
            ret.Fix();
            return ret;
        }

    }
    public class Chariot
    {
        public static State S = null, Prior = null;
        public static ArrayList<Diff> Diffs = new ArrayList<Diff>(512);
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

            if (Prior == null)
            {
                Diffs.Add(new Diff(S));
            }
            else
            {
                Diffs.Add(new Diff(Prior, S));
                DiffIndex++;
            }

            Prior = S.Clone();
        }
        public static void Backward()
        {
            if (DiffIndex <= 1)
                return;
            Diff df = Diffs[DiffIndex];
            S = df.Backward(S);

            if (DiffIndex >= 2)
            {
                Diff df2 = Diffs[DiffIndex - 1];
                Prior = df2.Backward(Prior);
            }
            else
            {
                Prior = null;
            }
            DiffIndex--;
        }
        public static void Forward()
        {
            if (DiffIndex >= Diffs.Count - 1)
                return;
            Diff df = Diffs[DiffIndex];
            S = df.Forward(S);

            if (DiffIndex >= 2)
            {
                Diff df2 = Diffs[DiffIndex - 1];
                Prior = df2.Forward(Prior);
            }
            else
            {
                Prior = null;
            }
            DiffIndex++;
        }
    }
}
