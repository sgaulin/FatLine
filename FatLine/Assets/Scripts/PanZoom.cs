using UnityEngine;
using UnityEngine.InputSystem;

public class PanZoom : MonoBehaviour
{
    [SerializeField] float zoomMin = 0.1f;
    [SerializeField] float zoomMax = 10;
    [SerializeField] float snapZoomTolerence = 1;
    [SerializeField] float snapPosTolerence = 0.5f;

    private Vector3 inputStart;


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {            
            inputStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.touchCount == 2)
        {
            Touch touchZero= Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = prevMagnitude - currentMagnitude;

            Zoom(difference * -0.01f);

            Pan();
        }

        ////For testing on desktop
        //if (Input.GetMouseButton(0))
        //{
        //    Pan();
        //}

        if (Input.GetMouseButtonUp(0))
        {
            Snap();
        }
    }

    void Zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomMin, zoomMax);
    }

    void Pan()
    {
        Vector3 direction = inputStart - Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        Camera.main.transform.position += direction;
    }

    void Snap()
    {
        //Position
        Transform cam = Camera.main.transform;
        if (cam.position.x < snapPosTolerence && cam.position.x > -snapPosTolerence)
        {
            if (cam.position.y < snapPosTolerence && cam.position.y > -snapPosTolerence)
            {
                cam.position = new Vector3(0, 0, cam.position.z);
            }
        }

        //Zoom
        if (Camera.main.orthographicSize < 5 + snapZoomTolerence && Camera.main.orthographicSize > 5 - snapZoomTolerence)
        {
            Camera.main.orthographicSize = 5;
        }
    }
    
}
