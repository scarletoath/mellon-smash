using UnityEngine;

public class SFXController : MonoSingleton<SFXController> {
	SFXManager manager;
	protected override void Awake () {
		manager = this.GetComponent<SFXManager> ();
	}
	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		if ( Input.GetKeyDown ( KeyCode.X ) ) {
			SFXController.Instance.PlayBreakMelon ();
		}
	}

	public void PlayBreakMelon () {
		manager.PlaySound ( "Scream" );
		manager.PlaySound ( "BreakMelon" );
	}

	public void PlaySwing () {
		manager.PlaySound ( "Swing" );
	}
	public void PlayFootsteps () {
		manager.PlaySound ( "Footsteps" );
	}

	public void PlaySeagulls () {
		manager.PlaySound ( "Seagulls" );
	}

	public void PlaySeedShot () {
		manager.PlaySound ( "SeedShot" , 4 );
	}
	public void PlayWave () {
		manager.PlaySound ( "Wave" );
	}
}
