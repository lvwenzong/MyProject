﻿using Sgmall.Application;
using Sgmall.CommonModel;
using Sgmall.DTO;
using Sgmall.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sgmall.Web.Framework
{
    public class BrowseHistrory
    {
        /// <summary>
        /// 添加商品浏览记录
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="userId"></param>
        public static void AddBrowsingProduct(long productId, long userId = 0)
        {
            List<ProductBrowsedHistoryModel> productIdList = new List<ProductBrowsedHistoryModel>();
            string productIds = Core.Helper.WebHelper.GetCookie(CookieKeysCollection.Sgmall_PRODUCT_BROWSING_HISTORY);
            if (!string.IsNullOrEmpty(productIds))
            {
                var arr = productIds.Split(',');
                foreach (var a in arr)
                {
                    var item = a.Split('#');
                    if (item.Length > 1)
                    {
                        productIdList.Add(new ProductBrowsedHistoryModel() { ProductId = long.Parse(item[0]), BrowseTime = DateTime.Parse(item[1]) });
                    }
                    else
                    {
                        productIdList.Add(new ProductBrowsedHistoryModel() { ProductId = long.Parse(item[0]), BrowseTime = DateTime.Now });
                    }
                }
            }
            if (productIdList.Count < 20 && !productIdList.Any(a => a.ProductId == productId))
            {
                productIdList.Add(new ProductBrowsedHistoryModel() { ProductId = productId, BrowseTime = DateTime.Now });
            }
            else if (productIdList.Count >= 20 && !productIdList.Any(a => a.ProductId == productId))
            {
                productIdList.RemoveAt(productIdList.Count - 1);
                productIdList.Add(new ProductBrowsedHistoryModel() { ProductId = productId, BrowseTime = DateTime.Now });
            }
            else
            {
                var model = productIdList.Where(a => a.ProductId == productId).FirstOrDefault();
                productIdList.Remove(model);
                productIdList.Add(new ProductBrowsedHistoryModel() { ProductId = productId, BrowseTime = DateTime.Now });
            }
            if (userId == 0)
            {
                var productsStr = "";
                foreach (var item in productIdList)
                {
                    productsStr += item.ProductId + "#" + item.BrowseTime.ToString() + ",";
                }
                Core.Helper.WebHelper.SetCookie(CookieKeysCollection.Sgmall_PRODUCT_BROWSING_HISTORY, productsStr.TrimEnd(','), DateTime.Now.AddDays(7));
            }
            else
            {
                foreach (var item in productIdList)
                {
                    try
                    {
                        ProductManagerApplication.AddBrowsingProduct(new BrowsingHistoryInfo() { MemberId = userId, BrowseTime = item.BrowseTime, ProductId = item.ProductId });
                    }
                    catch
                    {
                        continue;
                    }
                }
                Core.Helper.WebHelper.DeleteCookie(CookieKeysCollection.Sgmall_PRODUCT_BROWSING_HISTORY);
            }
        }
        /// <summary>
        /// 获取商品浏览记录
        /// </summary>
        /// <param name="num"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<ProductBrowsedHistoryModel> GetBrowsingProducts(int num, long userId = 0)
        {
            List<ProductBrowsedHistoryModel> productIdList = new List<ProductBrowsedHistoryModel>();
            string productIds = Core.Helper.WebHelper.GetCookie(CookieKeysCollection.Sgmall_PRODUCT_BROWSING_HISTORY);
            if (!string.IsNullOrEmpty(productIds))
            {
                var arr = productIds.Split(',');
                foreach (var a in arr)
                {
                    var item = a.Split('#');
                    if (item.Length > 1)
                    {
                        productIdList.Add(new ProductBrowsedHistoryModel() { ProductId = long.Parse(item[0]), BrowseTime = DateTime.Parse(item[1]) });
                    }
                    else
                    {
                        productIdList.Add(new ProductBrowsedHistoryModel() { ProductId = long.Parse(item[0]), BrowseTime = DateTime.Now });
                    }
                }
            }

            var ids = productIdList.Select(p => p.ProductId).ToList();
            List<FlashSalePrice> flashSaleList = LimitTimeApplication.GetPriceByProducrIds(ids);

            List<ProductBrowsedHistoryModel> model = new List<ProductBrowsedHistoryModel>();
            if (userId == 0)
            {
                var products = ProductManagerApplication.GetProductByIds(productIdList.Select(a => a.ProductId))
                    .Where(d => d.SaleStatus == ProductInfo.ProductSaleStatus.OnSale
                        && d.AuditStatus == ProductInfo.ProductAuditStatus.Audited).ToArray()
                    .Select(a => new ProductBrowsedHistoryModel()
                    {
                        ImagePath = Core.SgmallIO.GetProductSizeImage(a.RelativePath, 1, (int)ImageSize.Size_100),
                        ProductId = a.Id,
                        ProductName = a.ProductName,
                        ProductPrice = GetRealPrice(flashSaleList, a.Id, a.MinSalePrice),
                        ShopId = a.ShopId
                    }).ToList();

                //foreach (var product in products)
                //{
                //    var m = productIdList.Where(item => item.ProductId == product.ProductId).FirstOrDefault();
                //    m.ImagePath = product.ImagePath + "/1_150.png";
                //    m.ProductName = product.ProductName;
                //    m.ProductPrice = GetRealPrice( flashSaleList , product.ProductId , product.ProductPrice );
                //}
                return products.OrderByDescending(a => a.BrowseTime).ToList();
            }
            else
            {
                foreach (var m in productIdList)
                {
                    AddBrowsingProduct(m.ProductId, userId);
                }
                model = ProductManagerApplication.GetBrowsingProducts(userId)
                    .Where(d => d.Sgmall_Products.SaleStatus == ProductInfo.ProductSaleStatus.OnSale
                        && d.Sgmall_Products.AuditStatus == ProductInfo.ProductAuditStatus.Audited)
                   .OrderByDescending(a => a.BrowseTime).Take(num).ToArray().AsEnumerable()
                   .Select(a => new ProductBrowsedHistoryModel()
                   {
                       ImagePath = Core.SgmallIO.GetProductSizeImage(a.Sgmall_Products.RelativePath, 1, (int)ImageSize.Size_100),
                       ProductId = a.ProductId,
                       ProductName = a.Sgmall_Products.ProductName,
                       ProductPrice = GetRealPrice(flashSaleList, a.ProductId, a.Sgmall_Products.MinSalePrice),
                       BrowseTime = a.BrowseTime,
                       ShopId = a.Sgmall_Products.ShopId
                   }).ToList();
            }
            return model;
        }

        private static decimal GetRealPrice(List<FlashSalePrice> list, long productid, decimal oldPrice)
        {
            var model = list.Where(p => p.ProductId == productid).FirstOrDefault();
            if (model != null)
            {
                return model.MinPrice;
            }
            return oldPrice;
        }
    }
}
