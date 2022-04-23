using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	public static AudioManager Instance;

	[Tooltip("The tracks available to play sounds. 1st is for BGM and the 2nd is for SFX.")]
	[SerializeField] private List<AudioSource> _tracks = new List<AudioSource>();
	[SerializeField] private List<AudioClip> _sounds = new List<AudioClip>();

	private void OnEnable() => ShipDestroyer.OnShot += () => PlaySoundOneShot(2, 2);

	private void OnDisable() => ShipDestroyer.OnShot -= () => PlaySoundOneShot(2, 2);

	private void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}

	public void PlaySoundOneShot(int trackNumber, int sound) {
		AudioSource audioSource = GetTrack(trackNumber);
		if (audioSource != null)
			audioSource.PlayOneShot(GetSound(sound));
	}

	public void MuteSFX() {
		UnmuteAll();
		ChangeTrackMuteState(2, true);
	}

	public void MuteAll() {
		ChangeTrackMuteState(1, true);
		ChangeTrackMuteState(2, true);
	}

	public void UnmuteAll() {
		ChangeTrackMuteState(1, false);
		ChangeTrackMuteState(2, false);
	}

	private void ChangeTrackMuteState(int track, bool muteState) => GetTrack(track).mute = muteState;

	private AudioSource GetTrack(int trackNumber) {
		return _tracks[trackNumber - 1];
	}

	private AudioClip GetSound(int sound) {
		return _sounds[sound - 1];
	}
}
