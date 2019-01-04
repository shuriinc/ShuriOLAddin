using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Http.Headers;
using System.Threading;
using Newtonsoft.Json;

namespace ShuriOutlookAddIn
{


    public class DocObjectPost
    {
        public Guid id { get; set; }
        public Guid collection_Id { get; set; }
        public string typename { get; set; }
        public string name { get; set; }
        public string value { get; set; }
    }
    public class TextValuePair
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
    public class TextValuePairEntity
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public string Entity { get; set; }
    }
    public class ShTimeZone
    {
        public string id { get; set; }
        public string displayName { get; set; }
    }

    public class APIKeyRequest
    {
        public System.Guid Key { get; set; }
        public string URI { get; set; }
        public string Usage { get; set; }
    }


    public class DeleteEntityObject
    {
        public Guid id { get; set; }
    }

    public class ReportDef
    {
        public ReportDef()
        {
            url = name = description = templateType = customName = "";
            pagesize = 1;
            isWord = false;
            isCustom = false;
        }
        public string url { get; set; }
        public bool isWord { get; set; }
        public bool isCustom { get; set; }
        public int pagesize { get; set; }
        public Guid ownedByGroup_Id { get; set; }
        public string name { get; set; }
        public string customName { get; set; }
        public string templateType { get; set; }
        public string description { get; set; }
        public Guid collection_Id { get; set; }
        public EntityTypes entityType { get; set; }

    }
    public class PrequeryItem
    {
        public string DBName { get; set; }
        public Guid DBId { get; set; }
        public int count { get; set; }
        public EntityTypes entityType { get; set; }
    }
    public class Usage
    {
        public Usage()
        {
            entityType = EntityTypes.All;
            usageDate = DateTime.MinValue;
            method = resource = payload = description = ""; 
        }
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid EntityId { get; set; }
        public EntityTypes entityType { get; set; }
        public DateTime usageDate { get; set; }
        public string method { get; set; }
        public string resource { get; set; }
        public string payload { get; set; }
        public string description { get; set; }
    }

    public class QueryItem
    {
        public string Field { get; set; }
        public QueryOperator Operator { get; set; }
        public string Value { get; set; }

    }
    public class QueryRequest
    {
        public QueryRequest()
        {
            summary = "";
            collectionIds = new List<Guid>();
            groupIds = new List<Guid>();
            groups = new List<Group>();
            orgIds = new List<Guid>();
            organizations = new List<Group>();
            ownerIds = new List<Guid>();
            owners = new List<PersonTenured>();
            personIds = new List<Guid>();
            people = new List<PersonTenured>();
            tagIds = new List<Guid>();
            tagIdsAll = new List<Guid>();
            teamIds = new List<Guid>();
            teams = new List<Group>();
            touchIds = new List<Guid>();
            usertypeIds = new List<Guid>();
            timePeriod = "recent";
            queryItems = new List<QueryItem>();
            dateEndUTC = dateStartUTC = DateTime.MinValue;
            pagesize = 50;
            page = 1;
            proximity = new ProximityItem();
        }

        public Guid id { get; set; }
        public string summary { get; set; }
        public EntityTypes entityType { get; set; }
        public List<Guid> collectionIds { get; set; }
        public List<Guid> groupIds { get; set; }
        public List<Group> groups { get; set; }
        public List<Guid> orgIds { get; set; }
        public List<Group> organizations { get; set; }
        public List<Guid> ownerIds { get; set; }
        public List<PersonTenured> owners { get; set; }
        public List<Guid> personIds { get; set; }
        public List<PersonTenured> people { get; set; }
        public List<Guid> tagIds { get; set; }
        public List<Guid> tagIdsAll { get; set; }
        public List<Guid> teamIds { get; set; }
        public List<Group> teams { get; set; }
        public List<Guid> touchIds { get; set; }
        public List<Guid> usertypeIds { get; set; }
        public List<QueryItem> queryItems { get; set; }
        public string timePeriod { get; set; }
        public DateTime dateStartUTC { get; set; }
        public DateTime dateEndUTC { get; set; }
        public RecordType recordType { get; set; }
        public ProximityItem proximity { get; set;  }
        public int pagesize { get; set; }
        public int page { get; set; }
    }

    public class ProximityItem
    {
        public ProximityItem()
        {
            point = "";
            distanceKm = false;
            distance = 0;
        }
        public string point { get; set; }
        public bool distanceKm { get; set; }
        public double distance { get; set; }


    }
    public class AutocompleteResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageUrlThumb { get; set; }
        public string Nickname { get; set; }
        public EntityTypes EntityType { get; set; }
        public bool IsMember { get; set; }
        public string Sorter { get; set; }
    }

    public class WatchedItem
    {
        public Guid entity_Id { get; set; }
        public string entityName { get; set; }
        public EntityTypes entityType { get; set; }
        public WatchType watchType { get; set; }
        public string watchValue { get; set; }
        public Guid collection_Id { get; set; }
        public Guid ownedBy_Id { get; set; }
        public Guid ownedByGroup_Id { get; set; }
    }

    public class AuditSummary
    {
        public string auditTypename { get; set; }
        public AuditType auditType { get; set; }
        public int count { get; set; }

    }
    public class AuditItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string auditTypename { get; set; }
        public AuditType auditType { get; set; }
        public DateTime date { get; set; }
        public Guid OwnedBy_Id { get; set; }
        public string OwnedByName { get; set; }

    }
    public class FileDownload
    {
        public MemoryStream content { set; get; }
        public string filename { set; get; }
        public long size { set; get; }
        public MediaTypeHeaderValue mime { set; get; }
    }

    public class ResolveStringClass
    {
        public EntityTypes entityType { get; set; }
        public Guid entityId { get; set; }
    }

    
    public class ErrorMessage
    {
        public string message { set; get; }
    }

    #region Account Models

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        [Display(Name = "Days to Remember")]
        public int DaysRemember { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    //public class RegisterViewModel
    //{
    //    [Required]
    //    [Display(Name = "User name")]
    //    public string UserName { get; set; }

    //    [Required]
    //    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    //    [DataType(DataType.Password)]
    //    [Display(Name = "Password")]
    //    public string Password { get; set; }

    //    [DataType(DataType.Password)]
    //    [Display(Name = "Confirm password")]
    //    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    //    public string ConfirmPassword { get; set; }

    //    [Required]
    //    [Display(Prompt = "First")]
    //    [StringLength(140)]
    //    public string Firstname { get; set; }

    //    [Display(Prompt = "Middle")]
    //    [StringLength(140)]
    //    public string Middlename { get; set; }

    //    [Required]
    //    [Display(Prompt = "Last")]
    //    [StringLength(140)]
    //    public string Lastname { get; set; }

    //    [Required]
    //    [Display(Name = "Email address")]
    //    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    //    [StringLength(512)]
    //    public string Email { get; set; }

    //    [Display(Name = "SMS Phone")]
    //    [StringLength(512)]
    //    public string Phone { get; set; }


    //}

    public class RegisterModel
    {
        public RegisterModel()
        {
            UserName = Password = Firstname = Lastname = Sitename = "";
            sendAgreement = userAgreed = freeTrial = false;
        }
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [StringLength(140)]
        public string Firstname { get; set; }

        [StringLength(140)]
        public string Lastname { get; set; }
        public string Sitename { get; set; }

        public bool freeTrial { get; set; }
        public bool sendAgreement { get; set; }
        public bool userAgreed { get; set; }

    }

    //public class ReregisterViewModel
    //{

    //    [Required]
    //    [Display(Name = "User name")]
    //    public string UserName { get; set; }

    //    [Required]
    //    [Display(Prompt = "First")]
    //    [StringLength(140)]
    //    public string Firstname { get; set; }

    //    [Display(Prompt = "Middle")]
    //    [StringLength(140)]
    //    public string Middlename { get; set; }

    //    [Required]
    //    [Display(Prompt = "Last")]
    //    [StringLength(140)]
    //    public string Lastname { get; set; }

    //    [Required]
    //    [Display(Name = "Email address")]
    //    [StringLength(512)]
    //    public string Email { get; set; }

    //    public Guid UserId { get; set; }


    //}

    //public class ExternalLoginViewModel
    //{
    //  public string Name { get; set; }

    //  public string Url { get; set; }

    //  public string State { get; set; }
    //}

    //public class ManageInfoViewModel
    //{
    //  public string LocalLoginProvider { get; set; }

    //  public string UserName { get; set; }

    //  public IEnumerable<UserLoginInfoViewModel> Logins { get; set; }

    //  public IEnumerable<ExternalLoginViewModel> ExternalLoginProviders { get; set; }
    //}

    //public class UserInfoViewModel
    //{
    //  public string UserName { get; set; }

    //  public bool HasRegistered { get; set; }

    //  public string LoginProvider { get; set; }
    //}

    //public class UserLoginInfoViewModel
    //{
    //  public string LoginProvider { get; set; }

    //  public string ProviderKey { get; set; }
    //}

    //public class AddExternalLoginBindingModel
    //{
    //  [Required]
    //  [Display(Name = "External access token")]
    //  public string ExternalAccessToken { get; set; }
    //}

    public class ChangePasswordBindingModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }


    public class RemoveLoginBindingModel
    {
        [Required]
        [Display(Name = "Login provider")]
        public string LoginProvider { get; set; }

        [Required]
        [Display(Name = "Provider key")]
        public string ProviderKey { get; set; }
    }

    public class SetPasswordBindingModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordModel
    {
        [Required]
        public string UserName { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }

    }

    #endregion

    #region Worker Entities

    public class WorkQueueItem
    {
        public Touch touch { get; set; }
        public Guid subId { get; set; }
        public Guid entityId { get; set; }
        public EntityTypes entityType { get; set; }
        public string entityName { get; set; }
        public string workType { get; set; }
        public WorkerProcessStatus workerProcessStatus { get; set; }
        public WorkerRates workerRates { get; set; }

    }
    public class WorkQueueType
    {
        public WorkQueueType()
        {
            typeName = "";
            count = 0;
            reviewType = ReviewType.None;
            workerRates = new WorkerRates();
        }
        public string typeName { get; set; }
        public Guid subId { get; set; }
        public int count { get; set; }
        public ReviewType reviewType { get; set; }
        public WorkerRates workerRates { get; set; }
        public string workerName { get; set; }
        public Guid workerId { get; set; }

    }

    public class WorkerRates
    {
        public WorkerRates()
        {
            worker = reviewer = 0m;
        }
        public decimal worker { get; set; }
        public decimal reviewer { get; set; }

    }

    public class WorkerPay
    {
        public WorkerPay()
        {
            value = 0m;
            name = "";
        }

        public DateTime date { get; set; }
        public string name { get; set; }
        public Guid docId { get; set; }
        public Guid touchId { get; set; }
        public decimal value { get; set; }


    }


    public class WorkerPaySummary
    {
        public WorkerPaySummary()
        {
            Credit = Paid = ApprovalRate = 0m;
            Tasks30Days = TasksAllTime = 0;
        }
        public decimal Credit { get; set; }
        public decimal Paid { get; set; }
        public int TasksAllTime { get; set; }
        public int Tasks30Days { get; set; }
        public decimal ApprovalRate { get; set; }


    }

    #endregion

    public class SmtpConfig
    {
        public string host { get; set; }
        public int port { get; set; }
        public string userName { get; set; }
        public string password { get; set; }

    }
    /// <summary>
    /// A model with the data format of the Inbound Parse API's POST
    /// </summary>
    public class SendGridEvent
    {
        public SendGridEvent()
        {
            email = sendgrid_event_id = smtp_id = sg_message_id = sendgrid_event = type = reason = status = url = useragent = ip = "";
            args = new SGArgs();
            category = new List<string>();
        }
        public string email { get; set; }
        public int timestamp { get; set; }
        public int uid { get; set; }
        public int id { get; set; }
        public string sendgrid_event_id { get; set; }
        [JsonProperty("smtp-id")] // switched to underscore for consistancy
        public string smtp_id { get; set; }
        public string sg_message_id { get; set; }
        [JsonProperty("event")] // event is protected keyword
        public string sendgrid_event { get; set; }
        public string type { get; set; }
        public IList<string> category { get; set; }
        public string reason { get; set; }
        public string status { get; set; }
        public string url { get; set; }
        public string useragent { get; set; }
        public string ip { get; set; }
        public SGArgs args { get; set; }
}
    public class SGArgs
    {
        public SGArgs()
        {
            sentFrom = "";
            entityType = EntityTypes.All;
        }
        public string sentFrom { get; set; }
        public Guid touchId { get; set; }
        public EntityTypes entityType { get; set; }
        public Guid entityId { get; set; }
    }

    public class EmailTracker
    {
        public EmailTracker()
        {
            clickthrus = new List<ClickThrough>();
        }
        public bool sent { get; set; }
        public bool delivered { get; set; }
        public bool opened { get; set; }
        public DateTime? deliverDate { get; set; }
        public DateTime? openDate { get; set; }
        public List<ClickThrough> clickthrus { get; set; }

    }

    public class ClickThrough
    {
        public string url { get; set; }
        public DateTime? clickDate { get; set; }

    }

    #region WorkerRole Jobs
    public class WorkerJob
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public int DelayStartMinutes { get; set; }
        public int IntervalMinutes { get; set; }
        public DateTime ModifiedDt { get; set; }
        public Timer JobTimer { get; set; }
        public TimerCallback JobTimerCallback { get; set; }
        public bool IsActive { get; set; }
        public bool NeedsReboot { get; set; }
    }

    #endregion
}
