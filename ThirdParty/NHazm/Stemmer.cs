namespace NHazm
{
    public class Stemmer
    {
        private string[] _ends = new string[] {
            "ات", "ان", "ترین", "تر", "م", "ت", "ش", "یی", "ی", "ها", "ٔ"
        };

        public string Stem(string word)
        {

            foreach (var end in this._ends)
            {
                if (string.Equals(end, word, System.StringComparison.InvariantCultureIgnoreCase))
                    return "";

                try
                {
                    if (word.EndsWith(end))
                    {
                        word = word.Substring(0, word.Length - end.Length).Trim();
                    }
                }
                catch { }
            }

            return word;
        }
    }
}