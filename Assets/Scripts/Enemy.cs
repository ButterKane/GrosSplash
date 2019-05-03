using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int[] focusedTilesID;
    public float fleeSpeed = 10;
    public float normalSpeed = 5;
    public float fleeDistance = 5;
    public float attackCooldown = 5;
    public float maxHealth;
    public float actualHealth;

    private bool isFree;

    public float fireStrength;

    private Tile focusedTile;

    public NavMeshAgent navMesh;
    private float actualAttackCD;

    public Animator animator;

    

    private void Awake()
    {
        actualAttackCD = 0;
        actualHealth = maxHealth;
        isFree = true;
        navMesh.enabled = false;
        StartCoroutine(EnableNavMeshAgent(1f));
    }

    private void Update()
    {
        if (transform.position.y < -500)
        {
            TakeDamage(10000);
        }

        if (isFree && navMesh.enabled)
        {
            if (navMesh.destination != transform.position)
            {
                animator.SetBool("Running", true);
            }
            else
            {
                animator.SetBool("Running", false);
            }

            //Updates the cooldown
            if (actualAttackCD > 0)
            {
                actualAttackCD -= Time.deltaTime;
                actualAttackCD = Mathf.Clamp(actualAttackCD, 0, Mathf.Infinity);
            }

            //Get a random tile to burn
            if ((focusedTile == null || focusedTile.fireValue > 0) && actualAttackCD <= 0)
            {
                focusedTile = GameManager.i.gridManager.GetRandomExtinguishedTile(focusedTilesID[Random.Range(0, focusedTilesID.Length - 1)]);
                if (focusedTile == null) { return; }
                navMesh.SetDestination(focusedTile.transform.position);
                NavMeshPath path = new NavMeshPath();
                navMesh.CalculatePath(focusedTile.transform.position, path);
                if (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid)
                {
                    focusedTile = null;
                }
            }

            //Burn the tile
            if (focusedTile != null)
            {
                if (Vector3.Distance(transform.position, focusedTile.transform.position) < 2 && actualAttackCD <= 0)
                {
                    navMesh.SetDestination(transform.position);
                    focusedTile.fireValue = fireStrength;
                    focusedTile.Ignite();
                    animator.SetTrigger("Throwing");
                    focusedTile = null;
                    actualAttackCD = attackCooldown;
                    isFree = false;
                }
            }
            else
            {
                FleeFromPlayers();
            }
        }
        else // reinitialize
        {
            animator.SetBool("Running", false);
            focusedTile = null;
            actualAttackCD = attackCooldown;
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Throwing") || !animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            {
                isFree = true;
            }
        }
    }

    private void FleeFromPlayers()
    {
        foreach (PlayerMovement player in FindObjectsOfType<PlayerMovement>())
        {
            if (Vector3.Distance(player.transform.position, transform.position) < fleeDistance)
            {
                Vector3 runTo = transform.position + ((transform.position - player.transform.position) * 10);
                navMesh.SetDestination(runTo);
                navMesh.speed = fleeSpeed;
            } else
            {
                navMesh.speed = normalSpeed;
            }
        }
    }


    public IEnumerator WaitForAnimationToEnd(float damage)
    {
        isFree = false;
        yield return new WaitForSeconds(0.2f);
        isFree = true;
    }

    public IEnumerator EnableNavMeshAgent(float time)
    {
        yield return new WaitForSeconds(time);
        navMesh.enabled = true;
    }

     public void TakeDamage(float damage)
    {
        actualHealth -= damage;
        if (actualHealth <= 0)
        {
            GameManager.i.EnemiesKilled++;
            Destroy(this.gameObject);
        }
    }
    public void HitAnimationLaunch()
    {
        animator.SetTrigger("GettingHit");
    }
}
