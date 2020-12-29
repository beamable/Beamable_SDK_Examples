using DisruptorBeam;
using DisruptorBeam.Content;
using UnityEngine;

namespace Beamable.Examples.Features.InventoryFlow
{
   [ContentType("if_armor")]
   public class Armor : ItemContent
   {
      public string Name;
      public string Defense;
   }

   [ContentType("magic_armor")]
   public class MagicArmor : Armor
   {
      public string DefenseMultiplier;
   }

   [System.Serializable]
   public class ArmorContentRef : ContentRef<Armor> { }

   public class InventoryFlowExample : MonoBehaviour
   {
      protected async void Start()
      {
         await DisruptorEngine.Instance.Then(de =>
         {
         });
      }
   }
}

