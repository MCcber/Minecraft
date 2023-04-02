using cbhk_environment.CustomControls;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace cbhk_environment.GeneralTools.TreeViewComponentsHelper
{
    /// <summary>
    /// 负责JSON数据转换为TreeView成员
    /// </summary>
    public class TreeViewConveter
    {
        /// <summary>
        /// 普通文本块的灰刷
        /// </summary>
        private static SolidColorBrush grayBrush = new((Color)ColorConverter.ConvertFromString("#838383"));

        /// <summary>
        /// 处理JSON数据，返回一个可提醒UI更新的树视图节点集合
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ObservableCollection<RichTreeViewItems> Handler(string data)
        {
            //返回结果
            ObservableCollection<RichTreeViewItems> result = new();
            //将原始数据解析为JArray对象
            JArray rootArray = JArray.Parse(data);
            //遍历JArray对象
            foreach (JObject firstLayerObj in rootArray)
            {
                RichTreeViewItems rootItem = new()
                {
                    ConnectingLineFill = grayBrush,
                    Style = Application.Current.Resources["RichTreeViewItems"] as Style,
                    Tag = firstLayerObj.ToString()
                };
                result.Add(rootItem);
                if (JArray.Parse(firstLayerObj["children"].ToString()).Count > 0)
                {
                    rootItem.Items.Add(new TreeViewItem());
                    rootItem.Expanded += CompoundItemExpanded;
                }
            }

            return result;
        }

        /// <summary>
        /// 展开复合节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CompoundItemExpanded(object sender, RoutedEventArgs e)
        {
            #region 清空用于展开的空白子级
            RichTreeViewItems currentItem = sender as RichTreeViewItems;
            currentItem.Items.Clear();
            #endregion

            #region 判断是否拥有子级,有则处理子级数据
            string data = currentItem.Tag.ToString();
            JObject Obj = JObject.Parse(data);
            JArray childrenArray = JArray.Parse(Obj["children"].ToString());

            #region 设置为空,避免反复处理子结构
            Obj["children"] = "[]";
            currentItem.Tag = Obj.ToString();
            #endregion

            if (childrenArray.Count > 0)
            {
                foreach (JObject subObj in childrenArray.Cast<JObject>())
                {
                    #region 子节点
                    RichTreeViewItems subItem = new()
                    {
                        ConnectingLineFill = grayBrush,
                        Style = Application.Current.Resources["RichTreeViewItems"] as Style,
                        Tag = subObj.ToString()
                    };
                    JArray subChildren = JArray.Parse(subObj["children"].ToString());
                    if(subChildren.Count > 0)
                    {
                        subItem.Items.Add(new TreeViewItem());
                        subItem.Expanded += CompoundItemExpanded;
                    }
                    #endregion

                    #region 绑定器
                    Binding componentsBinder = new()
                    {
                        Path = new PropertyPath("Tag"),
                        Mode = BindingMode.OneTime,
                        RelativeSource = RelativeSource.Self,
                        Converter = new TagToHeader(),
                        ConverterParameter = subItem
                    };
                    #endregion

                    BindingOperations.SetBinding(subItem, HeaderedItemsControl.HeaderProperty, componentsBinder);
                    currentItem.Items.Add(subItem);
                }
            }
            #endregion
        }
    }
}
