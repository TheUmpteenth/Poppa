///---------------------------------------------------------------------------------------------------------------------
/// Copyright Davie Farrell - 2021
///---------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Token : MonoBehaviour, ILaunchable, IAttachable, IMatchable
{
    private enum TokenState
    {
        Locked,
        Loaded,
        Moving,
        ResolvingCollisions,
        Attaching,
        Attached,
        Matched,
    }

    private static readonly Vector2 NegateX = new Vector2(-1f, 1f);
    
    public event Action OnLaunchableBecomesInactive;
    public event Action<ILaunchable> OnLaunchableDestroyed;
    
    [SerializeField] private float m_baseSpeed = 1000f;
    [SerializeField] private TokenState m_state = TokenState.Loaded;
    
    public GameObject View => gameObject;
    public Collider2D Collider => m_collider;
    public List<IAttachable> AttachedItems { get; set; }
    public Vector2 Velocity { get; set; }
    public float Speed { get; set; }
    public MatchDefs.MatchType Type { get; set; }
    public Color Colour 
    { 
        get => m_color;
        set
        {
            m_color = value;
            View.GetComponent<Image>().color = m_color;
        } 
    }

    private Color m_color;
    
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
        Type = MatchDefs.MatchType.None;
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
        switch (m_state)
        {
            case TokenState.Moving:
            {
                UpdateMovement();
                break;
            }
            case TokenState.ResolvingCollisions:
            {
                m_state = TokenState.Attaching;
                break;
            }
            case TokenState.Attaching:
            {
                ResolveAttachments();
                break;
            }
            case TokenState.Matched:
            {
                OnLaunchableDestroyed?.Invoke(this);
                Destroy(gameObject);
                break;
            }
            default:
                break;
        }
    }

    public void Launch(Vector2 velocity)
    {
        Velocity = velocity.normalized * Speed;
        transform.SetParent(m_canvas.transform, true);
        m_state = TokenState.Moving;
    }

    public void UpdateMovement()
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
    
    public void OnCollisionEnter2D(Collision2D collision)
    {
        var otherAttachable = collision.collider.gameObject.GetComponent<IAttachable>();
        switch (m_state)
        {
            case TokenState.Moving:
                if (otherAttachable != null)
                {
                    m_rectTransform.position = otherAttachable.GetAttachedPosition(collision);

                    m_state = TokenState.ResolvingCollisions;
                    otherAttachable.SetAttachment(this);
                    this.SetAttachment(otherAttachable);
                }
                break;
            case TokenState.Attaching:
            case TokenState.ResolvingCollisions:
                m_state = TokenState.ResolvingCollisions;
                if (otherAttachable != null)
                {
                    otherAttachable.SetAttachment(this);
                    this.SetAttachment(otherAttachable);
                }
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
            m_attachPosition.x = AttachmentDefs.k_xComponentAttachmentAngle;
        }
        else
        {
            m_attachPosition.x = -AttachmentDefs.k_xComponentAttachmentAngle;
        }

        if (contactPoint.normal.y > AttachmentDefs.k_yComponentAngleForMidAttach) //top, middle or bottom
        {
            m_attachPosition.y = AttachmentDefs.k_yComponentAttachmentAngle;
        }
        else if (contactPoint.normal.y < -AttachmentDefs.k_yComponentAngleForMidAttach) //top, middle or bottom
        {
            m_attachPosition.y = -AttachmentDefs.k_yComponentAttachmentAngle;
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

    public void BeginAttaching(IAttachable other)
    {
        m_rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void SetAttachment(IAttachable other)
    {
        if (!AttachedItems.Contains(other))
        {
            AttachedItems.Add(other);
        }
    }

    public void ResolveAttachments()
    {
        OnLaunchableBecomesInactive?.Invoke();
        m_state = TokenState.Attached;
        if (Type == MatchDefs.MatchType.Bomb)
        {
            foreach (var attachable in AttachedItems)
            {
                if (attachable is IMatchable matchable)
                {
                    matchable.SetMatchComplete();
                    SetMatchComplete();
                }
            }
        }
        else
        {
            var matchedGroup = new List<IMatchable>();
            if (TryGetMatchedGroup(ref matchedGroup))
            {
                if (matchedGroup.Count >= MatchDefs.k_minimumGroupSize)
                {
                    foreach (var matchable in matchedGroup)
                    {
                        matchable.SetMatchComplete();
                    }
                }
            }
        }
    }

    public bool TryGetMatchedGroup(ref List<IMatchable> matchedGroup)
    {
        var matchFound = false;
        foreach (var attachable in AttachedItems)
        {
            if (attachable is IMatchable matchable)
            {
                if (!matchedGroup.Contains(matchable))
                {
                    if (matchable.IsMatched(Type))
                    {
                        matchFound = true;
                        if (matchedGroup.Count == 0)
                        {
                            matchedGroup.Add(this);
                        }
                        matchedGroup.Add(matchable);
                        matchable.TryGetMatchedGroup(ref matchedGroup);
                    }
                }
            }
        }

        return matchFound;
    }

    public bool IsMatched(MatchDefs.MatchType type)
    {
        return type == Type || type == MatchDefs.MatchType.Omni || Type == MatchDefs.MatchType.Omni;
    }

    public void SetMatchType(MatchDefs.MatchType type)
    {
        Type = type;
        Colour = MatchDefs.ColorFromType(type);
    }

    public void SetMatchComplete()
    {
        m_state = TokenState.Matched;
    }
}
