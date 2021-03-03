using UnityEngine;

namespace Beamable.Examples.Features.GameContentDesigner.Json
{
   [System.Serializable]
   public class WeaponJson
   {
      public string Name;
      public string Damage;
   }

   [System.Serializable]
   public class WeaponsJson
   {
      public WeaponJson[] Weapons;
   }

   public class GameContentDesignerDemo_Json : MonoBehaviour
   {
      [SerializeField]
      private TextAsset _weaponsJson;

      protected void Start()
      {
         WeaponsJson weapons = JsonUtility.FromJson<WeaponsJson>(_weaponsJson.text);

         foreach (WeaponJson weapon in weapons.Weapons)
         {
            Debug.Log($"weapon.Name = {weapon.Name}, weapon.Damage = {weapon.Damage}");
         }
      }
   }
}