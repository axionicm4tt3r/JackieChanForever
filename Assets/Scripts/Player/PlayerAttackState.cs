public enum PlayerAttackState
{
	Idle,
	Blocking,
	Charging,
	JumpKicking,
	SlideKicking
}

//Set the state of the attack you are going to be doing
//If you wish to combo in and out of attacks, then let's store the player's attack request within the state
//If we allow them, we can transition states to the next part at the next available moment
//This will definitely require a refactor

//We may need a state machine for this instead