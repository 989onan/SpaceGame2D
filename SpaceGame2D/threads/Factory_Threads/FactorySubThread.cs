using OpenTK.Graphics.ES11;
using SpaceGame2D.enviroment.blocks.machines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.threads.Factory_Threads
{
    public class FactorySubThread
    {

        public LinkedList<IMachine> machines = new LinkedList<IMachine>();

        public int seconds_recognized = 0;

        public int seconds_not_processed => ((int)(DateTime.Now-MainThread.Instance.gamestart).TotalSeconds) - seconds_recognized;

        //public IMachine startscan { get; set; } 
        
        
        public Thread Thread { get; private set; }

        public bool is_running = false;

        public FactorySubThread(IMachine start_scan) {
            //this.startscan = start_scan;
            this.Thread = new Thread(() =>
            {
                while (is_running) //this may throw an error.
                {
                    if (seconds_not_processed > seconds_recognized)
                    {
                        iterate();
                        seconds_recognized++;
                    }
                    if(seconds_not_processed - seconds_recognized > 10)
                    {
                        Console.WriteLine("Machine group of size \"" + machines.Count().ToString() + "\" is causing CPU lag out the wazoo!! It's thread is running \""+(((int)seconds_not_processed) - seconds_recognized).ToString()+" seconds behind! ");
                    }
                    if (this == null) //I have no clue if this will be an issue.
                    {
                        break;
                    }
                }
            });
            machines.AddFirst(start_scan);

            recursive_find_machines(machines);

            is_running = true;
            this.Thread.Start();
            MainThread.factories.Add(this);

        }

        ~FactorySubThread()
        {
            Console.WriteLine("Disposing of factory!");
            this.is_running = false;
        }

        //TODO: How should we determine how to start the position of a machine group scan, and how do we split machines
        //if the group splits in half?
        //like, if a machine line gets split in half, how do we create new threads, and how do we implement scanning better?
        //look this up when I get internet, for now, splitting groups causes issues. - @989onan
        public void recursive_find_machines(LinkedList<IMachine> machines_changed)
        {
            bool flag = false; //did we find a new machine?
            foreach (IMachine machine in machines_changed) {
                List<IMachine> nextMachines = machine.Links();

                if (!machines_changed.Contains(machine))
                {
                    machines_changed.AddLast(machine);
                    flag = true;
                }

                foreach (IMachine linkedMachine in nextMachines)
                {
                    if (!machines_changed.Contains(linkedMachine))
                    {
                        machines_changed.AddLast(linkedMachine);
                        flag = true;
                    }
                }
                machine.links_changed = false;
                
            }
            if (!flag)
            {
                return; //return if we didn't add any new machines, meaning we found the end of our massive contraption.
            }
            recursive_find_machines(machines_changed);
        }

        public void iterate()
        {


            //first, recalculate our grid if a machine has reported that we have to recalculate our machine grid, or one of our machines disappeared from the grid.
            List<IMachine> machines_old = machines.Where(o => (o != null)).Where(o => o.grid != null).ToList();
            if (machines_old.Where((o) => o.links_changed).Count() > 0 || machines_old.Count() != machines.Count())
            {
                Console.WriteLine("Machine thread group has found a moved or changed machine, recalculating machine group!");
                LinkedList<IMachine> new_machines = new LinkedList<IMachine>(machines_old);
                LinkedList<IMachine> new_machines_2 = new LinkedList<IMachine>();
                foreach (IMachine machine in new_machines.Where(o => !o.links_changed))
                {
                    new_machines_2.AddLast(machine);


                }
                foreach (IMachine machine in new_machines.Where(o => o.links_changed))
                {
                    //if we iterate through the new machines, and their links are updated through the recursion, they will be unchanged again.
                    //this is why this if statement is here, basically redundancy.
                    if (machine.links_changed)
                    {
                        
                        recursive_find_machines(new_machines_2);
                        break;
                    }
                }
                machines = new_machines_2;
            }

            
            if(!(machines.Count() > 0))
            {
                Console.WriteLine("Ticking machine thread has no more machines! destroying ourselves!");
                this.is_running = false;
                return;
            }
            Console.WriteLine("Ticking machine thread group of size: " + machines.Count().ToString());
            //iterate all machines in no particular order.
            foreach (IMachine machine in machines)
            {
                machine.Iterate();
            }

            
        }
    }
}
