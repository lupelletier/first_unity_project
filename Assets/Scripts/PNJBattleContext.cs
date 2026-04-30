using UnityEngine;

public class PNJBattleContext : MonoBehaviour
{
	[Header("Arena Override")]
	public BattleArena battleArenaOverride;

	[Header("Spawn Overrides")]
	public Transform playerSpawnPoint;
	public Transform opponentSpawnPoint;

	[Header("Camera Overrides")]
	public Camera battleCamera;
	public Camera explorationCamera;

	[Header("Audio Overrides")]
	public AudioClip battleMusic;
}
