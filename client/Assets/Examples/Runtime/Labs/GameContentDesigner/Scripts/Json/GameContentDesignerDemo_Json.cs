using UnityEngine;

namespace Beamable.Examples.Labs.GameContentDesigner.Json
{
   /// <summary>
   /// Demonstrates <see cref="GameContentDesignerDemo"/>.
   /// </summary>
   public class GameContentDesignerDemo_Json : MonoBehaviour
   {
      //  Fields  ---------------------------------------

      [SerializeField]
      private TextAsset _weaponsJson;

      //  Unity Methods  --------------------------------
      protected void Start()
      {
         Debug.Log($"Start()");
         
         WeaponsJson weapons = JsonUtility.FromJson<WeaponsJson>(_weaponsJson.text);

         foreach (WeaponJson weapon in weapons.Weapons)
         {
            Debug.Log($"weapon.Name = {weapon.Name}, weapon.Damage = {weapon.Damage}");
         }
      }
   }
}