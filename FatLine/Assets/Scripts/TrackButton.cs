using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrackButton : MonoBehaviour
{
    [SerializeField] GameObject brushPrefab;

    private string trackName;
    private TMP_InputField inputField;
    private GameObject parent;
    private Drawing drawing;

    public void Initialize(string name, float offset)
    {
        // Set Names
        trackName = name;
        gameObject.name = name;
        GetComponentInChildren<TextMeshProUGUI>().text = name;

        // Set Offset
        SetOffset(offset);

        // Find Lines Parent
        parent = FindFirstObjectByType<Drawing>().gameObject;

        // Find Drawing
        drawing = FindFirstObjectByType<Drawing>();

    }

    public void SetOffset(float offset)
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, offset);

    }

    public void Load()
    {
        if (drawing.isSaved)
        {
            // Replace InputField Text
            var obj = FindAnyObjectByType(typeof(TMP_InputField));
            obj.GetComponent<TMP_InputField>().text = trackName;
            EventSystem.current.SetSelectedGameObject(null);

            // Clear/Load and Instantiate Lines
            var foundLines = FindObjectsByType<LineRenderer>(FindObjectsSortMode.None);
            foreach (var line in foundLines)
            {
                Destroy(line.gameObject);
            }

            // Load and Instantiate Lines
            StartCoroutine(RebuildLines());

        }
        else if (!drawing.isSaved)
        {
            ConfirmationUI.Instance.ShowQuestion("Delete Line?", () =>
            {
                drawing.isSaved = true;
                Load();
            }, () =>
            {

            });
        }
    }

    IEnumerator RebuildLines()
    {
        yield return new WaitForEndOfFrame();

        var trackData = SaveSystem.LoadTrack(trackName);
        foreach (var line in trackData.lines)
        {
            var instance = Instantiate(brushPrefab, parent.transform);
            var lineRenderer = instance.GetComponent<LineRenderer>();

            // Rebuild Line
            for (int i = 0; i < line.positions.Count; i++)
            {
                lineRenderer.positionCount = line.positions.Count;
                lineRenderer.SetPosition(i, line.positions[i].ToVector3());
            }
        }

    }
}
