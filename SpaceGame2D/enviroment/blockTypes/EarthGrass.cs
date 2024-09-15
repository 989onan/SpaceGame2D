using SpaceGame2D.enviroment.materials;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.blockTypes
{
    internal class EarthGrass : Air
    {

        public new IMaterial surfaceProperties => new EarthMaterial();
    }
}
