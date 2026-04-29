using UnityEngine;

public class TallGrass : MonoBehaviour
{
	private float _timer = 0f;
	[SerializeField] private float _minAggroTime = 1f;
	[SerializeField] private float _maxAggroTime = 5f;
	private bool _isPlayerInside = false;

	[SerializeField] private WishemonCard[] _possibleEnemies;
	[SerializeField] private BattleManager _battleManager;

	private void Update()
	{
		if (_isPlayerInside)
		{	
			// Decrease the timer by the time elapsed since the last frame after entering the tall grass
			_timer -= Time.deltaTime;

			// If the timer reaches zero, the player is attacked
			if (_timer <= 0f)
			{
				if (_possibleEnemies.Length > 0 && _battleManager != null && !_battleManager.IsBattleActive)
				{
					WishemonCard enemy = _possibleEnemies[Random.Range(0, _possibleEnemies.Length)];
					_battleManager.TriggerEncounter(enemy);
				}
				_timer = Random.Range(_minAggroTime, _maxAggroTime);
			}
		}

		if (_isPlayerInside == false && _timer > 0f)
		{
			_timer = 0f;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Player player = other.gameObject.GetComponent<Player>();
		if (player != null)
		{
			_isPlayerInside = true;
			// Set the timer to a random value between min and max aggro time
			// Simulates the chance of being attacked
			_timer = Random.Range(_minAggroTime, _maxAggroTime);
			Debug.Log("Player entered tall grass, timer set to " + _timer);

		}
	}

	private void OnTriggerExit(Collider other)
	{
		Player player = other.gameObject.GetComponent<Player>();
		if (player != null)
		{
			_isPlayerInside = false;
			_timer = 0f;
		}
	}

	public void ForceExit()
	{
		_isPlayerInside = false;
		_timer = 0f;
	}
}