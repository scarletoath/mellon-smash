using UnityEngine;
using UnityEngine.UI;

namespace Game {

	public class HUDScore : MonoBehaviour {

		public Text ScoreText;

		public void UpdateScore ( int Score , int Delta ) {
			ScoreText.text = string.Format ( "x{0}" , Score );
		}

	}

}