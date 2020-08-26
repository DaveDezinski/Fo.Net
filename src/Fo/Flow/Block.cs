namespace Fonet.Fo.Flow
{
    using Fonet.Layout;

    internal class Block : FObjMixed
    {
        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new Block(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        private int _align;
        private int _alignLast;
        private int _breakAfter;
        private int _lineHeight;
        private int _startIndent;
        private int _endIndent;
        private int _spaceBefore;
        private int _spaceAfter;
        private int _textIndent;
        private int _keepWithNext;
        private int _blockWidows;
        private int _blockOrphans;
        private int _areaHeight = 0;
        private int _contentWidth = 0;
        private string _id;
        private readonly int _span;
        private bool _anythingLaidOut = false;

        public Block(FObj parent, PropertyList propertyList) : base(parent, propertyList)
        {
            this.name = "fo:block";

            switch (parent.GetName())
            {
                case "fo:basic-link":
                case "fo:block":
                case "fo:block-container":
                case "fo:float":
                case "fo:flow":
                case "fo:footnote-body":
                case "fo:inline":
                case "fo:inline-container":
                case "fo:list-item-body":
                case "fo:list-item-label":
                case "fo:marker":
                case "fo:multi-case":
                case "fo:static-content":
                case "fo:table-caption":
                case "fo:table-cell":
                case "fo:wrapper":
                    break;
                default:
                    throw new FonetException(
                        "fo:block must be child of " +
                            "fo:basic-link, fo:block, fo:block-container, fo:float, fo:flow, fo:footnote-body, fo:inline, fo:inline-container, fo:list-item-body, fo:list-item-label, fo:marker, fo:multi-case, fo:static-content, fo:table-caption, fo:table-cell or fo:wrapper " +
                            "not " + parent.GetName());
            }
            this._span = this.properties.GetProperty("span").GetEnum();
            ts = propMgr.getTextDecoration(parent);
        }

        public override Status Layout(Area area)
        {
            BlockArea blockArea;

            if (this.marker == MarkerBreakAfter)
            {
                return new Status(Status.OK);
            }

            if (this.marker == MarkerStart)
            {
                AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
                AuralProps mAurProps = propMgr.GetAuralProps();
                BorderAndPadding bap = propMgr.GetBorderAndPadding();
                BackgroundProps bProps = propMgr.GetBackgroundProps();
                HyphenationProps mHyphProps = propMgr.GetHyphenationProps();
                MarginProps mProps = propMgr.GetMarginProps();
                RelativePositionProps mRelProps = propMgr.GetRelativePositionProps();

                this._align = this.properties.GetProperty("text-align").GetEnum();
                this._alignLast = this.properties.GetProperty("text-align-last").GetEnum();
                this._breakAfter = this.properties.GetProperty("break-after").GetEnum();
                this._lineHeight =
                    this.properties.GetProperty("line-height").GetLength().MValue();
                this._startIndent =
                    this.properties.GetProperty("start-indent").GetLength().MValue();
                this._endIndent =
                    this.properties.GetProperty("end-indent").GetLength().MValue();
                this._spaceBefore =
                    this.properties.GetProperty("space-before.optimum").GetLength().MValue();
                this._spaceAfter =
                    this.properties.GetProperty("space-after.optimum").GetLength().MValue();
                this._textIndent =
                    this.properties.GetProperty("text-indent").GetLength().MValue();
                this._keepWithNext =
                    this.properties.GetProperty("keep-with-next").GetEnum();

                this._blockWidows =
                    this.properties.GetProperty("widows").GetNumber().IntValue();
                this._blockOrphans =
                    this.properties.GetProperty("orphans").GetNumber().IntValue();
                this._id = this.properties.GetProperty("id").GetString();

                if (area is BlockArea)
                {
                    area.end();
                }

                if (area.getIDReferences() != null)
                {
                    area.getIDReferences().CreateID(_id);
                }

                this.marker = 0;

                int breakBeforeStatus = propMgr.CheckBreakBefore(area);
                if (breakBeforeStatus != Status.OK)
                {
                    return new Status(breakBeforeStatus);
                }

                int numChildren = this.children.Count;
                for (int i = 0; i < numChildren; i++)
                {
                    FONode fo = (FONode)children[i];
                    if (fo is FOText text)
                    {
                        if (text.willCreateArea())
                        {
                            fo.SetWidows(_blockWidows);
                            break;
                        }
                        else
                        {
                            children.RemoveAt(i);
                            numChildren = this.children.Count;
                            i--;
                        }
                    }
                    else
                    {
                        fo.SetWidows(_blockWidows);
                        break;
                    }
                }

                for (int i = numChildren - 1; i >= 0; i--)
                {
                    FONode fo = (FONode)children[i];
                    if (fo is FOText text)
                    {
                        if (text.willCreateArea())
                        {
                            fo.SetOrphans(_blockOrphans);
                            break;
                        }
                    }
                    else
                    {
                        fo.SetOrphans(_blockOrphans);
                        break;
                    }
                }
            }

            if ((_spaceBefore != 0) && (this.marker == 0))
            {
                area.addDisplaySpace(_spaceBefore);
            }

            if (_anythingLaidOut)
            {
                this._textIndent = 0;
            }

            if (marker == 0 && area.getIDReferences() != null)
            {
                area.getIDReferences().ConfigureID(_id, area);
            }

            int spaceLeft = area.spaceLeft();
            blockArea =
                new BlockArea(propMgr.GetFontState(area.getFontInfo()),
                              area.getAllocationWidth(), area.spaceLeft(),
                              _startIndent, _endIndent, _textIndent, _align,
                              _alignLast, _lineHeight);
            blockArea.setGeneratedBy(this);
            this.areasGenerated++;
            if (this.areasGenerated == 1)
            {
                blockArea.isFirst(true);
            }
            blockArea.addLineagePair(this, this.areasGenerated);
            blockArea.setParent(area);
            blockArea.setPage(area.getPage());
            blockArea.setBackground(propMgr.GetBackgroundProps());
            blockArea.setBorderAndPadding(propMgr.GetBorderAndPadding());
            blockArea.setHyphenation(propMgr.GetHyphenationProps());
            blockArea.start();

            blockArea.setAbsoluteHeight(area.getAbsoluteHeight());
            blockArea.setIDReferences(area.getIDReferences());

            blockArea.setTableCellXOffset(area.getTableCellXOffset());

            for (int i = this.marker; i < children.Count; i++)
            {
                FONode fo = (FONode)children[i];
                Status status = fo.Layout(blockArea);
                if (status.isIncomplete())
                {
                    this.marker = i;
                    if (status.getCode() == Status.AREA_FULL_NONE)
                    {
                        if ((i != 0))
                        {
                            status = new Status(Status.AREA_FULL_SOME);
                            area.addChild(blockArea);
                            area.setMaxHeight(area.getMaxHeight() - spaceLeft
                                + blockArea.getMaxHeight());
                            area.increaseHeight(blockArea.GetHeight());
                            _anythingLaidOut = true;

                            return status;
                        }
                        else
                        {
                            _anythingLaidOut = false;
                            return status;
                        }
                    }
                    area.addChild(blockArea);
                    area.setMaxHeight(area.getMaxHeight() - spaceLeft
                        + blockArea.getMaxHeight());
                    area.increaseHeight(blockArea.GetHeight());
                    _anythingLaidOut = true;
                    return status;
                }
                _anythingLaidOut = true;
            }

            blockArea.end();

            area.setMaxHeight(area.getMaxHeight() - spaceLeft
                + blockArea.getMaxHeight());

            area.addChild(blockArea);

            area.increaseHeight(blockArea.GetHeight());

            if (_spaceAfter != 0)
            {
                area.addDisplaySpace(_spaceAfter);
            }

            if (area is BlockArea)
            {
                area.start();
            }
            _areaHeight = blockArea.GetHeight();
            _contentWidth = blockArea.getContentWidth();
            int breakAfterStatus = propMgr.CheckBreakAfter(area);
            if (breakAfterStatus != Status.OK)
            {
                this.marker = MarkerBreakAfter;
                return new Status(breakAfterStatus);
            }

            if (_keepWithNext != 0)
            {
                return new Status(Status.KEEP_WITH_NEXT);
            }

            blockArea.isLast(true);
            return new Status(Status.OK);
        }

        public int GetAreaHeight()
        {
            return _areaHeight;
        }

        public override int GetContentWidth()
        {
            return _contentWidth;
        }

        public int GetSpan()
        {
            return this._span;
        }

        public override void ResetMarker()
        {
            _anythingLaidOut = false;
            base.ResetMarker();
        }
    }
}