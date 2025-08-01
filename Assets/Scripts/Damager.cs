using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour
{
	[SerializeField]
	private int damage = 3;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Enemy enemy = collision.gameObject.GetComponent<Enemy>();
		if (enemy != null)
		{
			enemy.Damage(damage);
		}
	}
}
