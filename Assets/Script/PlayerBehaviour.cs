using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float angularSpeed;

    private Rigidbody rb;
    float hor, vert;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        hor = Input.GetAxis("Horizontal");
        vert = Input.GetAxis("Vertical");
        //float mouseX = Input.GetAxis("Mouse X");
        //transform.Rotate(transform.up, angularSpeed * mouseX);
        transform.Rotate(transform.up, angularSpeed * hor);
        Debug.DrawRay(transform.position, transform.forward * 20, Color.blue);
        Debug.DrawRay(transform.position, Vector3.forward * 20, Color.cyan);
    }

    private void FixedUpdate()
    {
        rb.AddForce(transform.forward * moveSpeed * vert);
    }
}
