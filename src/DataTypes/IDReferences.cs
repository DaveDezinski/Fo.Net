namespace Fonet.DataTypes
{
    using System.Collections;
    using System.Text;
    using Fonet.Layout;
    using Fonet.Pdf;

    internal class IDReferences
    {
        private readonly Hashtable _idReferences;
        private readonly Hashtable _idValidation;
        private readonly Hashtable _idUnvalidated;
        private const int ID_PADDING = 5000;

        public IDReferences()
        {
            _idReferences = new Hashtable();
            _idValidation = new Hashtable();
            _idUnvalidated = new Hashtable();
        }

        public void InitializeID(string id, Area area)
        {
            CreateID(id);
            ConfigureID(id, area);
        }

        public void CreateID(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (DoesUnvalidatedIDExist(id))
                {
                    RemoveFromUnvalidatedIDList(id);
                    RemoveFromIdValidationList(id);
                }
                else if (doesIDExist(id))
                {
                    throw new FonetException("The id \"" + id
                        + "\" already exists in this document");
                }
                else
                {
                    createNewId(id);
                    RemoveFromIdValidationList(id);
                }

            }
        }

        public void CreateUnvalidatedID(string id)
        {
            if (!string.IsNullOrEmpty(id) && !doesIDExist(id))
            {
                createNewId(id);
                AddToUnvalidatedIdList(id);
            }
        }

        public void AddToUnvalidatedIdList(string id)
        {
            _idUnvalidated[id] = "";
        }

        public void RemoveFromUnvalidatedIDList(string id)
        {
            _idUnvalidated.Remove(id);
        }

        public bool DoesUnvalidatedIDExist(string id)
        {
            return _idUnvalidated.ContainsKey(id);
        }

        public void ConfigureID(string id, Area area)
        {
            if (!string.IsNullOrEmpty(id))
            {
                setPosition(id,
                            area.getPage().getBody().getXPosition()
                                + area.getTableCellXOffset() - ID_PADDING,
                            area.getPage().getBody().GetYPosition()
                                - area.getAbsoluteHeight() + ID_PADDING);
                setPageNumber(id, area.getPage().getNumber());
                area.getPage().addToIDList(id);
            }
        }

        public void AddToIdValidationList(string id)
        {
            _idValidation[id] = "";
        }

        public void RemoveFromIdValidationList(string id)
        {
            _idValidation.Remove(id);
        }

        public void RemoveID(string id)
        {
            _idReferences.Remove(id);
        }

        public bool IsEveryIdValid()
        {
            return (_idValidation.Count == 0);
        }

        public string GetInvalidIds()
        {
            StringBuilder list = new StringBuilder();
            foreach (object o in _idValidation.Keys)
            {
                list.Append("\n\"");
                list.Append(o.ToString());
                list.Append("\" ");
            }
            return list.ToString();
        }

        public bool doesIDExist(string id)
        {
            return _idReferences.ContainsKey(id);
        }

        public bool doesGoToReferenceExist(string id)
        {
            IDNode node = (IDNode)_idReferences[id];
            return node.IsThereInternalLinkGoTo();
        }

        public PdfGoTo getInternalLinkGoTo(string id)
        {
            IDNode node = (IDNode)_idReferences[id];
            return node.GetInternalLinkGoTo();
        }

        public PdfGoTo createInternalLinkGoTo(string id, PdfObjectId objectId)
        {
            IDNode node = (IDNode)_idReferences[id];
            node.CreateInternalLinkGoTo(objectId);
            return node.GetInternalLinkGoTo();
        }

        public void createNewId(string id)
        {
            IDNode node = new IDNode(id);
            _idReferences[id] = node;
        }

        public PdfGoTo getPDFGoTo(string id)
        {
            return getInternalLinkGoTo(id);
        }

        public void setInternalGoToPageReference(string id,
                                                 PdfObjectReference pageReference)
        {
            IDNode node = (IDNode)_idReferences[id];
            if (node != null)
            {
                node.SetInternalLinkGoToPageReference(pageReference);
            }
        }

        public void setPageNumber(string id, int pageNumber)
        {
            IDNode node = (IDNode)_idReferences[id];
            node.SetPageNumber(pageNumber);
        }

        public string getPageNumber(string id)
        {
            if (doesIDExist(id))
            {
                IDNode node = (IDNode)_idReferences[id];
                return node.GetPageNumber();
            }
            else
            {
                AddToIdValidationList(id);
                return null;
            }
        }

        public void setPosition(string id, int x, int y)
        {
            IDNode node = (IDNode)_idReferences[id];
            node.SetPosition(x, y);
        }

        public ICollection getInvalidElements()
        {
            return _idValidation.Keys;
        }
    }
}