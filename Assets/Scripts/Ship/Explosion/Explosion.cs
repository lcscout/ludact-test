using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(AudioSource))]
public class Explosion : MonoBehaviour {
	private ParticleSystem _explosion;
	private AudioSource _audioSource;
	private Action<Explosion> _killAction;
	private GameManager.Sound _lastSoundState = GameManager.Sound.UnmuteAll;

	private void Awake() {
		_explosion = GetComponent<ParticleSystem>();
		_audioSource = GetComponent<AudioSource>();
	}

	private void Update() {
		if (!_explosion.isPlaying)
			_killAction(this);

		HandleSound();
	}

	public void Configure(Action<Explosion> killAction, Vector3 position) {
		_killAction = killAction;
		transform.position = position;
	}

	private void HandleSound() {
		if (_lastSoundState != GameManager.SoundState) // to avoid multiple calls
			CacheSoundState();
		else
			return;

		if (!(GameManager.SoundState == GameManager.Sound.UnmuteAll))
			_audioSource.mute = true;
		else
			_audioSource.mute = false;
	}

	private void CacheSoundState() => _lastSoundState = GameManager.SoundState;
}
