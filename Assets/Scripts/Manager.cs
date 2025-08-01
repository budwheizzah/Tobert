using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
	public static Manager Instance;

	public enum GameState { Playing, Paused };
	public enum EnvtLayer { Background, Midground, Foreground, Road };

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

	[Header("Characters")]
	[SerializeField]
	private Player bitche;

	[Header("Elements")]
	[SerializeField]
	private GameObject background;

	[SerializeField]
	private SpriteRenderer road;

	[Header("Slots")]
	[SerializeField]
	private GameObject backgroundSpawn;

	[Header("Spawn Elements")]
	[SerializeField]
	private GameObject[] trees;

	[SerializeField]
	private GameObject[] enemies;

	[Header("UI")]
	[SerializeField]
	private Text gasLabel;

	[SerializeField]
	private GameObject gasDepleted;

	private List<GameObject> backgroundObjects;

	private float basicRate = 0.5f; //offset per second

	public GameState gameState = GameState.Playing;

	public bool isRunning = true;

	private void Awake()
	{
		backgroundObjects = new List<GameObject>();
		Instance = this;
		bitche.onOutOfGas += AmmoDepleted;
		bitche.onConsumeGas += AmmoConsume;
	}

	private void Start()
	{
		StartCoroutine(ScrollBackground());
		StartCoroutine(SpawnBackground());
		StartCoroutine(SpawnEnemies());
		gasDepleted.SetActive(false);
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
				float waitTime = Random.Range(spawnBgRate, spawnBgRate + spawnBgVariance);
				yield return new WaitForSeconds(waitTime);

				int spawnIndex = Random.Range(0, trees.Length);
				Vector3 spawnPosition = backgroundSpawn.transform.position;
				spawnPosition.y = Random.Range(spawnPosition.y, spawnPosition.y + spawnBgHeightVariance);
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
				float waitTime = Random.Range(spawnEnRate, spawnEnRate + spawnEnVariance);
				yield return new WaitForSeconds(waitTime);

				int spawnIndex = Random.Range(0, enemies.Length);
				Enemy e = enemies[spawnIndex].GetComponent<Enemy>();
				if (e != null)
				{
					Vector3 spawnPosition = new Vector3(e.spawnHorizontal, Random.Range(e.spawnLowest, e.spawnLowest + e.spawnHeight), e.bitcheBehind);
					spawnPosition.y = Random.Range(spawnPosition.y, spawnPosition.y);
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

	public float GetSpeed(EnvtLayer speedLayer)
	{
		switch (speedLayer)
		{
			case EnvtLayer.Background:
				return basicRate * backgroundRate * currentSpeed * Time.deltaTime;
				break;
			case EnvtLayer.Midground:
				return basicRate * midgroundRate * currentSpeed * Time.deltaTime;
				break;
			case EnvtLayer.Foreground:
				return basicRate * foregroundRate * currentSpeed * Time.deltaTime;
				break;

		}

		// The default is the scrolling road, the baseRate
		return basicRate * currentSpeed * Time.deltaTime;
	}

	private void OnDestroy()
	{
		bitche.onOutOfGas -= AmmoDepleted;
		bitche.onConsumeGas -= AmmoConsume;
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
		gasLabel.text = Mathf.Round(amount).ToString("0");
	}

	private void AmmoReplenish(float amount)
	{
		//Debug.LogError("AMMO GIVE " + amount);
		gasLabel.color = Color.white;
	}

	private IEnumerator DismissGasWarning()
	{
		yield return new WaitForSeconds(3);
		gasDepleted.SetActive(false);
	}

}
