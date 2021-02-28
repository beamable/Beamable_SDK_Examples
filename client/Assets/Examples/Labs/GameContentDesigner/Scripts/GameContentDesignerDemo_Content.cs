using Beamable.Common.Content;
using UnityEngine;

namespace Beamable.Examples.Features.GameContentDesignerDemo.Content
{
   [ContentType("gcd_weapon")]
   public class GCDWeapon : ContentObject
   {
      public string Name;
      public string Damage;
   }

   [ContentType("gcd_weapons")]
   public class GCDWeapons : ContentObject
   {
      public WeaponContentRef[] WeaponContentRefs;
   }

   [System.Serializable]
   public class WeaponContentRef : ContentRef<GCDWeapon> { }

   [System.Serializable]
   public class WeaponsContentRef : ContentRef<GCDWeapons> { }

   public class GameContentDesignerDemo_Content : MonoBehaviour
   {
      [SerializeField]
      private WeaponsContentRef _weaponsContentRef;

      protected async void Start()
      {
         GCDWeapons weaponsContent = await _weaponsContentRef.Resolve();

         foreach (WeaponContentRef weaponContentRef in weaponsContent.WeaponContentRefs)
         {
            GCDWeapon weapon = await weaponContentRef.Resolve();

            Debug.Log("weapon: " + weapon.Name + " " + weapon.Damage);
         }
      }
   }
}

