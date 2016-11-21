using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

/// <summary>
/// A class that can read a string representation of a JSON object and convert it to an object
/// </summary>
public class ReadJSON
{
    private List<JsonObject> jsonSounds;

    public ReadJSON(string jsonString)
    {
        List<string> sounds = parseJson(jsonString);
        jsonSounds = new List<JsonObject>();

        foreach (string s in sounds)
        {
            jsonSounds.Add(JsonObject.CreateFromJSON(s));
        }

    }

    /// <summary>
    /// Gets the positions of the sounds in the real world
    /// </summary>
    /// <returns>List containing positions of sounds in the real world</returns>
    public List<Vector3> getPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (JsonObject o in jsonSounds)
        {
            //Divide Y by 2 in order to create less of a drastic change in height when raycasting
            if (o.location[2] > 0)
                positions.Add(new Vector3(o.location[0], o.location[2]/2, o.location[1]));
            else
                positions.Add(new Vector3(o.location[0], o.location[2]/2, o.location[1]));
        }
        return positions;
    }

    /// <summary>
    /// Gets the loudnesses of all sounds detected in the real world
    /// </summary>
    /// <returns>A list containing the loudness of all sounds detected in the real world</returns>
    public List<double> getLoudness()
    {
        List<double> loudnessValues = new List<double>();
        foreach (JsonObject o in jsonSounds)
        {
            loudnessValues.Add(o.loudness);
        }
        return loudnessValues;
    }

    /// <summary>
    /// Gets the first frame of each sound in the JSON object
    /// </summary>
    /// <returns>A list of ints representing the first frame of each sound in the JSON object</returns>
    public List<int> getFirstFrameIDs()
    {
        List<int> firstFrameList = new List<int>();
        foreach (JsonObject o in jsonSounds)
        {
            firstFrameList.Add(o.first_frame);
        }
        return firstFrameList;
    }

    /// <summary>
    /// Parses the JSON string that is retrieved from the server
    /// </summary>
    /// <param name="jsonString">A string representing a JSON object</param>
    /// <returns>A list of string representations of the detected sound positions in the real world</returns>
    List<string> parseJson(string jsonString)
    {
        List<string> ret = new List<string>();
        if (jsonString.Length == 0 || jsonString == null) return ret;
        if (jsonString.Contains("[]")) return ret;
        string remainingJson = jsonString.Substring(jsonString.IndexOf("[") + 1);
        ret.Add(remainingJson.Substring(0, remainingJson.IndexOf("}") + 1));
        remainingJson = remainingJson.Substring(remainingJson.IndexOf("}") + 1);
        //Parse through the json string and remove each object inside the array
        while (remainingJson.Length > 5)
        {
            ret.Add(remainingJson.Substring(1, remainingJson.IndexOf("}")).Trim());
            remainingJson = remainingJson.Substring(remainingJson.IndexOf("}") + 1);
        }
        return ret;
    }
}

/// <summary>
/// An object that can hold information pertaining to a JSON object from the server
/// </summary>
[Serializable]
public class JsonObject
{
    public float[] location;
    public int first_frame;
    public int last_frame;
    public double loudness;

    public static JsonObject CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<JsonObject>(jsonString);
    }
}