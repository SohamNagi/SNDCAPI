using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace SNDCAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class IncidentController : ControllerBase
{

    private readonly string _serviceNowUrl;
    private readonly string _username;
    private readonly string _password;

    public IncidentController(IConfiguration configuration)
    {
        _serviceNowUrl = configuration["ServiceNowUrl"]!;
        _username = configuration["Username"]!;
        _password = configuration["Password"]!;
    }


    [HttpPost]
    [Consumes("application/xml")]
    public async Task<IActionResult> PostIncident([FromBody] session session)
    {
        try
        {

            // Extract the relevant data and flatten to json
            FormData flatData = new FormData();
            flatData.u_brand = session.data.policy.brandFlag;
            flatData.u_policynumber = session.data.policy.policyNumber;
            flatData.u_caseid = session.data.policy.workbenchCaseId;
            flatData.u_businessunit = session.data.policy.organizationalUnitDropdown;
            flatData.u_marketdimension = session.data.policy.marketDimension;
            flatData.u_transaction = session.data.currentTransactionType;

            // Convert Json to string
            string jsonData = JsonSerializer.Serialize(flatData);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}")));

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{_serviceNowUrl}/api/now/table/incident", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var incident = JsonSerializer.Deserialize<Root>(responseBody);
                    return Ok($"{_serviceNowUrl}{"/incident.do?number="}{incident.result.number}");

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

