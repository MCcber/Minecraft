using System.Text.Json;

namespace cbhk_environment.GeneralTools
{
    public class JsonHelper
    {
        /// <summary>
        /// 格式化JSON文本
        /// </summary>
        /// <param name="unPrettyJson"></param>
        /// <returns></returns>
        public string PrettyJson(string unPrettyJson)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            var jsonElement = JsonSerializer.Deserialize<JsonElement>(unPrettyJson);

            return JsonSerializer.Serialize(jsonElement, options);
        }
    }
}
