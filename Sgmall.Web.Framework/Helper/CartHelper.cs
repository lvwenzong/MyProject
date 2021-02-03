using Sgmall.Application;
using Sgmall.CommonModel;
using Sgmall.Core;
using Sgmall.Core.Helper;
using Sgmall.Model;
using System.Collections.Generic;
using System.Linq;

namespace Sgmall.Web.Framework
{
    public class CartHelper
    {
        /*
         *购物车存储说明：
         *游客访问时，点击加入购物车，购物车信息保存至Cookie中，游客点击结算时，Cookie中的购物车信息转移至数据库中并清空Cookie中购物车信息。
         *登录会员点击加入购物车时，购物车信息保存至数据库中。
         *Cookie存储格式： skuId1:count1,skuId2:count2,.....
         */

        /// <summary>
        /// 同步客户端购物车信息至服务器
        /// </summary>
        public void UpdateCartInfoFromCookieToServer(long memberId,long agencyShopId)
        {
            string cartInfo = WebHelper.GetCookie(CookieKeysCollection.Sgmall_CART);
            if (!string.IsNullOrWhiteSpace(cartInfo))
            {
                string[] cartItems = cartInfo.Split(',');
                var shoppingCartItems = new ShoppingCartItem[cartItems.Length];
                int i = 0;
                foreach (string cartItem in cartItems)
                {
                    var cartItemParts = cartItem.Split(':');
                    shoppingCartItems[i++] = new ShoppingCartItem() { SkuId = cartItemParts[0], Quantity = int.Parse(cartItemParts[1]) };
                }
                CartApplication.AddToCart(shoppingCartItems, memberId, agencyShopId);
            }
            WebHelper.DeleteCookie(CookieKeysCollection.Sgmall_CART);
        }

        /// <summary>
        /// 获取购物车中的商品
        /// </summary>
        /// <returns></returns>
        public IEnumerable<long> GetCartProductIds(long memberId)
        {
            long[] productIds = new long[] { };
            if (memberId > 0)//已经登录，系统从服务器读取购物车信息，否则从Cookie获取购物车信息
            {
                var cartInfo = CartApplication.GetCart(memberId);
                productIds = cartInfo.Items.Select(item => item.ProductId).ToArray();
            }
            else
            {
                string cartInfo = WebHelper.GetCookie(CookieKeysCollection.Sgmall_CART);
                if (!string.IsNullOrWhiteSpace(cartInfo))
                {
                    string[] cartItems = cartInfo.Split(',');
                    productIds = new long[cartItems.Length];
                    int i = 0;
                    foreach (string cartItem in cartItems)
                    {
                        var cartItemParts = cartItem.Split(':');
                        productIds[i++] = long.Parse(cartItemParts[0]);//获取商品Id
                    }
                }
            }
            return productIds;
        }


        /// <summary>
        /// 获取购物车中的商品
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetCartProductSKUIds(long memberId)
        {
            string[] productIds = new string[] { };
            if (memberId > 0)//已经登录，系统从服务器读取购物车信息，否则从Cookie获取购物车信息
            {
                var cartInfo = CartApplication.GetCart(memberId);
                productIds = cartInfo.Items.Select(item => item.SkuId).ToArray();
            }
            else
            {
                string cartInfo = WebHelper.GetCookie(CookieKeysCollection.Sgmall_CART);
                if (!string.IsNullOrWhiteSpace(cartInfo))
                {
                    string[] cartItems = cartInfo.Split(',');
                    productIds = new string[cartItems.Length];
                    int i = 0;
                    foreach (string cartItem in cartItems)
                    {
                        var cartItemParts = cartItem.Split(':');
                        productIds[i++] = cartItemParts[0];//获取商品SKUId
                    }
                }
            }
            return productIds;
        }

        /// <summary>
        /// 获取购物车中的商品
        /// </summary>
        /// <returns></returns>
        public ShoppingCartInfo GetCart(long memberId)
        {
            ShoppingCartInfo shoppingCartInfo;
            if (memberId > 0)//已经登录，系统从服务器读取购物车信息，否则从Cookie获取购物车信息
                shoppingCartInfo = CartApplication.GetCart(memberId);
            else
            {
                shoppingCartInfo = new ShoppingCartInfo();

                string cartInfo = WebHelper.GetCookie(CookieKeysCollection.Sgmall_CART);
                if (!string.IsNullOrWhiteSpace(cartInfo))
                {
                    string[] cartItems = cartInfo.Split(',');
                    var cartInfoItems = new ShoppingCartItem[cartItems.Length];
                    int i = 0;
                    foreach (string cartItem in cartItems)
                    {
                        var cartItemParts = cartItem.Split(':');
                        cartInfoItems[i++] = new ShoppingCartItem() { ProductId = long.Parse(cartItemParts[0].Split('_')[0]), SkuId = cartItemParts[0], Quantity = int.Parse(cartItemParts[1]) };
                    }
                    shoppingCartInfo.Items = cartInfoItems;
                }
            }
            return shoppingCartInfo;
        }


        public void RemoveFromCart(string skuId, long memberId)
        {
            if (memberId > 0)
                CartApplication.DeleteCartItem(skuId, memberId);
            else
            {
                string cartInfo = WebHelper.GetCookie(CookieKeysCollection.Sgmall_CART);
                if (!string.IsNullOrWhiteSpace(cartInfo))
                {
                    string[] cartItems = cartInfo.Split(',');
                    string newCartInfo = string.Empty;
                    foreach (string cartItem in cartItems)
                    {
                        string[] cartItemParts = cartItem.Split(':');
                        string cartItemSkuId = cartItemParts[0];
                        if (cartItemSkuId != skuId)
                            newCartInfo += "," + cartItem;
                    }
                    if (!string.IsNullOrWhiteSpace(newCartInfo))
                        newCartInfo = newCartInfo.Substring(1);
                    WebHelper.SetCookie(CookieKeysCollection.Sgmall_CART, newCartInfo);
                }
            }
        }

        public void RemoveFromCart(IEnumerable<string> skuIds, long memberId)
        {
            if (memberId > 0)
                CartApplication.DeleteCartItem(skuIds, memberId);
            else
            {
                string cartInfo = WebHelper.GetCookie(CookieKeysCollection.Sgmall_CART);
                if (!string.IsNullOrWhiteSpace(cartInfo))
                {
                    string[] cartItems = cartInfo.Split(',');
                    string newCartInfo = string.Empty;
                    foreach (string cartItem in cartItems)
                    {
                        string[] cartItemParts = cartItem.Split(':');
                        string cartItemSkuId = cartItemParts[0];
                        if (!skuIds.Contains(cartItemSkuId))
                            newCartInfo += "," + cartItem;
                    }
                    if (!string.IsNullOrWhiteSpace(newCartInfo))
                        newCartInfo = newCartInfo.Substring(1);
                    WebHelper.SetCookie(CookieKeysCollection.Sgmall_CART, newCartInfo);
                }
            }
        }


        /// <summary>
        /// 更新购物车
        /// </summary>
        /// <param name="skuId"></param>
        /// <param name="count"></param>
        /// <param name="memberId"></param>
        public void UpdateCartItem(string skuId, int count, long memberId,long agencyShopId)
        {
            if (memberId > 0)
                CartApplication.UpdateCart(skuId, count, memberId, agencyShopId);
            else
            {
                string cartInfo = WebHelper.GetCookie(CookieKeysCollection.Sgmall_CART);
                if (!string.IsNullOrWhiteSpace(cartInfo))
                {
                    string[] cartItems = cartInfo.Split(',');
                    string newCartInfo = string.Empty;
                    foreach (string cartItem in cartItems)
                    {
                        var cartItemParts = cartItem.Split(':');
                        if (cartItemParts[0] == skuId)
                            newCartInfo += "," + skuId + ":" + count;
                        else
                            newCartInfo += "," + cartItem;
                    }

                    if (!string.IsNullOrWhiteSpace(newCartInfo))
                        newCartInfo = newCartInfo.Substring(1);
                    WebHelper.SetCookie(CookieKeysCollection.Sgmall_CART, newCartInfo);
                }
                else
                {
                    WebHelper.SetCookie(CookieKeysCollection.Sgmall_CART, string.Format("{0}:{1}", skuId, count));
                }
            }
        }


        public void AddToCart(string skuId, int count, long memberId, long agencyShopId)
        {
            CheckSkuIdIsValid(skuId);
            //判断库存
            var sku = ProductManagerApplication.GetSKU(skuId);
            if (sku == null)
            {
                throw new SgmallException("Error SKU");
            }
            if (count > sku.Stock)
            {
                throw new SgmallException("Sản phẩm không đủ");
            }
            #region 商品限购判断
            var prouctInfo = ProductManagerApplication.GetProduct(sku.ProductId);
            if (prouctInfo != null && prouctInfo.MaxBuyCount > 0 && !prouctInfo.IsOpenLadder)//商品有限购数量
            {
                var carInfo = CartApplication.GetCart(memberId);
                if (carInfo != null)
                {
                    var quantity = carInfo.Items.Where(p => p.ProductId == sku.ProductId).Sum(d => d.Quantity);//当前用户该商品已加入购物车数量
                    if (count + quantity > prouctInfo.MaxBuyCount)//已有数量+新数量
                        throw new SgmallException(string.Format("每个ID限购{0}件", prouctInfo.MaxBuyCount));
                }
            }
            #endregion

            if (memberId > 0)
            //CartApplication.AddToCart(skuId, count, memberId,agencyShopId);
            {
                //判断库存
                var carInfo = CartApplication.GetCart(memberId);
                if (carInfo != null)
                {
                    var quantity = carInfo.Items.Where(p => p.ProductId == sku.ProductId).Sum(d => d.Quantity);//当前用户该商品已加入购物车数量
                    if (count + quantity > sku.Stock)//已有数量+新数量
                        throw new SgmallException("Sản phẩm không đủ");
                }
                CartApplication.AddToCart(skuId, count, memberId, agencyShopId);
            }
            else
            {
                string cartInfo = WebHelper.GetCookie(CookieKeysCollection.Sgmall_CART);
                if (!string.IsNullOrWhiteSpace(cartInfo))
                {
                    string[] cartItems = cartInfo.Split(',');
                    string newCartInfo = string.Empty;
                    bool exist = false;
                    foreach (string cartItem in cartItems)
                    {
                        var cartItemParts = cartItem.Split(':');
                        if (cartItemParts[0] == skuId)
                        {
                            newCartInfo += "," + skuId + ":" + (int.Parse(cartItemParts[1]) + count);
                            exist = true;
                        }
                        else
                            newCartInfo += "," + cartItem;
                    }
                    if (!exist)
                        newCartInfo += "," + skuId + ":" + count;

                    if (!string.IsNullOrWhiteSpace(newCartInfo))
                        newCartInfo = newCartInfo.Substring(1);
                    WebHelper.SetCookie(CookieKeysCollection.Sgmall_CART, newCartInfo);
                }
                else
                {
                    WebHelper.SetCookie(CookieKeysCollection.Sgmall_CART, string.Format("{0}:{1}", skuId, count));
                }
            }
        }

        public bool CheckSkuId(string skuId)
        {
            bool status = true;
            long productId = 0;
            long.TryParse(skuId.Split('_')[0], out productId);
            if (productId == 0)
                status = false;
            var skuItem = ProductManagerApplication.GetSKU(skuId);
            if (skuItem == null)
                status = false;
            var sku = ProductManagerApplication.GetSKU(skuId);
            if (sku == null)
            {
                status = false;
            }
            return status;
        }

        void CheckSkuIdIsValid(string skuId)
        {
            long productId = 0;
            long.TryParse(skuId.Split('_')[0], out productId);
            if (productId == 0)
                throw new Sgmall.Core.InvalidPropertyException("SKUId无效");

            var skuItem = ProductManagerApplication.GetSKU(skuId);
            if (skuItem == null)
                throw new Sgmall.Core.InvalidPropertyException("SKUId无效");

        }

    }


    public class BranchCartHelper
    {
        /*
         *购物车存储说明：
         *游客访问时，点击加入购物车，购物车信息保存至Cookie中，游客点击结算时，Cookie中的购物车信息转移至数据库中并清空Cookie中购物车信息。
         *登录会员点击加入购物车时，购物车信息保存至数据库中。
         *Cookie存储格式： skuId1:count1,skuId2:count2,.....
         */

        /// <summary>
        /// 同步客户端购物车信息至服务器
        /// </summary>
        public void UpdateCartInfoFromCookieToServer(long memberId,long agencyShopId)
        {
            string cartInfo = WebHelper.GetCookie(CookieKeysCollection.Sgmall_CART_BRANCH);
            if (!string.IsNullOrWhiteSpace(cartInfo))
            {
                string[] cartItems = cartInfo.Split(',');
                var shoppingCartItems = new ShoppingCartItem[cartItems.Length];
                int i = 0;
                foreach (string cartItem in cartItems)
                {
                    var cartItemParts = cartItem.Split(':');
                    var skuId = cartItemParts[0];
                    int quantity = int.Parse(cartItemParts[1]);
                    long shopBranchId = long.Parse(cartItemParts[2]);
                    shoppingCartItems[i++] = new ShoppingCartItem() { ShopBranchId = shopBranchId, SkuId = cartItemParts[0], Quantity = int.Parse(cartItemParts[1]) };
                }
                CartApplication.AddToShopBranchCart(shoppingCartItems, memberId,agencyShopId);
            }
            WebHelper.DeleteCookie(CookieKeysCollection.Sgmall_CART_BRANCH);
        }

        /// <summary>
        /// 获取购物车中的商品
        /// </summary>
        /// <returns></returns>
        public IEnumerable<long> GetCartProductIds(long memberId, long shopBranchId)
        {
            long[] productIds = new long[] { };
            if (memberId > 0)//已经登录，系统从服务器读取购物车信息，否则从Cookie获取购物车信息
            {
                var cartInfo = CartApplication.GetShopBranchCart(memberId, shopBranchId);
                productIds = cartInfo.Items.Select(item => item.ProductId).ToArray();
            }
            else
            {
                string cartInfo = WebHelper.GetCookie(CookieKeysCollection.Sgmall_CART_BRANCH);
                if (!string.IsNullOrWhiteSpace(cartInfo))
                {
                    string[] cartItems = cartInfo.Split(',');
                    productIds = new long[cartItems.Length];
                    int i = 0;
                    foreach (string cartItem in cartItems)
                    {
                        var cartItemParts = cartItem.Split(':');
                        productIds[i++] = long.Parse(cartItemParts[0]);//获取商品Id
                    }
                }
            }
            return productIds;
        }


        /// <summary>
        /// 获取购物车中的商品
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetCartProductSKUIds(long memberId, long shopBranchId)
        {
            string[] productIds = new string[] { };
            if (memberId > 0)//已经登录，系统从服务器读取购物车信息，否则从Cookie获取购物车信息
            {
                var cartInfo = CartApplication.GetShopBranchCart(memberId, shopBranchId);
                productIds = cartInfo.Items.Select(item => item.SkuId).ToArray();
            }
            else
            {
                string cartInfo = WebHelper.GetCookie(CookieKeysCollection.Sgmall_CART_BRANCH);
                if (!string.IsNullOrWhiteSpace(cartInfo))
                {
                    string[] cartItems = cartInfo.Split(',');
                    productIds = new string[cartItems.Length];
                    int i = 0;
                    foreach (string cartItem in cartItems)
                    {
                        var cartItemParts = cartItem.Split(':');
                        productIds[i++] = cartItemParts[0];//获取商品SKUId
                    }
                }
            }
            return productIds;
        }

        /// <summary>
        /// 获取购物车中的商品
        /// </summary>
        /// <returns></returns>
        public ShoppingCartInfo GetCart(long memberId, long shopBranchId)
        {
            ShoppingCartInfo shoppingCartInfo;
            if (memberId > 0)//已经登录，系统从服务器读取购物车信息，否则从Cookie获取购物车信息
                shoppingCartInfo = CartApplication.GetShopBranchCart(memberId, shopBranchId);
            else
            {
                shoppingCartInfo = new ShoppingCartInfo();

                string cartInfo = WebHelper.GetCookie(CookieKeysCollection.Sgmall_CART_BRANCH);
                if (!string.IsNullOrWhiteSpace(cartInfo))
                {
                    string[] cartItems = cartInfo.Split(',');
                    var cartInfoItems = new List<ShoppingCartItem>();
                    int i = 0;
                    foreach (string cartItem in cartItems)
                    {
                        var cartItemParts = cartItem.Split(':');
                        if (shopBranchId == 0 || cartItemParts[2] == shopBranchId.ToString())
                        {
                            cartInfoItems.Add(new ShoppingCartItem() { ProductId = long.Parse(cartItemParts[0].Split('_')[0]), SkuId = cartItemParts[0], Quantity = int.Parse(cartItemParts[1]), ShopBranchId = long.Parse(cartItemParts[2]) });
                        }
                    }
                    shoppingCartInfo.Items = cartInfoItems;
                }
            }
            return shoppingCartInfo;
        }

        public ShoppingCartInfo GetCartNoCache(long memberId, long shopBranchId)
        {
            ShoppingCartInfo shoppingCartInfo;
            if (memberId > 0)//已经登录，系统从服务器读取购物车信息，否则从Cookie获取购物车信息
                shoppingCartInfo = CartApplication.GetShopBranchCartNoCache(memberId, shopBranchId);
            else
            {
                shoppingCartInfo = new ShoppingCartInfo();

                string cartInfo = WebHelper.GetCookie(CookieKeysCollection.Sgmall_CART_BRANCH);
                if (!string.IsNullOrWhiteSpace(cartInfo))
                {
                    string[] cartItems = cartInfo.Split(',');
                    var cartInfoItems = new ShoppingCartItem[cartItems.Length];
                    int i = 0;
                    foreach (string cartItem in cartItems)
                    {
                        var cartItemParts = cartItem.Split(':');
                        if (shopBranchId == 0 || cartItemParts[2] == shopBranchId.ToString())
                            cartInfoItems[i++] = new ShoppingCartItem() { ProductId = long.Parse(cartItemParts[0].Split('_')[0]), SkuId = cartItemParts[0], Quantity = int.Parse(cartItemParts[1]), ShopBranchId = long.Parse(cartItemParts[2]) };
                    }
                    shoppingCartInfo.Items = cartInfoItems;
                }
            }
            return shoppingCartInfo;
        }

        public void RemoveFromCart(string skuId, long memberId, long shopBranchId)
        {
            if (memberId > 0)
                CartApplication.DeleteShopBranchCartItem(skuId, memberId, shopBranchId);
            else
            {
                string cartInfo = WebHelper.GetCookie(CookieKeysCollection.Sgmall_CART_BRANCH);
                if (!string.IsNullOrWhiteSpace(cartInfo))
                {
                    string[] cartItems = cartInfo.Split(',');
                    string newCartInfo = string.Empty;
                    foreach (string cartItem in cartItems)
                    {
                        string[] cartItemParts = cartItem.Split(':');
                        string cartItemSkuId = cartItemParts[0];
                        string cartItemshopBranchId = cartItemParts[2];
                        if (cartItemSkuId != skuId && shopBranchId.ToString() != cartItemshopBranchId)
                            newCartInfo += "," + cartItem;
                    }
                    if (!string.IsNullOrWhiteSpace(newCartInfo))
                        newCartInfo = newCartInfo.Substring(1);
                    WebHelper.SetCookie(CookieKeysCollection.Sgmall_CART_BRANCH, newCartInfo);
                }
            }
        }

        public void RemoveFromCart(IEnumerable<string> skuIds, long memberId, long shopBranchId)
        {
            if (memberId > 0)
                CartApplication.DeleteShopBranchCartItem(skuIds, memberId, shopBranchId);
            else
            {
                string cartInfo = WebHelper.GetCookie(CookieKeysCollection.Sgmall_CART_BRANCH);
                if (!string.IsNullOrWhiteSpace(cartInfo))
                {
                    string[] cartItems = cartInfo.Split(',');
                    string newCartInfo = string.Empty;
                    foreach (string cartItem in cartItems)
                    {
                        string[] cartItemParts = cartItem.Split(':');
                        string cartItemSkuId = cartItemParts[0];
                        string cartItemshopBranchId = cartItemParts[2];
                        if (!skuIds.Contains(cartItemSkuId) && shopBranchId.ToString() != cartItemshopBranchId)
                            newCartInfo += "," + cartItem;
                    }
                    if (!string.IsNullOrWhiteSpace(newCartInfo))
                        newCartInfo = newCartInfo.Substring(1);
                    WebHelper.SetCookie(CookieKeysCollection.Sgmall_CART_BRANCH, newCartInfo);
                }
            }
        }



        public void UpdateCartItem(string skuId, int count, long memberId, long shopBranchId, long agencyShopId)
        {
            CheckSkuIdIsValid(skuId, shopBranchId);
            //判断库存
            var sku = ProductManagerApplication.GetSKU(skuId);
            if (sku == null)
            {
                throw new SgmallException("错误的SKU");
            }
            //if (count > sku.Stock)
            //{
            //    throw new SgmallException("库存不足");
            //}
            var shopBranch = ShopBranchApplication.GetShopBranchById(shopBranchId);
            if (shopBranch == null)
            {
                throw new SgmallException("错误的门店id");
            }
            var shopBranchSkuList = ShopBranchApplication.GetSkusByIds(shopBranchId, new List<string> { skuId });
            if (shopBranchSkuList == null || shopBranchSkuList.Count == 0 || shopBranchSkuList[0].Status == ShopBranchSkuStatus.InStock)
            {
                throw new SgmallException("门店没有该商品或已下架");
            }
            var sbsku = shopBranchSkuList.FirstOrDefault();
            if (sbsku.Stock < count)
            {
                throw new SgmallException("门店库存不足");
            }

            if (memberId > 0)
                CartApplication.UpdateShopBranchCart(skuId, count, memberId, shopBranchId,agencyShopId);
            else
            {
                string cartInfo = WebHelper.GetCookie(CookieKeysCollection.Sgmall_CART_BRANCH);
                if (!string.IsNullOrWhiteSpace(cartInfo))
                {
                    string[] cartItems = cartInfo.Split(',');
                    string newCartInfo = string.Empty;
                    bool exist = false;
                    foreach (string cartItem in cartItems)
                    {
                        var cartItemParts = cartItem.Split(':');
                        if (cartItemParts[0] == skuId && cartItemParts[2] == shopBranchId.ToString())
                        {
                            newCartInfo += "," + skuId + ":" + count + ":" + shopBranchId;
                            exist = true;
                        }
                        else
                            newCartInfo += "," + cartItem;
                    }
                    if (!exist)
                        newCartInfo += "," + skuId + ":" + count + ":" + shopBranchId;

                    if (!string.IsNullOrWhiteSpace(newCartInfo))
                        newCartInfo = newCartInfo.Substring(1);
                    WebHelper.SetCookie(CookieKeysCollection.Sgmall_CART_BRANCH, newCartInfo);
                }
                else
                {
                    WebHelper.SetCookie(CookieKeysCollection.Sgmall_CART_BRANCH, string.Format("{0}:{1}:{2}", skuId, count, shopBranchId));
                }
            }
        }


        public void AddToCart(string skuId, int count, long memberId, long shopBranchId, long agencyShopId)
        {
            CheckSkuIdIsValid(skuId, shopBranchId);
            //判断库存
            var sku = ProductManagerApplication.GetSKU(skuId);
            if (sku == null)
            {
                throw new SgmallException("错误的SKU");
            }
            if (count > sku.Stock)
            {
                throw new SgmallException("库存不足");
            }
            var shopBranch = ShopBranchApplication.GetShopBranchById(shopBranchId);
            if (shopBranch == null)
            {
                throw new SgmallException("错误的门店id");
            }
            var shopBranchSkuList = ShopBranchApplication.GetSkusByIds(shopBranchId, new List<string> { skuId });
            if (shopBranchSkuList == null || shopBranchSkuList.Count == 0 || shopBranchSkuList[0].Status == ShopBranchSkuStatus.InStock)
            {
                throw new SgmallException("门店没有该商品或已下架");
            }

            if (memberId > 0)
                CartApplication.AddToShopBranchCart(skuId, count, memberId, shopBranchId,agencyShopId);
            else
            {
                string cartInfo = WebHelper.GetCookie(CookieKeysCollection.Sgmall_CART_BRANCH);
                if (!string.IsNullOrWhiteSpace(cartInfo))
                {
                    string[] cartItems = cartInfo.Split(',');
                    string newCartInfo = string.Empty;
                    bool exist = false;
                    foreach (string cartItem in cartItems)
                    {
                        var cartItemParts = cartItem.Split(':');
                        if (cartItemParts[0] == skuId && cartItemParts[2] == shopBranchId.ToString())
                        {
                            newCartInfo += "," + skuId + ":" + (int.Parse(cartItemParts[1]) + count) + ":" + shopBranchId;
                            exist = true;
                        }
                        else
                            newCartInfo += "," + cartItem;
                    }
                    if (!exist)
                        newCartInfo += "," + skuId + ":" + count + ":" + shopBranchId;

                    if (!string.IsNullOrWhiteSpace(newCartInfo))
                        newCartInfo = newCartInfo.Substring(1);
                    WebHelper.SetCookie(CookieKeysCollection.Sgmall_CART_BRANCH, newCartInfo);
                }
                else
                {
                    WebHelper.SetCookie(CookieKeysCollection.Sgmall_CART_BRANCH, string.Format("{0}:{1}:{2}", skuId, count, shopBranchId));
                }
            }
        }

        void CheckSkuIdIsValid(string skuId, long shopBranchId)
        {
            long productId = 0;
            long.TryParse(skuId.Split('_')[0], out productId);
            if (productId == 0)
                throw new Sgmall.Core.InvalidPropertyException("SKUId无效");

            var skuItem = ProductManagerApplication.GetSKU(skuId);
            if (skuItem == null)
                throw new Sgmall.Core.InvalidPropertyException("SKUId无效");

        }

    }

    public class MobileHomeProducts
    {
        /// <summary>
        /// 获取APP首页商品列表
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="platformType"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="keyWords"></param>
        /// <param name="shopName"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public object GetMobileHomeProducts(long shopId, PlatformType platformType, int page, int rows, string keyWords, string shopName, long? categoryId = null)
        {
            var homeProducts = MobileHomeProductApplication.GetMobileHomePageProducts(shopId, platformType, page, rows, keyWords, shopName, categoryId);
            var model = homeProducts.Models.ToArray().Select(item =>
            {
                var brand = BrandApplication.GetBrand(item.Sgmall_Products.BrandId);
                return new
                {
                    name = item.Sgmall_Products.ProductName,
                    image = item.Sgmall_Products.GetImage(ImageSize.Size_50),
                    price = item.Sgmall_Products.MinSalePrice.ToString("F2"),
                    brand = brand == null ? "" : brand.Name,
                    sequence = item.Sequence,
                    categoryName = CategoryApplication.GetCategory(long.Parse(CategoryApplication.GetCategory(item.Sgmall_Products.CategoryId).Path.Split('|').Last())).Name,
                    id = item.Id,
                    productId = item.ProductId,
                    shopName = item.Sgmall_Products.Sgmall_Shops.ShopName
                };
            });
            return new { rows = model, total = homeProducts.Total };
        }
        /// <summary>
        /// 获取指定商家店铺移动端首页商品
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="platformType"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="brandName"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public object GetSellerMobileHomePageProducts(long shopId, PlatformType platformType, int page, int rows, string brandName, long? categoryId = null)
        {
            var homeProducts = MobileHomeProductApplication.GetSellerMobileHomePageProducts(shopId, platformType, page, rows, brandName, categoryId);

            var model = homeProducts.Models.ToArray().Select(item =>
            {
                var brand = BrandApplication.GetBrand(item.Sgmall_Products.BrandId);
                var cate = item.Sgmall_Products.Sgmall_ProductShopCategories.FirstOrDefault();
                return new
                {
                    name = item.Sgmall_Products.ProductName,
                    image = item.Sgmall_Products.GetImage(ImageSize.Size_50),
                    price = item.Sgmall_Products.MinSalePrice.ToString("F2"),
                    brand = brand == null ? "" : brand.Name,
                    sequence = item.Sequence,
                    id = item.Id,
                    categoryName = cate == null ? "" : cate.ShopCategoryInfo.Name,
                    productId = item.ProductId,
                };
            });


            return new { rows = model, total = homeProducts.Total };
        }
        /// <summary>
        /// 添加商品至首页
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="productIds"></param>
        /// <param name="platformType"></param>
        public void AddHomeProducts(long shopId, string productIds, PlatformType platformType)
        {
            var productIdsArr = productIds.Split(',').Select(item => long.Parse(item));
            MobileHomeProductApplication.AddProductsToHomePage(shopId, platformType, productIdsArr);
        }
        /// <summary>
        /// 更新顺序
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="id"></param>
        /// <param name="sequence"></param>
        public void UpdateSequence(long shopId, long id, short sequence)
        {
            MobileHomeProductApplication.UpdateSequence(shopId, id, sequence);
        }
        /// <summary>
        /// 从首页删除商品
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="id"></param>
        public void Delete(long shopId, long id)
        {
            MobileHomeProductApplication.Delete(shopId, id);
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        public void DeleteList(long[] ids)
        {
            MobileHomeProductApplication.DeleteList(ids);
        }
        /// <summary>
        /// 获取指定店铺移动端首页商品
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="platformType"></param>
        /// <returns></returns>
        public object GetAllHomeProductIds(long shopId, PlatformType platformType)
        {
            var homeProductIds = MobileHomeProductApplication.GetMobileHomePageProducts(shopId, platformType).Select(item => item.ProductId);
            return homeProductIds;
        }
    }
}