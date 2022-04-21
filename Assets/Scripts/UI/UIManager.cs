using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {
	[SerializeField] private TMP_Text _shipsCreatedText;
	[SerializeField] private TMP_Text _shipsSpawnedText;
	[SerializeField] private TMP_Text _shipsDestroyedText;
	[SerializeField] private TMP_Text _activeShipsText;
	[SerializeField] private TMP_Text _roundsText;
	[SerializeField] private TMP_Text _pauseButtonText;
	[SerializeField] private Image _pauseBackground;
	[SerializeField] private TMP_Text _gameModeText;

	private int _shipsCreatedCounter = 0;
	private int _shipsSpawnedCounter = 0;
	private int _shipsDestroyedCounter = 0;
	private int _activeShipsCounter;
	private int _roundsCounter = 0;

	private void OnEnable() {
		ShipSpawner.OnShipCreated += () => IncreaseShipCount(ref _shipsCreatedCounter, _shipsCreatedText, "Ships Created: ");
		ShipSpawner.OnShipSpawned += () => IncreaseShipCount(ref _shipsSpawnedCounter, _shipsSpawnedText, "Ships Spawned: ");
		ShipSpawner.OnRoundSpawnsFinished += (roundShipsCount) => CacheActiveShipsCount(roundShipsCount);
		ShipSpawner.OnNewRound += () => IncreaseRounds();
		Bullet.OnShipDestroyed += () => ShipDestroyed();
		GameManager.OnGamePausedOrResumed += (isPaused) => HandlePauseState(isPaused);
		GameManager.OnGameModeChanged += () => _gameModeText.text = "MODE: " + GameManager.GameMode.ToString().ToUpper();
	}

	private void OnDisable() {
		ShipSpawner.OnShipCreated -= () => IncreaseShipCount(ref _shipsCreatedCounter, _shipsCreatedText, "Ships Created: ");
		ShipSpawner.OnShipSpawned -= () => IncreaseShipCount(ref _shipsSpawnedCounter, _shipsSpawnedText, "Ships Spawned: ");
		ShipSpawner.OnRoundSpawnsFinished -= (roundShipsCount) => CacheActiveShipsCount(roundShipsCount);
		ShipSpawner.OnNewRound -= () => IncreaseRounds();
		Bullet.OnShipDestroyed -= () => ShipDestroyed();
		GameManager.OnGamePausedOrResumed -= (isPaused) => HandlePauseState(isPaused);
		GameManager.OnGameModeChanged -= () => _gameModeText.text = "MODE: " + GameManager.GameMode.ToString().ToUpper();
	}

	private void IncreaseShipCount(ref int shipCounter, TMP_Text text, string phrase) {
		shipCounter++;
		UpdateText(text, phrase, shipCounter);
	}

	private void CacheActiveShipsCount(int roundShipsCount) {
		_activeShipsCounter = roundShipsCount;
		UpdateText(_activeShipsText, "Active Ships: ", _activeShipsCounter);
	}

	private void ShipDestroyed() {
		IncreaseShipCount(ref _shipsDestroyedCounter, _shipsDestroyedText, "Ships Destroyed: ");
		DecreaseActiveShipsCount();
	}

	private void DecreaseActiveShipsCount() {
		_activeShipsCounter--;
		UpdateText(_activeShipsText, "Active Ships: ", _activeShipsCounter);
	}

	private void UpdateText(TMP_Text text, string phrase) => text.text = phrase;

	private void UpdateText(TMP_Text text, string phrase, int number) => text.text = phrase + number;

	private void IncreaseRounds() {
		_roundsCounter++;
		UpdateText(_roundsText, "Round: ", _roundsCounter);
	}

	private void HandlePauseState(bool isGamePaused) {
		if (isGamePaused)
			UpdateText(_pauseButtonText, "RESUME");
		else
			UpdateText(_pauseButtonText, "PAUSE");

		ShowPauseBackground(isGamePaused);
	}

	private void ShowPauseBackground(bool isGamePaused) {
		if (_pauseBackground != null)
			_pauseBackground.enabled = isGamePaused;
	}
}
