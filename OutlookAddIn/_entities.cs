using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DA = System.ComponentModel.DataAnnotations.Schema;
//lost all change tracking Oct 7-10 


namespace ShuriOutlookAddIn
{

    #region Base Classes

    public abstract class Owned
    {
        public Owned()
        {
            OwnedByName = OwnedByGroupName = CreatedByName = ModifiedByName = sortname = "";
            Updatable = true;
            changeType = ChangeType.Update;
        }
        [Required]
        public Guid OwnedBy_Id { get; set; }
        public string OwnedByName { get; set; }
        public Guid OwnedByGroup_Id { get; set; }
        public string OwnedByGroupName { get; set; }
        public System.DateTime CreatedDt { get; set; }
        public string CreatedByName { get; set; }
        public Guid CreatedBy_Id { get; set; }
        public System.DateTime ModifiedDt { get; set; }
        public string ModifiedByName { get; set; }
        public Guid ModifiedBy_Id { get; set; }
        public bool Updatable { get; set; }
        public bool Deletable { get; set; }
        public Guid Collection_Id { get; set; }
        public bool CollectionChanged { get; set; }
        public ChangeType changeType { get; set; }
        public string sortname { get; set; }
    }

    /// <summary>
    /// Email address, Twitter handle, phone, etc.
    /// </summary>
    public class ContactPoint : Owned
    {
        public ContactPoint()
        {
            People = new List<Person>();
            Groups = new List<Group>();
            Description = Typename = "";
            changeType = ChangeType.None;
        }

        [Key, DatabaseGenerated(DA.DatabaseGeneratedOption.Identity)]
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        [MaxLength(512)]
        public string Description { get; set; }
        public Guid UserType_Id { get; set; }
        [MaxLength(140)]
        public string Typename { get; set; }
        public ContactPointPrimitive Primitive { get; set; }
        public string Primitivename { get; set; }

        public virtual List<Group> Groups { get; set; }
        public virtual List<Person> People { get; set; }

    }

    /// <summary>
    /// All Other Attributes & custom fields
    /// </summary>
    public class Document : Owned
    {
        public Document()
        {
            Groups = new List<Group>();
            People = new List<Person>();
            Tags = new List<Tag>();
            Touches = new List<Touch>();
            changeType = ChangeType.None;

        }

        [Key, DatabaseGenerated(DA.DatabaseGeneratedOption.Identity)]
        public System.Guid Id { get; set; }
        public Guid UserType_Id { get; set; }
        [MaxLength(140)]
        public string Typename { get; set; }
        public string UserTypeValue { get; set; }
        public DocumentPrimitive Primitive { get; set; }
        public string Primitivename { get; set; }
        [MaxLength(140)]
        public string Name { get; set; }
        [MaxLength(4000)]
        public string Value { get; set; }
        [MaxLength(140)]
        public string Url { get; set; }
        public bool ForPeople { get; set; }
        public bool ForTouches { get; set; }
        public long? SizeKB { get; set; }

        public virtual List<Group> Groups { get; set; }
        public virtual List<Person> People { get; set; }
        public virtual List<Tag> Tags { get; set; }
        public virtual List<Touch> Touches { get; set; }

    }

    /// <summary>
    /// Any grouping of the other entities.  Could be arbitary group or a specific Organization, company, subscription, work group, etc.
    ///     GroupsPeople relation will have StartDate and EndDate components so that a person's employment history may be maintained
    /// </summary>
    public class Group : Owned
    {
        public Group()
        {
            ContactPoints = new List<ContactPoint>();
            Documents = new List<Document>();
            Locations = new List<Location>();
            Groups = new List<Group>();
            People = new List<PersonTenured>();
            Tags = new List<Tag>();
            Touches = new List<Touch>();
            UserTypes = new List<UserType>();
            Name = Description = ImageUrl = ImageUrlThumb = GrpTypename = Nickname = "";
            DocValue = 0;
            changeType = ChangeType.None;
        }

        [Key, DatabaseGenerated(DA.DatabaseGeneratedOption.Identity)]
        public System.Guid Id { get; set; }

        [MaxLength(140)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string Nickname { get; set; }

        [MaxLength(4000)]
        public string Description { get; set; }
        [MaxLength(512)]
        public string ImageUrl { get; set; }
        public string ImageUrlThumb { get; set; }

        public System.Guid PrimaryCP_Id { get; set; }

        public GroupType GrpType { get; set; }
        [MaxLength(140)]
        public string GrpTypename { get; set; }

        public int DocValue { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsPrivateCollection { get; set; }

        public virtual List<ContactPoint> ContactPoints { get; set; }
        public virtual List<Document> Documents { get; set; }
        public virtual List<Group> Groups { get; set; }
        public virtual List<Location> Locations { get; set; }
        public virtual List<PersonTenured> People { get; set; }
        public virtual List<Tag> Tags { get; set; }
        public virtual List<Touch> Touches { get; set; }
        public virtual List<UserType> UserTypes { get; set; }

        public int GrpsCount { get; set; }
        public int OrgsCount { get; set; }
        public int PeopleCount { get; set; }
        public int TagsCount { get; set; }
        public int TouchesCount { get; set; }
    }

    /// <summary>
    /// Physical address:  has geo capability (i.e. "distance" from queries)
    /// </summary>
    public class Location : Owned
    {
        public Location()
        {
            People = new List<Person>();
            Groups = new List<Group>();
            Place_Id = Address = Postal = Country = State = City = Street = "";
            Latitude = Longitude = 0m;
            changeType = ChangeType.None;
        }

        [Key, DatabaseGenerated(DA.DatabaseGeneratedOption.Identity)]
        public System.Guid Id { get; set; }
        public Guid UserType_Id { get; set; }
        [MaxLength(1024)]
        public string Address { get; set; }
        [MaxLength(50)]
        public string Postal { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Place_Id { get; set; }

        [MaxLength(140)]
        public string Typename { get; set; }
        public LocationPrimitive Primitive { get; set; }
        public string Primitivename { get; set; }

        public virtual List<Group> Groups { get; set; }
        public virtual List<Person> People { get; set; }

    }

    /// <summary>
    /// People (possibly other resources) that might be involved in a Touch.  Also users of the system
    /// </summary>
    public class Person : Owned
    {
        public Person()
        {
            ContactPoints = new List<ContactPoint>();
            Documents = new List<Document>();
            Locations = new List<Location>();
            Groups = new List<GroupTenured>();
            Tags = new List<Tag>();
            Touches = new List<Touch>();
            Firstname = Middlename = Lastname = Prefix = Suffix = ImageUrl = ImageUrlThumb = Name = Description = "";
            DocValue = GrpsCount = OrgsCount = TagsCount = TouchesCount = 0;
            changeType = ChangeType.None;
        }

        [Key, DatabaseGenerated(DA.DatabaseGeneratedOption.Identity)]
        public System.Guid Id { get; set; }
        public Guid UserType_Id { get; set; }
        [MaxLength(140)]
        public string Typename { get; set; }
        public PersonPrimitive Primitive { get; set; }
        [MaxLength(140)]
        public string Firstname { get; set; }
        [MaxLength(140)]
        public string Middlename { get; set; }
        [MaxLength(140)]
        public string Lastname { get; set; }
        [MaxLength(50)]
        public string Nickname { get; set; }
        [MaxLength(50)]
        public string Prefix { get; set; }
        [MaxLength(50)]
        public string Suffix { get; set; }
        [MaxLength(512)]
        public string ImageUrl { get; set; }
        public string ImageUrlThumb { get; set; }
        [MaxLength(4000)]
        public string Description { get; set; }

        public System.Guid PrimaryCP_Id { get; set; }
        public System.Guid SecurityCP_Id { get; set; }

        public int DocValue { get; set; }
        public bool IsFavorite { get; set; }

        public int GrpsCount { get; set; }
        public int OrgsCount { get; set; }
        public int TagsCount { get; set; }
        public int TouchesCount { get; set; }

        [MaxLength(140)]
        public string Name { get; set; }

        public virtual List<ContactPoint> ContactPoints { get; set; }
        public virtual List<Document> Documents { get; set; }
        public virtual List<GroupTenured> Groups { get; set; }
        public virtual List<Location> Locations { get; set; }
        public virtual List<Tag> Tags { get; set; }
        public virtual List<Touch> Touches { get; set; }
    }

    /// <summary>
    /// Tag - anything that categorizes People, Orgs or Touches
    /// </summary>
    public class Tag : Owned
    {
        public Tag()
        {
            Groups = new List<Group>();
            People = new List<Person>();
            Touches = new List<Touch>();
            changeType = ChangeType.None;
            Name = Description = Typename = "";

        }

        [Key, DatabaseGenerated(DA.DatabaseGeneratedOption.Identity)]
        public System.Guid Id { get; set; }
        [MaxLength(140)]
        public string Name { get; set; }
        [MaxLength(4000)]
        public string Description { get; set; }
        public Guid UserType_Id { get; set; }
        [MaxLength(140)]
        public string Typename { get; set; }
        public TagPrimitive Primitive { get; set; }
        public string Primitivename { get; set; }
        public bool IsFavorite { get; set; }

        public virtual List<Group> Groups { get; set; }
        public virtual List<Person> People { get; set; }
        public virtual List<Touch> Touches { get; set; }

        public int GrpsCount { get; set; }
        public int OrgsCount { get; set; }
        public int PeopleCount { get; set; }
        public int TouchesCount { get; set; }
    }

    /// <summary>
    /// An activity or interaction between People
    ///     Touches have types such as Phone call, email, in-person, webinar, meeting, etc.
    /// </summary>
    public class Touch : Owned
    {
        public Touch()
        {
            Groups = new List<Group>();
            People = new List<Person>();
            ContactPoints = new List<ContactPoint>();
            Documents = new List<Document>();
            Locations = new List<Location>();
            Tags = new List<Tag>();
            Touches = new List<Touch>();
            resolveStrings = new List<string>();
            changeType = ChangeType.None;
            DateStart = DateTime.UtcNow;
            Name = Description = Typename = Primitivename = From = ReplyTo = "";
            OrgsCount = PeopleCount = TagsCount = GrpsCount = 0;
            IsScheduled = false;
        }


        [Key, DatabaseGenerated(DA.DatabaseGeneratedOption.Identity)]
        public System.Guid Id { get; set; }
        [MaxLength(140), Required]
        public string Name { get; set; }
        [MaxLength(4000)]
        public string Description { get; set; }
        public Guid UserType_Id { get; set; }
        [MaxLength(140)]
        public string Typename { get; set; }
        public TouchPrimitive Primitive { get; set; }
        public string Primitivename { get; set; }
        public System.DateTime DateStart { get; set; }
        public Nullable<System.DateTime> DateEnd { get; set; }
        public Nullable<System.DateTime> DateSchedule { get; set; }
        public Nullable<System.DateTime> DateSent { get; set; }
        public bool IsScheduled { get; set; }
        public Guid Location_Id { get; set; }
        public Guid DescriptDoc_Id { get; set; }

        public string From { get; set; }
        public string ReplyTo { get; set; }
        public int GrpsCount { get; set; }
        public int OrgsCount { get; set; }
        public int PeopleCount { get; set; }
        public int TagsCount { get; set; }

        public virtual List<ContactPoint> ContactPoints { get; set; }
        public virtual List<Document> Documents { get; set; }
        public virtual List<Group> Groups { get; set; }
        public virtual List<Location> Locations { get; set; }
        public virtual List<Person> People { get; set; }
        public virtual List<Tag> Tags { get; set; }
        public virtual List<Touch> Touches { get; set; }
        public virtual List<string> resolveStrings { get; set; }
    }

    public class UserType : Owned
    {
        public UserType()
        {
            ContactPoints = new List<ContactPoint>();
            Documents = new List<Document>();
            People = new List<Person>();
            Tags = new List<Tag>();
            Touches = new List<Touch>();
            Value = EntityName = PrimitiveName = CollectionName = CodeName = "";
        }

        [Key, DatabaseGenerated(DA.DatabaseGeneratedOption.Identity)]
        public System.Guid Id { get; set; }
        [MaxLength(140)]
        public string Name { get; set; }
        [MaxLength(4000)]
        public string Value { get; set; }
        public EntityTypes EntityType { get; set; }
        public int Primitive { get; set; }
        public bool ForPeople { get; set; }
        public bool ForOrgs { get; set; }
        public bool ForTouches { get; set; }
        public bool ForAllDBs { get; set; }

        [MaxLength(50)]
        public string CodeName { get; set; }

        public string CollectionName { get; set; }
        public string EntityName { get; set; }
        public string PrimitiveName { get; set; }

        public virtual List<ContactPoint> ContactPoints { get; set; }
        public virtual List<Document> Documents { get; set; }
        public virtual List<Person> People { get; set; }
        public virtual List<Tag> Tags { get; set; }
        public virtual List<Touch> Touches { get; set; }

    }

    #endregion

    #region Additional CLASSES 
    public class AppUser : Person
    {
        public AppUser()
        {
            IsSysAdmin = IsUser = IsDev = IsWorker = IsReviewer =  false;
            Subscriptions = new List<Subscription>();
            _subscriptionIds = new List<Guid>();
            usage = new List<Usage>();
            Teams = new List<Group>();
            APIAuthToken = "";
            ownedPeople = ownedTouches = ownedOrgs = licensedItems = 0;
            licenseLevel = LicenseLevel.None;
            licenseStatus = LicenseStatus.OK;
            UserFileStorageMB = 0;
            TimezoneOffset = 0;
            //licenseGraceDate = DateTime.MaxValue;
        }
        public System.Guid PrivateCollection_Id { get; set; }
        public System.Guid DefaultCollection_Id { get; set; }
        public Guid DefaultOwnedByGroup_Id { get; set; }
        public string Username { get; set; }
        public string UsernameProvider { get; set; }
        public decimal UserFileStorageMB { get; set; }
        public string EmailAddress { get; set; }
        public int TimezoneOffset { get; set; }

        public bool IsSysAdmin { get; set; }
        public bool IsDev { get; set; }
        public bool IsUser { get; set; }
        public bool IsWorker { get; set; }
        public bool IsReviewer { get; set; }

 
        public string APIAuthToken { get; set; }

        //licensing
        public int ownedPeople { get; set; }
        public int ownedTouches { get; set; }
        public int ownedOrgs { get; set; }

        public LicenseLevel licenseLevel { get; set; }
        public int licensedItems { get; set; }
        public LicenseStatus licenseStatus { get; set; }
        //public DateTime licenseGraceDate { get; set; }

        public virtual List<Guid> UpdatableSubscriptionIds
        {
            get
            {
                List<Guid> updbl = new List<Guid>();

                      try
                    {
                        foreach (Subscription sub in Subscriptions)
                        {
                            if (sub.UpdatableGroup || sub.Group_Id == Guid.Empty) updbl.Add(sub.Group_Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        //todo fix this
                        //DAL.HandleError("AppUser:SubscriptionIds", ex);
                        string x = ex.Message;
                    }
                    return updbl;

            }

        }

        private List<Guid> _subscriptionIds = new List<Guid>();
        public virtual List<Guid> SubscriptionIds
        {
            get
            {
                if (_subscriptionIds == null) _subscriptionIds = new List<Guid>();
                return _subscriptionIds;
            }
            set
            {
                _subscriptionIds = value;
            }
        }

        private List<Subscription> _subscriptions = new List<Subscription>();
        public virtual List<Subscription> Subscriptions
        {
            get
            {
                return _subscriptions;
            }
            set
            {
                _subscriptions = value;
            }
        }

        public List<Group> Teams { get; set; }
        public List<Usage> usage { get; set; }


    }

    public class GroupTenured : Group
    {
        public GroupTenured(Group grp)
        {
            if (grp != null)
            {
                this.ContactPoints = grp.ContactPoints;
                this.CreatedBy_Id = grp.CreatedBy_Id;
                this.CreatedByName = grp.CreatedByName;
                this.CreatedDt = grp.CreatedDt;
                this.Deletable = grp.Deletable;
                this.Description = grp.Description;
                this.Documents = grp.Documents;
                this.DocValue = grp.DocValue;
                this.Groups = grp.Groups;
                this.GrpType = grp.GrpType;
                this.GrpTypename = grp.GrpTypename;
                this.Id = grp.Id;
                this.ImageUrl = grp.ImageUrl;
                this.ImageUrlThumb = grp.ImageUrlThumb;
                this.IsFavorite = grp.IsFavorite;
                this.Locations = grp.Locations;
                this.ModifiedBy_Id = grp.ModifiedBy_Id;
                this.ModifiedByName = grp.ModifiedByName;
                this.ModifiedDt = grp.ModifiedDt;
                this.Name = grp.Name;
                this.Nickname = grp.Nickname;
                this.OwnedBy_Id = grp.OwnedBy_Id;
                this.OwnedByGroup_Id = grp.OwnedByGroup_Id;
                this.OwnedByGroupName = grp.OwnedByGroupName;
                this.OwnedByName = grp.OwnedByName;
                this.People = grp.People;
                this.PrimaryCP_Id = grp.PrimaryCP_Id;
                this.Tags = grp.Tags;
                this.Touches = grp.Touches;
                this.Updatable = grp.Updatable;
                this.UserTypes = grp.UserTypes;
                this.sortname = grp.sortname;

                this.Title = "";
                this.StartDt = DateTime.UtcNow;

            }
        }
        public System.Guid TenuredId { get; set; }
        public string Title { get; set; }
        public DateTime StartDt { get; set; }
        public DateTime? EndDt { get; set; }

    }

    public class PersonTenured : Person
    {
        public PersonTenured(Person per)
        {
            if (per != null)
            {
                this.CollectionChanged = per.CollectionChanged;
                this.Collection_Id = per.Collection_Id;
                this.ContactPoints = per.ContactPoints;
                this.CreatedBy_Id = per.CreatedBy_Id;
                this.CreatedByName = per.CreatedByName;
                this.CreatedDt = per.CreatedDt;
                this.Deletable = per.Deletable;
                this.Description = per.Description;
                this.Documents = per.Documents;
                this.DocValue = per.DocValue;
                this.Firstname = per.Firstname;
                this.Groups = per.Groups;
                this.Id = per.Id;
                this.ImageUrl = per.ImageUrl;
                this.ImageUrlThumb = per.ImageUrlThumb;
                this.IsFavorite = per.IsFavorite;
                this.Lastname = per.Lastname;
                this.Locations = per.Locations;
                this.Middlename = per.Middlename;
                this.ModifiedBy_Id = per.ModifiedBy_Id;
                this.ModifiedByName = per.ModifiedByName;
                this.ModifiedDt = per.ModifiedDt;
                this.Name = per.Name;
                this.Nickname = per.Nickname;
                this.OwnedBy_Id = per.OwnedBy_Id;
                this.OwnedByGroup_Id = per.OwnedByGroup_Id;
                this.OwnedByGroupName = per.OwnedByGroupName;
                this.OwnedByName = per.OwnedByName;
                this.Prefix = per.Prefix;
                this.PrimaryCP_Id = per.PrimaryCP_Id;
                this.Primitive = per.Primitive;
                this.SecurityCP_Id = per.SecurityCP_Id;
                this.Suffix = per.Suffix;
                this.Tags = per.Tags;
                this.Touches = per.Touches;
                this.Typename = per.Typename;
                this.Updatable = per.Updatable;
                this.UserType_Id = per.UserType_Id;
                this.sortname = per.sortname;
                this.changeType = per.changeType;
                this.OrgsCount = per.OrgsCount;
                this.GrpsCount = per.GrpsCount;
                this.TagsCount = per.TagsCount;
                this.TouchesCount = per.TouchesCount;

                this.Title = "";
                this.StartDt = DateTime.UtcNow;
            }
        }
        public System.Guid TenuredId { get; set; }
        public string Title { get; set; }
        public DateTime StartDt { get; set; }
        public DateTime? EndDt { get; set; }
    }

    /// <summary>
    /// Offerings for GrpType = Subscription
    /// </summary>
    public class Subscription : Owned
    {
        public Subscription()
        {
            Name = Description = PaymentTypename = SubscriptionTypename = AvailableToGroupname = productId = "";
            Value = 0;
            CountSubscribers = 0;
            PayType = PaymentType.Comp;
            EndDt = DateTime.MaxValue;
            ApprovalStatus = SubscriptionApprovalStatus.Pending;
            Subscribers = new List<Subscriber>();
            licenseLevel = LicenseLevel.None;
        }
        [Key]
        public System.Guid Id { get; set; }
        public System.Guid Group_Id { get; set; }
        public bool IsPrivateCollection { get; set; }

        [MaxLength(140)]
        public string Name { get; set; }
        public string Description { get; set; }
        public Single Value { get; set; }
        public SubscriptionType SubType { get; set; }
        public string SubscriptionTypename { get; set; }

        public SubscriptionApprovalStatus ApprovalStatus { get; set; }
        public string ApprovalStatusname { get; set; }

        public PaymentType PayType { get; set; }
        public string PaymentTypename { get; set; }
        public DateTime StartDt { get; set; }
        public DateTime? EndDt { get; set; }
        public bool Active { get; set; }
        public bool IsSubscribed { get; set; }
        public bool UpdatableGroup { get; set; }
        public int CountSubscribers { get; set; }

        public Guid AvailableToGroup_Id { get; set; }
        public string AvailableToGroupname { get; set; }

        public LicenseLevel licenseLevel { get; set; }
        public string productId { get; set; }
        public string familyId { get; set; }
        public List<Subscriber> Subscribers { get; set; }

    }

    public class Subscriber
    {
        public Subscriber()
        {
            PaymentTypename = Name = "";
            paymentType = PaymentType.Comp;
            EndDt = DateTime.MaxValue;
            receipt = signature = transactionId = userStatus = "";
            Value = 0;
        }
        public System.Guid Subscription_Id { get; set; }
        public System.Guid Person_Id { get; set; }
        public string Name { get; set; }
        public string receipt { get; set; }
        public string signature { get; set; }
        public string transactionId { get; set; }
        public string productId { get; set; }
        public string userStatus { get; set; }
        public PaymentType paymentType { get; set; }
        public string PaymentTypename { get; set; }
        public DateTime StartDt { get; set; }
        public DateTime EndDt { get; set; }
        public bool Active { get; set; }
        public Single Value { get; set; }

    }

    public class DocumentEntity : Document
    {
        public System.Guid Entity_Id { get; set; }
        public EntityTypes EntityType { get; set; }
        public string EntityName { get; set; }
        public System.Guid Organization_Id { get; set; }
        public string OrganizationName { get; set; }
        public string ImageUrl { get; set; }
        public string ImageUrlThumb { get; set; }
        public bool IsDefault { get; set; }

    }
    #endregion

    public class GroupsQueryResult
    {
        public GroupsQueryResult()
        {
            items = new List<Group>();
            totalCount = 0;
        }

        public List<Group> items { get; set; }
        public int totalCount { get; set; }
    }

    public class PeopleQueryResult
    {
        public PeopleQueryResult()
        {
            items = new List<PersonTenured>();
            totalCount = 0;
        }

        public List<PersonTenured> items { get; set; }
        public int totalCount { get; set; }
    }
    public class TouchesQueryResult
    {
        public TouchesQueryResult()
        {
            items = new List<Touch>();
            totalCount = 0;
        }

        public List<Touch> items { get; set; }
        public int totalCount { get; set; }
    }
}
