namespace cbhk_environment.GeneralTools
{
    public static class JsonTypeSetting
    {
        public static string PraseToJson(string str)
        {
            var tabIndex = 0;
            for (int i = 0; i < str.Length; i++)
            {
                var cr = str[i];

                if (cr == '{' || cr == '[')
                {
                    var prestr = str.Substring(0, i);
                    var strsuff = str.Substring(i + 1, str.Length - i - 1);
                    str = prestr + cr + "\n" + getSpace(tabIndex + 1) + strsuff;

                    i += (1 + 2 * (tabIndex + 1));
                    tabIndex++;
                }
                else if (cr == '}' || cr == ']')
                {
                    var prestr = str.Substring(0, i);
                    var strsuff = str.Substring(i + 1, str.Length - i - 1);
                    str = prestr + "\n" + getSpace(tabIndex - 1) + cr + strsuff;

                    i += (1 + 2 * (tabIndex - 1));
                    tabIndex--;

                }
                else if (cr == ',')
                {
                    var prestr = str.Substring(0, i + 1);
                    var strsuff = str.Substring(i + 1, str.Length - i - 1);
                    str = prestr + "\n" + getSpace(tabIndex) + strsuff;
                    i += (1 + 2 * tabIndex);
                }
            }

            return str.Trim();
        }

        private static string getSpace(int num)
        {
            string result = string.Empty;
            for (int i = 0; i < num; i++)
            {
                result += "  ";
            }

            return result;
        }
    }
}
