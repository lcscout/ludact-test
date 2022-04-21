using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static event Action<bool> OnGamePausedOrResumed;

	public static bool IsGamePaused { get; private set; }

	[Tooltip("The targeted fps limit")]
	[SerializeField] private int _targetFrameRate = 60;

	private void Awake() => LimitFrameRate();

	public void PauseAndResume() {
		if (IsGamePaused)
			Time.timeScale = 1f;
		else
			Time.timeScale = 0f;

		IsGamePaused = !IsGamePaused;

		OnGamePausedOrResumed?.Invoke(IsGamePaused);
	}

	private void LimitFrameRate() {
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = _targetFrameRate;
	}
}
