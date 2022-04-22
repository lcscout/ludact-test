using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Utils;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour {
	[SerializeField] private float _speed = 50f;

	private Rigidbody2D _rigidbody;
	private Action<Bullet> _killAction;
	private GameObject _target;

	private void Awake() => _rigidbody = GetComponent<Rigidbody2D>();

	private void FixedUpdate() {
		transform.up = _target.transform.position - transform.position; // game utils - look at
		_rigidbody.velocity = transform.up * _speed * Time.fixedDeltaTime;
	}

	private void Update() => transform.position = Gameplay.RemoveZAxisOf(transform);

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject == _target) {
			other.gameObject.GetComponent<Ship>()?.Kill();
			_killAction(this);
		}
	}

	public void ResetPosition(Vector3 spawnPosition) => transform.position = spawnPosition;

	public void SetKill(Action<Bullet> killAction) => _killAction = killAction;

	public void SetTarget(GameObject target) => _target = target;
}
