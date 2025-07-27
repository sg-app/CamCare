using System.ComponentModel.DataAnnotations;

namespace CamCare
{
    public enum AddressType
    {
        [Display(Description = "Rechnungsadresse")]
        Billing = 1,
        [Display(Description = "Lieferadresse")]
        Shipping = 2,
    }
}