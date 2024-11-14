using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/savedata.json";
    private static SaveData currentData = new SaveData();

    // Save Classes
    [System.Serializable]
    public class SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [System.Serializable]
    public class LineData
    {
        public List<SerializableVector3> positions = new List<SerializableVector3>();

    }

    [System.Serializable]
    public class TrackData
    {
        public string name;
        public List<LineData> lines = new List<LineData>();

        public TrackData(string name)
        {
            this.name = name;
            lines = new List<LineData>();
        }

    }

    [System.Serializable]
    public class SaveData
    {
        public List<TrackData> tracks = new List<TrackData>();

        public TrackData GetTrack(string name)
        {
            return tracks.FirstOrDefault(track => track.name == name);
        }

        public void SetTrack(string name, List<LineData> newLines)
        {
            TrackData track = GetTrack(name);
            if (track == null)
            {
                track = new TrackData(name);
                tracks.Add(track);
            }

            track.lines = newLines;
        }

        public void RemoveTrack(string name)
        {
            tracks.RemoveAll(track => track.name == name);
        }

    }


    // Save Functions
    public static void SaveTrack(string name, List<LineData> lines)
    {
        currentData.SetTrack(name, lines);
        Save();

    }

    public static void Save()
    {
        string json = JsonUtility.ToJson(currentData, true);
        File.WriteAllText(savePath, json);

    }

    // Helper method to create LineData from a LineRenderer
    public static LineData CreateLineDataFromRenderer(LineRenderer renderer)
    {
        LineData lineData = new LineData();
        Vector3[] positions = new Vector3[renderer.positionCount];
        renderer.GetPositions(positions);

        lineData.positions = positions.Select(pos => new SerializableVector3(pos)).ToList();
        return lineData;

    }    

    // Load Function
    public static TrackData LoadTrack(string name)
    {
        //Load();
        return currentData.GetTrack(name);
    }

    public static void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currentData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            currentData = new SaveData();
        }

    }



    // Delete Functions
    public static void Delete(string name)
    {
        currentData.RemoveTrack(name);
        Save();

    }

    public static void DeleteAll()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            currentData = new SaveData();
        }

    }

    // Static method to get all saved track names
    public static string[] GetAllTrackNames()
    {
        Load();
        return currentData.tracks.Select(track => track.name).ToArray();

    }

    // Static method to get number of lines in a track
    public static int GetLineCount(string name)
    {
        Load();
        var track = currentData.GetTrack(name);
        return track?.lines.Count ?? 0;

    }


}
