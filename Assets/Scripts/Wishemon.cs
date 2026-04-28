using UnityEngine;

public class Wishemon : MonoBehaviour
{
    private WishemonCard _card = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SpawnWishemon(WishemonCard card)
    {
        _card = card;
        Instantiate(_card.Prefab, transform.position, Quaternion.identity);
        
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
