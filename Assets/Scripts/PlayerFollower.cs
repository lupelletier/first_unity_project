using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    [SerializeField] Player _player = null;
    [SerializeField] Vector3 _offset = Vector3.zero;

	private void Update()
	{
		transform.position = _player.transform.position + _offset;
	}
}