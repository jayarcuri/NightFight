﻿using UnityEngine;
using System.Collections;

public class JumpSequence : FrameSequence {
	MoveSequence[] supplementaryMove;
	Vector2 velocity;
	double maxJumpHeight;

	public double currentHeight { get; private set; }
	public bool isFalling { get; private set; }

	public JumpSequence(int jumpLengthInFrames, double jumpHeight, double horizontalDistanceCovered) {
		SetUp ();
		this.maxJumpHeight = jumpHeight;
		this.velocity = new Vector2 ((float) jumpHeight * 2 / jumpLengthInFrames, (float) horizontalDistanceCovered / jumpLengthInFrames);
	}

	public MoveFrame GetNext() {
		if (HasNext ()) {
			currentHeight += velocity.x;

			if (currentHeight >= maxJumpHeight) {
				isFalling = true;
				velocity = new Vector2 (-velocity.x, velocity.y);
			}

			return new MoveFrame (velocity, MoveType.AIRBORNE);
		} else {
			throw new System.IndexOutOfRangeException("Current sequence does not have a next move!");
		}
	}

	public bool HasNext() {
		return (!isFalling || currentHeight > 0);
	}

	public MoveFrame Peek () {
		MoveFrame peekFrame;

		peekFrame = HasNext () ? new MoveFrame (velocity, MoveType.AIRBORNE) : null;

		return peekFrame; 
	}

	public int MoveLength () {
		return -1;
	}

	public void Reset () {
		if (isFalling) {
			velocity = new Vector2 (-velocity.x, velocity.y);
		}
		SetUp ();
	}

	public void AddSupplimentaryFrameSequence (FrameSequence newSequence) {
		return;
	}

	void SetUp () {
		supplementaryMove = null;
		isFalling = false;
		currentHeight = 0;
	}
		
}
