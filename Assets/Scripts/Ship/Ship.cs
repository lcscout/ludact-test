using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Game.Utils;

[RequireComponent(typeof(Rigidbody2D))]
public class Ship : MonoBehaviour {
	private Rigidbody2D _rigidbody;
	private Action<Ship> _killAction;
	private Vector3 _lastVelocity;
	private Vector3 _reflectedDirection = Vector3.up;
	private float _speed = 0;
	private bool _canAlignTransformUp = false;
	private bool _ignoreTriggerWithCameraBounds = false;

	private void Awake() => _rigidbody = GetComponent<Rigidbody2D>();

	private void FixedUpdate() {
		_speed += Time.fixedDeltaTime;
		if (_canAlignTransformUp)
			transform.up = _reflectedDirection.normalized;
		_rigidbody.AddForce(transform.up * _speed, ForceMode2D.Force);
		_lastVelocity = _rigidbody.velocity;
	}

	private void Update() => transform.position = Gameplay.RemoveZAxisOf(transform);

	private void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.CompareTag("World Bounds"))
			IgnoreSecondTrigger();
		if (other.gameObject.CompareTag("World Bounds") || other.gameObject.CompareTag("Destroyer")) {
			_reflectedDirection = Vector3.Reflect(_lastVelocity.normalized, other.contacts[0].normal);
			_canAlignTransformUp = true;
			_rigidbody.velocity = transform.up * _lastVelocity.magnitude;
		}
	}

	private void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.CompareTag("Camera Bounds")) {
			if (!_ignoreTriggerWithCameraBounds) {
				Vector3 closestPoint = other.ClosestPoint(transform.position);
				Vector3 normal = (transform.position - closestPoint).normalized;

				WarpToOppositeEdge(normal);
			}

			IgnoreSecondTrigger();
		}
	}

	// private void OnDrawGizmos() {
	// 	Gizmos.color = Color.red;
	// 	if (_rigidbody != null)
	// 		Gizmos.DrawLine(transform.position, _rigidbody.velocity);

	// 	Gizmos.color = Color.blue;
	// 	Gizmos.DrawLine(transform.position, _reflectedDirection);
	// }

	public void Reset(Vector3 position, Quaternion rotation) {
		transform.position = position;
		transform.rotation = rotation;
	}

	public void SetKill(Action<Ship> killAction) => _killAction = killAction;

	public void Kill() => _killAction(this);

	public void SetSpeed(float speed) => _speed = speed;

	private void WarpToOppositeEdge(Vector3 normal) {
		Vector3 invertedVec = transform.position;
		if (normal.x != 0)
			invertedVec.x *= -1;
		if (normal.y != 0)
			invertedVec.y *= -1;
		transform.position = invertedVec;
	}

	private void IgnoreSecondTrigger() => _ignoreTriggerWithCameraBounds = !_ignoreTriggerWithCameraBounds;
}
