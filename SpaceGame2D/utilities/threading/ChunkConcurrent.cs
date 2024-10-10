using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.utilities.threading
{
    public static class ChunkConcurrent
    {

        //I think this is one of my coolest generics here - @989onan
        public static Task[] ChunkTask<TSource>(this IEnumerable<TSource> source, int size, Action<TSource[]> action)
        {
            Task[] tasks = new Task[source.Count()];
            int i = 0;  
            source.Chunk(size).ToList().ForEach(o => {
                tasks[i] = Task.Run(()=> action(o));
                i++;
            });
            return tasks;
        }
    }
}
