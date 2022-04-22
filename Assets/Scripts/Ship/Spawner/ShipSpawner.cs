using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;
using Game.Utils;

public class ShipSpawner : MonoBehaviour {
	public static event Action OnShipCreated;
	public static event Action OnShipSpawned;
	public static event Action OnNewRound;

	[Tooltip("The ship prefab")]
	[SerializeField] private Ship _shipPrefab;

	[Header("Settings")]
	[Tooltip("The minimum and maximum speeds the ships can spawn with")]
	[SerializeField] private Vector2 _speedMinMax = new Vector2(1f, 10f);

	private ObjectPool<Ship> _pool;
	private int _shipsCounter = 0;
	private int _round = 0;
	private int _fibonacci;

	private void Start() => _pool = new ObjectPool<Ship>(CreateShip, OnGetShipFromPool, OnReleaseShipToPool, ship => Destroy(ship.gameObject));

	private void Update() {
		if (_pool.CountActive == 0)
			NewRound();
		if (_shipsCounter < _fibonacci)
			SpawnShip();
	}

	private Ship CreateShip() {
		OnShipCreated?.Invoke();
		return Instantiate(_shipPrefab, Gameplay.GetRandomPositionInSpawnableArea(), Gameplay.GetRandomRotation(), transform);
	}

	private void OnGetShipFromPool(Ship ship) => ship.gameObject.SetActive(true);

	private void OnReleaseShipToPool(Ship ship) {
		ship.Reset(Gameplay.GetRandomPositionInSpawnableArea(), Gameplay.GetRandomRotation());
		ship.gameObject.SetActive(false);
	}

	private void SpawnShip() {
		Ship ship = _pool.Get();
		ship.Configure(Kill, Random.Range(_speedMinMax.x, _speedMinMax.y), "Ship (" + _shipsCounter + ")");

		_shipsCounter++;

		OnShipSpawned?.Invoke();
	}

	private void Kill(Ship ship) => _pool.Release(ship);

	private void NewRound() {
		_shipsCounter = 0;
		_round++;
		_fibonacci = Algorithm.Fibonacci(_round);

		OnNewRound?.Invoke();
	}
}