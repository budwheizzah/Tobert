using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

	public AudioClip[] sounds;
	public AudioClip[] damages;
	public AudioClip[] deaths;

	public int damage = 1;
	public int hp = 1;

	private bool isDeceased = false;
	private bool isDamaging = false;

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
				if (transform.position.y < Player.Instance.gameObject.transform.position.y)
				{
					MoveY(trackingRate);
				}
				else if (transform.position.y > Player.Instance.gameObject.transform.position.y)
				{
					MoveY(-trackingRate);
				}
			}
			yield return null;
		}
	}

	private void MoveY(float value)
	{
		Vector3 tmpPos = transform.position;
		tmpPos.y += value * Time.deltaTime;
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

		// If bitche dies, wait and then destroy
		if (Manager.Instance.gameState == Manager.GameState.Death)
		{
			yield return new WaitForSeconds(deathDelay);
			Destroy(gameObject);
		}
	}

	public void Damage(int hit)
	{
		if (isDamaging)
		{
			return;
		}

		isDamaging = true;
		hp -= hit;

		if (hp <= 0)
		{
			hp = 0;
			isDeceased = true;
			if (deaths.Length > 0)
			{
				Audio.Instance.EnemySound(deaths);
			}
			GetComponent<BoxCollider2D>().enabled = false;
			Manager.Instance.AddKills(1);
		}
		else
		{
			if (damages.Length > 0)
			{
				Audio.Instance.EnemySound(damages);
			}
		}

		StartCoroutine(DamageRoutine());
	}

	private IEnumerator DamageRoutine()
	{
		float totalTime = 0;
		bool lastToggle = false;

		while (totalTime < damageDelay)
		{
			yield return new WaitForSeconds(damageBlink);
			if (lastToggle)
			{
				spriteRenderer.color = Color.white;
			}
			else
			{
				spriteRenderer.color = damageColor;
			}
			lastToggle = !lastToggle;
			totalTime += damageBlink;
		}

		isDamaging = false;

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
