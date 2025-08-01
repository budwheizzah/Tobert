using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public static Player Instance;

	[Header("Player values")]
	public int hp = 3;
	public int damage = 1;

	[SerializeField]
	private float damageDelay = 0.8f;

	[SerializeField]
	private Color damageColor = Color.red;

	[SerializeField]
	private float damageBlink = 0.1f;

	[Header("Movement bounds and rate")]
	[SerializeField]
	private float top = 1.5f;

	[SerializeField]
	private float bottom = -2.4f;

	[SerializeField]
	private float left = -7.5f;

	[SerializeField]
	private float right = -4.5f;

	[SerializeField,Range(1f,9f)]
	private float moveRate = 3f;

	[Header("Gasoline and rate")]
	[SerializeField]
	private float gas = 50f;

	[SerializeField, Range(1f, 100f)]
	private float gasRate = 40f;

	[SerializeField]
	private GameObject flames;

	[Header("Elements")]
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	public Animation animate;

	[SerializeField]
	private AudioClip[] audioDamage;

	[SerializeField]
	private AudioClip[] audioDie;

	[SerializeField]
	private AudioClip[] audioReplenish;

	[SerializeField]
	private AudioClip audioWeapon;

	[Header("Dimensions")]
	[SerializeField,Tooltip("Offset to apply when reporting Y position to other scripts through GetVertical()")]
	private float offsetVertical = -1f;

	private float vertical = -1.7f;
	private float horizontal = -4f;

	public Action onOutOfGas;
	public Action<float> onUpdateGas;
	public Action onStartGas;
	public Action onEndGas;
	public Action onReplenishGas;
	public Action<int> onDamage;
	public Action onFail;

	private bool isDamaging = false;
	private bool isFiring = false;
	private bool wasFiring = false;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		MoveTo(vertical, horizontal);
		flames.SetActive(false);
		onUpdateGas?.Invoke(gas); // just report the initial gas amount to main
		animate.isFrozen = false;
	}

	private void Update()
	{
		if (Manager.Instance.gameState == Manager.GameState.Playing)
		{
			processInputs();

			if (wasFiring != isFiring)
			{
				if (isFiring)
				{
					onStartGas?.Invoke();
					Audio.Instance.WeaponSound(audioWeapon);
				}
				else
				{
					flames.SetActive(false);
					onEndGas?.Invoke();
					Audio.Instance.WeaponSound(); // null/blank = stop
				}
			}

			wasFiring = isFiring;
		}
	}

	private void processInputs()
	{
		if (Input.GetKey(KeyCode.W))
		{
			Vertical(true);
		}
		else if (Input.GetKey(KeyCode.S))
		{
			Vertical();
		}

		if (Input.GetKey(KeyCode.A))
		{
			Horizontal();
		}
		else if (Input.GetKey(KeyCode.D))
		{
			Horizontal(true);
		}

		if (Input.GetKey(KeyCode.Space))
		{
			isFiring = Fire();
		}
		else
		{
			isFiring = false;
		}
	}

	private void Vertical(bool up = false)
	{
		vertical += (up ? moveRate : -moveRate) * Time.deltaTime;
		if (vertical > top)
		{
			vertical = top;
		}
		else if (vertical < bottom)
		{
			vertical = bottom;
		}
		MoveTo(vertical, horizontal);
	}

	private void Horizontal(bool forward = false)
	{
		horizontal += (forward ? moveRate : -moveRate) * Time.deltaTime;
		if (horizontal > right)
		{
			horizontal = right;
		}
		else if (horizontal < left)
		{
			horizontal = left;
		}
		MoveTo(vertical, horizontal);
	}

	private void MoveTo(float vertical, float horizontal)
	{
		Vector3 tempPos = transform.position;
		tempPos.y = vertical;
		tempPos.x = horizontal;
		transform.position = tempPos;
	}

	private bool Fire()
	{
		gas -= Time.deltaTime * gasRate;
		if (gas > 0)
		{
			flames.SetActive(true);
			onUpdateGas?.Invoke(gas);
			return true;
		}
		else
		{
			gas = 0;
			onUpdateGas?.Invoke(0);
			onOutOfGas?.Invoke();
			return false;
		}
	}

	public void Replenish(float amount)
	{
		gas += amount;
		if (audioReplenish.Length > 0)
		{
			Audio.Instance.EnemySound(audioReplenish); // Use enemy for multitrack
		}
		onUpdateGas?.Invoke(gas);
	}

	public float GetVertical()
	{
		return transform.position.y + offsetVertical;
	}

	private void Damage(int hit)
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
			if (audioDie.Length > 0)
			{
				Audio.Instance.PlayerSound(audioDie);
			}

			onFail?.Invoke();
			animate.isFrozen = true;
		}
		else
		{
			if (audioDamage.Length > 0)
			{
				Audio.Instance.PlayerSound(audioDamage);
			}
		}

		StartCoroutine(DamageRoutine());
		onDamage?.Invoke(hp);
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
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Enemy enemy = collision.gameObject.GetComponent<Enemy>();
		if (enemy != null)
		{
			Damage(enemy.damage);
			enemy.Damage(damage);
			return;
		}

		Pickup item = collision.gameObject.GetComponent<Pickup>();
		if (item != null)
		{
			switch (item.pickupType)
			{
				case Pickup.PickupType.Gas:
					Replenish(item.value);
					Destroy(item.gameObject);
					break;
				// TBD health? Maybe not.
			}
			return;
		}
	}

	private void OnDestroy()
	{
		animate.isFrozen = true;
	}
}
