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

   [ContentType("gcd_weaponsgroup")]
   public class GCDWeaponsGroup : ContentObject
   {
      public WeaponContentRef[] WeaponContentRefs;
   }

   [System.Serializable]
   public class WeaponContentRef : ContentRef<GCDWeapon> { }

   [System.Serializable]
   public class WeaponsGroupContentRef : ContentRef<GCDWeaponsGroup> { }

   /// <summary>
   /// Demonstrates <see cref="GameContentDesignerDemo"/>.
   /// </summary>
   public class GameContentDesignerDemo_Content : MonoBehaviour
   {
      //  Fields  ---------------------------------------
      [SerializeField] private WeaponsGroupContentRef _weaponsGroupContentRef = null;

      //  Unity Methods   -------------------------------
      protected async void Start()
      {
         Debug.Log($"Start()");
         
         GCDWeaponsGroup weaponsGroupContent = await _weaponsGroupContentRef.Resolve();

         Debug.Log($"weaponsGroupContent.ContentName = {weaponsGroupContent.ContentName}");
         
         foreach (WeaponContentRef weaponContentRef in weaponsGroupContent.WeaponContentRefs)
         {
            GCDWeapon weapon = await weaponContentRef.Resolve();

            Debug.Log($"weapon.Name = {weapon.Name}, weapon.Damage = {weapon.Damage}");
         }
      }
   }
}

