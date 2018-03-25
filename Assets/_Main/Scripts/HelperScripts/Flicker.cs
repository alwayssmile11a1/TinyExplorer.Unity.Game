using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

public class Flicker : MonoBehaviour {

    private SpriteRenderer m_SpriteRenderer;
    private Color m_OriginalColor;
    private float m_Duration;
    private float m_TimeBetweenFlickering;
    private Coroutine m_Coroutine;

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_OriginalColor = m_SpriteRenderer.color;
    }


    public Coroutine StartFlickering(float duration, float timeBetweenFlickering)
    {
        m_Duration = duration;
        m_TimeBetweenFlickering = timeBetweenFlickering;

        m_Coroutine =  StartCoroutine(Flickering());

        return m_Coroutine;
    }

    public void StopFlickering()
    {
        StopCoroutine(m_Coroutine);
    }

    private IEnumerator Flickering()
    {

        float timer = 0f;
        float sinceLastChange = 0.0f;

        Color transparent = m_OriginalColor;
        transparent.a = 0.2f;
        int state = 1;

        m_SpriteRenderer.color = transparent;

        while (timer < m_Duration)
        {
            yield return null;
            timer += Time.deltaTime;
            sinceLastChange += Time.deltaTime;
            if (sinceLastChange > m_TimeBetweenFlickering)
            {
                sinceLastChange -= m_TimeBetweenFlickering;
                state = 1 - state;
                m_SpriteRenderer.color = state == 1 ? transparent : m_OriginalColor;
            }
        }

        m_SpriteRenderer.color = m_OriginalColor;
    }


}
