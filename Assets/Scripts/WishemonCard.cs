using UnityEngine;

[CreateAssetMenu(fileName = "WishemonCards", menuName = "Wishemons")]

public class WishemonCard : ScriptableObject
{
    [SerializeField] private string _name = string.Empty;
    [SerializeField] private GameObject _prefab = null;
    [SerializeField] private int _pv = 0;
    [SerializeField] private int _attack = 0;
    [SerializeField] private int _defense = 0;

    public string Name { get {return _name; }}
    public GameObject Prefab { get {return _prefab; }}
    public int PV { get {return _pv; }}
    public int Attack { get {return _attack; }}
    public int Defense { get {return _defense; }}

    void InitializeCard()
    {
        Debug.Log("Card initialized with name: " + _name + ", prefab: " + _prefab.name + ", PV: " + _pv + ", Attack: " + _attack + ", Defense: " + _defense);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
