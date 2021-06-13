////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// Copyright Davie Farrell - 2021
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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

    private static readonly Vector2 NegateX = new Vector2(-1f, 1f);
    
    [SerializeField] private float m_baseSpeed = 1000f;
    [SerializeField] private TokenState m_state = TokenState.Loaded;
    
    public GameObject View => gameObject;
    public Collider Collider => m_collider;
    public List<IAttachable> AttachedItems { get; set; }
    public Vector2 Velocity { get; set; }
    public float Speed { get; set; }

    public Canvas ParentCanvas
    {
        set => m_canvas = value;
    }

    private Canvas m_canvas;
    private RectTransform m_rectTransform;
    private Vector2 m_bounds;
    private Collider m_collider;

    public void Awake()
    {
        AttachedItems = new List<IAttachable>();
        Velocity = Vector2.zero;
        Speed = m_baseSpeed;
        m_rectTransform = View.transform as RectTransform;
        m_collider = GetComponent<Collider>();
    }

    public void Start()
    {
        var halfWidth = (m_canvas.pixelRect.width * 0.5f) - (m_rectTransform.rect.width * 0.5f);
        m_bounds = new Vector2(halfWidth * -1f, halfWidth);
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

    public bool TryAttach(IAttachable other)
    {
        return false;
    }
}
