#if !UNITY_5_3_OR_NEWER
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Photon.LoadBalancing.Custom.Common
{
    public class Slugfy
    {

        public static void TestString()
        {
            string test = Slugfy.GenerateSlug("tên tiếng việt test");
            Console.WriteLine(test);
        }

        protected Config _config { get; set; }

        private static Slugfy _instance;
        public static Slugfy Instance { get { if (_instance == null) { _instance = new Slugfy(); } return _instance; } }

        public Slugfy() : this(new Slugfy.Config()) { }

        public Slugfy(Config config)
        {
            if (config != null)
                _config = config;
            else
                throw new ArgumentNullException("config", "can't be null use default config or empty construct.");
        }

        public static String GenerateSlug(String str)
        {
            if (Instance._config.ForceLowerCase) str = str.ToLower();

            str = Instance.CleanWhiteSpace(str, Instance._config.CollapseWhiteSpace);
            str = Instance.ApplyReplacements(str, Instance._config.CharacterReplacements);
            str = Instance.RemoveDiacritics(str);
            str = Instance.DeleteCharacters(str, Instance._config.DeniedCharactersRegex);

            return str;
        }

        protected String CleanWhiteSpace(String str, Boolean collapse)
        {
            return Regex.Replace(str, collapse ? @"\s+" : @"\s", " ");
        }

        // Thanks http://stackoverflow.com/a/249126!
        protected String RemoveDiacritics(String str)
        {
            string stFormD = str.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

        protected String ApplyReplacements(String str, Dictionary<string, string> replacements)
        {
            StringBuilder sb = new StringBuilder(str);

            foreach (KeyValuePair<string, string> replacement in replacements)
                sb.Replace(replacement.Key, replacement.Value);

            return sb.ToString();
        }

        protected String DeleteCharacters(String str, String regex)
        {
            return Regex.Replace(str, regex, "");
        }

        public class Config
        {
            public Dictionary<String, String> CharacterReplacements { get; set; }
            public Boolean ForceLowerCase { get; set; }
            public Boolean CollapseWhiteSpace { get; set; }
            public String DeniedCharactersRegex { get; set; }

            public Config()
            {
                CharacterReplacements = new Dictionary<string, string>();
                CharacterReplacements.Add(" ", "-");

                ForceLowerCase = true;
                CollapseWhiteSpace = true;
                DeniedCharactersRegex = @"[^a-zA-Z0-9\-\._]";
            }
        }

    }
}
#endif