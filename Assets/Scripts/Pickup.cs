using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : Scroller
{
	public enum PickupType { Health, Gas };

	[SerializeField]
	public PickupType pickupType = PickupType.Gas;

	[SerializeField]
	public int value;
}
