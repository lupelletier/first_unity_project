using System.Collections;
using UnityEngine;

public class BattleArena : MonoBehaviour
{
	[SerializeField] private Transform _enemySpawnPoint;
	[SerializeField] private Transform _playerSpawnPoint;
	[SerializeField] private Camera _battleCamera;
	[SerializeField] private Camera _mainCamera;

	private GameObject _spawnedEnemy;
	private GameObject _spawnedPlayer;
	private Camera _activeBattleCamera;
	private Camera _activeMainCamera;

	public void SetupArena(WishemonCard playerCard, WishemonCard enemyCard, Transform playerSpawnOverride = null, Transform enemySpawnOverride = null, Camera battleCameraOverride = null, Camera mainCameraOverride = null)
	{
		_activeMainCamera = mainCameraOverride != null ? mainCameraOverride : _mainCamera;
		_activeBattleCamera = battleCameraOverride != null ? battleCameraOverride : _battleCamera;
		Transform resolvedPlayerSpawn = playerSpawnOverride != null ? playerSpawnOverride : _playerSpawnPoint;
		Transform resolvedEnemySpawn = enemySpawnOverride != null ? enemySpawnOverride : _enemySpawnPoint;

		if (_activeMainCamera != null)
			_activeMainCamera.gameObject.SetActive(false);
		if (_activeBattleCamera != null)
			_activeBattleCamera.gameObject.SetActive(true);

		if (enemyCard.Prefab != null && resolvedEnemySpawn != null)
		{
			_spawnedEnemy = Instantiate(enemyCard.Prefab, resolvedEnemySpawn.position, resolvedEnemySpawn.rotation);
			TrySetTrigger(_spawnedEnemy, "Idle");
		}

		if (playerCard.Prefab != null && resolvedPlayerSpawn != null)
		{
			_spawnedPlayer = Instantiate(playerCard.Prefab, resolvedPlayerSpawn.position, resolvedPlayerSpawn.rotation);
			TrySetTrigger(_spawnedPlayer, "Idle");
		}
	}

	public void ClearArena()
	{
		if (_activeBattleCamera != null)
			_activeBattleCamera.gameObject.SetActive(false);
		if (_activeMainCamera != null)
			_activeMainCamera.gameObject.SetActive(true);

		if (_spawnedEnemy != null) Destroy(_spawnedEnemy);
		if (_spawnedPlayer != null) Destroy(_spawnedPlayer);

		_activeBattleCamera = null;
		_activeMainCamera = null;
	}

	public void PlayPlayerAttack() => TrySetTrigger(_spawnedPlayer, "Attack");
	public void PlayEnemyAttack()  => TrySetTrigger(_spawnedEnemy,  "Attack");
	public void PlayEnemyDeath()   => TrySetTrigger(_spawnedEnemy,  "Death");
	public void PlayPlayerDeath()  => TrySetTrigger(_spawnedPlayer, "Death");
	public void PlayPlayerHit()
	{
		TrySetTrigger(_spawnedPlayer, "Hit");
		StartCoroutine(FlashHit(_spawnedPlayer));
	}

	public void PlayEnemyHit()
	{
		TrySetTrigger(_spawnedEnemy, "Hit");
		StartCoroutine(FlashHit(_spawnedEnemy));
	}

	private void TrySetTrigger(GameObject go, string triggerName)
	{
		if (go == null) return;
		Animator anim = go.GetComponent<Animator>();
		if (anim != null) anim.SetTrigger(triggerName);
	}

	private IEnumerator FlashHit(GameObject go)
	{
		if (go == null) yield break;
		Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
		Color[] original = new Color[renderers.Length];

		for (int i = 0; i < renderers.Length; i++)
		{
			original[i] = renderers[i].material.color;
			renderers[i].material.color = Color.red;
		}

		yield return new WaitForSeconds(0.2f);

		for (int i = 0; i < renderers.Length; i++)
			renderers[i].material.color = original[i];
	}
}
