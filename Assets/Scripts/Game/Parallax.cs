using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Utils;

public class Parallax : MonoBehaviour {
	[SerializeField] private Image _lowerLayer;
	[SerializeField] private Image _mediumLayer;
	[SerializeField] private Image _higherLayer;

	[Tooltip("Time rate in seconds when a new target position will be assigned")]
	[SerializeField] private float _newTargetRate = 2.5f;
	[SerializeField] private float _damping = .5f;

	private Vector3 _targetPosition = Vector3.zero;
	private float _timer = 0f;

	private void Start() => AssignNewTarget();

	private void Update() {
		_timer += Time.deltaTime;
		if ((int)(_timer % 60) >= _newTargetRate)
			AssignNewTarget();

		FollowTarget();
	}

	private void AssignNewTarget() {
		_timer = 0f;
		_targetPosition = Gameplay.GetRandomPositionInSpawnableArea();
	}

	private void FollowTarget() {
		SlerpLayer(_lowerLayer.transform, _damping/8);
		SlerpLayer(_mediumLayer.transform, _damping/3);
		SlerpLayer(_higherLayer.transform, _damping);
	}

	private void SlerpLayer(Transform layer, float speed) {
		layer.position = Vector3.Slerp(layer.position, _targetPosition, speed * Time.deltaTime);
		KeepLayerAligned(layer);
	}

	private void KeepLayerAligned(Transform layer) => layer.position = new Vector3(layer.position.x, layer.position.y, layer.parent.position.z);
}
