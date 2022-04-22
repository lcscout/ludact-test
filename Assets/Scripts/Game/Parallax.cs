using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Utils;

public class Parallax : MonoBehaviour {
	[Header("Image Layers")]
	[SerializeField] private Image _lowerLayer;
	[SerializeField] private Image _mediumLayer;
	[SerializeField] private Image _higherLayer;

	[Header("Settings")]
	[Tooltip("Time rate in seconds when a new designated position will be assigned")]
	[SerializeField] private float _designatedPositionRate = 2.5f;

	[Tooltip("Sort of the speed which the higher layer reach the random designated position, the other layers speeds are relative to this one")]
	[SerializeField] private float _damping = .5f;

	private Vector3 _targetPosition = Vector3.zero;
	private float _timer = 0f;

	private void Start() => AssignNewTargetPosition();

	private void Update() {
		WaitAndCall(_designatedPositionRate, AssignNewTargetPosition);
		FollowTargetPosition();
	}

	private void WaitAndCall(float secondsToWait, Action action) {
		_timer += Time.deltaTime;
		int seconds = (int)(_timer % 60);

		if (seconds >= secondsToWait)
			action();
	}

	private void AssignNewTargetPosition() {
		_timer = 0f;
		_targetPosition = Gameplay.GetRandomPositionInSpawnableArea();
	}

	private void FollowTargetPosition() {
		SlerpLayer(_lowerLayer.transform, _damping/8);
		SlerpLayer(_mediumLayer.transform, _damping/3);
		SlerpLayer(_higherLayer.transform, _damping);
	}

	private void SlerpLayer(Transform layer, float speed) {
		layer.position = Vector3.Slerp(layer.position, _targetPosition, speed * Time.deltaTime);
		layer.position = Gameplay.KeepParentZAxisOf(layer);
	}
}
