using Beamable.Common.Content;
using UnityEngine;

namespace Beamable.Examples.Labs.GameContentDesignerDemo.Content
{
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
        protected void Start()
        {
            Debug.Log($"Start()");
         
            SetupBeamable();
        }
      
        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            //2
            var beamContext = BeamContext.Default;
            await beamContext.OnReady;

            Debug.Log($"beamContext.PlayerId = {beamContext.PlayerId}");
            
            //1
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