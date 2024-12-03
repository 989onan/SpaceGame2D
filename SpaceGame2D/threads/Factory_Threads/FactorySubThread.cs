using OpenTK.Graphics.ES11;
using SpaceGame2D.enviroment.blocks;
using SpaceGame2D.enviroment.blocks.machines;
using SpaceGame2D.enviroment.storage;
using SpaceGame2D.utilities.math;
using SpaceGame2D.utilities.threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace SpaceGame2D.threads.Factory_Threads
{
    public class FactorySubThread
    {

        public List<IMachine> machines = new List<IMachine>();

        public IStorageObject inputStorage = null;
        public IStorageObject outputStorage = null;

        public int seconds_not_processed => ((int)(DateTime.Now - MainThread.Instance.gamestart).TotalSeconds - seconds_recognized);
        public int seconds_recognized { get; private set;}

        //public IMachine startscan { get; set; } 

        private DateTime LastTime;
        private DateTime ThisTime;
        
        public Task IterationTask { get; private set; }

        public bool is_running = false;
        private IEnumerable<ModifiableTuple2Diff<ModifiableTuple2<Dictionary<string, int>>, int>> processors;

        public FactorySubThread(IMachine start_scan) {
            seconds_recognized = 0; //this is needed or setting it later with seconds_not_recognized throws a not initalized error!
            this.IterationTask = new Task(() =>
            {
                seconds_recognized = seconds_not_processed;
                while (is_running) //this may throw an error.
                {
                    ThisTime = DateTime.Now;
                    iterate();
                    LastTime = DateTime.Now;
                    

                    if ((TimeSpan.FromSeconds(1) - (LastTime - ThisTime)).TotalSeconds <= 0)
                    {
                        Console.WriteLine("Machine group of size \"" + machines.Count().ToString() + "\" is causing CPU lag out the wazoo!! It's thread is running \""+ seconds_not_processed.ToString() + " seconds behind! ");
                    }
                    else
                    {
                        Task.Delay((TimeSpan.FromSeconds(1) - (LastTime - ThisTime)));
                    }
                }
            });
            machines.Add(start_scan);
            Recalculate();
            

            is_running = true;
            RecalcRecipeSelectors();
            this.IterationTask.Start();
            MainThread.factories.Add(this);

        }

        public void RecalcRecipeSelectors() {
            ConcurrentBag<ModifiableTuple2<Dictionary<string, int>>> processors_bag = new ConcurrentBag<ModifiableTuple2<Dictionary<string, int>>>();
            machines.AsParallel().ForAll(o =>
            {
                processors_bag.Add(o.ProcessingTypes.ElementAt(o.selected_recipe));
            });

            List<ModifiableTuple2<Dictionary<string, int>>> processors_temp = processors_bag.ToList();

            processors = processors_temp.GroupDistincts();
        }

        public void Recalculate()
        {
            
            FactorySubThread.GetMachines(machines);
            if(machines.Select(o => o.source_factory).Distinct().Count() > 1)
            {
                machines.AsParallel().ForAll(o => {
                    o.source_factory.is_running = false;
                    o.source_factory = this;
                    });
            }
            
        }

        public static List<IMachine> GetMachines(List<IMachine> current_machines)
        {
            foreach (IMachine item in current_machines)
            {
                IEnumerable<IMachine> new_machines = item.grid.getAxies(item.block_position).OfType<IMachine>();

                foreach(IMachine machine in new_machines)
                {
                    if (!current_machines.Contains(machine))
                    {
                        current_machines.Add(machine);
                        GetMachines(current_machines);
                    }
                }
            }
            return current_machines;
        }

        ~FactorySubThread()
        {
            Console.WriteLine("Disposing of factory!");
            this.is_running = false;
        }

        public void iterate()
        {


            //first, recalculate our grid if a machine has reported that we have to recalculate our machine grid, or one of our machines disappeared from the grid.

            
            if(!(machines.Count() > 0))
            {
                Console.WriteLine("Ticking machine thread has no more machines! destroying ourselves!");
                this.is_running = false;
                return;
            }
            Console.WriteLine("Ticking machine thread group of size: " + machines.Count().ToString());
            //iterate all machines in no particular order.

            //TODO: Remake this to iterate the machine group for input and output item and power lists instead, rather than every machine individually - @989onan
            
            List<ItemSlot> slots_input = new List<ItemSlot>();

            //foreach (ModifiableTuple2Diff<ModifiableTuple2<Dictionary<string, int>>, int> list in processors)
            //{
            //    foreach(KeyValuePair<string, int> Item_iteration in list.Item1.Item1)
            //    {
            //        ItemSlot item_input = slots_input.Where(o => o.stored != null).ToList().Find(o => o.stored.Name == Item_iteration.Key);
            //        if(item_input == null)
            //        {
            //            item_input = new ItemSlot(null, 0);
            //            slots_input.Add(item_input);
            //        }
            //        item_input.max += list.Item2;
            //    }   
                
            //    //inputStorage.inventory.TryTakeAll() list.Item1
            //}
            //if (this.inputStorage.inventory.TryTakeAll(slots_input.ToArray()))
            //{
            //    Console.WriteLine("consuming items");
            //    slots_input.ForEach(o => o.count = 0);
            //    slots_input.ForEach(o => {
            //        this.inputStorage.inventory.RemoveFromAny(o);
                    
            //    }); 
            //    if(slots_input.Any(o=>o.max != o.count))
            //    {
            //        throw new Exception("List of items was modified inside of container during taking.");
            //    }
                
            //}
            //else
            //{
            //    return;
            //}

            //List<ItemSlot> slots_output = new List<ItemSlot>();
            //foreach (ModifiableTuple2Diff<ModifiableTuple2<Dictionary<string, int>>, int> list in processors)
            //{
            //    foreach (KeyValuePair<string, int> Item_iteration in list.Item1.Item2)
            //    {
            //        ItemSlot item_input = slots_output.Where(o => o.stored != null).ToList().Find(o => o.stored.Name == Item_iteration.Key);
            //        if (item_input == null)
            //        {
            //            item_input = new ItemSlot(null, 0);
            //            slots_output.Add(item_input);
            //        }
            //        item_input.max += list.Item2;
            //    }

            //    //inputStorage.inventory.TryTakeAll() list.Item1
            //}

            //if (this.inputStorage.inventory.TryPutAll(slots_input.ToArray()))
            //{
            //    Console.WriteLine("consuming items");
            //    slots_input.ForEach(o => o.count = o.max);
            //    slots_input.ForEach(o => {
            //        this.inputStorage.inventory.AddToAny(o);
            //    });
            //    if (slots_input.Any(o => 0 != o.count))
            //    {
            //        throw new Exception("List of items was modified inside of container during taking.");
            //    }

            //}
            //else
            //{
            //    return;
            //}
        }
    }
}
