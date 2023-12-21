namespace KiboWebhookListener.Exceptions;

using System;
using System.Net.Http;

public class KiboException : Exception
{

		public KiboException(HttpResponseMessage response)
		{

		}

}
