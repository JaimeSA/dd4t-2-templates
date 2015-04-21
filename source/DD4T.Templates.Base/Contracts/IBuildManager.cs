using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCM = Tridion.ContentManager.ContentManagement;
using Dynamic = DD4T.ContentModel;
using TComm = Tridion.ContentManager.CommunicationManagement;
using DD4T.ContentModel.Contracts.Serializing;
using DD4T.Templates.Base.Builder;
using DD4T.Templates.Base.Utils;
using Tridion.ContentManager.Templating;
using DD4T.ContentModel;

namespace DD4T.Templates.Base.Contracts
{
    public interface IBuildManager
    {
        BinaryPublisher BinaryPublisher
        {
            get;
            set;
        }

        void Initialize(Package p, Engine e);
        BuildProperties BuildProperties { get; set; }
        ISerializerService SerializerService { get; set; }

        Dynamic.Page BuildPage(TComm.Page tcmPage, Engine engine);

        List<Dynamic.Category> BuildCategories(TComm.Page page);

        List<Dynamic.Category> BuildCategories(TCM.Component component);

        Dynamic.Component BuildComponent(TCM.Component tcmComponent);

        Dynamic.Component BuildComponent(TCM.Component tcmComponent, int currentLinkLevel);

        Dynamic.ComponentPresentation BuildComponentPresentation(TComm.ComponentPresentation tcmComponentPresentation,
            Engine engine, int linkLevels, bool resolveWidthAndHeight);

        Dynamic.ComponentTemplate BuildComponentTemplate(TComm.ComponentTemplate tcmComponentTemplate);

        Dynamic.Field BuildField(TCM.Fields.ItemField tcmItemField, int currentLinkLevel);

        Dynamic.FieldSet BuildFields(TCM.Fields.ItemFields tcmItemFields);

        Dynamic.FieldSet BuildFields(TCM.Fields.ItemFields tcmItemFields, int currentLinkLevel);

        Dynamic.Keyword BuildKeyword(TCM.Keyword keyword);

        Dynamic.Keyword BuildKeyword(TCM.Keyword keyword, int linkLevels);

        Dynamic.OrganizationalItem BuildOrganizationalItem(TComm.StructureGroup tcmStructureGroup);

        Dynamic.OrganizationalItem BuildOrganizationalItem(TCM.Folder tcmFolder);

        Dynamic.PageTemplate BuildPageTemplate(TComm.PageTemplate tcmPageTemplate);

        Dynamic.Publication BuildPublication(TCM.Repository tcmPublication);

        Dynamic.Schema BuildSchema(TCM.Schema tcmSchema);

        void AddXpathToFields(Dynamic.FieldSet fieldSet, string baseXpath);

        string PublishMultimediaComponent(Component component);

        string PublishBinariesInRichTextField(string v);
    }
}
