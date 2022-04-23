using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public static event Action<bool> OnGamePausedOrResumed;
	public static event Action OnGameModeChanged;
	public static event Action OnSoundStateChanged;

	public static bool IsGamePaused { get; private set; }
	public static Mode GameMode { get; private set; }
	public static Sound SoundState { get; private set; }

	public enum Mode {
		Confined,
		Free
	}

	public enum Sound {
		MuteSFX,
		MuteAll,
		UnmuteAll
	}

	[Header("Components Reference")]
	[SerializeField] private EdgeCollider2D _cameraBounds;

	[Header("Settings")]
	[Tooltip("The targeted fps limit")]
	[SerializeField] private int _targetFrameRate = 60;

	private Sound _lastSoundState;

	private void Awake() {
		ResetGameToInitialState();
		LimitFrameRate();
	}

	public void PauseAndResume() {
		IsGamePaused = !IsGamePaused;

		ChangeTimeScale(IsGamePaused ? 0f : 1f);
		OnGamePausedOrResumed?.Invoke(IsGamePaused);

		MuteSoundDuringPause();
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

	public void ChangeSoundState() {
		SoundState++;
		if ((int)SoundState > 2)
			SoundState = Sound.MuteSFX;

		OnSoundStateChanged?.Invoke();
	}

	private void ChangeSoundState(Sound soundState) {
		SoundState = soundState;
		OnSoundStateChanged?.Invoke();
	}

	private void ChangeTimeScale(float scale) => Time.timeScale = scale;

	private void ResetGameToInitialState() {
		IsGamePaused = false;
		GameMode = Mode.Confined;
		Time.timeScale = 1f;
		SoundState = Sound.UnmuteAll;

		if (_cameraBounds != null)
			_cameraBounds.isTrigger = false;
	}

	private void LimitFrameRate() {
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = _targetFrameRate;
	}

	private void MuteSoundDuringPause() {
		if (IsGamePaused) {
			_lastSoundState = SoundState;
			ChangeSoundState(Sound.MuteAll);
		}
		else
			ChangeSoundState(_lastSoundState);
	}
}
