using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType
{
    SpeedUp = 0,
    SizeUp,
    MultiUp,
    PointsUp,
    HealthUp
}

public class PowerUp : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip m_CollectSound;

    [Header("PowerUp")]
    [SerializeField] private PowerUpType m_PowerUpType;
    [SerializeField] private int m_PointsOnCollect = 10;
    [SerializeField] private float m_TimePoweredUp = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Paddle paddle = collision.GetComponent<Paddle>();
        if(paddle != null)
        {
            switch (m_PowerUpType)
            {
                case PowerUpType.SpeedUp:
                    ScoreManager.m_Instance.AddPoints(m_PointsOnCollect);
                    GameManager.m_Instance.GivePowerUp(m_PowerUpType, m_TimePoweredUp);
                    break;
                case PowerUpType.SizeUp:
                    ScoreManager.m_Instance.AddPoints(m_PointsOnCollect);
                    GameManager.m_Instance.GivePowerUp(m_PowerUpType, m_TimePoweredUp);
                    break;
                case PowerUpType.MultiUp:
                    ScoreManager.m_Instance.AddPoints(m_PointsOnCollect);
                    if(GameManager.m_Instance.m_CurrentPaddle)
                    {
                        ObjectPooler.m_Instance.SetActiveFromPool("MultiBall", new Vector2(0, GameManager.m_Instance.m_CurrentPaddle.transform.position.x), Quaternion.identity);
                    }
                    break;
                case PowerUpType.HealthUp:
                    ScoreManager.m_Instance.AddPoints(m_PointsOnCollect);
                    paddle.ChangeHealth(1);
                    break;
                case PowerUpType.PointsUp:
                    ScoreManager.m_Instance.AddPoints(100 + m_PointsOnCollect);
                    break;
                default:
                    break;
            }

            AudioSource.PlayClipAtPoint(m_CollectSound, transform.position);
            gameObject.SetActive(false);
        }
    }
}
