using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtTrigger : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip m_ExplosionSound;

    [Header("Damage")]
    [SerializeField] private int m_DamageToPaddle;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Ball ball = collision.GetComponent<Ball>();
        if(ball != null && ball.GetBallType() == BallType.Ball)
        {
            if(m_DamageToPaddle == 0)
            {
                Debug.LogError("Hey jij daar! De damage van de hurt trigger staat op 0!");
                return;
            }

            Paddle paddle = GameManager.m_Instance.m_CurrentPaddle.GetComponent<Paddle>();
            paddle.ChangeHealth(-m_DamageToPaddle);
            StartCoroutine(GameManager.m_Instance.SpawnBallWithDelay());
            
        }

        GameObject effect = ObjectPooler.m_Instance.SetActiveFromPool("BreakEffect", new Vector2(collision.transform.position.x, collision.transform.position.y + 0.4f), transform.rotation);
        ParticleSystem.MainModule settings = effect.GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient(Color.red);

        AudioSource.PlayClipAtPoint(m_ExplosionSound, transform.position);
        collision.gameObject.SetActive(false);
    }
}
