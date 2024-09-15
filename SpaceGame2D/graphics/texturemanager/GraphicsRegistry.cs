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

        private static List<IRenderableTile> _graphicObjects = new List<IRenderableTile>();
        public GraphicsRegistry() {
        
        }

        public static void LoadAll()
        {
            foreach(IRenderableTile tile in _graphicObjects)
            {
                tile.LoadGraphic();
            }
            
        }

        public static bool registerRenderGraphic(IRenderableTile renderGraphic)
        {
            if (_graphicObjects.Contains(renderGraphic)) {
                return false;
            }
            _graphicObjects.Add(renderGraphic);
            return true;
        }

        public static bool deregisterAABB(IRenderableTile renderGraphic)
        {
            if (!_graphicObjects.Contains(renderGraphic))
            {
                return false;
            }
            renderGraphic.DisposeGraphic();
            _graphicObjects.Remove(renderGraphic);
            return true;
        }

        public static IReadOnlyList<IRenderableTile> getAll() {
            return _graphicObjects.AsReadOnly(); 
        }
    }
}
