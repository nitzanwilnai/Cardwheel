/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavyTitle : MonoBehaviour
{
    public AnimationCurve TitleAnimCurve;
    public float AnimYMult = 1.0f;

    public GameObject[] Title;
    RectTransform[] m_rectTransforms;

    float m_time;

    void Awake()
    {
        m_rectTransforms = new RectTransform[Title.Length];
        for (int i = 0; i < Title.Length; i++)
            m_rectTransforms[i] = Title[i].GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        m_time += dt;
        if (m_time > 1.0f)
            m_time -= 1.0f;

        for (int i = 0; i < Title.Length; i++)
        {
            float time = m_time + (i / (float)Title.Length);
            if (time > 1.0f)
                time -= 1.0f;
         
            // Vector3 pos = Title[i].transform.localPosition;
            // pos.y = TitleAnimCurve.Evaluate(time) * AnimYMult;
            // Title[i].transform.localPosition = pos;

            Vector2 anchoredPos = m_rectTransforms[i].anchoredPosition;
            anchoredPos.y = TitleAnimCurve.Evaluate(time) * AnimYMult;
            m_rectTransforms[i].anchoredPosition = anchoredPos;
        }
    }
}
