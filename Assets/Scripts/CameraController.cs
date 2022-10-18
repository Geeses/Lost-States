using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject map;
    public float panSpeed = 1f;
    public CursorLockMode cursorMode = CursorLockMode.Confined;
    public Vector2 mousePosition;

    [SerializeField] private List<Selectable> selectedObjects = new List<Selectable>();

    void Start()
    {
        Cursor.lockState = cursorMode;
    }

    private void Update()
    {
        mousePosition = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;

                if (objectHit.tag == "Selectable")
                {
                    Selectable selectable = objectHit.GetComponentInParent<Selectable>();

                    if (!selectable.Selected)
                    {
                        foreach (var unit in selectedObjects)
                        {
                            unit.Unselect();
                        }
                        selectedObjects.Clear();
                        selectedObjects.Add(selectable);
                        selectable.Select();
                    }

                }
                else
                {
                    if (selectedObjects.Count > 0)
                    {
                        foreach (var selectable in selectedObjects)
                        {
                            selectable.Unselect();
                        }

                        selectedObjects.Clear();
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if(mousePosition.x >= 0.95f)
        {
            PanCamera(Vector2.right);
        }
        else if (mousePosition.x <= 0.05f)
        {
            PanCamera(Vector2.left);
        }

        if(mousePosition.y >= 0.95f)
        {
            PanCamera(Vector2.up);
        }
        else if (mousePosition.y <= 0.05f)
        {
            PanCamera(Vector2.down);
        }
    }

    private void PanCamera(Vector2 direction)
    {
        gameObject.transform.Translate(new Vector3(direction.x * panSpeed, direction.y * panSpeed, 0f), Space.Self);
    }
}
