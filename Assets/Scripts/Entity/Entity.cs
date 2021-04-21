using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Entity : MonoBehaviour, IHealthChangeable
{
    [Header("Audio")]
    [SerializeField] private AudioClip m_BrickBreak;

    [Header("Components")]
    [SerializeField] protected PaddleData m_PaddleData;
    private Brick m_Brick;

    [Header("Health")]
    [SerializeField] protected float m_MaxHealth = 1;
    protected float m_Health;
    private bool m_IsInvincible;

    [Header("Score")]
    [SerializeField] private int m_PointsOnBreak = 100;

    protected virtual void Awake()
    {
        if(!gameObject.CompareTag("Paddle"))
        {
            m_Brick = GetComponent<Brick>();
        }
    }

    protected virtual void Start()
    {
        m_Health = m_MaxHealth;
    }

    public void ChangeHealth(float amount) //Als ik tijd heb Invince en Heal powerup :)
    {
        if (amount < 0)
        {
            if (m_IsInvincible)
            {
                return;
            }
        }

        m_Health = Mathf.Clamp(m_Health + amount, 0, m_MaxHealth);
        EntityUpdateSprite();

        if (m_Health <= 0 && gameObject.activeInHierarchy)
        {
            KillEntity();
        }
    }

    private void KillEntity()
    {
        GameObject effect = ObjectPooler.m_Instance.SetActiveFromPool("BreakEffect", new Vector2(transform.position.x, transform.position.y + 0.5f), transform.rotation);
        ParticleSystem.MainModule settings = effect.GetComponent<ParticleSystem>().main;

        if (!gameObject.CompareTag("Paddle"))
        {
            effect.transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 180);
            settings.startColor = new ParticleSystem.MinMaxGradient(Color.white);

            ScoreManager.m_Instance.AddPoints(m_PointsOnBreak);
            GameManager.m_Instance.RemoveBrick(gameObject);
            AudioSource.PlayClipAtPoint(m_BrickBreak, transform.position);

            SpawnPowerUp();
        }
        else
        {
            settings.startColor = new ParticleSystem.MinMaxGradient(Color.blue);

            ScoreManager.m_Instance.ResetScore();
            GameManager.m_Instance.RemovePaddle();
            AudioSource.PlayClipAtPoint(m_PaddleData.m_BreakSound, transform.position);
        }


        m_Health = m_MaxHealth;
        transform.localPosition = Vector2.zero;
        transform.rotation = quaternion.identity;
        gameObject.SetActive(false);
    }

    private void SpawnPowerUp()
    {
        int randomNumber = UnityEngine.Random.Range(0, 20);
        if (randomNumber > 12)
        {
            if (randomNumber == 18)
            {
                ObjectPooler.m_Instance.SetActiveFromPool("SpeedUp", transform.position, Quaternion.identity);
            }
            else if (randomNumber == 17)
            {
                ObjectPooler.m_Instance.SetActiveFromPool("SizeUp", transform.position, Quaternion.identity);
            }
            else if (randomNumber == 16)
            {
                ObjectPooler.m_Instance.SetActiveFromPool("MultiUp", transform.position, Quaternion.identity);
            }
            else if(randomNumber == 15 && GameManager.m_Instance.m_CurrentPaddle.GetComponent<Paddle>().GetIfHurt())
            {
                ObjectPooler.m_Instance.SetActiveFromPool("HealthUp", transform.position, Quaternion.identity);
            }
            else
            {
                ObjectPooler.m_Instance.SetActiveFromPool("PointsUp", transform.position, Quaternion.identity);
            }
        }
    }

    private void EntityUpdateSprite()
    {
        if(m_Brick != null)
        {
            m_Brick.UpdateBrickSprite();
        }
        else
        {
            GameManager.m_Instance.m_CurrentPaddle.GetComponent<Paddle>().UpdateHeartSprite();
        }
    }
}



//public IEnumerator GiveInvincibility(float timeInvincible) //Schild powerup
//{
//    m_IsInvincible = true;
//    yield return new WaitForSeconds(timeInvincible);
//    m_IsInvincible = false;
//}
//m_Paddle.UpdateHeartSprite();
//private float m_InvincibleTimer;
//if(gameObject.CompareTag("Enemy"))
//{
//    m_PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;
//}
//private GameObject SpawnEffect()
//{
//    GameObject effect = ObjectPooler.m_Instance.SetActiveFromPool("BreakEffect", transform.position, Quaternion.identity);
//    ParticleSystem.MainModule settings = effect.GetComponent<ParticleSystem>().main;
//    if(m_Brick != null)
//    {
//        if (m_Brick.GetBrickType() == BrickType.Gold)
//        {
//            settings.startColor = new ParticleSystem.MinMaxGradient(m_Gold);
//        }
//        else if(m_Brick.GetBrickType() == BrickType.Silver)
//        {
//            settings.startColor = new ParticleSystem.MinMaxGradient(m_Silver);

//        }
//        else if(m_Brick.GetBrickType() == BrickType.Normal)
//        {
//            settings.startColor = new ParticleSystem.MinMaxGradient(m_White);
//        }
//    }
//    else
//    {
//        settings.startColor = new ParticleSystem.MinMaxGradient(m_Paddle);
//    }

//    return effect;
//}
//[SerializeField] private Color m_Gold, m_Silver, m_White, m_Paddle;
