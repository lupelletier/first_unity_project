using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionManager : MonoBehaviour
{
	public static InteractionManager Instance;

	[Header("Dialog UI")]
	public GameObject dialogPanel;
	public TMP_Text dialogText;
	public Button acceptButton;
	public Button declineButton;
	public EncounterCameraController cameraController;

	private PNJ _activePNJ;
	private Player _activePlayer;

	private void Awake()
	{
		Instance = this;
		if (dialogPanel != null) dialogPanel.SetActive(false);
		if (acceptButton != null) acceptButton.onClick.AddListener(OnAcceptFight);
		if (declineButton != null) declineButton.onClick.AddListener(CloseDialog);
	}

	public void ShowDialog(PNJ pnj, Player player)
	{
		_activePNJ = pnj;
		_activePlayer = player;
		if (dialogPanel != null) dialogPanel.SetActive(true);
		if (dialogText != null)
		{
			string line = (pnj.dialogLines != null && pnj.dialogLines.Length > 0) ? pnj.dialogLines[0] : "I challenge you to a fight — win and you'll gain " + pnj.goldReward + " gold.";
			dialogText.text = pnj.npcName + ": " + line;
		}
		_activePlayer?.SetMovementEnabled(false);
		// Override dialogue camera if the PNJ has a custom one assigned.
		if (pnj.dialogueCamera != null)
			cameraController?.SetDialogueCamera(pnj.dialogueCamera);
		cameraController?.ShowDialogue(player != null ? player.transform : null, pnj.transform);
	}

	public void CloseDialog()
	{
		if (dialogPanel != null) dialogPanel.SetActive(false);
		_activePlayer?.SetMovementEnabled(true);
		cameraController?.SetModeExploration();
		_activePNJ = null;
		_activePlayer = null;
	}

	public void OnAcceptFight()
	{
		if (_activePlayer != null && _activePNJ != null)
		{
			if (dialogPanel != null) dialogPanel.SetActive(false);
			cameraController?.SetModeExploration();
			bool battleStarted = BattleManager.Instance != null && BattleManager.Instance.TriggerEncounter(_activePNJ);
			if (!battleStarted)
				_activePlayer.SetMovementEnabled(true);
		}
		_activePNJ = null;
		// The battle manager re-enables movement after the encounter ends.
	}

	public bool IsDialogOpen => dialogPanel != null && dialogPanel.activeSelf;
}
