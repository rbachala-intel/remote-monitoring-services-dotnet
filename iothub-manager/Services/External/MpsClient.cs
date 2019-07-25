// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.IoTSolutions.IotHubManager.Services.Diagnostics;
using Microsoft.Azure.IoTSolutions.IotHubManager.Services.Exceptions;
using Microsoft.Azure.IoTSolutions.IotHubManager.Services.Http;
using Microsoft.Azure.IoTSolutions.IotHubManager.Services.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HttpClient = Microsoft.Azure.IoTSolutions.IotHubManager.Services.Http.IHttpClient;

namespace Microsoft.Azure.IoTSolutions.IotHubManager.Services.External
{
  public class MpsClient : IMpsClient
  {
    private string mpsServer;
    private HttpClient httpClient;
    private ILogger logger;
    public MpsClient(string mpsServer, IHttpClient httpClient, ILogger logger)
    {
      this.mpsServer = (mpsServer.ToLowerInvariant().StartsWith("https://") ? mpsServer : "https://" + mpsServer);
      this.httpClient = httpClient;
      this.logger = logger;
    }
    // restart = 10
    public Task PowerOffAsync(string guid)
    {
      return PowerActionAsync(guid, 8);
    }
    public async Task PowerActionAsync(string guid, int action)
    {
      /*
      {  
   "apiKey":"string",
   "method":"PowerAction",
   "payload":{  
      "guid":"038d0240-045c-05f4-7706-980700080009",
      "action":2
   }
}

       */
      JObject powerActionObj =
            new JObject(
              new JProperty("method", "PowerAction"),
              new JProperty("payload",
                  new JObject(
                      new JProperty("guid", guid),
                      new JProperty("action", action)
                  )
              ));

      var request = this.CreateRequest("amt", powerActionObj.ToString());

      var response = await this.httpClient.PostAsync(request);
      
      this.logger.Info($"received response from mps statuscode - {response.StatusCode}, content - {response.Content}", () => {});

    }

    private HttpRequest CreateRequest(string path, string content = null)
    {
      var request = new HttpRequest();
      request.SetUriFromString($"{this.mpsServer}/{path}");
      if (this.mpsServer.ToLowerInvariant().StartsWith("https:"))
      {
        request.Options.AllowInsecureSSLServer = true;
      }

      if (content != null)
      {
        request.SetContent(content);
      }

      return request;
    }
    public Task PowerOnAsync(string guid)
    {
      return PowerActionAsync(guid, 2);
    }

    public Task RebootAsync(string guid)
    {
      return PowerActionAsync(guid, 10);
    }
  }
}