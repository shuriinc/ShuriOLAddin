using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuriOutlookAddIn
{
    public enum AppRole : int
    {
        None = 0,
        User = 1,
        Developer = 2,
        SystemAdmin = 3
    }

    public enum AuditType : int
    {
        SQL,
        System,
        Services,
        API,
        Error,
        Delete
    }

    public enum ChangeType : int
    {
        None = 0,
        Update = 1,
        Remove = 2
    }

    public enum ContactPointPrimitive : int
    {
        Unknown = 0,
        Email = 1,
        Phone = 2,
        Url = 3,
        SMHandle = 4
    }

    public enum DocumentPrimitive : int
    {
        None = 0,
        File = 1,
        CustomText = 2,
        CustomLongText = 3,
        CustomInteger = 4,
        CustomFloat = 5,
        CustomBinary = 6,
        CustomDate = 7,
        RatingYesNo = 8,
        RatingYesNoMaybe = 9,
        Rating0to5 = 10,
        Rating0to100 = 11,
        Currency = 12,
        Object = 13,
        Credentials = 14,
        UniqueIdentifier = 15

    }

    public enum EntityTypes : int
    {
        All = -1,
        ContactPoint = 0,
        Document = 1,
        Group = 2,
        Location = 3,
        Person = 4,
        Tag = 5,
        Touch = 6,
        Ref = 7,
        UserType = 8,
        Organization = 9,
        Team = 10,
        Private = 11,
        Subscription = 12,
        PersonTeam = 13,
        User = 14,
        Subscriber = 15
    }

    public enum GroupType : int
    {
        Private = 0,
        Collection = 1,
        Team = 2,
        Organization = 3
    }

    public enum LicenseLevel : int
    {
        None = 0,
        Free = 1,
        Professional = 2,
        Enterprise = 3
    }
    public enum LicenseStatus : int
    {
        OK = 0,
        Grace = 1,
        ShutDown = 2
    }


    public enum LocationPrimitive : int
    {
        Business = 0,
        Residential = 1
    }

    public enum PaymentType : int
    {
        Comp = 0,
        InApp = 1,
        Other = 2,
        Stripe = 3
    }

    public enum PersonPrimitive : int
    {
        Person = 0,
        Resource = 1
    }

    public enum QueryOperator : int
    {
        Equals = 0,
        IsTrue = 1,
        IsFalse = 2,
        Begins = 3,
        Contains = 4,
        Between = 5,
        IsGreaterThan = 6,
        IsGreaterOrEqual = 7,
        IsLessThan = 8,
        IsLessOrEqual = 9

    }

    public enum RecordType : int
    {
        Minimum = 0,
        Full = 1,
        Pivot = 2
    }

    public enum ReviewType : int
    {
        None = 0,
        Regular = 1,
        Expert = 2
    }

    public enum Sh_StorageLocation : int
    {
        User = 0,
        UserCdn = 1,
        Imageurl = 2,
        Images = 3,
        Temporary = 4
    }
    public enum SubscriptionApprovalStatus : int
    {
        NotRequired = 0,
        Approved = 1,
        Pending = 2,
        Disapproved = 3
    }

    public enum SubscriptionType : int
    {
        Demo = 0,
        Private = 1,
        Monthly = 2,
        Annual = 3
    }

    public enum TagPrimitive : int
    {
        Tag = 0,
        Process = 1
    }

    public enum TouchPrimitive : int
    {
        Meeting = 0,
        TimedMeeting = 1,
        Email = 2,
        TrackedEmail = 3,
        //SocialMedia = 4,
        //Event = 5,
        MediaCapture = 6,
        Update = 7,
        Payment = 8
    }

    public enum UserRole
    {
        Registered = 0, User = 1, Worker = 2, Reviewer = 3, Dev = 4, SysAdmin = 5
    }
    public enum WatchType
    {
        None = 0, TwitterId = 1, TwitterHashtag = 2
    }

    public enum WorkerProcessStatus : int
    {
        ReadyWork = 0,
        InWork = 1,
        ReadyReview = 2,
        InReview = 3,
        Rejected = 4,
        Approved = 5,
        Paid = 6,
        ReadyExpert = 7,
        InExpert = 8,
        RejectedReview = 9
    }

 }
