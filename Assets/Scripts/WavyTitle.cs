using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavyTitle : MonoBehaviour
{
    public AnimationCurve TitleAnimCurve;
    public float AnimYMult = 1.0f;

    public GameObject[] Title;

    float m_time;

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        m_time += dt;
        if (m_time > 1.0f)
            m_time -= 1.0f;

        for (int i = 0; i < Title.Length; i++)
        {
            Vector3 pos = Title[i].transform.localPosition;
            float time = m_time + (i / (float)Title.Length);
            if (time > 1.0f)
                time -= 1.0f;
            pos.y = TitleAnimCurve.Evaluate(time) * AnimYMult;
            Title[i].transform.localPosition = pos;
        }
    }
}
