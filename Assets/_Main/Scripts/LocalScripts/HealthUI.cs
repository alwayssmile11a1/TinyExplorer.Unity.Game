using System.Collections;
using UnityEngine;
using Gamekit2D;
public class HealthUI : MonoBehaviour
{
    public Damageable representedDamageable;
    public GameObject healthIconPrefab;

    public Vector2 anchoredPosition = Vector2.zero;
    public float distanceBetweenIcon = 0.041f;

    protected Animator[] m_HealthIconAnimators;

    protected readonly int m_HashActivePara = Animator.StringToHash("Active");
    protected readonly int m_HashInactiveState = Animator.StringToHash("Inactive");

    IEnumerator Start()
    {
        if (representedDamageable == null)
            yield break;

        yield return null;

        m_HealthIconAnimators = new Animator[representedDamageable.startingHealth];

        for (int i = 0; i < representedDamageable.startingHealth; i++)
        {
            GameObject healthIcon = Instantiate(healthIconPrefab);
            healthIcon.transform.SetParent(transform);
            RectTransform healthIconRect = healthIcon.transform as RectTransform;
            healthIconRect.anchoredPosition = anchoredPosition;
            //healthIconRect.sizeDelta = Vector2.zero;
            healthIconRect.anchorMin += new Vector2(distanceBetweenIcon, 0f) * i;
            healthIconRect.anchorMax += new Vector2(distanceBetweenIcon, 0f) * i;
            m_HealthIconAnimators[i] = healthIcon.GetComponent<Animator>();

            if (representedDamageable.CurrentHealth < i + 1)
            {
                m_HealthIconAnimators[i].Play(m_HashInactiveState);
                m_HealthIconAnimators[i].SetBool(m_HashActivePara, false);
            }
        }
    }

    public void ChangeHitPointUI(Damageable damageable)
    {
        if (m_HealthIconAnimators == null)
            return;

        for (int i = 0; i < m_HealthIconAnimators.Length; i++)
        {
            m_HealthIconAnimators[i].SetBool(m_HashActivePara, damageable.CurrentHealth >= i + 1);
        }
    }

}