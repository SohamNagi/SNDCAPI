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
    private readonly string _serviceNowUrl;
    private readonly string _clientID;

    public IncidentController(IConfiguration configuration)
    {
        _serviceNowUrl = configuration["ServiceNowUrl"]!;
        _clientID = configuration["clientId"]!;
    }

    [HttpPost]
    public async Task<IActionResult> CreateIncident(string access_token)
    {
        string jsonData;

        // Read, Parse and Store Body Data in Session
        using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
        {
            string xmldata = reader.ReadToEndAsync().Result;

            FormData flatData = new FormData();
            // Create an XDocument
            XDocument xmlDoc = XDocument.Parse(xmldata);
            flatData.u_application = "Wave";
            flatData.u_brand = xmlDoc.XPathSelectElement("/session/data/policy/BrandFlag").Value;
            flatData.u_policynumber = xmlDoc.XPathSelectElement("session/data/policy/PolicyNumber").Value;
            flatData.u_caseid = xmlDoc.XPathSelectElement("session/data/policy/WorkbenchCaseId").Value;
            flatData.u_businessunit = xmlDoc.XPathSelectElement("/session/data/policy/OrganizationalUnitDropdown").Value;
            flatData.u_marketdimension = xmlDoc.XPathSelectElement("/session/data/policy/MarketDimension").Value;
            flatData.u_transaction = xmlDoc.XPathSelectElement("/session/data/CurrentTransactionType").Value;

            // Convert Json to string
            jsonData = JsonSerializer.Serialize(flatData);
        }

        try
        {
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
