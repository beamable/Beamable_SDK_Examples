using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Examples.Shared
{
   /// <summary>
   /// The UI for shared use.
   /// </summary>
   public class ExampleCanvasUI : MonoBehaviour
   {
      //  Fields  ---------------------------------------
      /// <summary>
      /// Serialized, but not
      /// populated: So subclasses must properly drag references before use
      /// </summary>
      [SerializeField] 
      protected Button _resetPlayerButton = null;
      
      /// <summary>
      /// Serialized, but not
      /// populated: So subclasses must properly drag references before use
      /// </summary>
      [SerializeField] 
      protected List<ExamplePanelUI> _examplePanelUIs = null;

      // Courtesy Shortcuts to UI API. Helps create concise code elsewhere.
      protected TMP_Text TitleText01 { get { return _examplePanelUIs[0].TitleText; }}
      protected TMP_Text TitleText02 { get { return _examplePanelUIs[1].TitleText; }}
      protected TMP_Text TitleText03 { get { return _examplePanelUIs[2].TitleText; }}
      protected TMP_Text BodyText01 { get { return _examplePanelUIs[0].BodyText; }}
      protected TMP_Text BodyText02 { get { return _examplePanelUIs[1].BodyText; }}
      protected TMP_Text BodyText03 { get { return _examplePanelUIs[2].BodyText; }}
      protected Button Button01 { get { return _examplePanelUIs[0].Buttons[0];}}
      protected Button Button02 { get { return _examplePanelUIs[0].Buttons[1];}}
      protected Button Button03 { get { return _examplePanelUIs[0].Buttons[2];}}
      
      //  Unity Methods  --------------------------------
      protected virtual void Start()
      {
         if (this.GetType() == typeof(ExampleCanvasUI))
         {
            Debug.LogError($"The class {typeof(ExampleCanvasUI)} must be subclassed before using.");
         }
         
         if (_resetPlayerButton == null)
         {
            Debug.LogError($"This reference `{_resetPlayerButton}` must not be null. Drag via inspector.");
            return;
         }
         
         _resetPlayerButton.onClick.AddListener(ResetPlayerButton_OnClicked);
      }

      protected async void ResetPlayerButton_OnClicked()
      {
         _resetPlayerButton.interactable = false;
         // Prepare session
         var beamContext = BeamContext.Default;
         await beamContext.OnReady;
         
         // Use session
         ExampleProjectHacks.ClearDeviceUsersAndReloadScene();
      }
   }
}


