using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteScaler : MonoBehaviour
{
    int m_oldWidth;
    int m_oldHeight;

    private void Awake()
    {
        m_oldWidth = Screen.width;
        m_oldHeight = Screen.height;

    }

    // Start is called before the first frame update
    void Start()
    {
        ScaleScreen();
    }

    // Update is called once per frame
    void Update()
    {
        if (Screen.width != m_oldWidth || Screen.height != m_oldHeight)
        {
            ScaleScreen();
        }
    }

    void ScaleScreen()
    {
        float width = Screen.width;
        float height = Screen.height;
        float ratio = width / height;

        // Debug.Log("m_oldWidth " + m_oldWidth + " Screen.width " + Screen.width + " ratio " + ratio);

        if (ratio < (9.0f / 16.0f))
        {
            float defaultWidth = 1080.0f;
            float newWidth = ratio * 1920.0f;

            transform.localScale = Vector3.one * (newWidth / defaultWidth);

            // Debug.Log("defaultWidth " + defaultWidth + " newWidth " + newWidth + " transform.localScale.x " + transform.localScale.x);
        }
        else
            transform.localScale = Vector3.one;

        m_oldWidth = Screen.width;
    }
}
