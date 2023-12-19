using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Moving
    Rigidbody _rigidbody;
    
    [SerializeField] float walkSpeed = 800f;
    [SerializeField] float runSpeed = 1000f;
	[SerializeField] float jumpForce = 50f;
	[SerializeField] float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    [SerializeField] Transform cam;

    [SerializeField] float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;
    Vector3 direction;

	//Animations
	bool running;
    Animator animator;
    bool isJumping;
    bool isFalling;
	float previousVelocity;
    

    [Header("Ground Check")]
    bool isGrounded = true;
    public float playerHeight;
    public LayerMask whatIsGround;


    //Climbing
    bool hanging;
    bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
		Cursor.lockState = CursorLockMode.Locked;
	}

    // Update is called once per frame
    void Update()
    {
        LedgeGrab();
		animator.SetBool("isHanging", hanging);
		if (canMove)
        {
			float forwardInput = Input.GetAxisRaw("Vertical");
			float sideInput = Input.GetAxisRaw("Horizontal");
			direction = new Vector3(sideInput, 0f, forwardInput);

			//ground check
			isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
            animator.SetBool("isGrounded", isGrounded);
            isJumping = false;
			animator.SetBool("isJumping", isJumping);

            //Falling animation
			var currentVelocity = _rigidbody.velocity.y;
			if (isGrounded)
            {
                currentVelocity = 0f;
                isFalling = false;
            }
                
			if (currentVelocity < previousVelocity) isFalling = true;
			animator.SetBool("isFalling", isFalling);
			previousVelocity = currentVelocity;

			
			//sprint staitment
			if (Input.GetKey(KeyCode.LeftShift))
			{
				//speed = runSpeed;
                running = true;
			}
			else
			{
				//speed = walkSpeed;
                running = false;
			}

            float targetSpeed = ((running) ? runSpeed : walkSpeed * direction.magnitude);
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);


			


            

            

			float animationSpeedPercent = ((running) ? 1 : .5f * direction.magnitude); //variable for changing animetion walking to running
			animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
		}
		
		//jump and hanging
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || hanging))
        {
            if (hanging)
            {
                _rigidbody.useGravity = true;
                hanging = false;
				isJumping = true;
				_rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                StartCoroutine(EnableCanMove(0.25f));
            }
            else
            {
                isJumping = true;
				hanging = false;
				animator.SetBool("isJumping", isJumping);
				_rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                
			}
            
        }
	}

    IEnumerator EnableCanMove(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        canMove = true;
    }

    void LedgeGrab()
    {
        if (_rigidbody.velocity.y < 0 && !hanging)
        {
            RaycastHit downHit;
            Vector3 lineDownStart = (transform.position + Vector3.up * 2.5f) + transform.forward;
            Vector3 lineDownEnd = (transform.position + Vector3.up * 1.7f) + transform.forward;
            Physics.Linecast(lineDownStart, lineDownEnd, out downHit, LayerMask.GetMask("Ground"));
            Debug.DrawLine(lineDownStart, lineDownEnd);
            
            if (downHit.collider != null)
            {
				RaycastHit fwdHit;
				Vector3 lineFwdStart = new Vector3(transform.position.x, downHit.point.y - 0.1f, transform.position.z);
				Vector3 lineFwdEnd = (transform.position + Vector3.up * 0.7f) + transform.forward;
				Physics.Linecast(lineFwdStart, lineFwdEnd, out fwdHit, LayerMask.GetMask("Ground"));
				Debug.DrawLine(lineFwdStart, lineFwdEnd);

                if (fwdHit.collider != null)
                {
                    _rigidbody.useGravity = false;
                    _rigidbody.velocity = Vector3.zero;

                    hanging = true;
                    isFalling = false;

                    Vector3 hangPos = new Vector3(fwdHit.point.x,downHit.point.y - 1.1f, fwdHit.point.z);
                    Vector3 offset = transform.forward * -0.1f + transform.up * -1f;
                    hangPos += offset;
                    transform.position = hangPos;
                    transform.forward = -fwdHit.normal;
                    canMove = false;
                }
			}
		}
    }

	private void FixedUpdate()
	{
		//character moving
		if (direction.magnitude >= 0.1f)
		{
			float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
			float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
			transform.rotation = Quaternion.Euler(0f, angle, 0f);
			Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

			_rigidbody.AddForce(moveDirection * currentSpeed * Time.fixedDeltaTime, ForceMode.Acceleration);
		}
	}
}
