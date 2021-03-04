using Beamable.Common.Content;
using UnityEngine;

namespace Beamable.Examples.Labs.GameContentDesignerDemo.Content
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

   /// <summary>
   /// Demonstrates <see cref="GameContentDesignerDemo"/>.
   /// </summary>
   public class GameContentDesignerDemo_Content : MonoBehaviour
   {
      //  Fields  ---------------------------------------
      [SerializeField] private WeaponsContentRef _weaponsContentRef = null;

      //  Unity Methods   -------------------------------
      protected async void Start()
      {
         Debug.Log($"Start()");
         
         GCDWeapons weaponsContent = await _weaponsContentRef.Resolve();

         foreach (WeaponContentRef weaponContentRef in weaponsContent.WeaponContentRefs)
         {
            GCDWeapon weapon = await weaponContentRef.Resolve();

            Debug.Log($"weapon.Name = {weapon.Name}, weapon.Damage = {weapon.Damage}");
         }
      }
   }
}

