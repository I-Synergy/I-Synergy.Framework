namespace ISynergy
{
    public static class ControllerNames
    {
        public const string Templates = "templates";
        public const string MasterData = "masterdata";
        public const string Settings = "settings";
        public const string Reports = "reports";

        public const string Relations = "relations";
        public const string RelationMainGroups = "relationmaingroups";
        public const string RelationGroups = "relationgroups";

        public const string Commodities = "commodities";
        public const string Products = "products";
        public const string CommodityMainGroups = "commoditymaingroups";
        public const string CommodityGroups = "commoditygroups";

        public const string Invoices = "invoices";
        public const string Orders = "orders";
        public const string Quotes = "quotes";
        public const string Sales = "sales";
        public const string Payment = "payment";
    }

    public static class ControllerPaths
    {
        public const string Accounts = "accounts";
        public const string Roles = "roles";
        public const string Licenses = "licenses";
        public const string Users = "users";

        public const string Check = "check";
        public const string Email = "email";
        public const string License = "license";
        public const string ForgotPassword = "forgotpassword";
        public const string RegisterExternal = "registerexternal";

        public const string Id = "{id}";
        public const string Number = "number";
        public const string Key = "{key}";
        public const string Code = "code/{code}";
        public const string FileFromId = "{id}/file";

        public const string List = "list";
        public const string Year = "year";
        public const string DtoList = "dto/list";
        public const string ListWithGroupId = "list/group/{group_id}";
        public const string DtoListWithGroupId = "dto/list/group/{group_id}";
        public const string DtoListWithType = "dto/list/type/{type}";
        public const string DtoListWithGroupAndType = "dto/list/group/{group_id}/type/{type}";
        public const string ListWithFull = "list/full";

        public const string ParentalIdWithList = "/{id}/list";
        public const string ParentalIdWithUnitId = "/{id}/unit/{unit_id}";
        public const string UnitWithCommodityId = "/{code}/{id}";
        public const string ParentalList = "/list";
        public const string ParentalListFull = "/list/full";
        public const string ParentalId = "/{id}";
        public const string ParentalCustomerId = "/{customer_id}";
        public const string ParentalCustomerSubscriptionId = "/{customer_id}/{subscription_id}";

        public const string ListWithId = "/list/id/{id}";
        public const string ListAllWithId = "/list/id/{id}/all";
        public const string ListWithRelationId = "/list/relation/{relation_id}";

        public const string ParentalIdWithProductAndUnitId = "/{id}/product/{commodity_id}/{unit_id}";

        public const string ListWithUserIdFilter = "list/user/{user_id}";
        public const string ListWithRelationIdFilter = "list/relation/{relation_id}";
        public const string ListWithType = "list/type/{type}";

        public const string CountWithType = "/count/{type}";
        public const string SumWithType = "/sum/{type}";

        public const string ListNonBilled = "list/non-billed";
        public const string ListNonOrdered = "list/non-ordered";

        public const string IdWithGroups = "{id}/groups";
        public const string IdWithReport = "{id}/report";

        public const string DtoSearch = "dto/search";
        public const string Search = "search";
        public const string File = "file";
        public const string Type = "type";
        public const string Relation = "relation";
        public const string Product = "product";
        public const string Report = "report";
        public const string Groups = "groups";

        public const string Payments = "payments";
        public const string Customers = "customers";
        public const string Mandates = "mandates";
        public const string Refunds = "refunds";
        public const string Subscriptions = "subscriptions";


        public const string CheckNumber = "check/number/{number}";
        public const string CheckCode = "check/code/{code}";

        public const string NewNumber = "new/number";
        public const string NewCode = "new/code";

        public const string New = "/new";
        public const string Download = "/download";

        public const string Sales = "sales";
        public const string Revenue = "revenue";
        public const string NetProfit = "netprofit";
        public const string Expenses = "expenses";
        public const string PredictedRevenue = "predictedrevenue";
        public const string AccountsReceivable = "accountsreceivable";
        public const string Age = "age";
        public const string Online = "online";
        public const string TimeZones = "timezones";
    }
}
