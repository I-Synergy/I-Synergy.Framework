namespace ISynergy.Library
{
    public enum SoftwareEnvironments
    {
        //Environment to production backend
        Production = 0,
        //Environment to testing backend
        Test = -1
    }

    public enum DataActions
    {
        Save = 0,
        Delete = 1
    }

    public enum DataTypes
    {
        StringData = 0,
        IntegerData = 1,
        DecimalData = 2,
        BooleanData = 3,
        BinaryData = 4
    }

    public enum ScreenModes
    {
        Basic = 0,
        Extended = 1
    }

    public enum ImageFormats
    {
        PNG = 0,
        JPG = 1,
        GIF = 2,
    }

    public enum DocumentFormats
    {
        TXT = 0,
        PDF = 1,
        DOCX = 2,
        XLSX = 3,
    }

    public enum TileModes
    {
        Standaard = 1,
        Wide = 2,
        Small = 3,
        Medium = 4,
        Bar = 5,
        WideStandaard = 6,
        StandaardSmall = 7,
        Seperator = 8,
        Custom = 9
    }

    public enum TimerModes
    {
        NA = 0,
        Date = 1,
        Time = 2,
        DateTime = 3
    }

    public enum NotificationTypes
    {
        Success = 0,
        Information = 1,
        Warning = 2,
        Error = 3
    }

    public enum MessageBoxResult
    {
        None = 0,
        OK = 1,
        Cancel = 2,
        Yes = 6,
        No = 7
    }

    public enum MessageBoxButton
    {
        OK = 0,
        OKCancel = 1,
        YesNoCancel = 3,
        YesNo = 4
    }

    public enum MessageBoxImage
    {
        None = 0,
        Hand = 16,
        Stop = 16,
        Error = 16,
        Question = 32,
        Exclamation = 48,
        Warning = 48,
        Asterisk = 64,
        Information = 64
    }

    public enum GrantTypes
    {
        PasswordGrantType,
        RefreshTokenAndAuthorizationCodeGrantType,
        ClientCredentialsGrantType,
    }
}