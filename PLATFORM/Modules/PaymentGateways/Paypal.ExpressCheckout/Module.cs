﻿using System;
using Microsoft.Practices.Unity;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Common;
using VirtoCommerce.Domain.Marketing.Services;
using VirtoCommerce.Domain.Order.Services;
using VirtoCommerce.Domain.Payment.Model;
using VirtoCommerce.Domain.Payment.Services;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;
using Paypal.ExpressCheckout.Managers;

namespace Paypal.ExpressCheckout
{
	public class Module : IModule
	{
		private readonly IUnityContainer _container;

		public Module(IUnityContainer container)
		{
			_container = container;
		}

		#region IModule Members

		public void SetupDatabase(SampleDataLevel sampleDataLevel)
		{

		}

		public void Initialize()
		{
			var settings = _container.Resolve<ISettingsManager>().GetModuleSettings("Paypal.ExpressCheckout");

			Func<PaypalExpressCheckoutPaymentMethod> paypalBankCardsExpressCheckoutPaymentMethodFactory = () =>
			{
				return new PaypalExpressCheckoutPaymentMethod()
				{
					Name = "Paypal Express Checkout",
					Description = "PayPal express checkout integration",
					LogoUrl = "http://www.credit-card-logos.com/images/multiple_credit-card-logos-2/credit_card_paypal_logos_2.gif",
					Settings = settings
				};
			};

			_container.Resolve<IPaymentMethodsService>().RegisterPaymentMethod(paypalBankCardsExpressCheckoutPaymentMethodFactory);
		}

		public void PostInitialize()
		{

		}

		#endregion
	}
}
