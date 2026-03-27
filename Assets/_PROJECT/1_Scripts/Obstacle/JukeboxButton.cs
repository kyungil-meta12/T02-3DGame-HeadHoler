using UnityEngine;

public class JukeboxButton : MonoBehaviour
{
	private Jukebox jukebox;

	private void Start()
	{
		jukebox = GetComponentInParent<Jukebox>();
	}

	[ContextMenu("테스트")]
	public void OnButtonHit()
	{
		if (jukebox != null)
		{
			jukebox.ButtonHit();
		}
	}
}
