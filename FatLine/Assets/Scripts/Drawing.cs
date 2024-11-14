using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Drawing : MonoBehaviour
{
    [SerializeField] Camera m_camera;
    [SerializeField] GameObject brush;
    [SerializeField] float lineTolerence = 0.01f;
    [SerializeField] string validSurfaceTag = "Bowl";

    public bool isSaved { get; set; } = true;

    private LineRenderer currentLineRenderer;
    private Vector2 lastPos;
    private bool isBrushCreated = false;

    private List<GameObject> lineList = new List<GameObject>();
    private List<GameObject> undoList = new List<GameObject>();

    private void Update()
    {
        Draw();
    }
    
    void Draw()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && isSurfaceValid())
        {
            CreateBrush();
        }
        if (Input.GetKey(KeyCode.Mouse0) && isSurfaceValid() && isBrushCreated && Input.touchCount <= 1)
        {
            Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);

            if (Vector2.Distance(mousePos, lastPos) > lineTolerence)
            {
                AddPoint(mousePos);
                lastPos = mousePos;
                isSaved = false;
            }
        }
        else
        {
            //Delete brush if it's just a click
            if(currentLineRenderer != null && currentLineRenderer.positionCount <= 1)
            {
                lineList.RemoveAt(lineList.Count - 1);
                Destroy(currentLineRenderer.gameObject);

                if(lineList.Count == 0)
                {
                    isSaved = true;
                }
            }

            currentLineRenderer = null;
            isBrushCreated = false;
        }
    }

    private void CreateBrush()
    {
        Clear(undoList);

        GameObject brushInstance = Instantiate(brush, transform);
        currentLineRenderer = brushInstance.GetComponent<LineRenderer>();

        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);

        currentLineRenderer.SetPosition(0,mousePos);
        currentLineRenderer.SetPosition(1,mousePos);
        currentLineRenderer.positionCount = 0;

        isBrushCreated = true;
        
        lineList.Add(brushInstance);
    }

    private void AddPoint(Vector2 pointPos)
    {
        currentLineRenderer.positionCount++;
        int positionIndex = currentLineRenderer.positionCount - 1;
        currentLineRenderer.SetPosition(positionIndex, pointPos);
    }

    public void Undo()
    {
        if(lineList.Count > 0)
        {
            var lastLineIndex = lineList.Count - 1;
            lineList[lastLineIndex].gameObject.SetActive(false);

            undoList.Add(lineList[lastLineIndex]);
            lineList.RemoveAt(lastLineIndex);
        }        
    }

    public void Redo()
    {
        if(undoList.Count > 0)
        {
            var lastUndoIndex = undoList.Count - 1;
            undoList[lastUndoIndex].gameObject.SetActive(true);

            lineList.Add(undoList[lastUndoIndex]);
            undoList.RemoveAt(lastUndoIndex);
        }        
    }        

    public void Clear()
    {
        // Clear Lines in List
        foreach (var line in lineList)
        {
            Destroy(line.gameObject);
        }

        // Clear List
        lineList.Clear();
        undoList.Clear();

        // Clear Loaded Lines
        var foundLines = FindObjectsByType<LineRenderer>(FindObjectsSortMode.None);
        foreach (var line in foundLines)
        {
            Destroy(line.gameObject);
        }
    }

    public void Clear(List<GameObject> lineList)
    {
        foreach (var line in lineList)
        {
            Destroy(line.gameObject);
        }

        lineList.Clear();
    }

    private bool isSurfaceValid()
    {
        RaycastHit hit;
        Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;

            if (objectHit.CompareTag(validSurfaceTag))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

}
