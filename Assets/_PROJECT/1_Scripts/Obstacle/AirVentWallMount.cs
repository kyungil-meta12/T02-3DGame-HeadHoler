using UnityEngine;

public class AirVentWallMount : Obstacle
{
	public AirVentBase baseBody;

	protected override void UniqueInteraction()
	{
		if (baseBody != null)
		{
			baseBody.StartFalling();
		}

		base.UniqueInteraction();
	}
}
