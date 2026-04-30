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

	public void SetupArena(WishemonCard playerCard, WishemonCard enemyCard)
	{
		_mainCamera.gameObject.SetActive(false);
		_battleCamera.gameObject.SetActive(true);

		if (enemyCard.Prefab != null)
		{
			_spawnedEnemy = Instantiate(enemyCard.Prefab, _enemySpawnPoint.position, _enemySpawnPoint.rotation);
			TrySetTrigger(_spawnedEnemy, "Idle");
		}

		if (playerCard.Prefab != null)
		{
			_spawnedPlayer = Instantiate(playerCard.Prefab, _playerSpawnPoint.position, _playerSpawnPoint.rotation);
			TrySetTrigger(_spawnedPlayer, "Idle");
		}
	}

	public void ClearArena()
	{
		_battleCamera.gameObject.SetActive(false);
		_mainCamera.gameObject.SetActive(true);

		if (_spawnedEnemy != null) Destroy(_spawnedEnemy);
		if (_spawnedPlayer != null) Destroy(_spawnedPlayer);
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
