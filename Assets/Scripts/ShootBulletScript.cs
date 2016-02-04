using System.Collections;
using UnityEngine;

public class ShootBulletScript : MonoBehaviour {
	public float bulletSpeed = 3f;
	public GameObject bullet;
	public float minShootingDuration;
	public float maxShootingDuration;
	public float minBullets;
	public float maxBullets;
	public float intervalDuration;
	// Use this for initialization
	void Start () {
		StartCoroutine ( ShootRandomBulletRounds () );
	}

	// Update is called once per frame
	void Update () {

	}

	IEnumerator ShootRandomBulletRounds () {
		float duration = Random.Range ( minShootingDuration , maxShootingDuration );
		float numBullets = Random.Range ( minBullets , maxBullets );
		float interval = duration / numBullets;
		for ( int x = 0 ; x < numBullets ; x++ ) {
			ShootBullet ();
			yield return new WaitForSeconds ( interval );
		}
		yield return new WaitForSeconds ( intervalDuration );
		StartCoroutine ( ShootRandomBulletRounds () );
	}

	public void ShootBullet () {
		SFXController.Instance.PlaySeedShot ();
		GameObject spawnBullet = GameObject.Instantiate ( bullet , this.transform.position , Quaternion.identity ) as GameObject;
		//spawnBullet.transform.LookAt(Camera.main.transform);
		//spawnBullet.GetComponent<Rigidbody>().velocity = spawnBullet.transform.forward * bulletSpeed;
		Vector3 randomCamPosition = Camera.main.transform.position - Random.insideUnitSphere * 1.5f;
		spawnBullet.GetComponent<Rigidbody> ().velocity = ( randomCamPosition - this.transform.position ).normalized * bulletSpeed;
		Destroy ( spawnBullet , 5f );
	}
}
