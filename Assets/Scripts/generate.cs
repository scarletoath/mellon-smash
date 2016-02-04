using UnityEngine;

public class generate : MonoBehaviour {

	public Transform Container;
	public GameObject model;
	private int [] pos;
	private int watermelonNum;
	private string waterName;

	// Use this for initialization
	void Start () {
		//melon = new Object[24];
		pos = new int [ 24 ];
		watermelonNum = 0;
	}

	// Update is called once per frame
	void Update () {
		if ( Input.GetKeyDown ( KeyCode.A ) ) {
			Generate ( 1 );
		}
	}

	public void Generate ( int Count ) {
		ranPos ( Count );
		genNew ( Count );
	}

	void delW ( int num ) {
		for ( int i = 0 ; i < 24 ; ++i ) {
			if ( pos [ i ] != 0 ) {
				waterName = "melon[" + i + ']';
				GameObject del = GameObject.Find ( waterName );
				Destroy ( del );
				//Destroy(melon[i]);
			}
		}
	}

	void ranPos ( int num ) {
		pos = new int [ 24 ];
		for ( int i = 0 ; i < num ; ++i ) {

			int a = Random.Range ( 0 , 24 );
			while ( pos [ a ] != 0 ) {
				a = Random.Range ( 0 , 24 );
			}
			pos [ a ] = 1;
		}
		Debug.Log ( "random" );
		for ( int i = 0 ; i < 24 ; ++i ) {
			if ( pos [ i ] != 0 ) {
				Debug.Log ( i );
			}
			//Debug.Log(pos[i]);
		}
		Debug.Log ( "end" );
	}

	public void genNew ( int num ) {

		for ( int i = 0 ; i < 24 ; ++i ) {
			if ( pos [ i ] != 0 ) {
				GameObject melon;
				int r = Random.Range ( 3 , 5 );
				waterName = "melon[" + i + ']';
				Vector3 tran = transform.localPosition + new Vector3 ( r * Mathf.Cos ( 15 * i * Mathf.PI / 180 ) , 0.5f , r * Mathf.Sin ( 15 * i * Mathf.PI / 180 ) );
				Quaternion quat = new Quaternion ( Random.Range ( 0 , Mathf.PI ) , Random.Range ( 0 , Mathf.PI ) , Random.Range ( 0 , Mathf.PI ) , 0 );
				melon = Instantiate ( model , tran , quat ) as GameObject;
				melon.name = waterName;
				melon.transform.parent = Container;
			}
		}
	}
}
