using System.Collections;
using UnityEngine;

public enum BattleState { Inactive, TeamSelection, Fighting }

public class BattleManager : MonoBehaviour
{
	[SerializeField] private Player _player;
	[SerializeField] private TeamSelectionUI _teamSelectionUI;
	[SerializeField] private BattleUI _battleUI;
	[SerializeField] private BattleArena _battleArena;
	[SerializeField] private Transform _safePoint;
	[SerializeField] private WishemonCard[] _possibleEnemies;
	[SerializeField] private AudioSource _musicSource;
	[SerializeField] private AudioClip _defaultBattleMusic;
	[SerializeField] private AudioClip _explorationMusic;

	private WishemonCard _playerCard;
	private WishemonCard _enemyCard;
	private int _playerCurrentHP;
	private int _enemyCurrentHP;
	private BattleState _state = BattleState.Inactive;

	private PNJ _currentPNJ;
	private Transform _currentPostCombatSpawn;
	private PNJBattleContext _currentBattleContext;
	private BattleArena _currentBattleArena;
	private AudioClip _previousMusic;
	private bool _musicWasSwitched;

	public static BattleManager Instance;

	public bool IsBattleActive => _state != BattleState.Inactive;

	private void Awake()
	{
		Instance = this;
	}

	public bool TriggerEncounter(WishemonCard enemyCard, BattleArena battleArenaOverride = null, Transform postCombatSpawnOverride = null)
	{
		return _beginEncounter(enemyCard, null, postCombatSpawnOverride, null, battleArenaOverride);
	}

	public bool TriggerEncounter(PNJ pnj)
	{
		if (pnj == null)
			return false;

		WishemonCard enemyCard = pnj.enemyCardOverride != null ? pnj.enemyCardOverride : GetRandomFallbackEnemy();
		if (enemyCard == null)
		{
			Debug.LogWarning("PNJ encounter could not start because no enemy card is available.", this);
			return false;
		}

		BattleArena arenaOverride = pnj.battleContext != null ? pnj.battleContext.battleArenaOverride : null;
		return _beginEncounter(enemyCard, pnj, pnj.postCombatSpawnPoint, pnj.battleContext, arenaOverride);
	}

	private bool _beginEncounter(WishemonCard enemyCard, PNJ pnj, Transform postCombatSpawn, PNJBattleContext battleContext, BattleArena battleArenaOverride)
	{
		if (enemyCard == null)
		{
			Debug.LogWarning("Encounter ignored because enemy card is null.", this);
			return false;
		}

		_enemyCard = enemyCard;
		_currentPNJ = pnj;
		_currentPostCombatSpawn = postCombatSpawn;
		_currentBattleContext = battleContext;
		_currentBattleArena = battleArenaOverride != null ? battleArenaOverride : _battleArena;
		if (_currentBattleArena == null)
		{
			Debug.LogWarning("Encounter ignored because no BattleArena is assigned.", this);
			return false;
		}
		Debug.Log($"Encounter triggered with {enemyCard.Name}", this);
		_state = BattleState.TeamSelection;
		_player.SetMovementEnabled(false);
		// Hide the player and PNJ visuals for the duration of the battle so only arena-spawned prefabs are visible.
		_player.SetVisible(false);
		if (pnj != null)
			pnj.SetVisible(false);
		StartBattleMusic();
		_teamSelectionUI.Show(_player.Team, enemyCard);
		return true;
	}

	private void StartBattleMusic()
	{
		if (_musicSource == null)
			return;

		AudioClip encounterClip = _defaultBattleMusic;
		if (_currentBattleContext != null && _currentBattleContext.battleMusic != null)
			encounterClip = _currentBattleContext.battleMusic;

		if (encounterClip == null)
			return;

		if (!_musicWasSwitched)
		{
			_previousMusic = _musicSource.clip;
			_musicWasSwitched = true;
		}

		if (_musicSource.clip != encounterClip)
		{
			_musicSource.clip = encounterClip;
			_musicSource.Play();
		}
	}

	private void RestoreExplorationMusic()
	{
		if (_musicSource == null || !_musicWasSwitched)
			return;

		AudioClip restoreClip = _explorationMusic != null ? _explorationMusic : _previousMusic;
		if (restoreClip != null && _musicSource.clip != restoreClip)
		{
			_musicSource.clip = restoreClip;
			_musicSource.Play();
		}

		_previousMusic = null;
		_musicWasSwitched = false;
	}

	private WishemonCard GetRandomFallbackEnemy()
	{
		if (_possibleEnemies == null || _possibleEnemies.Length == 0)
			return null;

		return _possibleEnemies[Random.Range(0, _possibleEnemies.Length)];
	}

	public void OnTeamMemberSelected(WishemonCard playerCard)
	{
		_playerCard = playerCard;
		_playerCurrentHP = playerCard.PV;
		_enemyCurrentHP = _enemyCard.PV;
		_state = BattleState.Fighting;

		_teamSelectionUI.Hide();
		Transform playerSpawnOverride = _currentBattleContext != null ? _currentBattleContext.playerSpawnPoint : null;
		Transform enemySpawnOverride = _currentBattleContext != null ? _currentBattleContext.opponentSpawnPoint : null;
		Camera battleCameraOverride = _currentBattleContext != null ? _currentBattleContext.battleCamera : null;
		Camera mainCameraOverride = _currentBattleContext != null ? _currentBattleContext.explorationCamera : null;
		_currentBattleArena.SetupArena(playerCard, _enemyCard, playerSpawnOverride, enemySpawnOverride, battleCameraOverride, mainCameraOverride);
		_battleUI.Show(playerCard, _enemyCard, _playerCurrentHP, _enemyCurrentHP);
	}

	public void OnAttackPressed()
	{
		if (_state != BattleState.Fighting) return;
		StartCoroutine(AttackSequence());
	}

	private IEnumerator AttackSequence()
	{
		_battleUI.SetButtonsInteractable(false);

		// Player attacks
		_currentBattleArena.PlayPlayerAttack();
		int dmg = Mathf.Max(1, _playerCard.Attack - _enemyCard.Defense);
		_enemyCurrentHP -= dmg;
		_currentBattleArena.PlayEnemyHit();
		_battleUI.SetBattleLog($"You dealt {dmg} damage!");
		_battleUI.UpdateEnemyHP(_enemyCurrentHP, _enemyCard.PV);

		if (_enemyCurrentHP <= 0)
		{
			yield return new WaitForSeconds(1f);
			_currentBattleArena.PlayEnemyDeath();
			EndBattle($"You defeated {_enemyCard.Name}!", won: true);
			yield break;
		}

		// Pause before enemy turn
		yield return new WaitForSeconds(1f);

		// Enemy attacks
		_currentBattleArena.PlayEnemyAttack();
		int enemyDmg = Mathf.Max(1, _enemyCard.Attack - _playerCard.Defense);
		_playerCurrentHP -= enemyDmg;
		_currentBattleArena.PlayPlayerHit();
		_battleUI.SetBattleLog($"{_enemyCard.Name} dealt {enemyDmg} damage!");
		_battleUI.UpdatePlayerHP(_playerCurrentHP, _playerCard.PV);

		if (_playerCurrentHP <= 0)
		{
			yield return new WaitForSeconds(1f);
			_currentBattleArena.PlayPlayerDeath();
			EndBattle("You were defeated...", won: false);
			yield break;
		}

		_battleUI.SetButtonsInteractable(true);
	}

	public void OnRunPressed()
	{
		if (_state != BattleState.Fighting) return;
		EndBattle("Got away safely!", won: false);
	}

	private void EndBattle(string resultMessage, bool won)
	{
		if (won && _currentPNJ != null)
		{
			var inventory = _player.GetComponent<PlayerInventory>();
			if (inventory != null && _currentPNJ.goldReward > 0)
				inventory.AddGold(_currentPNJ.goldReward);
		}

		_state = BattleState.Inactive;
		_player.SetMovementEnabled(true);
		// Restore visuals
		_player.SetVisible(true);
		if (_currentPNJ != null)
			_currentPNJ.SetVisible(true);

		if (_currentPostCombatSpawn != null)
			_player.Teleport(_currentPostCombatSpawn.position);
		else if (_safePoint != null)
			_player.Teleport(_safePoint.position);
		RestoreExplorationMusic();
		foreach (TallGrass tg in FindObjectsByType<TallGrass>(FindObjectsSortMode.None))
			tg.ForceExit();
		_battleUI.ShowEndMessage(resultMessage);
		StartCoroutine(HideBattleAfterDelay());
		// exploration camera will be restored after the arena is cleared to avoid camera conflicts
		_currentPNJ = null;
		_currentPostCombatSpawn = null;
		_currentBattleContext = null;
	}

	private IEnumerator HideBattleAfterDelay()
	{
		yield return new WaitForSeconds(2f);
		_battleUI.Hide();
		if (_currentBattleArena != null)
			_currentBattleArena.ClearArena();
		_currentBattleArena = null;

		if (EncounterCameraController.Instance != null)
			EncounterCameraController.Instance.SetModeExploration();
	}
}