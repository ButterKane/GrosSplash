using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public float moveSpeed;
    public float debugMoveAngle;
    public Vector3 debugMoveDirection;
    public Transform CharacterModel;
    public GameObject debugEmitter;
    public ParticleSystem particleEmitter;

    // Start is called before the first frame update
    void Start()
    {
        particleEmitter = debugEmitter.GetComponent<ParticleSystem>();

    }

    // Update is called once per frame
    void Update()
    {
        //INPUT GESTION ///////////////////////////////////////////////////////////////////////////////////////
        //Left stick (move the player) ==========================================================
        float lStickX = Input.GetAxis("LStickX");
        float lStickY = Input.GetAxis("LStickY");
        float lStickStrengh = Vector2.Distance(new Vector2(lStickX, lStickY), Vector2.zero); // How far you're pushing the joystick
        float lStickAngle = GetStickAngle(new Vector2(lStickX, lStickY),Vector2.zero); //The result is in radian, so we have to convert it to degrees

        //Right stick (rotates the player) ==========================================================
        float rStickX = Input.GetAxis("RStickX");
        float rStickY = Input.GetAxis("RStickY");
        float rStickStrengh = Vector2.Distance(new Vector2(rStickX, rStickY), Vector2.zero); // How far you're pushing the joystick
        float rStickAngle = GetStickAngle(new Vector2(rStickX, rStickY),Vector2.zero); //The result is in radian, so we have to convert it to degrees

        //Triggers ==========================================================
        // The values are between 0 (not pressed) and 1 (fully pressed)
        float LeftTrigger = Input.GetAxis("LTrigger");
        float RightTrigger = Input.GetAxis("RTrigger");



        //PRE-CALCULATIONS ===============================================
        Vector3 trueDirection = GetStickDirection(lStickAngle); // From the joystick angle, gives a direction relative to player
        Vector3 trueAim = GetStickDirection(rStickAngle);

        //MOVEMENT ===================================================
        Vector3 moveStick = new Vector3(trueDirection.x * moveSpeed * lStickStrengh * Time.deltaTime, 0f, trueDirection.z * moveSpeed * lStickStrengh * Time.deltaTime); //Get the values of the movement.

        Vector3 targetPosition = transform.position + new Vector3 (moveStick.x, moveStick.y, - moveStick.z); // where the player will go
        transform.position = targetPosition; // move the character

        //AIM ==============================================================
        Vector3 targetAim = transform.position + new Vector3(trueAim.x, trueAim.y, -trueAim.z).normalized;
        CharacterModel.LookAt(targetAim); //Orientate the character

        if (RightTrigger > 0.5)
        {
            particleEmitter.Play();

        }
        else
        {
            particleEmitter.Stop();
        }
    }


    private float GetStickAngle(Vector2 target, Vector2 origin)
    {
        return Mathf.Atan2(new Vector2(target.x, target.y).y - origin.y, new Vector2(target.x, target.y).x - origin.x) * (180 / Mathf.PI); //The result is in radian, so we have to convert it to degrees
    }

    private Vector3 GetStickDirection(float StickAngle)
    {
        return new Vector3(Mathf.Cos(Mathf.Deg2Rad * StickAngle), 0, Mathf.Sin(Mathf.Deg2Rad * StickAngle)); // From the joystick angle, gives a direction relative to player
    }
}
