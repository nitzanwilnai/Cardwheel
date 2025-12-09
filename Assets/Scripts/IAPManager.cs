/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using UnityEngine.Purchasing;

namespace Cardwheel
{

    public class IAPManager : MonoBehaviour
    {
#if UNITY_IOS || UNITY_ANDROID

        // Cross-platform ID you’ll use in code (must match the one you create on the stores)
        public const string PremiumUpgradeId = "com.nitzan.games.cardwheel.remove_ads";

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
                // Game.Instance.ShowError("Premium product not available yet.");
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
            // string s = "";
            // if (products != null)
            // {
            //     foreach (var p in products)
            //     {
            //         Debug.Log($"[IAP] Product: id={p.definition.id}, " +
            //                   $"storeSpecificId={p.definition.storeSpecificId}, " +
            //                   $"type={p.definition.type}, " +
            //                   $"available={p.availableToPurchase}, " +
            //                   $"price={p.metadata.localizedPriceString}");
            //         s += "\n" + $"[IAP] Product: id={p.definition.id}, " +
            //         $"storeSpecificId={p.definition.storeSpecificId}, " +
            //         $"type={p.definition.type}, " +
            //         $"available={p.availableToPurchase}, " +
            //         $"price={p.metadata.localizedPriceString}";
            //     }
            // }

            string s = "";

            // Cache the product for later purchase calls
            _premiumProduct = _products.GetProductById(PremiumUpgradeId);
            if (_premiumProduct == null)
            {
                Debug.Log("Premium product not returned by store. Check IDs and store setup.");
                // Game.Instance.ShowError("Premium product not returned by store. Check IDs and store setup.");
                s += "\nPremium product not returned by store. Check IDs and store setup.";
            }
            else
            {
                Debug.Log($"Fetched product: {_premiumProduct.definition.id} | price={_premiumProduct.metadata.localizedPriceString}");
                // Game.Instance.ShowError($"Fetched product: {_premiumProduct.definition.id} | price={_premiumProduct.metadata.localizedPriceString}");
                // s += "\n" + $"Fetched product: {_premiumProduct.definition.id} | price={_premiumProduct.metadata.localizedPriceString}";
            }

            // Game.Instance.ShowError(s);
        }

        void OnProductsFetchFailed(ProductFetchFailed failure)
        {
            Debug.Log($"Products fetch failed: {failure.FailureReason} - {failure.FailedFetchProducts.ToString()}");
            // Game.Instance.ShowError($"Products fetch failed: {failure.FailureReason} - {failure.FailedFetchProducts.ToString()}");
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
            if (product == null)
            {
                Debug.Log("Pending order has no items.");
                // Game.Instance.ShowError("Pending order has no items.");
                return;
            }

            if (product.definition.id == PremiumUpgradeId)
            {
                _purchases.ConfirmPurchase(order); // confirm after granting/validating
            }
        }

        void OnPurchaseConfirmed(Order order)
        {
            var product = GetProductFromOrder(order);
            Debug.Log($"Purchase confirmed: {product?.definition.id}");
            Game.Instance.RemoveAdsPurchased();
        }

        void OnPurchaseFailed(FailedOrder failure)
        {
            // FailedOrder : Order  → still has CartOrdered
            var product = GetProductFromOrder(failure);
            var productId = product?.definition?.id ?? "(unknown)";
            Debug.LogWarning(
                $"Purchase failed: {productId} | reason={failure.FailureReason} | details={failure.Details}");
            Game.Instance.ShowError(
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
                    Game.Instance.RemoveAdsPurchased();
                }
            }

            // If you also want to auto-handle any pending orders fetched on startup,
            // v5 can surface them via OnPurchasePending by default (you can change that via ProcessPendingOrdersOnPurchasesFetched).
        }

        void OnPurchasesFetchFailed(PurchasesFetchFailureDescription failure)
        {
            Debug.LogWarning($"Purchases fetch failed: {failure.FailureReason} - {failure.Message}");
            // Game.Instance.ShowError($"Purchases fetch failed: {failure.FailureReason} - {failure.Message}");
        }

        void OnStoreDisconnected(StoreConnectionFailureDescription desc)
        {
            Debug.LogWarning($"Store disconnected: {desc.Message} | retryable={desc.IsRetryable}");
            // Game.Instance.ShowError($"Store disconnected: {desc.Message} | retryable={desc.IsRetryable}");
            // You can rely on the retry policy, or trigger a manual reconnect/UI here.
        }

#endif
    }
}