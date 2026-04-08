using UnityEngine;

public class JukeboxButton : Obstacle
{
	private Jukebox jukebox;

	private void Start()
	{
		jukebox = GetComponentInParent<Jukebox>();
	}

	[ContextMenu("테스트")]
	public override void Hit(Vector3 hitPoint)
	{
		if (jukebox != null)
		{
			jukebox.ButtonHit();
		}
	}
	
	protected override void UniqueInteraction(){}
	protected override void OnTriggerEnter(Collider other){}
}
