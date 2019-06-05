using Gamekit2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OneShotLaserBeam : MonoBehaviour
{
    public float speed = 5;

    private AlessiaController m_Player;
    private ZombieKnightAIController m_ZombileKnightAI;
    private bool m_RestartScene = false;

    private void Awake()
    {
        m_Player = FindObjectOfType<AlessiaController>();
        m_ZombileKnightAI = FindObjectOfType<ZombieKnightAIController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_ZombileKnightAI.transform.position.x - transform.position.x >= 20)
        {
            this.transform.position = this.transform.position + Vector3.right * speed * Time.deltaTime;
        }
        else
        {
            this.transform.position = this.transform.position - Vector3.right * speed * Time.deltaTime;
        }

        if(this.transform.position.x > m_Player.transform.position.x && !m_RestartScene)
        {
            ResetGame();
        }
    }

    public void ResetGame()
    {
        m_Player.ForceDieResetGame();
        m_RestartScene = true;
    }
}
