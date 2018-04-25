using UnityEngine;
using Gamekit2D;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Portal : MonoBehaviour {

    public Portal linkedPortal;
    [Tooltip("Which layers can portal?")]
    public LayerMask layers;

    [Tooltip("Use this to modify the position of game object being portalled")]
    public Vector3 portalOffset;

    [System.Serializable]
    public class PortallingEvent : UnityEvent<Portal, Portal, Collider2D>
    {   }

    public PortallingEvent OnPortalling;

    protected bool m_ExitTrigger2D = true;

    private bool m_CanPortal = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(linkedPortal!=null && m_ExitTrigger2D && m_CanPortal && layers.Contains(collision.gameObject))
        {
            linkedPortal.m_ExitTrigger2D = false;

            //Rigidbody2D rb2D = collision.GetComponent<Rigidbody2D>();

            //if(rb2D!=null)
            //{
            //    rb2D.position = linkedPortal.GetComponent<Transform>().position + portalOffset;
            //}
            //else
            //{
                collision.transform.position = linkedPortal.GetComponent<Transform>().position + portalOffset;
            //}

            OnPortalling.Invoke(this, linkedPortal, collision);

        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (linkedPortal != null && m_CanPortal && layers.Contains(collision.gameObject))
        {
            m_ExitTrigger2D = true;
        }
    }

    public void AddListener(UnityAction<Portal, Portal, Collider2D> listener)
    {
        OnPortalling.AddListener(listener);

    }

    public void RemoveListener(UnityAction<Portal, Portal, Collider2D> listener)
    {
        OnPortalling.RemoveListener(listener);

    }

    public void RemoveAllListeners()
    {
        OnPortalling.RemoveAllListeners();
    }

    public void CanPortal(bool canPortal)
    {
        m_CanPortal = canPortal;
    }


}
