﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Domain.Payment.Model
{
	public class CaptureProcessPaymentResult : IProcessResult
	{
		public bool IsSuccess { get; set; }
		public string ErrorMessage { get; set; }
		public PaymentStatus NewPaymentStatus { get; set; }
		public string OuterId { get; set; }
	}
}
