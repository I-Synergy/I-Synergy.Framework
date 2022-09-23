using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Collections;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Enumerations;
using Sample.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sample.ViewModels
{
    public class TreeNodeViewModel : ViewModelNavigation<TreeNode<Guid, PublicationItem>>
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get { return BaseCommonServices.LanguageService.GetString("TreeNode"); } }

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
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="logger">The logger factory.</param>
        public TreeNodeViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILogger logger)
            : base(context, commonServices, logger)
        {
            Publication = new ObservableCollection<TreeNode<Guid, PublicationItem>>();

            var publication = new Publication("Test publication", "1.0.0", 0);

            var chapter1Model = new PublicationItem(publication.PublicationId, "Chapter 1", PublicationItemTypes.Chapter);
            var topicAModel = new PublicationItem(publication.PublicationId, "Topic A", PublicationItemTypes.Topic);
            var documentA1Model = new PublicationItem(publication.PublicationId, "Document A1", PublicationItemTypes.Document);
            var documentA2Model = new PublicationItem(publication.PublicationId, "Document A2", PublicationItemTypes.Document);
            var documentA3Model = new PublicationItem(publication.PublicationId, "Document A3", PublicationItemTypes.Document);
            var topicBModel = new PublicationItem(publication.PublicationId, "Topic B", PublicationItemTypes.Topic);
            var documentB1Model = new PublicationItem(publication.PublicationId, "Document B1", PublicationItemTypes.Document);
            var documentB2Model = new PublicationItem(publication.PublicationId, "Document B2", PublicationItemTypes.Document);
            var documentB3Model = new PublicationItem(publication.PublicationId, "Document B3", PublicationItemTypes.Document);
            var documentB4Model = new PublicationItem(publication.PublicationId, "Document B4", PublicationItemTypes.Document);
            var chapter2Model = new PublicationItem(publication.PublicationId, "Chapter 2", PublicationItemTypes.Chapter);
            var topicCModel = new PublicationItem(publication.PublicationId, "Topic C", PublicationItemTypes.Topic);
            var documentC1Model = new PublicationItem(publication.PublicationId, "Document C1", PublicationItemTypes.Document);
            var documentC2Model = new PublicationItem(publication.PublicationId, "Document C2", PublicationItemTypes.Document);
            var documentC3Model = new PublicationItem(publication.PublicationId, "Document C3", PublicationItemTypes.Document);
            var documentC4Model = new PublicationItem(publication.PublicationId, "Document C4", PublicationItemTypes.Document);
            var documentC5Model = new PublicationItem(publication.PublicationId, "Document C5", PublicationItemTypes.Document);

            var chapter1 = publication.AddChild(new TreeNode<Guid, PublicationItem>(chapter1Model.ItemId, chapter1Model));
            var topicA = chapter1.AddChild(new TreeNode<Guid, PublicationItem>(topicAModel.ItemId, topicAModel));
            topicA.AddChild(new TreeNode<Guid, PublicationItem>(documentA1Model.ItemId, documentA1Model));
            topicA.AddChild(new TreeNode<Guid, PublicationItem>(documentA2Model.ItemId, documentA2Model));
            topicA.AddChild(new TreeNode<Guid, PublicationItem>(documentA3Model.ItemId, documentA3Model));

            var topicB = chapter1.AddChild(new TreeNode<Guid, PublicationItem>(topicBModel.ItemId, topicBModel));
            topicB.AddChild(new TreeNode<Guid, PublicationItem>(documentB1Model.ItemId, documentB1Model));
            topicB.AddChild(new TreeNode<Guid, PublicationItem>(documentB2Model.ItemId, documentB2Model));
            topicB.AddChild(new TreeNode<Guid, PublicationItem>(documentB3Model.ItemId, documentB3Model));
            topicB.AddChild(new TreeNode<Guid, PublicationItem>(documentB4Model.ItemId, documentB4Model));

            var chapter2 = publication.AddChild(new TreeNode<Guid, PublicationItem>(chapter2Model.ItemId, chapter2Model));

            var topicC = chapter2.AddChild(new TreeNode<Guid, PublicationItem>(topicCModel.ItemId, topicCModel));
            topicC.AddChild(new TreeNode<Guid, PublicationItem>(documentC1Model.ItemId, documentC1Model));
            topicC.AddChild(new TreeNode<Guid, PublicationItem>(documentC2Model.ItemId, documentC2Model));
            topicC.AddChild(new TreeNode<Guid, PublicationItem>(documentC3Model.ItemId, documentC3Model));
            topicC.AddChild(new TreeNode<Guid, PublicationItem>(documentC4Model.ItemId, documentC4Model));
            topicC.AddChild(new TreeNode<Guid, PublicationItem>(documentC5Model.ItemId, documentC5Model));

            Publication.Add(publication);
        }
    }
}
