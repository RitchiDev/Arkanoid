using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public enum BrickType
{
    Normal = 0,
    Silver,
    Gold,
    Unbreakable,
    Special
}


public class Brick : Entity
{
    private Animator m_Animator;
    private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private BrickType m_BrickType;
    [SerializeField] private List<Sprite> m_BrickSprites = new List<Sprite>();

    protected override void Awake()
    {
        base.Awake();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        if (m_BrickType != BrickType.Normal)
        {
            m_Animator = GetComponent<Animator>();
            m_Animator.SetFloat("Health", m_Health);
        }
    }

    protected override void Start()
    {
        base.Start();
        if(m_BrickType == BrickType.Normal)
        {
            m_SpriteRenderer.sprite = m_BrickSprites[Random.Range(0, m_BrickSprites.Count)];
        }
    }

    public void UpdateBrickSprite()
    {
        if (m_BrickType != BrickType.Normal)
        {
            m_Animator.SetFloat("Health", m_Health);
        }
    }
}
