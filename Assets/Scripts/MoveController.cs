using UnityEngine;

namespace Game {

	public enum AIAction {

		Patrol,
		Arrive,
		Seek,
		Wander,
		Idle
	}

	public class MoveController : MonoBehaviour {

		public AIAction MovementAiAction;
		public Transform patrolTargetLeft;
		public Transform patrolTargetRight;
		public Transform arriveTarget;
		public float maxSpeed = 5f;
		public float maxForce = 5f;
		private Vector3 currentVelocity;
		private Rigidbody rb;
		private Vector3 currentPos;
		private Vector3 finalSteering;
		private Vector3 currentTargetPos;
		private float randomAngle;

		// Use this for initialization
		private void Awake () {
			finalSteering = Vector3.zero;
			currentPos = this.transform.position;
			rb = GetComponent<WatermelonScript> ().Complete.GetComponent<Rigidbody> ();
			currentVelocity = rb.velocity;
		}

		// Update is called once per frame
		private void Update () {
			finalSteering = Vector3.zero;
			currentVelocity = rb.velocity;
			currentPos = this.transform.position;
			//do actions
			if ( MovementAiAction == AIAction.Patrol ) {
				currentTargetPos = patrolTargetLeft.position;
				Patrol ( patrolTargetLeft.position , patrolTargetRight.position , 5f );
			}
			if ( MovementAiAction == AIAction.Arrive ) {
				Arrive ( arriveTarget.position , 5f );
			}
			if ( MovementAiAction == AIAction.Wander ) {
				Wander ( 40f , 150f );
			}
			//v0 + a*t
			Vector3 finalVelocity = currentVelocity + finalSteering * Time.deltaTime;
			rb.velocity = Vector3.ClampMagnitude ( finalVelocity , maxForce );
		}

		public void Seek ( Vector3 targetPos ) {
			finalSteering += _Seek ( targetPos );
		}

		private Vector3 _Seek ( Vector3 targetPos ) {
			Vector3 desiredVelocity = ( targetPos - currentPos ).normalized * maxSpeed;
			Vector3 steering = desiredVelocity - currentVelocity;
			if ( steering.magnitude < 1f )
				steering = steering.normalized;
			return steering;
		}

		public void Patrol ( Vector3 targetPos1 , Vector3 targetPos2 , float slowDistance ) {
			if ( Vector3.Distance ( this.transform.position , targetPos1 ) < 1 ) {
				currentTargetPos = targetPos2;
			}
			if ( Vector3.Distance ( this.transform.position , targetPos2 ) < 1 ) {
				currentTargetPos = targetPos1;
			}
			Arrive ( currentTargetPos , slowDistance );
		}

		public void Arrive ( Vector3 targetPos , float slowDistance ) {
			finalSteering += _Arrive ( targetPos , slowDistance );
		}

		private Vector3 _Arrive ( Vector3 targetPos , float slowDistance ) {
			float distance = ( targetPos - currentPos ).magnitude;
			//distance/slowDistance will be greater than 1 if he's farther away
			//slowSpeed will never exceed maxSpeed because of clamp.
			float slowSpeed = Mathf.Clamp ( maxSpeed * distance / slowDistance , 0f , maxSpeed );
			Vector3 desiredVelocity = ( targetPos - currentPos ).normalized * slowSpeed;
			Vector3 steering = desiredVelocity - currentVelocity;
			if ( steering.magnitude < 1f )
				steering = steering.normalized;
			//else
			//	steering = Vector3.ClampMagnitude (steering, maxForce);
			return steering;
		}

		public void Wander ( float angleRange , float maxAngle ) {
			finalSteering += _Wander ( angleRange , maxAngle );
		}

		private Vector3 _Wander ( float angleRange , float maxAngle ) {
			randomAngle = randomAngle + Random.Range ( -angleRange , angleRange );
			//reset randomAngle if it goes past maxAngle degrees
			if ( Mathf.Abs ( randomAngle ) > maxAngle )
				randomAngle = 0f;
			Vector3 steering = Quaternion.Euler ( 0 , randomAngle , 0 ) * currentVelocity;
			if ( currentVelocity == Vector3.zero )
				steering = Quaternion.Euler ( 0 , randomAngle , 0 ) * transform.forward;
			steering = steering.normalized * maxSpeed / 3;
			//Debug.DrawRay (currentPos, steering, Color.blue);
			return steering;
		}

	}

}