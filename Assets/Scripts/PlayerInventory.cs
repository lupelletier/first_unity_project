using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
	public int gold = 0;

	public void AddGold(int amount)
	{
		gold += amount;
		Debug.Log($"Gold added: {amount}. Total: {gold}");
		// TODO: update UI here if you have a HUD
	}
}
