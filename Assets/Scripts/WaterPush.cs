using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPush : MonoBehaviour
{
    public ParticleSystem particles;
    public ParticleSystem impactParticles;
    public ParticleSystem impactParticles2;

    public float pushForce;
    public float WaterDamage;

    private List<ParticleCollisionEvent> particlesCollisionEvents = new List<ParticleCollisionEvent>();

    // these lists are used to contain the particles which match
    // the trigger conditions each frame.
    private List<ParticleSystem.Particle> enterTriggerParticles = new List<ParticleSystem.Particle>();


    public void OnParticleCollision(GameObject other)
    {
        int colCount = particles.GetSafeCollisionEventSize();

        if (colCount > particlesCollisionEvents.Count)
            particlesCollisionEvents = new List<ParticleCollisionEvent>();

        int eventCount = particles.GetCollisionEvents(other, particlesCollisionEvents);

        for (int i = 0; i < eventCount; i++)
        {
            //Make the push back vector
            Vector3 impactPos = particlesCollisionEvents[i].intersection;
            float impactDirectionX = other.transform.position.x - impactPos.x;
            float impactDirectionZ = other.transform.position.z - impactPos.z;
            Vector3 impactDirection = new Vector3(impactDirectionX, 0f, impactDirectionZ).normalized;

            //Push back the object
            if (other.GetComponent<Rigidbody>() && (other.tag == "Movable" || other.tag == "Enemy" || other.tag == "Player"))
            {
                other.GetComponent<Rigidbody>().AddForce(impactDirection * pushForce, ForceMode.Impulse);
            }

            if (other.tag == "Waterizable" || other.tag == "Movable" || other.tag == "Enemy" || other.tag == "Player")
            {
                //Spawn impact particles
                Instantiate(impactParticles, particlesCollisionEvents[i].intersection, Quaternion.identity);
                Instantiate(impactParticles2, particlesCollisionEvents[i].intersection, Quaternion.identity);

            }


            if (other.tag == "Enemy")
            {
                AudioSource tempAudioSource = other.GetComponent<AudioSource>();
                print("initiate drown");
                if(!tempAudioSource.isPlaying)
                {
                    other.GetComponent<PlaySoundDrowning>().PlaySounds();
                }

                other.GetComponent<Enemy>().TakeDamage(WaterDamage); //Can be different for each shoot style, each player, even modified on runtime
                other.GetComponent<Enemy>().HitAnimationLaunch();
            }
        }
    }

}
