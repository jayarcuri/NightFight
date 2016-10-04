﻿using System;
using System.Collections;
using NUnit.Framework;

public class CharacterManagerTests {

	[Test]
	public void BackwardStepTest () {
		CharacterManager tester = new CharacterManager ();
		tester.GetCurrentFrame (DirectionalInput.Right, AttackType.None);
		Assert.IsNotNull (tester.currentMove);
	}

	[Test]
	public void LightAttackTest () {
		CharacterManager tester = new CharacterManager ();
		tester.GetCurrentFrame (DirectionalInput.Neutral, AttackType.Light);
		Assert.IsNotNull (tester.currentMove);
	}

	[Test]
	public void TestCancellableJumpAttack () {
		CharacterManager tester = new CharacterManager ();

		tester.GetCurrentFrame (new DirectionalInput(8), AttackType.None);

		for (int i = 0; i < 2; i++) {
			tester.GetCurrentFrame (DirectionalInput.Neutral, AttackType.None);
		}
		MoveFrame currentFrame = tester.GetCurrentFrame (DirectionalInput.Neutral, AttackType.None);
		Assert.IsTrue (MoveType.AIRBORNE.Equals (currentFrame.moveType));
		// Make jump get enqueued
		// test that jump is currently occurring.
		tester.GetCurrentFrame (DirectionalInput.Neutral, AttackType.Light);

		// Make jump attack become enqueued
		// test that attack is actually enqueued (hit frame on appropriate frame)
	}

}
