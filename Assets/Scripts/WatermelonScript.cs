using System.Collections;
using UnityEngine;

public class WatermelonScript : MonoBehaviour {

	public GameObject Complete;
	public GameObject Parts;
	public GameObject Splash;

	[Space]

	public float PostExplodeLifetime = 5.0f;

	public bool HasContacted = false;

	// Use this for initialization
	void Start () {
		//StartCoroutine ( WaitXSeconds ( 1 ) );
		Vector3 RayStartPos = transform.position - new Vector3 ( 0 , Complete.GetComponent<Renderer> ().bounds.extents.y , 0 );
		RaycastHit HitInfo;
		Physics.Raycast ( new Ray ( RayStartPos , Vector3.down ) , out HitInfo );
		transform.Translate ( Vector3.down * ( HitInfo.distance + 1 ) , Space.World );

		Rigidbody r = Complete.GetComponent<Rigidbody> ();
		r.velocity = r.angularVelocity = Vector3.zero;
	}

	IEnumerator WaitXSeconds ( int x ) {
		yield return new WaitForSeconds ( x );
		Contact ();
	}


	public void Contact () {
		HasContacted = true;

		Parts.transform.localPosition = Splash.transform.localPosition = Complete.transform.localPosition;

		Complete.SetActive ( false );

		Parts.SetActive ( true );
		Splash.SetActive ( true );

		Destroy ( gameObject , PostExplodeLifetime + Random.Range ( -1.5f , 2.0f ) );
	}
}
