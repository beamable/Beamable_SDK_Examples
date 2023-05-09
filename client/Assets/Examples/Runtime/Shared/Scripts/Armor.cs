using System;
using Beamable.Common.Content;
using Beamable.Common.Inventory;

namespace Beamable.Examples.Shared
{
    [Serializable]
    public class ArmorLink : ContentLink<Armor> {}

    [Serializable]
    public class ArmorRef : ContentRef<Armor> {}

    /// <summary>
    /// This type defines a custom ItemContent for use in
    /// several example scenes.
    ///
    /// Note that content types are hierarchical. Because this is a subclass of
    /// ItemContent, it also inherits "items" as a parent content type. The
    /// resulting content type will be "items.armor", nested under "items".
    /// </summary>
    [ContentType("armor")]
    public class Armor : ItemContent
    {
        public string Name = "";
        public int Defense = 0;
    }
}
