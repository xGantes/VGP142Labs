using PlayerSettings;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerCameraInput
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
    public class PlayerCamera : MonoBehaviour
    {
        [Header("Cinemachine")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 70.0f;
        public float BottomClamp = -30.0f;
        public float CameraAngleOverride = 0.0f;
        public bool LockCameraPosition = false;

        // cine machine
        private float cinemachineTargetYaw;
        private float cinemachineTargetPitch;
        private const float threshold = 0.01f;

        private Inputs input;
        private Player player;
        private GameObject mainCamera;

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
            input = GetComponent<Inputs>();
        }

        private void LateUpdate()
        {
            CameraRotation();
            TargetRotation();
        }

        private void TargetRotation()
        {

            Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;

            if (input.move != Vector2.zero)
            {
                Player.targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, 
                Player.targetRotation, ref Player.rotationVelocity, Player.RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (input.look.sqrMagnitude >= threshold && !LockCameraPosition)
            {
                cinemachineTargetYaw += input.look.x * Time.deltaTime;
                cinemachineTargetPitch += input.look.y * Time.deltaTime;
            }

            // clamp our rotations so our values are limited 360 degrees
            cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + CameraAngleOverride, cinemachineTargetYaw, 0.0f);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

    }
}

