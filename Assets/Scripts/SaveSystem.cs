using UnityEngine;

public static class SaveSystem
{
    public static string GetStringValue(string key)
    {
        return PlayerPrefs.GetString(key);
    }

    public static void SetStringValue(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }

    public static float GetFloatValue(string key)
    {
        return PlayerPrefs.GetFloat(key);
    }

    public static void SetFloatValue(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    public static int GetIntValue(string key)
    {
        return PlayerPrefs.GetInt(key);
    }

    public static void SetIntValue(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }
}
