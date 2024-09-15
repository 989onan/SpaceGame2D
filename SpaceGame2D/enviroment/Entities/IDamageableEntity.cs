using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.Entities
{
    public interface IDamageableEntity: IEntity
    {
        int Health { get; set; }

        int Damage(int health);

        int Heal(int health);

        int Armor {  get; }

        int MagicArmor { get; }


    }
}
