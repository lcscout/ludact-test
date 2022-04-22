using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Utils;

public class ShipDestroyer : MonoBehaviour {
	[Header("Components References")]
	[SerializeField] private BulletSpawner _bulletSpawner;
	[SerializeField] private Transform _crosshair;

	[Header("Settings")]
	[Tooltip("How fast the destroyer will rotate towards its target")]
	[SerializeField] private float _destroyerDamp = .2f;

	[Tooltip("How fast/strict the crosshair will be when following target")]
	[SerializeField] private float _crosshairDamp = .08f;

	private Ship _target;
	private float _timer = 0f;
	private bool _isShotPrepared = true;

	private const float DESTROY_RATE = 1F;

	private void OnEnable() => Ship.OnShipDestroyed += () => PrepareForShoot();

	private void OnDisable() => Ship.OnShipDestroyed -= () => PrepareForShoot();

	private void Update() {
		if (GameManager.IsGamePaused)
			return;

		if (_target != null && _target.isActiveAndEnabled) {
			LookAt(_target.transform);
			AimAt(_target.transform);

			if (Wait(DESTROY_RATE) && _isShotPrepared)
				SpawnBullet(_target.gameObject);
		}
		else
			FindNewTarget();
	}

	private void LookAt(Transform target) => transform.up = Vector3.Slerp(transform.up, target.position - transform.position, _destroyerDamp);

	private void AimAt(Transform target) {
		_crosshair.position = Vector3.Slerp(_crosshair.position, target.position, _crosshairDamp);
		_crosshair.position = Gameplay.KeepParentZAxisOf(_crosshair);
	}

	private bool Wait(float secondsToWait) {
		_timer += Time.deltaTime;
		int seconds = (int)(_timer % 60);
		if (seconds >= secondsToWait) {
			_timer = 0f;
			return true;
		}

		return false;
	}

	private void SpawnBullet(GameObject target) {
		_bulletSpawner.SpawnBullet(target);
		_isShotPrepared = false;
	}

	private void FindNewTarget() => _target = FindObjectOfType<Ship>();

	private void PrepareForShoot() {
		FindNewTarget();
		_isShotPrepared = true;
	}
}
