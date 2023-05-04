using System;
using Beamable.Common.Content;
using Beamable.Common.Content.Validation;
using Beamable.Common.Inventory;

namespace Beamable.Examples.Services.ContentService
{
    [Serializable]
    public class ComplexItemLink : ContentLink<ComplexItem> {}

    /// <summary>
    /// This demonstrates validation rules
    /// for use with any fields within a <<see cref="ContentObject"/>
    /// subclass.
    ///
    /// Using validation is optional.
    ///
    /// See "Beamable.Common.Content.Validation" for full list.
    ///
    /// The content type for this class will be "items.complex_item", nested
    /// under the "items" content type.
    /// </summary>
    [ContentType("complex_item")]
    public class ComplexItem : ItemContent
    {
        /// <summary>
        /// Built-in: Validation requires that the value be NOT blank.
        /// </summary>
        [CannotBeBlank]
        public string Name = "";

        /// <summary>
        /// Custom: Validation requires that the value be string and of
        /// string length of 2 or 3.
        /// See <see cref="MustBeStringLength"/>.
        /// </summary>
        [MustBeStringLength (2, 3)]
        public string FavoriteLetters = "";

        /// <summary>
        /// Built-in: Validation requires that the value be positive and
        /// non-zero.
        /// </summary>
        [MustBePositive(false)]
        public int Defense = 0;

        /// <summary>
        /// Built-In: Here is an optional value with no validation.
        /// </summary>
        public OptionalInt Health;
    }
}
