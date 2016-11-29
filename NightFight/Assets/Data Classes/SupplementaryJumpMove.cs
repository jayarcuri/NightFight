﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class SupplementaryJumpMove : MoveSequence
	{
	protected MoveFrame finalRepeatingFrame;

	public SupplementaryJumpMove (MoveFrame[] moveSequence, AttackType mappedToButton) 
		: base (moveSequence, mappedToButton) {
		finalRepeatingFrame = new MoveFrame (Vector2.zero, MoveType.RECOVERY, true);
	}

	public static SupplementaryJumpMove GetSupplementaryJumpMoveWithFrameData(int startUp, int activeFrames, AttackFrameData attackData, AttackType mappedToButton) {
		SupplementaryJumpMove newJumpMove = new SupplementaryJumpMove(new MoveFrame[0], mappedToButton);
		newJumpMove.PopulateMoveSequenceUsingFrameData (startUp, activeFrames, 0, attackData);
		newJumpMove.finalRepeatingFrame = new MoveFrame (Vector2.zero, MoveType.RECOVERY, true);

		return newJumpMove;
	}

	public override MoveFrame GetNext ()
	{
		MoveFrame nextFrame;
		if (base.HasNext ()) {
			nextFrame = base.GetNext ();
		} else {
			nextFrame = finalRepeatingFrame;
		}

		return nextFrame;
	}

	public override MoveFrame Peek ()
	{
		MoveFrame peekFrame;
		if (base.HasNext ()) {
			peekFrame = base.Peek ();
		} else {
			peekFrame = finalRepeatingFrame;
		}

		return peekFrame;
	}

	public override bool HasNext ()
	{
		return true;
	}

}

