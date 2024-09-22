using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.species
{
    public class SpeciesRegistry
    {
        private static Dictionary<string,ISpecies> Registry = new Dictionary<string,ISpecies>();
        public static void GenerateList()
        {
            Registry.Clear();
            Registry.Add("SpaceGame2D:Human", new Human());
            Registry.Add("SpaceGame2D:Avali", new Avali());

        }

        public static ISpecies getSpecies(string name)
        {
            Registry.TryGetValue(name, out ISpecies species);
            return species;
        }



        public delegate void LoadSpecies(Dictionary<string, ISpecies> speciesList);

        public event LoadSpecies LoadingSpecies;

        protected virtual void RaiseSampleEvent()
        {
            // Raise the event in a thread-safe manner using the ?. operator.
            LoadingSpecies?.Invoke(SpeciesRegistry.Registry);
        }


    }
}
