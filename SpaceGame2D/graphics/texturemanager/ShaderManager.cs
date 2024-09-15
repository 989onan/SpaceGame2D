using SpaceGame2D.graphics.compiledshaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.graphics.texturemanager
{
    public class ShaderManager
    {
        private static Dictionary<string,IShader> _shaders = new Dictionary<string,IShader>();
        public ShaderManager()
        {

        }

        public static void LoadAll()
        {
            foreach (IShader shader in _shaders.Values)
            {
                shader.Load();
            }
        }

        public static bool register(string shader_key, IShader shader)
        {
            if (_shaders.ContainsKey(shader_key))
            {
                return false;
            }
            _shaders.Add(shader_key,shader);
            return true;
        }

        public static bool deregister(string shader_key)
        {
            if (!_shaders.ContainsKey(shader_key))
            {
                return false;
            }
            _shaders.TryGetValue(shader_key, out IShader shader);
            shader.Dispose();
            _shaders.Remove(shader_key);
            return true;
        }

        public static IReadOnlyDictionary<string,IShader> getAll()
        {
            return _shaders.AsReadOnly();
        }
    }
}
