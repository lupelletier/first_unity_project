using UnityEngine;

public class PNJ : MonoBehaviour
{
	public string npcName = "PNJ";
	[TextArea]
	public string[] dialogLines;
	public WishemonCard enemyCardOverride;
	public PNJBattleContext battleContext;
	public int goldReward = 10;
	public Transform postCombatSpawnPoint;
	public Camera dialogueCamera; // Optional: overrides the controller's default dialogue camera for this PNJ's interactions.

	// Temporarily hide the PNJ's renderers/animator during battles without disabling the whole GameObject.
	public void SetVisible(bool visible)
	{
		Renderer[] renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
		foreach (var r in renderers)
			r.enabled = visible;

		Animator anim = GetComponentInChildren<Animator>();
		if (anim != null)
			anim.enabled = visible;

		Collider[] cols = GetComponentsInChildren<Collider>(includeInactive: true);
		foreach (var c in cols)
			c.enabled = visible;
	}

	public void Interact()
	{
		// This can be called by other systems; InteractionManager handles dialog flow.
		InteractionManager.Instance?.ShowDialog(this, FindObjectOfType<Player>());
	}
}
