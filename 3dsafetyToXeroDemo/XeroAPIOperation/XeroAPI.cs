using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xero.Api.Core;
using Xero.Api.Core.Model;
using Xero.Api.Core.Model.Types;

namespace _3dsafetyToXeroDemo.XeroAPIOperation
{
    internal class XeroAPI
    {
        private readonly XeroCoreApi _api;

        public XeroAPI(XeroCoreApi api)
        {
            _api = api;
        }
//===================== Contact Operation ================

        // Create Contact in Xero
        public string ContactCreate(Contact contact)
        {
            return GenerateContactId(contact);
        }
        //Update Contact
        public bool ContactUpdate(string contactId, Address userAddress)
        {
            return UpdateContactAddress(contactId, userAddress);
        }

        // Get contact by Id
        public bool ContactSearchById(string contactId)
        {
            return IsContactExist(contactId);
        }


//===================== Invoice Operation ================

        //Create Invoice for existed Contact
        public string InvoiceCreate(Invoice invoice,string contactId)
        {
            invoice.Contact = GetContactById(contactId);
            // Could get distince item codes from LineItems if the invoice has multi line items.
            foreach(LineItem item in invoice.LineItems)
            {
                string itemCode = item.ItemCode;

                if(!IsItemCodeExist(itemCode))
                {
                    ItemCreate(itemCode);
                }
            }
            return GenerateInvoiceId(invoice);
        }
        //Get invoice by ID
        public string GetInvoice(string invoiceId)
        {
            Invoice existedInvoice = new Invoice();
            // Guid newGuid = new Guid();
            existedInvoice = _api.Invoices.Find(new Guid(invoiceId));
            return existedInvoice.Id.ToString();
        }
      

//===================== Payment Operation ================
        //get payment 
        public bool PaymentGet(string contactid)
        {
            List<Payment> paymentList = new List<Payment>();
            paymentList = _api.Payments.Find().ToList();
            return true;
        }
        // Make payment against the invoice
        public void InvoicePay(string invoiceId, decimal paidAmount)
        {
            Payment payment = new Payment();
            Invoice payingInvoice = _api.Invoices.Find(new Guid(invoiceId));
            payment.Invoice = payingInvoice;
            Account payingAccount = new Account();
            payingAccount.Code = "880";
            payment.Account = payingAccount;
            payment.Amount = paidAmount;
            payment.Date = DateTime.Now;
            _api.Payments.Create(payment);
        }

        //Check contact is existed in Xero
        private bool IsContactExist(string contactId)
        {
            bool isExistedContact = true;
            Contact result = new Contact();

            try
            {
                result = _api.Contacts.Find(new Guid(contactId));
                isExistedContact = !string.IsNullOrEmpty(result.Name);
            }
            catch (Exception ex)
            {
                isExistedContact = false;
                //throw ex;
            }
            return isExistedContact;
        }
        // Searching existed contact in Xero by Id
        private Contact GetContactById(string contactId)
        {
            Contact result = new Contact();
            try
            {
                result = _api.Contacts.Find(new Guid(contactId));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        // Generate Contact Id
        private string GenerateContactId(Contact c)
        {
            Contact result = new Contact();

            try
            {
                result = _api.Create(c);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result.Id.ToString();
        }
        // Update Contact by Id
        private bool UpdateContactAddress(string contactId,Address address)
        {
            bool isUpdated = true;
            Contact result = new Contact();
            try
            {
                result = _api.Contacts.Find(new Guid(contactId));
                result.Addresses.Add(address);
                _api.Contacts.Update(result);
            }
            catch (Exception ex)
            {
                isUpdated = false;
               // throw ex;
            }
            return isUpdated;
        }
        // Generate Invoice Id
        private string GenerateInvoiceId(Invoice i)
        {
            Invoice result = new Invoice();

            try
            {
                result = _api.Create(i);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result.Id.ToString();
        }
        //Check Invoice Item is existed in Xero
        private bool IsItemCodeExist(string itemCode)
        {
            Item result = new Item();
            try
            {
                result = _api.Items.Find(itemCode);
                return !string.IsNullOrEmpty(result.Code);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        // Create item
        private void ItemCreate(string itemCode)
        {
            GenerateItemCode(itemCode);
        }
        // Generate Item code in Xero
        private string GenerateItemCode(string itemCode)
        {
            Item item = new Item();

            try
            {
                item.Code = itemCode;
                item.Description = itemCode + " description";
                item = _api.Create(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return item.Code;
        }
    }
}
