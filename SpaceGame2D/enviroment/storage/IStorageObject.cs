using SpaceGame2D.graphics.ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.storage
{
    public interface IStorageObject
    {
        StorageContainer inventory { get; }

        IStorageScreen storageScreen { get; }
        string Name { get; set;  }
    }
}
