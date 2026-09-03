using System.Collections;
using UnityEngine;

public class Enemy : Scroller
{
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private float soundInterval = 0.6f;

	[SerializeField]
	private float intervalVariance = 2f;

	[SerializeField]
	private float deathDelay = 3f;

	[SerializeField]
	private float damageDelay = 0.8f;

	[SerializeField]
	private Color damageColor = Color.red;

	[SerializeField]
	private float damageBlink = 0.1f;

	[SerializeField]
	private float trackingProbability = 0.15f;

	[SerializeField]
	private float trackingRate = 1f;

	[SerializeField]
	private float floor = 0.75f;

	public AudioClip[] sounds;
	public AudioClip[] damages;
	public AudioClip[] deaths;

	public int damage = 1;
	public int hp = 1;

	private bool isDeceased = false;
	private bool isDamaging = false;

	private Damageable damageControl = new Damageable();

	protected override void Start()
	{
		base.Start();

		onOverflow += OnDodged;
		
		float trackingDraw = Random.Range(0,1);
		if (trackingDraw <= trackingProbability)
		{
			StartCoroutine(Tracking());
		}

		if (sounds.Length > 0)
		{
			StartCoroutine(Lifespan());
		}
	}

	private IEnumerator Tracking()
	{
		while (!isDeceased)
		{
			if (Manager.Instance.gameState == Manager.GameState.Playing) 
			{
				float referencePosition = transform.position.y;
				float playerPosition = Player.Instance.gameObject.transform.position.y;

				if (referencePosition < playerPosition)
				{
					MoveByFlat(trackingRate);
				}
				else if (referencePosition > playerPosition)
				{
					MoveByFlat(-trackingRate);
				}

				if (Manager.Instance.is3D)
				{
					referencePosition = transform.position.z;
					playerPosition = Player.Instance.gameObject.transform.position.z;

					if (referencePosition < playerPosition)
					{
						MoveByDepth(trackingRate);
					}
					else if (referencePosition > playerPosition)
					{
						MoveByDepth(-trackingRate);
					}
				}

			}
			yield return null;
		}
	}

	private void MoveByFlat(float value)
	{
		Vector3 tmpPos = transform.position;
		float nextValue = tmpPos.y + (value * Time.deltaTime);
		if (floor < nextValue)
		{ 
			tmpPos.y = nextValue;
		}
		else
		{
			tmpPos.y = floor;
		}
		transform.position = tmpPos;
	}

	private void MoveByDepth(float value)
	{
		Vector3 tmpPos = transform.position;
		tmpPos.z += value * Time.deltaTime;
		transform.position = tmpPos;
	}

	private IEnumerator Lifespan()
	{
		// Play sounds while bitche is playing and this enemy not dead
		while ((!isDeceased) && (Manager.Instance.gameState != Manager.GameState.Death))
		{
			yield return new WaitForSeconds(Random.Range(soundInterval, soundInterval + intervalVariance));
			Audio.Instance.EnemySound(sounds);
		}
	}

	public void Damage(int hit)
	{
		hp = damageControl.Damage(hp, hit, false, Hit, Die);
		StartCoroutine(damageControl.DamageRoutine(spriteRenderer, damageDelay, damageBlink, damageColor, Postmortem));
	}

	private void Hit(int reportedHp)
	{
		if (damages.Length > 0)
		{
			Audio.Instance.EnemySound(damages);
		}
	}

	private void Die()
	{
		isDeceased = true;
		if (deaths.Length > 0)
		{
			Audio.Instance.EnemySound(deaths);
		}
		GetComponent<BoxCollider>().enabled = false;
		Manager.Instance.AddKills(1);
	}

	private void Postmortem()
	{
		if (isDeceased)
		{
			Destroy(gameObject);
		}
	}

	private void OnDodged()
	{
		Manager.Instance.AddDodges(1);
	}

	private void OnDestroy()
	{
		onOverflow -= OnDodged;
	}
}
