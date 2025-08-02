using System;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
	private const string prefTag = "_";

	public static Audio Instance;
	[SerializeField]
	private AudioSource[] enemySound;

	[SerializeField]
	private AudioSource playerSound;

	[SerializeField]
	private AudioSource backgroundSound;

	[SerializeField]
	private AudioSource weaponSound;

	[SerializeField]
	private AudioSource[] auxiliarySound; // Will use player volume


	[SerializeField, Range(0, 1)]
	private float masterVolume = 1f;

	[SerializeField, Range(0,1)]
	private float backgroundVolume = 0.5f;

	[SerializeField, Range(0, 1)]
	private float playerVolume = 0.5f;

	[SerializeField, Range(0, 1)]
	private float enemyVolume = 0.5f;

	[SerializeField, Range(0, 1)]
	private float weaponVolume = 0.5f;

	[SerializeField]
	private int enemyTracks = 4;

	[SerializeField]
	private int auxiliaryTracks = 4;

	private int enemyTrack = 0;
	private int auxiliaryTrack = 0;

	private float lastMasterVolume = 0f;

	private List<AudioSource> enemySources = new List<AudioSource>();
	private List<AudioSource> auxiliarySources = new List<AudioSource>();

	public Action<float> onUpdateVolume;

	private void Awake()
	{
		Instance = this;

		masterVolume = PlayerPrefs.GetFloat("volumeMaster" + prefTag, masterVolume);
		backgroundVolume = PlayerPrefs.GetFloat("volumeBackground" + prefTag, backgroundVolume);
		enemyVolume = PlayerPrefs.GetFloat("volumeEnemies" + prefTag, enemyVolume);
		playerVolume = PlayerPrefs.GetFloat("volumePlayer" + prefTag, playerVolume);
		weaponVolume = PlayerPrefs.GetFloat("volumePlayer" + prefTag, weaponVolume);

		int i = 0;
		for (i =0; i < enemyTracks; i++)
		{
			enemySources.Add(gameObject.AddComponent<AudioSource>());
		}

		for (i = 0; i < auxiliaryTracks; i++)
		{
			auxiliarySources.Add(gameObject.AddComponent<AudioSource>());
		}
	}

	private void Update()
	{
		if (lastMasterVolume != masterVolume)
		{
			UpdateVolume();
			lastMasterVolume = masterVolume;
		}
	}

	public void VaryVolume(float variance)
	{
		masterVolume += variance;
		if (masterVolume > 1)
		{
			masterVolume = 1;
		}
		else if (masterVolume < 0)
		{
			masterVolume = 0;
		}
		UpdateVolume();
	}

	private void UpdateVolume()
	{
		int i = 0;
		for (i = 0; i < enemyTracks; i++)
		{
			enemySources[i].volume = masterVolume * enemyVolume;
		}
		for (i = 0; i < auxiliaryTracks; i++)
		{
			auxiliarySources[i].volume = masterVolume * playerVolume;
		}

		backgroundSound.volume = masterVolume * backgroundVolume;
		weaponSound.volume = masterVolume * weaponVolume;
		playerSound.volume = masterVolume * playerVolume;

		PlayerPrefs.SetFloat("volumeMaster" + prefTag, masterVolume);
		PlayerPrefs.SetFloat("volumeBackground" + prefTag, backgroundVolume);
		PlayerPrefs.SetFloat("volumeEnemies" + prefTag, enemyVolume);
		PlayerPrefs.SetFloat("volumePlayer" + prefTag, playerVolume);
		PlayerPrefs.SetFloat("volumeWeapon" + prefTag, weaponVolume);

		PlayerPrefs.Save();

		onUpdateVolume?.Invoke(masterVolume);
	}

	public void UpdateRate(float rate)
	{
		for (int i = 0; i < enemyTracks; i++)
		{
			enemySources[i].pitch = rate;
		}

		backgroundSound.pitch = rate;
		// Leave these out of slowdowns
		// weaponSound.volume = masterVolume * weaponVolume;
		// playerSound.volume = masterVolume * playerVolume;
	}

	public void BackgroundSound(AudioClip ac)
	{
		backgroundSound.volume = masterVolume * backgroundVolume;
		PlayInterrupt(backgroundSound, ac);
	}

	public void WeaponSound(AudioClip ac = null)
	{
		if (ac == null)
		{
			weaponSound.Stop();
			return;
		}
		weaponSound.volume = masterVolume * weaponVolume;
		PlayInterrupt(weaponSound, ac);
	}

	public void PlayerSound(AudioClip[] aca)
	{
		playerSound.volume = masterVolume * playerVolume;
		RandomizeSource(playerSound, aca);
	}

	public void EnemySound(AudioClip[] aca)
	{
		enemySources[enemyTrack].volume = masterVolume * enemyVolume;
		RandomizeSource(enemySources[enemyTrack], aca);
		enemyTrack++;
		if (enemyTrack >= enemySources.Count)
		{
			enemyTrack = 0;
		}
	}

	public void AuxiliarySound(AudioClip[] aca)
	{
		auxiliarySources[auxiliaryTrack].volume = masterVolume * playerVolume;
		RandomizeSource(auxiliarySources[auxiliaryTrack], aca);
		auxiliaryTrack++;
		if (auxiliaryTrack >= auxiliarySources.Count)
		{
			auxiliaryTrack = 0;
		}
	}

	private void RandomizeSource(AudioSource aso, AudioClip[] aca)
	{
		if (aca.Length == 0)
		{
			Debug.LogWarning("Calling audio source to play nothing!");
			return;
		}
		PlayInterrupt(aso, aca[UnityEngine.Random.Range(0, aca.Length)]);
	}

	private void PlayInterrupt(AudioSource aso, AudioClip ac)
	{
		if (aso.isPlaying)
		{
			aso.Stop();
		}

		aso.clip = ac;
		aso.Play();
	}
}
