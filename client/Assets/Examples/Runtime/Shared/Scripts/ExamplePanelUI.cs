using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Examples.Shared
{
   /// <summary>
   /// The UI for shared use.
   /// </summary>
   public class ExamplePanelUI : MonoBehaviour
   {
      //  Properties  ---------------------------------------
      public TMP_Text TitleText
      {
         get
         {
            return _titleText;
         }
         set
         {
            _titleText = value;
         }
      }
      
      public TMP_Text BodyText
      {
         get
         {
            return _bodyText;
         }
         set
         {
            _bodyText = value;
         }
      }

      public List<Button> Buttons
      {
         get
         {
            return _buttons;
         }
      }

      //  Fields  ---------------------------------------
      [SerializeField] private TMP_Text _titleText = null;
      [SerializeField] private TMP_Text _bodyText = null;
      [SerializeField] private List<Button> _buttons = null;

   }
}


