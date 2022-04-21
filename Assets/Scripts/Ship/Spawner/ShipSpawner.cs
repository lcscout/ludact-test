using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;
using Game.Utils;

public class ShipSpawner : MonoBehaviour {
	public static event Action<int> OnRoundSpawnsFinished;
	public static event Action OnShipCreated;
	public static event Action OnShipSpawned;
	public static event Action OnNewRound;

	[Tooltip("The ship prefab")]
	[SerializeField] private Ship _shipPrefab;

	[Tooltip("The minimum and maximum speeds the ships can spawn with")]
	[SerializeField] private Vector2 _speedMinMax = new Vector2(1f, 10f);

	private ObjectPool<Ship> _pool;
	private int _shipsCounter = 0;
	private int _round = 0;
	private int _fibonacci;
	private bool _areSpawnsFinished = false;

	private void OnEnable() => OnRoundSpawnsFinished += ships => _areSpawnsFinished = false;

	private void OnDisable() => OnRoundSpawnsFinished += ships => _areSpawnsFinished = false;

	private void Start() => _pool = new ObjectPool<Ship>(CreateShip, OnGetShipFromPool, OnReleaseShipToPool, Ship => Destroy(Ship.gameObject), true, 10, 10000);

	private void Update() {
		if (_pool.CountActive == 0)
			NewRound();
		if (_shipsCounter < _fibonacci)
			SpawnShip();
		else if (_areSpawnsFinished)
			OnRoundSpawnsFinished?.Invoke(_shipsCounter);
	}

	private Ship CreateShip() {
		OnShipCreated?.Invoke();
		return Instantiate(_shipPrefab, Gameplay.GetRandomPositionInSpawnableArea(), Gameplay.GetRandomRotation(), transform);
	}

	private void OnGetShipFromPool(Ship Ship) => Ship.gameObject.SetActive(true);

	private void OnReleaseShipToPool(Ship Ship) {
		Ship.Reset(Gameplay.GetRandomPositionInSpawnableArea(), Gameplay.GetRandomRotation());
		Ship.gameObject.SetActive(false);
	}

	private void SpawnShip() {
		Ship ship = _pool.Get();
		ship.name = "Ship (" + _shipsCounter + ")"; // good for debug
		ship.SetKill(Kill);
		ship.SetSpeed(Random.Range(_speedMinMax.x, _speedMinMax.y));

		_shipsCounter++;

		OnShipSpawned?.Invoke();
	}

	private void Kill(Ship Ship) => _pool.Release(Ship);

	private void NewRound() {
		_shipsCounter = 0;
		_round++;
		_fibonacci = Algorithm.Fibonacci(_round);

		_areSpawnsFinished = true;

		OnNewRound?.Invoke();
	}
}