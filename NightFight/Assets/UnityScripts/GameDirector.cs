﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Eppy;

public class GameDirector : MonoBehaviour {
	public EndGameMenuController victoryWindowController;
	public GameTimer gameTimer;
	public HitEffectsManager shaker;

	public CharacterManager[] characters;

	//	Should be a float > 0f & <= 1f
	private float cornerPushBackModifier = 1f;

	void Start () {
		MovementCollisionUtils.SetUp ();
		victoryWindowController = GameObject.FindGameObjectWithTag ("VictoryWindow").GetComponent<EndGameMenuController> ();
		gameTimer = GameObject.FindGameObjectWithTag ("GameTimer").GetComponent<GameTimer> ();
		//stateManager = gameObject.GetComponent<GameStateManager> ();
		victoryWindowController.gameObject.SetActive (false);
		foreach (CharacterManager characterManager in characters) {
			if (characterManager == null) {
				throw new UnityException ("We are missing a player's character.");
			}
		}
		// GameStateManager.SetCurrentGameState (GameState.GAME_RUNNING);
		StartCoroutine (SetUpGame());
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		GameState currentState = GameStateManager.GetCurrentGameState ();

		if (currentState == GameState.GAME_RUNNING) {
			bool[] hitsOccurred = new bool[2];
			Tuple<MoveFrame, bool>[] currentFrames = new  Tuple<MoveFrame, bool>[2];
			Tuple<Vector2, Vector2> characterVelocities;

			//	Get what each player is trying to do this turn.
			for (int i = 0; i < characters.Length; i++) {
				currentFrames [i] = characters [i].GetCurrentFrame ();
			}
			characterVelocities = new Tuple<Vector2, Vector2> (currentFrames [0].Item1.movementDuringFrame, currentFrames [1].Item1.movementDuringFrame);
			//	Attempt to get modified character velocities
			Tuple<Vector2, Vector2> newVelocities = ResolveCharacterCollisions (currentFrames [0].Item1, currentFrames [1].Item1);

			if (!newVelocities.Item1.Equals (MovementCollisionUtils.NaV2)) {
				characterVelocities = newVelocities;
			}

			characters [0].ExecuteFrame (currentFrames [0].Item1, characterVelocities.Item1, currentFrames [0].Item2);
			characters [1].ExecuteFrame (currentFrames [1].Item1, characterVelocities.Item2, currentFrames [1].Item2);
			//	Resolve any attack which should be hitting a character right now.
			for (int i = 0; i < characters.Length; i++) {
				hitsOccurred [i] = characters [i].ResolveAttackCollisions ();
			}
			//	Update character states.
			for (int i = 0; i < characters.Length; i++) {
				characters [i].UpdateCharacterState ();
			}
			//	If a hit connected, alert HitEffects.
			if (hitsOccurred [0] || hitsOccurred [1]) {
				shaker.SetCameraToShake ();
				for (int i = 0; i < characters.Length; i++) {
					if (hitsOccurred [i]) {
						MoveType lastFrameMoveType = characters [i].GetLastFrameMoveType () == MoveType.BLOCKING 
							? MoveType.BLOCKING 
							: MoveType.IN_HITSTUN;
						characters [i].SetCharacterLight (true, lastFrameMoveType);
					}
				}
			}
			//	Check for a winner.
			CheckIfGameHasEnded ();
		} else if (currentState == GameState.HIT_SHAKE) {
			shaker.StepShakeForward ();
		}
	}

	Tuple<Vector2, Vector2> ResolveCharacterCollisions (MoveFrame player1Frame, MoveFrame player2Frame) {
		Transform player1Location = characters[0].gameObject.transform;
		Transform player2Location = characters[1].gameObject.transform;
		Vector2 player1Velocity = player1Frame.movementDuringFrame;
		Vector2 player2Velocity = player2Frame.movementDuringFrame;
		Vector2 newPlayer1Velocity = player1Velocity;
		Vector2 newPlayer2Velocity = player2Velocity;

		Tuple<Vector2, Vector2> newVelocities = MovementCollisionUtils.GetVelocitiesWithoutCharacterCollisions (player1Location, player1Velocity, player2Location, player2Velocity);

		newPlayer1Velocity = !newVelocities.Item1.Equals (MovementCollisionUtils.NaV2) ? newVelocities.Item1 : newPlayer1Velocity;
		newPlayer2Velocity = !newVelocities.Item2.Equals (MovementCollisionUtils.NaV2) ? newVelocities.Item2 : newPlayer2Velocity;

		Vector2 levelConstrainedP1Velocity = MovementCollisionUtils.GetLevelConstraintedVelocity (player1Location, player1Velocity);
		Vector2 levelConstrainedP2Velocity = MovementCollisionUtils.GetLevelConstraintedVelocity (player2Location, player2Velocity);
		bool p1VelocityChanged_X = levelConstrainedP1Velocity.x != newPlayer1Velocity.x && !float.IsNaN (levelConstrainedP1Velocity.x);
		bool p1VelocityChanged_Y = levelConstrainedP1Velocity.y != newPlayer1Velocity.y && !float.IsNaN (levelConstrainedP1Velocity.y);
		bool p2VelocityChanged_X = levelConstrainedP2Velocity.x != newPlayer2Velocity.x && !float.IsNaN (levelConstrainedP2Velocity.x);
		bool p2VelocityChanged_Y = levelConstrainedP2Velocity.y != newPlayer2Velocity.y && !float.IsNaN (levelConstrainedP2Velocity.y);

		newPlayer1Velocity = p1VelocityChanged_X || p1VelocityChanged_Y ? levelConstrainedP1Velocity : newPlayer1Velocity;
		newPlayer2Velocity = p2VelocityChanged_X || p2VelocityChanged_Y ? levelConstrainedP2Velocity : newPlayer2Velocity;

		if (p1VelocityChanged_X) {
			float newXVelocity = MovementCollisionUtils.GetNonOverlappingXVelocity(player1Location, newPlayer1Velocity, player2Location, newPlayer2Velocity);

			if (float.IsNaN (newXVelocity) && characters [0].IsBlockingOrHit ()) {
				newXVelocity = newPlayer2Velocity.x + (player1Velocity.x - newPlayer1Velocity.x) * cornerPushBackModifier;
			}
			if (!float.IsNaN (newXVelocity)) {
				newPlayer2Velocity = new Vector2 (newXVelocity, newPlayer2Velocity.y);
			}
		} else if (p2VelocityChanged_X) {
			float newXVelocity = MovementCollisionUtils.GetNonOverlappingXVelocity(player2Location, newPlayer2Velocity, player1Location, newPlayer1Velocity);

			if (float.IsNaN (newXVelocity) && characters [1].IsBlockingOrHit ()) {
				newXVelocity = newPlayer1Velocity.x + (player2Velocity.x - newPlayer2Velocity.x) * cornerPushBackModifier;
			}
			if (!float.IsNaN (newXVelocity)) {
				newPlayer1Velocity = new Vector2 (newXVelocity, newPlayer1Velocity.y);
			}
		}

		return new Tuple<Vector2, Vector2>(newPlayer1Velocity, newPlayer2Velocity);
	}

	public void RestartGame() {
		SceneManager.LoadScene (SceneManager.GetActiveScene().name);
		Time.timeScale = 1;
	}

	void CheckIfGameHasEnded () {
		int player1Health = characters [0].characterDataManager.GetCurrentHealth ();
		int player2Health = characters [1].characterDataManager.GetCurrentHealth ();
		if (player1Health <= 0 || player2Health <= 0 || gameTimer.secondsLeftInRound <= 0) {
			EndGameWithWinner (GetWinner (player1Health, player2Health));
		}
	}

	WinningPlayer GetWinner (int player1Health, int player2Health) {
		WinningPlayer winner;
		if (player1Health < 1 && player2Health < 1 || player1Health == player2Health) {
			winner = WinningPlayer.NONE;
		} else if (player1Health < player2Health) {
			winner = WinningPlayer.PLAYER_2;
		} else {
			winner = WinningPlayer.PLAYER_1;
		}
		return winner;
	}

	void EndGameWithWinner (WinningPlayer winner) {
		GameStateManager.SetCurrentGameState (GameState.GAME_OVER);
		victoryWindowController.gameObject.SetActive (true);
		victoryWindowController.SetVictoryTitleForWinner (winner);
	}

	IEnumerator SetUpGame () {
		while (GameStateManager.GetCurrentGameState () != GameState.GAME_RUNNING) {
			yield return null;
		}
		gameTimer.SetUpTimer ();
	}
}
