using Game;
using UnityEngine;

public class ColliderEvent : MonoBehaviour {

	public GameManager GameManager;
	private Rigidbody rb;
	// Use this for initialization
	private void Awake () {
		rb = this.GetComponent<Rigidbody> ();
	}

	// Update is called once per frame
	private void LateUpdate () {
		Debug.Log ( ( rb.velocity.magnitude ) );
		if ( rb.velocity.magnitude > 2f ) {
			SFXController.Instance.PlaySwing ();
		}
	}

	public void OnCollisionEnter ( Collision collision ) {
		DoShit ( collision , "enter" );
	}

	private void DoShit ( Collision Collision , string colreason ) {
		WatermelonScript w = Collision.collider.transform.parent.GetComponent<WatermelonScript> ();
		if ( w != null && !w.HasContacted ) {
			print ( Collision.relativeVelocity.sqrMagnitude );
			// Too slow
			if ( Collision.relativeVelocity.sqrMagnitude < 1.5f ) {
				return;
			}

			w.Contact ();
			SFXController.Instance.PlayBreakMelon ();

			GameManager.ChangeScore ( 1 );
		}
	}

}