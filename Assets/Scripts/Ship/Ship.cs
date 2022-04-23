using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Game.Utils;

[RequireComponent(typeof(Rigidbody2D))]
public class Ship : MonoBehaviour {
	public static event Action<Vector3> OnShipDestroyed;

	[Header("Settings")]
	[Tooltip("The constant amount of increase in speed")]
	[SerializeField] private float _linearSpeedGrowth = 0.05f;

	private Rigidbody2D _rigidbody;
	private AudioSource _audioSource;
	private Action<Ship> _killAction;
	private Vector3 _lastVelocity;
	private Vector3 _reflectedDirection = Vector3.up;
	private GameManager.Sound _lastSoundState = GameManager.Sound.UnmuteAll;
	private float _speed;
	private bool _bouncedOnBoundsAtLeastOnce = false;
	private bool _ignoreTriggerWithCameraBounds = false;

	private void Awake() {
		_rigidbody = GetComponent<Rigidbody2D>();
		_audioSource = GetComponent<AudioSource>();
	}

	private void FixedUpdate() {
		MoveForward();

		_lastVelocity = _rigidbody.velocity;

		if (_bouncedOnBoundsAtLeastOnce)
			transform.up = _reflectedDirection.normalized;
	}

	private void Update() {
		transform.position = Gameplay.RemoveZAxisOf(transform);

		HandleSound();
	}

	private void OnCollisionEnter2D(Collision2D other) {
		bool hitWorldBoundsOrDestroyer = other.gameObject.CompareTag("World Bounds") || other.gameObject.CompareTag("Destroyer");
		bool gameModeIsConfinedAndHitCameraBounds = GameManager.GameMode == GameManager.Mode.Confined && other.gameObject.CompareTag("Camera Bounds");

		if (other.gameObject.CompareTag("World Bounds"))
			IgnoreTriggerUntilEnterCameraBounds();
		if (hitWorldBoundsOrDestroyer || gameModeIsConfinedAndHitCameraBounds)
			BounceOnBounds(other.contacts[0].normal);
	}

	private void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.CompareTag("Camera Bounds")) {
			if (!_ignoreTriggerWithCameraBounds) {
				Vector3 closestPoint = other.ClosestPoint(transform.position);
				Vector3 normal = (transform.position - closestPoint).normalized;

				WarpToOppositeEdge(normal);
			}

			IgnoreSecondTriggerWithCameraBounds(); // this is necessary because the ship is warped behind the bounds
		}
	}

	public void Reset(Vector3 position, Quaternion rotation) {
		transform.position = position;
		transform.rotation = rotation;
	}

	public void Configure(Action<Ship> killAction, float initialSpeed, string name) {
		_killAction = killAction;
		_speed = initialSpeed;
		this.name = name;
	}

	public void Kill() {
		OnShipDestroyed?.Invoke(transform.position);
		_killAction(this);
	}

	private void MoveForward() {
		IncreaseSpeedLinearly();
		_rigidbody.AddForce(transform.up * _speed, ForceMode2D.Force);
	}

	private void IncreaseSpeedLinearly() => _speed += _linearSpeedGrowth;

	private void BounceOnBounds(Vector3 normal) {
		_reflectedDirection = Vector3.Reflect(_lastVelocity.normalized, normal);
		_rigidbody.velocity = transform.up * _lastVelocity.magnitude; // reflect rigidbody's velocity

		_bouncedOnBoundsAtLeastOnce = true;
	}

	private void WarpToOppositeEdge(Vector3 normal) {
		Vector3 invertedVec = transform.position;

		if (normal.x != 0)
			invertedVec.x *= -1;
		if (normal.y != 0)
			invertedVec.y *= -1;

		transform.position = invertedVec;
	}

	private void IgnoreSecondTriggerWithCameraBounds() => _ignoreTriggerWithCameraBounds = !_ignoreTriggerWithCameraBounds;

	private void IgnoreTriggerUntilEnterCameraBounds() => _ignoreTriggerWithCameraBounds = true;

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
