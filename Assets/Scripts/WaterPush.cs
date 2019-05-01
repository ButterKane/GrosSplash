using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPush : MonoBehaviour
{
    public ParticleSystem particles;
    public ParticleSystem impactParticles;
    public ParticleSystem impactParticles2;

    public float pushForce;

    private List<ParticleCollisionEvent> particlesCollisionEvents;

    // Start is called before the first frame update
    void Start()
    {
        particlesCollisionEvents = new List<ParticleCollisionEvent>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnParticleCollision(GameObject other)
    {
        print("Une particle touche");
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
            if (other.GetComponent<Rigidbody>() && other.tag == "Movable")
            {
                other.GetComponent<Rigidbody>().AddForce(impactDirection * pushForce, ForceMode.Impulse);

            }

            if (other.tag == "Waterizable" || other.tag == "Movable")
            {
                //Spawn impact particles
                Instantiate(impactParticles, particlesCollisionEvents[i].intersection, Quaternion.identity);
                Instantiate(impactParticles2, particlesCollisionEvents[i].intersection, Quaternion.identity);
            }
        }
    }
}
