using System.Collections.Generic;
using System.IO;
using Fonet.DataTypes;
using Fonet.Fo.Flow;
using Fonet.Fo.Pagination;

namespace Fonet.Layout
{
    internal class AreaTree
    {
        private FontInfo fontInfo;

        private readonly StreamRenderer streamRenderer;

        public AreaTree(StreamRenderer streamRenderer)
        {
            this.streamRenderer = streamRenderer;
        }

        public void setFontInfo(FontInfo fontInfo)
        {
            this.fontInfo = fontInfo;
        }

        public FontInfo getFontInfo()
        {
            return this.fontInfo;
        }

        public void addPage(Page page)
        {
            try
            {
                streamRenderer.QueuePage(page);
            }
            catch (IOException e)
            {
                throw new FonetException("", e);
            }
        }

        public IDReferences getIDReferences()
        {
            return streamRenderer.GetIDReferences();
        }

        public List<Marker> GetDocumentMarkers()
        {
            return streamRenderer.GetDocumentMarkers();
        }

        public PageSequence GetCurrentPageSequence()
        {
            return streamRenderer.GetCurrentPageSequence();
        }

        public List<Marker> GetCurrentPageSequenceMarkers()
        {
            return streamRenderer.GetCurrentPageSequenceMarkers();
        }
    }
}