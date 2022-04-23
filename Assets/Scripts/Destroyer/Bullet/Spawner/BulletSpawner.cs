using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BulletSpawner : MonoBehaviour {

	[Tooltip("The bullet prefab")]
	[SerializeField] private Bullet _bulletPrefab;

	private ObjectPool<Bullet> _pool;

	private void Start() => _pool = new ObjectPool<Bullet>(CreateBullet, OnGetBulletFromPool, OnReleaseBulletToPool, bullet => Destroy(bullet.gameObject));

	public void SpawnBullet(GameObject target) {
		Bullet bullet = _pool.Get();
		bullet.Configure(Kill, target);
	}

	private Bullet CreateBullet() {
		return Instantiate(_bulletPrefab, transform.position, _bulletPrefab.transform.rotation, transform);
	}

	private void OnGetBulletFromPool(Bullet bullet) {
		bullet.ResetPosition(transform.position);
		bullet.gameObject.SetActive(true);
	}

	private void OnReleaseBulletToPool(Bullet bullet) => bullet.gameObject.SetActive(false);

	private void Kill(Bullet bullet) => _pool.Release(bullet);
}
