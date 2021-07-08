using System;
using Beamable.Common.Content;
using Beamable.Common.Content.Validation;

namespace Beamable.Examples.Services.ContentService
{
    /// <summary>
    /// Demonstrates a custom validation rule for a
    /// field of a <see cref="ContentObject"/> subclass.
    /// </summary>
    public class MustBeStringLength : ValidationAttribute
    {
        //  Fields  ---------------------------------------
        private const string STRING_TYPE = "Value must be a string type.";
        private const string ARGUMENT_ERROR = "The StringLengthMin of {0} must be <= StringLengthMax of {1}.";
        private const string VALUE_ERROR = "The field string length of {0} must be >= {1} and <= {2}.";
        private int _stringLengthMin = 0;
        private int _stringLengthMax = 0;

        //  Constructor Methods  --------------------------------
        
        /// <summary>
        /// Optional. Pass validation arguments.
        /// </summary>
        /// <param name="stringLengthMin"></param>
        /// <param name="stringLengthMax"></param>
        public MustBeStringLength(int stringLengthMin, int stringLengthMax)
        {
            _stringLengthMin = stringLengthMin;
            _stringLengthMax = stringLengthMax;
        }

        //  Other Methods  --------------------------------
        
        /// <summary>
        /// Performs the validation using the current field type,
        /// field value, and any validation arguments.
        ///
        /// Any thrown <see cref="ContentValidationException"/> will
        /// show helpful text in the inspector to the game maker.
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <exception cref="ContentValidationException"></exception>
        public override void Validate(ContentValidationArgs args)
        {
            ValidationFieldWrapper validationField = args.ValidationField;
            IContentObject content = args.Content;
            Type type = validationField.FieldType;
            object obj = validationField.GetValue();
            
            if (typeof(Optional).IsAssignableFrom(type))
            {
                Optional optional = obj as Optional;
                if (!optional.HasValue) return;
                type = optional.GetOptionalType();
                obj = optional.GetValue();
            }

            // Validation: Is it a string?
            if (ValidationAttribute.IsNumericType(type))
            {
                throw new ContentValidationException(content, validationField, STRING_TYPE );  
            }

            // Validation: Are the arguments correct?
            if (_stringLengthMin > _stringLengthMax)
            {
                throw new ContentValidationException(content, validationField, 
                    string.Format(ARGUMENT_ERROR, _stringLengthMin, _stringLengthMax)); 
            }

            // Validation: Is the current value correct?
            string stringValue = obj as string;
            if (stringValue == null || 
                !(stringValue.Length >= _stringLengthMin && stringValue.Length <= _stringLengthMax))
            {
                throw new ContentValidationException(content, validationField, 
                    string.Format(VALUE_ERROR, stringValue.Length, _stringLengthMin, _stringLengthMax));
            }
        }
    }
}