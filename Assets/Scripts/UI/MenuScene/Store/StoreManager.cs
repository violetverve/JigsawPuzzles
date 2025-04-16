using Player;
using UI;
using UnityEngine;
using UnityEngine.Purchasing;

public class StoreManager : MonoBehaviour
{ 

    public void BuyCoins(Product product)
    {
        Debug.Log("Purchased: " + product.definition.id);
        PlayerData.Instance.AddCoins((int)product.definition.payout.quantity);
        Coins.CoinsChanged?.Invoke();
    }

}
