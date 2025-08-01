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

	private List<AudioSource> enemySources = new List<AudioSource>();

	private void Awake()
	{
		Instance = this;

		for (int i =0; i < enemyTracks; i++)
		{
			enemySources.Add(gameObject.AddComponent<AudioSource>());
		}
	}

	public void BackgroundSound(AudioClip ac)
	{
		backgroundSound.volume = backgroundVolume;
		PlayInterrupt(backgroundSound, ac);
	}

	public void WeaponSound(AudioClip ac = null)
	{
		if (ac == null)
		{
			weaponSound.Stop();
			return;
		}
		weaponSound.volume = weaponVolume;
		PlayInterrupt(weaponSound, ac);
	}

	public void PlayerSound(AudioClip[] aca)
	{
		playerSound.volume = playerVolume;
		RandomizeSource(playerSound, aca);
	}

	public void EnemySound(AudioClip[] aca)
	{
		enemySources[enemyTrack].volume = enemyVolume;
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
