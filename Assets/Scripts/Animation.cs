using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour
{
	[SerializeField]
	private float animTime = 0.6f;

	[SerializeField]
	private Sprite[] frames;

	[SerializeField]
	private SpriteRenderer me;

	private int frame = 0;

	public bool isFrozen = false;

	private void Start()
	{
		StartCoroutine(animateFrames());
	}

	private IEnumerator animateFrames()
	{
		while (Manager.Instance.isRunning)
		{
			if ((Manager.Instance.gameState == Manager.GameState.Playing) && (!isFrozen))
			{
				me.sprite = frames[frame];
				frame++;
				if (frame >= frames.Length)
				{
					frame = 0;
				}
				yield return new WaitForSeconds(animTime);
			}
			yield return null;
		}

	}
}
