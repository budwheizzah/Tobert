using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
	[SerializeField]
	private GameObject healthPrefab;

	private int lastHp = 0;
	private List<Image> images = new List<Image>();

	public void Initialize(int hp)
	{
		if (hp > lastHp)
		{
			lastHp = hp;
		}

		for (int i = 0; i < hp; i++)
		{
			images.Add(Instantiate(healthPrefab, transform).GetComponent<Image>());
		}
	}

	public void SetHealth(int hp)
	{
		for (int i = 0; i < lastHp; i++)
		{
			if (i >= hp)
			{
				images[i].color = new Color(0, 0, 0, 0.2f);
			}
			else
			{
				images[i].color = Color.white;
			}
		}
	}

}
