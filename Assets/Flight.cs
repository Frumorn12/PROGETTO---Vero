using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flight : MonoBehaviour
{
   
    public float flySpeed = 5f; // Velocità di volo
    public float gravity = 9.81f; // Gravità
    public KeyCode flyKey = KeyCode.Space; // Tasto per attivare il volo
    public bool isFlying = false; // Stato del volo

    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Controlla se il tasto per il volo è stato premuto
        if (Input.GetKeyDown(flyKey))
        {
            isFlying = !isFlying; // Inverti lo stato del volo
        }

        // Se il personaggio sta volando
        if (isFlying)
        {
            Fly(); // Chiama il metodo per gestire il volo
        }
        else
        {
            // Se non sta volando, applica la gravità
            ApplyGravity();
        }
    }

    private void Fly()
    {
        // Calcola il movimento verticale basato sull'input dell'asse verticale
        float verticalMovement = Input.GetAxis("Vertical") * flySpeed * Time.deltaTime;

        // Calcola il vettore di movimento
        Vector3 moveDirection = transform.up * verticalMovement;

        // Applica il movimento
        controller.Move(moveDirection);
    }

    private void ApplyGravity()
    {
        // Applica la gravità al CharacterController
        if (!controller.isGrounded)
        {
            Vector3 gravityVector = Vector3.down * gravity * Time.deltaTime;
            controller.Move(gravityVector);
        }
    }
}


