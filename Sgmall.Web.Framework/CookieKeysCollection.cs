
namespace Sgmall.Web.Framework
{
    /// <summary>
    /// Cookie集合
    /// </summary>
    public class CookieKeysCollection
    {
        /// <summary>
        /// 平台管理员登录标识
        /// </summary>
        public const string PLATFORM_MANAGER = "Sgmall-PlatformManager";

        /// <summary>
        /// 商家管理员登录标识
        /// </summary>
        public const string SELLER_MANAGER = "Sgmall-SellerManager";

        /// <summary>
        /// 会员登录标识
        /// </summary>
        public const string Sgmall_USER = "Sgmall-User";
        /// <summary>
        /// 会员登录标识
        /// </summary>
        public const string Sgmall_ACTIVELOGOUT = "d783ea20966909ff";  //Sgmall_ACTIVELOGOUT做MD5后的16位字符
        /// <summary>
        /// 分销合作者编号
        /// </summary>
        public const string Sgmall_DISTRIBUTIONUSERLINKIDS = "d2cccb104922d434";   //Sgmall_DISTRIBUTIONUSERLINKIDS做MD5后的16位字符
        /// <summary>
        /// 不同平台用户key
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static string Sgmall_USER_KEY(string platform)
        {
            return string.Format("Sgmall-{0}User", platform);
        }
        /// <summary>
        /// 
        /// </summary>
        public const string Sgmall_USER_OpenID = "Sgmall-User_OpenId";
        /// <summary>
        /// 购物车
        /// </summary>
        public const string Sgmall_CART = "Sgmall-CART";
        /// <summary>
        /// 门店购物车
        /// </summary>
        public const string Sgmall_CART_BRANCH = "Sgmall-CART-BRANCH";

        /// <summary>
        /// 商品浏览记录
        /// </summary>
        public const string Sgmall_PRODUCT_BROWSING_HISTORY = "Sgmall_ProductBrowsingHistory";
        
        /// <summary>
        /// 最后产生访问时间
        /// </summary>
        public const string Sgmall_LASTOPERATETIME = "Sgmall_LastOpTime";

        /// <summary>
        /// 标识是平台还是商家公众号
        /// </summary>
        public const string MobileAppType = "Sgmall-Mobile-AppType";
        /// <summary>
        /// 访问的微店标识
        /// </summary>
        public const string Sgmall_VSHOPID = "Sgmall-VShopId";

		/// <summary>
		/// 用户角色(Admin)
		/// </summary>
		public const string USERROLE_ADMIN = "0";
		/// <summary>
		/// 用户角色(SellerAdmin)
		/// </summary>
		public const string USERROLE_SELLERADMIN = "1";
		/// <summary>
		/// 用户角色(User)
		/// </summary>
		public const string USERROLE_USER = "2";
    }
}