using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public static Player Instance;

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
	private Animation animate;

	[Header("Dimensions")]
	[SerializeField,Tooltip("Offset to apply when reporting Y position to other scripts through GetVertical()")]
	private float offsetVertical = -1f;

	private float vertical = 0f;
	private float horizontal = -6.5f;

	public Action onOutOfGas;
	public Action<float> onConsumeGas;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		MoveTo(vertical, horizontal);
		onConsumeGas?.Invoke(gas); // just report the initial gas amount to main
	}

	private void Update()
	{
		flames.SetActive(false);

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
			Fire();
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

	private void Fire()
	{
		gas -= Time.deltaTime * gasRate;
		if (gas > 0)
		{
			onConsumeGas?.Invoke(gas);
			flames.SetActive(true);
		}
		else
		{
			onOutOfGas?.Invoke();
			gas = 0;
		}
	}

	public void Replenish(float amount)
	{
		gas += amount;
	}

	public float GetVertical()
	{
		return transform.position.y + offsetVertical;
	}

	private void OnDestroy()
	{
		animate.isFrozen = true;
	}
}
