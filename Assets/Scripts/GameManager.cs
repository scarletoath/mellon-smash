using System;
using System.Collections;
using HOV.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Game {

	public class GameManager : MonoBehaviour {

		[Serializable]
		private class GameManagerEvent : UnityEvent<int , int> {

		}

		private const float DURATION_WARNING_SCREEN = 15.0f;

		//public FadeToBlack Fader;
		public ColoredOverlay Overlay;
		public generate Generator;
		public GameObject IntroPanel;
		public GameObject HUDPanel;
		public CharacterController Controller;

		[Space]

		public float TimeBeforeBlindfold = 5.0f;

		private float TimeSinceStart = 0;
		private Step CurrentStep;
		private int CurrentLevel = 1;
		private int RemainingCount = 0;

		private enum Step {

			WarningScreen,
			StartScreen,
			GameStart,
			GameRunning,

		}

		[SerializeField]
		private GameManagerEvent OnScoreChanged;

		void Start () {
			Overlay.Hide ();
			IntroPanel.SetActive ( false );
			HUDPanel.SetActive ( false );
			Controller.enabled = false;

			CurrentStep = Step.WarningScreen;
			SFXController.Instance.PlayWave ();
			SFXController.Instance.PlaySeagulls ();
		}

		void Update () {
			switch ( CurrentStep ) {
				case Step.WarningScreen:
					TimeSinceStart += Time.deltaTime;
					if ( Input.anyKey || TimeSinceStart >= DURATION_WARNING_SCREEN ) {
						CurrentStep = Step.StartScreen;
						TimeSinceStart = 0;
						IntroPanel.SetActive ( true );
					}
					break;
				case Step.StartScreen:
					TimeSinceStart += Time.deltaTime;
					if ( TimeSinceStart > 5.0f ) {
						CurrentStep = Step.GameStart;
						Destroy ( IntroPanel );
					}
					break;
				case Step.GameStart:
					SetupLevel ();
					HUDPanel.SetActive ( true );

					CurrentStep = Step.GameRunning;

					break;
				case Step.GameRunning:
					if ( RemainingCount <= 0 ) {
						ProgressNextLevel ();
					}

					break;
			}
		}

		public int Score { get; private set; }

		public void ChangeScore ( int Delta ) {
			int OldScore = Score;
			if ( Delta < 0 ) {
				Score = 0;
			}
			else {
				Score += Delta;
			}

			RemainingCount -= Score - OldScore;

			if ( OnScoreChanged.GetPersistentEventCount () > 0 ) {
				OnScoreChanged.Invoke ( Score , Score - OldScore );
			}
		}

		public void ProgressNextLevel () {
			CurrentLevel++;

			Overlay.OverlayColor.a -= 0.0125f;
			Overlay.Show ();

			CurrentStep = Step.GameStart;
		}

		private void SetupLevel () {
			switch ( CurrentLevel ) {
				case 1:
					DelayTask ( Overlay.Show , TimeBeforeBlindfold );
					DelayTask ( () => Controller.enabled = true , TimeBeforeBlindfold + Overlay.Duration );
					ChangeScore ( -1 );

					Generator.Generate ( RemainingCount = 1 );

					break;

				case 2:
					Generator.Generate ( RemainingCount = 2 );

					break;

				case 3:
					Generator.Generate ( RemainingCount = 4 );

					break;

				case 4:
					Generator.Generate ( RemainingCount = 5 );

					break;

				case 5:
					Generator.Generate ( RemainingCount = 8 );

					break;

				default:
					// no more levels - you win!
					RemainingCount = 0xfffffff;

					break;
			}
		}

		private void DelayTask ( Action Task , float Delay ) {
			StartCoroutine ( ExecuteDelayTask ( Task , Delay ) );
		}

		private IEnumerator ExecuteDelayTask ( Action Task , float Delay ) {
			yield return new WaitForSeconds ( Delay );

			Task ();
		}

	}

}