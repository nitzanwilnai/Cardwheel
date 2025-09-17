using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class MyIAPManager
{
    StoreController m_StoreController;

    async void InitializeIAP()
    {
        m_StoreController = UnityIAPServices.StoreController();

        // m_StoreController.OnPurchasePending += OnPurchasePending;

        await m_StoreController.Connect();

        m_StoreController.OnProductsFetched += OnProductsFetched;
        m_StoreController.OnPurchasesFetched += OnPurchasesFetched;

        var initialProductsToFetch = new List<ProductDefinition>
        {
            // new(goldProductId, ProductType.Consumable),
            // new(diamondProductId, ProductType.Consumable)
        };

        m_StoreController.FetchProducts(initialProductsToFetch);
    }

    // void OnPurchasePending(Purchase purchase)
    // {

    // }

    void OnProductsFetched(List<Product> products)
    {
        // Handle fetched products  
        m_StoreController.FetchPurchases();
    }
    void OnPurchasesFetched(Orders orders)
    {
        // Process purchases, e.g. check for entitlements from completed orders  
    }
}