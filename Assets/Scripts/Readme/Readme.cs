using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Readme", menuName = "ScriptableObjects/Readme", order = 1)]
public class Readme : ScriptableObject {
	public Texture2D icon;
	public string title;
	public Section[] sections;

	[Serializable]
	public class Section {
		public string heading, text, linkText, url;
	}
}
