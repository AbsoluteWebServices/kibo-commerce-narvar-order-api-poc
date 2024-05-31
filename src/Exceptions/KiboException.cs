namespace KiboWebhookListener.Exceptions;

using System;
using System.Net.Http;

public class KiboException : Exception
{

		public KiboException(HttpResponseMessage response)
		{
			Console.WriteLine("KiboException: " + response.StatusCode);
			Console.WriteLine("KiboException: " + response.ReasonPhrase);
			Console.WriteLine("KiboException: " + response.Content.ReadAsStringAsync().Result);

		}

}
