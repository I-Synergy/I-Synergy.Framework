using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Messages.Base;

namespace ISynergy.Framework.Core.Messages;

/// <summary>
/// Published by <see cref="ISynergy.Framework.Core.Abstractions.Services.IAuthenticationService"/>
/// when the active tenant changes (e.g. user switches organizations or switches back to a personal context).
/// The <see cref="BaseMessage{T}.Content"/> carries the new <see cref="IProfile.TenantId"/>.
/// </summary>
public sealed class TenantSwitchedMessage : BaseMessage<Guid>
{
    public TenantSwitchedMessage(Guid newTenantId)
        : base(newTenantId)
    {
    }
}
