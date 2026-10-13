using UnityEngine;

public class EffectAttach : MonoBehaviour
{
    [SerializeField]
    [Tooltip("tana EF")]
    private ParticleSystem particle;

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            ParticleSystem newParticle = Instantiate(particle);
            newParticle.transform.position = transform.position;
            newParticle.Play();
        }
    }
}
