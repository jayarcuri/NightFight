﻿using UnityEngine;
using System.Collections;

public class MoveSequence : FrameSequence {
	int index;
	MoveFrame[] moveSequence;

	public MoveSequence(MoveFrame[] moveSequence) {
		index = -1;
		this.moveSequence = moveSequence;
	}

	public virtual MoveFrame GetNext() {
		if(HasNext()) {
			++index;
			return moveSequence [index];
			}
		else
			throw new System.IndexOutOfRangeException("Current move sequence does not have a next move!");
		}

	public virtual MoveFrame Peek () {
		if (HasNext()) {
			return moveSequence [index+1];
		}
		else
			throw new System.IndexOutOfRangeException("Current move sequence does not have a next move!");
	}

	public virtual bool HasNext() {
		return (index < moveSequence.Length - 1);
	}

	public virtual MoveFrame GetPrevious() {
		MoveFrame returnMove;

		if (index == -1)
			returnMove = null;
		else
			returnMove = moveSequence [index - 1];
		return returnMove;
	}
	// appears to exist only to test; consider deleting
	public virtual int MoveLength () {
		return moveSequence.Length;
	}

	public virtual void Reset() {
		index = -1;
	}


}
