using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public float moveSpeed;
    public float debugMoveAngle;
    public Vector3 debugMoveDirection;
    public Transform CharacterModel;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //INPUT GESTION ///////////////////////////////////////////////////////////////////////////////////////
        //Left stick (move the player) ==========================================================
        float lStickX = Input.GetAxis("LStickX");
        float lStickY = Input.GetAxis("LStickY");
        float lStickStrengh = Vector2.Distance(new Vector2(lStickX, lStickY), Vector2.zero); // How far you're pushing the joystick

        float lStickAngle = Mathf.Atan2(new Vector2(lStickX, lStickY).y - Vector2.zero.y, new Vector2(lStickX, lStickY).x - Vector2.zero.x) * (180 / Mathf.PI); //The result is in radian, so we have to convert it to degrees

        Vector3 trueDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad * lStickAngle), 0, Mathf.Sin(Mathf.Deg2Rad * lStickAngle)); // From the joystick angle, gives a direction relative to player

        //Triggers ==========================================================
        // The values are between 0 (not pressed) and 1 (fully pressed)
        float LeftTrigger = Input.GetAxis("LTrigger");
        float RightTrigger = Input.GetAxis("RTrigger");


        //NORMAL MOVEMENT ===================================================
        Vector3 moveStick = new Vector3(trueDirection.x * moveSpeed * lStickStrengh * Time.deltaTime, 0f, trueDirection.z * moveSpeed * lStickStrengh * Time.deltaTime); //Get the values of the movement.

        debugMoveAngle = lStickAngle;
        debugMoveDirection = trueDirection;

      
        Vector3 targetPosition = transform.position + new Vector3 (moveStick.x, moveStick.y, - moveStick.z); // where the player will go

        CharacterModel.LookAt(targetPosition); //Orientate the character
        transform.position = targetPosition; // move the character
    }
}
