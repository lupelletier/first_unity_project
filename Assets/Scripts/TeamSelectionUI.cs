using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelectionUI : MonoBehaviour
{
	[SerializeField] private GameObject _panel;
	[SerializeField] private TMP_Text _messageText;
	[SerializeField] private Transform _buttonContainer;
	[SerializeField] private GameObject _teamButtonPrefab;

	private BattleManager _battleManager;

	private void Awake()
	{
		_battleManager = FindAnyObjectByType<BattleManager>();
	}

	public void Show(WishemonCard[] team, WishemonCard enemy)
	{
		_panel.SetActive(true);
		_messageText.text = $"A wild {enemy.Name} appeared!\nWhich Wishemon do you send?";

		foreach (Transform child in _buttonContainer)
			Destroy(child.gameObject);

		foreach (WishemonCard card in team)
		{
			GameObject btnGO = Instantiate(_teamButtonPrefab, _buttonContainer);
			btnGO.GetComponentInChildren<TMP_Text>().text = $"{card.Name}\nHP: {card.PV}";
			WishemonCard captured = card;
			btnGO.GetComponent<Button>().onClick.AddListener(() => _battleManager.OnTeamMemberSelected(captured));
		}
	}

	public void Hide()
	{
		_panel.SetActive(false);
	}
}
