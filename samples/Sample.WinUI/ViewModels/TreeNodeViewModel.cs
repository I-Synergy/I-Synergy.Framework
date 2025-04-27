using ISynergy.Framework.Core.Collections;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Enumerations;
using Sample.Models;
using System.Collections.ObjectModel;

namespace Sample.ViewModels;

public class TreeNodeViewModel : ViewModelNavigation<TreeNode<Guid, PublicationItem>>
{
    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public override string Title { get { return LanguageService.Default.GetString("TreeNode"); } }

    /// <summary>
    /// Gets or sets the Publication property value.
    /// </summary>
    public ObservableCollection<TreeNode<Guid, PublicationItem>> Publication
    {
        get => GetValue<ObservableCollection<TreeNode<Guid, PublicationItem>>>();
        set => SetValue(value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InfoViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger"></param>
    public TreeNodeViewModel(ICommonServices commonServices, ILogger<TreeNodeViewModel> logger)
        : base(commonServices, logger)
    {
        Publication = [];

        Publication publication = new("Test publication", "1.0.0", 0);

        PublicationItem chapter1Model = new(publication.PublicationId, "Chapter 1", PublicationItemTypes.Chapter);
        PublicationItem topicAModel = new(publication.PublicationId, "Topic A", PublicationItemTypes.Topic);
        PublicationItem documentA1Model = new(publication.PublicationId, "Document A1", PublicationItemTypes.Document);
        PublicationItem documentA2Model = new(publication.PublicationId, "Document A2", PublicationItemTypes.Document);
        PublicationItem documentA3Model = new(publication.PublicationId, "Document A3", PublicationItemTypes.Document);
        PublicationItem topicBModel = new(publication.PublicationId, "Topic B", PublicationItemTypes.Topic);
        PublicationItem documentB1Model = new(publication.PublicationId, "Document B1", PublicationItemTypes.Document);
        PublicationItem documentB2Model = new(publication.PublicationId, "Document B2", PublicationItemTypes.Document);
        PublicationItem documentB3Model = new(publication.PublicationId, "Document B3", PublicationItemTypes.Document);
        PublicationItem documentB4Model = new(publication.PublicationId, "Document B4", PublicationItemTypes.Document);
        PublicationItem chapter2Model = new(publication.PublicationId, "Chapter 2", PublicationItemTypes.Chapter);
        PublicationItem topicCModel = new(publication.PublicationId, "Topic C", PublicationItemTypes.Topic);
        PublicationItem documentC1Model = new(publication.PublicationId, "Document C1", PublicationItemTypes.Document);
        PublicationItem documentC2Model = new(publication.PublicationId, "Document C2", PublicationItemTypes.Document);
        PublicationItem documentC3Model = new(publication.PublicationId, "Document C3", PublicationItemTypes.Document);
        PublicationItem documentC4Model = new(publication.PublicationId, "Document C4", PublicationItemTypes.Document);
        PublicationItem documentC5Model = new(publication.PublicationId, "Document C5", PublicationItemTypes.Document);

        TreeNode<Guid, PublicationItem> chapter1 = publication.AddChild(chapter1Model);
        TreeNode<Guid, PublicationItem> topicA = chapter1.AddChild(topicAModel);
        topicA.AddChild(documentA1Model);
        topicA.AddChild(documentA2Model);
        topicA.AddChild(documentA3Model);

        TreeNode<Guid, PublicationItem> topicB = chapter1.AddChild(topicBModel);
        topicB.AddChild(documentB1Model);
        topicB.AddChild(documentB2Model);
        topicB.AddChild(documentB3Model);
        topicB.AddChild(documentB4Model);

        TreeNode<Guid, PublicationItem> chapter2 = publication.AddChild(chapter2Model);

        TreeNode<Guid, PublicationItem> topicC = chapter2.AddChild(topicCModel);
        topicC.AddChild(documentC1Model);
        topicC.AddChild(documentC2Model);
        topicC.AddChild(documentC3Model);
        topicC.AddChild(documentC4Model);
        topicC.AddChild(documentC5Model);

        Publication.Add(publication);
    }
}
