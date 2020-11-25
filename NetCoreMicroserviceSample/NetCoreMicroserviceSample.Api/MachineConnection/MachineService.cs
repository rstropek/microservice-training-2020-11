using NetCoreMicroserviceSample.MachineService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace NetCoreMicroserviceSample.Api.MachineConnection
{
    /// <summary>
    /// This class is acting as gRPC communication endpoint. See https://grpc.io/ for further information.
    /// </summary>
    [SuppressMessage("", "CA1724", Justification = "Name is fine")]
    public class MachineService : IMachineService
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly IConfiguration configuration;
        private readonly ILogger<MachineService> logger;
        private readonly MachineAccess.MachineAccessClient machineClient;
        private static Metadata? accessToken;
        private static DateTime? accessTokenValidUntil;

        [SuppressMessage("Design", "CA1812", Justification = "Instantiated by JsonSerializer")]
        private class OidcAccessToken
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; } = string.Empty;
        }

        public MachineService(IHttpClientFactory clientFactory, IConfiguration configuration,
            ILogger<MachineService> logger, MachineAccess.MachineAccessClient machineClient)
        {
            this.clientFactory = clientFactory;
            this.configuration = configuration;
            this.logger = logger;
            this.machineClient = machineClient;
        }

        private async Task RefreshAccessToken()
        {
            if (accessToken != null && DateTime.UtcNow.AddMinutes(5) < accessTokenValidUntil)
            {
                // Refresh token is still fine
                return;
            }

            // We have to refresh the access token.
            logger.LogInformation("Refreshing access token");

            var client = clientFactory.CreateClient("identity-server");
            var formDictionary = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", configuration["Oidc:MachineClientId"] },
                { "client_secret", configuration["Oidc:MachineClientSecret"] },
                { "audience", "api" }
            };
            using var formContent = new FormUrlEncodedContent(formDictionary!);
            var response = await client.PostAsync(new Uri($"{configuration["Oidc:Domain"]}/connect/token"), formContent);
            response.EnsureSuccessStatusCode();
            var token = await response.Content.ReadFromJsonAsync<OidcAccessToken>();
            if (token == null || string.IsNullOrEmpty(token.AccessToken))
            {
                throw new InvalidOperationException("Cannot get access token, this should never happen");
            }

            accessToken = new Metadata
            {
                { "Authorization", $"Bearer {token.AccessToken}" }
            };
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token.AccessToken);
            accessTokenValidUntil = jwtToken.ValidTo;
            logger.LogInformation("New access token valid until {validUntil}", accessTokenValidUntil);
        }

        public async Task<MachineResponse> UpdateSettingsAsync(MachineSettingsUpdate request)
        {
            await RefreshAccessToken();
            return await machineClient.UpdateSettingsAsync(request, accessToken);
        }

        public async Task<MachineResponse> TriggerSwitchAsync(SwitchTrigger request)
        {
            await RefreshAccessToken();
            return await machineClient.TriggerSwitchAsync(request, accessToken);
        }

        public virtual async Task<AsyncServerStreamingCall<MeasurementResponse>> GetMeasurementStream(MeasurementRequest request, CancellationToken cancellationToken)
        {
            await RefreshAccessToken();
            return machineClient.GetMeasurementStream(request, accessToken, cancellationToken: cancellationToken);
        }
    }
}
