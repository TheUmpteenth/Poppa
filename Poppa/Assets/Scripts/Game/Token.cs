///---------------------------------------------------------------------------------------------------------------------
/// Copyright Davie Farrell - 2021
///---------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

public class Token : MonoBehaviour, ILaunchable, IAttachable
{
    private enum TokenState
    {
        Loaded,
        Launched,
        Attached,
        Locked
    }

    private const float k_midpointNormalOffset = 0.342045f;//20 degrees
    private static readonly Vector2 NegateX = new Vector2(-1f, 1f);
    
    public event Action OnLaunchableBecomesInactive;
    
    [SerializeField] private float m_baseSpeed = 1000f;
    [SerializeField] private TokenState m_state = TokenState.Loaded;
    
    public GameObject View => gameObject;
    public Collider2D Collider => m_collider;
    public List<IAttachable> AttachedItems { get; set; }
    public Vector2 Velocity { get; set; }
    public float Speed { get; set; }
    
    private Vector2 m_attachPosition = Vector2.zero;

    public Canvas ParentCanvas
    {
        set => m_canvas = value;
    }

    private Canvas m_canvas;
    private RectTransform m_rectTransform;
    private Vector2 m_bounds;
    private Collider2D m_collider;
    private Rigidbody2D m_rigidBody;

    public void Awake()
    {
        AttachedItems = new List<IAttachable>();
        Velocity = Vector2.zero;
        Speed = m_baseSpeed;
        m_rectTransform = View.transform as RectTransform;
        m_collider = GetComponent<Collider2D>();
        m_rigidBody = GetComponent<Rigidbody2D>();
    }

    public void Start()
    {
        if (m_state != TokenState.Locked)
        {
            var halfWidth = (m_canvas.pixelRect.width * 0.5f) - (m_rectTransform.rect.width * 0.5f);
            m_bounds = new Vector2(halfWidth * -1f, halfWidth);
        }

        if (m_state == TokenState.Locked)
        {
            m_rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    public void Update()
    {
        UpdateMovement();
    }

    public void Launch(Vector2 velocity)
    {
        Velocity = velocity.normalized * Speed;
        transform.SetParent(m_canvas.transform, true);
        m_state = TokenState.Launched;
    }
    
    public void UpdateMovement()
    {
        if (m_state == TokenState.Launched)
        {
            var position = m_rectTransform.anchoredPosition;
            position += Velocity * Time.deltaTime;
            
            if (position.x <= m_bounds.x)
            {
                var xOffset = position.x - m_bounds.x;
                position.x = m_bounds.x - xOffset;
                Velocity *= NegateX;
            }

            if (position.x >= m_bounds.y)
            {
                var xOffset = position.x - m_bounds.y;
                position.x = m_bounds.y - xOffset;
                Velocity *= NegateX;
            }
            
            m_rectTransform.anchoredPosition = position;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Collision Detected {m_state}");
        switch (m_state)
        {
            case TokenState.Launched:
                var otherAttachable = collision.collider.gameObject.GetComponent<IAttachable>();
                if (otherAttachable != null)
                {
                    m_rectTransform.position = otherAttachable.GetAttachedPosition(collision);

                    TryAttach(otherAttachable);
                }
                break;
            case TokenState.Attached:
                m_rigidBody.Sleep();
                var newAttachable = collision.collider.gameObject.GetComponent<IAttachable>();
                TryAttach(newAttachable);
                break;
            default:
                m_rigidBody.Sleep();
                break;
        }
    }

    public Vector2 GetAttachedPosition(Collision2D collision)
    {
        var contactPoint = collision.GetContact(0);

        if (contactPoint.normal.x > 0) //left or right
        {
            m_attachPosition.x = 0.5f;
        }
        else
        {
            m_attachPosition.x = -0.5f;
        }

        if (contactPoint.normal.y > k_midpointNormalOffset) //top, middle or bottom
        {
            m_attachPosition.y = 0.866f;
        }
        else if (contactPoint.normal.y < -k_midpointNormalOffset) //top, middle or bottom
        {
            m_attachPosition.y = -0.866f;
        }
        else
        {
            m_attachPosition.y = 0f;
        }

        m_attachPosition.Normalize();

        m_attachPosition *= ((CircleCollider2D) m_collider).radius * 2f;

        var retval = (Vector2)m_rectTransform.position + m_attachPosition;

        return retval;
    }

    public bool TryAttach(IAttachable other)
    {
        OnLaunchableBecomesInactive?.Invoke();
        m_state = TokenState.Attached;
        m_rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        AttachedItems.Add(other);
        return false;
    }
}
