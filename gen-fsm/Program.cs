using System;
using System.Collections.Generic;

namespace gen_fsm
{

    public enum ProcessState
    {
        Inactive,
        Active,
        Exited,
        Paused
    }

    public enum ProcessTransitions
    {
        Begin,
        End,
        Pause,
        Resume,
        Exit
    }

    public class Machine
    {
        public class MachineTransition{
            readonly ProcessState CurrentState;
            readonly ProcessTransitions Transitions;

            public MachineTransition(ProcessState currentState, ProcessTransitions transitions)
            {
                CurrentState = currentState;
                Transitions = transitions;
            }

            public override int GetHashCode()
            {
                return 19 + 32 * CurrentState.GetHashCode() + 32 * Transitions.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                MachineTransition mt = obj as MachineTransition;
                return mt != null && (this.CurrentState == mt.CurrentState) && (this.Transitions == mt.Transitions);
            }
        }
            Dictionary<MachineTransition, ProcessState> transitions;
            public ProcessState CurrentState { get; private set; }  
            
            public Machine()
            {
                CurrentState = ProcessState.Inactive;
                transitions = new Dictionary<MachineTransition, ProcessState>
                {
                    {new MachineTransition(ProcessState.Inactive, ProcessTransitions.Exit), ProcessState.Exited},
                    {new MachineTransition(ProcessState.Inactive, ProcessTransitions.Begin), ProcessState.Active},
                    {new MachineTransition(ProcessState.Active, ProcessTransitions.End), ProcessState.Inactive},
                    {new MachineTransition(ProcessState.Active, ProcessTransitions.Pause), ProcessState.Paused},
                    {new MachineTransition(ProcessState.Paused, ProcessTransitions.End), ProcessState.Inactive},
                    {new MachineTransition(ProcessState.Paused,ProcessTransitions.Resume), ProcessState.Active}

                };
            }

            // Deterministically Get Next State
            public ProcessState GetNext(ProcessTransitions procTransitions)
            {
                MachineTransition transition  = new MachineTransition(CurrentState, procTransitions);
                ProcessState nextProcState;
                if (!transitions.TryGetValue(transition, out nextProcState))
                {
                    throw new Exception("No known Transition: " + CurrentState + "->" + procTransitions);
                   
                }
                 return nextProcState;
            }

            // Mutate to the next Machine State

            public ProcessState MoveNext(ProcessTransitions procTransitions)
            {
                CurrentState = GetNext(procTransitions);
                return CurrentState;
            }

    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello FSM C# !");
             Machine machine  = new Machine();
             Console.WriteLine("Current State is : " + machine.CurrentState);
             Console.WriteLine("Received Command Begin: Current State is: " + machine.MoveNext(ProcessTransitions.Begin));
             Console.WriteLine("Current State is : " + machine.CurrentState);
             Console.WriteLine("Received Command Pause: Current State is: " + machine.MoveNext(ProcessTransitions.Pause));
             Console.WriteLine("Current State is : " + machine.CurrentState);
             Console.WriteLine("Received Command End: Current State is: " + machine.MoveNext(ProcessTransitions.End));
             Console.WriteLine("Current State is : " + machine.CurrentState);
             Console.WriteLine("Received Command Exit: Current State is: " + machine.MoveNext(ProcessTransitions.Exit));
             Console.WriteLine("Current State is : " + machine.CurrentState);
            Console.ReadLine();
        }
    }
}
