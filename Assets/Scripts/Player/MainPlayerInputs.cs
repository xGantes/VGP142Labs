using UnityEngine;
using UnityEngine.InputSystem;


namespace VGP142.PlayerInputs
{
	public class MainPlayerInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool attack;

		//trial
		public bool die;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		//move forward
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

		//yaw and pitch
        public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        //jump
        public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

		//attack
		public void OnAttack(InputValue value)
        {
			AttackInput(value.isPressed);
        }
		public void AttackInput(bool newFireState)
        {
			attack = newFireState;
        }

		//die trial
        public void OnDie(InputValue value)
        {
            DieInput(value.isPressed);
        }
        public void DieInput(bool newDieState)
        {
            die = newDieState;
        }

        //sprint
        public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

		//lock camera
		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}