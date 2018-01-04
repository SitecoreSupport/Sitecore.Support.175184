namespace Sitecore.Support.Web.RequestValidators
{
  using System;
  using System.Linq;
  using System.Web;
  using System.Web.Util;
  using Sitecore.Diagnostics;

  public class SitecoreBackendRequestValidator : RequestValidator
  {
    [NotNull]
    internal static readonly string[] SitecoreTrustedUrls = new string[] { "/sitecore/shell/", "/sitecore/admin/", "/-/speak/request/" };

    #region Constructors
    public SitecoreBackendRequestValidator() : this(urlStartPartsToBypass: SitecoreTrustedUrls)
    {
    }

    protected SitecoreBackendRequestValidator(params string[] urlStartPartsToBypass)
    {
      this.UrlStartPartsToBypassValidation = urlStartPartsToBypass;
    }
    #endregion

    [NotNull]
    protected string[] UrlStartPartsToBypassValidation { get; private set; }

    public virtual bool ShouldIgnoreValidation([NotNull]string rawUrl)
    {
      Assert.ArgumentNotNull(rawUrl, "request");
      return this.UrlStartPartsToBypassValidation.Any(urlToByPass => rawUrl.StartsWith(urlToByPass, StringComparison.InvariantCultureIgnoreCase));
    }

    protected override bool IsValidRequestString(HttpContext context, string value, RequestValidationSource requestValidationSource, string collectionKey, out int validationFailureIndex)
    {
      validationFailureIndex = 0;
      var requestUrl = this.ExtractRequestUrl(context);
      if (this.ShouldIgnoreValidation(requestUrl))
      {
        return true;
      }
      return base.IsValidRequestString(context, value, requestValidationSource, collectionKey, out validationFailureIndex);
    }

    public virtual string ExtractRequestUrl([CanBeNull]HttpContext context)
    {
      if ((context == null) || (context.Request == null))
      {
        return string.Empty;
      }
      return context.Request.RawUrl ?? string.Empty;
    }
  }
}