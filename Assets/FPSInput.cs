using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Control Script/FPS Input")]

public class FPSInput : MonoBehaviour
{
    private CharacterController _charController;

    public float speed = 6.0f;
    public float runSpeed = 12.0f;
    public float gravity = -9.8f;
    public float jumpForce = 8.0f;
    private Vector3 moveDirection = Vector3.zero; // Aggiungi una variabile per la direzione del movimento

    void Start(){
        _charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : speed;

        float deltaX = Input.GetAxis("Horizontal") * currentSpeed;
        float deltaZ = Input.GetAxis("Vertical") * currentSpeed;
        Vector3 movement = new Vector3 (deltaX, 0, deltaZ);
        movement = Vector3.ClampMagnitude(movement, currentSpeed);

        // Se il personaggio è a terra e il tasto della barra spaziatrice è premuto, applica una forza di salto
        if (_charController.isGrounded && Input.GetButtonDown("Jump")) {
            moveDirection.y = jumpForce;
        } else {
            moveDirection.y += gravity * Time.deltaTime;
        }

        movement.y = moveDirection.y;

        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);
        _charController.Move(movement);
    }
}
