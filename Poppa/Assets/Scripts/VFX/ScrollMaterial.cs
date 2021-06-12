////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// Copyright Davie Farrell - 2021
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;

public class ScrollMaterial : MonoBehaviour
{
    private readonly int k_texID = Shader.PropertyToID("_MainTex");
    
    [SerializeField] private float m_xSpeed;
    [SerializeField] private float m_ySpeed;
    [SerializeField] private Image m_image;

    private Vector2 m_offset;
    
    void Update()
    {
        m_offset.x = Time.time * m_xSpeed % 1f;
        m_offset.y = Time.time * m_ySpeed % 1f;
        
        m_image.material.SetTextureOffset(k_texID, m_offset);
    }

    private void OnValidate()
    {
        m_offset = Vector2.zero;
        Update();
    }
}
