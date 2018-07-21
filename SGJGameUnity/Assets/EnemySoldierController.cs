﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoldierController : SoldierController
{
    public float farDistance = 20.0f;
    public float mediumDistance = 10.0f;
    public float nearDistance = 3.0f;
    public float shootTimeMin = 0.5f;
    public float shootTimeMax = 2.5f;
    public float tangentMoveProbability = 0.4f;
    public float tangentMoveMin = 8.0f;
    public float tangentMoveMax = 12.0f;
    public float fineMoveMin = 3.0f;
    public float fineMoveMax = 8.0f;
    public float targetAchievedTreshold = 1.0f;
    public float stuckDistance = 1.0f;
    public float aimMissAngle = 20.0f;
    public float facingTreshold = 20.0f;

    private float logicDelay = 0.1f;

    private Coroutine logicCoroutine;

    GameObject player;
    bool walkToTarget = false;
    Vector3 walkTarget;

    void walkToSetup(Vector3 target)
    {
        walkToTarget = true;
        walkTarget = target;
        var targetDir = target - transform.position;
        if (targetDir.magnitude > 0.01f)
            targetDirection = targetDir.normalized;
    }

    /**
     * Enemy logic:
     *  - If distance is above farDistance, go to mediumDistance.
     *  - If distance is below nearDistance, go to mediumDistance.
     *  - Shoot player for random(shootTimeMin, shootTimeMax).
     *  - Move tangentially by random(tangentMoveMin, tangentMoveMax) with tangentMoveProbability.
     *  - Move by random(fineMoveMin, fineMoveMax).
     */
    IEnumerator logic()
    {
        yield return new WaitForSeconds(logicDelay);    // We need to wait until Weapons.instance will be ready.

        player = GameObject.FindGameObjectWithTag("Player");
        walkTarget = transform.position;
        while (true)
        {
            walkToSetup(transform.position);

            {
                var vecToPlayer = (player.transform.position - transform.position);
                var distToPlayer = vecToPlayer.magnitude;
                var mediumPoint = player.transform.position - vecToPlayer.normalized * mediumDistance;
                if ((distToPlayer > farDistance) || (distToPlayer < nearDistance))
                {
                    //Debug.Log("Moving to medium distance");
                    walkToSetup(mediumPoint);
                    yield return new WaitForSeconds(logicDelay);
                }
            }

            if (player.GetComponent<PlayerHitable>().health > 0)
            {
                walkToTarget = false;
                var shootingUntil = Time.time + Random.Range(shootTimeMin, shootTimeMax);
                while (Time.time < shootingUntil)
                {
                    //Debug.Log("Shooting");
                    var vecToPlayer = (player.transform.position - transform.position);
                    var dirToPlayer = vecToPlayer.normalized;

                    var aimCorrection = Random.Range(-aimMissAngle / 2, aimMissAngle / 2);
                    var correctedDir = Quaternion.Euler(0, 0, aimCorrection) * dirToPlayer;

                    targetDirection = correctedDir;
                    if (Vector3.Angle(targetDirection, currentDirection) < facingTreshold)
                    {
                        ShootWeapon();
                    }
                    yield return new WaitForSeconds(logicDelay);
                }
            }

            {
                var vecToPlayer = (player.transform.position - transform.position);
                var dirToPlayer = vecToPlayer.normalized;

                var moveDir = Vector3.zero;
                if (Random.value <= tangentMoveProbability)
                {
                    //Debug.Log("Tangent move");
                    var tangentMove = Random.Range(tangentMoveMin, tangentMoveMax);
                    if (Random.value < 0.5f)
                        tangentMove *= -1;

                    var tangentDir = Quaternion.Euler(0, 0, 90) * dirToPlayer;
                    moveDir = transform.position + tangentDir * tangentMove;
                }
                else
                {
                    //Debug.Log("Fine move");
                }

                moveDir += Weapons.randomDirection() * Random.Range(fineMoveMin, fineMoveMax);
                var moveTarget = transform.position + moveDir;
                var targetVec = moveTarget - player.transform.position;
                var mixedDist = (farDistance + mediumDistance) / 2;
                if (targetVec.magnitude > mixedDist)
                {
                    targetVec = targetVec.normalized * mixedDist;
                    moveTarget = player.transform.position + targetVec;
                }
                walkToSetup(moveTarget);
            }

            while (true)
            {
                var vecToTarget = walkTarget - transform.position;
                if (vecToTarget.magnitude < targetAchievedTreshold)
                {
                    walkToTarget = false;
                    //Debug.Log("Target achieved.");
                    break;
                }
                yield return new WaitForSeconds(logicDelay);
                var vecToTarget2 = walkTarget - transform.position;

                if (Mathf.Abs(vecToTarget.magnitude - vecToTarget2.magnitude) < stuckDistance)
                {
                    walkToTarget = false;
                    //Debug.Log("Stuck.");
                    break;
                }
            }
        }
    }

    new void Awake()
    {
        base.Awake();
    }

    // Use this for initialization
    new void Start()
    {
        base.Start();
    }

    void OnEnable()
    {
        logicCoroutine = StartCoroutine(logic());
    }

    void OnDisable()
    {
        StopCoroutine(logicCoroutine);
        logicCoroutine = null;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnims();
        Debug.DrawLine(transform.position, walkTarget, Color.yellow);
    }

    // Physics in FixedUpdate
    void FixedUpdate()
    {
        FixedLerpDirection();

        var move = Vector3.zero;

        if (walkToTarget)
        {
            move = walkTarget - transform.position;
            if (move.magnitude > 0.01f)
            {
                move = move.normalized * moveMultiplier;
            }

            FixedMove(move);
        }

        FixedMove(move);
    }
}