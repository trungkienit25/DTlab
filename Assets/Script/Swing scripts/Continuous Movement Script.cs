using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ContinuousMovementScript : MonoBehaviour
{
    [Header("Character Control")]
    public float speed = 1;
    public float turnspeed = 60;
    private float jumpVelocity = 7;
    public float jumpHeight = 1.5f;
    public bool onlyMoveWhenGrounded = false;

    [Header("Input Action")]
    public InputActionProperty turnIntputSource;
    public InputActionProperty moveInputSource;
    public InputActionProperty jumpInputSource;
    public InputActionProperty rightSwingInputSource; //Control Right Swing
    public InputActionProperty leftSwingInputSource; //Control Left Swing
    [Header("Body Properties")]
    public Rigidbody rb;
    public LayerMask groundLayer;
    public Transform directionSource;
    public Transform turnSource;
    public CapsuleCollider bodyCollider;

    [Header("Swing Control")]
    public GameObject rightSwingObject;
    public GameObject leftSwingObject;


    private Vector2 InputMoveAxis;
    private float inputTurnAxis;
    private bool isGrounded;

    void Start()
    {
        if (rightSwingObject != null)
        {
            rightSwingObject.SetActive(false);
        }
        if (leftSwingObject != null)
        {
            leftSwingObject.SetActive(false);
        }
        rightSwingInputSource.action.performed += ctx => ToggleRightSwing();
        leftSwingInputSource.action.performed += ctx => ToggleLeftSwing();
    }
    void OnDestroy()
    {
        // Unsubscribe from the event when this object is destroyed to prevent memory leaks.
        rightSwingInputSource.action.performed -= ctx => ToggleRightSwing();
        leftSwingInputSource.action.performed -= ctx => ToggleLeftSwing();
    }

    void Update()
    {
        InputMoveAxis = moveInputSource.action.ReadValue<Vector2>();
        inputTurnAxis = turnIntputSource.action.ReadValue<Vector2>().x;

        bool jumpInput = jumpInputSource.action.WasPressedThisFrame();
        if (jumpInput && isGrounded) 
        {
            jumpVelocity = Mathf.Sqrt(2* -Physics.gravity.y * jumpHeight);
            rb.velocity = Vector3.up * jumpVelocity;
        }
    }

    private void FixedUpdate()
    {
        isGrounded = CheckIfGrounded();
        if (!onlyMoveWhenGrounded || (onlyMoveWhenGrounded && isGrounded))
        {
            Quaternion yaw = Quaternion.Euler(0,directionSource.eulerAngles.y,0);
            Vector3 direction = yaw * new Vector3(InputMoveAxis.x, 0, InputMoveAxis.y);

            Vector3 targetMovePosition = rb.position + direction * Time.fixedDeltaTime * speed;


            Vector3 axis = Vector3.up;
            float angle = turnspeed * Time.fixedDeltaTime * inputTurnAxis;

            Quaternion q = Quaternion.AngleAxis(angle, axis);

            rb.MoveRotation(rb.rotation * q);

            Vector3 newPosition = q * (targetMovePosition - turnSource.position) + turnSource.position;

            rb.MovePosition(newPosition);
        }
       
    }

    public bool CheckIfGrounded()
    {
        Vector3 start = bodyCollider.transform.TransformPoint(bodyCollider.center);
        float rayLength = bodyCollider.height / 2 - bodyCollider.radius + 0.05f;
        bool hashit = Physics.SphereCast(start, bodyCollider.radius, Vector3.down,out RaycastHit hitInfo, rayLength,groundLayer);
        return hashit;
    }


    private void ToggleRightSwing()
    {
        if (rightSwingObject != null)
        {
            rightSwingObject.SetActive(!rightSwingObject.activeSelf);
        }
    }

    private void ToggleLeftSwing()
    {
        if (leftSwingObject != null)
        {
            leftSwingObject.SetActive(!leftSwingObject.activeSelf);
        }
    }


}
