using System;
using System.Collections;
using UnityEngine;

public class Damageable
{
	private bool isDamaging = false;

	public int Damage(int hp, int hit, bool postHitInvincibility, Action<int> onDamage = null, Action onDie = null)
	{
		if (isDamaging)
		{
			return hp;
		}

		isDamaging = postHitInvincibility;

		hp -= hit;

		if (hp <= 0)
		{
			hp = 0;
			onDie?.Invoke();
		}
		else
		{
			onDamage?.Invoke(hp);
		}

		return hp;
	}

	public IEnumerator DamageRoutine(SpriteRenderer spriteRenderer, float damageDelay, float damageBlink, Color damageColor, Action onComplete = null)
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

		onComplete?.Invoke();
	}
}
