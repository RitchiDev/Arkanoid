using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTimer : MonoBehaviour
{
    private ParticleSystem m_ParticleSystem;

    private void Awake()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        m_ParticleSystem.Play();
        StartCoroutine(DeactivateEffect(m_ParticleSystem.main.duration));
    }

    private IEnumerator DeactivateEffect(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
