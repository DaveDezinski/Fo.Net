namespace Fonet.Fo.Flow
{
    using Fonet.Fo.Properties;
    using Fonet.Layout;

    internal class BlockContainer : FObj
    {
        private int _position;
        private int _top;
        private int _bottom;
        private int _left;
        private int _right;
        private int _width;
        private int _height;
        private int _span;
        private AreaContainer _areaContainer;

        new internal class Maker : FObj.Maker
        {
            public override FObj Make(FObj parent, PropertyList propertyList)
            {
                return new BlockContainer(parent, propertyList);
            }
        }

        new public static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected BlockContainer(FObj parent, PropertyList propertyList)
            : base(parent, propertyList)
        {
            this.name = "fo:block-container";
            this._span = this.properties.GetProperty("span").GetEnum();
        }

        public override Status Layout(Area area)
        {
            if (this.marker == MarkerStart)
            {
                AbsolutePositionProps mAbsProps = propMgr.GetAbsolutePositionProps();
                BorderAndPadding bap = propMgr.GetBorderAndPadding();
                BackgroundProps bProps = propMgr.GetBackgroundProps();
                MarginProps mProps = propMgr.GetMarginProps(); this.marker = 0;
                this._position = this.properties.GetProperty("position").GetEnum();
                this._top = this.properties.GetProperty("top").GetLength().MValue();
                this._bottom = this.properties.GetProperty("bottom").GetLength().MValue();
                this._left = this.properties.GetProperty("left").GetLength().MValue();
                this._right = this.properties.GetProperty("right").GetLength().MValue();
                this._width = this.properties.GetProperty("width").GetLength().MValue();
                this._height = this.properties.GetProperty("height").GetLength().MValue();
                _span = this.properties.GetProperty("span").GetEnum();

                string id = this.properties.GetProperty("id").GetString();
                area.getIDReferences().InitializeID(id, area);
            }

            AreaContainer container;
            // Apply fix proposed by claytonrumley
            // on http://fonet.codeplex.com/workitem/4647
            if (area is BlockArea area1)
            {
                container = area1.getNearestAncestorAreaContainer();
            }
            else
            {
                container = (AreaContainer)area;
            } 
            if ((this._width == 0) && (this._height == 0))
            {
                _width = _right - _left;
                _height = _bottom - _top;
            }

            this._areaContainer =
                new AreaContainer(propMgr.GetFontState(container.getFontInfo()),
                                  container.getXPosition() + _left,
                                  container.GetYPosition() - _top, _width, _height,
                                  _position);

            _areaContainer.setPage(area.getPage());
            _areaContainer.setBackground(propMgr.GetBackgroundProps());
            _areaContainer.setBorderAndPadding(propMgr.GetBorderAndPadding());
            _areaContainer.start();

            _areaContainer.setAbsoluteHeight(0);
            _areaContainer.setIDReferences(area.getIDReferences());

            int numChildren = this.children.Count;
            for (int i = this.marker; i < numChildren; i++)
            {
                FObj fo = (FObj)children[i];
                fo.Layout(_areaContainer);
            }

            _areaContainer.end();
            if (_position == Position.ABSOLUTE)
            {
                _areaContainer.SetHeight(_height);
            }
            area.addChild(_areaContainer);

            return new Status(Status.OK);
        }

        public override int GetContentWidth()
        {
            if (_areaContainer != null)
            {
                return _areaContainer.getContentWidth();
            }
            else
            {
                return 0;
            }
        }

        public override bool GeneratesReferenceAreas()
        {
            return true;
        }

        public int GetSpan()
        {
            return this._span;
        }
    }
}