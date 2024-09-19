using SpaceGame2D.graphics.compiledshaders;
using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.graphics.texturemanager
{
    public class GraphicsRegistry
    {

        private static List<IRenderableGraphic> _graphicObjects = new List<IRenderableGraphic>();
        public GraphicsRegistry() {}

        public static void LoadAll()
        {
            foreach(IRenderableGraphic tile in _graphicObjects)
            {
                tile.LoadGraphic();
            }
            
        }

        public static bool registerRenderGraphic(IRenderableGraphic renderGraphic)
        {
            if (_graphicObjects.Contains(renderGraphic)) {
                return false;
            }
            _graphicObjects.Add(renderGraphic);
            return true;
        }

        public static bool deregisterRenderGraphic(IRenderableGraphic renderGraphic)
        {
            if (!_graphicObjects.Contains(renderGraphic))
            {
                return false;
            }
            renderGraphic.DisposeGraphic();
            _graphicObjects.Remove(renderGraphic);
            return true;
        }

        public static IReadOnlyList<IRenderableGraphic> getAll() {
            return _graphicObjects.AsReadOnly(); 
        }
    }
}
