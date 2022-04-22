using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public static event Action<bool> OnGamePausedOrResumed;
	public static event Action OnGameModeChanged;

	public static bool IsGamePaused { get; private set; }
	public static Mode GameMode { get; private set; }

	public enum Mode {
		Confined,
		Free
	}

	[Tooltip("The targeted fps limit")]
	[SerializeField] private int _targetFrameRate = 60;
	[SerializeField] private EdgeCollider2D _cameraBounds;

	private void Awake() {
		ResetGameState();
		LimitFrameRate();
	}

	public void PauseAndResume() {
		if (IsGamePaused)
			Time.timeScale = 1f;
		else
			Time.timeScale = 0f;

		IsGamePaused = !IsGamePaused;

		OnGamePausedOrResumed?.Invoke(IsGamePaused);
	}

	public void ChangeGameMode() {
		if (GameMode == Mode.Confined) {
			GameMode = Mode.Free;
			_cameraBounds.isTrigger = true;
		}
		else {
			GameMode = Mode.Confined;
			_cameraBounds.isTrigger = false;
		}

		OnGameModeChanged?.Invoke();
	}

	public void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

	public void SkipRound() {
		foreach(Ship ship in FindObjectsOfType<Ship>())
			ship.Kill();
	}

	private void ResetGameState() {
		IsGamePaused = false;
		GameMode = Mode.Confined;
		Time.timeScale = 1f;

		if (_cameraBounds != null)
			_cameraBounds.isTrigger = false;
	}

	private void LimitFrameRate() {
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = _targetFrameRate;
	}
}
