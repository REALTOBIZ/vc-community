﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VirtoCommerce.OrderModule.Web.Model
{
	public class PaymentIn : Operation
	{
		public string OrganizationName { get; set; }
		public string OrganizationId { get; set; }
		public string CustomerName { get; set; }
		public string CustomerId { get; set; }

		public string Purpose { get; set; }

		public string GatewayCode { get; set; }

		public DateTime? IncomingDate { get; set; }
		public string OuterId { get; set; }
	}
}