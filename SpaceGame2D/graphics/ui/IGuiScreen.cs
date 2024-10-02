using SpaceGame2D.graphics.renderables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.graphics.ui
{
    public interface IGuiScreen: IRenderable
    {

        public bool IsVisible { get; set;  }


        public void OpenUI();

        public void CloseGui();

    }
}
