#region License
/*
 * Managers/SFXManager.cs
 *
 * Copyright (c) 2015, 2016 Faceless
 * Copyright (c) 2015, 2016 See Shuen Leong (Sam), Zongye Yang (Chiu)
 *
 * This software is solely licensed pursuant to the CMU Educational Project License Agreement,
 * Available at:  [LICENSE.pdf].
 * All other use is strictly prohibited.  
*/
#endregion

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The SoundEffect class manages AudioClips associated with a designated sound effect.
/// </summary>
[System.Serializable]
public class SoundEffect {

	private const float BUFFER_INTERVAL = 0.5f; // Seconds

	/// <summary>
	/// The name of the SoundEffect.
	/// </summary>
	public string Name;
	/// <summary>
	/// Indicates whether only one instance of the SoundEffect can be playing at any one moment.
	/// </summary>
	public bool IsSingleInstanceOnly = false;
	/// <summary>
	/// Indicates whether the SoundEffect is to be looped when played.
	/// </summary>
	public bool IsLooping = false;
	public SoundEffectLoopParameters LoopParams;
	/// <summary>
	/// The AudioClips associated with the SoundEffect.
	/// </summary>
	public List<AudioClip> Clips;

	/// <summary>
	/// The earliest time at which a single-instance SoundEffect can be next played.
	/// </summary>
	private float MinNextTime = 0.0f;
	private int LastClipIndex = -1;

	/// <summary>
	/// Indicates whether a new instance of the SoundEffect can be played.
	/// </summary>
	public bool CanPlay {
		get {
			return !IsSingleInstanceOnly || Time.time >= MinNextTime;
		}
	}

	/// <summary>
	/// Obtains a random AudioClip associated with the SoundEffect.
	/// </summary>
	/// <returns>A random AudioClip from the SoundEffect; null if none are associated.</returns>
	public AudioClip GetClip () {
		if ( !CanPlay || Clips.Count == 0 ) {
			return null;
		}

		AudioClip Clip = null;
		if ( Clips.Count == 1 ) {
			LastClipIndex = 0;
		}
		else {
			switch ( LoopParams.PlayOrder ) {
				case SoundEffectLoopPlayOrder.Random:
					LastClipIndex = Random.Range ( 0 , Clips.Count );

					break;

				case SoundEffectLoopPlayOrder.RandomWithoutRepeat:
					int Index = -1;
					do {
						Index = Random.Range ( 0 , Clips.Count );
					} while ( LastClipIndex != -1 && Index == LastClipIndex );
					LastClipIndex = Index;

					break;

				case SoundEffectLoopPlayOrder.Forward:
					if ( LastClipIndex < Clips.Count - 1 ) {
						LastClipIndex++;
					}
					else {
						LastClipIndex = 0;
					}

					break;

				case SoundEffectLoopPlayOrder.Reverse:
					if ( LastClipIndex > 0 ) {
						LastClipIndex--;
					}
					else {
						LastClipIndex = Clips.Count - 1;
					}

					break;

				default:
					LastClipIndex = -1;

					break;
			}
		}

		if ( LastClipIndex > -1 ) {
			Clip = Clips [ LastClipIndex ];

			if ( IsSingleInstanceOnly ) {
				UpdateMinNextTime ( Clip.length );
			}
		}

		return Clip;
	}

	/// <summary>
	/// Obtains the AudioClip with the specified index associated with the SoundEffect.
	/// </summary>
	/// <param name="Index">The index of the AudioClip to retrieve.</param>
	/// <returns>The desired AudioClip; null if invalid.</returns>
	public AudioClip GetClip ( int Index ) {
		if ( !CanPlay || Index < 0 || Index >= Clips.Count ) {
			return null;
		}

		AudioClip Clip = Clips [ Index ];
		LastClipIndex = Index;

		if ( IsSingleInstanceOnly ) {
			UpdateMinNextTime ( Clip.length );
		}

		return Clip;
	}

	private void UpdateMinNextTime ( float Duration ) {
		MinNextTime = Time.time + Duration + BUFFER_INTERVAL;
	}

}

public enum SoundEffectLoopPlayOrder {

	Random,
	RandomWithoutRepeat,

	Forward,
	Reverse,

}

[System.Serializable]
public class SoundEffectLoopParameters {

	public SoundEffectLoopPlayOrder PlayOrder = SoundEffectLoopPlayOrder.RandomWithoutRepeat;
	public float IntervalDelay = 0.0f;

}

public class SFXManager : MonoSingleton<SFXManager> {

	public float DefaultFadeDuration = 1.5f;
	public List<SoundEffect> SoundEffects;

	private Dictionary<string , SoundEffect> SoundEffectDict;

	private AudioSource AudioSource;
	private Dictionary<SoundEffect , AudioSource> LoopingAudioSources;

	//private LTDescr FadeTween;

	protected override bool IsPersistentBetweenScenes {
		get {
			return false;
		}
	}

	// Use this for initialization
	override protected void Awake () {
		base.Awake ();

		SoundEffectDict = new Dictionary<string , SoundEffect> ();
		RecreateDictionary ();

		AudioSource = GetComponent<AudioSource> ();
		if ( AudioSource == null ) {
			AudioSource = gameObject.AddComponent<AudioSource> ();
		}

		LoopingAudioSources = new Dictionary<SoundEffect , AudioSource> ();
	}

	// Update is called once per frame
	void Update () {

	}

	protected override void OnDestroy () {
		base.OnDestroy ();

		if ( Application.isPlaying ) {
			// 				if ( FadeTween != null ) {
			// 					FadeTween.cancel ();
			// 				}
		}
	}

	/// <summary>
	/// Plays any associated SoundEffect with a volume of 1 (maximum). This special
	/// overload of PlaySound() is required for the method to appear in the Unity
	/// Editor's event system.
	/// </summary>
	/// <param name="SoundEffectName">The name of the SoundEffect to play.</param>
	public void PlaySound ( string SoundEffectName ) {
		PlaySoundInternal ( SoundEffectName , -1 , 1.0f , transform.position );
	}

	/// <summary>
	/// Plays any associated SoundEffect with the specified volume.
	/// </summary>
	/// <param name="SoundEffectName">The name of the SoundEffect to play.</param>
	/// <param name="Volume">The volume to play the sound at.</param>
	/// <returns>The duration of the played sound; zero if none.</returns>
	public float PlaySound ( string SoundEffectName , float Volume ) {
		return PlaySoundInternal ( SoundEffectName , -1 , Volume , transform.position );
	}

	public float PlaySoundAtLocation ( string SoundEffectname , Vector3 WorldPos , float Volume ) {
		return PlaySoundInternal ( SoundEffectname , -1 , Volume , WorldPos );
	}

	/// <summary>
	/// Plays the specified associated SoundEffect with the specified volume.
	/// </summary>
	/// <param name="SoundEffectName">The name of the SoundEffect to play.</param>
	/// <param name="Index">The index of the sound associated with the SoundEffect.</param>
	/// <param name="Volume">The volume to play the sound at.</param>
	/// <returns>The duration of the played sound; zero if none.</returns>
	public float PlaySoundByIndex ( string SoundEffectName , int Index , float Volume = 1.0f ) {
		return PlaySoundInternal ( SoundEffectName , Index , Volume , transform.position );
	}

	/// <summary>
	/// Stops the specified associated SoundEffect. If the SoundEffect does
	/// not loop or is not playing at all, this method does nothing.
	/// </summary>
	/// <param name="SoundEffectName">The name of the looping and playing SoundEffect to stop.</param>
	public void StopLoopingSound ( string SoundEffectName ) {
		SoundEffect LoopingSoundEffect = this [ SoundEffectName ];

		// Do nothing if type not found or not a looping sound effect
		if ( LoopingSoundEffect == null || !LoopingSoundEffect.IsLooping ) {
			return;
		}

		// The sound effect can only be stopped if it is even playing.
		if ( LoopingAudioSources.ContainsKey ( LoopingSoundEffect ) ) {
			AudioSource LoopingAudioSource = LoopingAudioSources [ LoopingSoundEffect ];
			LoopingAudioSource.Stop ();

			LoopingAudioSources.Remove ( LoopingSoundEffect );
			Destroy ( LoopingAudioSource.gameObject );
		}
	}

	public void StopAllSounds () {
		StopAllCoroutines ();
		AudioSource.Stop ();
	}

	public void FadeIn ( float Duration = -1 ) {
		FadeGlobalVolume ( true , Duration );
	}

	public void FadeOut ( float Duration = -1 ) {
		FadeGlobalVolume ( false , Duration );
	}

	/// <summary>
	/// Recreates the internal dictionary using the current state of the SoundEffects array.
	/// </summary>
	public void RecreateDictionary () {
		SoundEffectDict.Clear ();

		foreach ( SoundEffect SoundEffect in SoundEffects ) {
			SoundEffectDict [ SoundEffect.Name ] = SoundEffect;
		}
	}

	private SoundEffect this [ string Name ] {
		get {
			return SoundEffectDict.ContainsKey ( Name ) ? SoundEffectDict [ Name ] : null;
		}
	}

	/// <summary>
	/// Plays the specified associated SoundEffect with the specified volume; a
	/// random associated sound is selected if Index = -1.
	/// </summary>
	/// <param name="SoundEffectName">The name of the SoundEffect to play.</param>
	/// <param name="Index">The index of the sound associated with the SoundEffect; use -1 for any sound.</param>
	/// <param name="Volume">The volume to play the sound at.</param>
	/// <returns>The duration of the played sound; zero if none.</returns>
	private float PlaySoundInternal ( string SoundEffectName , int Index , float Volume , Vector3 WorldPos ) {
		SoundEffect SoundEffect = this [ SoundEffectName ];
		AudioClip Clip = null;

		// Do nothing if type not found
		if ( SoundEffect == null ) {
			throw new KeyNotFoundException ( "Sound '" + SoundEffectName + "' not in sound database!" );
		}

		if ( SoundEffect.IsLooping && LoopingAudioSources.ContainsKey ( SoundEffect ) ) {
			return 0;
		}

		if ( Index >= 0 ) {
			Clip = SoundEffect.GetClip ( Index );
		}
		else { // Index <= -1
			Clip = SoundEffect.GetClip ();
		}

		if ( Clip != null ) {
			if ( SoundEffect.IsLooping ) {
				AudioSource LoopingAudioSource = CreateLoopingAudioSource ( SoundEffect );
				LoopingAudioSource.Stop ();
				LoopingAudioSource.clip = Clip;
				LoopingAudioSource.volume = Volume;
				LoopingAudioSource.Play ();

				StartCoroutine ( LoopClip ( SoundEffect , Clip.length ) );

				return Mathf.Infinity;
			}
			else {
				AudioSource.transform.position = WorldPos;
				AudioSource.PlayOneShot ( Clip , Volume );

				return Clip.length;
			}
		}
		else {
			throw new UnassignedReferenceException ( "Sound '" + SoundEffectName + "' does not have any assigned clips!" );
		}
	}

	private IEnumerator LoopClip ( SoundEffect CurrentSoundEffect , float CurrentDuration ) {
		AudioSource LoopingAudioSource = LoopingAudioSources [ CurrentSoundEffect ];

		yield return new WaitForSeconds ( CurrentDuration );

		LoopingAudioSource.Stop ();

		if ( CurrentSoundEffect.LoopParams.IntervalDelay > 0 ) {
			yield return new WaitForSeconds ( CurrentSoundEffect.LoopParams.IntervalDelay );
		}

		LoopingAudioSource.clip = CurrentSoundEffect.GetClip ();
		LoopingAudioSource.Play ();

		StartCoroutine ( LoopClip ( CurrentSoundEffect , LoopingAudioSource.clip.length ) );
	}

	private AudioSource CreateLoopingAudioSource ( SoundEffect LoopingSoundEffect ) {
		// TODO : Should use pooling instead creating new.

		// Just return the audio source if it is already created.
		if ( LoopingAudioSources.ContainsKey ( LoopingSoundEffect ) ) {
			return LoopingAudioSources [ LoopingSoundEffect ];
		}

		AudioSource LoopingAudioSource = new GameObject ( "Looping - " + LoopingSoundEffect.Name ).AddComponent<AudioSource> ();
		LoopingAudioSource.transform.SetParent ( transform );

		LoopingAudioSource.Stop ();
		LoopingAudioSource.loop = true;

		LoopingAudioSources.Add ( LoopingSoundEffect , LoopingAudioSource );

		return LoopingAudioSource;
	}

	private void FadeGlobalVolume ( bool IsFadeIn , float FadeDuration ) {
		FadeDuration = FadeDuration == -1 ? DefaultFadeDuration : FadeDuration;

		float StartVol = IsFadeIn ? 0 : 1;
		float EndVol = 1 - StartVol;

		//FadeTween = LeanTween.value ( gameObject , Vol => AudioListener.volume = Vol , StartVol , EndVol , FadeDuration );
	}

}