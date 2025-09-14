using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonTools
{
    public class Notch : MonoBehaviour
    {
        public float yOffset;

        private void Awake()
        {
            if ((float)Screen.width / (float)Screen.height < 0.5f)
            {
                Vector3 position = gameObject.transform.localPosition;
                position.y += yOffset;
                gameObject.transform.localPosition = position;
            }

        }
    }
}