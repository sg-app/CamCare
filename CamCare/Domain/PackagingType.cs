using System.ComponentModel.DataAnnotations;

namespace CamCare
{
    public enum PackagingType
    {
        [Display(Description = "Kundenkarton")]
        CustomersBox = 1,         // Employee uses customer's box
        [Display(Description = "Neuer Karton")]
        NewBox = 2,   // Employee use a new box
    }
}
