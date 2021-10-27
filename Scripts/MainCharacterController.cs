using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainCharacterController : MonoBehaviour
{
    [SerializeField] LayerMask groundLayers;
    [Space]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float horizontalMovementSpeed = 1f;

    private float tempSpeed;
    private float tempHMSpeed;
    private Vector3 startPoint;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private Animator animator;
    private GameManager gm;

    private GameSituation gameSituation;

    private bool isGrounded;

    [SerializeField] private Camera mainCam;

    void Start()
    {

        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        animator = this.GetComponent<Animator>();

        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        tempSpeed = speed;
        tempHMSpeed = horizontalMovementSpeed;

        startPoint = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        gameSituation = gm.gameSituation;

        if (gameSituation == GameSituation.Continues )
        {
            if (capsuleCollider.enabled == false)
                capsuleCollider.enabled = true;

            CheckGrounded();

            rb.MovePosition(rb.position + new Vector3(0,0,1) * speed * Time.fixedDeltaTime);

            HorizontalMovement();
        }

        if(gameSituation == GameSituation.Win)
        {
            if(mainCam.transform.rotation != Quaternion.Euler(20, 0, 0))
            {
                mainCam.transform.Rotate(Vector3.right * -12f * Time.deltaTime, Space.Self);
            }

        }

    }

    private void CheckGrounded()
    {
        // Check if grounded
        isGrounded = Physics.CheckSphere(transform.position, 0.1f, groundLayers, QueryTriggerInteraction.Ignore);

        if (isGrounded)
        {
            rb.useGravity = false;
            animator.SetBool("isRunning", true);

        }
        else
        {
            if (isGrounded == false)
            {
                rb.useGravity = true;
                animator.SetBool("isRunning", false);
                animator.SetBool("isFalling", true);

                Lose();

            }
        }
    }
    private void HorizontalMovement()
    {
        Vector3 deltaPosition = transform.forward * speed;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.deltaPosition.x >= Screen.width / 20)
            {
                if (transform.position.x < 3.75f)
                    deltaPosition += new Vector3(Screen.width / 20 * horizontalMovementSpeed, 0, 0);


            }
            else if (touch.deltaPosition.x <= -Screen.width / 20)
            {
                if (transform.position.x > -4f)
                    deltaPosition += new Vector3(-Screen.width / 20 * horizontalMovementSpeed, 0, 0);

            }
            else
            {
                if (transform.position.x > -4f && transform.position.x < 3.75f)
                    deltaPosition += new Vector3(touch.deltaPosition.x * horizontalMovementSpeed, 0, 0);
            }
        }

        transform.position += deltaPosition * Time.deltaTime;
    }
    public void FallToFlat()
    {
        speed = 0;
        horizontalMovementSpeed = 0;
        animator.SetTrigger("isCaptured");

        Lose();
    } // Call when catch by AIs

    private void Lose()
    {
        if(gameSituation == GameSituation.Continues)
        {
            gm.gameSituation = GameSituation.Lose;
            speed = 0;
            horizontalMovementSpeed = 0;
            gm.restartPanel.SetActive(true);
        }

    }
    private IEnumerator Win()
    {
        if (gameSituation == GameSituation.Continues)
        {



            yield return new WaitForSeconds(0.5f);

            gm.gameSituation = GameSituation.Win;
            gm.winPanel.SetActive(true);
            speed = 0;
            horizontalMovementSpeed = 0;
            animator.SetBool("isRunning", false);



        }
    }
    public void Finish()
    {
        StartCoroutine(Win());
    }

    public void ResetCharacter()
    {
        mainCam.transform.rotation = Quaternion.Euler(30, 0, 0);

        speed = tempSpeed;
        horizontalMovementSpeed = tempHMSpeed;
        transform.position = startPoint;

        animator.SetBool("isRunning", false);

        capsuleCollider.enabled = false;
    } 


}
