using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;
using Game.Utils;

public class ExplosionSpawner : MonoBehaviour {

	[Tooltip("The ship prefab")]
	[SerializeField] private Explosion _explosionPrefab;

	private ObjectPool<Explosion> _pool;
	private Vector3 _targetShipLastPosition;

	private void OnEnable() => Ship.OnShipDestroyed += (lastPosition) => SpawnExplosion(lastPosition);

	private void OnDisable() => Ship.OnShipDestroyed -= (lastPosition) => SpawnExplosion(lastPosition);

	private void Start() => _pool = new ObjectPool<Explosion>(CreateExplosion, OnGetExplosionFromPool, OnReleaseExplosionToPool,
		explosion => Destroy(explosion.gameObject));

	private Explosion CreateExplosion() {
		return Instantiate(_explosionPrefab, _explosionPrefab.transform.position, _explosionPrefab.transform.rotation, transform);
	}

	private void OnGetExplosionFromPool(Explosion explosion) => explosion.gameObject.SetActive(true);

	private void OnReleaseExplosionToPool(Explosion explosion) => explosion.gameObject.SetActive(false);

	private void SpawnExplosion(Vector3 shipPosition) {
		// couldn't find out why, but it seems the OnShipDestroyed event is not being unsubscribed or unity is calling it before ExplosionSpawner exists, causing a
		// missing reference for this script whenever the user restarts the game. To prevent this, I figured that checking if the script was initialized works
		if (this != null) {
			Explosion explosion = _pool.Get();
			explosion.Configure(Kill, shipPosition);
		}
	}

	private void Kill(Explosion explosion) => _pool.Release(explosion);
}