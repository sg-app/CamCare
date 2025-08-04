using CamCare.Domain;
using CamCare.Interfaces.Services;
using CamCare.Models;
using System.Linq;

namespace CamCare.Services
{
    public class Mapper : IMapper
    {
        public TDestination Map<TSource, TDestination>(TSource source)
            where TDestination : new()
        {
            var destination = new TDestination();
            Map(source, destination);
            return destination;
        }

        public void Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            switch (source)
            {
                case Address a when destination is AddressVm vm:
                    vm.Id = a.Id;
                    vm.CustomerId = a.CustomerId;
                    vm.AddressType = a.AddressType;
                    vm.Street = a.Street;
                    vm.HouseNumber = a.HouseNumber;
                    vm.PostalCode = a.PostalCode;
                    vm.City = a.City;
                    vm.State = a.State;
                    vm.Country = a.Country;
                    vm.CreatedAt = a.CreatedAt;
                    vm.UpdatedAt = a.UpdatedAt;
                    break;
                case AddressVm vm when destination is Address a:
                    a.Id = vm.Id;
                    a.CustomerId = vm.CustomerId;
                    a.AddressType = vm.AddressType;
                    a.Street = vm.Street;
                    a.HouseNumber = vm.HouseNumber;
                    a.PostalCode = vm.PostalCode;
                    a.City = vm.City;
                    a.State = vm.State;
                    a.Country = vm.Country;
                    break;
                case Customer c when destination is CustomerVm cvm:
                    cvm.Id = c.Id;
                    cvm.CompanyName = c.CompanyName;
                    cvm.FirstName = c.FirstName;
                    cvm.LastName = c.LastName;
                    cvm.Email = c.Email;
                    cvm.PhoneNumber = c.PhoneNumber;
                    cvm.CreatedAt = c.CreatedAt;
                    cvm.UpdatedAt = c.UpdatedAt;
                    if (c.Addresses != null)
                        cvm.Addresses = c.Addresses.Select(Map<Address, AddressVm>).ToList();
                    if (c.Cameras != null)
                        cvm.Cameras = c.Cameras.Select(Map<Camera, CameraVm>).ToList();
                    break;
                case CustomerVm cvm when destination is Customer c:
                    c.Id = cvm.Id;
                    c.CompanyName = cvm.CompanyName;
                    c.FirstName = cvm.FirstName;
                    c.LastName = cvm.LastName;
                    c.Email = cvm.Email;
                    c.PhoneNumber = cvm.PhoneNumber;
                    if (cvm.Addresses != null)
                        c.Addresses = cvm.Addresses.Select(Map<AddressVm, Address>).ToList();
                    if (cvm.Cameras != null)
                        c.Cameras = cvm.Cameras.Select(Map<CameraVm, Camera>).ToList();
                    break;
                case Camera cam when destination is CameraVm camVm:
                    camVm.SerialNumber = cam.SerialNumber;
                    camVm.CustomerId = cam.CustomerId;
                    camVm.CameraTypeId = cam.CameraTypeId;
                    camVm.CreatedAt = cam.CreatedAt;
                    camVm.UpdatedAt = cam.UpdatedAt;
                    if (cam.CameraType != null)
                        camVm.CameraType = Map<CameraType, CameraTypeVm>(cam.CameraType);
                    break;
                case CameraVm camVm when destination is Camera cam:
                    cam.SerialNumber = camVm.SerialNumber;
                    cam.CustomerId = camVm.CustomerId;
                    cam.CameraTypeId = camVm.CameraTypeId;
                    cam.CreatedAt = camVm.CreatedAt;
                    cam.UpdatedAt = camVm.UpdatedAt;
                    if (camVm.RepairOrders != null)
                        cam.RepairOrders = camVm.RepairOrders.Select(Map<RepairOrderVm, RepairOrder>).ToList();
                    break;
                case RepairOrder ro when destination is RepairOrderVm rovm:
                    rovm.Id = ro.Id;
                    rovm.CameraSerialNumber = ro.CameraSerialNumber;
                    rovm.CustomerId = ro.CustomerId;
                    rovm.LogisticProviderId = ro.LogisticProviderId;
                    rovm.RepairOrderStatusId = ro.RepairOrderStatusId;
                    rovm.ShippingMethod = ro.ShippingMethod;
                    rovm.ArrivedAt = ro.ArrivedAt;
                    rovm.CreatedAt = ro.CreatedAt;
                    rovm.UpdatedAt = ro.UpdatedAt;
                    if (ro.Customer != null)
                        rovm.Customer = Map<Customer, CustomerVm>(ro.Customer);
                    if (ro.Camera != null)
                        rovm.Camera = Map<Camera, CameraVm>(ro.Camera);
                    if (ro.RepairOrderStatus != null)
                        rovm.RepairOrderStatus = Map<RepairOrderStatus, RepairOrderStatusVm>(ro.RepairOrderStatus);
                    if (ro.LogisticProvider != null)
                        rovm.LogisticProvider = Map<LogisticProvider, LogisticProviderVm>(ro.LogisticProvider);
                    if (ro.Defectives != null)
                        rovm.Defectives = ro.Defectives.Select(Map<Defective, DefectiveVm>).ToList();
                    if (ro.RepairPositions != null)
                        rovm.RepairPositions = ro.RepairOrderRepairPositions.Select(Map<RepairOrderRepairPosition, RepairPositionVm>).ToList();
                    break;
                case RepairOrderVm rovm when destination is RepairOrder ro:
                    ro.Id = rovm.Id;
                    ro.CustomerId = rovm.CustomerId;
                    ro.CameraSerialNumber = rovm.CameraSerialNumber;
                    ro.RepairOrderStatusId = rovm.RepairOrderStatusId;
                    ro.ShippingMethod = rovm.ShippingMethod;
                    ro.LogisticProviderId = rovm.LogisticProviderId;
                    ro.ArrivedAt = rovm.ArrivedAt;
                    ro.CreatedAt = rovm.CreatedAt;
                    ro.UpdatedAt = rovm.UpdatedAt;
                    //if (rovm.Camera != null)
                    //    ro.Camera = Map<CameraVm, Camera>(rovm.Camera);
                    //if (rovm.RepairPositions != null)
                    //    ro.RepairPositions = rovm.RepairPositions.Select(Map<RepairPositionVm, RepairPosition>).ToList();
                    //if (rovm.Defectives != null)
                    //    ro.Defectives = rovm.Defectives.Select(Map<DefectiveVm, Defective>).ToList();
                    break;
                case RepairOrderRepairPosition rorp when destination is RepairPositionVm rpvm:
                    rpvm.Id = rorp.RepairPostionId;
                    rpvm.Artikelnummer = rorp.RepairPosition.Artikelnummer;
                    rpvm.Description = rorp.RepairPosition.Description;
                    rpvm.SortOrder = rorp.RepairPosition.SortOrder;
                    rpvm.Quantity = rorp.Quantity;
                    rpvm.CreatedAt = rorp.RepairPosition.CreatedAt;
                    rpvm.UpdatedAt = rorp.RepairPosition.UpdatedAt;
                    break;
                case RepairPositionVm rpvm when destination is RepairPosition rp:
                    rp.Id = rpvm.Id;
                    rp.Description = rpvm.Description;
                    rp.SortOrder = rpvm.SortOrder;
                    rp.CreatedAt = rpvm.CreatedAt;
                    rp.UpdatedAt = rpvm.UpdatedAt;
                    if (rpvm.RepairOrders != null)
                        rp.RepairOrders = rpvm.RepairOrders.Select(Map<RepairOrderVm, RepairOrder>).ToList();
                    break;
                case RepairPosition rp when destination is RepairPositionVm rpvm:
                    rpvm.Id = rp.Id;
                    rpvm.Description = rp.Description;
                    rpvm.SortOrder = rp.SortOrder;
                    rpvm.CreatedAt = rp.CreatedAt;
                    rpvm.UpdatedAt = rp.UpdatedAt;
                    if (rp.RepairOrders != null)
                        rpvm.RepairOrders = rp.RepairOrders.Select(Map<RepairOrder, RepairOrderVm>).ToList();
                    break;
                case RepairPositionVm rpvm when destination is RepairPosition rp:
                    rp.Id = rpvm.Id;
                    rp.Description = rpvm.Description;
                    rp.SortOrder = rpvm.SortOrder;
                    rp.CreatedAt = rpvm.CreatedAt;
                    rp.UpdatedAt = rpvm.UpdatedAt;
                    if (rpvm.RepairOrders != null)
                        rp.RepairOrders = rpvm.RepairOrders.Select(Map<RepairOrderVm, RepairOrder>).ToList();
                    break;
                case Defective def when destination is DefectiveVm defvm:
                    defvm.Id = def.Id;
                    defvm.Description = def.Description;
                    defvm.CreatedAt = def.CreatedAt;
                    defvm.UpdatedAt = def.UpdatedAt;
                    break;
                case DefectiveVm defvm when destination is Defective def:
                    def.Id = defvm.Id;
                    def.Description = defvm.Description;
                    break;
                case CameraType ct when destination is CameraTypeVm ctv:
                    ctv.Id = ct.Id;
                    ctv.Name = ct.Name;
                    break;
                case CameraTypeVm ctv when destination is CameraType ct:
                    ct.Id = ctv.Id;
                    ct.Name = ctv.Name;
                    break;
                case RepairOrderStatus ros when destination is RepairOrderStatusVm rosvm:
                    rosvm.Id = ros.Id;
                    rosvm.Name = ros.Name;
                    rosvm.Description = ros.Description;
                    rosvm.Order = ros.Order;
                    rosvm.BackgroundColor = ros.BackgroundColor;
                    rosvm.FontColor = ros.FontColor;
                    rosvm.IsActive = ros.IsActive;
                    rosvm.IsDefault = ros.IsDefault;
                    rosvm.IsOrderClose = ros.IsOrderClose;
                    rosvm.CreatedAt = ros.CreatedAt;
                    rosvm.UpdatedAt = ros.UpdatedAt;
                    break;
                case RepairOrderStatusVm rosvm when destination is RepairOrderStatus ros:
                    ros.Id = rosvm.Id;
                    ros.Name = rosvm.Name;
                    ros.Description = rosvm.Description;
                    ros.Order = rosvm.Order;
                    ros.BackgroundColor = rosvm.BackgroundColor;
                    ros.FontColor = rosvm.FontColor;
                    ros.IsActive = rosvm.IsActive;
                    ros.IsDefault = rosvm.IsDefault;
                    ros.IsOrderClose = rosvm.IsOrderClose;
                    ros.CreatedAt = rosvm.CreatedAt;
                    ros.UpdatedAt = rosvm.UpdatedAt;
                    break;
                case LogisticProvider lp when destination is LogisticProviderVm lpvm:
                    lpvm.Id = lp.Id;
                    lpvm.Name = lp.Name;
                    lpvm.IsDefault = lp.IsDefault;
                    lpvm.IsActive = lp.IsActive;
                    lpvm.CreatedAt = lp.CreatedAt;
                    lpvm.UpdatedAt = lp.UpdatedAt;
                    break;
                case LogisticProviderVm lpvm when destination is LogisticProvider lp:
                    lp.Id = lpvm.Id;
                    lp.Name = lpvm.Name;
                    lp.IsDefault = lpvm.IsDefault;
                    lp.IsActive = lpvm.IsActive;
                    break;

                default:
                    throw new NotSupportedException($"Mapping from {typeof(TSource)} to {typeof(TDestination)} is not supported.");
            }
        }
    }
}
