using UnityEngine;

public class ExplodeScript : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		foreach ( Transform t in this.transform ) {
			t.GetComponent<Rigidbody> ().AddRelativeForce ( Random.onUnitSphere * 1000 );
		}
	}

	// Update is called once per frame
	void Update () {
		if ( Input.GetKeyDown ( KeyCode.Space ) ) {
			//this.transform.GetChild(0).GetComponent<Rigidbody>().AddExplosionForce(1000, this.transform.position-Vector3.up*3, 5);
		}
	}
}
