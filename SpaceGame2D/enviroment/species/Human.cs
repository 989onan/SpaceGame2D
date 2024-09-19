using SpaceGame2D.graphics.texturemanager;
using SpaceGame2D.graphics.texturemanager.packer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.species
{
    public class Human: ISpecies
    {
        public string standing_image => "species/human/standing.png";
        
        public Human() {
            Atlas.AddToQue(new Texture(standing_image));
            
        }

        public static Human instance = new Human();
    }
}
