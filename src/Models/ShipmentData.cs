
public class ShipmentData
{
    public Embedded? _embedded { get; set; }
    public Links _links { get; set; }
    public Page page { get; set; }
}

public class Address
{
    public string address1 { get; set; }
    public string address2 { get; set; }
    public string address3 { get; set; }
    public string address4 { get; set; }
    public string addressType { get; set; }
    public string cityOrTown { get; set; }
    public string countryCode { get; set; }
    public bool isValidated { get; set; }
    public string postalOrZipCode { get; set; }
    public string stateOrProvince { get; set; }
}

public class AuditInfo
{
    public DateTime updateDate { get; set; }
    public DateTime createDate { get; set; }
    public string updateBy { get; set; }
    public string createBy { get; set; }
}

public class Backorder
{
    public string href { get; set; }
    public string method { get; set; }
}

public class BackorderItems
{
    public string href { get; set; }
    public string method { get; set; }
}

public class Cancel
{
    public string href { get; set; }
    public string method { get; set; }
}

public class CancelItems
{
    public string href { get; set; }
    public string method { get; set; }
}

public class ChangeMessage
{
    public string changeMessageId { get; set; }
    public string appId { get; set; }
    public string appKey { get; set; }
    public DateTime createdDate { get; set; }
    public string correlationId { get; set; }
    public string message { get; set; }
    public string oldValue { get; set; }
    public object newValue { get; set; }
    public string subject { get; set; }
    public string userId { get; set; }
}

public class CurrentState
{
    public string name { get; set; }
    public string oldValue { get; set; }
    public string value { get; set; }
    public DateTime updateDate { get; set; }
}

public class Customer
{
    public CustomerContact customerContact { get; set; }
}

public class CustomerContact
{
    public int id { get; set; }
    public Address address { get; set; }
    public string companyOrOrganization { get; set; }
    public string email { get; set; }
    public string firstName { get; set; }
    public string lastNameOrSurname { get; set; }
    public string middleNameOrInitial { get; set; }
    public string shortFullName { get; set; }
    public string fullName { get; set; }
    public PhoneNumbers phoneNumbers { get; set; }
}

public class Data
{
    public InventoryInfo inventoryInfo { get; set; }
    public DateTime createTime { get; set; }
    public List<int> quantitySelector { get; set; }
    public bool loggedInUser { get; set; }
    public int pointsToBeEarned { get; set; }
}

public class Destination
{
    public DestinationContact destinationContact { get; set; }
    public bool isDestinationCommercial { get; set; }
}

public class DestinationContact
{
    public int id { get; set; }
    public Address address { get; set; }
    public string companyOrOrganization { get; set; }
    public string email { get; set; }
    public string firstName { get; set; }
    public string lastNameOrSurname { get; set; }
    public string middleNameOrInitial { get; set; }
    public string shortFullName { get; set; }
    public string fullName { get; set; }
    public PhoneNumbers phoneNumbers { get; set; }
}

public class Embedded
{
    public List<KiboShipment> shipments { get; set; }
}

public class Fulfill
{
    public string href { get; set; }
    public string method { get; set; }
}

public class Initiator
{
    public string name { get; set; }
    public string oldValue { get; set; }
    public string value { get; set; }
    public DateTime updateDate { get; set; }
}

public class Input
{
    public string name { get; set; }
    public string type { get; set; }
}

public class InventoryInfo
{
    public bool manageStock { get; set; }
    public string outOfStockBehavior { get; set; }
    public int onlineStockAvailable { get; set; }
    public int onlineSoftStockAvailable { get; set; }
    public string onlineLocationCode { get; set; }
    public bool isSubstitutable { get; set; }
}

public class Item
{
    public int lineId { get; set; }
    public string originalOrderItemId { get; set; }
    public string goodsType { get; set; }
    public string productCode { get; set; }
    public int quantity { get; set; }
    public int transferQuantity { get; set; }
    public string imageUrl { get; set; }
    public string name { get; set; }
    public string upc { get; set; }
    public bool allowsBackOrder { get; set; }
    public double unitPrice { get; set; }
    public bool isTaxable { get; set; }
    public double actualPrice { get; set; }
    public double itemDiscount { get; set; }
    public double lineItemCost { get; set; }
    public double itemTax { get; set; }
    public double shipping { get; set; }
    public double shippingDiscount { get; set; }
    public double shippingTax { get; set; }
    public double handling { get; set; }
    public double handlingDiscount { get; set; }
    public double handlingTax { get; set; }
    public double duty { get; set; }
    public double weightedShipmentAdjustment { get; set; }
    public double weightedLineItemTaxAdjustment { get; set; }
    public double weightedShippingAdjustment { get; set; }
    public double weightedShippingTaxAdjustment { get; set; }
    public double weightedHandlingAdjustment { get; set; }
    public double weightedHandlingTaxAdjustment { get; set; }
    public double weightedDutyAdjustment { get; set; }
    public double taxableShipping { get; set; }
    public double taxableLineItemCost { get; set; }
    public double taxableHandling { get; set; }
    public double weight { get; set; }
    public double length { get; set; }
    public double width { get; set; }
    public double height { get; set; }
    public string weightUnit { get; set; }
    public Data data { get; set; }
    public AuditInfo auditInfo { get; set; }
    public List<object> options { get; set; }
    public bool manageStock { get; set; }
    public double creditValue { get; set; }
    public bool isAssemblyRequired { get; set; }
    public bool isPackagedStandAlone { get; set; }
    public bool allowsFutureAllocate { get; set; }
    public bool isReservedInventory { get; set; }
    public bool allowsSubstitution { get; set; }
    public int originalQuantity { get; set; }
}

public class Links
{
    public Self self { get; set; }
    public ShipmentsLink shipmentslink { get; set; }
    public Tasks tasks { get; set; }
    public WorkflowInstanceImage workflowInstanceImage { get; set; }
    public Backorder backorder { get; set; }
    public BackorderItems backorderItems { get; set; }
    public Reject reject { get; set; }
    public RejectItems rejectItems { get; set; }
    public Cancel cancel { get; set; }
    public Fulfill fulfill { get; set; }
    public Reassign reassign { get; set; }
    public ReassignItems reassignItems { get; set; }
    public CancelItems cancelItems { get; set; }
}

public class Package
{
    public string packageId { get; set; }
    public string packagingType { get; set; }
    public string shippingMethodCode { get; set; }
    public List<string>? trackingNumbers { get; set; }
    public List<object> returnTrackingNumbers { get; set; }
    public List<Tracking> trackings { get; set; }
    public bool hasLabel { get; set; }
    public List<object> packingSlipItemDetails { get; set; }
    public AuditInfo auditInfo { get; set; }
}

public class Page
{
    public int size { get; set; }
    public int totalElements { get; set; }
    public int totalPages { get; set; }
    public int number { get; set; }
}

public class PhoneNumbers
{
    public string home { get; set; }
    public string mobile { get; set; }
    public string work { get; set; }
}

public class Reassign
{
    public string href { get; set; }
    public string method { get; set; }
}

public class ReassignItems
{
    public string href { get; set; }
    public string method { get; set; }
}

public class Reject
{
    public string href { get; set; }
    public string method { get; set; }
}

public class RejectItems
{
    public string href { get; set; }
    public string method { get; set; }
}

public class Root
{
    public Embedded _embedded { get; set; }
    public Links _links { get; set; }
    public Page page { get; set; }
}

public class Self
{
    public string href { get; set; }
}

public class ShipmentsLink
{
    public string href { get; set; }
}


public class KiboShipment
{
    public int shipmentNumber { get; set; }
    public string orderId { get; set; }
    public int orderNumber { get; set; }
    public DateTime orderSubmitDate { get; set; }
    public int customerAccountId { get; set; }
    public int tenantId { get; set; }
    public int siteId { get; set; }
    public DateTime fulfillmentDate { get; set; }
    public string shipmentType { get; set; }
    public string shipmentStatus { get; set; }
    public string fulfillmentStatus { get; set; }
    public string fulfillmentLocationCode { get; set; }
    public string assignedLocationCode { get; set; }
    public string workflowProcessId { get; set; }
    public string workflowProcessContainerId { get; set; }
    public double shipmentAdjustment { get; set; }
    public double lineItemSubtotal { get; set; }
    public double lineItemTaxAdjustment { get; set; }
    public double lineItemTaxTotal { get; set; }
    public double lineItemTotal { get; set; }
    public double shippingAdjustment { get; set; }
    public double shippingSubtotal { get; set; }
    public double shippingTaxAdjustment { get; set; }
    public double shippingTaxTotal { get; set; }
    public double shippingTotal { get; set; }
    public double handlingAdjustment { get; set; }
    public double handlingSubtotal { get; set; }
    public double handlingTaxAdjustment { get; set; }
    public double handlingTaxTotal { get; set; }
    public double handlingTotal { get; set; }
    public double dutyAdjustment { get; set; }
    public double dutyTotal { get; set; }
    public double total { get; set; }
    public string currencyCode { get; set; }
    public Destination destination { get; set; }
    public Customer customer { get; set; }
    public string shippingMethodCode { get; set; }
    public string shippingMethodName { get; set; }
    public List<Item>? items { get; set; }
    public List<object> canceledItems { get; set; }
    public List<object> reassignedItems { get; set; }
    public List<object> rejectedItems { get; set; }
    public List<object> transferredItems { get; set; }
    public List<Package> packages { get; set; }
    public WorkflowState workflowState { get; set; }
    public List<ChangeMessage> changeMessages { get; set; }
    public Data data { get; set; }
    public string email { get; set; }
    public bool isExpress { get; set; }
    public AuditInfo auditInfo { get; set; }
    public bool readyToCapture { get; set; }
    public bool isOptInForSms { get; set; }
    public ShopperNotes shopperNotes { get; set; }
    public List<object> shipmentNotes { get; set; }
    public bool isAutoAssigned { get; set; }
    public bool isHistoricalImport { get; set; }
    public List<object> substitutedItems { get; set; }
    public string orderType { get; set; }
    public string workflowProcessVersion { get; set; }
    public bool isFlatRateShipping { get; set; }
    public Links _links { get; set; }
    public string href { get; set; }
}

public class ShopperNotes
{
}

public class TaskList
{
    public string name { get; set; }
    public string subject { get; set; }
    public List<Input> inputs { get; set; }
    public bool active { get; set; }
    public bool completed { get; set; }
    public DateTime completedDate { get; set; }
    public Links _links { get; set; }
}

public class Tasks
{
    public string href { get; set; }
}

public class Tracking
{
    public string number { get; set; }
}

public class Variables
{
    public Initiator initiator { get; set; }
    public CurrentState currentState { get; set; }
}

public class WorkflowInstanceImage
{
    public string href { get; set; }
}

public class WorkflowState
{
    public string shipmentState { get; set; }
    public List<TaskList> taskList { get; set; }
    public string processInstanceId { get; set; }
    public AuditInfo auditInfo { get; set; }
    public Variables variables { get; set; }
}

