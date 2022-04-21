using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BulletSpawner : MonoBehaviour {

	[Tooltip("The Bullet prefab")]
	[SerializeField] private Bullet _bulletPrefab;

	private ObjectPool<Bullet> _pool;

	private void Start() => _pool = new ObjectPool<Bullet>(CreateBullet, OnGetBulletFromPool, OnReleaseBulletToPool, Bullet => Destroy(Bullet.gameObject), true, 10, 10000);

	public void SpawnBullet(GameObject target) {
		Bullet Bullet = _pool.Get();
		Bullet.SetKill(Kill);
		Bullet.SetTarget(target);
	}

	private Bullet CreateBullet() {
		return Instantiate(_bulletPrefab, transform.position, _bulletPrefab.transform.rotation, transform);
	}

	private void OnGetBulletFromPool(Bullet Bullet) => Bullet.gameObject.SetActive(true);

	private void OnReleaseBulletToPool(Bullet Bullet) {
		Bullet.ResetPosition(transform.position);
		Bullet.gameObject.SetActive(false);
	}

	private void Kill(Bullet Bullet) => _pool.Release(Bullet);
}
