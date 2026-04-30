using UnityEngine;

public class EncounterCameraController : MonoBehaviour
{
	public static EncounterCameraController Instance;

	[SerializeField] private Camera _explorationCamera;
	[SerializeField] private Camera _dialogueCamera;
	[SerializeField] private Camera _battleCamera;
	[SerializeField] private float _cameraBlendSpeed = 6f;
	[SerializeField] private bool _useFixedDialogueCamera = true; // If true, dialogue camera keeps its inspector position; if false, it auto-frames player and target.

	private Camera _defaultDialogueCamera; // Store the default to restore after per-PNJ overrides.
	[SerializeField] private Vector3 _dialogueOffset = new Vector3(0f, 1.8f, -3f);
	[SerializeField] private Vector3 _battleOffset = new Vector3(0f, 2.2f, -6f);

	private Transform _dialoguePlayer;
	private Transform _dialogueTarget;
	private Transform _battleTarget;
	private bool _isDialogueActive;
	private bool _isBattleActive;

	private void Awake()
	{
		Instance = this;
		_defaultDialogueCamera = _dialogueCamera;
		SetModeExploration();
	}

	private void LateUpdate()
	{
		if (_isDialogueActive && _dialogueCamera != null && _dialoguePlayer != null && _dialogueTarget != null && !_useFixedDialogueCamera)
		{
			Vector3 midpoint = (_dialoguePlayer.position + _dialogueTarget.position) * 0.5f;
			Vector3 desiredPosition = midpoint + _dialogueOffset;
			_dialogueCamera.transform.position = Vector3.Lerp(_dialogueCamera.transform.position, desiredPosition, Time.deltaTime * _cameraBlendSpeed);
			_dialogueCamera.transform.LookAt(midpoint + Vector3.up * 1.1f);
		}

		if (_isBattleActive && _battleCamera != null && _battleTarget != null)
		{
			Vector3 desiredPosition = _battleTarget.position + _battleOffset;
			_battleCamera.transform.position = Vector3.Lerp(_battleCamera.transform.position, desiredPosition, Time.deltaTime * _cameraBlendSpeed);
			_battleCamera.transform.LookAt(_battleTarget.position + Vector3.up * 1f);
		}
	}

	public void ShowDialogue(Transform player, Transform target)
	{
		_dialoguePlayer = player;
		_dialogueTarget = target;
		_isDialogueActive = true;
		_isBattleActive = false;
		SetOnlyCameraActive(_dialogueCamera);
	}

	public void ShowBattle(Transform target)
	{
		_battleTarget = target;
		_isBattleActive = true;
		_isDialogueActive = false;
		SetOnlyCameraActive(_battleCamera);
	}

	public void SetModeExploration()
	{
		_isDialogueActive = false;
		_isBattleActive = false;
		_dialoguePlayer = null;
		_dialogueTarget = null;
		_battleTarget = null;
		_dialogueCamera = _defaultDialogueCamera; // Restore default dialogue camera
		SetOnlyCameraActive(_explorationCamera);
	}

	// Temporarily override the dialogue camera for a specific interaction (e.g., per-PNJ camera).
	public void SetDialogueCamera(Camera cam)
	{
		if (cam != null)
			_dialogueCamera = cam;
	}

	private void SetOnlyCameraActive(Camera activeCamera)
	{
		if (_explorationCamera != null) _explorationCamera.gameObject.SetActive(activeCamera == _explorationCamera);
		if (_dialogueCamera != null) _dialogueCamera.gameObject.SetActive(activeCamera == _dialogueCamera);
		if (_battleCamera != null) _battleCamera.gameObject.SetActive(activeCamera == _battleCamera);
	}
}
