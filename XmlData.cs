using System.Xml.Serialization;

namespace SNDCAPI;

[Serializable, XmlRoot("session")]
public class session
{
    [XmlElement("data")]
    public data data { get; set; }
}

public partial class data
{
    [XmlElement("policy")]
    public policy policy { get; set; }

    [XmlElement("currentTransactionType")]
    public string currentTransactionType { get; set; }
}

public partial class policy
{
    [XmlElement("brandFlag")]
    public string brandFlag { get; set; }

    [XmlElement("policyNumber")]
    public string policyNumber { get; set; }

    [XmlElement("workbenchCaseId")]
    public string workbenchCaseId { get; set; }

    [XmlElement("organizationalUnitDropdown")]
    public string organizationalUnitDropdown { get; set; }

    [XmlElement("marketDimension")]
    public string marketDimension { get; set; }
}

public class FormData
{
    public const string u_application = "Wave";
    public string u_policynumber { get; set; }
    public string u_caseid { get; set; }
    public string u_brand { get; set; }
    public string u_businessunit { get; set; }
    public string u_marketdimension { get; set; }
    public string u_transaction { get; set; }
    public FormData(string policyNumber, string caseID, string brand, string businessUnit, string marketDimension, string transaction)
    {
        u_policynumber = policyNumber;
        u_caseid = caseID;
        u_brand = brand;
        u_businessunit = businessUnit;
        u_marketdimension = marketDimension;
        u_transaction = transaction;
    }

    public FormData() { }
}

public class OpenedBy
{
    public string Link { get; set; }
    public string Value { get; set; }
}

public class Result
{
    public string parent { get; set; }
    public string made_sla { get; set; }
    public string caused_by { get; set; }
    public string watch_list { get; set; }
    public string upon_reject { get; set; }
    public string sys_updated_on { get; set; }
    public string child_incidents { get; set; }
    public string hold_reason { get; set; }
    public string origin_table { get; set; }
    public string task_effective_number { get; set; }
    public string approval_history { get; set; }
    public string number { get; set; }
    public string resolved_by { get; set; }
    public string sys_updated_by { get; set; }
    public OpenedBy opened_by { get; set; }
    public string user_input { get; set; }
    public string sys_created_on { get; set; }
    public SysDomain sys_domain { get; set; }
    public string state { get; set; }
    public string route_reason { get; set; }
    public string sys_created_by { get; set; }
    public string knowledge { get; set; }
    public string order { get; set; }
    public string calendar_stc { get; set; }
    public string closed_at { get; set; }
    public string cmdb_ci { get; set; }
    public string delivery_plan { get; set; }
    public string contract { get; set; }
    public string impact { get; set; }
    public string active { get; set; }
    public string work_notes_list { get; set; }
    public string business_service { get; set; }
    public string business_impact { get; set; }
    public string priority { get; set; }
    public string sys_domain_path { get; set; }
    public string rfc { get; set; }
    public string time_worked { get; set; }
    public string expected_start { get; set; }
    public string opened_at { get; set; }
    public string business_duration { get; set; }
    public string group_list { get; set; }
    public string work_end { get; set; }
    public string caller_id { get; set; }
    public string reopened_time { get; set; }
    public string resolved_at { get; set; }
    public string approval_set { get; set; }
    public string subcategory { get; set; }
    public string work_notes { get; set; }
    public string universal_request { get; set; }
    public string short_description { get; set; }
    public string close_code { get; set; }
    public string correlation_display { get; set; }
    public string delivery_task { get; set; }
    public string work_start { get; set; }
    public string assignment_group { get; set; }
    public string additional_assignee_list { get; set; }
    public string business_stc { get; set; }
    public string cause { get; set; }
    public string description { get; set; }
    public string origin_id { get; set; }
    public string calendar_duration { get; set; }
    public string close_notes { get; set; }
    public string notify { get; set; }
    public string service_offering { get; set; }
    public string sys_class_name { get; set; }
    public string closed_by { get; set; }
    public string follow_up { get; set; }
    public string parent_incident { get; set; }
    public string sys_id { get; set; }
    public string contact_type { get; set; }
    public string reopened_by { get; set; }
    public string incident_state { get; set; }
    public string urgency { get; set; }
    public string problem_id { get; set; }
    public string company { get; set; }
    public string reassignment_count { get; set; }
    public string activity_due { get; set; }
    public string assigned_to { get; set; }
    public string severity { get; set; }
    public string comments { get; set; }
    public string approval { get; set; }
    public string sla_due { get; set; }
    public string comments_and_work_notes { get; set; }
    public string due_date { get; set; }
    public string sys_mod_count { get; set; }
    public string reopen_count { get; set; }
    public string sys_tags { get; set; }
    public string escalation { get; set; }
    public string upon_approval { get; set; }
    public string correlation_id { get; set; }
    public string location { get; set; }
    public string category { get; set; }
}

public class jsonRoot
{
    public Result result { get; set; }
}

public class SysDomain
{
    public string link { get; set; }
    public string value { get; set; }
}

