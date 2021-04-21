using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "PaddleData")]
public class PaddleData : ScriptableObject
{
    [Header("Audio")]
    [SerializeField]
    public AudioClip m_StretchSound;
    public AudioClip m_ShrinkSound;
    public AudioClip m_BreakSound;

    [Header("UI")]
    public Sprite m_FullHeart;
    public Sprite m_EmptyHeart;

    [Header("Movement")]
    [Range(1f, 30f)]
    public float m_DefaultSpeed = 5f;
    [Range(1f, 30f)]
    public float m_PoweredUpSpeed = 5f;
    [Range(1f, 30f)]
    public float m_BaseMoveLimit = 5f;

    [Header("Collision")]
    public LayerMask m_WallLayer;
    public Vector2 m_ShortPaddleSize, m_LongPaddleSize;
    public Vector2 m_WallDetectSize;
    public Vector2 m_RightOffset, m_LeftOffset;
}
