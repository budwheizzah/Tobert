using UnityEngine;

public class Damager : MonoBehaviour
{
	[SerializeField]
	private int damage = 3;

	private void OnTriggerEnter(Collider collision)
	{
		Enemy enemy = collision.gameObject.GetComponent<Enemy>();
		if (enemy != null)
		{
			enemy.Damage(damage);
		}
	}
}
