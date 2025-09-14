using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonTools
{
    public class NotchAutoPad : MonoBehaviour
    {
        public float Amount;

        private void Awake()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            if ((float)Screen.width / (float)Screen.height < 0.5f)
                rectTransform.sizeDelta = new Vector2(0.0f, Amount);
            else
                rectTransform.sizeDelta = Vector2.zero;

        }
    }
}