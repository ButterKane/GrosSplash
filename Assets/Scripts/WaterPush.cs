using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPush : MonoBehaviour
{
    public ParticleSystem particles;
    public ParticleSystem impactParticles;
    public ParticleSystem impactParticles2;
    public Transform[] triggerColliders;    //Needs to be initialized with all the trigger of level

    public float pushForce;

    private List<ParticleCollisionEvent> particlesCollisionEvents;

    // these lists are used to contain the particles which match
    // the trigger conditions each frame.
    private List<ParticleSystem.Particle> enterTriggerParticles = new List<ParticleSystem.Particle>();

    // Start is called before the first frame update
    void Start()
    {
        particlesCollisionEvents = new List<ParticleCollisionEvent>();

        for (int i = 0; i < triggerColliders.Length; i++)
        {
            particles.trigger.SetCollider(i, triggerColliders[i]); //Adds the referenced trigger to the list of trigger to check
        }
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

            if (other.tag == "Debug")
            {
                print("trigger enter");
            }
        }
    }

    public void OnParticleTrigger()
    {
        // get the particles which matched the trigger conditions this frame
        int numEnter = particles.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterTriggerParticles);

        // iterate through the particles which entered the trigger
        for (int i = 0; i < numEnter; i++)
        {
            // MAKE HERE WHAT YOU WANT WHEN ENTERING TRIGGER
        }
    }

}
