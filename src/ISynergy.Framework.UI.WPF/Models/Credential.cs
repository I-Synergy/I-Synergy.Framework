using ISynergy.Framework.UI.Enumerations;

namespace ISynergy.Framework.UI.Models
{
    public class Credential
    {
        private readonly string _resource;
        private readonly string _username;
        private readonly string _password;
        private readonly CredentialTypes _credentialType;

        public CredentialTypes CredentialType
        {
            get { return _credentialType; }
        }

        public string Resource
        {
            get { return _resource; }
        }

        public string Username
        {
            get { return _username; }
        }

        public string Password
        {
            get { return _password; }
        }

        public Credential(CredentialTypes credentialType, string resource, string username, string password)
        {
            _resource = resource;
            _username = username;
            _password = password;
            _credentialType = credentialType;
        }

        public override string ToString() =>
            $"CredentialType: {CredentialType}, Resource: {Resource}, Username: {Username}, Password: {Password}";
    }
}
