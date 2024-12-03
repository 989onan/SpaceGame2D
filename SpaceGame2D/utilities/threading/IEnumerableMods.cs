using OpenTK.Graphics.ES11;
using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.utilities.threading
{
    public static class IEnumerableMods
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

        public static IEnumerable<ModifiableTuple2Diff<TSource,int>> GroupDistincts<TSource>(this IEnumerable<TSource> source) {
            List<ModifiableTuple2Diff<TSource, int>> result = new List<ModifiableTuple2Diff<TSource, int>>();


            foreach (TSource item in source)
            {
                if (source.Contains(item))
                {
                    result.Find(o => item.Equals(item)).Item2++;

                }
                else
                {
                    result.Add(new ModifiableTuple2Diff<TSource,int>(item, 1));
                }
            }

            return result.AsEnumerable();
        }
    }
}
