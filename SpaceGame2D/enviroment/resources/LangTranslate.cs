using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace SpaceGame2D.enviroment.resources
{
    public class LangTranslate
    {
        public static string language { get; private set; }

        private static LanguageSelection translation_dict;

        public static string translate(string key)
        {
            return translation_dict.messages[key];
        }

        public static string[] getAuthors()
        {
            return translation_dict.authors;
        }

        public static bool LoadTranslations(string language)
        {
            string file_path = Path.Join(BootStrapper.path, "lang", language + ".json");
            if (!Path.Exists(file_path))
            {
                return false;
            }
            LangTranslate.language = language.ToLower();
            using (StreamReader r = new StreamReader(file_path))
            {
                translation_dict = JsonSerializer.Deserialize< LanguageSelection>(r.ReadToEnd());
            }

            return true;
        }


        private class LanguageSelection
        {
            public string[] authors;
            public Dictionary<string, string> messages;
            public LanguageSelection()
            {

            }

        }
    }
}
