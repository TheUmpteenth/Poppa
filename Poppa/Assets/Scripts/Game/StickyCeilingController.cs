///---------------------------------------------------------------------------------------------------------------------
/// Copyright Davie Farrell - 2021
///---------------------------------------------------------------------------------------------------------------------

using UnityEngine;

public class StickyCeilingController : MonoBehaviour
{
    [SerializeField] private Canvas m_canvas;
    [SerializeField] private GameObject m_hiddenTokenPrefab;

    private float m_width;
    
    private void Start()
    {
        Vector3 pos = transform.position;
        m_width = m_canvas.pixelRect.width;
        pos.x = m_width * -0.5f;
        transform.position = pos;
        BuildFullCeiling();
    }

    private void BuildFullCeiling()
    {
        Vector2 spawnPos = Vector2.zero;
        spawnPos.y = transform.position.y;
        while (spawnPos.x < m_width)
        {
            Instantiate(m_hiddenTokenPrefab, spawnPos, Quaternion.identity, transform);
            spawnPos.x += AttachmentDefs.k_xDistBetweenAttachedTokens;
        }
    }
}
