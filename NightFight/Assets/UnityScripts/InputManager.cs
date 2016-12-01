﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Eppy;

public class InputManager : MonoBehaviour
{
	public string horizontalAxis;
	public string verticalAxis;
	public string lightAttack;
	bool lightAttackPressed;
	public string heavyAttack;
	bool heavyAttackPressed;
	public string illuminateButton;
	bool illuminateButtonPressed;
	public string block;

	Tuple<string, AttackType>[] buttons;

	// Inputs are represented by an enum which corresponds with an int

	void Start () {
		buttons = new Tuple<string, AttackType>[] {
			new Tuple<string, AttackType>(heavyAttack, AttackType.Heavy),
			new Tuple<string, AttackType>(lightAttack, AttackType.Light),
			new Tuple<string, AttackType>(block, AttackType.Block)
		};
	}

	public void GetInputs (out DirectionalInput directionalInput, out AttackType attack, out bool toggleLight)
	{
		// read directional inputs
		int horizontal = (int)Input.GetAxisRaw (horizontalAxis);
		int vertical = (int)Input.GetAxisRaw (verticalAxis) * -1;

		directionalInput = new DirectionalInput (horizontal, vertical);

		if (Input.GetButton (heavyAttack) && !heavyAttackPressed) {
			heavyAttackPressed = true;
			attack = AttackType.Heavy;
		} else if (Input.GetButton (lightAttack) && !lightAttackPressed) {
			if (Input.GetButton (block)) {
				lightAttackPressed = true;
				attack = AttackType.Throw;
			} else {
				lightAttackPressed = true;
				attack = AttackType.Light;
			}
		} else if (Input.GetButton (block)) {
			attack = AttackType.Block;
		} else {
			attack = AttackType.None;
		}

		if (Input.GetButton (illuminateButton) && !illuminateButtonPressed) {
			illuminateButtonPressed = true;
			toggleLight = true;
		} else {
			toggleLight = false;
		}

		RecordDisengagedButtons ();
	}

	void RecordDisengagedButtons ()
	{
		if (!Input.GetButton (heavyAttack)) {
			heavyAttackPressed = false;
		}
		if (!Input.GetButton (lightAttack)) {
			lightAttackPressed = false;
		}
		if (!Input.GetButton (illuminateButton)) {
			illuminateButtonPressed = false;
		}

	}

	public void NewGetInputs (out ButtonInput[] activeButtons, out bool toggleLight)
	{
		// read directional inputs
		List<ButtonInput> activeButtonList = new List<ButtonInput> ();
		/*
		int horizontal = (int)Input.GetAxisRaw (horizontalAxis);
		int vertical = (int)Input.GetAxisRaw (verticalAxis);

		directionalInput = new DirectionalInput (horizontal, vertical);
		*/

		foreach (Tuple<string, AttackType> button in buttons) {
			ButtonState buttonState = GetCurrentButtonStateForButton (button.Item1);
			if (buttonState != ButtonState.none) {
				activeButtonList.Add (new ButtonInput(button.Item2, buttonState));
			}
		}

		if (Input.GetButton (illuminateButton) && !illuminateButtonPressed) {
			illuminateButtonPressed = true;
			toggleLight = true;
		} else {
			if (!Input.GetButton (illuminateButton)) {
				illuminateButtonPressed = false;
			}
			toggleLight = false;
		}

		activeButtons = activeButtonList.ToArray ();
	}

	ButtonState GetCurrentButtonStateForButton(string buttonName) {
		if (Input.GetButtonDown (buttonName)) {
			return ButtonState.depressed;
		} else if (Input.GetButtonUp (buttonName)) {
			return ButtonState.released;
		} else if (Input.GetButton (buttonName)) {
			return ButtonState.sustained;
		} else {
			return ButtonState.none;
		}
	}
}
