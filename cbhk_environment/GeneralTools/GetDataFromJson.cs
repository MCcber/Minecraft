using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Windows.Media.Capture;

namespace cbhk_environment.GeneralTools
{
    public class GetDataFromJson
    {
        private static List<string> result = new List<string>();
        private static TreeView view = new TreeView();
        public static List<string> GetData(string json,List<string> data_path)
        {
            string current_path = "";
            if (data_path.Count>0)
            {
                Regex.Match(current_path, @"(?<=\[)([\s\S]*)(?=\])");
                current_path = data_path[0];
                int index;
                //解析数组
                if(current_path.Contains("[") && current_path.Contains("?"))
                {
                    data_path.RemoveAt(0);
                    JArray array = JArray.Parse(json);
                    if (data_path.Count > 0)
                        GetData(array.ToString(),data_path);
                }
            }
            JToken result_token = JToken.Parse(json);
            switch (result_token.Type)
            {
                case JTokenType.None:
                    break;
                case JTokenType.Object:
                    break;
                case JTokenType.Array:
                    {
                        foreach (JToken item in result_token.Children())
                        {
                            if (!result.Contains(item.ToString()))
                                result.Add(item.ToString().Replace("{","").Replace("}",""));
                        }
                    }
                    break;
                case JTokenType.Constructor:
                    break;
                case JTokenType.Property:
                    break;
                case JTokenType.Comment:
                    break;
                case JTokenType.Integer:
                    break;
                case JTokenType.Float:
                    break;
                case JTokenType.String:
                    break;
                case JTokenType.Boolean:
                    break;
                case JTokenType.Null:
                    break;
                case JTokenType.Undefined:
                    break;
                case JTokenType.Date:
                    break;
                case JTokenType.Raw:
                    break;
                case JTokenType.Bytes:
                    break;
                case JTokenType.Guid:
                    break;
                case JTokenType.Uri:
                    break;
                case JTokenType.TimeSpan:
                    break;
                default:
                    break;
            }
            return result;
        }

        public static TreeView GetTreeViewFromJson(StringBuilder json,TreeViewItem viewItem)
        {
            JObject obj = JToken.Parse(json.ToString()) as JObject;
            foreach (var item in obj.Children())
            {
                if ((item.Type == JTokenType.Property || item.Type == JTokenType.Array || item.Type == JTokenType.Object) && item.Children().Count() > 0)
                {
                    TreeViewItem subnewItem = new TreeViewItem()
                    {
                        Header = Regex.Match(item.ToString(), "\".*\"").ToString().Replace("\"","")
                    };
                    viewItem.Items.Add(subnewItem);
                    GetTreeViewFromJson(new StringBuilder(item.ToString()), subnewItem);
                }
                TreeViewItem newItem = new TreeViewItem()
                {
                    Header = Regex.Match(item.ToString(), "\".*\"").ToString()
                };
                viewItem.Items.Add(newItem);
            }
            return view;
        }
    }
}
