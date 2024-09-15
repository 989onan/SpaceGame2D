using SpaceGame2D.threads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D
{
    public class BootStrapper
    {

#pragma warning disable CS8601 // Possible null reference assignment.
        public static string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
#pragma warning restore CS8601 // Possible null reference assignment.
        public static void Main(string[] args)
        {
            MainThread mainThread = new MainThread();
        }
    }
}
