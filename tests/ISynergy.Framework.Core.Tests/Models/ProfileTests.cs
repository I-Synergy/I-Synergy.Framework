using ISynergy.Framework.Core.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Models;

[TestClass]
public class ProfileTests
{
    #region IOrganization — direct Name implementation

    [TestMethod]
    public void Organization_WithDirectName_ReturnsName()
    {
        // Arrange
        var org = new TestOrganization
        {
            OrganizationId = Guid.Parse("{79C13C79-B50B-4BEF-B796-294DED5676BB}"),
            Name = "Demo Organization"
        };

        // Act & Assert
        Assert.AreEqual("Demo Organization", org.Name);
        Assert.AreNotEqual(Guid.Empty, org.OrganizationId);
    }

    [TestMethod]
    public void Organization_AsInterface_ExposesName()
    {
        // Arrange
        IOrganization org = new TestOrganization
        {
            OrganizationId = Guid.NewGuid(),
            Name = "My Org"
        };

        // Act & Assert
        Assert.AreEqual("My Org", org.Name);
    }

    #endregion

    #region IOrganization — explicit interface (alias) implementation

    [TestMethod]
    public void OrganizationWithAlias_AliasMapsToName()
    {
        // Arrange
        IOrganization org = new TestOrganizationWithAlias
        {
            OrganizationId = Guid.NewGuid(),
            Alias = "keycloak-alias"
        };

        // Act & Assert
        Assert.AreEqual("keycloak-alias", org.Name);
    }

    [TestMethod]
    public void OrganizationWithAlias_SettingNameUpdatesAlias()
    {
        // Arrange
        var org = new TestOrganizationWithAlias
        {
            OrganizationId = Guid.NewGuid(),
            Alias = "original"
        };

        // Act
        ((IOrganization)org).Name = "updated";

        // Assert
        Assert.AreEqual("updated", org.Alias);
    }

    #endregion

    #region IProfile — TenantId for personal accounts (no organizations)

    [TestMethod]
    public void Profile_WithNoOrganizations_TenantIdDefaultsToUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profile = new TestProfile
        {
            UserId = userId,
            TenantId = userId, // personal account
            Token = new Token()
        };

        // Act & Assert
        Assert.AreEqual(userId, profile.TenantId);
        Assert.AreEqual(userId, profile.UserId);
        Assert.IsEmpty(((IProfile)profile).Organizations);
    }

    [TestMethod]
    public void Profile_WithNoOrganizations_TenantIdCanDifferFromUserId()
    {
        // This is allowed — e.g. an admin acting on behalf of another tenant
        var userId = Guid.NewGuid();
        var adminTenantId = Guid.NewGuid();

        var profile = new TestProfile
        {
            UserId = userId,
            TenantId = adminTenantId,
            Token = new Token()
        };

        Assert.AreEqual(adminTenantId, profile.TenantId);
        Assert.AreNotEqual(profile.UserId, profile.TenantId);
    }

    #endregion

    #region IProfile — TenantId with organizations

    [TestMethod]
    public void Profile_WithSingleOrganization_TenantIdSetToOrgId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var orgId = Guid.NewGuid();
        var profile = new TestProfile
        {
            UserId = userId,
            TenantId = orgId,
            Token = new Token(),
            Organizations =
            [
                new TestOrganization { OrganizationId = orgId, Name = "My Org" }
            ]
        };

        // Act & Assert
        Assert.AreEqual(orgId, profile.TenantId);
        Assert.AreNotEqual(userId, profile.TenantId);
    }

    [TestMethod]
    public void Profile_WithMultipleOrganizations_TenantIdReflectsSelectedOrg()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var firstOrgId = Guid.NewGuid();
        var secondOrgId = Guid.NewGuid();

        var profile = new TestProfile
        {
            UserId = userId,
            TenantId = firstOrgId,
            Token = new Token(),
            Organizations =
            [
                new TestOrganization { OrganizationId = firstOrgId, Name = "First Org" },
                new TestOrganization { OrganizationId = secondOrgId, Name = "Second Org" }
            ]
        };

        // Assert — initial state: first org selected
        Assert.AreEqual(firstOrgId, profile.TenantId);

        // Act — switch organization
        profile.TenantId = secondOrgId;

        // Assert — tenant now points to second org
        Assert.AreEqual(secondOrgId, profile.TenantId);
    }

    [TestMethod]
    public void Profile_OrganizationsExposedAsReadOnlyViaInterface()
    {
        // Arrange
        var profile = new TestProfile
        {
            UserId = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            Token = new Token(),
            Organizations =
            [
                new TestOrganization { OrganizationId = Guid.NewGuid(), Name = "Org A" },
                new TestOrganization { OrganizationId = Guid.NewGuid(), Name = "Org B" }
            ]
        };

        // Act
        var orgs = ((IProfile)profile).Organizations;

        // Assert
        Assert.AreEqual(2, orgs.Count);
        Assert.AreEqual("Org A", orgs[0].Name);
        Assert.AreEqual("Org B", orgs[1].Name);
    }

    #endregion

    #region IProfile — full construction

    [TestMethod]
    public void Profile_FullConstruction_AllPropertiesSet()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        var token = new Token
        {
            AccessToken = "access-token",
            IdToken = "id-token",
            RefreshToken = "refresh-token",
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        // Act
        var profile = new TestProfile
        {
            UserId = userId,
            TenantId = tenantId,
            Name = "John Doe",
            TimeZoneId = "Europe/Amsterdam",
            CountryCode = "NL",
            CultureCode = "nl-NL",
            Email = "john@example.com",
            Roles = ["Administrator", "User"],
            Features = ["Dashboard", "Settings"],
            Expiration = now.AddHours(24),
            Token = token,
            Organizations =
            [
                new TestOrganization { OrganizationId = tenantId, Name = "Acme Corp" }
            ]
        };

        // Assert
        Assert.AreEqual(userId, profile.UserId);
        Assert.AreEqual(tenantId, profile.TenantId);
        Assert.AreEqual("John Doe", profile.Name);
        Assert.AreEqual("Europe/Amsterdam", profile.TimeZoneId);
        Assert.AreEqual("NL", profile.CountryCode);
        Assert.AreEqual("nl-NL", profile.CultureCode);
        Assert.AreEqual("john@example.com", profile.Email);
        Assert.AreEqual(2, profile.Roles.Count);
        Assert.AreEqual("Administrator", profile.Roles[0]);
        Assert.AreEqual(2, profile.Features.Count);
        Assert.AreEqual(now.AddHours(24), profile.Expiration);
        Assert.AreEqual("access-token", profile.Token.AccessToken);
        Assert.AreEqual("id-token", profile.Token.IdToken);
        Assert.AreEqual("refresh-token", profile.Token.RefreshToken);
        Assert.AreEqual(3600, profile.Token.ExpiresIn);
        Assert.AreEqual("Bearer", profile.Token.TokenType);

        var orgs = ((IProfile)profile).Organizations;
        Assert.AreEqual(1, orgs.Count);
        Assert.AreEqual(tenantId, orgs[0].OrganizationId);
        Assert.AreEqual("Acme Corp", orgs[0].Name);
    }

    #endregion

    #region TenantId derivation logic

    [TestMethod]
    public void TenantDerivation_NoOrgNoTenantClaim_FallsBackToUserId()
    {
        // Simulates: JWT has sub (UserId) but no tenant_id and no organization claim
        var userId = Guid.NewGuid();

        Guid? tenantClaim = null;
        var orgs = Array.Empty<IOrganization>();

        var effectiveTenantId = tenantClaim is { } tc && tc != Guid.Empty
            ? tc
            : orgs.Length > 0
                ? orgs[0].OrganizationId
                : userId;

        Assert.AreEqual(userId, effectiveTenantId);
    }

    [TestMethod]
    public void TenantDerivation_HasTenantClaim_UsesClaim()
    {
        // Simulates: JWT has tenant_id claim
        var userId = Guid.NewGuid();
        var tenantClaim = Guid.NewGuid();
        var orgs = Array.Empty<IOrganization>();

        var effectiveTenantId = tenantClaim != Guid.Empty
            ? tenantClaim
            : orgs.Length > 0
                ? orgs[0].OrganizationId
                : userId;

        Assert.AreEqual(tenantClaim, effectiveTenantId);
    }

    [TestMethod]
    public void TenantDerivation_NoClaimHasOrg_UsesFirstOrg()
    {
        // Simulates: JWT has no tenant_id, but has organization claim
        var userId = Guid.NewGuid();
        var firstOrgId = Guid.NewGuid();

        Guid tenantClaim = Guid.Empty;
        var orgs = new IOrganization[]
        {
            new TestOrganization { OrganizationId = firstOrgId, Name = "First Org" }
        };

        var effectiveTenantId = tenantClaim != Guid.Empty
            ? tenantClaim
            : orgs.Length > 0
                ? orgs[0].OrganizationId
                : userId;

        Assert.AreEqual(firstOrgId, effectiveTenantId);
    }

    #endregion

    #region TenantId — switching between personal and org contexts

    [TestMethod]
    public void Profile_SwitchFromOrgBackToPersonal_UpdatesTenantId()
    {
        // Arrange: user starts in an org context
        var userId = Guid.NewGuid();
        var orgId = Guid.NewGuid();
        var profile = new TestProfile
        {
            UserId = userId,
            TenantId = orgId,
            Token = new Token(),
            Organizations =
            [
                new TestOrganization { OrganizationId = orgId, Name = "My Org" }
            ]
        };

        Assert.AreEqual(orgId, profile.TenantId);

        // Act: user switches to personal context (no org selected)
        profile.TenantId = userId;

        // Assert
        Assert.AreEqual(userId, profile.TenantId);
        Assert.AreEqual(1, ((IProfile)profile).Organizations.Count); // orgs still present, just not selected
    }

    [TestMethod]
    public void Profile_SwitchFromPersonalToOrg_UpdatesTenantId()
    {
        // Arrange: personal account
        var userId = Guid.NewGuid();
        var orgId = Guid.NewGuid();
        var profile = new TestProfile
        {
            UserId = userId,
            TenantId = userId,
            Token = new Token(),
            Organizations =
            [
                new TestOrganization { OrganizationId = orgId, Name = "My Org" }
            ]
        };

        Assert.AreEqual(userId, profile.TenantId);

        // Act: user selects an organization
        profile.TenantId = orgId;

        // Assert
        Assert.AreEqual(orgId, profile.TenantId);
    }

    #endregion

    #region Edge cases — default/empty states

    [TestMethod]
    public void Profile_DefaultConstruction_HasEmptyCollections()
    {
        // Arrange & Act
        var profile = new TestProfile { Token = new Token() };

        // Assert
        Assert.AreEqual(Guid.Empty, profile.UserId);
        Assert.AreEqual(Guid.Empty, profile.TenantId);
        Assert.IsEmpty(profile.Roles);
        Assert.IsEmpty(profile.Features);
        Assert.IsEmpty(((IProfile)profile).Organizations);
    }

    [TestMethod]
    public void Profile_ExpiredToken_ExpirationIsInPast()
    {
        // Arrange
        var expired = DateTimeOffset.UtcNow.AddHours(-1);
        var profile = new TestProfile
        {
            UserId = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            Token = new Token(),
            Expiration = expired
        };

        // Assert
        Assert.IsTrue(profile.Expiration < DateTimeOffset.UtcNow);
    }

    [TestMethod]
    public void Organization_WithEmptyGuid_IsAllowed()
    {
        // The interface doesn't enforce non-empty — validation is at a higher layer
        var org = new TestOrganization
        {
            OrganizationId = Guid.Empty,
            Name = "Invalid Org"
        };

        Assert.AreEqual(Guid.Empty, org.OrganizationId);
        Assert.AreEqual("Invalid Org", org.Name);
    }

    #endregion

    #region Tenant derivation — edge cases

    [TestMethod]
    public void TenantDerivation_EmptyGuidClaim_FallsThroughToOrg()
    {
        // Simulates: JWT tenant_id is Guid.Empty (treated as absent)
        var userId = Guid.NewGuid();
        var firstOrgId = Guid.NewGuid();

        Guid tenantClaim = Guid.Empty;
        var orgs = new IOrganization[]
        {
            new TestOrganization { OrganizationId = firstOrgId, Name = "Only Org" }
        };

        var effectiveTenantId = tenantClaim != Guid.Empty
            ? tenantClaim
            : orgs.Length > 0
                ? orgs[0].OrganizationId
                : userId;

        Assert.AreEqual(firstOrgId, effectiveTenantId);
    }

    [TestMethod]
    public void TenantDerivation_MultipleOrgs_AlwaysUsesFirstWhenNoClaim()
    {
        // First org is always the default when no tenant_id claim
        var userId = Guid.NewGuid();
        var firstOrgId = Guid.NewGuid();
        var secondOrgId = Guid.NewGuid();

        Guid tenantClaim = Guid.Empty;
        var orgs = new IOrganization[]
        {
            new TestOrganization { OrganizationId = firstOrgId, Name = "Alpha" },
            new TestOrganization { OrganizationId = secondOrgId, Name = "Beta" }
        };

        var effectiveTenantId = tenantClaim != Guid.Empty
            ? tenantClaim
            : orgs.Length > 0
                ? orgs[0].OrganizationId
                : userId;

        Assert.AreEqual(firstOrgId, effectiveTenantId);
    }

    #endregion

    #region SwitchOrganization — validation logic

    // The core validation logic shared by all IAuthenticationService implementations:
    // 1. If organizationId equals UserId → allow (personal context)
    // 2. If organizationId is in Organizations → allow (org context)
    // 3. Otherwise → reject (no-op)

    [TestMethod]
    public void SwitchOrganization_ValidOrg_ShouldSwitch()
    {
        var userId = Guid.NewGuid();
        var orgId = Guid.NewGuid();
        var profile = CreateProfile(userId, userId, (orgId, "My Org"));

        var result = TrySwitchOrganization(profile, orgId);

        Assert.IsTrue(result);
        Assert.AreEqual(orgId, profile.TenantId);
    }

    [TestMethod]
    public void SwitchOrganization_ToPersonalContext_ShouldSwitch()
    {
        var userId = Guid.NewGuid();
        var orgId = Guid.NewGuid();
        var profile = CreateProfile(userId, orgId, (orgId, "My Org"));

        var result = TrySwitchOrganization(profile, userId);

        Assert.IsTrue(result);
        Assert.AreEqual(userId, profile.TenantId);
    }

    [TestMethod]
    public void SwitchOrganization_UnknownOrg_ShouldNotSwitch()
    {
        var userId = Guid.NewGuid();
        var knownOrgId = Guid.NewGuid();
        var unknownOrgId = Guid.NewGuid();
        var profile = CreateProfile(userId, knownOrgId, (knownOrgId, "My Org"));

        var result = TrySwitchOrganization(profile, unknownOrgId);

        Assert.IsFalse(result);
        Assert.AreEqual(knownOrgId, profile.TenantId); // unchanged
    }

    [TestMethod]
    public void SwitchOrganization_NoOrganizations_OnlyPersonalContextAllowed()
    {
        var userId = Guid.NewGuid();
        var profile = CreateProfile(userId, userId);

        // Switching to personal context is a no-op but should succeed
        Assert.IsTrue(TrySwitchOrganization(profile, userId));

        // Switching to an unknown org should fail
        Assert.IsFalse(TrySwitchOrganization(profile, Guid.NewGuid()));
    }

    [TestMethod]
    public void SwitchOrganization_WithMultipleOrgs_CanSwitchBetweenAll()
    {
        var userId = Guid.NewGuid();
        var orgA = Guid.NewGuid();
        var orgB = Guid.NewGuid();
        var orgC = Guid.NewGuid();
        var profile = CreateProfile(userId, orgA,
            (orgA, "Alpha"), (orgB, "Beta"), (orgC, "Gamma"));

        // Switch A → B
        Assert.IsTrue(TrySwitchOrganization(profile, orgB));
        Assert.AreEqual(orgB, profile.TenantId);

        // Switch B → C
        Assert.IsTrue(TrySwitchOrganization(profile, orgC));
        Assert.AreEqual(orgC, profile.TenantId);

        // Switch C → personal
        Assert.IsTrue(TrySwitchOrganization(profile, userId));
        Assert.AreEqual(userId, profile.TenantId);

        // Switch personal → A
        Assert.IsTrue(TrySwitchOrganization(profile, orgA));
        Assert.AreEqual(orgA, profile.TenantId);
    }

    [TestMethod]
    public void SwitchOrganization_SameOrgTwice_IsIdempotent()
    {
        var userId = Guid.NewGuid();
        var orgId = Guid.NewGuid();
        var profile = CreateProfile(userId, orgId, (orgId, "My Org"));

        Assert.IsTrue(TrySwitchOrganization(profile, orgId));
        Assert.AreEqual(orgId, profile.TenantId);

        // Switching to the same org again succeeds (idempotent)
        Assert.IsTrue(TrySwitchOrganization(profile, orgId));
        Assert.AreEqual(orgId, profile.TenantId);
    }

    #endregion

    #region Helpers

    private static TestProfile CreateProfile(Guid userId, Guid tenantId, params (Guid OrgId, string Name)[] orgs)
    {
        return new TestProfile
        {
            UserId = userId,
            TenantId = tenantId,
            Token = new Token(),
            Organizations = orgs
                .Select(o => new TestOrganization { OrganizationId = o.OrgId, Name = o.Name })
                .ToList<IOrganization>()
        };
    }

    private static bool TrySwitchOrganization(IProfile profile, Guid organizationId)
    {
        var isPersonalContext = organizationId == profile.UserId;
        var isKnownOrg = profile.Organizations.Any(o => o.OrganizationId == organizationId);

        if (!isPersonalContext && !isKnownOrg)
            return false;

        profile.TenantId = organizationId;
        return true;
    }

    #endregion
}
