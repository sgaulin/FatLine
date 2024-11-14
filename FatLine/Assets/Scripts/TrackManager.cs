using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class TrackManager : MonoBehaviour
{    
    [SerializeField] GameObject parent; 
    [SerializeField] GameObject buttonPrefab;
    [SerializeField] float listSpacing = 30; 
    [SerializeField] List<GameObject> buttonList = new List<GameObject>(); //TODO: Merge with dictionary?
    private Drawing drawing;
    private bool isInitialized;


    private void Start()
    {
        Initialize();

    }

    private void Initialize()
    {
        // Clear Button List
        buttonList.Clear();

        // Load SaveData
        SaveSystem.Load();

        // Intantiate Buttons
        foreach(var trackName in SaveSystem.GetAllTrackNames())
        {
            EnterTrack(trackName);
        }

        // Find Drawing
        drawing = FindFirstObjectByType<Drawing>();

        isInitialized= true;
        
    }

    public void EnterTrack(string value)
    {
        if (isUnique(value) && !string.IsNullOrEmpty(value))
        {
            //Instantiate button
            var instance = Instantiate(buttonPrefab, parent.transform);

            //Calculate offset
            int count = parent.transform.childCount - 1;
            float offset = listSpacing * count;

            //Initialize button
            instance.GetComponent<TrackButton>().Initialize(value, offset);

            //Add to list
            buttonList.Add(instance);

            if (isInitialized)
            {
                //Save
                SaveSystem.SaveTrack(value, GetLines());
                drawing.isSaved = true;
            }            
        }   
        
    }

    public void DeledeTrack()
    {
        //Get the name
        var inputField = FindAnyObjectByType(typeof(TMP_InputField));
        var input = inputField.GetComponent<TMP_InputField>();
        string name = input.text;

        //Clear InputField
        input.text = null;

        //Delete Button
        var trackButton = GameObject.Find(name);
        Destroy(trackButton);

        //Delete from list
        for (int i = 0; i < buttonList.Count; i++) //TODO: Use dictionary instead
        { 
            if (buttonList[i].name == name) 
            { 
                buttonList.RemoveAt(i);
                break;
            }
        }

        //Delete from Save
        SaveSystem.Delete(name);

        //Recalculate offsets
        for (int i = 0; i < buttonList.Count; i++)
        {
            float offset = listSpacing * i;

            buttonList[i].GetComponent<TrackButton>().SetOffset(offset);
        }

    }

    private bool isUnique(string name)
    {
        for (int i = 0; i < buttonList.Count; i++)  //TODO: Use dictionary instead
        {
            if (buttonList[i].name == name)
            {
                return false;
            }            
        }
        return true;

    }

    private List<SaveSystem.LineData>GetLines()
    {
        List<SaveSystem.LineData> lines = new List<SaveSystem.LineData>();

        var foundLines = FindObjectsByType<LineRenderer>(FindObjectsSortMode.None);
        foreach (LineRenderer line in foundLines) 
        {
            SaveSystem.LineData data = SaveSystem.CreateLineDataFromRenderer(line);
            lines.Add(data);
        }

        return lines;
    }
        
}
