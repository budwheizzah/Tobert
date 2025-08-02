using System;
using System.Collections;
using UnityEngine;

public class Scroller : MonoBehaviour
{
	private enum LocalLayer { Behind, Front, Nothing };

	[SerializeField]
	private float minimumHorizontal = -10f;

	[SerializeField]
	public float spawnHorizontal = 10f;

	[SerializeField]
	public float spawnLowest = 1f; //spawn at y=

	[SerializeField]
	public float spawnHeight = 3f; //spawn at spawnLowest + spawnHeight max

	[SerializeField]
	private float speedMinimum = 1f;

	[SerializeField]
	private float speedVariance = 1f;

	[SerializeField]
	public float bitcheBehind = -0.04f;

	[SerializeField]
	public float bitcheFront = -0.06f;

	private float speedLocal = 0f;

	public bool isStopped = false;

	public Action onOverflow;

	private LocalLayer localLayer = LocalLayer.Nothing;

	protected virtual void Start()
	{
		speedLocal = UnityEngine.Random.Range(speedMinimum, speedMinimum + speedVariance);
		StartCoroutine(ScrollObject());
	}

	private IEnumerator ScrollObject()
	{
		while (Manager.Instance.isRunning)
		{
			if ((Manager.Instance.gameState == Manager.GameState.Playing) && (!isStopped))
			{
				Vector3 tmpPos = transform.position;
				tmpPos.x -= Manager.Instance.GetSpeed(Manager.EnvtLayer.Foreground) * speedLocal;
				transform.position = tmpPos;
				if (tmpPos.x <= minimumHorizontal)
				{
					onOverflow?.Invoke();
					Destroy(gameObject);
				}
			}

			float layerThreshold = Player.Instance.GetVertical();
			float layerCompare = transform.position.y;
			if ((layerCompare > layerThreshold) && (localLayer != LocalLayer.Behind))// This means we are BEHIND bitche
			{
				localLayer = LocalLayer.Behind;
				SetLayer(bitcheBehind);
			}
			else if ((layerCompare <= layerThreshold) && (localLayer != LocalLayer.Front)) // Front of bitche
			{
				localLayer = LocalLayer.Front;
				SetLayer(bitcheFront);
			}

			yield return null;
		}
	}

	private void SetLayer(float lv)
	{
		Vector3 tmpPos = transform.position;
		tmpPos.z = lv;
		transform.position = tmpPos;
	}

}
