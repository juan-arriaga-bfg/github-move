using System;
using System.Text;
using UnityEngine;

public static class EnumExtensions
{
    //checks to see if an enumerated value contains a type
    public static bool Has<T>(this System.Enum type, T value)
    {
        try
        {
            return (((int)(object)type &
              (int)(object)value) == (int)(object)value);
        }
        catch
        {
            return false;
        }
    }

    //checks if the value is only the provided type
    public static bool Is<T>(this System.Enum type, T value)
    {
        try
        {
            return (int)(object)type == (int)(object)value;
        }
        catch
        {
            return false;
        }
    }

    //appends a value
    public static T Add<T>(this System.Enum type, T value)
    {
        try
        {
            return (T)(object)(((int)(object)type | (int)(object)value));
        }
        catch (Exception)
        {
            Debug.Log(string.Format("Could not append value from enumerated type '{0}'.", typeof(T).Name));
        }
        return value;
    }

    //completely removes the value
    public static T Remove<T>(this System.Enum type, T value)
    {
        try
        {
            return (T)(object)(((int)(object)type & ~(int)(object)value));
        }
        catch (Exception)
        {
            Debug.Log(string.Format("Could not remove value from enumerated type '{0}'.", typeof(T).Name));
        }
        return value;
    }

    public static string PrettyPrint(this System.Enum val)
    {
        StringBuilder sb = new StringBuilder();
        foreach (Enum v in Enum.GetValues(val.GetType()))
        {
            if (val.Has(v))
            {
                sb.Append(v + " ");
            }
        }
        return sb.ToString();
    }
}