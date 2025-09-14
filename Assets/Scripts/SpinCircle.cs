using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardwheel
{
    public class SpinCircle : MonoBehaviour
    {
        public float Angle = 0.0f;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, Angle);
        }
    }
}