#region License
/*
 * UI/ColoredOverlay.cs
 *
 * Copyright (c) 2015, 2016 Faceless
 * Copyright (c) 2015, 2016 See Shuen Leong (Sam), Zongye Yang (Chiu)
 *
 * This software is solely licensed pursuant to the CMU Educational Project License Agreement,
 * Available at:  [LICENSE.pdf].
 * All other use is strictly prohibited.  
*/
#endregion

using UnityEngine;
using UnityEngine.UI;

namespace HOV.UI {

	public class ColoredOverlay : MonoBehaviour {

		[SerializeField]
		private Image OverlayImage;

		[Space]

		[SerializeField]
		public Color OverlayColor = Color.white;

		[Tooltip ( "How long the overlay is visible for when shown (in seconds)." )]
		[Range ( 0 , 1000 )]
		[SerializeField]
		public float Duration = 0.5f;

		private bool IsInit = false;

		public void Show () {
			if ( !enabled ) {
				return;
			}

			if ( !IsInit ) {
				Awake ();
			}

			gameObject.SetActive ( true );

			LeanTween.alpha ( OverlayImage.rectTransform , OverlayColor.a , Duration );
			//OverlayImage.color = OverlayColor;

			// Call Hide() after duration
			// 			if ( Duration > 0 ) {
			// 				LeanTween.cancel ( gameObject );
			// 				LeanTween.delayedCall ( gameObject , Duration , Hide );
			// 			}
		}

		public void Hide () {
			gameObject.SetActive ( false );
		}

		// Use this for initialization
		void Awake () {
			if ( IsInit ) {
				return;
			}

			if ( OverlayImage == null ) {
				Debug.LogError ( GetType ().ToString () + " requires a UnityEngine.UI.Image to function!" );

				enabled = false;
			}

			Hide ();

			IsInit = true;
		}

	}

}