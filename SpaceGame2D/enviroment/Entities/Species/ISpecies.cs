using SpaceGame2D.graphics.compiledshaders;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.Entities.Species
{
    public interface ISpecies
    {
        

        //data
        string name { get; }

        string description { get; }

        //locomotion
        Vector2 stand_size { get;  }

        Vector2 crouchsize { get;  }

        Vector2 crawlsize { get;  }

        float walk_speed { get; } //Meters Per Second

        float run_speed { get; } //Meters Per Second
        float crouch_speed { get; } //Meters Per Second
        float crouch_run_speed { get; } //Meters Per Second

        float crawl_speed { get; } //Meters Per Second

        float crawl_run_speed { get; } //Meters Per Second

        float weight { get; } //kilograms in earth gravity

        float jump_height { get; } //meters


        //damaging
        int base_armor {  get; }

        int max_health { get; }

        int base_mana {  get; }

        string standing { get; }
    }
}
