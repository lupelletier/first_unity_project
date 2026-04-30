using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
	[SerializeField] private CharacterController _controller = null;
	[SerializeField] private Animator _animator = null;
	[SerializeField] private InputActionReference _moveRef = null;
	[SerializeField] private InputActionReference _interactRef = null;
	[SerializeField] private float _speed = 3f;
	[SerializeField] private Transform _rayStartPoint = null;
	[SerializeField] private float _rayDistance = 1f;


	[SerializeField] private WishemonCard[] _team;
    [SerializeField] private Wishemon _wishemon = null;


	private bool _movementEnabled = true;

	public WishemonCard[] Team => _team;

	public void SetMovementEnabled(bool enabled)
	{
		_movementEnabled = enabled;
	}

	// Hide or show player visual components (animator and renderers) while keeping scripts active.
	public void SetVisible(bool visible)
	{
		if (_animator != null)
			_animator.enabled = visible;

		Renderer[] renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
		foreach (var r in renderers)
			r.enabled = visible;
	}

	public void Teleport(Vector3 position)
	{
		_controller.enabled = false;
		transform.position = position;
		_controller.enabled = true;
	}

	private void Update()
	{
		if (!_movementEnabled) return;
		UpdateMovement();
		UpdateInteraction();
	}

    
    private void Start()
    {
        if (_team != null && _team.Length > 0)
            _wishemon.SpawnWishemon(_team[0]);
    }

	private void UpdateMovement()
	{
		Vector2 move = _moveRef.action.ReadValue<Vector2>();
		bool isWalking = move.magnitude >= 0.1f;

		_animator.SetBool("Walking", isWalking);

		if (isWalking)
		{
			if (move.y >= 0.1f)
			{
				// haut
				transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			}
			else if (move.x >= 0.1f)
			{
				// droite
				transform.rotation = Quaternion.Euler(0f, 90f, 0f);
			}
			else if (move.y <= -0.1f)
			{
				// bas
				transform.rotation = Quaternion.Euler(0f, 180f, 0f);
			}
			else if (move.x <= -0.1f)
			{
				// gauche
				transform.rotation = Quaternion.Euler(0f, 270f, 0f);
			}

			_controller.SimpleMove(transform.forward * _speed);
		}
	}

	private void UpdateInteraction()
	{
		if (InteractionManager.Instance != null && InteractionManager.Instance.IsDialogOpen)
			return;

		if (_interactRef.action.WasPerformedThisFrame())
		{
			Ray ray = new Ray(_rayStartPoint.position, _rayStartPoint.forward);
			if (Physics.Raycast(ray, out RaycastHit hit, _rayDistance))
			{
				var pnj = hit.collider.GetComponentInParent<PNJ>();
				if (pnj != null)
				{
					if (InteractionManager.Instance != null)
						InteractionManager.Instance.ShowDialog(pnj, this);
				}
				else
				{
					Debug.Log("Interactable hit, but no PNJ found.");
				}
			}
		}
	}
}