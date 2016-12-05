﻿using System;
using System.Collections;
using NUnit.Framework;

public class CharacterDataManagerTests {

	[Test]
	public void BackwardStepTest () {
		CharacterDataManager tester = new CharacterDataManager ();
		bool x = false;
		tester.GetCurrentFrame (DirectionalInput.Right, AttackType.NONE, out x);
		Assert.IsNotNull (tester.currentMove);
	}

	[Test]
	public void LightAttackTest () {
		CharacterDataManager tester = new CharacterDataManager ();
		bool x = false;
		tester.GetCurrentFrame (DirectionalInput.Neutral, AttackType.LIGHT, out x);
		Assert.IsNotNull (tester.currentMove);
	}

	[Test]
	public void TestCancellableJumpAttack () {
		CharacterDataManager tester = new CharacterDataManager ();
		bool x = false;

		tester.GetCurrentFrame (new DirectionalInput(8), AttackType.NONE, out x);

		for (int i = 0; i < 2; i++) {
			tester.GetCurrentFrame (DirectionalInput.Neutral, AttackType.NONE, out x);
		}
		MoveFrame currentFrame = tester.GetCurrentFrame (DirectionalInput.Neutral, AttackType.NONE, out x);
		Assert.IsTrue (MoveType.AIRBORNE.Equals (currentFrame.moveType));
		// Make jump get enqueued
		// test that jump is currently occurring.
		tester.GetCurrentFrame (DirectionalInput.Neutral, AttackType.LIGHT, out x);
		Console.WriteLine ((char)AttackType.LIGHT);
		// Make jump attack become enqueued
		// test that attack is actually enqueued (hit frame on appropriate frame)
	}

}
