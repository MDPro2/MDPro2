using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ShowFPS : MonoBehaviour
{
    private float m_lastUpdateShowTime = 0f;
    private readonly float m_updateTime = 0.5f;
    private int m_frames = 0;
    private float m_FPS = 0;

    UILabel m_label;
    private void Start()
    {
        m_lastUpdateShowTime = Time.realtimeSinceStartup;
        m_label = GetComponent<UILabel>();
    }

    private void Update()
    {
        m_frames++;
        if (Time.realtimeSinceStartup - m_lastUpdateShowTime >= m_updateTime)
        {
            m_FPS = m_frames / (Time.realtimeSinceStartup - m_lastUpdateShowTime);
            m_lastUpdateShowTime = Time.realtimeSinceStartup;
            m_frames = 0;
            if (Program.I().setting.setting.fps.value)
                m_label.text = "FPS: " + (int)m_FPS;
            else
                m_label.text = "";
        }
    }
}