using UnityEngine;

public class TallGrass : MonoBehaviour
{
	private float _timer = 0f;
	[SerializeField] private float _minAggroTime = 1f;
	[SerializeField] private float _maxAggroTime = 5f;
	private bool _isPlayerInside = false;

	private void Update()
	{
		if (_isPlayerInside)
		{	
			// Decrease the timer by the time elapsed since the last frame after entering the tall grass
			_timer -= Time.deltaTime;

			// If the timer reaches zero, the player is attacked 
			if (_timer == 0f)
			{
				Debug.Log("Aggro");
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
}