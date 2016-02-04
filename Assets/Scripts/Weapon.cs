using System.Linq;
using UnityEngine;

namespace Game {

	internal class Weapon : MonoBehaviour {

		public GameManager GameManager;
		public GameObject [] Weapons = new GameObject [ 1 ];

		private GameObject CurrentWeapon;
		private int CurrentWeaponIndex = -1;

		public void Awake () {
			if ( Weapons.Length > 0 && Weapons.All ( w => w != null ) ) {
				CurrentWeapon = transform.GetChild ( 0 ).gameObject;
				SetWeapon ( 0 );
			}
		}

		public void Start () { }

		public void Update () {
			return;
			if ( Input.GetKeyUp ( KeyCode.W ) ) {
				if ( CurrentWeaponIndex < Weapons.Length ) {
					UpgradeWeapon ();
				}
				else {
					SetWeapon ( 0 );
				}
			}
		}

		public void UpgradeWeapon () {
			SetWeapon ( CurrentWeaponIndex + 1 );
		}

		public void SetWeapon ( int Index ) {
			if ( Index >= 0 && Index < Weapons.Length && Index != CurrentWeaponIndex ) {
				CurrentWeaponIndex = Index;

				if ( CurrentWeapon != null ) {
					Destroy ( CurrentWeapon );
				}

				CurrentWeapon = Instantiate ( Weapons [ CurrentWeaponIndex ] );
				CurrentWeapon.transform.parent = transform;
				CurrentWeapon.transform.localPosition = Weapons [ CurrentWeaponIndex ].transform.localPosition;
				CurrentWeapon.transform.localRotation = Quaternion.identity;
				CurrentWeapon.transform.localScale = Weapons [ CurrentWeaponIndex ].transform.localScale;

				CurrentWeapon.GetComponentInChildren<ColliderEvent> ().GameManager = GameManager;
			}
		}

	}

}
