using System;
using UnityEngine;
using UnityEngine.UI;

namespace Test {

	internal class OVR_Test : MonoBehaviour {

		public Text Text;
		public Transform TrackingCameraTransform;

		private void Start () {
			if ( Text == null || TrackingCameraTransform == null ) {
				enabled = false;

				throw new NullReferenceException ();
			}

			
		}

		private void Update () {
			Vector3 Pos;
			//Pos = OVRInput.GetLocalHandPosition ( OVRInput.Hand.Left );
			Pos = TrackingCameraTransform.position;
			Text.text = Pos.ToString ( "F2" );
		}
	}

}