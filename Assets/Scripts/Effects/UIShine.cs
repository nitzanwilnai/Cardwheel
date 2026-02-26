using UnityEngine;

public class UIShine : MonoBehaviour
{
    public enum SHINE_STATE
    {
        START,
        SHINE,
        WAIT,
    };

    SHINE_STATE m_shineState = SHINE_STATE.WAIT;

    public float StartX;
    public float EndX;
    public float Velocity;
    public float StartTime;
    public float DelayTime;

    float m_startTimer;
    float m_delayTimer;
    float m_currentX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_currentX = StartX;
        m_delayTimer = 0.0f;
        m_shineState = SHINE_STATE.START;

        m_startTimer = 0.0f;

        transform.localPosition = new Vector3(m_currentX, 0.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_shineState == SHINE_STATE.START)
        {
            m_startTimer += Time.deltaTime;
            if(m_startTimer >= StartTime)
            m_shineState = SHINE_STATE.SHINE;
        }
        else if (m_shineState == SHINE_STATE.WAIT)
        {
            m_delayTimer += Time.deltaTime;
            if (m_delayTimer >= DelayTime)
            {
                m_shineState = SHINE_STATE.SHINE;
                m_currentX = StartX;
            }
        }
        else if (m_shineState == SHINE_STATE.SHINE)
        {
            m_currentX += Velocity * Time.deltaTime;
            if (m_currentX > EndX)
            {
                m_shineState = SHINE_STATE.WAIT;
                m_delayTimer = 0.0f;
            }
            transform.localPosition = new Vector3(m_currentX, 0.0f, 0.0f);
        }
    }
}
