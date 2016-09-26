﻿using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
	public string horizontalAxis;
	public string verticalAxis;
	public string lightAttack;
	public string heavyAtack;

	// Inputs are represented by an enum which corresponds with an int 

	public void GetInputs (out DirectionalInput directionalInput, out AttackType attack) {
		// read directional inputs
		int horizontal =  (int)Input.GetAxisRaw (horizontalAxis);
		int vertical = (int)Input.GetAxisRaw (verticalAxis);

		directionalInput = new DirectionalInput(horizontal, vertical);

		if (Input.GetButtonDown (heavyAtack)) {
			attack = AttackType.Heavy;
		} // Light uses GetButton to allow for chaining jabs
		else if (Input.GetButton (lightAttack)) {
			attack = AttackType.Light;
		} else
			attack = AttackType.None;
	}
}