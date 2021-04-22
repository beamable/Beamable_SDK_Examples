using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Examples.Shared
{ 
    /// <summary>
    /// Move the <see cref="StoreFlow"/> fully onscreen
    /// so the buy button can take user input.
    ///
    /// Note: This is a temporary HACK.
    /// 
    /// </summary>
    public class StoreFlowHack : MonoBehaviour
    {
        //  Properties  -----------------------------------
        //  Fields  ---------------------------------------
        [SerializeField] private VerticalLayoutGroup _verticalLayoutGroup = null;
        
        //  Unity Methods  --------------------------------

        protected void Start()
        {
            StartCoroutine(UpdateUILayout_Coroutine());
        }
        
        //  Methods  --------------------------------------
        private IEnumerator UpdateUILayout_Coroutine()
        {
            // Find child
            Transform child = null;
            do
            {
                if (_verticalLayoutGroup.transform.childCount > 0)
                {
                    child = _verticalLayoutGroup.transform.GetChild(0);
                }
                
                yield return new WaitForEndOfFrame();
                
            } while (child == null);
            
            Debug.Log($"StoreFlowHack.UpdateUILayout()");
            
            // Reparent child
            Transform parentParent = _verticalLayoutGroup.transform.parent.parent;
            
            child.transform.SetParent(parentParent);
            
            // Move child fully onscreen (to be clickable)
            float x = child.GetComponent<RectTransform>().localPosition.x;
            child.GetComponent<RectTransform>().localPosition = new Vector3(x, -50);

        }
    }
}