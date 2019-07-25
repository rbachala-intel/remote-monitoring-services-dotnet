// Copyright (c) Microsoft. All rights reserved.

using System.Threading.Tasks;

namespace Microsoft.Azure.IoTSolutions.IotHubManager.Services.External
{
    public interface IMpsClient
    {
        Task PowerActionAsync(string guid, int action);
        Task PowerOnAsync(string guid);
        Task PowerOffAsync(string guid);
        Task RebootAsync(string guid);
    }
}