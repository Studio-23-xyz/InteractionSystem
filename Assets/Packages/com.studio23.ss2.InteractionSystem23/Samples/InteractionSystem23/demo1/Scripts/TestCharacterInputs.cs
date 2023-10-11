using UnityEngine;
using UnityEngine.InputSystem;

namespace com.studio23.ss2.InteractionSystem23.Samples.Demo1
{
	public class TestCharacterInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 _move;
		public Vector2 _look;
		public bool _jump;
		public bool _sprint;

		 [Header("Movement Settings")]
		public bool _analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool _cursorLocked = true; 
		public bool _cursorInputForLook = true;

		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(_cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}


		public void MoveInput(Vector2 newMoveDirection)
		{
			_move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			_look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			_jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			_sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(_cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}
