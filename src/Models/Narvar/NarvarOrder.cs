using KiboWebhookListener.Models;

namespace KiboWebhookListener.src.Models.Narvar
{
    public class NarvarOrder
    {
        public OrderInfo order_info { get; set; }
    }
    
    public class OrderInfo
    {
        public string order_number { get; set; }
        public string order_date { get; set; }
        public string currency_code { get; set; }
        public List<OrderItem> order_items { get; set; }
        public List<NarvarShipment> shipments { get; set; }
        
        public OrderAttributes attributes { get; set; }
        
        public Billing billing { get; set; }
        public ContactWithAddress customer { get; set; }
    }
    
    public class ContactWithAddressAndId : ContactWithAddress
	{
	    public string customer_id { get; set; }
	}

    public class Billing
    {
	    public ContactWithAddress billed_to { get; set; }
    }
    
    public class ContactWithAddress
	{
		public string first_name { get; set; }
		public string last_name { get; set; }
		public string phone { get; set; }
		public string email { get; set; }
		public Address address { get; set; }
		
	}

	public class Customer
	{
		
	}

    public class OrderAttributes
    {
	    // any key/value
	    // order_id
		// logged_in
		// points_to_be_earned
		public string order_id { get; set; }
		public string logged_in { get; set; }
		public string points_to_be_earned { get; set; }
	    
    }
    
    public class OrderItem
    {
        public object item_id { get; set; }
        public string sku { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public decimal quantity { get; set; }
        public double unit_price { get; set; }
        public List<object> categories { get; set; }
        public string item_image { get; set; }
        public string item_url { get; set; }
        public bool is_final_sale { get; set; }
        public string fulfillment_status { get; set; }
        public bool is_gift { get; set; }
        public Attributes attributes { get; set; }
        public string color { get; set; }
        public object events { get; set; }
    }
    
    public class Attributes
    {
    }
    
    public class NarvarShipment
    {
        public string? ship_source { get; set; }
        public List<ItemsInfo> items_info { get; set; }
        public string carrier { get; set; }
        public ShippedTo shipped_to { get; set; }
        public double ship_discount { get; set; }
        public double ship_total { get; set; }
        public double ship_tax { get; set; }
        public DateTime ship_date { get; set; }
        public string tracking_number { get; set; }
        public string ship_method { get; set; }
    }
    
    public class ItemsInfo
    {
	    public string item_id { get; set; }
        public int quantity { get; set; }
        public string sku { get; set; }
    }
    
    public class ShippedTo
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public Address address { get; set; }
    }
    
    public class Address
    {
        public string street_1 { get; set; }
        public string street_2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
    }
}
