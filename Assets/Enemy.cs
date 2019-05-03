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

    public float fireStrength;

    private Tile focusedTile;

    public NavMeshAgent navMesh;
    private float actualAttackCD;

    private void Awake()
    {
        actualAttackCD = 0;
    }

    private void Update()
    {
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
            navMesh.SetDestination(focusedTile.transform.position);
            NavMeshPath path = new NavMeshPath();
            navMesh.CalculatePath(focusedTile.transform.position, path);
            if (path.status == NavMeshPathStatus.PathPartial)
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
                focusedTile = null;
                actualAttackCD = attackCooldown;
            }
        } else
        {
            FleeFromPlayers();
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
}
