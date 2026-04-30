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

	public void Interact()
	{
		// This can be called by other systems; InteractionManager handles dialog flow.
		InteractionManager.Instance?.ShowDialog(this, FindObjectOfType<Player>());
	}
}
