using UnityEngine;

public class Pickup : Scroller
{
	public enum PickupType { Health, Gas, Slowdown, Pipebomb };

	[SerializeField]
	public PickupType pickupType = PickupType.Gas;

	[SerializeField, Tooltip("HP for health, Gas points for gas and seconds for timed pickups")]
	public int value;
}
