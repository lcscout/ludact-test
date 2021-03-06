using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {
	[Header("Total Info UI")]
	[SerializeField] private TMP_Text _shipsCreatedText;
	[SerializeField] private TMP_Text _shipsSpawnedText;
	[SerializeField] private TMP_Text _shipsDestroyedText;

	[Header("Round Info UI")]
	[SerializeField] private TMP_Text _shipsInRoundText;
	[SerializeField] private TMP_Text _activeShipsText;
	[SerializeField] private TMP_Text _roundsText;

	[Header("Pause UI")]
	[SerializeField] private TMP_Text _pauseButtonText;
	[SerializeField] private Image _pauseBackground;
	[SerializeField] private List<Button> _buttonsToDisableDuringPause = new List<Button>();

	[Header("Other UI")]
	[SerializeField] private TMP_Text _gameModeText;
	[SerializeField] private TMP_Text _soundStateText;

	private int _shipsCreatedCounter = 0;
	private int _shipsSpawnedCounter = 0;
	private int _shipsDestroyedCounter = 0;
	private int _shipsInRoundCounter = 0;
	private int _activeShipsCounter = 0;
	private int _roundsCounter = 0;

	private void OnEnable() {
		ShipSpawner.OnShipCreated += () => IncreaseShipCount(ref _shipsCreatedCounter, _shipsCreatedText, "Ships Created: ");
		ShipSpawner.OnShipSpawned += () => ShipSpawned();
		ShipSpawner.OnNewRound += () => NewRound();
		Ship.OnShipDestroyed += (lastPosition) => ShipDestroyed();
		GameManager.OnGamePausedOrResumed += (isPaused) => HandlePauseState(isPaused);
		GameManager.OnGameModeChanged += () => UpdateText(_gameModeText, "MODE: " + GameManager.GameMode.ToString().ToUpper());
		GameManager.OnSoundStateChanged += () => HandleSoundState();
	}

	private void OnDisable() {
		ShipSpawner.OnShipCreated -= () => IncreaseShipCount(ref _shipsCreatedCounter, _shipsCreatedText, "Ships Created: ");
		ShipSpawner.OnShipSpawned -= () => ShipSpawned();
		ShipSpawner.OnNewRound -= () => NewRound();
		Ship.OnShipDestroyed -= (lastPosition) => ShipDestroyed();
		GameManager.OnGamePausedOrResumed -= (isPaused) => HandlePauseState(isPaused);
		GameManager.OnGameModeChanged -= () => UpdateText(_gameModeText, "MODE: " + GameManager.GameMode.ToString().ToUpper());
		GameManager.OnSoundStateChanged -= () => HandleSoundState();
	}

	public void PlayClickSound() => AudioManager.Instance.PlaySoundOneShot(1, 1);

	private void IncreaseShipCount(ref int shipCounter, TMP_Text text, string phrase) {
		shipCounter++;
		UpdateText(text, phrase, shipCounter);
	}

	private void DecreaseShipCount(ref int shipCounter, TMP_Text text, string phrase) {
		shipCounter--;
		UpdateText(text, phrase, shipCounter);
	}

	private void UpdateText(TMP_Text text, string phrase) => text.text = phrase;

	private void UpdateText(TMP_Text text, string phrase, int number) => text.text = phrase + number;

	private void ShipSpawned() {
		IncreaseShipCount(ref _shipsSpawnedCounter, _shipsSpawnedText, "Ships Spawned: ");
		IncreaseShipCount(ref _shipsInRoundCounter, _shipsInRoundText, "Ships in Round: ");
		IncreaseShipCount(ref _activeShipsCounter, _activeShipsText, "Active Ships: ");
	}

	private void NewRound() {
		ResetRoundVariables();

		_roundsCounter++;
		UpdateText(_roundsText, "Round: ", _roundsCounter);
	}

	private void ResetRoundVariables() {
		_activeShipsCounter = 0;
		_shipsInRoundCounter = 0;
	}

	private void ShipDestroyed() {
		IncreaseShipCount(ref _shipsDestroyedCounter, _shipsDestroyedText, "Ships Destroyed: ");
		DecreaseShipCount(ref _activeShipsCounter, _activeShipsText, "Active Ships: ");
	}

	private void HandlePauseState(bool isGamePaused) {
		if (isGamePaused)
			UpdateText(_pauseButtonText, "RESUME");
		else
			UpdateText(_pauseButtonText, "PAUSE");

		ShowPauseBackground(isGamePaused);

		DisableButtonsDuringPause(isGamePaused);
	}

	private void ShowPauseBackground(bool isGamePaused) {
		if (_pauseBackground != null)
			_pauseBackground.enabled = isGamePaused;
	}

	private void HandleSoundState() {
		switch(GameManager.SoundState) {
			case GameManager.Sound.MuteSFX:
				AudioManager.Instance.MuteSFX();
				UpdateText(_soundStateText, "MUTE ALL");
				break;
			case GameManager.Sound.MuteAll:
				AudioManager.Instance.MuteAll();
				UpdateText(_soundStateText, "UNMUTE ALL");
				break;
			case GameManager.Sound.UnmuteAll:
				AudioManager.Instance.UnmuteAll();
				UpdateText(_soundStateText, "MUTE SFX");
				break;
		}
	}

	private void DisableButtonsDuringPause(bool isGamePaused) {
		foreach (Button button in _buttonsToDisableDuringPause)
			if (button != null)
				button.interactable = !isGamePaused;
	}
}
