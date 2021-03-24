using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Basic")]
    [SerializeField] Vector3 movement;
    [SerializeField] Transform grounded;
    [SerializeField] GameObject playerCamera; 

    [Header("Custom")]
    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float jumping;



    private void Update()
    {
        float MoveX = Input.GetAxisRaw("Horizontal");
        float MoveZ = Input.GetAxisRaw("Vertical");

        movement = new Vector3(MoveX, 0, MoveZ);
        movement.Normalize();

        gameObject.GetComponent<Transform>().position = movement;
    }
}
