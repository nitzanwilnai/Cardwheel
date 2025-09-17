using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour
{
    // Cross-platform ID you’ll use in code (must match the one you create on the stores)
    public const string PremiumUpgradeId = "unlock_premium";

    IStoreService _store;           // Connects to the store
    IProductService _products;      // Fetches product data
    IPurchaseService _purchases;    // Orders/Restores/Confirms purchases

    Product _premiumProduct;        // Resolved Product reference after fetch
    PendingOrder _pendingOrder;     // Holds a pending order until confirmed

    async void Start()
    {
        Debug.Log("IAPManager Start()");

        await UnityServices.InitializeAsync();

        _store = UnityIAPServices.DefaultStore();
        _products = UnityIAPServices.DefaultProduct();
        _purchases = UnityIAPServices.DefaultPurchase();

        _store.OnStoreDisconnected += OnStoreDisconnected;

        _products.OnProductsFetched += OnProductsFetched;
        _products.OnProductsFetchFailed += OnProductsFetchFailed;
        _purchases.OnPurchasePending += OnPurchasePending;
        _purchases.OnPurchaseConfirmed += OnPurchaseConfirmed;
        _purchases.OnPurchaseFailed += OnPurchaseFailed;
        _purchases.OnPurchasesFetched += OnPurchasesFetched;
        _purchases.OnPurchasesFetchFailed += OnPurchasesFetchFailed;

        await _store.Connect(); // <- await

        _products.FetchProducts(new List<ProductDefinition> {
            new ProductDefinition(PremiumUpgradeId, ProductType.NonConsumable)
        });

        _purchases.FetchPurchases();
    }

    // ===== Public UI Hooks =====

    public void BuyPremium()
    {
        if (_premiumProduct == null || !_premiumProduct.availableToPurchase)
        {
            Debug.LogWarning("Premium product not available yet.");
            return;
        }
        _purchases.PurchaseProduct(_premiumProduct);
    }

    public void RestoreOnIOS()
    {
        // iOS expects an explicit Restore action; no-op on some stores.
        _purchases.RestoreTransactions((ok, err) =>
        {
            Debug.Log($"Restore finished. success={ok} err={err}");
        });
    }

    // ===== Product callbacks =====

    void OnProductsFetched(List<Product> products)
    {
        // Cache the product for later purchase calls
        _premiumProduct = _products.GetProductById(PremiumUpgradeId);
        if (_premiumProduct == null)
            Debug.LogError("Premium product not returned by store. Check IDs and store setup.");
        else
            Debug.Log($"Fetched product: {_premiumProduct.definition.id} | price={_premiumProduct.metadata.localizedPriceString}");
    }

    void OnProductsFetchFailed(ProductFetchFailed failure)
    {
        Debug.LogError($"Products fetch failed: {failure.FailureReason} - {failure.FailedFetchProducts.ToString()}");
    }

    static Product GetProductFromOrder(Order order)
    {
        var items = order.CartOrdered.Items();
        return items != null && items.Count > 0 ? items[0].Product : null;
    }

    // ===== Purchase callbacks =====

    void OnPurchasePending(PendingOrder order)
    {
        var product = GetProductFromOrder(order);
        if (product == null) { Debug.LogError("Pending order has no items."); return; }

        if (product.definition.id == PremiumUpgradeId)
        {
            GrantPremiumLocally();
            _purchases.ConfirmPurchase(order); // confirm after granting/validating
        }
    }

    void OnPurchaseConfirmed(Order order)
    {
        var product = GetProductFromOrder(order);
        Debug.Log($"Purchase confirmed: {product?.definition.id}");
    }

    void OnPurchaseFailed(FailedOrder failure)
    {
        // FailedOrder : Order  → still has CartOrdered
        var product = GetProductFromOrder(failure);
        var productId = product?.definition?.id ?? "(unknown)";
        Debug.LogWarning(
            $"Purchase failed: {productId} | reason={failure.FailureReason} | details={failure.Details}");
    }

    // ===== Restore / Existing purchases =====


    void OnPurchasesFetched(Orders orders)
    {
        // Non-consumables you already own will appear as confirmed orders
        foreach (var confirmed in orders.ConfirmedOrders)
        {
            var product = GetProductFromOrder(confirmed);
            if (product != null && product.definition.id == PremiumUpgradeId)
            {
                GrantPremiumLocally();
                Debug.Log("Premium entitlement restored.");
            }
        }

        // If you also want to auto-handle any pending orders fetched on startup,
        // v5 can surface them via OnPurchasePending by default (you can change that via ProcessPendingOrdersOnPurchasesFetched).
    }

    void OnPurchasesFetchFailed(PurchasesFetchFailureDescription failure)
    {
        Debug.LogWarning($"Purchases fetch failed: {failure.FailureReason} - {failure.Message}");
    }

    void OnStoreDisconnected(StoreConnectionFailureDescription desc)
    {
        Debug.LogWarning($"Store disconnected: {desc.Message} | retryable={desc.IsRetryable}");
        // You can rely on the retry policy, or trigger a manual reconnect/UI here.
    }

    // ===== Entitlement grant =====

    void GrantPremiumLocally()
    {
        // TODO: persist this flag in your own saved data (and consider server entitlements).
        PlayerPrefs.SetInt("premium_unlocked", 1);
        PlayerPrefs.Save();
        // Update UI/feature flags here
    }
}
