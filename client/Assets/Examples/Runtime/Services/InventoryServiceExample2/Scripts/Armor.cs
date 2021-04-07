using Beamable.Common.Content;
using Beamable.Common.Inventory;

namespace Beamable.Examples.Services.InventoryService
{
    [ContentType("armor")]
    public class Armor : ItemContent
    {
        public string Name = "";
        public int Defense = 0;
    }
}