using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SNDCAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class IncidentController : ControllerBase
{

    private static Random random = new Random();
    public static string RandomString(int length) =>
        new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", length)
                              .Select(s => s[random.Next(s.Length)]).ToArray());
    private readonly string _serviceNowUrl;
    private readonly string _clientID;

    public IncidentController(IConfiguration configuration)
    {
        _serviceNowUrl = configuration["ServiceNowUrl"]!;
        _clientID = configuration["clientId"]!;
    }

    [HttpPost]
    public IActionResult AcceptIncident()
    {
        using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
        {
            HttpContext.Session.SetString("StoredXml", reader.ReadToEndAsync().Result);
        }

        // Construct the OAuth URL with necessary parameters
        var state = RandomString(5); // Generating a unique state value for each request for CSRF protection
        var redirectUri = "https://webhook.site/fd6dbc2a-9926-4d94-b393-cb17912c9ef6";
        var authorizationUrl = $"{_serviceNowUrl}/oauth_auth.do?response_type=token&client_id={_clientID}&redirect_uri={redirectUri}&state={state}";

        // Store state in session for future verification
        HttpContext.Session.SetString("State", state);

        // Redirect the user to the OAuth provider's authorization page
        return Ok(authorizationUrl);
    }

    [HttpGet("OAuthCallback")]
    public async Task<IActionResult> OAuthCallback(string access_token, string state)
    {
        // Validate the 'state' parameter
        if (state != HttpContext.Session.GetString("State"))
        {
            return StatusCode(999, "CSRF attack: State Mismatch");
        }

        // Retrieve any stored session data
        var storedData = HttpContext.Session.GetString("StoredData");

        // Using the access token to make Table API call
        try
        {
            FormData flatData = new FormData();
            // Create an XDocument
            XDocument xmlDoc = XDocument.Parse(storedData);
            flatData.u_brand = xmlDoc.XPathSelectElement("/session/data/policy/BrandFlag").Value;
            flatData.u_policynumber = xmlDoc.XPathSelectElement("session/data/policy/PolicyNumber").Value;
            flatData.u_caseid = xmlDoc.XPathSelectElement("session/data/policy/WorkbenchCaseId").Value;
            flatData.u_businessunit = xmlDoc.XPathSelectElement("/session/data/policy/OrganizationalUnitDropdown").Value;
            flatData.u_marketdimension = xmlDoc.XPathSelectElement("/session/data/policy/MarketDimension").Value;
            flatData.u_transaction = xmlDoc.XPathSelectElement("/session/data/CurrentTransactionType").Value;

            // Convert Json to string
            string jsonData = JsonSerializer.Serialize(flatData);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer", access_token);

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{_serviceNowUrl}/api/now/table/incident", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var incident = JsonSerializer.Deserialize<jsonRoot>(responseBody);
                    return Ok($"{_serviceNowUrl}{"/nav_to.do?uri=incident.do?sysparm_query=number%3D"}{incident.result.number}");
                }
                else
                {
                    return StatusCode((int)response.StatusCode,
                                  "Error sending data: " + await response.Content.ReadAsStringAsync());
                }
            }
        }
        catch (Exception ex)
        {
            return BadRequest("Error processing data: " + ex.Message);
        }
    }
}
