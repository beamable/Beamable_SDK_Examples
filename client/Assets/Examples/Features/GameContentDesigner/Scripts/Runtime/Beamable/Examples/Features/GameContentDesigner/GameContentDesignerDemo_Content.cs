
using DisruptorBeam.Content;
using UnityEngine;

namespace Beamable.Examples.Features.GameContentDesignerDemo.Content
{
   [ContentType("weapon")]
   public class Weapon : ContentObject
   {
      public string Name;
      public string Damage;
   }

   [ContentType("weapons")]
   public class Weapons : ContentObject
   {
      public WeaponContentRef[] WeaponContentRefs;
   }

   [System.Serializable]
   public class WeaponContentRef : ContentRef<Weapon> { }

   [System.Serializable]
   public class WeaponsContentRef : ContentRef<Weapons> { }

   public class GameContentDesignerDemo_Content : MonoBehaviour
   {
      [SerializeField]
      private WeaponsContentRef _weaponsContentRef;

      protected async void Start()
      {
         Weapons weaponsContent = await _weaponsContentRef.Resolve();

         foreach (WeaponContentRef weaponContentRef in weaponsContent.WeaponContentRefs)
         {
            Weapon weapon = await weaponContentRef.Resolve();

            Debug.Log("weapon: " + weapon.Name + " " + weapon.Damage);
         }
      }
   }
}

