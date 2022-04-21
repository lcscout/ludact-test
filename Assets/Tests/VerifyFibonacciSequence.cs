using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Game.Utils;

public class VerifyFibonacciSequence {
	private readonly int[] _correctFibonacciUntilN28 = { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765, 10946, 17711, 28657, 46368, 75025, 121393, 196418, 317811 };

	[Test]
	public void VerifyFibonacciSequenceUntilN28() {
		for (int i = 0; i < _correctFibonacciUntilN28.Length; i++)
			Assert.AreEqual(_correctFibonacciUntilN28[i], Algorithm.Fibonacci(i));
	}
}
