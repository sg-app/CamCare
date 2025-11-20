using System.ComponentModel.DataAnnotations;

namespace CamCare
{
    public enum ShippingMethod
    {
        [Display(Description = "Kamera wurde abgeholt")]
        PickedUp = 1,         // Camera was picked up
        [Display(Description = "Kamera wurde von Kunde selbst versendet")]
        SentByCustomer = 2,   // Camera was sent by customer
        [Display(Description = "Kamera wurde vorbei gebracht")]
        BroughtByCustomer = 3    // Camera was brought by customer
    }
}
