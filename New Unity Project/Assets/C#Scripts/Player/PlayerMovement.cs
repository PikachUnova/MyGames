using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement playerMovement;

    [Header("Movement")]
    private float moveSpeed = 6f;
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 12f;
    [SerializeField] private float flyingSpeed = 8f;
    [SerializeField] private float flyingFastSpeed = 14f;

    // Vectors for motion magnitude and direction
    private Vector3 moveDirection;
    private Vector3 velocity;

    // States
    private bool isGrounded = true;
    private bool isCrouching = false;
    private bool isRunning = false;
    private bool isTalking = false;
    private bool isJumping = false;
    private bool isDodging = false;
    private bool isClimbing = false;
    private bool isGrabbingLedge = false;
    private bool isInWater = false;
    private bool isFlying = false;
    private bool isUsingSpecial = false;

    // Variables that deals with jumping
    [Header("Ground Test")]    
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jumping Settings")]
    private float gravity = -9.81f;     // Gravity constant
    private float jumpHeight = 5f;    
    private float jumpHoldTime = 0f;    // Time the jump button has been held


    [Header("Wall Jump Settings")]
    [SerializeField] private float wallCheckDistance = 1f;   // Distance to check for wall
    [SerializeField] private float clingTime = 5f;             // Time player can cling to wall
    public LayerMask wallLayer;              // Layer mask to detect walls
    private bool isTouchingWall = false;             // If the player is touching the wall
    private bool isAttachedToWall = false;  // The player clings into a wall in a limited time
    private float clingTimer = 0f;                // Timer for how long the player can cling to the wall
    private Vector3 wallNormal;              // Normal vector of the wall the player is touching
    private float wallDirection;             // The direction the player is facing on the wall

    [Header("Dodging")]
    [SerializeField] private float dodgeDistance = 3f;
    [SerializeField] private float dodgeDuration = 0.2f;

    private CharacterController controller; // Component to control the character

    [Header("Camera Settings")]
    public Transform cam;
    float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public CinemachineFreeLook freeLookCamera;  // Reference to the Cinemachine FreeLook Camera
    public CinemachineVirtualCamera virtualCamera;  // Reference to the Virtaul Camera
    private bool isAiming = false;

    [Header("FootStep Sounds")]
    private AudioSource audioSource;
    public AudioClip jump;
    public AudioClip swipeEffect;
    public AudioClip[] walkOnGrass;
    public AudioClip[] walkOnSnow;
    public AudioClip[] walkOnHardSurface;

    public GameObject lFoot;
    public GameObject rFoot;
    public GameObject footPrint;
    int numSteps = 0;

    // Adjusted play interval for stepping
    private float playInterval = 0.5f;
    private float nextPlayTime;

    [Header("Animation + Rigging")]
    public GameObject playermodelAnimation;
    public Transform lookTarget;
    public Transform bone;

    [Header("Melee Attacks")]
    float coolDown = 0.3f;
    float lastTriggered = 0.3f;
    int counterCombo = 0;
    public GameObject slashAttackObject;
    public GameObject biteAttackObject;

    [Header("Water and Swimming")]
    [SerializeField] private float swimSpeed = 6;

    [Header("Text Commands")]
    public GameObject InteractableImage;
    public Text InteractableCommandText;
    public Text InteractableButtonText;

    private BoxCollider boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        controller = GetComponent<CharacterController>();
        if (PlayerMovement.playerMovement != null)
        {
            Destroy(this.gameObject);
            return;
        }
        playerMovement = this;
        DontDestroyOnLoad(this);

        audioSource = GetComponent<AudioSource>();
        nextPlayTime = Time.time + playInterval; // Initial delay before the first play

        virtualCamera.gameObject.SetActive(false);
        InteractableImage.gameObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(this.transform.position, groundCheckRadius);
    }

    void Update() // Update is called once per frame
    {
        RaycastHit hit;
        isGrounded = Physics.CheckSphere(this.transform.position, groundCheckRadius, groundLayer); // Update Grounded

        if (GetComponent<PlayerHealth>().isDead || isUsingSpecial || isTalking) // Don't Move in certain conidtions
        { 
            InteractableImage.SetActive(false);
            return;
        }
        Attack();
        ClimbLadder();
        LedgeGrab();
        
        if (Input.GetKeyDown("r") && !isInWater && !isClimbing && !isGrabbingLedge) // Start taking off to fly
        {
            if (!isFlying)
            {
                isFlying = true;
                playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().PlayTakeOff();
                velocity.y = 0;
            }
            else
                isFlying = false;
        }

        if (Input.GetMouseButtonDown(1)) // Trigger Aiming
        {
            if (!isAiming)
            {
                isAiming = true;
                LockCameraRotation(true);
                freeLookCamera.gameObject.SetActive(false);
                virtualCamera.gameObject.SetActive(true);
            }
            else
            {
                isAiming = false;
                LockCameraRotation(false);
                freeLookCamera.gameObject.SetActive(true);
                virtualCamera.gameObject.SetActive(false);
            }
        }

        if (isFlying)
            Fly();
        
        if (isInWater)
            Swim();

        if (!isGrabbingLedge && !isInWater && !isFlying && !isClimbing)
        { 
            if (!isClimbing)
                Move();
            
            Dodge(); // Dodge
            Crouch(); // Crouch
            Jump(); // Jump
            WallJump(); // Wall Jump

            if (!isAttachedToWall)
                Fall(); // Fall if not touching to any wall
            Land();
        }

        if (isGrounded) // Speed does not change when midair
        {
            if (isRunning) // Adjust speed when walking or running
            {
                moveSpeed = runSpeed;
                playInterval = 0.25f;
            }
            else
            {
                moveSpeed = walkSpeed;
                playInterval = 0.5f;
            }
        }

        if (isFlying)
        {
            if (isRunning) // Adjust speed while flying
            {
                moveSpeed = flyingFastSpeed;
                playInterval = 0.25f;
            }
            else
            {
                moveSpeed = flyingSpeed;
                playInterval = 0.25f;
            }
        }

        if (!isAiming) // Adjust Look Target while aiming
        {
            if (isRunning && IsMoving() && !isCrouching || isInWater)
                lookTarget.localPosition = new Vector3(lookTarget.localPosition.x, 0.65f, lookTarget.localPosition.z);
            else if (isCrouching)
                lookTarget.localPosition = new Vector3(lookTarget.localPosition.x, 0.45f, lookTarget.localPosition.z);
            else if (!isUsingSpecial)
                lookTarget.localPosition = new Vector3(lookTarget.localPosition.x, 0.86f, lookTarget.localPosition.z);
        }
        
        SimulateFriction(); // Prevent player from moving forever
        OnSlopeSliding(); // Interact or Slide on steep surfaces
        
        isTouchingWall = Physics.Raycast(transform.position, transform.forward, out hit, wallCheckDistance, wallLayer);
        if (!isTouchingWall)
            isAttachedToWall = false;

        if (isTouchingWall && !isAttachedToWall && !isGrounded) // Wall Jump logic
        {
            if (Input.GetKey(KeyCode.LeftShift) && !isAttachedToWall) // Use shift instead of jump because there are two keys conflicting each other
            {
                isAttachedToWall = true;
                isJumping = false;
                velocity.y = 0;
            }
        }

        if (isTouchingWall && isAttachedToWall && clingTimer <= clingTime)
        {
            clingTimer += Time.deltaTime;
            playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().PlayWallCling();
        }
        else if (clingTimer > clingTime && !isGrounded && isTouchingWall)
        { 
            playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().PlayWallCling();
            velocity.y = -3;
        }
        else
        { 
            if (isGrounded || !isTouchingWall) // Reset cling time 
                clingTimer = 0f;
            isAttachedToWall = false;
        }

    }

    private void Move() // Move the player by changing position and/or angle
    {
        if (!isAiming)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveZ = Input.GetAxisRaw("Vertical");
            moveDirection = new Vector3(moveX, 0, moveZ).normalized;

            if (IsMoving())
            {
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDirectionAngle = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                if (IsOnSlipperySurface())
                {
                    velocity = Vector3.Lerp(velocity, moveDirectionAngle.normalized * moveSpeed, 0.01f);
                }
                else
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f, groundLayer)) // Go up and down the slope
                    {
                        float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                        Vector3 slopeDirection = Vector3.ProjectOnPlane(moveDirectionAngle, hit.normal).normalized;
                        controller.Move(slopeDirection.normalized * moveSpeed * Time.deltaTime);
                    }
                    else
                        controller.Move(moveDirectionAngle.normalized * moveSpeed * Time.deltaTime);
                }
                Run();
                AnimatePlayerMotion();
            }
            else
                playermodelAnimation.GetComponent<PlayerAnimator>().SetLocomotive(0f);
        }
        else
        {
            ShootAim();
            MoveAim();
            Run();
        }

        if (!IsOnSlipperySurface()) // No slipping after leaving the slippery surface
        {
            velocity.x = 0f;
            velocity.z = 0f;
        }

        if (IsMoving())
            PlayFootstepSound();
        else
            numSteps = 0;
    }

    private void OnSlopeSliding()
    {
        float slideSpeed = 4f;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f, groundLayer)) // Go up and down the slope
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up); // Adjust ground CheckRadius via slope angle
            
            if (slopeAngle >= 54f && slopeAngle <= controller.slopeLimit)
                groundCheckRadius = 0.5f;
            else if (slopeAngle >= 47f && slopeAngle <= controller.slopeLimit)
                groundCheckRadius = 0.45f;
            else if (slopeAngle >= 39f && slopeAngle <= controller.slopeLimit)
                groundCheckRadius = 0.4f;
            else if (slopeAngle >= 29f && slopeAngle <= controller.slopeLimit)
                groundCheckRadius = 0.35f;
            else
                groundCheckRadius = 0.3f;
            
        }

        if (Physics.SphereCast(transform.position, controller.radius, Vector3.down, out hit, controller.height / 2 + 0.5f, groundLayer))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

            if (slopeAngle > controller.slopeLimit)
            {
                // Slide along slope, but always add downward motion
                Vector3 slideDir = new Vector3(hit.normal.x, -hit.normal.y, hit.normal.z);

                // Ensure a minimum downward push so player doesn't stick
                slideDir.y = Mathf.Min(slideDir.y, -0.5f);

                controller.Move(slideDir.normalized * slideSpeed * Time.deltaTime);
            }
        }
    }

    private bool IsOnSlipperySurface()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1f))
        {
            if (hit.collider.sharedMaterial != null && hit.collider.sharedMaterial.name.Contains("Slippery"))
                return true;
        }
        return false;
    }

    void Run()
    {
        if (isCrouching) return;

        if (Input.GetKey(KeyCode.LeftShift) && !isAiming)
            isRunning = true;
        else
            isRunning = false;
    }

    void Jump()
    {
        if (isGrounded)
        {
            isJumping = false;
            jumpHoldTime = 0;

            if (velocity.y < 0)
                velocity.y = -2f;
        }

        if (!isJumping && isGrounded && Input.GetButtonDown("Jump"))
        {
            audioSource.PlayOneShot(jump);
            if (!isCrouching)
                playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().PlayJumpStart();
            else
                playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().PlayJumpCrouching();
            isJumping = true;
            velocity.y = jumpHeight;
            jumpHoldTime = 0.2f;
        }
        if (isJumping && !isGrounded && Input.GetButton("Jump") && jumpHoldTime > 0)
        {
            velocity.y += 4f * jumpHeight * Time.deltaTime;
            jumpHoldTime -= Time.deltaTime;
        }
    }

    void WallJump()
    {
        if (isAttachedToWall)
        {
            if (Input.GetButtonDown("Jump")) 
            {
                audioSource.PlayOneShot(jump);
                playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().PlayJumpStart(); // Play jump animation
                isAttachedToWall = false;
                clingTimer = 0f;
                velocity.y = jumpHeight * 1.8f;
            }
        }
    }

    void Fall()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (!isGrounded && velocity.y < 0f)
        {
            if (!isCrouching)
                playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().PlayFall();
            else
                playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().PlayFallCrouching();
        }
    }

    void Land()
    {
        if (isGrounded && velocity.y < 0)
        {
            playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().PlayLand();
            velocity.y = 0;
        }
    }

    void Crouch()
    {
        if (Input.GetKeyDown("c") && !isCrouching && !IsMoving())
        {
            isCrouching = true;
            boxCollider.center = new Vector3(0f, 0.27f, 0f);
            boxCollider.size = new Vector3(0.6f, 0.95f, 1.6f);
            playermodelAnimation.GetComponent<PlayerAnimator>().PlayCrouch();

        }
        else if (Input.GetKeyDown("c") && isCrouching && !IsMoving() && isGrounded)
        {
            UndoCrouch();
            playermodelAnimation.GetComponent<PlayerAnimator>().PlayCrouchGetUp();
        }
    }

    private void UndoCrouch()
    {
        boxCollider.center = new Vector3(0f, 0.55f, 0f);
        boxCollider.size = new Vector3(0.6f, 1.5f, 0.8f);
        isCrouching = false;
    }

    private void Dodge()
    {
        if (!isDodging)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                playermodelAnimation.GetComponent<PlayerAnimator>().PlayDodgeLeft();
                StartCoroutine(Dodge(-transform.right)); // Left
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                playermodelAnimation.GetComponent<PlayerAnimator>().PlayDodgeRight();
                StartCoroutine(Dodge(transform.right)); // Right
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                playermodelAnimation.GetComponent<PlayerAnimator>().PlayDodgeBack();
                StartCoroutine(Dodge(-transform.forward)); // back
            }
        }
    }

    private System.Collections.IEnumerator Dodge(Vector3 direction)
    {
        isDodging = true;

        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + direction * dodgeDistance;

        while (elapsedTime < dodgeDuration)
        {
            Vector3 nextPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / dodgeDuration);
            controller.Move(nextPosition - transform.position); // move difference
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        controller.Move(targetPosition - transform.position);
        isDodging = false;
    }

    void Fly()
    {
        if (isAiming)
        {
            MoveAim();
            ShootAim();
        }
        else
            Hover();
        Ascend();
        Run();
    }

    void Hover()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        if (IsMoving())
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirectionAngle = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirectionAngle.normalized * moveSpeed * Time.deltaTime);
            playermodelAnimation.GetComponent<PlayerAnimator>().SetLocomotive(0.5f);
        }
        else
            playermodelAnimation.GetComponent<PlayerAnimator>().SetLocomotive(0f);
    }

    void Ascend()
    {
        if (Input.GetButton("Jump"))
        {
            velocity = new Vector3(velocity.x, velocity.y, velocity.z);
            velocity.y = flyingSpeed;
        }
        else
            velocity.y = 0f;
    }

    void PlayFootstepSound()
    {
        if (Time.time < nextPlayTime || !isGrounded)
            return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 10f))
        {
            Terrain terrain = hit.collider.GetComponent<Terrain>();
            if (terrain != null)
            {
                int textureIndex = GetMainTextureIndex(terrain, hit.point); // Get the texture under the player
                AudioClip randomClip = null;

                switch (textureIndex)
                {
                    case 0: // Grass
                        randomClip = walkOnGrass[Random.Range(0, walkOnGrass.Length)];
                        break;
                    case 1: // HardSurface
                        randomClip = walkOnHardSurface[Random.Range(0, walkOnHardSurface.Length)];
                        break;
                }

                if (randomClip != null)
                    audioSource.PlayOneShot(randomClip);
            }
            else // Play a random sound depending on the non-terrian surface
            {
                if (Physics.Raycast(transform.position, Vector3.down, out hit, 10f, groundLayer) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Grass"))
                {
                    AudioClip randomClip = walkOnGrass[Random.Range(0, walkOnGrass.Length)];
                    audioSource.PlayOneShot(randomClip);
                }
                else if (Physics.Raycast(transform.position, Vector3.down, out hit, 10f, groundLayer) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Snow"))
                {
                    AudioClip randomClip = walkOnSnow[Random.Range(0, walkOnSnow.Length)];
                    audioSource.PlayOneShot(randomClip);
                    AddFootsteps();
                }
                else if (Physics.Raycast(transform.position, Vector3.down, out hit, 10f, groundLayer) && hit.collider.gameObject.layer == LayerMask.NameToLayer("HardSurface"))
                {
                    AudioClip randomClip = walkOnHardSurface[Random.Range(0, walkOnHardSurface.Length)];
                    audioSource.PlayOneShot(randomClip);
                }
            }
        }

        nextPlayTime = Time.time + playInterval; // set next play time
    }

    void AddFootsteps()
    {
        if (numSteps == 0)
        {
            numSteps++;
            Instantiate(footPrint, lFoot.transform.position, lFoot.transform.rotation);
        }
        else
        {
            numSteps--;
            Instantiate(footPrint, rFoot.transform.position, rFoot.transform.rotation);
        }
    }

    int GetMainTextureIndex(Terrain terrain, Vector3 worldPos)
    {
        TerrainData data = terrain.terrainData;
        Vector3 terrainPos = worldPos - terrain.transform.position;

        int x = Mathf.FloorToInt(terrainPos.x / data.size.x * data.alphamapWidth);
        int z = Mathf.FloorToInt(terrainPos.z / data.size.z * data.alphamapHeight);

        float[,,] splatmap = data.GetAlphamaps(x, z, 1, 1);

        int maxIndex = 0;
        float maxValue = 0f;

        for (int i = 0; i < splatmap.GetLength(2); i++)
        {
            if (splatmap[0, 0, i] > maxValue)
            {
                maxValue = splatmap[0, 0, i];
                maxIndex = i;
            }
        }

        return maxIndex;
    }

    private void AnimatePlayerMotion()
    {
        if (isGrounded)
        {
            if (!isRunning)
                playermodelAnimation.GetComponent<PlayerAnimator>().SetLocomotive(0.5f);
            else
                playermodelAnimation.GetComponent<PlayerAnimator>().SetLocomotive(1f);
        }
    }

    private void AnimatePlayerAiming(float direction)
    {
        if (isGrounded)
        {
            if (direction < 0f)
                playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().SetLocomotive(-0.5f);
            else if (direction > 0f)
                playermodelAnimation.GetComponent<PlayerAnimator>().SetLocomotive(0.5f);
            
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) // Hit a Bouncy Object or Push Object
    {
        // Push a rigidbody object
        Rigidbody rigidbody = hit.collider.attachedRigidbody;
        if (rigidbody != null)
        {
            Vector3 forceDirection = hit.gameObject.transform.position - transform.position;
            forceDirection.y = 0;
            forceDirection.Normalize();
            rigidbody.AddForceAtPosition(forceDirection * 10f, transform.position, ForceMode.Impulse);
        }

        // Collide with a bouncy object
        Collider collider = hit.collider;
        PhysicsMaterial material = collider.material;

        if (material != null && material.bounciness > 0.8f)
        {
            float bounceForce = 15f;
            Vector3 bounceDirection = hit.normal;
            playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().PlayJumpStart();

            // Calculate the new velocity by reflecting the current velocity off the collision normal
            velocity = Vector3.Reflect(velocity, bounceDirection).normalized * bounceForce;
            controller.Move(velocity * Time.deltaTime);
        }
    }

    void SimulateFriction()
    {
        float friction = 0.99f; // Decay velocity gradually to simulate friction

        // Apply the friction to the horizontal components of the velocity
        velocity.x *= friction;
        velocity.z *= friction;
        controller.Move(velocity * Time.deltaTime);
    }

    void Swim()
    {
        if (!isAiming)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveZ = Input.GetAxisRaw("Vertical");
            moveDirection = new Vector3(moveX, 0, moveZ).normalized;

            if (IsMoving())
            {
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDirectionAngle = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDirectionAngle.normalized * swimSpeed * Time.deltaTime);
                playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().SetLocomotive(0.5f);
            }
            else
                playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().SetLocomotive(0f);
        }
        else
        {
            MoveAim();
            ShootAim();
        }

        velocity.y = 0;
        if (Input.GetButton("Jump")) // Swim up the water
        {
            velocity = new Vector3(velocity.x, velocity.y, velocity.z);
            velocity.y = Mathf.Clamp(velocity.y, -1, 1);
            velocity.y = swimSpeed / 2;
            playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().SetLocomotive(0.5f);
        }
        if (Input.GetKey(KeyCode.LeftShift)) // Dive down deeper
        {
            velocity = new Vector3(velocity.x, velocity.y, velocity.z);
            velocity.y = Mathf.Clamp(velocity.y, -1, 1);
            velocity.y = -swimSpeed / 2;
            playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().SetLocomotive(0.5f);
        }

    }

    void Attack()
    {
        if (Input.GetKeyDown("e"))
        {
            if (!isCrouching && !isAttachedToWall)
            {
                GameObject attack = slashAttackObject;
                counterCombo++;
                if (Time.time - lastTriggered >= coolDown)
                {
                    if (counterCombo == 1)
                        playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().PlayClawAttack1();
                    if (counterCombo == 2) // After attack 1 animation
                        playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().PlayClawAttack2();

                    attack = Instantiate(attack, this.transform.position, this.transform.rotation);
                    AudioSource.PlayClipAtPoint(swipeEffect, transform.position, 100f);
                    attack.transform.Translate(slashAttackObject.transform.position.x, slashAttackObject.transform.position.y + 1, slashAttackObject.transform.position.z + 2);
                    attack.transform.Rotate(slashAttackObject.transform.rotation.x, slashAttackObject.transform.rotation.y + 90, slashAttackObject.transform.rotation.z);
                }
            }
            else if (isCrouching)
            {
                playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().PlayBiteAttack();

                if (counterCombo == 0)
                    biteAttackObject.SetActive(true);
                counterCombo++;
            }
        }

        if (counterCombo >= 2)
        {
            counterCombo = 0;
            biteAttackObject.SetActive(false);
        }
    }

    // Interact with NPCs, Chests, and Doors
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("NPC")) 
        {
            InteractableCommandText.text = "Talk";
            InteractableButtonText.text = "E";
            InteractableImage.SetActive(true);
        } 
        else if (other.gameObject.CompareTag("Chest"))
        {
            InteractableCommandText.text = "Open";
            InteractableButtonText.text = "E";
            InteractableImage.SetActive(true);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("NPC") && Input.GetKeyDown(KeyCode.E))
            other.GetComponent<NPCMovement>().Talk();
        
        else if (other.CompareTag("Chest") && Input.GetKeyDown(KeyCode.E))
            other.GetComponent<Animator>().Play("Open");
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC") || other.CompareTag("Chest"))
            InteractableImage.SetActive(false);
    }

    public void ShootAim()
    {
        if (lookTarget != null)
        {
            // Get mouse input
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Get camera's right and up directions (ignore Z for up, ignore Y for right)
            Vector3 cameraRight = Camera.main.transform.right;
            Vector3 cameraUp = Camera.main.transform.up;

            // Flatten the axes so lookTarget stays in the desired plane
            cameraRight.y = 0; // Keep horizontal movement flat
            cameraUp.z = 0;    // Keep vertical movement flat
            cameraRight.Normalize();
            cameraUp.Normalize();

            // Move the lookTarget based on mouse input and camera axes
            Vector3 moveDelta = cameraRight * mouseX + cameraUp * mouseY;
            lookTarget.position += moveDelta * 10f * Time.deltaTime;

            // Clamp relative to the initial local position 
            Vector3 localPos = lookTarget.localPosition;
            float minX = -10f, maxX = 10f;
            float minY = -5f, maxY = 5f;
            localPos.x = Mathf.Clamp(localPos.x, minX, maxX);
            localPos.y = Mathf.Clamp(localPos.y, minY, maxY);
            localPos.z = 5f; // Keep it in front of player
            lookTarget.localPosition = localPos;
        }
    }

    private void LockCameraRotation(bool lockRotation)
    {
        if (freeLookCamera != null)
        {
            if (lockRotation) // Disable the FreeLook camera's orbit behavior during aiming
            {
                freeLookCamera.m_XAxis.m_InputAxisName = "";
                freeLookCamera.m_YAxis.m_InputAxisName = "";
                freeLookCamera.m_Lens.FieldOfView = 30f;
            }
            else // Re-enable FreeLook camera's rotation when not aiming
            {
                freeLookCamera.m_XAxis.m_InputAxisName = "Mouse X";
                freeLookCamera.m_YAxis.m_InputAxisName = "Mouse Y";
                StartCoroutine(SmoothlyResetLookTargetPosition());
                freeLookCamera.m_Lens.FieldOfView = 40f;
            }
        }
    }

    private IEnumerator SmoothlyResetLookTargetPosition()
    {
        Vector3 initialPosition = lookTarget.transform.localPosition;
        Vector3 targetPosition = new Vector3(0f, 1f, 3f);
        float duration = 0.5f;
        float timeElapsed = 0f;

        // Smooth transition using Lerp
        while (timeElapsed < duration)
        {
            lookTarget.transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;  // Wait for the next frame
        }
        lookTarget.transform.localPosition = targetPosition; // Final position is exactly the target position
    }

    private void MoveAim()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().SetDirection(moveX);

        // Rotate while aiming and hold SHIFT to lock rotation
        if (Mathf.Abs(moveX) > 0.01f && !Input.GetKey(KeyCode.LeftShift))
            transform.Rotate(Vector3.up, moveX * 180f * Time.deltaTime);
        
        // Get the camera's forward and right directions
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // Flatten the forward direction (ignore the Y component)
        cameraForward.y = 0;
        cameraRight.y = 0;

        // Normalize the direction vectors
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate the movement direction based on camera orientation
        moveDirection = (cameraForward * moveZ + cameraRight * moveX).normalized;

        if (IsMoving())
        {
            if (IsOnSlipperySurface())
                velocity = Vector3.Lerp(velocity, moveDirection.normalized * moveSpeed, 0.01f);
            else
                controller.Move(moveDirection * moveSpeed * Time.deltaTime);
            AnimatePlayerAiming(moveZ);
        }
        else
            playermodelAnimation.GetComponent<PlayerAnimator>().SetLocomotive(0f);
    }

    public void MoveLookTarget(float x, float y, float z) // Move looking point at the desired position
    {
        Vector3 targetPos = lookTarget.transform.localPosition + new Vector3(x, y, z);
        lookTarget.transform.localPosition = targetPos;
    }

    private void LedgeGrab()
    {
        if (!isGrabbingLedge)
        {
            RaycastHit downHit;
            Vector3 lineDownStart = (transform.position + Vector3.up * 1.7f) + transform.forward;
            Vector3 lineDownEnd = (transform.position + Vector3.up * 0.9f) + transform.forward;
            Physics.Linecast(lineDownStart, lineDownEnd, out downHit, LayerMask.GetMask("Grass"));
            Debug.DrawLine(lineDownStart, lineDownEnd);

            if (downHit.collider != null)
            {
                RaycastHit frontHit;
                Vector3 lineFrontStart = new Vector3(transform.position.x, downHit.point.y - 0.1f, transform.position.z);
                Vector3 lineFrontEnd = new Vector3(transform.position.x, downHit.point.y - 0.1f, transform.position.z) + transform.forward;
                Physics.Linecast(lineFrontStart, lineFrontEnd, out frontHit, LayerMask.GetMask("Grass"));
                Debug.DrawLine(lineFrontStart, lineFrontEnd);

                if (frontHit.collider != null)
                {
                    velocity.y = 0f;
                    isGrabbingLedge = true;

                    Vector3 hangPosition = new Vector3(frontHit.point.x, downHit.point.y, frontHit.point.z);
                    Vector3 offset = transform.forward * -0.1f + transform.up * -1f;
                    hangPosition += offset;
                    transform.position = hangPosition;
                    transform.forward = -frontHit.normal;
                    playermodelAnimation.gameObject.GetComponent<PlayerAnimator>().PlayGrabLedge();
                }
            }
        }

        if (isGrabbingLedge) // Pull Ledge once grabbed
            StartCoroutine(PullLedge(0.5f));
    }

    IEnumerator PullLedge(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        velocity.y = jumpHeight / 2f;
        yield return new WaitForSeconds(waitTime*2f);
        isGrabbingLedge = false;
    }

    private void ClimbLadder()
    {
        if (isClimbing) // Climb the ladder
        {
            float sideways = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            velocity.x = sideways * 4f;
            velocity.y = vertical * 4f;

            if (velocity.y > 0)
                playermodelAnimation.GetComponent<PlayerAnimator>().PlayClimbLadder(1f);
            else if (velocity.y < 0)
                playermodelAnimation.GetComponent<PlayerAnimator>().PlayClimbLadder(-1f);
            else
                playermodelAnimation.GetComponent<PlayerAnimator>().PlayClimbLadder(0f);
        }
        else
            playermodelAnimation.GetComponent<PlayerAnimator>().PlayGetOffLadder();
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public bool IsMoving()
    {
        if (moveDirection.magnitude >= 0.1f)
            return true;
        return false;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public bool IsCrouching()
    {
        return isCrouching;
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    public bool IsAiming()
    {
        return isAiming;
    }

    public bool IsInWater()
    {
        return isInWater;
    }

    public bool IsAttachedToWall()
    {
        return isAttachedToWall;
    }

    public void EnterOrExitWater(bool inWater)
    {
        isInWater = inWater;
        playermodelAnimation.GetComponent<PlayerAnimator>().StartSwimming();
    }

    public bool IsUsingSpecial()
    {
        return isUsingSpecial;
    }

    public void SetUsingSpecialStatus(bool special)
    {
        isUsingSpecial = special;
    }

    public bool IsTalking()
    {
        return isTalking;
    }

    public void SetIsTalking(bool talking)
    {
        isTalking = talking;
    }

    public bool IsFlying()
    {
        return isFlying;
    }

    public bool IsClimbing()
    {
        return isClimbing;
    }
    public void StartStopClimbing(bool climb)
    {
        isClimbing = climb;
    }

}
