using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
	public static Manager Instance;
	public static bool skipIntro = false;

	public enum GameState { Intro, Playing, Paused, Death };
	public enum EnvtLayer { Background, Midground, Foreground, Road, MultiplierOnly };

	private const float fadeRate = 1f;

	[Header("Speeds")]
	[SerializeField]
	private float currentSpeed = 0;

	[SerializeField]
	private float backgroundRate = 3f; // This is a strange offset to position conversion, so number shall vary

	[SerializeField]
	private float midgroundRate = 5f; // elements between game and background

	[SerializeField]
	private float foregroundRate = 6f; // enemies and such

	[Header("Background Rates")]
	[SerializeField]
	private float spawnBgRate = 1f;

	[SerializeField]
	private float spawnBgVariance = 4f;

	[SerializeField]
	private float spawnBgHeightVariance = 0.1f;

	[SerializeField]
	private float spawnMinimumX = -10f;

	[Header("Enemy Values")]
	[SerializeField]
	private float spawnEnRate = 0.8f;

	[SerializeField]
	private float spawnEnVariance = 6f;

	[Header("Pickup Values")]
	[SerializeField]
	private float spawnItemRate = 5f;

	[SerializeField]
	private float spawnItemVariance = 5f;

	[SerializeField]
	private float slowdownRate = 0.3f;

	[Header("Characters")]
	[SerializeField]
	private Player bitche;

	[Header("Elements")]
	[SerializeField]
	private GameObject background;

	[SerializeField]
	private SpriteRenderer road;

	[SerializeField]
	private GameObject pipebomb;

	[SerializeField]
	private Animation pipebombAnimate;

	[Header("Slots")]
	[SerializeField]
	private GameObject backgroundSpawn;

	[Header("Spawn Elements")]
	[SerializeField]
	private GameObject[] trees;

	[SerializeField]
	private GameObject[] enemies;

	[SerializeField]
	private GameObject[] items;

	[Header("UI")]
	[SerializeField]
	private Text gasLabel;

	[SerializeField]
	private Text killsLabel;

	[SerializeField]
	private Text dodgesLabel;

	[SerializeField]
	private Text timeLabel;

	[SerializeField]
	private Text[] volumeLabel;

	[SerializeField]
	private GameObject gasDepleted;

	[SerializeField]
	private Healthbar healthBar;

	[SerializeField]
	private CanvasGroup gameGroup;

	[SerializeField]
	private CanvasGroup titleGroup;

	[SerializeField]
	private CanvasGroup deathGroup;


	[Header("Audio")]
	[SerializeField]
	private AudioClip themeSong;

	[SerializeField]
	private AudioClip deathSong;

	[Header("Delays")]
	[SerializeField]
	private float titleDelay = 1;

	private List<GameObject> backgroundObjects;

	private float basicRate = 0.5f; //offset per second

	public GameState gameState = GameState.Intro;

	public bool isRunning = true;

	private int kills = 0;
	private int dodges = 0;

	private int totalSeconds = 0;

	private float speedMultiplier = 1f;

	private void Awake()
	{
		titleGroup.alpha = 1;
		gameGroup.alpha = 0;
		deathGroup.alpha = 0;

		pipebomb.SetActive(false);

		backgroundObjects = new List<GameObject>();
		Instance = this;
		bitche.onOutOfGas += AmmoDepleted;
		bitche.onUpdateGas += AmmoConsume;
		bitche.onReplenishGas += AmmoReplenish;
		bitche.onHealth += PlayerHealth;
		bitche.onFail += PlayerFail;

		healthBar.Initialize(bitche.hp);
	}

	private void Start()
	{
		gasDepleted.SetActive(false);
		Audio.Instance.onUpdateVolume += UpdateVolume;
		Audio.Instance.BackgroundSound(themeSong);

		if (skipIntro)
		{
			titleGroup.alpha = 0;
			gameGroup.alpha = 1;
		}

		StartCoroutine(StartDelayed());
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			Audio.Instance.VaryVolume(-0.05f);
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			Audio.Instance.VaryVolume(0.05f);
		}
	}

	public void AddKills(int k)
	{
		kills += k;
		killsLabel.text = kills.ToString();
	}

	public void AddDodges(int d)
	{
		dodges += d;
		dodgesLabel.text = dodges.ToString();
	}

	private IEnumerator StartDelayed()
	{
		if (!skipIntro)
		{
			while ((!Input.GetMouseButton(0)) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
			{
				yield return null;
			}

			StartCoroutine(FadeCanvas(titleGroup, false));
			StartCoroutine(FadeCanvas(gameGroup, true));
		}

		yield return new WaitForSeconds(titleDelay);

		gameState = GameState.Playing;
		bitche.animate.Play();

		StartCoroutine(Timer());
		StartCoroutine(SpawnBackground());
		StartCoroutine(SpawnEnemies());
		StartCoroutine(SpawnItems());
		StartCoroutine(ScrollBackground());
	}

	private void MoveObjects(List<GameObject> move)
	{
		for (int x=0; x< move.Count; x++)
		{
			GameObject bgo = move[x];
			Vector2 tmpPos = bgo.transform.position;
			tmpPos.x -= GetSpeed(EnvtLayer.Background);
			bgo.transform.position = tmpPos;
			if (tmpPos.x <= spawnMinimumX)
			{
				move.RemoveAt(x);
				Destroy(bgo);
				break;
			}
		}
	}

	private IEnumerator ScrollBackground()
	{
		while (isRunning)
		{
			if (gameState == GameState.Playing)
			{
				Vector2 currentOffset = road.material.mainTextureOffset;
				currentOffset.x += GetSpeed(EnvtLayer.Road);
				if (currentOffset.x > 1f)
				{
					currentOffset.x = 0;
				}
				road.material.mainTextureOffset = currentOffset;

				MoveObjects(backgroundObjects);
			}
			yield return null;
		}
	}

	private IEnumerator SpawnBackground()
	{
		while (isRunning)
		{
			if (gameState == GameState.Playing)
			{
				float waitTime = UnityEngine.Random.Range(spawnBgRate, spawnBgRate + spawnBgVariance);
				yield return new WaitForSeconds(waitTime);

				int spawnIndex = UnityEngine.Random.Range(0, trees.Length);
				Vector3 spawnPosition = backgroundSpawn.transform.position;
				spawnPosition.y = UnityEngine.Random.Range(spawnPosition.y, spawnPosition.y + spawnBgHeightVariance);
				GameObject tree = Instantiate(trees[spawnIndex], spawnPosition, backgroundSpawn.transform.rotation);
				backgroundObjects.Add(tree);
			}
			yield return null;
		}
	}

	private IEnumerator SpawnEnemies()
	{
		while (isRunning)
		{
			if (gameState == GameState.Playing)
			{
				float waitTime = UnityEngine.Random.Range(spawnEnRate, spawnEnRate + spawnEnVariance);
				yield return new WaitForSeconds(waitTime);

				int spawnIndex = UnityEngine.Random.Range(0, enemies.Length);
				Enemy e = enemies[spawnIndex].GetComponent<Enemy>();
				if (e != null)
				{
					Vector3 spawnPosition = new Vector3(e.spawnHorizontal, UnityEngine.Random.Range(e.spawnLowest, e.spawnLowest + e.spawnHeight), e.bitcheBehind);
					spawnPosition.y = UnityEngine.Random.Range(spawnPosition.y, spawnPosition.y);
					GameObject enemy = Instantiate(enemies[spawnIndex], spawnPosition, backgroundSpawn.transform.rotation);
				}
				else
				{
					Debug.LogError("Enemy spawn missing Enemy component");
				}
			}
			yield return null;
		}
	}

	private IEnumerator SpawnItems()
	{
		while (isRunning)
		{
			if (gameState == GameState.Playing)
			{
				float waitTime = UnityEngine.Random.Range(spawnItemRate, spawnItemRate + spawnItemVariance);
				yield return new WaitForSeconds(waitTime);

				int spawnIndex = UnityEngine.Random.Range(0, items.Length);
				Pickup pu = items[spawnIndex].GetComponent<Pickup>();
				if (pu != null)
				{
					Vector3 spawnPosition = new Vector3(pu.spawnHorizontal, UnityEngine.Random.Range(pu.spawnLowest, pu.spawnLowest + pu.spawnHeight), pu.bitcheBehind);
					spawnPosition.y = UnityEngine.Random.Range(spawnPosition.y, spawnPosition.y);
					GameObject item = Instantiate(items[spawnIndex], spawnPosition, backgroundSpawn.transform.rotation);
				}
				else
				{
					Debug.LogError("Item spawn missing Pickup component");
				}
			}
			yield return null;
		}
	}

	public float GetSpeed(EnvtLayer speedLayer)
	{
		float localRate = speedMultiplier * currentSpeed * Time.deltaTime;
		switch (speedLayer)
		{
			case EnvtLayer.Background:
				return basicRate * backgroundRate * localRate;
				break;
			case EnvtLayer.Midground:
				return basicRate * midgroundRate * localRate;
				break;
			case EnvtLayer.Foreground:
				return basicRate * foregroundRate * localRate;
				break;
			case EnvtLayer.MultiplierOnly:
				return speedMultiplier;
				break;
		}

		// The default is the scrolling road, the baseRate
		return basicRate * localRate;
	}

	private void OnDestroy()
	{
		bitche.onOutOfGas -= AmmoDepleted;
		bitche.onUpdateGas -= AmmoConsume;
		bitche.onReplenishGas -= AmmoReplenish;
		bitche.onHealth -= PlayerHealth;
		bitche.onFail -= PlayerFail;
		Audio.Instance.onUpdateVolume -= UpdateVolume;
	}

	private void UpdateVolume(float volume)
	{
		foreach (Text vl in volumeLabel)
		{
			vl.text = (volume * 100).ToString("0");
		}
	}

	private void AmmoDepleted()
	{
		//Debug.LogError("AMMO DEPLETED!");
		gasLabel.color = Color.red;
		if (!gasDepleted.activeSelf)
		{
			StartCoroutine(DismissGasWarning());
		}
		gasDepleted.SetActive(true);
	}

	private void AmmoConsume(float amount)
	{
		//Debug.LogError("AMMO LEFT " + amount);
		gasLabel.text = Mathf.Floor(amount).ToString("0");
	}

	private void AmmoReplenish()
	{
		//Debug.LogError("AMMO GIVE " + amount);
		gasLabel.color = Color.black;
	}

	private void PlayerHealth(int remain)
	{
		healthBar.SetHealth(remain);
	}

	private void PlayerFail()
	{
		gameState = GameState.Death;

		// Launch post mortem shit here
		Audio.Instance.BackgroundSound(deathSong);
		StartCoroutine(Die());
	}

	public void TimedPickup(Pickup.PickupType pt, int duration)
	{
		StartCoroutine(TimedPickupLifespan(pt, duration));
	}

	private IEnumerator TimedPickupLifespan(Pickup.PickupType pt, int duration)
	{
		switch(pt)
		{
			case Pickup.PickupType.Slowdown:
				speedMultiplier = slowdownRate;
				Audio.Instance.UpdateRate(speedMultiplier);
				yield return new WaitForSeconds(duration);
				speedMultiplier = 1;
				Audio.Instance.UpdateRate(speedMultiplier);
				break;
		}
		yield return null;
	}

	private IEnumerator Timer()
	{
		while (gameState != GameState.Death)
		{
			if (gameState == GameState.Playing)
			{
				TimeSpan timeSpan = TimeSpan.FromSeconds(totalSeconds);
				timeLabel.text = timeSpan.ToString(@"hh\:mm\:ss");
				totalSeconds++;
			}
			yield return new WaitForSeconds(1);
		}
	}

	private IEnumerator Die()
	{
		//StartCoroutine(FadeCanvas(gameGroup, false));

		yield return new WaitForSeconds(titleDelay);

		StartCoroutine(FadeCanvas(deathGroup, true));

		while (!Input.anyKey)
		{
			yield return null;
		}

		//Reload game
		skipIntro = true;
		SceneManager.LoadScene(0);
	}



	private IEnumerator DismissGasWarning()
	{
		yield return new WaitForSeconds(3);
		gasDepleted.SetActive(false);
	}

	public IEnumerator FirePipebomb(float duration)
	{
		pipebomb.SetActive(true);
		pipebombAnimate.Play();
		yield return new WaitForSeconds(duration);
		pipebombAnimate.Stop();
		pipebomb.SetActive(false);
	}


	private IEnumerator FadeCanvas(CanvasGroup cg, bool show)
	{
		float incr = fadeRate;
		if (!show)
		{
			incr = -incr;
		}

		float ca = cg.alpha + (incr * Time.deltaTime);
		while ((ca > 0) && (ca < 1))
		{
			ca += incr * Time.deltaTime;
			if (ca > 1)
			{
				ca = 1;
			}
			else if (ca < 0)
			{
				ca = 0;
			}
			cg.alpha = ca;
			yield return null;
		}
	}

}
