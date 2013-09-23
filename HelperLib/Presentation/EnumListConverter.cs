using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HelperLib.Presentation {
    public class EnumListConverter {
        /// <summary>
        /// return the enumration value based on the provided description
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public static T GetItem<T>(string description) {
            Type enumType = typeof(T);
            foreach(object item in Enum.GetValues(enumType)) {
                string itemString = GetItemDescription((T)item);
                if(itemString == description) {
                    return (T)item;
                }
            }
            return default(T);
        }

        public static string GetItemDescription<T>(T item) {
            Type enumType = typeof(T);
            return GetItemDescription(enumType, item);
        }

        public static string GetItemDescription(Type enumType, object item) {
            string itemString = item.ToString();
            FieldInfo field = enumType.GetField(itemString);
            object[] attribs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if(null != attribs && attribs.Length > 0) {
                itemString = ((DescriptionAttribute)attribs[0]).Description;
            }
            return itemString;
        }

        public static List<string> GetItemDescriptions(Type enumType) {
            List<string> items = new List<string>();
            foreach(object item in Enum.GetValues(enumType)) {
                items.Add(GetItemDescription(enumType, item));
            }
            return items;
        }
    }
}
