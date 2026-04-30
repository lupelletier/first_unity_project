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

	private WishemonCard _playerCard;
	private WishemonCard _enemyCard;
	private int _playerCurrentHP;
	private int _enemyCurrentHP;
	private BattleState _state = BattleState.Inactive;

	public bool IsBattleActive => _state != BattleState.Inactive;

	public void TriggerEncounter(WishemonCard enemyCard)
	{
		_enemyCard = enemyCard;
		Debug.Log($"Encounter triggered with {enemyCard.Name}", this);
		_state = BattleState.TeamSelection;
		_player.SetMovementEnabled(false);
		_teamSelectionUI.Show(_player.Team, enemyCard);
	}

	public void OnTeamMemberSelected(WishemonCard playerCard)
	{
		_playerCard = playerCard;
		_playerCurrentHP = playerCard.PV;
		_enemyCurrentHP = _enemyCard.PV;
		_state = BattleState.Fighting;

		_teamSelectionUI.Hide();
		_battleArena.SetupArena(playerCard, _enemyCard);
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
		_battleArena.PlayPlayerAttack();
		int dmg = Mathf.Max(1, _playerCard.Attack - _enemyCard.Defense);
		_enemyCurrentHP -= dmg;
		_battleArena.PlayEnemyHit();
		_battleUI.SetBattleLog($"You dealt {dmg} damage!");
		_battleUI.UpdateEnemyHP(_enemyCurrentHP, _enemyCard.PV);

		if (_enemyCurrentHP <= 0)
		{
			yield return new WaitForSeconds(1f);
			_battleArena.PlayEnemyDeath();
			EndBattle($"You defeated {_enemyCard.Name}!", won: true);
			yield break;
		}

		// Pause before enemy turn
		yield return new WaitForSeconds(1f);

		// Enemy attacks
		_battleArena.PlayEnemyAttack();
		int enemyDmg = Mathf.Max(1, _enemyCard.Attack - _playerCard.Defense);
		_playerCurrentHP -= enemyDmg;
		_battleArena.PlayPlayerHit();
		_battleUI.SetBattleLog($"{_enemyCard.Name} dealt {enemyDmg} damage!");
		_battleUI.UpdatePlayerHP(_playerCurrentHP, _playerCard.PV);

		if (_playerCurrentHP <= 0)
		{
			yield return new WaitForSeconds(1f);
			_battleArena.PlayPlayerDeath();
			EndBattle("You were defeated...", won: false);
			yield break;
		}

		_battleUI.SetButtonsInteractable(true);
	}

	public void OnRunPressed()
	{
		if (_state != BattleState.Fighting) return;
		EndBattle("Got away safely!", won: true);
	}

	private void EndBattle(string resultMessage, bool won)
	{
		_state = BattleState.Inactive;
		_player.SetMovementEnabled(true);
		if (_safePoint != null)
			_player.Teleport(_safePoint.position);
		foreach (TallGrass tg in FindObjectsByType<TallGrass>(FindObjectsSortMode.None))
			tg.ForceExit();
		_battleUI.ShowEndMessage(resultMessage);
		StartCoroutine(HideBattleAfterDelay());
	}

	private IEnumerator HideBattleAfterDelay()
	{
		yield return new WaitForSeconds(2f);
		_battleUI.Hide();
		_battleArena.ClearArena();
	}
}
