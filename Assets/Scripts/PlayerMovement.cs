﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveState
{
    Idle,
    Walk,
    Blocked,
}

public class PlayerMovement : MonoBehaviour
{
    static public PlayerMovement instance;

    [Header("Components")]
    public Transform self;
    public Rigidbody body;
    public float deadzone = 0.2f;
    public Animator playerAnim;
    public Camera cam;
    public ParticleSystem highIntensityParticles;
    public ParticleSystem lowIntensityParticles;
    public AudioSource audioSource;

    [Space(2)]
    [Header("General settings")]
    public float playerHeight = 2.2f;
    public int MaxHP;

    [Space(2)]
    [Header("Water settings")]
    public float lowIntensityWaterForce = 1f;
    public float highIntensityWaterForce = 5f;

    public float lowIntensityRayLength = 5f;
    public float highIntensityRayLength = 5f;

    public int lowIntensityWaterRaycastCount = 6;
    public float lowIntensityWaterRaycastSpace = 1;
    [Space(2)]
    [Header("Multi settings")]
    public int playerIndex;
    public string axisMoveHorizontal, axisMoveVertical, axisAimHorizontal, axisAimVertical;
    public KeyCode weaponKey;

    [Space(2)]
    [Header("Movement settings")]
    public MoveState moveState;
    public AnimationCurve accelerationCurve;

    [Space(2)]
    [Header("Sounds settings")]
    public List<PlayerSoundsClass> soundsList;

    [Tooltip("Minimum required speed to go to walking state")] public float minWalkSpeed = 0.1f;
    public float maxSpeedIdle = 9;
    public float maxSpeedShooting;
    public float maxAcceleration = 10;

    [Space(2)]
    public float movingDrag = .4f;
    public float idleDrag = .4f;
    public float onGroundGravityMultiplyer;

    [Space(2)]
    [Range(0.01f, 1f)]
    public float turnSpeed = .25f;
    public AnimationCurve walkAnimationSpeedCurve;

    [Space(2)]
    [Header("Debug")]
    ParticleSystem actualParticleSystem;
    public int currentHP;
    Vector3 speedVector;
    float accelerationTimer;
    Vector3 lastVelocity;
    Vector3 input;
    Vector3 aim;
    Quaternion turnRotation;
    float distance;
    bool inputDisabled;
    float speed;
    float customDrag;
    float customGravity;
    float maxSpeed;
    bool isJumping;
    float rTrigger;
    bool shootAble = true;
    bool shooting;


    private void Awake()
    {
        customGravity = onGroundGravityMultiplyer;
        customDrag = idleDrag;
        currentHP = MaxHP;
        maxSpeed = maxSpeedIdle;
        actualParticleSystem = lowIntensityParticles;
        cam = Camera.main;
    }

    private void Start()
    {
        highIntensityParticles.Stop();
        lowIntensityParticles.Stop();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        GetInput();
        if (inputDisabled) { return; }
    }

    private void FixedUpdate()
    {
        CheckMoveState();
        Rotate();
        Shoot();
        if (input.magnitude != 0)
        {
            accelerationTimer += Time.fixedDeltaTime;
            Accelerate();
        }
        else
        {
            accelerationTimer = 0;
        }
        Move();
        ApplyDrag();
        ApplyCustomGravity();
        UpdateAnimatorBlendTree();
    }

    #region Input
    void GetInput()
    {
        if (inputDisabled) { input = Vector3.zero; return; }
        if (HasGamepad())
        {
            GamepadInput();
        }
        else
        {
            KeyboardInput();
        }
    }

    void GamepadInput()
    {
        //Vector3 _inputX = Input.GetAxisRaw("Horizontal_" + inputIndex.ToString()) * cam.transform.right;
        //Vector3 _inputZ = Input.GetAxisRaw("Vertical_" + inputIndex.ToString()) * cam.transform.forward;
        Vector3 _inputX = Input.GetAxisRaw(axisMoveHorizontal + playerIndex) * cam.transform.right;
        Vector3 _inputZ = Input.GetAxisRaw(axisMoveVertical + playerIndex) * cam.transform.forward;
        input = _inputX - _inputZ;
        input.y = 0;
        input = input.normalized * ((input.magnitude - deadzone) / (1 - deadzone));
        Debug.DrawLine(transform.position, transform.position + input * 10);

        Vector3 _aimX = Input.GetAxisRaw(axisAimHorizontal + playerIndex) * cam.transform.right;
        Vector3 _aimZ = Input.GetAxisRaw(axisAimVertical + playerIndex) * cam.transform.forward;
        aim = _aimX + _aimZ;
        aim.y = 0;
        aim = aim.normalized;

        if (Input.GetKey(weaponKey))
        {
            StartCoroutine(SwitchWeapon());
        }
    }

    void KeyboardInput()
    {
        //Vector3 _inputX = Input.GetAxisRaw("Horizontal_" + inputIndex.ToString()) * cam.transform.right;
        //Vector3 _inputZ = Input.GetAxisRaw("Vertical_" + inputIndex.ToString()) * cam.transform.forward;
        Vector3 _inputX = Input.GetAxisRaw("Horizontal") * cam.transform.right;
        Vector3 _inputZ = Input.GetAxisRaw("Vertical") * cam.transform.forward;
        input = _inputX + _inputZ;
        input.y = 0;
        input.Normalize();
        Debug.DrawLine(transform.position, transform.position + input * 10);
    }

    bool HasGamepad()
    {
        string[] names = Input.GetJoystickNames();
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i].Length > 0)
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region Movement

    void CheckMoveState()
    {
        //print("vitesse: " + body.velocity.magnitude);
        if (moveState == MoveState.Blocked) { return; }

        if (body.velocity.magnitude <= minWalkSpeed)
        {
            if (moveState != MoveState.Idle)
            {
                
                body.velocity = new Vector3(0, body.velocity.y, 0);
            }
            customDrag = idleDrag;
            moveState = MoveState.Idle;
            
        }
    }


    private void UpdateAnimatorBlendTree()
    {
     //   playerAnim.SetFloat("IdleRunningBlend", speed / maxSpeed);
    }

    void Rotate() //Rotate according to Aim Input
    {
        if (shooting)
        {
            if (aim.magnitude >= 0.1f)
                turnRotation = Quaternion.Euler(0, Mathf.Atan2(aim.x, -aim.z) * 180 / Mathf.PI, 0);
        }
        else
        {
            if (input.magnitude >= 0.1f)
                turnRotation = Quaternion.Euler(0, Mathf.Atan2(input.x, input.z) * 180 / Mathf.PI, 0);
        }

        self.rotation = Quaternion.Slerp(transform.rotation, turnRotation, turnSpeed);
    }

    void Accelerate()
    {
        if (moveState == MoveState.Blocked) { return; }
        body.AddForce(input * (accelerationCurve.Evaluate(body.velocity.magnitude / maxSpeed) * maxAcceleration), ForceMode.Acceleration);
        customDrag = movingDrag;
    }

    void Move()
    {
        if (moveState == MoveState.Blocked) { speed = 0; return; }
        Vector3 myVel = body.velocity;
        myVel.y = 0;
        myVel = Vector3.ClampMagnitude(myVel, maxSpeed);
        myVel.y = body.velocity.y;
        body.velocity = myVel;
        speed = body.velocity.magnitude;

        if (body.velocity.magnitude > minWalkSpeed)
        {
            playerAnim.SetBool("Running?", true);
            if (!audioSource.isPlaying)
            {
                audioSource.clip = soundsList[2].clip; //A faire mieux
                audioSource.Play();
            }
        }
        else
        {
            playerAnim.SetBool("Running?", false);
            if (audioSource.isPlaying && audioSource.clip == soundsList[2].clip)
            {
                audioSource.Stop();
            }
        }

    }

    void ApplyDrag()
    {
        Vector3 myVel = body.velocity;
        myVel.x *= 1 - customDrag;
        myVel.z *= 1 - customDrag;
        body.velocity = myVel;
    }

    void ApplyCustomGravity()
    {
        body.AddForce(new Vector3(0, -9.81f * customGravity, 0));
    }
    #endregion

    #region Actions

    void CheckWateredTiles()
    {
        if (actualParticleSystem == highIntensityParticles)
        {
            RaycastHit[] hits;
            hits = Physics.RaycastAll(highIntensityParticles.transform.position, highIntensityParticles.transform.forward, highIntensityRayLength);
            Debug.DrawRay(highIntensityParticles.transform.position, highIntensityParticles.transform.forward * highIntensityRayLength, Color.green);

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                Tile potentialTile;
                if (hit.transform.gameObject.GetComponentInChildren<Wall>() != null)
                {
                    potentialTile = hit.transform.GetComponent<Tile>();

                    if (potentialTile)
                    {
                        potentialTile.fireValue -= lowIntensityWaterForce;
                        potentialTile.UpdateFireScale();
                    }
                    return;
                }
                potentialTile = hit.transform.GetComponent<Tile>();

                if (potentialTile)
                {
                    potentialTile.fireValue -= highIntensityWaterForce;
                    potentialTile.UpdateFireScale();
                }
            }
        }
        else if (actualParticleSystem == lowIntensityParticles)
        {
            for (int i = 0; i < lowIntensityWaterRaycastCount; i++)
            {
                Vector3 endPosition = lowIntensityParticles.transform.forward * lowIntensityRayLength + lowIntensityParticles.transform.right * ((lowIntensityWaterRaycastCount / 2 - i));
                Debug.DrawRay(lowIntensityParticles.transform.position, endPosition, Color.green);
                RaycastHit[] hits;
                hits = Physics.RaycastAll(lowIntensityParticles.transform.position, endPosition, lowIntensityRayLength);
                for (int x = 0; x < hits.Length; x++)
                {
                    RaycastHit hit = hits[x];
                    Tile potentialTile;
                    if (hit.transform.gameObject.GetComponentInChildren<Wall>() != null)
                    {
                        potentialTile = hit.transform.GetComponent<Tile>();

                        if (potentialTile)
                        {
                            potentialTile.fireValue -= lowIntensityWaterForce;
                            potentialTile.UpdateFireScale();
                        }
                        return;
                    }
                    potentialTile = hit.transform.GetComponent<Tile>();

                    if (potentialTile)
                    {
                        potentialTile.fireValue -= lowIntensityWaterForce;
                        potentialTile.UpdateFireScale();
                    }
                }
            }
        }
    }

    void Shoot()
    {
        AudioSource actualWaterAudio = actualParticleSystem.GetComponent<AudioSource>();
        if (shootAble)
        {
            if (aim.magnitude>0.3f)
            {
                CheckWateredTiles();
                Debug.Log("Shooting");
                actualParticleSystem.Play();
                maxSpeed = maxSpeedShooting;
                shooting = true;
                
                if(!actualWaterAudio.isPlaying)
                { 
                    actualWaterAudio.Play();
                }
                //playerAnim.SetBool("Shooting", true);
            }
            else
            {
                actualParticleSystem.Stop();
                maxSpeed = maxSpeedIdle;
                shooting = false;

                actualWaterAudio.Stop();
                //playerAnim.SetBool("Shooting", false);
            }
        }
        else
        {
            actualParticleSystem.Stop();
            shooting = false;

            actualWaterAudio.Stop();
            //playerAnim.SetBool("Shooting", false);
        }
    }

    IEnumerator SwitchWeapon()
    {
        if (actualParticleSystem == highIntensityParticles)
        {
            actualParticleSystem.Stop();
            actualParticleSystem = lowIntensityParticles;
        }
        else
        {
            actualParticleSystem.Stop();
            actualParticleSystem = highIntensityParticles;
        }
        shootAble = false;
        yield return new WaitForSeconds(.5f);
        shootAble = true;
    }

    #endregion
}
