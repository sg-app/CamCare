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
                    a.CreatedAt = vm.CreatedAt;
                    a.UpdatedAt = vm.UpdatedAt;
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
                    c.CreatedAt = cvm.CreatedAt;
                    c.UpdatedAt = cvm.UpdatedAt;
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
                    if (cam.RepairOrders != null)
                        camVm.RepairOrders = cam.RepairOrders.Select(Map<RepairOrder, RepairOrderVm>).ToList();
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
                    rovm.RepairOrderStatusId = ro.RepairOrderStatusId;
                    rovm.ArrivedAt = ro.ArrivedAt;
                    rovm.CreatedAt = ro.CreatedAt;
                    rovm.UpdatedAt = ro.UpdatedAt;
                    if (ro.Camera != null)
                        rovm.Camera = Map<Camera, CameraVm>(ro.Camera);
                    if (ro.RepairOrderStatus != null)
                        rovm.RepairOrderStatus = Map<RepairOrderStatus, RepairOrderStatusVm>(ro.RepairOrderStatus);
                    if (ro.RepairPositions != null)
                        rovm.RepairPositions = ro.RepairPositions.Select(Map<RepairPosition, RepairPositionVm>).ToList();
                    if (ro.Defectives != null)
                        rovm.Defectives = ro.Defectives.Select(Map<Defective, DefectiveVm>).ToList();
                    break;
                case RepairOrderVm rovm when destination is RepairOrder ro:
                    ro.Id = rovm.Id;
                    ro.CameraSerialNumber = rovm.CameraSerialNumber;
                    ro.RepairOrderStatusId = rovm.RepairOrderStatusId;
                    ro.ArrivedAt = rovm.ArrivedAt;
                    ro.CreatedAt = rovm.CreatedAt;
                    ro.UpdatedAt = rovm.UpdatedAt;
                    if (rovm.Camera != null)
                        ro.Camera = Map<CameraVm, Camera>(rovm.Camera);
                    if (rovm.RepairPositions != null)
                        ro.RepairPositions = rovm.RepairPositions.Select(Map<RepairPositionVm, RepairPosition>).ToList();
                    if (rovm.Defectives != null)
                        ro.Defectives = rovm.Defectives.Select(Map<DefectiveVm, Defective>).ToList();
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
                case CameraType ct when destination is CameraTypeVm ctv:
                    ctv.Id = ct.Id;
                    ctv.Name = ct.Name;
                    break;
                case CameraTypeVm ctv when destination is CameraType ct:
                    ct.Id = ctv.Id;
                    ct.Name = ctv.Name;
                    break;
                default:
                    throw new NotSupportedException($"Mapping from {typeof(TSource)} to {typeof(TDestination)} is not supported.");
            }
        }
    }
}
