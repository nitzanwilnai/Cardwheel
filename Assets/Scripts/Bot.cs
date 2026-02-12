using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cardwheel
{
    public class Bot : MonoBehaviour
    {
        public RectTransform CanvasRect;   // Root canvas RectTransform
        public RectTransform ClickGO;    // The UI Image you want to move
        public Camera Camera;            // Needed only for Screen Space - Camera

        public bool DoClick = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (DoClick)
            {
                Vector2 screenPosition = new Vector2(Random.value * (float)Screen.width, Random.value * (float)Screen.height);
                ClickAtPosition(screenPosition);
            }
        }

        public void ClickAtPosition(Vector2 screenPosition)
        {
            Debug.Log("ClickAtPosition(" + screenPosition.ToString() + ")");
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = screenPosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0)
            {
                GameObject hit = results[0].gameObject;

                Button button = hit.GetComponentInChildren<Button>();

                if (button != null)
                {
                    Debug.Log("Clicked on " + hit.name);

                    bool notSettings = !hit.name.Contains("Settings");
                    if (notSettings)
                    {
                        ExecuteEvents.Execute(hit, pointerData, ExecuteEvents.pointerClickHandler);


                        // DoClick = false;
                    }
                    MoveToScreenPosition(screenPosition);
                }
            }
        }

        public void MoveToScreenPosition(Vector2 screenPosition)
        {
            Vector2 localPoint;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                CanvasRect,
                screenPosition,
                Camera,
                out localPoint
            );

            ClickGO.anchoredPosition = localPoint;
        }

    }
}