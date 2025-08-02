using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
	public static Audio Instance;
	[SerializeField]
	private AudioSource[] enemySound;

	[SerializeField]
	private AudioSource playerSound;

	[SerializeField]
	private AudioSource backgroundSound;

	[SerializeField]
	private AudioSource weaponSound;

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

	private int enemyTrack = 0;

	private float lastMasterVolume = 0f;

	private List<AudioSource> enemySources = new List<AudioSource>();

	private void Awake()
	{
		Instance = this;

		masterVolume = PlayerPrefs.GetFloat("volumeMaster", masterVolume);
		backgroundVolume = PlayerPrefs.GetFloat("volumeBackground", backgroundVolume);
		enemyVolume = PlayerPrefs.GetFloat("volumeEnemies", enemyVolume);
		playerVolume = PlayerPrefs.GetFloat("volumePlayer", playerVolume);
		weaponVolume = PlayerPrefs.GetFloat("volumePlayer", weaponVolume);

		for (int i =0; i < enemyTracks; i++)
		{
			enemySources.Add(gameObject.AddComponent<AudioSource>());
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

	public void UpdateVolume()
	{
		for (int i = 0; i < enemyTracks; i++)
		{
			enemySources[i].volume = masterVolume * enemyVolume;
		}

		backgroundSound.volume = masterVolume * backgroundVolume;
		weaponSound.volume = masterVolume * weaponVolume;
		playerSound.volume = masterVolume * playerVolume;

		PlayerPrefs.SetFloat("volumeMaster", masterVolume);
		PlayerPrefs.SetFloat("volumeBackground", backgroundVolume);
		PlayerPrefs.SetFloat("volumeEnemies", enemyVolume);
		PlayerPrefs.SetFloat("volumePlayer", playerVolume);
		PlayerPrefs.SetFloat("volumeWeapon", weaponVolume);

		PlayerPrefs.Save();
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

	private void RandomizeSource(AudioSource aso, AudioClip[] aca)
	{
		if (aca.Length == 0)
		{
			Debug.LogWarning("Calling audio source to play nothing!");
			return;
		}
		PlayInterrupt(aso, aca[Random.Range(0, aca.Length)]);
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
