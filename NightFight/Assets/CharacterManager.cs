﻿using UnityEngine;
using System.Collections;
using System;

public class CharacterManager {
	CharacterState characterState;
	CharacterData characterData;
	public MoveSequence currentMove;

	public CharacterManager() {
		characterData = new CharacterData ();
		characterState = new CharacterState (characterData.maxHealth);

		currentMove = null;
	}

	public MoveFrame GetCurrentFrame(DirectionalInput directionalInput, AttackType attackType) {
		MoveFrame currentMoveFrame;
		ResolveInput (directionalInput, attackType);

		currentMoveFrame = currentMove != null && currentMove.HasNext () ? currentMove.GetNext () : null;
			
		return currentMoveFrame;
	}

	public bool HasQueuedFrames() {
		return currentMove != null && currentMove.HasNext ();
	}

	void ResolveInput(DirectionalInput directionalInput, AttackType attackType) {
		MoveFrame currentFrame;
		if (currentMove != null && currentMove.HasNext ()) {
				currentFrame = currentMove.Peek ();
		} else {
			currentFrame = null;
		}
		MoveSequence newMove = characterData.GetNewMove (characterState.GetCurrentAction(), currentFrame, directionalInput, attackType);

		if (newMove != null) {
			currentMove = newMove;
		}
	}



}
