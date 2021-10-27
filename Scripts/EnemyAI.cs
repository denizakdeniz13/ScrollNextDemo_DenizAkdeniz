using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Basic,
    Advanced,
    Premium,
}
public enum MoveDirection
{
    Forward,
    Left,
    Right,
    ForwardLeft,
    ForwardRight,

}

public class EnemyAI : MonoBehaviour
{
    [SerializeField] LayerMask groundLayers;
    [SerializeField] private EnemyType enemyType;
    [Space]
    [SerializeField] private float gravity = -1f;
    [SerializeField] private float speed = 5f;
    [SerializeField] float rotationSpeed = 1.0f;
    [Space]
    [SerializeField] private bool onPlatform;


    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;

    private GameObject player;
    private Animator animator;

    private Transform target;

    private GameManager gm;
    private GameSituation gameSituation;

    private FallingBoxManager boxManager;
    private FallingBox currentBox;
    private FallingBox nextBox;

    private MoveDirection lastMove;

    private float dashSpeed = 4.0f;

    void Start()
    {
        characterController = this.GetComponent<CharacterController>();
        animator = this.GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").gameObject;
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        boxManager = GameObject.FindGameObjectWithTag("FallingBoxManager").GetComponent<FallingBoxManager>();

        target = player.transform;

    }

    void Update()
    {
        gameSituation = gm.gameSituation;


        if (gameSituation == GameSituation.Continues)
        {

            Strike();
            CheckGrounded();

            if (enemyType == EnemyType.Basic)
            {
                if (onPlatform)
                    BasicMovement();
                else
                {
                    if (target.position.z >= this.transform.position.z)
                        BasicMovement();
                }
            }
                

            if (enemyType == EnemyType.Advanced)
            {
                if (onPlatform)
                    AdvancedMovement();
                else
                {
                    if(target.position.z >= this.transform.position.z)
                        BasicMovement();
                }

            }

            if (enemyType == EnemyType.Premium)
            {
                if(onPlatform)
                    PremiumMovement();
                else
                {
                    if (target.position.z >= this.transform.position.z)
                        BasicMovement();
                }
            }


        }

    }

    private void CheckGrounded()
    {
        characterController.Move(velocity * Time.deltaTime);

        isGrounded = Physics.CheckSphere(transform.position, 0.1f, groundLayers, QueryTriggerInteraction.Ignore);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0;

        }
        else
        {
            //Add gravity
            velocity.y += gravity * Time.deltaTime;

        }
    }
    private void BasicMovement()
    {
        animator.SetBool("isRunning", true);


        if(onPlatform)
            characterController.Move(new Vector3(0, 0, 1) * Time.deltaTime * speed);
        else
        {
            if(player.transform.position.x > this.transform.position.x) // Check left or right stair
                characterController.Move(new Vector3(1, 0, 0) * Time.deltaTime * speed);
            else
                characterController.Move(new Vector3(1, 0, 0) * Time.deltaTime * speed * -1);
        }


    }
    private void AdvancedMovement()
    {
        if(currentBox != null)
        {
            animator.SetBool("isRunning", true);
            if(nextBox != null)
                MoveTowardsTarget(nextBox.gameObject.transform.position);
            else
                FakeJump();

        }
        else
            FindCollision();
    }
    private void PremiumMovement()
    {
        if (currentBox != null)
        {
            animator.SetBool("isRunning", true);
            if (nextBox != null)
                MoveTowardsTarget(nextBox.gameObject.transform.position);
            else
                FakeJump();

        }
        else
            FindCollision();
    }

    private void Strike() // Jump to character when close enough
    {
        float dist = Vector3.Distance(player.transform.position, transform.position);

        if(dist <= 2f)
        {
            speed = 0;

            Vector3 targetDirection = target.position - transform.position;

            float singleStep = rotationSpeed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

            transform.rotation = Quaternion.LookRotation(newDirection);

            target.gameObject.GetComponent<MainCharacterController>().FallToFlat();
            animator.SetTrigger("Jump");


        }
    }

    private void FakeJump() // Dash
    {
        Vector3 offset;
        if(enemyType == EnemyType.Advanced )
            offset = Vector3.forward;
        else // Detect jump direction for premium AI
        {
            MoveDirection direction = FindCharacterDirection();

            if (direction == MoveDirection.Forward)
                offset = Vector3.forward;
            else if (direction == MoveDirection.Left)
                offset = new Vector3(-1f, 0, 1f);
            else
                offset = new Vector3(1f, 0, 1f);

        }

        if (offset.magnitude > .1f)
        {
            offset = offset.normalized * speed * dashSpeed;

            characterController.Move(offset * Time.deltaTime);
        }
        FindCollision();
    }  


    void MoveTowardsTarget(Vector3 target) //Move box by box ( target = next box )
    {
        Vector3 offset = target - transform.position;
        if (offset.magnitude > .1f)
        {
            offset = offset.normalized * speed;

            characterController.Move(offset * Time.deltaTime);
        }
        if (Vector3.Distance(this.transform.position, target) <= 0.7f)
        {
            currentBox = nextBox;
            FindBoxPosition();
        }
    }

    private void FindBoxPosition() // Find current box position on boxes matrix as line and colon
    {
        int lineCounter = -1;

        if(boxManager.lines.Count != 0)
        {
            foreach (Line line in boxManager.lines)
            {
                lineCounter += 1;
                int indexCounter = -1;

                foreach (FallingBox box in line.line)
                {
                    indexCounter += 1;
                    if(currentBox == box)
                    {


                        if (enemyType == EnemyType.Advanced)
                            nextBox = FindNextBox(lineCounter, indexCounter);
                        else if (enemyType == EnemyType.Premium)
                            nextBox = FindNextBoxPrem(lineCounter, indexCounter);
                    }
                }
            }
        }

    }

    private FallingBox FindNextBox(int lineCounter, int indexCounter)
    {
        FallingBox nextBox;

        if(lineCounter + 1 < boxManager.lines.Count)
        {
            if (boxManager.lines[lineCounter + 1].line[indexCounter].isFall == false)
            {
                nextBox = boxManager.lines[lineCounter + 1].line[indexCounter];
                lastMove = MoveDirection.Forward;
                return nextBox;
            }

            if (boxManager.lines[lineCounter].line.Length > indexCounter + 1 && indexCounter - 1 >= 0) // If left and right sides enabled to go check left forward and right forward 
            {
                if (lastMove == MoveDirection.Forward && boxManager.lines[lineCounter].line[indexCounter + 1].isFall == false && boxManager.lines[lineCounter].line[indexCounter - 1].isFall == false)
                {
                    if (lineCounter + 1 < boxManager.lines.Count && boxManager.lines[lineCounter].line.Length > indexCounter + 1)
                    {
                        if (boxManager.lines[lineCounter + 1].line[indexCounter + 1].isFall == false)
                        {
                            nextBox = boxManager.lines[lineCounter].line[indexCounter + 1];
                            lastMove = MoveDirection.Right;
                            return nextBox;
                        }

                    }

                    if (lineCounter + 1 < boxManager.lines.Count && indexCounter - 1 >= 0)
                    {
                        if (boxManager.lines[lineCounter + 1].line[indexCounter - 1].isFall == false)
                        {
                            nextBox = boxManager.lines[lineCounter].line[indexCounter - 1];
                            lastMove = MoveDirection.Left;
                            return nextBox;
                        }
                    }


                }
            }

            if (boxManager.lines[lineCounter].line.Length > indexCounter + 1)
            {
                if (boxManager.lines[lineCounter].line[indexCounter + 1].isFall == false && lastMove != MoveDirection.Left)
                {
                    nextBox = boxManager.lines[lineCounter].line[indexCounter + 1];
                    lastMove = MoveDirection.Right;
                    return nextBox;
                }
            }

            if (indexCounter - 1 >= 0)
            {
                if (boxManager.lines[lineCounter].line[indexCounter - 1].isFall == false && lastMove != MoveDirection.Right)
                {
                    nextBox = boxManager.lines[lineCounter].line[indexCounter - 1];
                    lastMove = MoveDirection.Left;
                    return nextBox;
                }
            }

            return null;
        }
        else
        {
            DestroyObject();
            return null;
        }



       
    }

    private FallingBox FindNextBoxPrem(int lineCounter, int indexCounter)
    {
        MoveDirection direction = FindCharacterDirection();

        if (lineCounter + 1 < boxManager.lines.Count && boxManager.lines[lineCounter].line.Length > indexCounter + 1) //Move Right Forward
        {
            if (direction == MoveDirection.Right && boxManager.lines[lineCounter + 1].line[indexCounter + 1].isFall == false)
            {
                nextBox = boxManager.lines[lineCounter + 1].line[indexCounter + 1];
                lastMove = MoveDirection.ForwardRight;
                return nextBox;

            }
        }

        if (lineCounter + 1 < boxManager.lines.Count && indexCounter - 1 >= 0) //Move Left Forward
        {
            if (direction == MoveDirection.Left && boxManager.lines[lineCounter + 1].line[indexCounter - 1].isFall == false)
            {
                nextBox = boxManager.lines[lineCounter + 1].line[indexCounter - 1];
                lastMove = MoveDirection.ForwardLeft;
                return nextBox;
            }
        }

        if (lineCounter + 1 < boxManager.lines.Count) // Move Forward
        {
            if (boxManager.lines[lineCounter + 1].line[indexCounter].isFall == false)
            {
                nextBox = boxManager.lines[lineCounter + 1].line[indexCounter];
                lastMove = MoveDirection.Forward;
                return nextBox;
            }
        }

        if (boxManager.lines[lineCounter].line.Length > indexCounter + 1 && indexCounter - 1 >= 0) // If left and right sides enable to go check left forward and right forward for direction
        {
            if (lastMove != MoveDirection.Right && lastMove != MoveDirection.Left && boxManager.lines[lineCounter].line[indexCounter + 1].isFall == false && boxManager.lines[lineCounter].line[indexCounter - 1].isFall == false)
            {
                if (lineCounter + 1 < boxManager.lines.Count && boxManager.lines[lineCounter].line.Length > indexCounter + 1)
                {
                    if (boxManager.lines[lineCounter + 1].line[indexCounter + 1].isFall == false)
                    {
                        nextBox = boxManager.lines[lineCounter].line[indexCounter + 1];
                        lastMove = MoveDirection.Right;
                        return nextBox;
                    }

                }

                if (lineCounter + 1 < boxManager.lines.Count && indexCounter - 1 >= 0)
                {
                    if (boxManager.lines[lineCounter + 1].line[indexCounter - 1].isFall == false)
                    {
                        nextBox = boxManager.lines[lineCounter].line[indexCounter - 1];
                        lastMove = MoveDirection.Left;
                        return nextBox;
                    }
                }


            }
        }

        if (boxManager.lines[lineCounter].line.Length > indexCounter + 1) // Move Right
        {
            if (boxManager.lines[lineCounter].line[indexCounter + 1].isFall == false && lastMove != MoveDirection.Left)
            {
                nextBox = boxManager.lines[lineCounter].line[indexCounter + 1];
                lastMove = MoveDirection.Right;
                return nextBox;
            }
        }

        if (indexCounter - 1 >= 0) // Move Left
        {
            if (boxManager.lines[lineCounter].line[indexCounter - 1].isFall == false && lastMove != MoveDirection.Right)
            {
                nextBox = boxManager.lines[lineCounter].line[indexCounter - 1];
                lastMove = MoveDirection.Left;
                return nextBox;
            }
        }

            
        return null;
    }

    private MoveDirection FindCharacterDirection()
    {
        if (this.transform.position.x > target.position.x)
            return MoveDirection.Left;

        else if (this.transform.position.x < target.position.x)
            return MoveDirection.Right;

        else
            return MoveDirection.Forward;

    }

    public void OnPlatform() // Call when AIs at stairs came main platform
    {
        float randomFloat = Random.Range(0.2f, 0.4f);
        StartCoroutine(SetOnPlatform(randomFloat));
    } 

    private IEnumerator SetOnPlatform(float randomTime)
    {
        yield return new WaitForSeconds(randomTime);
        transform.rotation = Quaternion.Euler(0, 0, 0);

        FindCollision();
        onPlatform = true;

    }
    

    private void FindCollision() // Detect current box 
    {
        if(enemyType != EnemyType.Basic)
        {
            var ray = new Ray(this.transform.position, -Vector3.up);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10f))
            {
                currentBox = hit.transform.gameObject.GetComponent<FallingBox>();
                FindBoxPosition();
            }
        }
    }

    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
