using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Base;
using Sample.Enumerations;
using System;

namespace Sample.Models;

/// <summary>
/// Class PublicationItem.
/// </summary>
public class PublicationItem : ObservableClass
{
    /// <summary>
    /// Gets or sets the ItemId property value.
    /// </summary>
    /// <value>The item identifier.</value>
    [Identity]
    public Guid ItemId
    {
        get => GetValue<Guid>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the ParentId property value.
    /// </summary>
    /// <value>The parent identifier.</value>
    public Guid ParentId
    {
        get => GetValue<Guid>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the PublicationId property value.
    /// </summary>
    /// <value>The publication identifier.</value>
    public Guid PublicationId
    {
        get => GetValue<Guid>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the Type property value.
    /// </summary>
    /// <value>The type.</value>
    public PublicationItemTypes Type
    {
        get => GetValue<PublicationItemTypes>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the Title property value.
    /// </summary>
    /// <value>The title.</value>
    [Title]
    public string Title
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the Description property value.
    /// </summary>
    /// <value>The description.</value>
    public string Description
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the Content property value.
    /// </summary>
    /// <value>The content.</value>
    public byte[] Content
    {
        get => GetValue<byte[]>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the Location property value.
    /// </summary>
    /// <value>The location.</value>
    public Uri Location
    {
        get => GetValue<Uri>();
        set => SetValue(value);
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public PublicationItem()
    {
        ItemId = Guid.NewGuid();
        ParentId = Guid.Empty;
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="publicationId">The publication identifier.</param>
    /// <param name="title">The title.</param>
    /// <param name="type">The type.</param>
    public PublicationItem(Guid publicationId, string title, PublicationItemTypes type = PublicationItemTypes.Document)
        : this()
    {
        PublicationId = publicationId;
        Title = title;
        Type = type;
    }

    /// <summary>
    /// default constructor.
    /// </summary>
    /// <param name="publicationId">The publication identifier.</param>
    /// <param name="title">The title.</param>
    /// <param name="parentId">The parent identifier.</param>
    /// <param name="type">The type.</param>
    public PublicationItem(Guid publicationId, string title, Guid parentId, PublicationItemTypes type = PublicationItemTypes.Document)
        : this(publicationId, title, type)
    {
        ParentId = parentId;
    }
}
