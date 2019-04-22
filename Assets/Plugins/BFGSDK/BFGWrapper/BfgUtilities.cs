using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace BFGSDK
{
    public class BfgUtilities
    {
        // Public

        /// <summary>
        /// Converts the dictionary of strings to json string.
        /// </summary>
        /// <returns>The dictionary of strings to json string.</returns>
        /// <param name="dictionaryOfStrings">Dictionary of strings.</param>
        public static string ConvertDictionaryOfStringsToJsonString(Dictionary<string, string> dictionaryOfStrings)
        {
            if (dictionaryOfStrings == null)
            {
                return null;
            }

            if (dictionaryOfStrings.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                foreach (KeyValuePair<string, string> entry in dictionaryOfStrings)
                {
                    sb.AppendFormat("\"{0}\":\"{1}\",", entry.Key, entry.Value);
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("}");

                return sb.ToString();
            }
            else
            {
                return "{}";
            }
        }

        /// <summary>
        /// Converts the array of strings to json string.
        /// </summary>
        /// <returns>The array of strings to json string.</returns>
        /// <param name="listOfStrings">List of strings.</param>
        public static string ConvertArrayOfStringsToJsonString(List<string> listOfStrings)
        {
            if (listOfStrings == null)
            {
                return null;
            }

            if (listOfStrings.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[");
                foreach (string s in listOfStrings)
                {
                    sb.AppendFormat("\"{0}\",", s);
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("]");

                return sb.ToString();
            }
            else
            {
                return "[]";
            }
        }

        /// <summary>
        /// Converts the object to json string.
        /// </summary>
        /// <returns>The object to json string.</returns>
        /// <param name="objectToSerialize">Object to serialize.</param>
        public static string ConvertObjectToJsonString(System.Object objectToSerialize)
        {
            return JsonUtility.ToJson(objectToSerialize);
        }
    }
}