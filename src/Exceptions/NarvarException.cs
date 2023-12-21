namespace KiboWebhookListener.Exceptions;

using System;
using System.Net.Http;
using System.Text.Json;
using Models.Narvar;

public class NarvarException : Exception
{
    public NarvarResponse? JsonParameter { get; private set; }

    public NarvarException(HttpResponseMessage response)
        : this(DeserializeError(response))
    {
    }

    public NarvarException(string code, string error)
        : this(GenerateError(code, error))
    {
    }

    private NarvarException(NarvarResponse? error)
    {
        JsonParameter = error ?? throw new ArgumentNullException(nameof(error));
    }

    private static NarvarResponse? DeserializeError(HttpResponseMessage response)
    {
        var errorResponseBody = response.Content?.ReadAsStringAsync().Result;
        return JsonSerializer.Deserialize<NarvarResponse>(errorResponseBody!);
    }

    private static NarvarResponse GenerateError(string code, string error)
    {
        return new NarvarResponse
        {
            status = "FAILURE",
            messages = new NarvarMessage[]
            {
                new()
                {
                    code = code,
                    message = error
                 }
             }
         };
    }
}
