using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDestroyer : MonoBehaviour {
	[SerializeField] private BulletSpawner _bulletSpawner;
	[SerializeField] private Transform _crosshair;
	[SerializeField] private float _crosshairDamp = .08f;
	[SerializeField] private float _destroyerDamp = .2f;

	private Ship _target;
	private float _timer = 0f;
	private bool _canSpawnBullet = true;

	private void OnEnable() => Bullet.OnShipDestroyed += () => PrepareForShot();

	private void OnDisable() => Bullet.OnShipDestroyed -= () => PrepareForShot();

	private void Update() {
		if (GameManager.IsGamePaused)
			return;

		if (_target != null) {
			LookAt(_target.transform);
			Aim(_target.transform);

			if (WaitSeconds(1f) && _canSpawnBullet)
				SpawnBullet(_target.gameObject);
		}
		else
			FindNewTarget();
	}

	private void LookAt(Transform target) => transform.up = Vector3.Slerp(transform.up, target.position - transform.position, _destroyerDamp);

	private void Aim(Transform target) {
		Vector3 targetPosition2D = new Vector3(target.position.x, target.position.y, 0);
		_crosshair.position = Vector3.Slerp(_crosshair.position, targetPosition2D, _crosshairDamp);
	}

	public bool WaitSeconds(float secondsToWait) {
		_timer += Time.deltaTime;
		if ((int)(_timer % 60) >= secondsToWait) {
			_timer = 0f;
			return true;
		}

		return false;
	}

	private void SpawnBullet(GameObject target) {
		_bulletSpawner.SpawnBullet(target);
		_canSpawnBullet = false; // wait until another ship is destroyed
	}

	private void FindNewTarget() => _target = FindObjectOfType<Ship>();

	private void PrepareForShot() {
		FindNewTarget();
		_canSpawnBullet = true;
	}
}
