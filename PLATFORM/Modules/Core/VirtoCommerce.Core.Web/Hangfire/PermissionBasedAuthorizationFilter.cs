﻿using System.Collections.Generic;
using Hangfire.Dashboard;
using Microsoft.Owin;
using VirtoCommerce.Framework.Web.Security;

namespace VirtoCommerce.CoreModule.Web.Hangfire
{
    public class PermissionBasedAuthorizationFilter : CheckPermissionAttribute, IAuthorizationFilter
    {
        private readonly IPermissionService _permissionService;

        public PermissionBasedAuthorizationFilter(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public bool Authorize(IDictionary<string, object> owinEnvironment)
        {
            var context = new OwinContext(owinEnvironment);
            var principal = context.Authentication.User;
            var isAuthorized = IsAuthorized(_permissionService, principal);
            return isAuthorized;
        }
    }
}
