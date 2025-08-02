using System.Collections;
using UnityEngine;

public class Animation : MonoBehaviour
{
	[SerializeField]
	private float animTime = 0.6f;

	[SerializeField]
	private Sprite[] frames;

	[SerializeField]
	private SpriteRenderer me;

	[SerializeField]
	private bool startOnLaunch = true;

	private int frame = 0;

	public bool isFrozen = false;

	private void Start()
	{
		if (startOnLaunch)
		{
			Play();
		}
	}

	public void Play()
	{
		Stop();
		StartCoroutine(animateFrames());
	}

	public void Stop()
	{
		StopAllCoroutines();
	}

	private IEnumerator animateFrames()
	{
		while (Manager.Instance.isRunning)
		{
			if ((Manager.Instance.gameState == Manager.GameState.Playing) && (!isFrozen) && (gameObject.activeInHierarchy))
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
