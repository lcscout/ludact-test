using UnityEngine;

namespace Game.Utils {
	public static class Algorithm {
		public static int Fibonacci(int number) {
			if (number == 0)
				return 0;
			if (number <= 2)
				return 1;
			else
				return Fibonacci(number - 1) + Fibonacci(number - 2);
		}
	}

	public static class Gameplay {
		public static Vector3 GetRandomPositionInSpawnableArea() {
			Vector3[] area = GetSpawnableArea();
			float horizontalPos = GetRandomPosition(area[0].x, area[1].x);
			float verticalPos = GetRandomPosition(area[1].y, area[0].y);

			return new Vector3(horizontalPos, verticalPos, 0);
		}

		public static Quaternion GetRandomRotation() {
			return Quaternion.Euler(0, 0, Random.Range(0f, 180f));
		}

		public static Vector3 RemoveZAxisOf(Transform transform) {
			return new Vector3(transform.position.x, transform.position.y, 0);
		}

		private static Vector3[] GetSpawnableArea() {
			Vector3[] area = new Vector3[2] {
				Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)), // top-left
				Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)), // bottom-right
			};

			return area;
		}

		private static float GetRandomPosition(float minPosition, float maxPosition) {
			return Random.Range(minPosition, maxPosition);
		}
	}
}