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
		public bool interact;
		public bool drop;
        public bool pause;

        //ingame
        public bool resume;
        public bool save;
        public bool load;
        public bool quit;

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

        //interact
        public void OnInteract(InputValue value)
        {
			InpteractInput(value.isPressed);
        }

        public void InpteractInput(bool newInteractState)
        {
            interact = newInteractState;
        }

        //drop
        public void OnDrop(InputValue value)
        {
            DropInput(value.isPressed);
        }

        public void DropInput(bool newDropState)
        {
            drop = newDropState;
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

        //pause
        public void OnPause(InputValue value)
        {
            PauseInput(value.isPressed);
        }
        public void PauseInput(bool newPauseState)
        {
            pause = newPauseState;
        }

        //resume
        public void OnResume(InputValue value)
        {
            ResumeInput(value.isPressed);
        }
        public void ResumeInput(bool newResumeState)
        {
            resume = newResumeState;
        }

        //save
        public void OnSave(InputValue value)
        {
            SaveInput(value.isPressed);
        }
        public void SaveInput(bool newSaveState)
        {
            save = newSaveState;
        }

        //load
        public void OnLoad(InputValue value)
        {
            LoadInput(value.isPressed);
        }
        public void LoadInput(bool newLoadState)
        {
            load = newLoadState;
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