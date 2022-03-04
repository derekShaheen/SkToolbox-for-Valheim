using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SkToolbox.Utility
{
    public static class SkUtilities
    {
        public static bool ConvertInternalWarningsErrors = false; // Should we allow output of warnings and errors from SkToolbox, or suppress them all to regular log output? // True = suppress
        public static BindingFlags BindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        /// <summary>
        /// Uses reflection to get the field value from an object.
        /// </summary>
        ///
        /// <param name="type">The instance type.</param>
        /// <param name="instance">The instance object.</param>
        /// <param name="fieldName">The field's name which is to be fetched.</param>
        ///
        /// <returns>The field value from the object.</returns>
        //internal static object GetInstanceField(System.Type type, object instance, string fieldName)
        //{
        //    FieldInfo field = type.GetField(fieldName, BindFlags);
        //    return field.GetValue(instance);
        //}

        //internal static object SetInstanceField(System.Type type, object instance, string fieldName, object fieldValue)
        //{
        //    FieldInfo field = type.GetField(fieldName, BindFlags);
        //    field.SetValue(instance, fieldValue);

        //    return field.GetValue(instance);
        //}

        public static void SetPrivateField(this object obj, string fieldName, object value)
        {
            var prop = obj.GetType().GetField(fieldName, BindFlags);
            prop.SetValue(obj, value);
        }

        public static T GetPrivateField<T>(this object obj, string fieldName)
        {
            var prop = obj.GetType().GetField(fieldName, BindFlags);
            var value = prop.GetValue(obj);
            return (T)value;
        }

        public static void SetPrivateProperty(this object obj, string propertyName, object value)
        {
            var prop = obj.GetType()
                .GetProperty(propertyName, BindFlags);
            prop.SetValue(obj, value, null);
        }

        public static T GetPrivateProperty<T>(this object obj, string propertyName)
        {
            var prop = obj.GetType().GetProperty(propertyName, BindFlags);
            var value = prop.GetValue(obj);
            return (T)value;
        }

        public static void InvokePrivateMethod(this object obj, string methodName, object[] methodParams)
        {
            MethodInfo dynMethod = obj.GetType().GetMethod(methodName, BindFlags);
            dynMethod.Invoke(obj, methodParams);
        }

        public static Component CopyComponent(Component original, Type originalType, Type overridingType,
            GameObject destination)
        {
            var copy = destination.AddComponent(overridingType);
            var fields = originalType.GetFields(BindFlags);
            foreach (var field in fields)
            {
                var value = field.GetValue(original);
                field.SetValue(copy, value);
            }

            return copy;
        }

        public enum Status
        {
            Initialized,
            Loading,
            Ready,
            Error,
            Unload
        }

        /// <summary>
        /// Used for logging to the console in a controlled manner<br>Example Usage: SkUtilities.Logz(new string[] { "CMD", "REQ" }, new string[] { "Submenu Created" });</br>
        /// </summary>
        /// <param name="categories"></param>
        /// <param name="messages"></param>
        /// <param name="callerClass"></param>
        /// <param name="callerMethod"></param>
        public static void Logz(string[] categories, string[] messages, LogType logType = LogType.Log)
        {
            string strBuild = string.Empty;
            if (categories != null)
            {
                foreach (string cat in categories)
                {
                    strBuild = strBuild + " (" + cat + ") -> ";
                }
            }
            if (messages != null)
            {
                foreach (string msg in messages)
                {
                    if (msg != null)
                    {
                        strBuild = strBuild + msg + " | ";
                    }
                    else
                    {
                        strBuild = strBuild + "NULL" + " | ";
                    }
                }
                strBuild = strBuild.Remove(strBuild.Length - 2, 1);
            }
            //Get the class that called the log
            if (!ConvertInternalWarningsErrors)
            {
                switch (logType)
                {
                    case LogType.Error:
                        Debug.LogError("(SkToolbox) -> " + strBuild);
                        break;
                    case LogType.Warning:
                        Debug.LogWarning("(SkToolbox) -> " + strBuild);
                        break;
                    default:
                        Debug.Log("(SkToolbox) -> " + strBuild);
                        break;
                }
            }
            else
            {
                Debug.Log("(SkToolbox) -> " + strBuild);
            }
        }

        public static string Logr(string[] categories, string[] messages)
        {
            string strBuild = string.Empty;
            if (categories != null)
            {
                foreach (string cat in categories)
                {
                    strBuild = strBuild + " (" + cat + ") -> ";
                }
            }
            if (messages != null)
            {
                foreach (string msg in messages)
                {
                    if (msg != null)
                    {
                        strBuild = strBuild + msg + " | ";
                    }
                    else
                    {
                        strBuild = strBuild + "NULL" + " | ";
                    }
                }
                strBuild = strBuild.Remove(strBuild.Length - 2, 1);
            }
            return "(SkToolbox) -> " + strBuild;
        }

        /// <summary>
        /// Used for logging to the console in a controlled manner
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callerClass"></param>
        /// <param name="callerMethod"></param>
        public static void Logz(string message)
        {
            string strBuild = string.Empty;

            strBuild += " (OUT) -> ";
            strBuild = $"{strBuild}{message} ";

            Debug.Log("(SkToolbox) -> " + strBuild);
        }

        // GUI Items
        public static void RectFilled(float x, float y, float width, float height, Texture2D text)
        {
            GUI.DrawTexture(new Rect(x, y, width, height), text);
        }

        public static void RectOutlined(float x, float y, float width, float height, Texture2D text, float thickness = 1f)
        {
            RectFilled(x, y, thickness, height, text);
            RectFilled(x + width - thickness, y, thickness, height, text);
            RectFilled(x + thickness, y, width - thickness * 2f, thickness, text);
            RectFilled(x + thickness, y + height - thickness, width - thickness * 2f, thickness, text);
        }
    }
}

