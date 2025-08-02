using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Controls : MonoBehaviour
{
	public static Controls Instance;
	public enum ButtonType { Up, Down, Left, Right, Fire };

	private Dictionary<ButtonType, Touch> buttonStates = new Dictionary<ButtonType, Touch>();

	[SerializeField]
	private Touch up;

	[SerializeField]
	private Touch down;

	[SerializeField]
	private Touch left;

	[SerializeField]
	private Touch right;

	[SerializeField]
	private Touch fire;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		buttonStates = new Dictionary<ButtonType, Touch>();
		buttonStates.Add(ButtonType.Up, up);
		buttonStates.Add(ButtonType.Down, down);
		buttonStates.Add(ButtonType.Left, left);
		buttonStates.Add(ButtonType.Right, right);
		buttonStates.Add(ButtonType.Fire, fire);

		//up.onClick.AddListener(()=> { Debug.LogError("Assraper"); });
	}

	public bool ButtonTouch(ButtonType bt)
	{
		return buttonStates[bt].touchState;
	}

}
