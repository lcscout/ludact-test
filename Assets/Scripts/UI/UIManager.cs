using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {
	[SerializeField] private TMP_Text _shipsCreatedText;
	[SerializeField] private TMP_Text _shipsSpawnedText;
	[SerializeField] private TMP_Text _activeShipsText;
	[SerializeField] private TMP_Text _roundsText;
	[SerializeField] private TMP_Text _pauseButtonText;
	[SerializeField] private Image _pauseBackground;

	private int _shipsCreatedCounter = 0;
	private int _shipsSpawnedCounter = 0;
	private int _activeShipsCounter;
	private int _roundsCounter = 0;

	private void OnEnable() {
		ShipSpawner.OnShipCreated += () => IncreaseShips(ref _shipsCreatedCounter, _shipsCreatedText, "Ships Created: ");
		ShipSpawner.OnShipSpawned += () => IncreaseShips(ref _shipsSpawnedCounter, _shipsSpawnedText, "Ships Spawned: ");
		ShipSpawner.OnRoundSpawnsFinished += (roundShipsCount) => CacheActiveShipsCount(roundShipsCount);
		ShipSpawner.OnNewRound += () => IncreaseRounds();
		Bullet.OnShipDestroyed += () => DecreaseActiveShipsCount();
		GameManager.OnGamePausedOrResumed += (isPaused) => HandlePauseState(isPaused);
	}

	private void OnDisable() {
		ShipSpawner.OnShipCreated -= () => IncreaseShips(ref _shipsCreatedCounter, _shipsCreatedText, "Ships Created: ");
		ShipSpawner.OnShipSpawned -= () => IncreaseShips(ref _shipsSpawnedCounter, _shipsSpawnedText, "Ships Spawned: ");
		ShipSpawner.OnRoundSpawnsFinished -= (roundShipsCount) => CacheActiveShipsCount(roundShipsCount);
		ShipSpawner.OnNewRound -= () => IncreaseRounds();
		Bullet.OnShipDestroyed -= () => DecreaseActiveShipsCount();
		GameManager.OnGamePausedOrResumed += (isPaused) => HandlePauseState(isPaused);
	}

	private void IncreaseShips(ref int shipCounter, TMP_Text text, string phrase) {
		shipCounter++;
		UpdateText(text, phrase, shipCounter);
	}

	private void CacheActiveShipsCount(int roundShipsCount) {
		_activeShipsCounter = roundShipsCount;
		UpdateText(_activeShipsText, "Active Ships: ", _activeShipsCounter);
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

	private void ShowPauseBackground(bool isGamePaused) => _pauseBackground.enabled = isGamePaused;
}
