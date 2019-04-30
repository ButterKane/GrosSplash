using System.Collections;
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

    [Space(2)]
    [Header("General settings")]
    public int inputIndex;
    public Color playerColor;
    public float playerHeight = 2.2f;
    public int MaxHP;

    [Space(2)]
    [Header("Movement settings")]
    public MoveState moveState;
    public AnimationCurve accelerationCurve;


    [Tooltip("Minimum required speed to go to walking state")] public float minWalkSpeed = 0.1f;
    public float maxSpeedMin = 9;
    public float maxSpeedMax = 11;
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


    private void Awake()
    {
        customGravity = onGroundGravityMultiplyer;
        customDrag = idleDrag;
        currentHP = MaxHP;
        maxSpeed = maxSpeedMin;
        actualParticleSystem = lowIntensityParticles;
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
        //UpdateAnimatorBlendTree();
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
        Vector3 _inputX = Input.GetAxisRaw("LStickX") * cam.transform.right;
        Vector3 _inputZ = Input.GetAxisRaw("LStickY") * cam.transform.forward;
        input = _inputX - _inputZ;
        input.y = 0;
        input = input.normalized * ((input.magnitude - deadzone) / (1 - deadzone));
        Debug.DrawLine(transform.position, transform.position + input * 10);

        Vector3 _aimX = Input.GetAxisRaw("RStickX") * cam.transform.right;
        Vector3 _aimZ = Input.GetAxisRaw("RStickY") * cam.transform.forward;
        aim = _aimX + _aimZ;
        aim.y = 0;
        aim = aim.normalized;

        rTrigger = Input.GetAxis("RTrigger");

        if (Input.GetButtonDown("SwitchWeapon"))
        {
            SwitchWeapon();
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
        if (moveState == MoveState.Blocked) { return; }

        else if (body.velocity.magnitude <= minWalkSpeed)
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
        playerAnim.SetFloat("IdleRunningBlend", speed / maxSpeed);
    }

    void Rotate() //Rotate according to Aim Input
    {
        if (aim.magnitude >= 0.1f)
            turnRotation = Quaternion.Euler(0, Mathf.Atan2(aim.x, -aim.z) * 180 / Mathf.PI, 0);

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

    void Shoot()
    {
        if (rTrigger > 0.2f)
        {
            actualParticleSystem.Play();
        }
        else
        {
            actualParticleSystem.Stop();
        }
    }

    void SwitchWeapon()
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
    }

    #endregion
}
