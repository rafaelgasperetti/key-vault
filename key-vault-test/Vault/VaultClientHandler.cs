namespace key_vault_test.Vault
{
    internal class VaultClientHandler : DelegatingHandler
    {
        private readonly HttpContent? Content = null;

        public VaultClientHandler(HttpContent? content = null, HttpClientHandler? httpHandler = null) : base(httpHandler ?? new HttpClientHandler())
        {
            Content = content;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (Content != null && request.Content == null)
            {
                request.Content = Content;
            }

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}
