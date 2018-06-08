using System.Collections;
using UnityEngine;
using Gamekit2D;
using System.Collections.Generic;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Damageable representedDamageable;
    public GameObject healthIconPrefab;

    public Vector2 anchoredPosition = Vector2.zero;
    public float distanceBetweenIcon = 0.041f;

    protected List<Animator> m_HealthIconAnimators;

    protected readonly int m_HashActivePara = Animator.StringToHash("Active");
    protected readonly int m_HashInactiveState = Animator.StringToHash("Inactive");
    protected readonly int m_HashAppearingState = Animator.StringToHash("Appearing");


    

    IEnumerator Start()
    {
        if (representedDamageable == null)
            yield break;

        yield return null;

        m_HealthIconAnimators = new List<Animator>();

        for (int i = 0; i < representedDamageable.startingHealth; i++)
        {
            GameObject healthIcon = Instantiate(healthIconPrefab);
            healthIcon.transform.SetParent(transform);
            RectTransform healthIconRect = healthIcon.transform as RectTransform;
            healthIconRect.anchoredPosition = anchoredPosition;
            //healthIconRect.sizeDelta = Vector2.zero;
            healthIconRect.anchorMin += new Vector2(distanceBetweenIcon, 0f) * i;
            healthIconRect.anchorMax += new Vector2(distanceBetweenIcon, 0f) * i;
            m_HealthIconAnimators.Add(healthIcon.GetComponent<Animator>());

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

        for (int i = 0; i < m_HealthIconAnimators.Count; i++)
        {
            m_HealthIconAnimators[i].SetBool(m_HashActivePara, damageable.CurrentHealth >= i + 1);
            m_HealthIconAnimators[i].GetComponentsInChildren<Image>()[2].fillAmount = damageable.CurrentHealth >= i + 1 ? 1f : 0f;
        }
    }


    public void GainHitPointUI(int amount, Damageable damageable)
    {
        int currentTotalHitPoint = m_HealthIconAnimators.Count;

        for (int i = 0; i < amount; i++)
        {
            GameObject healthIcon = Instantiate(healthIconPrefab);
            healthIcon.transform.SetParent(transform);
            RectTransform healthIconRect = healthIcon.transform as RectTransform;
            healthIconRect.anchoredPosition = anchoredPosition;
            //healthIconRect.sizeDelta = Vector2.zero;
            healthIconRect.anchorMin += new Vector2(distanceBetweenIcon, 0f) * (i + currentTotalHitPoint);
            healthIconRect.anchorMax += new Vector2(distanceBetweenIcon, 0f) * (i + currentTotalHitPoint);
            m_HealthIconAnimators.Add(healthIcon.GetComponent<Animator>());
            m_HealthIconAnimators[i + currentTotalHitPoint].SetTrigger(m_HashAppearingState);
            //if (representedDamageable.CurrentHealth < i + currentTotalHitPoint + 1)
            //{
            //    m_HealthIconAnimators[i].Play(m_HashInactiveState);
            //    m_HealthIconAnimators[i].SetBool(m_HashActivePara, false);
            //}
        }
    }

    public void FillHitPointUI(float fillAmount)
    {
        if (representedDamageable.CurrentHealth <= 0 || representedDamageable.CurrentHealth == representedDamageable.startingHealth) return;

        Image healthIconImage = m_HealthIconAnimators[representedDamageable.CurrentHealth].GetComponentsInChildren<Image>()[2];

        healthIconImage.fillAmount += fillAmount;

        if(healthIconImage.fillAmount >=1)
        {
            representedDamageable.GainHealth(1);
        }


    }

}