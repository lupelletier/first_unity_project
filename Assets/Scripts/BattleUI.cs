using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
	[SerializeField] private GameObject _battlePanel;

	[SerializeField] private TMP_Text _enemyNameText;
	[SerializeField] private Slider _enemyHPSlider;
	[SerializeField] private TMP_Text _enemyHPText;

	[SerializeField] private TMP_Text _playerNameText;
	[SerializeField] private Slider _playerHPSlider;
	[SerializeField] private TMP_Text _playerHPText;

	[SerializeField] private TMP_Text _battleLogText;

	[SerializeField] private Button _attackButton;
	[SerializeField] private Button _runButton;

	public void Show(WishemonCard playerCard, WishemonCard enemyCard, int playerHP, int enemyHP)
	{
		_battlePanel.SetActive(true);

		_enemyNameText.text = enemyCard.Name;
		_enemyHPSlider.maxValue = enemyCard.PV;
		_enemyHPSlider.value = enemyHP;
		_enemyHPText.text = $"{enemyHP} / {enemyCard.PV}";

		_playerNameText.text = playerCard.Name;
		_playerHPSlider.maxValue = playerCard.PV;
		_playerHPSlider.value = playerHP;
		_playerHPText.text = $"{playerHP} / {playerCard.PV}";

		_battleLogText.text = $"A wild {enemyCard.Name} appeared!";

		_attackButton.interactable = true;
		_runButton.interactable = true;
	}

	public void Hide()
	{
		_battlePanel.SetActive(false);
	}

	public void UpdatePlayerHP(int current, int max)
	{
		_playerHPSlider.value = current;
		_playerHPText.text = $"{current} / {max}";
	}

	public void UpdateEnemyHP(int current, int max)
	{
		_enemyHPSlider.value = current;
		_enemyHPText.text = $"{current} / {max}";
	}

	public void SetBattleLog(string msg)
	{
		_battleLogText.text = msg;
	}

	public void AppendBattleLog(string msg)
	{
		_battleLogText.text += msg;
	}

	public void ShowEndMessage(string msg)
	{
		_battleLogText.text = msg;
		_attackButton.interactable = false;
		_runButton.interactable = false;
	}
}
