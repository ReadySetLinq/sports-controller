
using System.Reflection;

namespace SportsController.Shared
{
    public static class Globals
    {
        public static bool DEBUG_MODE { set; get; } = false;

        public static int INVALID_INT = -763281543;

        // Set the property value of a class/object
        public static void SetObjectValue(string propertyName, string value, object obj)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName);
            // make sure object has the property we are after
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, value);
            }
        }

        public static string GetObjectValue(string propertyName, object obj)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName);
            // make sure object has the property we are after
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(obj).ToString();
            }

            return "";
        }
    }
}
