using Sgmall.Application;
using Sgmall.CommonModel;
using Sgmall.Core.Helper;
using System;
using System.Web.Mvc;

namespace Sgmall.Web.Framework
{
    public abstract class BaseWebController : BaseController
    {
        ISellerManager sellerManager = null;

        public string bizUserIdBase = System.Configuration.ConfigurationManager.AppSettings["bizUserIdBase"].ToString();
        /// <summary>
        /// 通联账户
        /// </summary>
        public string bizUserId {
            get {
                if (CurrentSellerManager != null)
                {
                    return bizUserIdBase + CurrentSellerManager.ShopId;
                }
                else {
                    return bizUserIdBase + Guid.NewGuid();
                }
            }
        }

        public long UserId
        {
            get
            {
                if (CurrentUser != null)
                    return CurrentUser.Id;
                return 0;
            }
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.IsChildAction && !filterContext.HttpContext.Request.IsAjaxRequest())
            {
                //统计代码
                StatisticApplication.StatisticPlatVisitUserCount();
            }
            base.OnActionExecuting(filterContext);
        }
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.IsChildAction)
            {
                return;
            }
            base.OnActionExecuted(filterContext);
        }

        /// <summary>
        /// 当前管理员
        /// </summary>
        public ISellerManager CurrentSellerManager
        {
            get
            {
                if (sellerManager != null)
                {
                    return sellerManager;
                }
                else
                {
                    long userId = UserCookieEncryptHelper.Decrypt(WebHelper.GetCookie(CookieKeysCollection.SELLER_MANAGER), CookieKeysCollection.USERROLE_SELLERADMIN);
                    if (userId != 0)
                    {
                        sellerManager = ManagerApplication.GetSellerManager(userId);
                    }
                }
                return sellerManager;
            }
        }
    }
}
