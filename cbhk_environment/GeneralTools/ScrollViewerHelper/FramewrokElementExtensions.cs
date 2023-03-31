using System.Windows.Media;
using System.Windows;

namespace cbhk_environment.GeneralTools.ScrollViewerHelper
{
    public static class FramewrokElementExtensions
    {
        /// <summary>
        /// WPF中查找元素的父元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        public static T FindParent<T>(this DependencyObject element) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            if (parent != null)
            {
                if (parent is T)
                {
                    return (T)parent;
                }
                else
                {
                    parent = FindParent<T>(parent);
                    if (parent != null && parent is T)
                    {
                        return (T)parent;
                    }
                }
            }
            return null;
        }
    }
}
