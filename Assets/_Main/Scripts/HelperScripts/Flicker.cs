using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

[RequireComponent(typeof(SpriteRenderer))]
public class Flicker : MonoBehaviour {

    public float duration;
    public float timeBetweenFlickering;

    private SpriteRenderer m_SpriteRenderer;
    private Color m_OriginalColor;
    private Coroutine m_Coroutine;


    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_OriginalColor = m_SpriteRenderer.color;
    }

    public void StartFlickering()
    {
        InternalFlickering(duration, timeBetweenFlickering);
    }

   
    public Coroutine StartFlickering(float duration, float timeBetweenFlickering)
    {
        return InternalFlickering(duration, timeBetweenFlickering);
    }

    private Coroutine InternalFlickering(float duration, float timeBetweenFlickering)
    {
        this.duration = duration;
        this.timeBetweenFlickering = timeBetweenFlickering;

        //stop previous corountine
        if (m_Coroutine != null)
        {
            m_SpriteRenderer.color = m_OriginalColor;
            StopFlickering();
        }

        //start new coroutine
        m_Coroutine = StartCoroutine(Flickering());

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

        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;
            sinceLastChange += Time.deltaTime;
            if (sinceLastChange > timeBetweenFlickering)
            {
                sinceLastChange -= timeBetweenFlickering;
                state = 1 - state;
                m_SpriteRenderer.color = state == 1 ? transparent : m_OriginalColor;
            }
        }

        m_SpriteRenderer.color = m_OriginalColor;
    }


}
