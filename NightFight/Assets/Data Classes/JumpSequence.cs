﻿using UnityEngine;
using System.Collections;

public class JumpSequence {
	int frameNumber;
	int moveLength;
	MoveSequence[] supplementaryMove;
	Vector2 velocity;
	double currentHeight;
	bool isFalling;
	//Vector2 currentVelocity;
	float gravity;

	public JumpSequence(int jumpLengthInFrames, float jumpHeight, float horizontalDistanceCovered) {
		//currentVelocity = initialVelocity;

		moveLength = jumpLengthInFrames;
		gravity = jumpHeight / (jumpLengthInFrames / 2);
		this.velocity = new Vector2 (gravity * (jumpLengthInFrames / 2), horizontalDistanceCovered / jumpLengthInFrames);
		//this.currentVelocity = initialVelocity;
		frameNumber = 0;
		isFalling = false;
		supplementaryMove = null;
//			new Vector2 (jumpHeight / jumpLengthInFrames, horizontalDistanceCovered / jumpLengthInFrames);
	}

	public MoveFrame GetNext() {
		return null;
	}

	public bool HasNext() {
		return (moveLength == frameNumber);
	}
}
