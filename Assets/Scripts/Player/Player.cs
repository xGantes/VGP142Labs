using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace VGP142.PlayerInputs
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class Player : MonoBehaviour
    {
        [Header("Player")]
        public float MoveSpeed = 2.0f;
        public float SprintSpeed = 5.335f;
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;

        [Space(10)]
        public float JumpTimeout = 0.50f;
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.28f;
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 70.0f;
        public float BottomClamp = -30.0f;
        public float CameraAngleOverride = 0.0f;
        public bool LockCameraPosition = false;

        [Header("Health")]
        public int maxHealth = 100;
        public int currentHealth;
        private bool isDying;
        float deathTimer = 0.0f;
        const float waitingTime = 4.0f;

        private bool isAttacking;

        // cinemachine
        private float cinemachineTargetYaw;
        private float cinemachineTargetPitch;

        // player
        private float speed;
        private float animationBlend;
        private float targetRotation = 0.0f;
        private float rotationVelocity;
        private float verticalVelocity;
        private float terminalVelocity = 53.0f;

        // timeout deltatime
        private float jumpTimeoutDelta;
        private float fallTimeoutDelta;

        // animation IDs
        private int animIDSpeed;
        private int animIDGrounded;
        private int animIDJump;
        private int animIDFreeFall;
        private int animIDMotionSpeed;

        private PlayerInput playerInput;
        private Animator animator;
        private CharacterController controller;
        private MainPlayerInputs input;
        private GameObject mainCamera;

        private const float threshold = 0.01f;
        private bool hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
                return playerInput.currentControlScheme == "KeyboardMouse";
            }
        }


        private void Awake()
        {
            // get a reference to our main camera
            if (mainCamera == null)
            {
                mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            hasAnimator = TryGetComponent(out animator);
            controller = GetComponent<CharacterController>();
            input = GetComponent<MainPlayerInputs>();
            playerInput = GetComponent<PlayerInput>();

            AssignAnimationIDs();

            // reset our timeouts on start
            jumpTimeoutDelta = JumpTimeout;
            fallTimeoutDelta = FallTimeout;

            currentHealth = maxHealth;
        }

        private void Update()
        {
            hasAnimator = TryGetComponent(out animator);

            maxHealth = currentHealth;

            JumpAndGravity();
            GroundedCheck();
            Move();
            Attack();
            Die();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            animIDSpeed = Animator.StringToHash("Speed");
            animIDGrounded = Animator.StringToHash("Grounded");
            animIDJump = Animator.StringToHash("Jump");
            animIDFreeFall = Animator.StringToHash("FreeFall");
            animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        #region Control Region

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (hasAnimator)
            {
                animator.SetBool(animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (input.look.sqrMagnitude >= threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                cinemachineTargetYaw += input.look.x * deltaTimeMultiplier;
                cinemachineTargetPitch += input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + CameraAngleOverride,
                cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            if (!isAttacking && !isDying)
            {
                float targetSpeed = input.sprint ? SprintSpeed : MoveSpeed;
                if (input.move == Vector2.zero) targetSpeed = 0.0f;

                float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;
                float speedOffset = 0.1f;
                float inputMagnitude = input.analogMovement ? input.move.magnitude : 1f;

                if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
                {
                    // creates curved result rather than a linear one giving a more organic speed change
                    // note T in Lerp is clamped, so we don't need to clamp our speed
                    speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                        Time.deltaTime * SpeedChangeRate);

                    // round speed to 3 decimal places
                    speed = Mathf.Round(speed * 1000f) / 1000f;
                }
                else
                {
                    speed = targetSpeed;
                }
                animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
                if (animationBlend < 0.01f) animationBlend = 0f;

                Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;
                if (input.move != Vector2.zero)
                {
                    targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
                    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }
                Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

                controller.Move(targetDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
                if (hasAnimator)
                {
                    animator.SetFloat(animIDSpeed, animationBlend);
                    animator.SetFloat(animIDMotionSpeed, inputMagnitude);
                }
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (hasAnimator)
                {
                    animator.SetBool(animIDJump, false);
                    animator.SetBool(animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (verticalVelocity < 0.0f)
                {
                    verticalVelocity = -2f;
                }

                // Jump
                if (input.jump && jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (hasAnimator)
                    {
                        animator.SetBool(animIDJump, true);
                    }
                }

                // jump timeout
                if (jumpTimeoutDelta >= 0.0f)
                {
                    jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (fallTimeoutDelta >= 0.0f)
                {
                    fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (hasAnimator)
                    {
                        animator.SetBool(animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (verticalVelocity < terminalVelocity)
            {
                verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        #endregion

        #region Attack Region

        private void StartAttacking()
        {
            isAttacking = true;
        }
        private void StopAttacking()
        {
            isAttacking = false;
        }

        private void Attack()
        {
            if (Grounded)
            {
                if (input.attack)
                {
                    if (!isAttacking && !isDying)
                    {
                        StartAttacking();
                        animator.SetTrigger("Attacks");
                    }
                }
                input.attack = false;
            }
        }

        #endregion

        #region TakeDamage and Die

        public void OnTakeDamage(int amount)
        {
            currentHealth -= amount;
        }

        private void Die()
        {
            //trial death
            if (input.die)
            {
                OnTakeDamage(10);
                Debug.Log(currentHealth);
                input.die = false;
            }

            if (currentHealth <= 0)
            {
                animator.SetTrigger("Die");
                isDying = true;
            }
            if (isDying)
            {
                deathTimer += Time.deltaTime;
                if (deathTimer >= waitingTime)
                {
                    
                    //Destroy(gameObject);
                    SceneManager.LoadScene("GameOverScene");
                }
            }
        }

        #endregion


        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(controller.center), FootstepAudioVolume);
            }
        }
    }
}